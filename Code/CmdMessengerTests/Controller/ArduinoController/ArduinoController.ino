// *** ArduinoController ***

// This example expands the previous Receive example. The Arduino will now send back a status.
// It adds a demonstration of how to:
// - Handle received commands that do not have a function attached
// - Send a command with a parameter to the PC
// - Shows how to invoke on the UI thread

#include <CmdMessenger.h>  // CmdMessenger

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

  kGetVersion,
  kGetModel,
  kGetNumberDigitalPins,
  kGetNumberAnalogPins,
  kGetDigitalBitMask,
  kGetPinOutputMask,
  kGetPinModeMask,
};

//DEFINITIONS-------------------------------------------------------------------
#define VERSION 0.1

//END----DEFINITIONS------------------------------------------------------------

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
  cmdMessenger.attach(kGetVersion, OnGetVersion);
  cmdMessenger.attach(kGetModel, OnGetModel);
  cmdMessenger.attach(kGetNumberDigitalPins, OnGetNumberDigitalPins);
  cmdMessenger.attach(kGetNumberAnalogPins, OnGetNumberAnalogPins);
  cmdMessenger.attach(kGetDigitalBitMask, OnGetDigitalBitMask);
  cmdMessenger.attach(kGetPinOutputMask, OnGetPinOutputMask);
  cmdMessenger.attach(kGetPinModeMask, OnGetPinModeMask);
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

void OnGetVersion()
{
	cmdMessenger.sendCmdStart(kGetVersion);
	cmdMessenger.sendCmdArg(VERSION);
	cmdMessenger.sendCmdEnd();
}

void OnGetModel()
{
	cmdMessenger.sendCmdStart(kGetModel);
	#if defined(__AVR_ATtiny24__)
	cmdMessenger.sendCmdArg(__AVR_ATtiny24__);
	#elif defined(__AVR_ATtiny44__)
	cmdMessenger.sendCmdArg(__AVR_ATtiny44__);
	#elif defined(__AVR_ATtiny84__)
	cmdMessenger.sendCmdArg(__AVR_ATtiny84__);
	#elif defined(__AVR_ATtiny25__)
	\cmdMessenger.sendCmdArg(__AVR_ATtiny25__);
	#elif defined(__AVR_ATtiny45__)
	cmdMessenger.sendCmdArg(__AVR_ATtiny45__);
	#elif defined(__AVR_ATtiny85__)
	cmdMessenger.sendCmdArg(__AVR_ATtiny85__);
	#elif defined(__AVR_ATmega1280__)
	cmdMessenger.sendCmdArg(__AVR_ATmega1280__);
	#elif defined(__AVR_ATmega2560__)
	cmdMessenger.sendCmdArg(__AVR_ATmega2560__);
	#elif defined(__AVR_ATmega1284__)
	cmdMessenger.sendCmdArg(__AVR_ATmega1284__);
	#elif defined(__AVR_ATmega1284P__)
	cmdMessenger.sendCmdArg(__AVR_ATmega1284P__);
	#elif defined(__AVR_ATmega644__)
	cmdMessenger.sendCmdArg(__AVR_ATmega644__);
	#elif defined(__AVR_ATmega644A__)
	cmdMessenger.sendCmdArg(__AVR_ATmega644A__);
	#elif defined(__AVR_ATmega644P__)
	cmdMessenger.sendCmdArg(__AVR_ATmega644P__);
	#elif defined(__AVR_ATmega644PA__)
	cmdMessenger.sendCmdArg(__AVR_ATmega644PA__);
	#endif
	cmdMessenger.sendCmdEnd();
}

void OnGetNumberDigitalPins()
{
	cmdMessenger.sendCmdStart(kGetNumberDigitalPins);
	cmdMessenger.sendCmdArg(NUM_DIGITAL_PINS);
	cmdMessenger.sendCmdEnd();
}

void OnGetNumberAnalogPins()
{
	cmdMessenger.sendCmdStart(kGetNumberAnalogPins);
	cmdMessenger.sendCmdArg(NUM_ANALOG_INPUTS);
	cmdMessenger.sendCmdEnd();
}

void OnGetDigitalBitMask()
{
	uint32_t mask = 0;
	mask = (PINB<<16)|(PINC<<8)|PIND;
	cmdMessenger.sendCmdStart(kGetDigitalBitMask);
	cmdMessenger.sendCmdBinArg(mask);
	cmdMessenger.sendCmdEnd();
}

void OnGetPinOutputMask()
{
	cmdMessenger.sendCmdStart(kGetPinModeMask);
	cmdMessenger.sendCmdBinArg((PORTD << 16)|(PORTB << 8)| PORTC );
	cmdMessenger.sendCmdEnd();
}

void OnGetPinModeMask()
{
	cmdMessenger.sendCmdStart(kGetPinModeMask);
	cmdMessenger.sendCmdBinArg((DDRD << 16) | (DDRB << 8) | DDRC);
	cmdMessenger.sendCmdEnd();
}

//---------------------ARDUINO--------------------------------------------------

// Setup function
void setup()
{
  // Listen on serial connection for messages from the PC
  Serial.begin(115200);
  while (!Serial){;} //wait for serial port to connect. Needed for Loenardo only

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
