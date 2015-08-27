﻿using System;
using System.Collections.Generic;
using System.Linq;
using Gdk;

namespace PrototypeBackend
{
	public class MeasurementCombination
	{
		#region Member

		public List<APin> Pins;

		public string Name{ get; set; }

		public string Unit { get; set; }

		public double Frequency {
			get {
				if (Pins.Count > 0)
					return Pins [0].EffectiveFrequency;
				else
					return -1;
			}
			private set{ }
		}

		public int Interval { get; set; }

		public Gdk.Color Color { get; set; }

		private Func<double[],double> Operation { get; set; }

		public double Value {
			get {
				if (Operation != null)
				{
					return (Operation (Pins.Select (o => o.Value).ToArray ()));
				}
				return double.NaN;
			}
			private set { }
		}

		public string OperationString { 
			get { 
				return OperationString_; 
			} 
			set { 
				Operation = OperationCompiler.CompileOperation (value, Pins.Select (o => o.Name).ToArray ());
				if (Operation != null)
				{
					OperationString_ = value; 
				} else
				{
					OperationString_ = string.Empty;
				}
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
		}

		public bool AddPin (APin pin)
		{
			if (!Pins.Contains (pin))
			{
				Pins.Add (pin);
				return true;
			}
			return false;
		}

		public void Run ()
		{
			foreach (APin ap in Pins)
			{
				ap.Run ();
			}
		}

		#endregion
	}
}