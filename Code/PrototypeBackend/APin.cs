﻿using System;
using System.Xml.Serialization;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

using Gdk;

namespace PrototypeBackend
{
	[Serializable]
	public class APin : IPin, ISerializable
	{
		#region Member


		public PrototypeBackend.PinType Type { get; set; }

		public PrototypeBackend.PinMode Mode { get; set; }

		public string Name { get; set; }

		public string DisplayName { get { return string.Format ("{0} ({1})", Name, DisplayNumber); } set { } }

		public string DisplayNumber { get { return string.Format ("A{0} | D{1}", Number.ToString ().PadLeft (2, ' '), DigitalNumber); } set { } }

		public string DisplayNumberShort { get { return string.Format ("A{0}", Number.ToString ().PadLeft (2, ' ')); } set { } }

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
				if (Values.Count > 0)
				{
					return new DateTimeValue (CalcValue (), Values.Last ().Time);	
				} else
				{
					return new DateTimeValue (double.NaN, DateTime.Now);
				}
			}
		}

		/// <summary>
		/// Gets or sets the interval.
		/// The Number of samlpes to build a mean value from.
		/// </summary>
		/// <value>The interval.</value>
		public UInt64 MeanValuesCount { get; set; }

		/// <summary>
		/// Gets or sets the period in milliseconds.
		/// </summary>
		/// <value>The period in ms.</value>
		public UInt64 Interval { get; set; }

		/// <summary>
		/// Gets the frequency in milliseconds.
		/// </summary>
		/// <value>The frequency in ms.</value>
		public UInt64 Frequency { get { return 1 / Interval; } private set { } }

		/// <summary>
		/// Gets the effective period (Period * Interval).
		/// </summary>
		/// <value>The effective period.</value>
		public double EffectivePeriod { get { return Interval * MeanValuesCount; } private set { } }

		#endregion

		#region Events

		/// <summary>
		/// Is invoked, if new value is added
		/// </summary>
		[NonSerialized]
		public EventHandler<NewMeasurementValue> OnNewValue;
		/// <summary>
		/// Is invoked, if new value is added
		/// </summary>
		[NonSerialized]
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
			MeanValuesCount = 1;
			Interval = 1000;
			Values = new List<DateTimeValue> ();
		}

		public APin (APin copy) : base ()
		{
			Name = copy.Name;
			Number = copy.Number;
			PlotColor = copy.PlotColor;
			Slope = copy.Slope;
			Offset = copy.Offset;
			MeanValuesCount = copy.MeanValuesCount;
			Interval = copy.Interval;
			Unit = copy.Unit;
			Type = PrototypeBackend.PinType.ANALOG;
			Mode = PrototypeBackend.PinMode.INPUT;
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

		public double CalcValue ()
		{
			if (Values.Count >= (int)MeanValuesCount)
			{
				if (MeanValuesCount == 1)
				{
					if (!double.IsNaN (Values.Last ().Value))
					{
						return ((Values.Last ().Value * Slope) + Offset);
					}
					return double.NaN;
				} else
				{
					if (Values.Count >= (int)MeanValuesCount)
					{
						double result = 0;
						for (int i = Values.Count - (int)MeanValuesCount; i < Values.Count; i++)
						{
							if (!double.IsNaN (Values [i].Value))
							{
								result += (Values [i].Value * Slope) + Offset;
							}
						}
						return result / MeanValuesCount;
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

		#region ISerializable implementation

		public void GetObjectData (SerializationInfo info, StreamingContext context)
		{
			info.AddValue ("Type", Type);
			info.AddValue ("Mode", Mode);
			info.AddValue ("Name", Name);
			info.AddValue ("Unit", Unit);
			info.AddValue ("Number", Number);
			info.AddValue ("DigitalNumber", DigitalNumber);
			info.AddValue ("SDA", SDA);
			info.AddValue ("SCL", SCL);
			info.AddValue ("RX", RX);
			info.AddValue ("TX", TX);
			info.AddValue ("RED", uintToByte (PlotColor.Red));
			info.AddValue ("GREEN", uintToByte (PlotColor.Green));
			info.AddValue ("BLUE", uintToByte (PlotColor.Blue));
			info.AddValue ("Slope", Slope);
			info.AddValue ("Offset", Offset);
			info.AddValue ("Interval", MeanValuesCount);
			info.AddValue ("Period", Interval);
		}

		public APin (SerializationInfo info, StreamingContext context)
		{
			Type = (PinType)info.GetByte ("Type");
			Mode = (PinMode)info.GetByte ("Mode");
			Name = info.GetString ("Name");
			Unit = info.GetString ("Unit");
			Number = info.GetUInt32 ("Number");
			DigitalNumber = info.GetUInt32 ("DigitalNumber");
			SDA = info.GetBoolean ("SDA");
			SCL = info.GetBoolean ("SCL");
			RX = info.GetBoolean ("RX");
			TX = info.GetBoolean ("TX");
			PlotColor = new Gdk.Color (info.GetByte ("RED"), info.GetByte ("GREEN"), info.GetByte ("BLUE"));
			Slope = info.GetDouble ("Slope");
			Offset = info.GetDouble ("Offset");
			MeanValuesCount = info.GetUInt64 ("Interval");
			Interval = info.GetUInt64 ("Period");

			Values = new List<DateTimeValue> ();
		}

		#endregion

		public static byte uintToByte (uint val)
		{
			return (byte)(byte.MaxValue / 65535.0 * val);
		}
	}
}

