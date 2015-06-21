using System;
using System.Xml.Serialization;
using System.IO;

namespace PrototypeBackend
{
	public class MeasurementData : IPin
	{
		public ArduinoController.PinType? PinType { get; set; }

		public ArduinoController.PinMode? PinMode { get; set; }

		public string PinLabel { get; set; }

		public string Unit { get; set; }

		public int PinNr{ get; set; }

		[XmlIgnore]
		public Action PinCmd{ get; set; }

		public MeasurementData ()
		{
			PinType = ArduinoController.PinType.ANALOG;
		}

		public override bool Equals (object obj)
		{
			if (obj != null)
			{
				if (obj is MeasurementData)
				{
					return (obj as MeasurementData).PinType == PinType &&
					(obj as MeasurementData).PinMode == PinMode &&
					(obj as MeasurementData).PinLabel.Equals (PinLabel) &&
					(obj as MeasurementData).Unit.Equals (Unit) &&
					(obj as MeasurementData).PinNr.Equals (PinNr);
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
			XmlSerializer tmp = new XmlSerializer (typeof(MeasurementData));
			string returnstring = "";
			TextWriter tw = new StreamWriter (returnstring);
			tmp.Serialize (tw, this);
			tw.Close ();
			return returnstring;
		}

	}
}

