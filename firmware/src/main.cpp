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

#pragma pack(push, 1)
struct app_regs_t
{
    volatile uint16_t voltage_raw;          // 32
    volatile uint16_t event_frequency_hz;   // 33
    // More app "registers" here.
} app_regs;

// Define "specs" per-register
RegSpecs app_reg_specs[reg_count]
{
    {(uint8_t*)&app_regs.voltage_raw, sizeof(app_regs.voltage_raw), U16},
    {(uint8_t*)&app_regs.event_frequency_hz, sizeof(app_regs.event_frequency_hz), U16}
    // More specs here if we add additional registers.
};

void write_event_frequency_hz(msg_t& msg)
{
    const uint16_t& event_frequency_hz = *((uint16_t*)msg.payload);
    if (event_frequency_hz > MAX_EVENT_FREQUENCY_HZ)
    {
        app_regs.event_frequency_hz = MAX_EVENT_FREQUENCY_HZ;
        HarpCore::send_harp_reply(WRITE_ERROR, msg.header.address);
    }
    else
        HarpCore::write_reg_generic(msg);
}

RegFnPair reg_handler_fns[reg_count]
{
    {HarpCore::read_reg_generic, HarpCore::write_to_read_only_reg_error},
    {HarpCore::read_reg_generic, write_event_frequency_hz}
    // More handler function pairs here if we add additional registers.
};

void update_app_state()
{
    if (HarpCore::is_muted())
        return;
    // Dispatch message at specified frequency.
    static uint32_t last_msg_time_us = time_us_32(); // init this value once.
    uint32_t curr_time_us = time_us_32();
    if ((curr_time_us - last_msg_time_us) >= 1'000'000/app_regs.event_frequency_hz)
    {
        last_msg_time_us = curr_time_us;
        const RegSpecs& reg_specs = app_reg_specs[0];
        HarpCore::send_harp_reply(EVENT, APP_REG_START_ADDRESS,
                                  reg_specs.base_ptr, reg_specs.num_bytes,
                                  reg_specs.payload_type);
    }
}

void reset_app()
{
    app_regs.event_frequency_hz = MAX_EVENT_FREQUENCY_HZ;
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
    stdio_uart_init_full(uart1, 921600, UART_TX_PIN, -1); // use uart1 tx only.
    printf("Hello, from an RP2040!\r\n");
#endif
    // Init Synchronizer.
    HarpSynchronizer::init(uart0, HARP_SYNC_RX_PIN);
    app.set_synchronizer(&HarpSynchronizer::instance());
    reset_app();
    //app.set_visual_indicators_fn(set_led_state);
    // Setup continuous writing of the latest ADC value to the corresponding
    // Harp register.
    thermistor.setup_dma_stream_to_memory(&app_regs.voltage_raw, 1);
    // Start PIO-connected hardware.
    thermistor.start();
    while(true)
        app.run();
}
