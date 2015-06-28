using System;
using System.Drawing;

namespace PrototypeBackend
{
	public interface IPin
	{
		string PinLabel{ get; set; }

		int PinNr{ get; set; }

		System.Drawing.Color PinColor{ get; set; }

		PrototypeBackend.PinType PinType { get; set; }

		PrototypeBackend.PinMode PinMode { get; set; }

		//Methods

		string ToString ();
	}
}

