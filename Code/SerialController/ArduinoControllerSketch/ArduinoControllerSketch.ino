#include "ArduinoController.h"

ArduinoController ACON;

void setup() {
  ACON = ArduinoController();
  
  Serial.begin(9600);
}

void loop() {  
    ACON.WorkThatCommand();
}
