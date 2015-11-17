using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Gdk;

namespace PrototypeBackend
{
	/// <summary>
	/// Measurement combination.
	/// </summary>
	[Serializable]
	public class MeasurementCombination : ISerializable
	{
		#region Member

		/// <summary>
		/// The pins.
		/// </summary>
		public List<APin> Pins;

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name.</value>
		public string Name{ get; set; }

		/// <summary>
		/// Gets or sets the display name <see cref="Name"/>.
		/// </summary>
		/// <value>The display name.</value>
		public string DisplayName { 
			get {
				string s = "";
				s += Name;
				s += "( ";
				foreach (APin a in Pins)
				{
					s += a.DisplayNumberShort + " ";
				}
				s += ")";
				return s; 
			} 
			set { } 
		}

		/// <summary>
		/// Gets or sets the unit.
		/// </summary>
		/// <value>The unit.</value>
		public string Unit { get; set; }

		/// <summary>
		/// Gets the frequency.
		/// </summary>
		/// <value>The frequency.</value>
		public double Frequency { 
			get { 
				return Pins.OrderByDescending (o => o.Interval).First ().Frequency; 
			} 
			private set { } 
		}

		/// <summary>
		/// Gets the interval.
		/// </summary>
		/// <value>The biggest interval of all the pins.</value>
		public int Interval {
			get { 
				return Pins.OrderByDescending (o => o.Interval).First ().Interval;
			} 
			private set { }
		}

		/// <summary>
		/// Gets or sets the mean values count.
		/// Determines the values used to create a mean value from.
		/// </summary>
		/// <value>The mean values count.</value>
		public int MeanValuesCount { get; set; }

		/// <summary>
		/// Gets or sets the color.
		/// </summary>
		/// <value>The color.</value>
		public Gdk.Color Color { get; set; }

		/// <summary>
		/// Gets or sets the operation.
		/// </summary>
		/// <value>The operation.</value>
		public Func<double[],double> Operation { get; set; }

		/// <summary>
		/// Gets the value.
		/// </summary>
		/// <value>The value.</value>
		public DateTimeValue Value {
			get {
				if (Pins.Count > 0)
				{
					if (Operation != null)
					{
						var time = Pins.OrderByDescending (o => o.Interval).First ().Value.Time;
						double val = CalcValue ();

						var dtv = new DateTimeValue () {
							Value = val,
							Time = time
						};
						Values.Add (dtv);
						return dtv;
					}
					return  new DateTimeValue (){ Value = double.NaN, Time = Pins.OrderBy (o => o.Interval).First ().Value.Time };
				} else
				{
					return new DateTimeValue (){ Value = double.NaN, Time = DateTime.Now.ToOADate () };
				}
			}
			private set { }
		}

		/// <summary>
		/// The values
		/// </summary>
		public List<DateTimeValue> Values{ get; private set; }

		/// <summary>
		/// Gets or sets the operation string.
		/// </summary>
		/// <value>The operation string.</value>
		public string OperationString { 
			get { 
				return OperationString_; 
			} 
			set { 
				OperationString_ = value; 
			} 
		}

		/// <summary>
		/// The operation string.
		/// </summary>
		private string OperationString_;

		#endregion

		#region Events

		public EventHandler<NewMeasurementValueArgs> OnNewValue;

		#endregion

		#region Methods

		/// <summary>
		/// Initializes a new instance of the <see cref="PrototypeBackend.MeasurementCombination"/> class.
		/// </summary>
		public MeasurementCombination ()
		{
			//Todo init
			Pins = new List<APin> ();
			Name = string.Empty;
			Color = Gdk.Color.Zero;
			Operation = null;
			OperationString_ = string.Empty;
			Unit = string.Empty;
			MeanValuesCount = 1;
			Values = new List<DateTimeValue> ();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PrototypeBackend.MeasurementCombination"/> class.
		/// </summary>
		/// <param name="copy">Copy.</param>
		public MeasurementCombination (MeasurementCombination copy) : base ()
		{
			Pins = copy.Pins;
			Name = copy.Name;
			Color = copy.Color;
			Operation = copy.Operation;
			OperationString = copy.OperationString;
			Unit = copy.Unit;
			MeanValuesCount = copy.MeanValuesCount;
			Values = copy.Values;
		}

		/// <summary>
		/// Adds a pin.
		/// </summary>
		/// <returns><c>true</c>, if pin was added, <c>false</c> otherwise.</returns>
		/// <param name="pin">Pin.</param>
		public bool AddPin (APin pin)
		{
			if (!Pins.Contains (pin))
			{
				Pins.Add (pin);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Calculates the value.
		/// </summary>
		/// <returns>The value.</returns>
		private double CalcValue ()
		{
			if (CheckPinIntervalEquality ())
			{
				if (Pins.TrueForAll (o => o.Values.Count % MeanValuesCount == 0))
				{
					double[] pinsvalues = new double[Pins.Count];
					for (int i = 0; i < Pins.Count; i++)
					{
						pinsvalues [i] =
							Pins [i].Values.GetRange (
							Pins [i].Values.Count - MeanValuesCount, MeanValuesCount
						).Sum (o => o.Value) / (double)MeanValuesCount;
					}
					return Operation (pinsvalues);
				} else
				{
					return double.NaN;
				}
			} else
			{
				return Operation (Pins.Select (o => o.Values.Last ().Value).ToArray ());
			}
		}

		/// <summary>
		/// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="PrototypeBackend.MeasurementCombination"/>.
		/// </summary>
		/// <param name="obj">The <see cref="System.Object"/> to compare with the current <see cref="PrototypeBackend.MeasurementCombination"/>.</param>
		/// <returns><c>true</c> if the specified <see cref="System.Object"/> is equal to the current
		/// <see cref="PrototypeBackend.MeasurementCombination"/>; otherwise, <c>false</c>.</returns>
		public override bool Equals (object obj)
		{
			MeasurementCombination MeCom = obj as MeasurementCombination;
			if (MeCom != null)
			{
				return 
				    this.Pins.SequenceEqual (MeCom.Pins)	&&
				this.Name.Equals (MeCom.Name) &&
				this.MeanValuesCount.Equals (MeCom.MeanValuesCount) &&
				this.OperationString_.Equals (MeCom.OperationString_) &&
				this.Interval.Equals (MeCom.Interval) &&
				this.Unit.Equals (MeCom.Unit) &&
				this.Color.Equal (MeCom.Color);
			}
			return false;
		}

		/// <summary>
		/// Gets the pin with largest interval.
		/// </summary>
		/// <returns>The pin with largest interval.</returns>
		public APin GetPinWithLargestInterval ()
		{
			return Pins.OrderByDescending (o => o.Interval).ToList () [0];
		}

		public bool CheckPinIntervalEquality ()
		{
			if (Pins.Count > 0)
			{
				return Pins.TrueForAll (o => o.Interval == Pins [0].Interval);
			} else
			{
				return true;
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
			info.AddValue ("Pins", Pins);
			info.AddValue ("Name", Name);
			info.AddValue ("Unit", Unit);
			info.AddValue ("RED", uintToByte (Color.Red));
			info.AddValue ("GREEN", uintToByte (Color.Green));
			info.AddValue ("BLUE", uintToByte (Color.Blue));
			info.AddValue ("Interval", MeanValuesCount);
			info.AddValue ("OperationString", OperationString);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PrototypeBackend.MeasurementCombination"/> class.
		/// </summary>
		/// <param name="info">Info.</param>
		/// <param name="context">Context.</param>
		public MeasurementCombination (SerializationInfo info, StreamingContext context) : base ()
		{
			Pins = new List<APin> ();
			Pins = (List<APin>)info.GetValue ("Pins", Pins.GetType ());
			Name = info.GetString ("Name");
			Unit = info.GetString ("Unit");
			Color = new Gdk.Color (info.GetByte ("RED"), info.GetByte ("GREEN"), info.GetByte ("BLUE"));
			MeanValuesCount = info.GetInt32 ("Interval");
			OperationString = info.GetString ("OperationString");

			Operation = OperationCompiler.CompileOperation (OperationString, Pins.Select (o => o.DisplayNumberShort).ToArray<string> ());

			Values = new List<DateTimeValue> ();
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