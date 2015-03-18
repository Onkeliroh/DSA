#include "ArduinoController.h"

#define ISANALOGPIN(p) 
byte incomming[4];

void ArduinoController::WorkThatCommand()
{
  if (Serial.available())
  {
    Serial.readBytesUntil(EndOfCommand,incomming,4);
    
    switch(incomming[0])
    {
      case 0x0:
        break;
      case READPIN:
        Serial.println(ReadPin(0,ANALOG));
        break;
      default:
        Serial.println("Worked dat shit");  
    }
  }
}

byte ArduinoController::ReadPin(byte pinNr, byte pinType)
{
  if ( pinType == DIGITAL )
  {
    return digitalRead(pinNr);
  }
  else
  {
    if (analogInputToDigitalPin(pinNr)==-1)
    {
      return analogRead(pinNr);
    }
    else
    {
      return analogRead(analogInputToDigitalPin(pinNr));
    }
  }
}
