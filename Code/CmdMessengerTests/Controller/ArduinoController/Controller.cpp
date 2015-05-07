#ifndef Controller_h
#include "Controller.h"
#endif

#include "CmdMessenger.h"

Controller::Controller()
{
  Serial.begin(115200);
  attachCommandCallbacks();
  cmdMessenger.sendCmd(kAcknowledge,"Arduino has started!");
}

// Callbacks define on which received commands we take action
void Controller::attachCommandCallbacks()
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

//Process incoming serial data, and perfor callbacks
void Controller::Controll()
{
  cmdMessenger.feedinSerialData();
  delay(10);
}

// Called when a received command has no attached function
void Controller::OnUnknownCommand()
{
  cmdMessenger.sendCmd(kError,"Command without attached callback");
}

void Controller::OnSetPin()
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

void Controller::OnSetPinMode()
{
  int pin = cmdMessenger.readInt16Arg();
  int mode = cmdMessenger.readInt16Arg();
  pinMode(pin,mode);
}

void Controller::OnSetPinState()
{
  int pin = cmdMessenger.readInt16Arg();
  int state = cmdMessenger.readInt16Arg();
  digitalWrite(pin,state);
}

void Controller::OnReadAnalogPin()
{
  int val = cmdMessenger.readInt16Arg();
  cmdMessenger.sendCmdStart(kReadAnalogPinResult);
  cmdMessenger.sendCmdSciArg(val);
  cmdMessenger.sendCmdSciArg(analogRead(val));
  cmdMessenger.sendCmdEnd();
}

void Controller::OnSetAnalogPin()
{
  int Pin = cmdMessenger.readInt16Arg();
  int Val = cmdMessenger.readInt16Arg();
  analogWrite(Pin,Val);
}

void Controller::OnSetAnalogPinMode()
{
  int Pin = cmdMessenger.readInt16Arg();
  pinMode(Pin,cmdMessenger.readInt16Arg());
}
