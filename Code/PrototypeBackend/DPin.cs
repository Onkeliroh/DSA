using System;
using PrototypeBackend;
using System.Xml.Serialization;
using System.IO;

namespace PrototypeBackend
{
	public class DPin : IPin
	{
		public PinType PinType { get; set; }

		public PinMode PinMode { get; set; }

		public string PinLabel { get; set; }

		public int PinNr { get ; set; }

		public PrototypeBackend.DPinState PinState = PrototypeBackend.DPinState.LOW;

		public Action PinCmd{ get; set; }

		//Constructors

		public DPin ()
		{
			PinType = PrototypeBackend.PinType.DIGITAL;
		}

		public DPin (string label, DateTime time, int pinnr)
		{
			PinLabel = label;
			PinNr = pinnr;
		}

		//Methods

		public override bool Equals (object obj)
		{
			var seq = obj as DPin;
			if (seq != null)
			{
				return (seq.PinNr == PinNr
				&& seq.PinLabel.Equals (PinLabel)
				&& seq.PinState.Equals (PinState));
			}
			return false;
		}

		public override int GetHashCode ()
		{
			return base.GetHashCode ();
		}

		public override string ToString ()
		{
			return string.Format ("Label: {0}\tNumber: {1}\tType: {2}\tState: {3}", PinLabel, PinNr, PinType, PinState);
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
			switch (PinMode)
			{
			case PrototypeBackend.PinMode.OUTPUT:
				PrototypeBackend.ArduinoController.SetPin (PinNr, PinMode, PinState);
				break;
			case PrototypeBackend.PinMode.INPUT:
				PinState = PrototypeBackend.ArduinoController.ReadPin (PinNr);
				break;
			}
		}
	}
}

