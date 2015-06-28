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

		public System.Drawing.Color PinColor { get; set; }

		public int PinValue { get; set; }

		public APin ()
		{
			PinType = PrototypeBackend.PinType.ANALOG;
			PinMode = PrototypeBackend.PinMode.INPUT;
			PinLabel = "";
			PinNr = -1;
			PinColor = System.Drawing.Color.Blue;
			PinValue = 0;
			Unit = "";
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
					(obj as APin).PinNr.Equals (PinNr) &&
					(obj as APin).PinColor.Equals (PinColor);
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
			return String.Format ("Label: {0}\tNumber: {1}\tPinType: {2}\tUnit: {3}\tColor: {4}", PinLabel, PinNr, PinType, Unit, PinColor);
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

