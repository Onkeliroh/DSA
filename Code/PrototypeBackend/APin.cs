using System;
using System.Xml.Serialization;
using System.IO;
using System.Collections.Generic;
using System.Linq;

using Gdk;

namespace PrototypeBackend
{
	public class APin : IPin
	{
		#region Member

		public PrototypeBackend.PinType Type { get; set; }

		public PrototypeBackend.PinMode Mode { get; set; }

		public string Name { get; set; }

		public string DisplayName { get { return string.Format ("{0} ({1})", Name, DisplayNumber); } set { } }

		public string DisplayNumber { get { return string.Format ("A{0} | D{1}", Number, DigitalNumber); } set { } }

		public string Unit { get; set; }

		public uint Number{ get; set; }

		public uint RealNumber { get { return DigitalNumber; } set { } }

		public uint DigitalNumber { get; set; }

		public bool SDA { get ; set ; }

		public bool SCL { get; set; }

		public bool RX { get; set; }

		public bool TX { get; set; }

		public Gdk.Color PlotColor { get; set; }

		public double Slope{ get; set; }

		public double Offset{ get; set; }

		public List<double> Values{ get; private set; }

		public double Value {
			set{ Values.Add (value); }
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

		/// <summary>
		/// Gets or sets the frequency in milliseconds.
		/// </summary>
		/// <value>The frequency in ms.</value>
		public Int64 Frequency { get; set; }

		public double EffectiveFrequency { get { return Frequency * Interval; } private set { } }

		#endregion

		#region Events

		public EventHandler OnNewValue;
		public EventHandler OnNewRAWValue;

		#endregion

		#region Methods

		public APin ()
		{
			Type = PrototypeBackend.PinType.ANALOG;
			Mode = PrototypeBackend.PinMode.INPUT;
			Name = "";
			Number = 0;
			PlotColor = new Gdk.Color (0, 0, 0);
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
			return String.Format ("Name: {0}\tNumber: {1}\tPinType: {2}\tUnit: {3}", Name, Number, Type, Unit);
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

		#endregion
	}
}

