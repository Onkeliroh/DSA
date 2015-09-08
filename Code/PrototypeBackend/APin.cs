﻿using System;
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

		/// <summary>
		/// Gets the values (raw).
		/// </summary>
		/// <value>The values.</value>
		public List<DateTimeValue> Values{ get; private set; }

		/// <summary>
		/// Set: Adds a new value to the list of values.
		///	Get: Returns the last value converted acording to the offset slope and relative voltage.
		/// </summary>
		/// <value>The value.</value>
		public DateTimeValue Value {
			set { 
				Values.Add (value); 
				if (OnNewValue != null)
				{
					OnNewValue.Invoke (this, new NewMeasurementValue (){ RAW = value.Value, Value = CalcValue (), Time = value.Time });
				}
			}

			get {
				return new DateTimeValue (CalcValue (), Values.Last ().Time);	
			}
		}

		/// <summary>
		/// Gets or sets the interval.
		/// The Number of samlpes to build a mean value from.
		/// </summary>
		/// <value>The interval.</value>
		public UInt64 Interval { get; set; }

		/// <summary>
		/// Gets or sets the period in milliseconds.
		/// </summary>
		/// <value>The period in ms.</value>
		public UInt64 Period { get; set; }

		/// <summary>
		/// Gets the frequency in milliseconds.
		/// </summary>
		/// <value>The frequency in ms.</value>
		public UInt64 Frequency { get { return 1 / Period; } private set { } }

		/// <summary>
		/// Gets the effective period (Period * Interval).
		/// </summary>
		/// <value>The effective period.</value>
		public double EffectivePeriod { get { return Period * Interval; } private set { } }

		#endregion

		#region Events

		/// <summary>
		/// Is invoked, if new value is added
		/// </summary>
		public EventHandler<NewMeasurementValue> OnNewValue;
		/// <summary>
		/// Is invoked, if new value is added
		/// </summary>
		public EventHandler<NewMeasurementValue> OnNewRAWValue;

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
			Period = 1000;
			Values = new List<DateTimeValue> ();
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

		public double CalcValue ()
		{
			if (Values.Count >= (int)Interval)
			{
				if (Interval == 1)
				{
					if (!double.IsNaN (Values.Last ().Value))
					{
						return ((Values.Last ().Value * Slope) + Offset);
					}
					return double.NaN;
				} else
				{
					if (Values.Count >= (int)Interval)
					{
						double result = 0;
						for (int i = Values.Count - (int)Interval; i < Values.Count; i++)
						{
							if (!double.IsNaN (Values [i].Value))
							{
								result += (Values [i].Value * Slope) + Offset;
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

		#endregion
	}
}

