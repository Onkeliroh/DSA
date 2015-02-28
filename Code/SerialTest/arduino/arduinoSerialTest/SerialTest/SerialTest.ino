/*
	Adruino sketch to test serial communication with mono
*/
void setup()
{
	Serial.begin(9600);	
}

void loop()
{
	analyse();
}

char com[20];
char inChar=-1;
byte index=0;

void analyse()
{
	while (Serial.available() > 0)
	{
		if(index < 19)
		{
			inChar = Serial.read();
			com[index] = inChar;
			index++;
			com[index] = '\0';
		}	
	}

	if (strcmp(com,"Hello") == 0)
	{
		Serial.write("Hi");
	}
	if ( strcmp(com,"How are you") == 0 )
	{
		Serial.write("good and you?");
	}
	Serial.flush();
	memset(com, 0, sizeof(com));
	index = 0;
	inChar=-1;
}
