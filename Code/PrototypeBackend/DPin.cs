using System;
using PrototypeBackend;
using System.Xml.Serialization;
using System.IO;

namespace PrototypeBackend
{
	public class DPin : IPin
	{
		public PinType Type { get; set; }

		public PinMode Mode { get; set; }

		public string Name { get; set; }

		public int Number { get ; set; }

		public System.Drawing.Color PlotColor { get; set; }

		public PrototypeBackend.DPinState State = PrototypeBackend.DPinState.LOW;

		//Constructors

		public DPin ()
		{
			Type = PrototypeBackend.PinType.DIGITAL;
			Mode = PrototypeBackend.PinMode.OUTPUT;
			Name = "";
			Number = -1;
			PlotColor = System.Drawing.Color.Empty;
		}

		public DPin (string label, int pinnr)
		{
			Name = label;
			Number = pinnr;
		}

		//Methods

		public override bool Equals (object obj)
		{
			var seq = obj as DPin;
			if (seq != null)
			{
				return (seq.Number == Number
				&& seq.Name.Equals (Name)
				&& seq.State.Equals (State)
				&& seq.PlotColor.Equals (PlotColor)
				&& seq.Type.Equals (Type)
				&& seq.Mode.Equals (Mode));
			}
			return false;
		}

		public override int GetHashCode ()
		{
			return base.GetHashCode ();
		}

		public override string ToString ()
		{
			return string.Format ("Label: {0}\tNumber: {1}\tType: {2}\tState: {3}", Name, Number, Type, State);
		}

		public string ToXML ()
		{
			XmlSerializer tmp = new XmlSerializer (typeof(DPin));
			string returnstring = "";
			TextWriter tw = new StreamWriter (returnstring);
			tmp.Serialize (tw, this);
			tw.Close ();
			return returnstring;
		}

		public void Run ()
		{
			switch (Mode)
			{
			case PrototypeBackend.PinMode.OUTPUT:
				PrototypeBackend.ArduinoController.SetPin (Number, Mode, State);
				break;
			case PrototypeBackend.PinMode.INPUT:
				State = PrototypeBackend.ArduinoController.ReadPin (Number);
				break;
			}
		}
	}
}

