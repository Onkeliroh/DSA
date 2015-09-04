//Arduino Analog Watchdog
void setup()
{
  Serial.begin(115200);
}

void loop()
{
  for(int i = 0; i< NUM_ANALOG_INPUTS; i++)
  {
      Serial.print(analogRead(i));
      Serial.print("\t");
  }
  Serial.print("\n");
}
