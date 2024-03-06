#ifndef CONFIG_H
#define CONFIG_H


#define DEFAULT_DISPATCH_FREQUENCY_HZ (1)
#define MAX_EVENT_FREQUENCY_HZ (1000)

#define UART_TX_PIN (0) // for printf-style debugging.
#define HARP_SYNC_RX_PIN (5)
#define LED0 (24)
#define LED1 (25)

// ADC
#define ADC_CS_PIN (18)
#define ADC_POCI_PIN (19)
#define ADC_SCK_PIN (20)

#define HARP_DEVICE_ID (0x0579)


#endif // CONFIG_H
