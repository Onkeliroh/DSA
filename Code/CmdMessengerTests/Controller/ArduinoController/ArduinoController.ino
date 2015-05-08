#include "CmdMessenger.h"  // CmdMessenger
#include "BachelorController.h"

// Attach a new CmdMessenger object to the default Serial port
CmdMessenger cmdMessenger = CmdMessenger(Serial);

// This is the list of recognized commands. These can be commands that can either be sent or received. 
// In order to receive, attach a callback function to these events
enum
{
	kAcknowledge,
	kError,
	kSetPinMode,
	kSetPinState,
	kSetAnalogPin,
	kSetAnalogPinMode,
	kSetPin,
	kReadPinMode,
	kReadPinState,
	kReadAnalogPin,
	kReadPinsMode,
	kReadPin,

	kReadPinModeResult,
	kReadPinStateResult,
	kReadAnalogPinResult,
	kReadPinsModeResult,
	kReadPinResult,
};

// Callbacks define on which received commands we take action
void attachCommandCallbacks()
{
	// Attach callback methods
	cmdMessenger.attach(OnUnknownCommand);
	cmdMessenger.attach(kSetPin, OnSetPin);
	cmdMessenger.attach(kReadAnalogPin, OnReadAnalogPin);
	cmdMessenger.attach(kSetPinMode, OnSetPinMode);
	cmdMessenger.attach(kSetPinState, OnSetPinState);
	cmdMessenger.attach(kSetAnalogPin, OnSetAnalogPin);
	cmdMessenger.attach(kSetAnalogPinMode, OnSetAnalogPinMode);
}

// Called when a received command has no attached function
void OnUnknownCommand()
{
	cmdMessenger.sendCmd(kError,"Command without attached callback");
}

void OnSetPin()
{
	int pinnr = cmdMessenger.readInt16Arg();
	byte Mode = cmdMessenger.readInt16Arg();
	byte State = cmdMessenger.readInt16Arg();

	pinMode(pinnr,Mode);
	if (Mode == OUTPUT)
	{
		digitalWrite(pinnr,State);
	}
}

void OnSetPinMode()
{
	int pin = cmdMessenger.readInt16Arg();
	int mode = cmdMessenger.readInt16Arg();
	pinMode(pin,mode);
}

void OnSetPinState()
{
	int pin = cmdMessenger.readInt16Arg();
	int state = cmdMessenger.readInt16Arg();
	digitalWrite(pin,state);
}

void OnReadAnalogPin()
{
	int val = cmdMessenger.readInt16Arg();
	cmdMessenger.sendCmdStart(kReadAnalogPinResult);
	cmdMessenger.sendCmdSciArg(val);
	cmdMessenger.sendCmdSciArg(analogRead(val));
	cmdMessenger.sendCmdEnd();
}

void OnSetAnalogPin()
{
	int Pin = cmdMessenger.readInt16Arg();
	int Val = cmdMessenger.readInt16Arg();
	analogWrite(Pin,Val);
}

void OnSetAnalogPinMode()
{
	int Pin = cmdMessenger.readInt16Arg();
	pinMode(Pin,cmdMessenger.readInt16Arg());
}


// Setup function
void setup() 
{
	// Listen on serial connection for messages from the PC
	Serial.begin(115200); 

	// Adds newline to every command
	//cmdMessenger.printLfCr();   

	// Attach my application's user-defined callback methods
	attachCommandCallbacks();

	// Send the status to the PC that says the Arduino has booted
	// Note that this is a good debug function: it will let you also know 
	// if your program had a bug and the arduino restarted  
	cmdMessenger.sendCmd(kAcknowledge,"Arduino has started!");
}

// Loop function
void loop() 
{
	// Process incoming serial data, and perform callbacks
	cmdMessenger.feedinSerialData();
	delay(10);
}

