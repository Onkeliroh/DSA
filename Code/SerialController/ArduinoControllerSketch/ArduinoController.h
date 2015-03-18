#ifndef ARDUINOCONTROLLER
#define ARDUINOCONTROLLER
#include <Arduino.h>

#define DIGITAL 0x0
#define ANALOG 0x1

#define EndOfCommand 0x47
#define READPIN 0x48

class ArduinoController {
  public:
    void WorkThatCommand();
  private:
    byte ReadPin(byte pinnr, byte pinType);
};
#endif
