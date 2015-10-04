﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Gdk;

namespace PrototypeBackend
{
	[Serializable]
	public class MeasurementCombination : ISerializable
	{
		#region Member

		public List<APin> Pins;

		public string Name{ get; set; }

		public string DisplayName { get { return Name; } set { } }

		public string Unit { get; set; }

		public double Frequency { 
			get { 
				return Pins.OrderByDescending (o => o.Interval).First ().Frequency; 
			} 
			private set { } 
		}

		public UInt64 Interval {
			get { 
				return Pins.OrderByDescending (o => o.Interval).First ().Interval;
			} 
			private set { }
		}

		public int MeanValuesCount { get; set; }

		public Gdk.Color Color { get; set; }

		public Func<double[],double> Operation { get; set; }

		public DateTimeValue Value {
			get {
				if (Pins.Count > 0) {
					if (Operation != null) {
						return new DateTimeValue () {
						
							Value = (Operation (Pins.Select (o => o.Value.Value).ToArray ())),
							Time = Pins.OrderByDescending (o => o.Interval).First ().Value.Time
						};
					}
					return new DateTimeValue (){ Value = double.NaN, Time = Pins.OrderBy (o => o.Interval).First ().Value.Time };
				} else {
					return new DateTimeValue (){ Value = double.NaN, Time = DateTime.Now };
				}
			}
			private set { }
		}

		public string OperationString { 
			get { 
				return OperationString_; 
			} 
			set { 
				OperationString_ = value; 
			} 
		}

		private string OperationString_;

		#endregion

		#region Methods

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
		}

		public MeasurementCombination (MeasurementCombination copy) : base ()
		{
			Pins = copy.Pins;
			Name = copy.Name;
			Color = copy.Color;
			Operation = copy.Operation;
			OperationString = copy.OperationString;
			Unit = copy.Unit;
			MeanValuesCount = copy.MeanValuesCount;
		}

		public bool AddPin (APin pin)
		{
			if (!Pins.Contains (pin)) {
				Pins.Add (pin);
				ManagePins ();
				return true;
			}
			return false;
		}

		private void ManagePins ()
		{
			var list = Pins.OrderByDescending (o => o.Interval);
//			list.First ().OnNewValue += (o, e) => {
//				if (OnNewValue != null) {
//					OnNewValue.Invoke (this, new NewMeasurementValue (){ Value = this.Value.Value, Time = this.Value.Time });
//				}
//			};
		}

		public override bool Equals (object obj)
		{
			MeasurementCombination MeCom = obj as MeasurementCombination;
			if (MeCom != null) {
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

		#endregion

		#region ISerializable implementation

		public void GetObjectData (SerializationInfo info, StreamingContext context)
		{
			info.AddValue ("Pins", Pins);
			info.AddValue ("Name", Name);
			info.AddValue ("Unit", Unit);
			info.AddValue ("Interval", MeanValuesCount);
			info.AddValue ("OperationString", OperationString);
		}

		public MeasurementCombination (SerializationInfo info, StreamingContext context)
		{
			Pins = new List<APin> ();
			Pins = (List<APin>)info.GetValue ("Pins", Pins.GetType ());
			Name = info.GetString ("Name");
			Unit = info.GetString ("Unit");
			MeanValuesCount = info.GetInt32 ("Interval");
			OperationString = info.GetString ("OperationString");

			Operation = OperationCompiler.CompileOperation (OperationString, Pins.Select (o => o.DisplayNumberShort).ToArray<string> ());
		}

		#endregion
	}
}