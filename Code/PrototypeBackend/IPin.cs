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

		//Methods

		string ToString ();

		bool Equals (object obj);
	}
}

