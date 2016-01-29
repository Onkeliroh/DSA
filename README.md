## Description
This application is part of my bachelor thesis. It provides a low entry level environment to create datalogging and/or sequencing setups with the Arduino.
The goal was to create a _easy to use_ interface with wich users without coding experience can use the arduino for datalogging and sequencing purposes.

This software is still under development and can therefor be concidered as "_not yet bug free_".

* `Autor:` Daniel Pollack
* `Email:` danielpollack@posteo.de

## Third party resources
The images used to portrait the Arduino Board are from the [Fritzing Software](http://fritzing.org/download/) which is under the [Creative Commons Licence BY CA](https://creativecommons.org/licenses/by-sa/3.0/)

## Dependencies/Requirements
- [Gtk#](https://github.com/mono/gtk-sharp) (Version >=2.12.0.0)
- [Mono](https://github.com/mono/mono) or [.Net Framework](https://www.microsoft.com/net) (depending on your OS)
- Access to the USB Ports on your maschine
- [CmdMessenger](https://github.com/thijse/Arduino-CmdMessenger) for compiling the Arduino sketch

## Install/Run

* Compile
  1. Download the Repository and compile the code with a Mono/C# compiler of your choise
  2. upload the [Arduino Sketch](https://github.com/Onkeliroh/DSA/blob/master/Code/ArduinoController/ArduinoController.ino) to the Arduino
  3. run the application


* Download Binary
	1. either run the `run.sh` script or the `.exe` file from the folder
      * [Linux](https://github.com/Onkeliroh/DSA/blob/master/DSA_Linux.zip)
      * [Windows](https://github.com/Onkeliroh/DSA/blob/master/DSA_Windows.zip)
	2. upload the [Arduino Sketch](https://github.com/Onkeliroh/DSA/blob/master/Code/ArduinoController/ArduinoController.ino) to the Arduino
	3. run the application
