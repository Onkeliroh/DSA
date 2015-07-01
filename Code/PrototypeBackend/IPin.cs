using System;
using System.Drawing;

namespace PrototypeBackend
{
	public interface IPin
	{
		string Name{ get; set; }

		int Number{ get; set; }

		System.Drawing.Color PlotColor{ get; set; }

		PrototypeBackend.PinType Type { get; set; }

		PrototypeBackend.PinMode Mode { get; set; }

		//Methods

		string ToString ();
	}
}

