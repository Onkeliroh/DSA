#include "BachelorController.h"
#include "CmdMessenger.h"

BachelorController::BachelorController()
{
	cmdMessenger = CmdMessenger(Serial);
}
