#include <pico/stdlib.h>
#include <cstring>
#include <pio_ads7049.h>
#include <config.h>
#include <harp_message.h>
#include <harp_core.h>
#include <harp_c_app.h>
#include <harp_synchronizer.h>
#ifdef DEBUG
    #include <cstdio> // for printf
#endif

#define SAMPLE_INTERVAL_US (10'000) // 100 Hz
#define DISCONNECT_CHECK_INTERVAL_US (2'000'000) // 0.5 Hz


// Create PIO SPI ADC instance for thermistor sensing.
PIO_ADS7049 thermistor(pio0, ADC_CS_PIN, ADC_SCK_PIN, ADC_POCI_PIN);

// Create device name array.
const uint16_t who_am_i = HARP_DEVICE_ID;
const uint8_t hw_version_major = 0;
const uint8_t hw_version_minor = 0;
const uint8_t assembly_version = 0;
const uint8_t harp_version_major = 0;
const uint8_t harp_version_minor = 0;
const uint8_t fw_version_major = 0;
const uint8_t fw_version_minor = 0;
const uint16_t serial_number = 0;

// Setup for Harp App
const size_t reg_count = 2;

uint32_t __not_in_flash("dispatch_interval_us") dispatch_interval_us;
uint32_t __not_in_flash("next_msg_dispatch_time_us") next_msg_dispatch_time_us;

#pragma pack(push, 1)
struct app_regs_t
{
    volatile uint16_t voltage_raw;          // 32.
    volatile uint16_t voltage_dispatch_frequency_hz;   // 33
    // More app "registers" here.
};
#pragma pack(pop)
app_regs_t __not_in_flash("app_regs") app_regs; // put app regs in ram.

// Define "specs" per-register
RegSpecs app_reg_specs[reg_count]
{
    {(uint8_t*)&app_regs.voltage_raw, sizeof(app_regs.voltage_raw), U16},
    {(uint8_t*)&app_regs.voltage_dispatch_frequency_hz, sizeof(app_regs.voltage_dispatch_frequency_hz), U16}
};

void write_voltage_dispatch_frequency_hz(msg_t& msg)
{
    HarpCore::copy_msg_payload_to_register(msg);
    // Cap maximum value.
    if (app_regs.voltage_dispatch_frequency_hz > MAX_EVENT_FREQUENCY_HZ)
    {
        // Update register and dependedent values.
        app_regs.voltage_dispatch_frequency_hz = MAX_EVENT_FREQUENCY_HZ;
        dispatch_interval_us = 1000; // precomputed
        next_msg_dispatch_time_us = time_us_32(); // reset interval.
        HarpCore::send_harp_reply(WRITE_ERROR, msg.header.address);
        return;
    }
    dispatch_interval_us = div_u32u32(1'000'000,
                                      app_regs.voltage_dispatch_frequency_hz);
    next_msg_dispatch_time_us = time_us_32(); // reset interval.
    HarpCore::send_harp_reply(WRITE, msg.header.address);
}

RegFnPair reg_handler_fns[reg_count]
{
    {HarpCore::read_reg_generic, HarpCore::write_to_read_only_reg_error},
    {HarpCore::read_reg_generic, write_voltage_dispatch_frequency_hz}
    // More handler function pairs here if we add additional registers.
};

void update_app_state()
{
    // Update periodic tasks.
    uint32_t curr_time_us = time_us_32();
    // Dispatch voltage msg at specified frequency if specified to do so.
    if (HarpCore::is_muted() || (app_regs.voltage_dispatch_frequency_hz == 0))
        return;
    if (int32_t(curr_time_us - next_msg_dispatch_time_us) >= dispatch_interval_us)
    {
        next_msg_dispatch_time_us += dispatch_interval_us;
        const RegSpecs& reg_specs = app_reg_specs[0]; // voltage_raw reg.
        HarpCore::send_harp_reply(EVENT, APP_REG_START_ADDRESS,
                                  reg_specs.base_ptr, reg_specs.num_bytes,
                                  reg_specs.payload_type);
    }
}

void reset_app()
{
    // Put app variables in starting state.
    next_msg_dispatch_time_us = time_us_32();
    app_regs.voltage_dispatch_frequency_hz = DEFAULT_DISPATCH_FREQUENCY_HZ;
}

// Create Core.
HarpCApp& app = HarpCApp::init(who_am_i, hw_version_major, hw_version_minor,
                               assembly_version,
                               harp_version_major, harp_version_minor,
                               fw_version_major, fw_version_minor,
                               serial_number, "Sniff Detector",
                               &app_regs, app_reg_specs,
                               reg_handler_fns, reg_count, update_app_state,
                               reset_app);

// Core0 main.
int main()
{
#ifdef DEBUG
    stdio_uart_init_full(uart0, 921600, UART_TX_PIN, -1); // use uart1 tx only.
    printf("Hello, from an RP2040!\r\n");
#endif
    // Init Synchronizer.
    HarpSynchronizer::init(uart1, HARP_SYNC_RX_PIN);
    app.set_synchronizer(&HarpSynchronizer::instance());
    reset_app();
    //app.set_visual_indicators_fn(set_led_state);
    // Setup continuous writing of the latest ADC value to the corresponding
    // Harp register.
    thermistor.setup_dma_stream_to_memory(&app_regs.voltage_raw, 1);
    // Start PIO-connected hardware.
    thermistor.start();
    reset_app();
    while(true)
        app.run();
}
