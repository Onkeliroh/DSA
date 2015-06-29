using System;
using System.Xml.Serialization;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace PrototypeBackend
{
	public class APin : IPin
	{
		#region Member

		public PrototypeBackend.PinType PinType { get; set; }

		public PrototypeBackend.PinMode PinMode { get; set; }

		public string PinLabel { get; set; }

		public string Unit { get; set; }

		public int PinNr{ get; set; }

		public System.Drawing.Color PinColor { get; set; }

		public double Gain{ get; set; }

		public double Offset{ get; set; }

		public List<double> Values{ get; private set; }

		public double PinValue {
			private set{ }
			get {
				if (Values.Count >= Interval)
				{
					if (Interval == 1)
					{
						if (!double.IsNaN (Values.Last ()))
						{
							return ((Values.Last () * Gain) + Offset);
						}
						return double.NaN;
					} else
					{
						double result = 0;
						for (int i = Values.Count - Interval; i < Values.Count; i++)
						{
							if (!double.IsNaN (Values [i]))
							{
								result += (Values [i] * Gain) + Offset;
							}
						}
						return result / Interval;
					}
				} else
				{
					return double.NaN;
				}
			}
		}

		public int Interval { get; set; }

		public double Frequency { get; set; }

		#endregion

		#region Methods

		public APin ()
		{
			PinType = PrototypeBackend.PinType.ANALOG;
			PinMode = PrototypeBackend.PinMode.INPUT;
			PinLabel = "";
			PinNr = -1;
			PinColor = System.Drawing.Color.Blue;
			Unit = "";
			Gain = 1;
			Offset = 0;
			Interval = 1;
			Frequency = 1000;
			Values = new List<double> ();
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
			Values.Add (PrototypeBackend.ArduinoController.ReadAnalogPin (PinNr));
		}

		#endregion
	}
}

