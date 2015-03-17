#ifndef ARDUINOCONTROLLER
#define ARDUINOCONTROLLER
#include <Arduino.h>

#define EndOfCommand 47

#define READPIN 48

class ArduinoController {
  public:
    void WorkThatCommand();
  private:
    byte ReadPin(byte pinnr);
};
#endif
