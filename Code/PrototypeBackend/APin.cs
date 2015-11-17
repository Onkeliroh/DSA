using System;
using System.Xml.Serialization;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

using Gdk;
using System.Threading;

namespace PrototypeBackend
{
	/// <summary>
	/// A class managing analog pins.
	/// </summary>
	[Serializable]
	public class APin : IPin, ISerializable
	{
		#region Member


		/// <summary>
		/// Gets or sets the type.
		/// </summary>
		/// <value>The type.</value>
		public PrototypeBackend.PinType Type { get; set; }

		/// <summary>
		/// Gets or sets the mode.
		/// </summary>
		/// <value>The mode.</value>
		public PrototypeBackend.PinMode Mode { get; set; }

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name.</value>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the display name. (Name + DisplayNumber)
		/// </summary>
		/// <value>The display name.</value>
		public string DisplayName { get { return string.Format ("{0} ({1})", Name, DisplayNumber); } set { } }

		/// <summary>
		/// Gets or sets the display number. (Ax | Dx) or (Ax)
		/// </summary>
		/// <value>The display number.</value>
		public string DisplayNumber { get { return string.Format ("A{0} | D{1}", Number.ToString ().PadLeft (2, ' '), DigitalNumber); } set { } }

		/// <summary>
		/// Gets or sets the display number short. (Ax)
		/// </summary>
		/// <value>The display number short.</value>
		public string DisplayNumberShort { get { return string.Format ("A{0}", Number.ToString ().PadLeft (2, ' ')); } set { } }

		/// <summary>
		/// Gets or sets the unit.
		/// </summary>
		/// <value>The unit.</value>
		public string Unit { get; set; }

		/// <summary>
		/// Gets or sets the number.
		/// this represents the hardware pin number under concideration of the pin type (e.g. Arduino UNO D14 = A0)
		/// </summary>
		/// <value>The number.</value>
		public uint Number{ get; set; }

		/// <summary>
		/// Gets or sets the real number.
		/// This represents the 'real' hardware pin number with out concideration of the pin type. (eg. Arduino UNO D0 = 0, A0 = 14)
		/// </summary>
		/// <value>The real number.</value>
		public uint RealNumber { get { return DigitalNumber; } set { } }

		/// <summary>
		/// Gets or sets the digital number.
		/// This is the same as the RealNumber.
		/// </summary>
		/// <value>The digital number.</value>
		public uint DigitalNumber { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="PrototypeBackend.APin"/> is SDA enabled.
		/// </summary>
		/// <value><c>true</c> if SDA enabled; otherwise, <c>false</c>.</value>
		public bool SDA { get ; set ; }

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="PrototypeBackend.APin"/> is SCL enabled.
		/// </summary>
		/// <value><c>true</c> if SCL enabled; otherwise, <c>false</c>.</value>
		public bool SCL { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="PrototypeBackend.APin"/> is RX enabled.
		/// </summary>
		/// <value><c>true</c> if RX enabled; otherwise, <c>false</c>.</value>
		public bool RX { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="PrototypeBackend.APin"/> is TX enabled.
		/// </summary>
		/// <value><c>true</c> if TX enabled; otherwise, <c>false</c>.</value>
		public bool TX { get; set; }

		/// <summary>
		/// Gets or sets the color of the pin values in the plot and elsewhere.
		/// </summary>
		/// <value>The color of the pin.</value>
		public Gdk.Color PlotColor { get; set; }

		/// <summary>
		/// Gets or sets the slope.
		/// This is used for linear conversion ( Values = RAWValue * Slope + Offset )
		/// </summary>
		/// <value>The slope.</value>
		public double Slope{ get; set; }

		/// <summary>
		/// Gets or sets the offset.
		/// This is used for linear conversion ( Values = RAWValue * Slope + Offset )
		/// </summary>
		/// <value>The offset.</value>
		public double Offset{ get; set; }

		/// <summary>
		/// Gets the values (raw).
		/// </summary>
		/// <value>The values.</value>
		public List<DateTimeValue> Values{ get; private set; }

		/// <summary>
		/// Gets the RAW values.
		/// </summary>
		/// <value>The RAW values.</value>
		public List<DateTimeValue> RAWValues { get; private set; }

		/// <summary>
		/// Set: Adds a new value to the list of values.
		///	Get: Returns the last value converted acording to the offset slope and relative voltage.
		/// </summary>
		/// <value>The value.</value>
		public DateTimeValue Value {
			set { 
				AddRawValue (value);
				double val = CalcValue ();
				if (!double.IsNaN (val))
				{
					Values.Add (new DateTimeValue (CalcValue (), value.Time));
				
					if (OnNewValue != null)
					{
						DateTime time = DateTime.FromOADate (value.Time);
						OnNewValue.Invoke (this, new NewMeasurementValueArgs () {
							RAW = value.Value,
							Value = val,
							Time = time
						});
					}
				}
			}

			get {
				if (Values.Count > 0)
				{
					return new DateTimeValue (Values.Last ());	
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
		public int MeanValuesCount { get; set; }

		/// <summary>
		/// Gets or sets the interval in milliseconds.
		/// </summary>
		/// <value>The interval in ms.</value>
		public int Interval { get; set; }

		/// <summary>
		/// Gets the frequency in milliseconds.
		/// </summary>
		/// <value>The frequency in ms.</value>
		public int Frequency { get { return 1 / Interval; } private set { } }

		/// <summary>
		/// Gets the effective interval (<paramref name="Interval"/> * <paramref name="MeanValuesCount"/>).
		/// </summary>
		/// <value>The effective interval.</value>
		public double EffectiveInterval { get { return Interval * MeanValuesCount; } private set { } }

		#endregion

		#region Events

		/// <summary>
		/// Is invoked, if new value is added
		/// </summary>
		[NonSerialized]
		public EventHandler<NewMeasurementValueArgs> OnNewValue;
		/// <summary>
		/// Is invoked, if new value is added
		/// </summary>
		[NonSerialized]
		public EventHandler<NewMeasurementValueArgs> OnNewRAWValue;

		#endregion

		#region Methods

		/// <summary>
		/// Initializes a new instance of the <see cref="PrototypeBackend.APin"/> class.
		/// </summary>
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
			RAWValues = new List<DateTimeValue> ();
			Unit = "V";
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PrototypeBackend.APin"/> class.
		/// </summary>
		/// <param name="copy">Copy.</param>
		public APin (APin copy) : base ()
		{

			Name = copy.Name;
			Number = copy.Number;
			DigitalNumber = copy.DigitalNumber;
			PlotColor = copy.PlotColor;
			Slope = copy.Slope;
			Offset = copy.Offset;
			MeanValuesCount = copy.MeanValuesCount;
			Interval = copy.Interval;
			Unit = copy.Unit;
			Type = PrototypeBackend.PinType.ANALOG;
			Mode = PrototypeBackend.PinMode.INPUT;
		}

		/// <summary>
		/// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="PrototypeBackend.APin"/>.
		/// </summary>
		/// <param name="obj">The <see cref="System.Object"/> to compare with the current <see cref="PrototypeBackend.APin"/>.</param>
		/// <returns><c>true</c> if the specified <see cref="System.Object"/> is equal to the current
		/// <see cref="PrototypeBackend.APin"/>; otherwise, <c>false</c>.</returns>
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

		/// <summary>
		/// Serves as a hash function for a <see cref="PrototypeBackend.APin"/> object.
		/// </summary>
		/// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a hash table.</returns>
		public override int GetHashCode ()
		{
			return base.GetHashCode ();
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="PrototypeBackend.APin"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="PrototypeBackend.APin"/>.</returns>
		public override string ToString ()
		{
			return String.Format ("Name: {0}\tNumber: {1}\tPinType: {2}\tUnit: {3}", Name, Number, Type, Unit);
		}

		/// <summary>
		/// Calculates the measured value under concideration of offset, slope and mean values.
		/// </summary>
		/// <returns>The value.</returns>
		public double CalcValue ()
		{
			if (RAWValues.Count >= MeanValuesCount)
			{
				if (MeanValuesCount == 1)
				{
					if (!double.IsNaN (RAWValues.Last ().Value))
					{
						return TranslateRAW (RAWValues.Last ().Value);
					}
					return double.NaN;
				} else
				{
					if (RAWValues.Count % MeanValuesCount == 0)
					{
						double result = 0;
						result = RAWValues.GetRange (RAWValues.Count - MeanValuesCount, MeanValuesCount).Sum (o => TranslateRAW (o.Value));
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

		private double TranslateRAW (double raw)
		{
			return (raw * Slope) + Offset;
		}

		private void AddRawValue (DateTimeValue value)
		{
			RAWValues.Add (value);
			if (OnNewRAWValue != null)
			{
				OnNewRAWValue.Invoke (
					this,
					new NewMeasurementValueArgs () {
						RAW = value.Value,
						Value = value.Value,
						Time = DateTime.FromOADate (value.Time) 
					}
				);
			}
		}

		#endregion

		#region ISerializable implementation

		/// <summary>
		/// Gets the object data.
		/// </summary>
		/// <param name="info">Info.</param>
		/// <param name="context">Context.</param>
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

		/// <summary>
		/// Initializes a new instance of the <see cref="PrototypeBackend.APin"/> class.
		/// </summary>
		/// <param name="info">Info.</param>
		/// <param name="context">Context.</param>
		public APin (SerializationInfo info, StreamingContext context) : base ()
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
			MeanValuesCount = info.GetInt32 ("Interval");
			Interval = info.GetInt32 ("Period");

			Values = new List<DateTimeValue> ();
			RAWValues = new List<DateTimeValue> ();
		}

		#endregion

		/// <summary>
		/// Uints to byte.
		/// This method is used to parse colors form one framework to another.
		/// </summary>
		/// <returns>The to byte.</returns>
		/// <param name="val">Value.</param>
		public static byte uintToByte (uint val)
		{
			return (byte)(byte.MaxValue / 65535.0 * val);
		}
	}
}

