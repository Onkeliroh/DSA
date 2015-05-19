int PIN = 0;
uint16_t Registeradresses[NUM_DIGITAL_PINS];
uint8_t UniqueRegister = 0;

void setup() {
  Serial.begin(9600);
   
  for (int i = 0; i < NUM_DIGITAL_PINS; i++)
  {
     Registeradresses[i] = digitalPinToPort(i);
  }
}

void loop() {
  
  pinMode(PIN,OUTPUT);
  digitalWrite(PIN,HIGH);
  
  Serial.print("PIN\t");
  Serial.print(PIN);
  Serial.print("\r\n");
  
  int PortOfPin = digitalPinToPort(PIN);
  Serial.print("PortOfPin\t");
  Serial.print(PortOfPin,BIN);
  Serial.print("\r\n");
  
  Serial.print("Adresses of Registers:\t");
  for (int i = 0; i < NUM_DIGITAL_PINS; i++)
  {
    Serial.print(Registeradresses[i],DEC);
    Serial.print(" | ");
  }
  Serial.print("\r\n");
  
  Serial.print("ModeMask:\r\n");
  Serial.print(DigitalPinsToModeMask(),BIN);
  Serial.print("\r\n");
  
  //Position des Bits im Port
  int DigitalPinToBitMask = digitalPinToBitMask(PIN);
  Serial.print("DigitalPinToPBitMask\t");
  Serial.print(DigitalPinToBitMask,BIN);
  Serial.print("\r\n");

  Serial.print("PortOutputRegister\t");
  Serial.print(*portOutputRegister(digitalPinToPort(PIN)),BIN);
  Serial.print("\r\n");

  Serial.print("PortInputRegister\t");
  Serial.print(*portInputRegister(digitalPinToPort(PIN)),BIN);
  Serial.print("\r\n");

  Serial.print("PortModeRegister\t");
  Serial.print(*portModeRegister(digitalPinToPort(PIN)),BIN);
  Serial.print("\r\n");
  Serial.print("\r\n");
  
 
  //pinMode(PIN,INPUT);
  PIN++;
  PIN = PIN%14; 
 
 delay(5000); 
}

//OUTPUT = 1
//INPUT = 0

int DigitalPinsToModeMask(){
  uint64_t PinsModeMask = 0;
  uint8_t bitmask;
  uint8_t port;
  for (int i = 0; i < NUM_DIGITAL_PINS; i++)
  {
     bitmask = digitalPinToBitMask(i);
     port = digitalPinToPort(i);
     
     if (bitRead(*portModeRegister(port),bit(bitmask)) == true)
     {
       bitSet(PinsModeMask,i);
     }
//     Serial.print("R:");
//     Serial.print(*portModeRegister(port),BIN);
//     Serial.print("\r\n");
//     Serial.print("M:");
//     Serial.print(bitmask,BIN);
//     Serial.print("\r\n");
  }
//  Serial.print("\r\n");
  return PinsModeMask;
}


