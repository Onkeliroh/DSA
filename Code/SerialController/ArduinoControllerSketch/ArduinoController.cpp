#include "ArduinoController.h"

byte incomming[3];

void ArduinoController::WorkThatCommand()
{
  if (Serial.available())
  {
    Serial.readBytesUntil(EndOfCommand,incomming,3);
    
    switch(incomming[0])
    {
      case 0x0:
        break;
      case READPIN:
        Serial.println(ReadPin(14));
        break;
      default:
        Serial.println("Worked dat shit");  
    }
  }
}

byte ArduinoController::ReadPin(byte PinNr)
{
   if (analogInputToDigitalPin(PinNr)!=-1)
   {
     return analogRead(PinNr);        
   }
   else
   {
     return digitalRead(PinNr);
   }
}
