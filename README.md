# Hexy2
A Project to control my [Hexy](http://arcbotics.com/products/hexy/) using a Raspberry Pi and Adafruit PCA9685 PWM Controllers

## How to Utilise / Build

Create the following folder structure

    <root folder>\
			Hexy2 (clone of this project)
			raspberry-sharp-io (clone of https://github.com/lloydsparkes/raspberry-sharp-io)

Hexy2 brings in projects from raspberry-sharp-io at the moment while those libraries are developed, and improved for my own purposed. I have found the first time you need to open the solution in raspberry-sharp-io and build it, to ensure it brings in all of its dependancies.

##Hardware Utilised



- 1x [Hexy](http://arcbotics.com/products/hexy/)
- 2x [Adafruit PCA9685 Servo Controllers](https://www.adafruit.com/product/815)
- 1x [10DOF Controller](http://astrobeano.blogspot.co.uk/2014/01/gy-80-orientation-sensor-on-raspberry-pi.html) (Not sure if you can still get this same variant but similar products are out there, or you can get them un-bundled from Adafruit and then just chain them)
- 1x [Raspberry Pi 2](https://www.raspberrypi.org/products/raspberry-pi-2-model-b/) (Hopefully the final Autonomous system will utilise a smaller (Pi Zero?)
- 1x Battery Pack for Powering it all ([Battery Pack](http://www.amazon.co.uk/gp/product/B00SYCF8OY?psc=1&redirect=true&ref_=oh_aui_detailpage_o01_s00))
- All Manner of wires, headers, and general tools (Soldering Iron etc)