using System;
using System.Xml.Serialization;
using System.IO;

namespace PrototypeBackend
{
	public class APin : IPin
	{
		public PrototypeBackend.PinType PinType { get; set; }

		public PrototypeBackend.PinMode PinMode { get; set; }

		public string PinLabel { get; set; }

		public string Unit { get; set; }

		public int PinNr{ get; set; }

		public int PinValue { get; set; }

		[XmlIgnore]
		public Action PinCmd{ get; set; }

		public APin ()
		{
			PinType = PrototypeBackend.PinType.ANALOG;
		}

		public override bool Equals (object obj)
		{
			if (obj != null)
			{
				if (obj is APin)
				{
					return (obj as APin).PinType == PinType &&
					(obj as APin).PinMode == PinMode &&
					(obj as APin).PinLabel.Equals (PinLabel) &&
					(obj as APin).Unit.Equals (Unit) &&
					(obj as APin).PinNr.Equals (PinNr);
				}
			}
			return false;
		}

		public override int GetHashCode ()
		{
			return base.GetHashCode ();
		}

		public override string ToString ()
		{
			return String.Format ("Label: {0}\tNumber: {1}\tPinType: {2}\tUnit: {3}", PinLabel, PinNr, PinType, Unit);
		}

		public string ToXML ()
		{
			XmlSerializer tmp = new XmlSerializer (typeof(APin));
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
				PrototypeBackend.ArduinoController.SetAnalogPin (PinNr, PinValue);
				break;
			case PrototypeBackend.PinMode.INPUT:
				PrototypeBackend.ArduinoController.ReadAnalogPin (PinNr);
				break;
			}
		}

	}
}

