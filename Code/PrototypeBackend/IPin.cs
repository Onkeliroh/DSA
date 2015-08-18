using System.Collections.Generic;

namespace PrototypeBackend
{
	public interface IPin
	{
		string Name{ get; set; }

		int Number{ get; set; }

		Gdk.Color PlotColor{ get; set; }

		PrototypeBackend.PinType Type { get; set; }

		PrototypeBackend.PinMode Mode { get; set; }

		bool SDA { get; set; }

		bool SDC{ get; set; }

		bool RX{ get; set; }

		bool TX{ get; set; }

		//Methods

		string ToString ();

		bool Equals (object obj);
	}
}

