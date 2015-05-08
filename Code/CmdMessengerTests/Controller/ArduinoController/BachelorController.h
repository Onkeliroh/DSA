#ifndef BACHELORCONTROLLER_H
#define BACHELORCONTROLLER_H

#include "CmdMessenger.h"

class BachelorController
{
	private:
		CmdMessenger cmdMessenger = CmdMessenger(Serial);
	
	public:
		BachelorController();
		
		
};
#endif
