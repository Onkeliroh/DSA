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
  kSetPin,
  kReadAnalogPin,
  kReadAnalogPinResult,
};

// Callbacks define on which received commands we take action
void attachCommandCallbacks()
{
  // Attach callback methods
  cmdMessenger.attach(OnUnknownCommand);
  cmdMessenger.attach(kSetPin, OnSetPin);
  cmdMessenger.attach(kReadAnalogPin, OnReadAnalogPin);
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
  digitalWrite(pinnr,State);
}

void OnReadAnalogPin()
{
  float val = cmdMessenger.readInt16Arg();
  cmdMessenger.sendCmdStart(kReadAnalogPinResult);
  cmdMessenger.sendCmdArg(analogRead(val));
  cmdMessenger.sendCmdEnd();
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
