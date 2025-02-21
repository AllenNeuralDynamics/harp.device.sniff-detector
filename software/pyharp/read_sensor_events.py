#!/usr/bin/env python3
from pyharp.device import Device, DeviceMode
from pyharp.messages import HarpMessage
from pyharp.messages import MessageType
from pyharp.messages import CommonRegisters as Regs
from struct import *
import logging
import os
import serial.tools.list_ports
from time import sleep, perf_counter

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
    # Enable periodic sniff sensor messages
    dispatch_rate_msg = HarpMessage.WriteU16(33, 1)
    device.send(dispatch_rate_msg.frame)
    while True:
        for msg in device.get_events():
            print(msg)
            print()
finally:
    # Disable periodic sniff sensor messages.
    dispatch_rate_msg = HarpMessage.WriteU16(33, 0)
    device.send(dispatch_rate_msg.frame)
    # Close connection
    device.disconnect()
