void setup()
{
	Serial.begin(9600);
}

void loop()
{
	Serial.print("D PINS: ");
	Serial.print(NUM_DIGITAL_PINS);
	Serial.print("\r\n");
	Serial.print("A PINS: ");
	Serial.print(NUM_ANALOG_INPUTS);
	Serial.print("\r\n");
	delay(5000);
}
