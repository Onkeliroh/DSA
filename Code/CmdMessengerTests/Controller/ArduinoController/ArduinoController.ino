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
};

//DEFINITIONS-------------------------------------------------------------------
#define VERSION 0.2

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
  cmdMessenger.sendCmdArg("__AVR_AT94K__");
  #elif defined (__AVR_AT43USB320__)
  cmdMessenger.sendCmdArg("__AVR_AT43USB320__");
  #elif defined (__AVR_AT43USB355__)
  cmdMessenger.sendCmdArg("__AVR_AT43USB355__");
  #elif defined (__AVR_AT76C711__)
  cmdMessenger.sendCmdArg("__AVR_AT76C711__");
  #elif defined (__AVR_AT86RF401__)
  cmdMessenger.sendCmdArg("__AVR_AT86RF401__");
  #elif defined (__AVR_AT90PWM1__)
  cmdMessenger.sendCmdArg("__AVR_AT90PWM1__");
  #elif defined (__AVR_AT90PWM2__)
  cmdMessenger.sendCmdArg("__AVR_AT90PWM2__");
  #elif defined (__AVR_AT90PWM2B__)
  cmdMessenger.sendCmdArg("__AVR_AT90PWM2B__");
  #elif defined (__AVR_AT90PWM3__)
  cmdMessenger.sendCmdArg("__AVR_AT90PWM3__");
  #elif defined (__AVR_AT90PWM3B__)
  cmdMessenger.sendCmdArg("__AVR_AT90PWM3B__");
  #elif defined (__AVR_AT90PWM216__)
  cmdMessenger.sendCmdArg("__AVR_AT90PWM216__");
  #elif defined (__AVR_AT90PWM316__)
  cmdMessenger.sendCmdArg("__AVR_AT90PWM316__");
  #elif defined (__AVR_AT90PWM161__)
  cmdMessenger.sendCmdArg("__AVR_AT90PWM161__");
  #elif defined (__AVR_AT90PWM81__)
  cmdMessenger.sendCmdArg("__AVR_AT90PWM81__");
  #elif defined (__AVR_ATmega8U2__)
  cmdMessenger.sendCmdArg("__AVR_ATmega8U2__");
  #elif defined (__AVR_ATmega16M1__)
  cmdMessenger.sendCmdArg("__AVR_ATmega16M1__");
  #elif defined (__AVR_ATmega16U2__)
  cmdMessenger.sendCmdArg("__AVR_ATmega16U2__");
  #elif defined (__AVR_ATmega16U4__)
  cmdMessenger.sendCmdArg("__AVR_ATmega16U4__");
  #elif defined (__AVR_ATmega32C1__)
  cmdMessenger.sendCmdArg("__AVR_ATmega32C1__");
  #elif defined (__AVR_ATmega32M1__)
  cmdMessenger.sendCmdArg("__AVR_ATmega32M1__");
  #elif defined (__AVR_ATmega32U2__)
  cmdMessenger.sendCmdArg("__AVR_ATmega32U2__");
  #elif defined (__AVR_ATmega32U4__)
  cmdMessenger.sendCmdArg("__AVR_ATmega32U4__");
  #elif defined (__AVR_ATmega32U6__)
  cmdMessenger.sendCmdArg("__AVR_ATmega32U6__");
  #elif defined (__AVR_ATmega64C1__)
  cmdMessenger.sendCmdArg("__AVR_ATmega64C1__");
  #elif defined (__AVR_ATmega64M1__)
  cmdMessenger.sendCmdArg("__AVR_ATmega64M1__");
  #elif defined (__AVR_ATmega128__)
  cmdMessenger.sendCmdArg("__AVR_ATmega128__");
  #elif defined (__AVR_ATmega128A__)
  cmdMessenger.sendCmdArg("__AVR_ATmega128A__");
  #elif defined (__AVR_ATmega1280__)
  cmdMessenger.sendCmdArg("__AVR_ATmega1280__");
  #elif defined (__AVR_ATmega1281__)
  cmdMessenger.sendCmdArg("__AVR_ATmega1281__");
  #elif defined (__AVR_ATmega1284__)
  cmdMessenger.sendCmdArg("__AVR_ATmega1284__");
  #elif defined (__AVR_ATmega1284P__)
  cmdMessenger.sendCmdArg("__AVR_ATmega1284P__");
  #elif defined (__AVR_ATmega128RFA1__)
  cmdMessenger.sendCmdArg("__AVR_ATmega128RFA1__");
  #elif defined (__AVR_ATmega128RFR2__)
  cmdMessenger.sendCmdArg("__AVR_ATmega128RFR2__");
  #elif defined (__AVR_ATmega1284RFR2__)
  cmdMessenger.sendCmdArg("__AVR_ATmega1284RFR2__");
  #elif defined (__AVR_ATmega256RFR2__)
  cmdMessenger.sendCmdArg("__AVR_ATmega256RFR2__");
  #elif defined (__AVR_ATmega2564RFR2__)
  cmdMessenger.sendCmdArg("__AVR_ATmega2564RFR2__");
  #elif defined (__AVR_ATmega2560__)
  cmdMessenger.sendCmdArg("__AVR_ATmega2560__");
  #elif defined (__AVR_ATmega2561__)
  cmdMessenger.sendCmdArg("__AVR_ATmega2561__");
  #elif defined (__AVR_AT90CAN32__)
  cmdMessenger.sendCmdArg("__AVR_AT90CAN32__");
  #elif defined (__AVR_AT90CAN64__)
  cmdMessenger.sendCmdArg("__AVR_AT90CAN64__");
  #elif defined (__AVR_AT90CAN128__)
  cmdMessenger.sendCmdArg("__AVR_AT90CAN128__");
  #elif defined (__AVR_AT90USB82__)
  cmdMessenger.sendCmdArg("__AVR_AT90USB82__");
  #elif defined (__AVR_AT90USB162__)
  cmdMessenger.sendCmdArg("__AVR_AT90USB162__");
  #elif defined (__AVR_AT90USB646__)
  cmdMessenger.sendCmdArg("__AVR_AT90USB646__");
  #elif defined (__AVR_AT90USB647__)
  cmdMessenger.sendCmdArg("__AVR_AT90USB647__");
  #elif defined (__AVR_AT90USB1286__)
  cmdMessenger.sendCmdArg("__AVR_AT90USB1286__");
  #elif defined (__AVR_AT90USB1287__)
  cmdMessenger.sendCmdArg("__AVR_AT90USB1287__");
  #elif defined (__AVR_ATmega64RFR2__)
  cmdMessenger.sendCmdArg("__AVR_ATmega64RFR2__");
  #elif defined (__AVR_ATmega644RFR2__)
  cmdMessenger.sendCmdArg("__AVR_ATmega644RFR2__");
  #elif defined (__AVR_ATmega64__)
  cmdMessenger.sendCmdArg("__AVR_ATmega64__");
  #elif defined (__AVR_ATmega64A__)
  cmdMessenger.sendCmdArg("__AVR_ATmega64A__");
  #elif defined (__AVR_ATmega640__)
  cmdMessenger.sendCmdArg("__AVR_ATmega640__");
  #elif defined (__AVR_ATmega644__)
  cmdMessenger.sendCmdArg("__AVR_ATmega644__");
  #elif (defined __AVR_ATmega644A__)
  cmdMessenger.sendCmdArg("__AVR_ATmega644A__");
  #elif defined (__AVR_ATmega644P__)
  cmdMessenger.sendCmdArg("__AVR_ATmega644P__");
  #elif defined (__AVR_ATmega644PA__)
  cmdMessenger.sendCmdArg("__AVR_ATmega644PA__");
  #elif defined (__AVR_ATmega645__)
  cmdMessenger.sendCmdArg("__AVR_ATmega645__");
  #elif (defined __AVR_ATmega645A__)
  cmdMessenger.sendCmdArg("__AVR_ATmega645A__");
  #elif (defined __AVR_ATmega645P__)
  cmdMessenger.sendCmdArg("__AVR_ATmega645P__");
  #elif defined (__AVR_ATmega6450__)
  cmdMessenger.sendCmdArg("__AVR_ATmega6450__");
  #elif (defined __AVR_ATmega6450A__)
  cmdMessenger.sendCmdArg("__AVR_ATmega6450A__");
  #elif (defined __AVR_ATmega6450P__)
  cmdMessenger.sendCmdArg("__AVR_ATmega6450P__");
  #elif defined (__AVR_ATmega649__)
  cmdMessenger.sendCmdArg("__AVR_ATmega649__");
  #elif (defined __AVR_ATmega649A__)
  cmdMessenger.sendCmdArg("__AVR_ATmega649A__");
  #elif defined (__AVR_ATmega6490__)
  cmdMessenger.sendCmdArg("__AVR_ATmega6490__");
  #elif (defined __AVR_ATmega6490A__)
  cmdMessenger.sendCmdArg("__AVR_ATmega6490A__");
  #elif (defined __AVR_ATmega6490P__)
  cmdMessenger.sendCmdArg("__AVR_ATmega6490P__");
  #elif defined (__AVR_ATmega649P__)
  cmdMessenger.sendCmdArg("__AVR_ATmega649P__");
  #elif defined (__AVR_ATmega64HVE__)
  cmdMessenger.sendCmdArg("__AVR_ATmega64HVE__");
  #elif defined (__AVR_ATmega64HVE2__)
  cmdMessenger.sendCmdArg("__AVR_ATmega64HVE2__");
  #elif defined (__AVR_ATmega103__)
  cmdMessenger.sendCmdArg("__AVR_ATmega103__");
  #elif defined (__AVR_ATmega32__)
  cmdMessenger.sendCmdArg("__AVR_ATmega32__");
  #elif defined (__AVR_ATmega32A__)
  cmdMessenger.sendCmdArg("__AVR_ATmega32A__");
  #elif defined (__AVR_ATmega323__)
  cmdMessenger.sendCmdArg("__AVR_ATmega323__");
  #elif defined (__AVR_ATmega324P__)
  cmdMessenger.sendCmdArg("__AVR_ATmega324P__");
  #elif (defined __AVR_ATmega324A__)
  cmdMessenger.sendCmdArg("__AVR_ATmega324A__");
  #elif defined (__AVR_ATmega324PA__)
  cmdMessenger.sendCmdArg("__AVR_ATmega324PA__");
  #elif defined (__AVR_ATmega325__)
  cmdMessenger.sendCmdArg("__AVR_ATmega325__");
  #elif (defined __AVR_ATmega325A__)
  cmdMessenger.sendCmdArg("__AVR_ATmega325A__");
  #elif defined (__AVR_ATmega325P__)
  cmdMessenger.sendCmdArg("__AVR_ATmega325P__");
  #elif defined (__AVR_ATmega325PA__)
  cmdMessenger.sendCmdArg("__AVR_ATmega325PA__");
  #elif defined (__AVR_ATmega3250__)
  cmdMessenger.sendCmdArg("__AVR_ATmega3250__");
  #elif (defined __AVR_ATmega3250A__)
  cmdMessenger.sendCmdArg("__AVR_ATmega3250A__");
  #elif defined (__AVR_ATmega3250P__)
  cmdMessenger.sendCmdArg("__AVR_ATmega3250P__");
  #elif defined (__AVR_ATmega3250PA__)
  cmdMessenger.sendCmdArg("__AVR_ATmega3250PA__");
  #elif defined (__AVR_ATmega328P__)
  cmdMessenger.sendCmdArg("__AVR_ATmega328P__");
  #elif (defined __AVR_ATmega328__)
  cmdMessenger.sendCmdArg("__AVR_ATmega328__");
  #elif defined (__AVR_ATmega329__)
  cmdMessenger.sendCmdArg("__AVR_ATmega329__");
  #elif (defined __AVR_ATmega329A__)
  cmdMessenger.sendCmdArg("__AVR_ATmega329A__");
  #elif defined (__AVR_ATmega329P__)
  cmdMessenger.sendCmdArg("__AVR_ATmega329P__");
  #elif (defined __AVR_ATmega329PA__)
  cmdMessenger.sendCmdArg("__AVR_ATmega329PA__");
  #elif (defined __AVR_ATmega3290PA__)
  cmdMessenger.sendCmdArg("__AVR_ATmega3290PA__");
  #elif defined (__AVR_ATmega3290__)
  cmdMessenger.sendCmdArg("__AVR_ATmega3290__");
  #elif (defined __AVR_ATmega3290A__)
  cmdMessenger.sendCmdArg("__AVR_ATmega3290A__");
  #elif defined (__AVR_ATmega3290P__)
  cmdMessenger.sendCmdArg("__AVR_ATmega3290P__");
  #elif defined (__AVR_ATmega32HVB__)
  cmdMessenger.sendCmdArg("__AVR_ATmega32HVB__");
  #elif defined (__AVR_ATmega32HVBREVB__)
  cmdMessenger.sendCmdArg("__AVR_ATmega32HVBREVB__");
  #elif defined (__AVR_ATmega406__)
  cmdMessenger.sendCmdArg("__AVR_ATmega406__");
  #elif defined (__AVR_ATmega16__)
  cmdMessenger.sendCmdArg("__AVR_ATmega16__");
  #elif defined (__AVR_ATmega16A__)
  cmdMessenger.sendCmdArg("__AVR_ATmega16A__");
  #elif defined (__AVR_ATmega161__)
  cmdMessenger.sendCmdArg("__AVR_ATmega161__");
  #elif defined (__AVR_ATmega162__)
  cmdMessenger.sendCmdArg("__AVR_ATmega162__");
  #elif defined (__AVR_ATmega163__)
  cmdMessenger.sendCmdArg("__AVR_ATmega163__");
  #elif defined (__AVR_ATmega164P__)
  cmdMessenger.sendCmdArg("__AVR_ATmega164P__");
  #elif (defined __AVR_ATmega164A__)
  cmdMessenger.sendCmdArg("__AVR_ATmega164A__");
  #elif defined (__AVR_ATmega164PA__)
  cmdMessenger.sendCmdArg("__AVR_ATmega164PA__");
  #elif defined (__AVR_ATmega165__)
  cmdMessenger.sendCmdArg("__AVR_ATmega165__");
  #elif (defined __AVR_ATmega165A__)
  cmdMessenger.sendCmdArg("__AVR_ATmega165A__");
  #elif defined (__AVR_ATmega165P__)
  cmdMessenger.sendCmdArg("__AVR_ATmega165P__");
  #elif defined (__AVR_ATmega165PA__)
  cmdMessenger.sendCmdArg("__AVR_ATmega165PA__");
  #elif defined (__AVR_ATmega168__)
  cmdMessenger.sendCmdArg("__AVR_ATmega168__");
  #elif (defined __AVR_ATmega168A__)
  cmdMessenger.sendCmdArg("__AVR_ATmega168A__");
  #elif defined (__AVR_ATmega168P__)
  cmdMessenger.sendCmdArg("__AVR_ATmega168P__");
  #elif defined (__AVR_ATmega168PA__)
  cmdMessenger.sendCmdArg("__AVR_ATmega168PA__");
  #elif defined (__AVR_ATmega168PB__)
  cmdMessenger.sendCmdArg("__AVR_ATmega168PB__");
  #elif defined (__AVR_ATmega169__)
  cmdMessenger.sendCmdArg("__AVR_ATmega169__");
  #elif (defined __AVR_ATmega169A__)
  cmdMessenger.sendCmdArg("__AVR_ATmega169A__");
  #elif defined (__AVR_ATmega169P__)
  cmdMessenger.sendCmdArg("__AVR_ATmega169P__");
  #elif defined (__AVR_ATmega169PA__)
  cmdMessenger.sendCmdArg("__AVR_ATmega169PA__");
  #elif defined (__AVR_ATmega8HVA__)
  cmdMessenger.sendCmdArg("__AVR_ATmega8HVA__");
  #elif defined (__AVR_ATmega16HVA__)
  cmdMessenger.sendCmdArg("__AVR_ATmega16HVA__");
  #elif defined (__AVR_ATmega16HVA2__)
  cmdMessenger.sendCmdArg("__AVR_ATmega16HVA2__");
  #elif defined (__AVR_ATmega16HVB__)
  cmdMessenger.sendCmdArg("__AVR_ATmega16HVB__");
  #elif defined (__AVR_ATmega16HVBREVB__)
  cmdMessenger.sendCmdArg("__AVR_ATmega16HVBREVB__");
  #elif defined (__AVR_ATmega8__)
  cmdMessenger.sendCmdArg("__AVR_ATmega8__");
  #elif defined (__AVR_ATmega8A__)
  cmdMessenger.sendCmdArg("__AVR_ATmega8A__");
  #elif (defined __AVR_ATmega48A__)
  cmdMessenger.sendCmdArg("__AVR_ATmega48A__");
  #elif defined (__AVR_ATmega48__)
  cmdMessenger.sendCmdArg("__AVR_ATmega48__");
  #elif defined (__AVR_ATmega48PA__)
  cmdMessenger.sendCmdArg("__AVR_ATmega48PA__");
  #elif defined (__AVR_ATmega48PB__)
  cmdMessenger.sendCmdArg("__AVR_ATmega48PB__");
  #elif defined (__AVR_ATmega48P__)
  cmdMessenger.sendCmdArg("__AVR_ATmega48P__");
  #elif defined (__AVR_ATmega88__)
  cmdMessenger.sendCmdArg("__AVR_ATmega88__");
  #elif (defined __AVR_ATmega88A__)
  cmdMessenger.sendCmdArg("__AVR_ATmega88A__");
  #elif defined (__AVR_ATmega88P__)
  cmdMessenger.sendCmdArg("__AVR_ATmega88P__");
  #elif defined (__AVR_ATmega88PA__)
  cmdMessenger.sendCmdArg("__AVR_ATmega88PA__");
  #elif defined (__AVR_ATmega88PB__)
  cmdMessenger.sendCmdArg("__AVR_ATmega88PB__");
  #elif defined (__AVR_ATmega8515__)
  cmdMessenger.sendCmdArg("__AVR_ATmega8515__");
  #elif defined (__AVR_ATmega8535__)
  cmdMessenger.sendCmdArg("__AVR_ATmega8535__");
  #elif defined (__AVR_AT90S8535__)
  cmdMessenger.sendCmdArg("__AVR_AT90S8535__");
  #elif defined (__AVR_AT90C8534__)
  cmdMessenger.sendCmdArg("__AVR_AT90C8534__");
  #elif defined (__AVR_AT90S8515__)
  cmdMessenger.sendCmdArg("__AVR_AT90S8515__");
  #elif defined (__AVR_AT90S4434__)
  cmdMessenger.sendCmdArg("__AVR_AT90S4434__");
  #elif defined (__AVR_AT90S4433__)
  cmdMessenger.sendCmdArg("__AVR_AT90S4433__");
  #elif defined (__AVR_AT90S4414__)
  cmdMessenger.sendCmdArg("__AVR_AT90S4414__");
  #elif defined (__AVR_ATtiny22__)
  cmdMessenger.sendCmdArg("__AVR_ATtiny22__");
  #elif defined (__AVR_ATtiny26__)
  cmdMessenger.sendCmdArg("__AVR_ATtiny26__");
  #elif defined (__AVR_AT90S2343__)
  cmdMessenger.sendCmdArg("__AVR_AT90S2343__");
  #elif defined (__AVR_AT90S2333__)
  cmdMessenger.sendCmdArg("__AVR_AT90S2333__");
  #elif defined (__AVR_AT90S2323__)
  cmdMessenger.sendCmdArg("__AVR_AT90S2323__");
  #elif defined (__AVR_AT90S2313__)
  cmdMessenger.sendCmdArg("__AVR_AT90S2313__");
  #elif defined (__AVR_ATtiny4__)
  cmdMessenger.sendCmdArg("__AVR_ATtiny4__");
  #elif defined (__AVR_ATtiny5__)
  cmdMessenger.sendCmdArg("__AVR_ATtiny5__");
  #elif defined (__AVR_ATtiny9__)
  cmdMessenger.sendCmdArg("__AVR_ATtiny9__");
  #elif defined (__AVR_ATtiny10__)
  cmdMessenger.sendCmdArg("__AVR_ATtiny10__");
  #elif defined (__AVR_ATtiny20__)
  cmdMessenger.sendCmdArg("__AVR_ATtiny20__");
  #elif defined (__AVR_ATtiny40__)
  cmdMessenger.sendCmdArg("__AVR_ATtiny40__");
  #elif defined (__AVR_ATtiny2313__)
  cmdMessenger.sendCmdArg("__AVR_ATtiny2313__");
  #elif defined (__AVR_ATtiny2313A__)
  cmdMessenger.sendCmdArg("__AVR_ATtiny2313A__");
  #elif defined (__AVR_ATtiny13__)
  cmdMessenger.sendCmdArg("__AVR_ATtiny13__");
  #elif defined (__AVR_ATtiny13A__)
  cmdMessenger.sendCmdArg("__AVR_ATtiny13A__");
  #elif defined (__AVR_ATtiny25__)
  cmdMessenger.sendCmdArg("__AVR_ATtiny25__");
  #elif defined (__AVR_ATtiny4313__)
  cmdMessenger.sendCmdArg("__AVR_ATtiny4313__");
  #elif defined (__AVR_ATtiny45__)
  cmdMessenger.sendCmdArg("__AVR_ATtiny45__");
  #elif defined (__AVR_ATtiny85__)
  cmdMessenger.sendCmdArg("__AVR_ATtiny85__");
  #elif defined (__AVR_ATtiny24__)
  cmdMessenger.sendCmdArg("__AVR_ATtiny24__");
  #elif defined (__AVR_ATtiny24A__)
  cmdMessenger.sendCmdArg("__AVR_ATtiny24A__");
  #elif defined (__AVR_ATtiny44__)
  cmdMessenger.sendCmdArg("__AVR_ATtiny44__");
  #elif defined (__AVR_ATtiny44A__)
  cmdMessenger.sendCmdArg("__AVR_ATtiny44A__");
  #elif defined (__AVR_ATtiny441__)
  cmdMessenger.sendCmdArg("__AVR_ATtiny441__");
  #elif defined (__AVR_ATtiny84__)
  cmdMessenger.sendCmdArg("__AVR_ATtiny84__");
  #elif defined (__AVR_ATtiny84A__)
  cmdMessenger.sendCmdArg("__AVR_ATtiny84A__");
  #elif defined (__AVR_ATtiny841__)
  cmdMessenger.sendCmdArg("__AVR_ATtiny841__");
  #elif defined (__AVR_ATtiny261__)
  cmdMessenger.sendCmdArg("__AVR_ATtiny261__");
  #elif defined (__AVR_ATtiny261A__)
  cmdMessenger.sendCmdArg("__AVR_ATtiny261A__");
  #elif defined (__AVR_ATtiny461__)
  cmdMessenger.sendCmdArg("__AVR_ATtiny461__");
  #elif defined (__AVR_ATtiny461A__)
  cmdMessenger.sendCmdArg("__AVR_ATtiny461A__");
  #elif defined (__AVR_ATtiny861__)
  cmdMessenger.sendCmdArg("__AVR_ATtiny861__");
  #elif defined (__AVR_ATtiny861A__)
  cmdMessenger.sendCmdArg("__AVR_ATtiny861A__");
  #elif defined (__AVR_ATtiny43U__)
  cmdMessenger.sendCmdArg("__AVR_ATtiny43U__");
  #elif defined (__AVR_ATtiny48__)
  cmdMessenger.sendCmdArg("__AVR_ATtiny48__");
  #elif defined (__AVR_ATtiny88__)
  cmdMessenger.sendCmdArg("__AVR_ATtiny88__");
  #elif defined (__AVR_ATtiny828__)
  cmdMessenger.sendCmdArg("__AVR_ATtiny828__");
  #elif defined (__AVR_ATtiny87__)
  cmdMessenger.sendCmdArg("__AVR_ATtiny87__");
  #elif defined (__AVR_ATtiny167__)
  cmdMessenger.sendCmdArg("__AVR_ATtiny167__");
  #elif defined (__AVR_ATtiny1634__)
  cmdMessenger.sendCmdArg("__AVR_ATtiny1634__");
  #elif defined (__AVR_AT90SCR100__)
  cmdMessenger.sendCmdArg("__AVR_AT90SCR100__");
  #elif defined (__AVR_ATxmega16A4__)
  cmdMessenger.sendCmdArg("__AVR_ATxmega16A4__");
  #elif defined (__AVR_ATxmega16A4U__)
  cmdMessenger.sendCmdArg("__AVR_ATxmega16A4U__");
  #elif defined (__AVR_ATxmega16C4__)
  cmdMessenger.sendCmdArg("__AVR_ATxmega16C4__");
  #elif defined (__AVR_ATxmega16D4__)
  cmdMessenger.sendCmdArg("__AVR_ATxmega16D4__");
  #elif defined (__AVR_ATxmega32A4__)
  cmdMessenger.sendCmdArg("__AVR_ATxmega32A4__");
  #elif defined (__AVR_ATxmega32A4U__)
  cmdMessenger.sendCmdArg("__AVR_ATxmega32A4U__");
  #elif defined (__AVR_ATxmega32C3__)
  cmdMessenger.sendCmdArg("__AVR_ATxmega32C3__");
  #elif defined (__AVR_ATxmega32C4__)
  cmdMessenger.sendCmdArg("__AVR_ATxmega32C4__");
  #elif defined (__AVR_ATxmega32D3__)
  cmdMessenger.sendCmdArg("__AVR_ATxmega32D3__");
  #elif defined (__AVR_ATxmega32D4__)
  cmdMessenger.sendCmdArg("__AVR_ATxmega32D4__");
  #elif defined (__AVR_ATxmega8E5__)
  cmdMessenger.sendCmdArg("__AVR_ATxmega8E5__");
  #elif defined (__AVR_ATxmega16E5__)
  cmdMessenger.sendCmdArg("__AVR_ATxmega16E5__");
  #elif defined (__AVR_ATxmega32E5__)
  cmdMessenger.sendCmdArg("__AVR_ATxmega32E5__");
  #elif defined (__AVR_ATxmega64A1__)
  cmdMessenger.sendCmdArg("__AVR_ATxmega64A1__");
  #elif defined (__AVR_ATxmega64A1U__)
  cmdMessenger.sendCmdArg("__AVR_ATxmega64A1U__");
  #elif defined (__AVR_ATxmega64A3__)
  cmdMessenger.sendCmdArg("__AVR_ATxmega64A3__");
  #elif defined (__AVR_ATxmega64A3U__)
  cmdMessenger.sendCmdArg("__AVR_ATxmega64A3U__");
  #elif defined (__AVR_ATxmega64A4U__)
  cmdMessenger.sendCmdArg("__AVR_ATxmega64A4U__");
  #elif defined (__AVR_ATxmega64B1__)
  cmdMessenger.sendCmdArg("__AVR_ATxmega64B1__");
  #elif defined (__AVR_ATxmega64B3__)
  cmdMessenger.sendCmdArg("__AVR_ATxmega64B3__");
  #elif defined (__AVR_ATxmega64C3__)
  cmdMessenger.sendCmdArg("__AVR_ATxmega64C3__");
  #elif defined (__AVR_ATxmega64D3__)
  cmdMessenger.sendCmdArg("__AVR_ATxmega64D3__");
  #elif defined (__AVR_ATxmega64D4__)
  cmdMessenger.sendCmdArg("__AVR_ATxmega64D4__");
  #elif defined (__AVR_ATxmega128A1__)
  cmdMessenger.sendCmdArg("__AVR_ATxmega128A1__");
  #elif defined (__AVR_ATxmega128A1U__)
  cmdMessenger.sendCmdArg("__AVR_ATxmega128A1U__");
  #elif defined (__AVR_ATxmega128A4U__)
  cmdMessenger.sendCmdArg("__AVR_ATxmega128A4U__");
  #elif defined (__AVR_ATxmega128A3__)
  cmdMessenger.sendCmdArg("__AVR_ATxmega128A3__");
  #elif defined (__AVR_ATxmega128A3U__)
  cmdMessenger.sendCmdArg("__AVR_ATxmega128A3U__");
  #elif defined (__AVR_ATxmega128B1__)
  cmdMessenger.sendCmdArg("__AVR_ATxmega128B1__");
  #elif defined (__AVR_ATxmega128B3__)
  cmdMessenger.sendCmdArg("__AVR_ATxmega128B3__");
  #elif defined (__AVR_ATxmega128C3__)
  cmdMessenger.sendCmdArg("__AVR_ATxmega128C3__");
  #elif defined (__AVR_ATxmega128D3__)
  cmdMessenger.sendCmdArg("__AVR_ATxmega128D3__");
  #elif defined (__AVR_ATxmega128D4__)
  cmdMessenger.sendCmdArg("__AVR_ATxmega128D4__");
  #elif defined (__AVR_ATxmega192A3__)
  cmdMessenger.sendCmdArg("__AVR_ATxmega192A3__");
  #elif defined (__AVR_ATxmega192A3U__)
  cmdMessenger.sendCmdArg("__AVR_ATxmega192A3U__");
  #elif defined (__AVR_ATxmega192C3__)
  cmdMessenger.sendCmdArg("__AVR_ATxmega192C3__");
  #elif defined (__AVR_ATxmega192D3__)
  cmdMessenger.sendCmdArg("__AVR_ATxmega192D3__");
  #elif defined (__AVR_ATxmega256A3__)
  cmdMessenger.sendCmdArg("__AVR_ATxmega256A3__");
  #elif defined (__AVR_ATxmega256A3U__)
  cmdMessenger.sendCmdArg("__AVR_ATxmega256A3U__");
  #elif defined (__AVR_ATxmega256A3B__)
  cmdMessenger.sendCmdArg("__AVR_ATxmega256A3B__");
  #elif defined (__AVR_ATxmega256A3BU__)
  cmdMessenger.sendCmdArg("__AVR_ATxmega256A3BU__");
  #elif defined (__AVR_ATxmega256C3__)
  cmdMessenger.sendCmdArg("__AVR_ATxmega256C3__");
  #elif defined (__AVR_ATxmega256D3__)
  cmdMessenger.sendCmdArg("__AVR_ATxmega256D3__");
  #elif defined (__AVR_ATxmega384C3__)
  cmdMessenger.sendCmdArg("__AVR_ATxmega384C3__");
  #elif defined (__AVR_ATxmega384D3__)
  cmdMessenger.sendCmdArg("__AVR_ATxmega384D3__");
  #elif defined (__AVR_ATA5790__)
  cmdMessenger.sendCmdArg("__AVR_ATA5790__");
  #elif defined (__AVR_ATA5790N__)
  cmdMessenger.sendCmdArg("__AVR_ATA5790N__");
  #elif defined (__AVR_ATA5272__)
  cmdMessenger.sendCmdArg("__AVR_ATA5272__");
  #elif defined (__AVR_ATA5505__)
  cmdMessenger.sendCmdArg("__AVR_ATA5505__");
  #elif defined (__AVR_ATA5795__)
  cmdMessenger.sendCmdArg("__AVR_ATA5795__");
  #elif defined (__AVR_ATA5702M322__)
  cmdMessenger.sendCmdArg("__AVR_ATA5702M322__");
  #elif defined (__AVR_ATA5782__)
  cmdMessenger.sendCmdArg("__AVR_ATA5782__");
  #elif defined (__AVR_ATA5831__)
  cmdMessenger.sendCmdArg("__AVR_ATA5831__");
  #elif defined (__AVR_ATA6285__)
  cmdMessenger.sendCmdArg("__AVR_ATA6285__");
  #elif defined (__AVR_ATA6286__)
  cmdMessenger.sendCmdArg("__AVR_ATA6286__");
  #elif defined (__AVR_ATA6289__)
  cmdMessenger.sendCmdArg("__AVR_ATA6289__");
  #elif defined (__AVR_ATA6612C__)
  cmdMessenger.sendCmdArg("__AVR_ATA6612C__");
  #elif defined (__AVR_ATA6613C__)
  cmdMessenger.sendCmdArg("__AVR_ATA6613C__");
  #elif defined (__AVR_ATA6614Q__)
  cmdMessenger.sendCmdArg("__AVR_ATA6614Q__");
  #elif defined (__AVR_ATA6616C__)
  cmdMessenger.sendCmdArg("__AVR_ATA6616C__");
  #elif defined (__AVR_ATA6617C__)
  cmdMessenger.sendCmdArg("__AVR_ATA6617C__");
  #elif defined (__AVR_ATA664251__)
  cmdMessenger.sendCmdArg("__AVR_ATA664251__");
  #elif defined (__AVR_ATtiny28__)
  cmdMessenger.sendCmdArg("__AVR_ATtiny28__");
  #elif defined (__AVR_AT90S1200__)
  cmdMessenger.sendCmdArg("__AVR_AT90S1200__");
  #elif defined (__AVR_ATtiny15__)
  cmdMessenger.sendCmdArg("__AVR_ATtiny15__");
  #elif defined (__AVR_ATtiny12__)
  cmdMessenger.sendCmdArg("__AVR_ATtiny12__");
  #elif defined (__AVR_M3000__)
  cmdMessenger.sendCmdArg("__AVR_M3000__");
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

