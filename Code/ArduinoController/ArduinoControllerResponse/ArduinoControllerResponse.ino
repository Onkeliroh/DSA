#include <CmdMessenger.h>  // CmdMessenger

// Attach a new CmdMessenger object to the default Serial port
CmdMessenger cmdMessenger = CmdMessenger(Serial);

// This is the list of recognized commands. These can be commands that can either be sent or received.
// In order to receive, attach a callback function to these events
enum
{
  kAcknowledge,
  kError,
  kReady,
  kSetPinMode,
  kSetPinState,
  kSetAnalogPin,
  kSetPin,
  kSetAnalogReference,
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
  kGetAnalogReference,
  kGetAnalogPinNumbers,
  kGetSDASCL,
};

//DEFINITIONS-------------------------------------------------------------------
#define VERSION 0.3

//END----DEFINITIONS------------------------------------------------------------

// Callbacks define on which received commands we take action
void attachCommandCallbacks()
{
  // Attach callback methods
  cmdMessenger.attach(OnUnknownCommand);
  cmdMessenger.attach(kReady,OnReady);
  cmdMessenger.attach(kSetPin, OnSetPin);
  cmdMessenger.attach(kReadAnalogPin, OnReadAnalogPin);
  cmdMessenger.attach(kReadPin, OnReadPin);
  cmdMessenger.attach(kSetPinMode, OnSetPinMode);
  cmdMessenger.attach(kSetPinState, OnSetPinState);
  cmdMessenger.attach(kSetAnalogPin, OnSetAnalogPin);
  cmdMessenger.attach(kSetAnalogReference, OnSetAnalogReference);
  cmdMessenger.attach(kGetVersion, OnGetVersion);
  cmdMessenger.attach(kGetModel, OnGetModel);
  cmdMessenger.attach(kGetNumberDigitalPins, OnGetNumberDigitalPins);
  cmdMessenger.attach(kGetNumberAnalogPins, OnGetNumberAnalogPins);
  cmdMessenger.attach(kGetDigitalBitMask, OnGetDigitalBitMask);
  cmdMessenger.attach(kGetPinOutputMask, OnGetPinOutputMask);
  cmdMessenger.attach(kGetPinModeMask, OnGetPinModeMask);
  cmdMessenger.attach(kGetAnalogReference, OnGetAnalogReference);
  cmdMessenger.attach(kGetAnalogPinNumbers, OnGetAnalogPinNumbers);
  cmdMessenger.attach(kGetSDASCL,OnGetSDASCL);
}

// Called when a received command has no attached function
void OnUnknownCommand()
{
  cmdMessenger.sendCmd(kError,"Command without attached callback");
}

void OnReady()
{
  cmdMessenger.sendCmd(kReady,"Ready");
}

void OnSetPin()
{
  int pinnr = cmdMessenger.readInt16Arg();
  byte Mode = cmdMessenger.readInt16Arg();
  byte State = cmdMessenger.readInt16Arg();

  pinMode(pinnr,Mode);
  digitalWrite(pinnr,State);

  cmdMessenger.sendCmdStart(kSetPin);
  cmdMessenger.sendCmdArg(pinnr);
  cmdMessenger.sendCmdArg(Mode);
  cmdMessenger.sendCmdArg(State);
  cmdMessenger.sendCmdEnd();
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

  cmdMessenger.sendCmd(kSetPinState);
}

void OnReadAnalogPin()
{
  int NrOfValues = cmdMessenger.readInt16Arg();
  int Pins[NrOfValues];

  cmdMessenger.sendCmdStart(kReadAnalogPin);

  for ( int i = 0; i < NrOfValues; i++)
  {
    Pins[i] = analogRead(Pins[i]);
    cmdMessenger.sendCmdSciArg(Pins[i]);
  }

  cmdMessenger.sendCmdEnd();
}

void OnReadPin()
{
  int nr = cmdMessenger.readInt16Arg();
  cmdMessenger.sendCmdStart(kReadPin);
  cmdMessenger.sendCmdArg(digitalRead(nr));
  cmdMessenger.sendCmdEnd();
}

void OnSetAnalogPin()
{
  int Pin = cmdMessenger.readInt16Arg();
  int Val = cmdMessenger.readInt16Arg();
  analogWrite(Pin,Val);
}

void OnSetAnalogReference()
{
  analogReference(cmdMessenger.readInt16Arg());
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
  #if defined (__AVR_AT94K__)
  cmdMessenger.sendCmdArg("AT94K");
  #elif defined (__AVR_AT43USB320__)
  cmdMessenger.sendCmdArg("AT43USB320");
  #elif defined (__AVR_AT43USB355__)
  cmdMessenger.sendCmdArg("AT43USB355");
  #elif defined (__AVR_AT76C711__)
  cmdMessenger.sendCmdArg("AT76C711");
  #elif defined (__AVR_AT86RF401__)
  cmdMessenger.sendCmdArg("AT86RF401");
  #elif defined (__AVR_AT90PWM1__)
  cmdMessenger.sendCmdArg("AT90PWM1");
  #elif defined (__AVR_AT90PWM2__)
  cmdMessenger.sendCmdArg("AT90PWM2");
  #elif defined (__AVR_AT90PWM2B__)
  cmdMessenger.sendCmdArg("AT90PWM2B");
  #elif defined (__AVR_AT90PWM3__)
  cmdMessenger.sendCmdArg("AT90PWM3");
  #elif defined (__AVR_AT90PWM3B__)
  cmdMessenger.sendCmdArg("AT90PWM3B");
  #elif defined (__AVR_AT90PWM216__)
  cmdMessenger.sendCmdArg("AT90PWM216");
  #elif defined (__AVR_AT90PWM316__)
  cmdMessenger.sendCmdArg("AT90PWM316");
  #elif defined (__AVR_AT90PWM161__)
  cmdMessenger.sendCmdArg("AT90PWM161");
  #elif defined (__AVR_AT90PWM81__)
  cmdMessenger.sendCmdArg("AT90PWM81");
  #elif defined (__AVR_ATmega8U2__)
  cmdMessenger.sendCmdArg("ATmega8U2");
  #elif defined (__AVR_ATmega16M1__)
  cmdMessenger.sendCmdArg("ATmega16M1");
  #elif defined (__AVR_ATmega16U2__)
  cmdMessenger.sendCmdArg("ATmega16U2");
  #elif defined (__AVR_ATmega16U4__)
  cmdMessenger.sendCmdArg("ATmega16U4");
  #elif defined (__AVR_ATmega32C1__)
  cmdMessenger.sendCmdArg("ATmega32C1");
  #elif defined (__AVR_ATmega32M1__)
  cmdMessenger.sendCmdArg("ATmega32M1");
  #elif defined (__AVR_ATmega32U2__)
  cmdMessenger.sendCmdArg("ATmega32U2");
  #elif defined (__AVR_ATmega32U4__)
  cmdMessenger.sendCmdArg("ATmega32U4");
  #elif defined (__AVR_ATmega32U6__)
  cmdMessenger.sendCmdArg("ATmega32U6");
  #elif defined (__AVR_ATmega64C1__)
  cmdMessenger.sendCmdArg("ATmega64C1");
  #elif defined (__AVR_ATmega64M1__)
  cmdMessenger.sendCmdArg("ATmega64M1");
  #elif defined (__AVR_ATmega128__)
  cmdMessenger.sendCmdArg("ATmega128");
  #elif defined (__AVR_ATmega128A__)
  cmdMessenger.sendCmdArg("ATmega128A");
  #elif defined (__AVR_ATmega1280__)
  cmdMessenger.sendCmdArg("ATmega1280");
  #elif defined (__AVR_ATmega1281__)
  cmdMessenger.sendCmdArg("ATmega1281");
  #elif defined (__AVR_ATmega1284__)
  cmdMessenger.sendCmdArg("ATmega1284");
  #elif defined (__AVR_ATmega1284P__)
  cmdMessenger.sendCmdArg("ATmega1284P");
  #elif defined (__AVR_ATmega128RFA1__)
  cmdMessenger.sendCmdArg("ATmega128RFA1");
  #elif defined (__AVR_ATmega128RFR2__)
  cmdMessenger.sendCmdArg("ATmega128RFR2");
  #elif defined (__AVR_ATmega1284RFR2__)
  cmdMessenger.sendCmdArg("ATmega1284RFR2");
  #elif defined (__AVR_ATmega256RFR2__)
  cmdMessenger.sendCmdArg("ATmega256RFR2");
  #elif defined (__AVR_ATmega2564RFR2__)
  cmdMessenger.sendCmdArg("ATmega2564RFR2");
  #elif defined (__AVR_ATmega2560__)
  cmdMessenger.sendCmdArg("ATmega2560");
  #elif defined (__AVR_ATmega2561__)
  cmdMessenger.sendCmdArg("ATmega2561");
  #elif defined (__AVR_AT90CAN32__)
  cmdMessenger.sendCmdArg("AT90CAN32");
  #elif defined (__AVR_AT90CAN64__)
  cmdMessenger.sendCmdArg("AT90CAN64");
  #elif defined (__AVR_AT90CAN128__)
  cmdMessenger.sendCmdArg("AT90CAN128");
  #elif defined (__AVR_AT90USB82__)
  cmdMessenger.sendCmdArg("AT90USB82");
  #elif defined (__AVR_AT90USB162__)
  cmdMessenger.sendCmdArg("AT90USB162");
  #elif defined (__AVR_AT90USB646__)
  cmdMessenger.sendCmdArg("AT90USB646");
  #elif defined (__AVR_AT90USB647__)
  cmdMessenger.sendCmdArg("AT90USB647");
  #elif defined (__AVR_AT90USB1286__)
  cmdMessenger.sendCmdArg("AT90USB1286");
  #elif defined (__AVR_AT90USB1287__)
  cmdMessenger.sendCmdArg("AT90USB1287");
  #elif defined (__AVR_ATmega64RFR2__)
  cmdMessenger.sendCmdArg("ATmega64RFR2");
  #elif defined (__AVR_ATmega644RFR2__)
  cmdMessenger.sendCmdArg("ATmega644RFR2");
  #elif defined (__AVR_ATmega64__)
  cmdMessenger.sendCmdArg("ATmega64");
  #elif defined (__AVR_ATmega64A__)
  cmdMessenger.sendCmdArg("ATmega64A");
  #elif defined (__AVR_ATmega640__)
  cmdMessenger.sendCmdArg("ATmega640");
  #elif defined (__AVR_ATmega644__)
  cmdMessenger.sendCmdArg("ATmega644");
  #elif (defined __AVR_ATmega644A__)
  cmdMessenger.sendCmdArg("ATmega644A");
  #elif defined (__AVR_ATmega644P__)
  cmdMessenger.sendCmdArg("ATmega644P");
  #elif defined (__AVR_ATmega644PA__)
  cmdMessenger.sendCmdArg("ATmega644PA");
  #elif defined (__AVR_ATmega645__)
  cmdMessenger.sendCmdArg("ATmega645");
  #elif (defined __AVR_ATmega645A__)
  cmdMessenger.sendCmdArg("ATmega645A");
  #elif (defined __AVR_ATmega645P__)
  cmdMessenger.sendCmdArg("ATmega645P");
  #elif defined (__AVR_ATmega6450__)
  cmdMessenger.sendCmdArg("ATmega6450");
  #elif (defined __AVR_ATmega6450A__)
  cmdMessenger.sendCmdArg("ATmega6450A");
  #elif (defined __AVR_ATmega6450P__)
  cmdMessenger.sendCmdArg("ATmega6450P");
  #elif defined (__AVR_ATmega649__)
  cmdMessenger.sendCmdArg("ATmega649");
  #elif (defined __AVR_ATmega649A__)
  cmdMessenger.sendCmdArg("ATmega649A");
  #elif defined (__AVR_ATmega6490__)
  cmdMessenger.sendCmdArg("ATmega6490");
  #elif (defined __AVR_ATmega6490A__)
  cmdMessenger.sendCmdArg("ATmega6490A");
  #elif (defined __AVR_ATmega6490P__)
  cmdMessenger.sendCmdArg("ATmega6490P");
  #elif defined (__AVR_ATmega649P__)
  cmdMessenger.sendCmdArg("ATmega649P");
  #elif defined (__AVR_ATmega64HVE__)
  cmdMessenger.sendCmdArg("ATmega64HVE");
  #elif defined (__AVR_ATmega64HVE2__)
  cmdMessenger.sendCmdArg("ATmega64HVE2");
  #elif defined (__AVR_ATmega103__)
  cmdMessenger.sendCmdArg("ATmega103");
  #elif defined (__AVR_ATmega32__)
  cmdMessenger.sendCmdArg("ATmega32");
  #elif defined (__AVR_ATmega32A__)
  cmdMessenger.sendCmdArg("ATmega32A");
  #elif defined (__AVR_ATmega323__)
  cmdMessenger.sendCmdArg("ATmega323");
  #elif defined (__AVR_ATmega324P__)
  cmdMessenger.sendCmdArg("ATmega324P");
  #elif (defined __AVR_ATmega324A__)
  cmdMessenger.sendCmdArg("ATmega324A");
  #elif defined (__AVR_ATmega324PA__)
  cmdMessenger.sendCmdArg("ATmega324PA");
  #elif defined (__AVR_ATmega325__)
  cmdMessenger.sendCmdArg("ATmega325");
  #elif (defined __AVR_ATmega325A__)
  cmdMessenger.sendCmdArg("ATmega325A");
  #elif defined (__AVR_ATmega325P__)
  cmdMessenger.sendCmdArg("ATmega325P");
  #elif defined (__AVR_ATmega325PA__)
  cmdMessenger.sendCmdArg("ATmega325PA");
  #elif defined (__AVR_ATmega3250__)
  cmdMessenger.sendCmdArg("ATmega3250");
  #elif (defined __AVR_ATmega3250A__)
  cmdMessenger.sendCmdArg("ATmega3250A");
  #elif defined (__AVR_ATmega3250P__)
  cmdMessenger.sendCmdArg("ATmega3250P");
  #elif defined (__AVR_ATmega3250PA__)
  cmdMessenger.sendCmdArg("ATmega3250PA");
  #elif defined (__AVR_ATmega328P__)
  cmdMessenger.sendCmdArg("ATmega328P");
  #elif (defined __AVR_ATmega328__)
  cmdMessenger.sendCmdArg("ATmega328");
  #elif defined (__AVR_ATmega329__)
  cmdMessenger.sendCmdArg("ATmega329");
  #elif (defined __AVR_ATmega329A__)
  cmdMessenger.sendCmdArg("ATmega329A");
  #elif defined (__AVR_ATmega329P__)
  cmdMessenger.sendCmdArg("ATmega329P");
  #elif (defined __AVR_ATmega329PA__)
  cmdMessenger.sendCmdArg("ATmega329PA");
  #elif (defined __AVR_ATmega3290PA__)
  cmdMessenger.sendCmdArg("ATmega3290PA");
  #elif defined (__AVR_ATmega3290__)
  cmdMessenger.sendCmdArg("ATmega3290");
  #elif (defined __AVR_ATmega3290A__)
  cmdMessenger.sendCmdArg("ATmega3290A");
  #elif defined (__AVR_ATmega3290P__)
  cmdMessenger.sendCmdArg("ATmega3290P");
  #elif defined (__AVR_ATmega32HVB__)
  cmdMessenger.sendCmdArg("ATmega32HVB");
  #elif defined (__AVR_ATmega32HVBREVB__)
  cmdMessenger.sendCmdArg("ATmega32HVBREVB");
  #elif defined (__AVR_ATmega406__)
  cmdMessenger.sendCmdArg("ATmega406");
  #elif defined (__AVR_ATmega16__)
  cmdMessenger.sendCmdArg("ATmega16");
  #elif defined (__AVR_ATmega16A__)
  cmdMessenger.sendCmdArg("ATmega16A");
  #elif defined (__AVR_ATmega161__)
  cmdMessenger.sendCmdArg("ATmega161");
  #elif defined (__AVR_ATmega162__)
  cmdMessenger.sendCmdArg("ATmega162");
  #elif defined (__AVR_ATmega163__)
  cmdMessenger.sendCmdArg("ATmega163");
  #elif defined (__AVR_ATmega164P__)
  cmdMessenger.sendCmdArg("ATmega164P");
  #elif (defined __AVR_ATmega164A__)
  cmdMessenger.sendCmdArg("ATmega164A");
  #elif defined (__AVR_ATmega164PA__)
  cmdMessenger.sendCmdArg("ATmega164PA");
  #elif defined (__AVR_ATmega165__)
  cmdMessenger.sendCmdArg("ATmega165");
  #elif (defined __AVR_ATmega165A__)
  cmdMessenger.sendCmdArg("ATmega165A");
  #elif defined (__AVR_ATmega165P__)
  cmdMessenger.sendCmdArg("ATmega165P");
  #elif defined (__AVR_ATmega165PA__)
  cmdMessenger.sendCmdArg("ATmega165PA");
  #elif defined (__AVR_ATmega168__)
  cmdMessenger.sendCmdArg("ATmega168");
  #elif (defined __AVR_ATmega168A__)
  cmdMessenger.sendCmdArg("ATmega168A");
  #elif defined (__AVR_ATmega168P__)
  cmdMessenger.sendCmdArg("ATmega168P");
  #elif defined (__AVR_ATmega168PA__)
  cmdMessenger.sendCmdArg("ATmega168PA");
  #elif defined (__AVR_ATmega168PB__)
  cmdMessenger.sendCmdArg("ATmega168PB");
  #elif defined (__AVR_ATmega169__)
  cmdMessenger.sendCmdArg("ATmega169");
  #elif (defined __AVR_ATmega169A__)
  cmdMessenger.sendCmdArg("ATmega169A");
  #elif defined (__AVR_ATmega169P__)
  cmdMessenger.sendCmdArg("ATmega169P");
  #elif defined (__AVR_ATmega169PA__)
  cmdMessenger.sendCmdArg("ATmega169PA");
  #elif defined (__AVR_ATmega8HVA__)
  cmdMessenger.sendCmdArg("ATmega8HVA");
  #elif defined (__AVR_ATmega16HVA__)
  cmdMessenger.sendCmdArg("ATmega16HVA");
  #elif defined (__AVR_ATmega16HVA2__)
  cmdMessenger.sendCmdArg("ATmega16HVA2");
  #elif defined (__AVR_ATmega16HVB__)
  cmdMessenger.sendCmdArg("ATmega16HVB");
  #elif defined (__AVR_ATmega16HVBREVB__)
  cmdMessenger.sendCmdArg("ATmega16HVBREVB");
  #elif defined (__AVR_ATmega8__)
  cmdMessenger.sendCmdArg("ATmega8");
  #elif defined (__AVR_ATmega8A__)
  cmdMessenger.sendCmdArg("ATmega8A");
  #elif (defined __AVR_ATmega48A__)
  cmdMessenger.sendCmdArg("ATmega48A");
  #elif defined (__AVR_ATmega48__)
  cmdMessenger.sendCmdArg("ATmega48");
  #elif defined (__AVR_ATmega48PA__)
  cmdMessenger.sendCmdArg("ATmega48PA");
  #elif defined (__AVR_ATmega48PB__)
  cmdMessenger.sendCmdArg("ATmega48PB");
  #elif defined (__AVR_ATmega48P__)
  cmdMessenger.sendCmdArg("ATmega48P");
  #elif defined (__AVR_ATmega88__)
  cmdMessenger.sendCmdArg("ATmega88");
  #elif (defined __AVR_ATmega88A__)
  cmdMessenger.sendCmdArg("ATmega88A");
  #elif defined (__AVR_ATmega88P__)
  cmdMessenger.sendCmdArg("ATmega88P");
  #elif defined (__AVR_ATmega88PA__)
  cmdMessenger.sendCmdArg("ATmega88PA");
  #elif defined (__AVR_ATmega88PB__)
  cmdMessenger.sendCmdArg("ATmega88PB");
  #elif defined (__AVR_ATmega8515__)
  cmdMessenger.sendCmdArg("ATmega8515");
  #elif defined (__AVR_ATmega8535__)
  cmdMessenger.sendCmdArg("ATmega8535");
  #elif defined (__AVR_AT90S8535__)
  cmdMessenger.sendCmdArg("AT90S8535");
  #elif defined (__AVR_AT90C8534__)
  cmdMessenger.sendCmdArg("AT90C8534");
  #elif defined (__AVR_AT90S8515__)
  cmdMessenger.sendCmdArg("AT90S8515");
  #elif defined (__AVR_AT90S4434__)
  cmdMessenger.sendCmdArg("AT90S4434");
  #elif defined (__AVR_AT90S4433__)
  cmdMessenger.sendCmdArg("AT90S4433");
  #elif defined (__AVR_AT90S4414__)
  cmdMessenger.sendCmdArg("AT90S4414");
  #elif defined (__AVR_ATtiny22__)
  cmdMessenger.sendCmdArg("ATtiny22");
  #elif defined (__AVR_ATtiny26__)
  cmdMessenger.sendCmdArg("ATtiny26");
  #elif defined (__AVR_AT90S2343__)
  cmdMessenger.sendCmdArg("AT90S2343");
  #elif defined (__AVR_AT90S2333__)
  cmdMessenger.sendCmdArg("AT90S2333");
  #elif defined (__AVR_AT90S2323__)
  cmdMessenger.sendCmdArg("AT90S2323");
  #elif defined (__AVR_AT90S2313__)
  cmdMessenger.sendCmdArg("AT90S2313");
  #elif defined (__AVR_ATtiny4__)
  cmdMessenger.sendCmdArg("ATtiny4");
  #elif defined (__AVR_ATtiny5__)
  cmdMessenger.sendCmdArg("ATtiny5");
  #elif defined (__AVR_ATtiny9__)
  cmdMessenger.sendCmdArg("ATtiny9");
  #elif defined (__AVR_ATtiny10__)
  cmdMessenger.sendCmdArg("ATtiny10");
  #elif defined (__AVR_ATtiny20__)
  cmdMessenger.sendCmdArg("ATtiny20");
  #elif defined (__AVR_ATtiny40__)
  cmdMessenger.sendCmdArg("ATtiny40");
  #elif defined (__AVR_ATtiny2313__)
  cmdMessenger.sendCmdArg("ATtiny2313");
  #elif defined (__AVR_ATtiny2313A__)
  cmdMessenger.sendCmdArg("ATtiny2313A");
  #elif defined (__AVR_ATtiny13__)
  cmdMessenger.sendCmdArg("ATtiny13");
  #elif defined (__AVR_ATtiny13A__)
  cmdMessenger.sendCmdArg("ATtiny13A");
  #elif defined (__AVR_ATtiny25__)
  cmdMessenger.sendCmdArg("ATtiny25");
  #elif defined (__AVR_ATtiny4313__)
  cmdMessenger.sendCmdArg("ATtiny4313");
  #elif defined (__AVR_ATtiny45__)
  cmdMessenger.sendCmdArg("ATtiny45");
  #elif defined (__AVR_ATtiny85__)
  cmdMessenger.sendCmdArg("ATtiny85");
  #elif defined (__AVR_ATtiny24__)
  cmdMessenger.sendCmdArg("ATtiny24");
  #elif defined (__AVR_ATtiny24A__)
  cmdMessenger.sendCmdArg("ATtiny24A");
  #elif defined (__AVR_ATtiny44__)
  cmdMessenger.sendCmdArg("ATtiny44");
  #elif defined (__AVR_ATtiny44A__)
  cmdMessenger.sendCmdArg("ATtiny44A");
  #elif defined (__AVR_ATtiny441__)
  cmdMessenger.sendCmdArg("ATtiny441");
  #elif defined (__AVR_ATtiny84__)
  cmdMessenger.sendCmdArg("ATtiny84");
  #elif defined (__AVR_ATtiny84A__)
  cmdMessenger.sendCmdArg("ATtiny84A");
  #elif defined (__AVR_ATtiny841__)
  cmdMessenger.sendCmdArg("ATtiny841");
  #elif defined (__AVR_ATtiny261__)
  cmdMessenger.sendCmdArg("ATtiny261");
  #elif defined (__AVR_ATtiny261A__)
  cmdMessenger.sendCmdArg("ATtiny261A");
  #elif defined (__AVR_ATtiny461__)
  cmdMessenger.sendCmdArg("ATtiny461");
  #elif defined (__AVR_ATtiny461A__)
  cmdMessenger.sendCmdArg("ATtiny461A");
  #elif defined (__AVR_ATtiny861__)
  cmdMessenger.sendCmdArg("ATtiny861");
  #elif defined (__AVR_ATtiny861A__)
  cmdMessenger.sendCmdArg("ATtiny861A");
  #elif defined (__AVR_ATtiny43U__)
  cmdMessenger.sendCmdArg("ATtiny43U");
  #elif defined (__AVR_ATtiny48__)
  cmdMessenger.sendCmdArg("ATtiny48");
  #elif defined (__AVR_ATtiny88__)
  cmdMessenger.sendCmdArg("ATtiny88");
  #elif defined (__AVR_ATtiny828__)
  cmdMessenger.sendCmdArg("ATtiny828");
  #elif defined (__AVR_ATtiny87__)
  cmdMessenger.sendCmdArg("ATtiny87");
  #elif defined (__AVR_ATtiny167__)
  cmdMessenger.sendCmdArg("ATtiny167");
  #elif defined (__AVR_ATtiny1634__)
  cmdMessenger.sendCmdArg("ATtiny1634");
  #elif defined (__AVR_AT90SCR100__)
  cmdMessenger.sendCmdArg("AT90SCR100");
  #elif defined (__AVR_ATxmega16A4__)
  cmdMessenger.sendCmdArg("ATxmega16A4");
  #elif defined (__AVR_ATxmega16A4U__)
  cmdMessenger.sendCmdArg("ATxmega16A4U");
  #elif defined (__AVR_ATxmega16C4__)
  cmdMessenger.sendCmdArg("ATxmega16C4");
  #elif defined (__AVR_ATxmega16D4__)
  cmdMessenger.sendCmdArg("ATxmega16D4");
  #elif defined (__AVR_ATxmega32A4__)
  cmdMessenger.sendCmdArg("ATxmega32A4");
  #elif defined (__AVR_ATxmega32A4U__)
  cmdMessenger.sendCmdArg("ATxmega32A4U");
  #elif defined (__AVR_ATxmega32C3__)
  cmdMessenger.sendCmdArg("ATxmega32C3");
  #elif defined (__AVR_ATxmega32C4__)
  cmdMessenger.sendCmdArg("ATxmega32C4");
  #elif defined (__AVR_ATxmega32D3__)
  cmdMessenger.sendCmdArg("ATxmega32D3");
  #elif defined (__AVR_ATxmega32D4__)
  cmdMessenger.sendCmdArg("ATxmega32D4");
  #elif defined (__AVR_ATxmega8E5__)
  cmdMessenger.sendCmdArg("ATxmega8E5");
  #elif defined (__AVR_ATxmega16E5__)
  cmdMessenger.sendCmdArg("ATxmega16E5");
  #elif defined (__AVR_ATxmega32E5__)
  cmdMessenger.sendCmdArg("ATxmega32E5");
  #elif defined (__AVR_ATxmega64A1__)
  cmdMessenger.sendCmdArg("ATxmega64A1");
  #elif defined (__AVR_ATxmega64A1U__)
  cmdMessenger.sendCmdArg("ATxmega64A1U");
  #elif defined (__AVR_ATxmega64A3__)
  cmdMessenger.sendCmdArg("ATxmega64A3");
  #elif defined (__AVR_ATxmega64A3U__)
  cmdMessenger.sendCmdArg("ATxmega64A3U");
  #elif defined (__AVR_ATxmega64A4U__)
  cmdMessenger.sendCmdArg("ATxmega64A4U");
  #elif defined (__AVR_ATxmega64B1__)
  cmdMessenger.sendCmdArg("ATxmega64B1");
  #elif defined (__AVR_ATxmega64B3__)
  cmdMessenger.sendCmdArg("ATxmega64B3");
  #elif defined (__AVR_ATxmega64C3__)
  cmdMessenger.sendCmdArg("ATxmega64C3");
  #elif defined (__AVR_ATxmega64D3__)
  cmdMessenger.sendCmdArg("ATxmega64D3");
  #elif defined (__AVR_ATxmega64D4__)
  cmdMessenger.sendCmdArg("ATxmega64D4");
  #elif defined (__AVR_ATxmega128A1__)
  cmdMessenger.sendCmdArg("ATxmega128A1");
  #elif defined (__AVR_ATxmega128A1U__)
  cmdMessenger.sendCmdArg("ATxmega128A1U");
  #elif defined (__AVR_ATxmega128A4U__)
  cmdMessenger.sendCmdArg("ATxmega128A4U");
  #elif defined (__AVR_ATxmega128A3__)
  cmdMessenger.sendCmdArg("ATxmega128A3");
  #elif defined (__AVR_ATxmega128A3U__)
  cmdMessenger.sendCmdArg("ATxmega128A3U");
  #elif defined (__AVR_ATxmega128B1__)
  cmdMessenger.sendCmdArg("ATxmega128B1");
  #elif defined (__AVR_ATxmega128B3__)
  cmdMessenger.sendCmdArg("ATxmega128B3");
  #elif defined (__AVR_ATxmega128C3__)
  cmdMessenger.sendCmdArg("ATxmega128C3");
  #elif defined (__AVR_ATxmega128D3__)
  cmdMessenger.sendCmdArg("ATxmega128D3");
  #elif defined (__AVR_ATxmega128D4__)
  cmdMessenger.sendCmdArg("ATxmega128D4");
  #elif defined (__AVR_ATxmega192A3__)
  cmdMessenger.sendCmdArg("ATxmega192A3");
  #elif defined (__AVR_ATxmega192A3U__)
  cmdMessenger.sendCmdArg("ATxmega192A3U");
  #elif defined (__AVR_ATxmega192C3__)
  cmdMessenger.sendCmdArg("ATxmega192C3");
  #elif defined (__AVR_ATxmega192D3__)
  cmdMessenger.sendCmdArg("ATxmega192D3");
  #elif defined (__AVR_ATxmega256A3__)
  cmdMessenger.sendCmdArg("ATxmega256A3");
  #elif defined (__AVR_ATxmega256A3U__)
  cmdMessenger.sendCmdArg("ATxmega256A3U");
  #elif defined (__AVR_ATxmega256A3B__)
  cmdMessenger.sendCmdArg("ATxmega256A3B");
  #elif defined (__AVR_ATxmega256A3BU__)
  cmdMessenger.sendCmdArg("ATxmega256A3BU");
  #elif defined (__AVR_ATxmega256C3__)
  cmdMessenger.sendCmdArg("ATxmega256C3");
  #elif defined (__AVR_ATxmega256D3__)
  cmdMessenger.sendCmdArg("ATxmega256D3");
  #elif defined (__AVR_ATxmega384C3__)
  cmdMessenger.sendCmdArg("ATxmega384C3");
  #elif defined (__AVR_ATxmega384D3__)
  cmdMessenger.sendCmdArg("ATxmega384D3");
  #elif defined (__AVR_ATA5790__)
  cmdMessenger.sendCmdArg("ATA5790");
  #elif defined (__AVR_ATA5790N__)
  cmdMessenger.sendCmdArg("ATA5790N");
  #elif defined (__AVR_ATA5272__)
  cmdMessenger.sendCmdArg("ATA5272");
  #elif defined (__AVR_ATA5505__)
  cmdMessenger.sendCmdArg("ATA5505");
  #elif defined (__AVR_ATA5795__)
  cmdMessenger.sendCmdArg("ATA5795");
  #elif defined (__AVR_ATA5702M322__)
  cmdMessenger.sendCmdArg("ATA5702M322");
  #elif defined (__AVR_ATA5782__)
  cmdMessenger.sendCmdArg("ATA5782");
  #elif defined (__AVR_ATA5831__)
  cmdMessenger.sendCmdArg("ATA5831");
  #elif defined (__AVR_ATA6285__)
  cmdMessenger.sendCmdArg("ATA6285");
  #elif defined (__AVR_ATA6286__)
  cmdMessenger.sendCmdArg("ATA6286");
  #elif defined (__AVR_ATA6289__)
  cmdMessenger.sendCmdArg("ATA6289");
  #elif defined (__AVR_ATA6612C__)
  cmdMessenger.sendCmdArg("ATA6612C");
  #elif defined (__AVR_ATA6613C__)
  cmdMessenger.sendCmdArg("ATA6613C");
  #elif defined (__AVR_ATA6614Q__)
  cmdMessenger.sendCmdArg("ATA6614Q");
  #elif defined (__AVR_ATA6616C__)
  cmdMessenger.sendCmdArg("ATA6616C");
  #elif defined (__AVR_ATA6617C__)
  cmdMessenger.sendCmdArg("ATA6617C");
  #elif defined (__AVR_ATA664251__)
  cmdMessenger.sendCmdArg("ATA664251");
  #elif defined (__AVR_ATtiny28__)
  cmdMessenger.sendCmdArg("ATtiny28");
  #elif defined (__AVR_AT90S1200__)
  cmdMessenger.sendCmdArg("AT90S1200");
  #elif defined (__AVR_ATtiny15__)
  cmdMessenger.sendCmdArg("ATtiny15");
  #elif defined (__AVR_ATtiny12__)
  cmdMessenger.sendCmdArg("ATtiny12");
  #elif defined (__AVR_M3000__)
  cmdMessenger.sendCmdArg("M3000");
  #else
  cmdMessenger.sendCmdArg("Unknown");
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
  cmdMessenger.sendCmdStart(kGetPinOutputMask);
  cmdMessenger.sendCmdBinArg((PIND << 16)|(PINB << 8)| PINC );
  cmdMessenger.sendCmdEnd();
}

void OnGetPinModeMask()
{
  cmdMessenger.sendCmdStart(kGetPinModeMask);
  cmdMessenger.sendCmdArg(DigitalPinsToModeMask());
  cmdMessenger.sendCmdEnd();
}

void OnGetAnalogReference()
{
  cmdMessenger.sendCmdStart(kGetAnalogReference);
  cmdMessenger.sendCmdArg("DEFAULT");
  cmdMessenger.sendCmdArg(DEFAULT);
  #ifdef INTERNAL
  cmdMessenger.sendCmdArg("INTERNAL");
  cmdMessenger.sendCmdArg(INTERNAL);
  #else
  cmdMessenger.sendCmdArg("INTERNAL1V1");
  cmdMessenger.sendCmdArg(INTERNAL1V1);
  cmdMessenger.sendCmdArg("INTERNAL2V56");
  cmdMessenger.sendCmdArg(INTERNAL2V56);
  #endif
  cmdMessenger.sendCmdArg("EXTERNAL");
  cmdMessenger.sendCmdArg(EXTERNAL);
  cmdMessenger.sendCmdEnd();
}

void OnGetAnalogPinNumbers()
{
  cmdMessenger.sendCmdStart(kGetAnalogPinNumbers);
  int i = 0;
  while(i <=NUM_ANALOG_INPUTS)
  {
    cmdMessenger.sendCmdArg( analogInputToDigitalPin( i ) );
    i += 1;
  }
  cmdMessenger.sendCmdEnd();
}

void OnGetSDASCL()
{
  cmdMessenger.sendCmdStart(kGetSDASCL);
  cmdMessenger.sendCmdArg(SDA);
  cmdMessenger.sendCmdArg(SCL);
  cmdMessenger.sendCmdEnd();
}
//---------------------ARDUINO--------------------------------------------------

// Setup function
void setup()
{
  // Listen on serial connection for messages from the PC
  Serial.begin(115200);
  while (!Serial){;} //wait for serial port to connect. Needed for Loenardo only

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

uint32_t DigitalPinsToModeMask(){
  uint32_t PinsModeMask = 0;
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
  }
  return PinsModeMask;
}
