#!/usr/bin/env python3
from pyharp.device import Device, DeviceMode
from pyharp.messages import HarpMessage
from pyharp.messages import MessageType
from pyharp.messages import CommonRegisters as Regs
from struct import *
from time import sleep, perf_counter
import logging
import os
import serial.tools.list_ports

#import logging
#logging.basicConfig()
#logger = logging.getLogger()
#logger.setLevel(logging.DEBUG)

# Open serial connection with the first Valve Controller.
com_port = None
ports = serial.tools.list_ports.comports()
for port, desc, hwid in sorted(ports):
    if desc.startswith("sniff-detector"):
        print("{}: {} [{}]".format(port, desc, hwid))
        com_port = port
        break
device = Device(com_port)

# Read encoder and torque raw measurements.
try:
    while True:
        measurement = device.send(HarpMessage.ReadU16(32).frame)
        print("Thermistor raw:")
        print(measurement.payload)
        print()
        sleep(0.015);
finally:
    # Close connection
    device.disconnect()
