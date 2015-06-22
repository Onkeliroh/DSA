using System;

namespace PrototypeBackend
{
	public interface IPin
	{
		string PinLabel{ get; set; }

		int PinNr{ get; set; }

		string ToString ();

		Action PinCmd{ get; set; }

		PrototypeBackend.PinType PinType { get; set; }

		PrototypeBackend.PinMode PinMode { get; set; }
	}
}

