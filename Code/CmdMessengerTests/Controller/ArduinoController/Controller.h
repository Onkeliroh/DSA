#ifndef Controller_h
#define Controller_h

#include "CmdMessenger.h"

extern "C"
{
	// callback functions always follow the signature: void cmd(void);
	typedef void(*messengerCallbackFunction) (void);
}

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

class Controller
{

  private:
    CmdMessenger cmdMessenger = CmdMessenger(Serial);

  public :
    Controller();

    // Callbacks define on which received commands we take action
    void attachCommandCallbacks();

    void Controll();

    // Called when a received command has no attached function
    void OnUnknownCommand();

    void OnSetPin();

    void OnSetPinMode();

    void OnSetPinState();

    void OnReadAnalogPin();

    void OnSetAnalogPin();

    void OnSetAnalogPinMode();


};

#endif
