using System;

namespace PrototypeBackend
{
	public interface IPin
	{
		string PinLabel{ get; set; }

		int PinNr{ get; set; }

		string ToString ();

		Action PinCmd{ get; set; }

		ArduinoController.PinType? PinType { get; set; }

		ArduinoController.PinMode? PinMode { get; set; }
	}
}

