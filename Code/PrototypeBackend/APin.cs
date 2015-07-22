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

		public PrototypeBackend.PinType Type { get; set; }

		public PrototypeBackend.PinMode Mode { get; set; }

		public string Name { get; set; }

		public string Unit { get; set; }

		public int Number{ get; set; }

		public System.Drawing.Color PlotColor { get; set; }

		public double Slope{ get; set; }

		public double Offset{ get; set; }

		public List<double> Values{ get; private set; }

		public double Value {
			private set{ }
			get {
				if (Values.Count >= Interval)
				{
					if (Interval == 1)
					{
						if (!double.IsNaN (Values.Last ()))
						{
							return ((Values.Last () * Slope) + Offset);
						}
						return double.NaN;
					} else
					{
						if (Values.Count >= Interval)
						{
							double result = 0;
							for (int i = Values.Count - Interval; i < Values.Count; i++)
							{
								if (!double.IsNaN (Values [i]))
								{
									result += (Values [i] * Slope) + Offset;
								}
							}
							return result / Interval;
						} else
						{
							return double.NaN;
						}
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
			Type = PrototypeBackend.PinType.ANALOG;
			Mode = PrototypeBackend.PinMode.INPUT;
			Name = "";
			Number = -1;
			PlotColor = System.Drawing.Color.Empty;
			Unit = "";
			Slope = 1;
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
					return (obj as APin).Type == Type &&
					(obj as APin).Mode == Mode &&
					(obj as APin).Name.Equals (Name) &&
					(obj as APin).Unit == this.Unit &&
					(obj as APin).Number.Equals (Number) &&
					(obj as APin).PlotColor.Equals (PlotColor);
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
			return String.Format ("Name: {0}\tNumber: {1}\tPinType: {2}\tUnit: {3}\tColor: {4}", Name, Number, Type, Unit, PlotColor);
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
			Values.Add (PrototypeBackend.ArduinoController.ReadAnalogPin (Number));
		}

		#endregion
	}
}

