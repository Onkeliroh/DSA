using System;
using ArduinoController;
using System.Xml.Serialization;
using System.IO;

namespace PrototypeBackend
{
	public class Sequence : IPin
	{
		public PinType? PinType { get; set; }

		public PinMode? PinMode { get; set; }

		public string PinLabel { get; set; }

		public int PinNr { get ; set; }

		public ArduinoController.DPinState PinState = ArduinoController.DPinState.LOW;

		public Action PinCmd{ get; set; }

		public Sequence ()
		{
			PinType = ArduinoController.PinType.DIGITAL;
		}

		public Sequence (string label, DateTime time, int pinnr)
		{
			PinLabel = label;
			PinNr = pinnr;
		}

		public override bool Equals (object obj)
		{
			var seq = obj as Sequence;
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
			XmlSerializer tmp = new XmlSerializer (typeof(Sequence));
			string returnstring = "";
			TextWriter tw = new StreamWriter (returnstring);
			tmp.Serialize (tw, this);
			tw.Close ();
			return returnstring;
		}
	}
}

