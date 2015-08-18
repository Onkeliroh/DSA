using System;
using System.Collections.Generic;
using System.Linq;
using Gdk;

namespace PrototypeBackend
{
	public class Signal
	{
		#region Member

		public List<APin> Pins;

		public string SignalName{ get; set; }

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

		public Gdk.Color SignalColor { get; set; }

		private Func<double[],double> SignalOperation { get; set; }

		public double SignalValue {
			get {
				if (SignalOperation != null)
				{
					return (SignalOperation (Pins.Select (o => o.Value).ToArray ()));
				}
				return double.NaN;
			}
			private set { }
		}

		public string SignalOperationString { 
			get { 
				return SignalOperationString_; 
			} 
			set { 
				SignalOperation = SignalOperationCompiler.CompileOperation (value, Pins.Select (o => o.Name).ToArray ());
				if (SignalOperation != null)
				{
					SignalOperationString_ = value; 
				} else
				{
					SignalOperationString_ = string.Empty;
				}
			} 
		}

		private string SignalOperationString_;

		#endregion

		#region Methods

		public Signal ()
		{
			//Todo init
			Pins = new List<APin> ();
			SignalName = string.Empty;
			SignalColor = Gdk.Color.Zero;
			SignalOperation = null;
			SignalOperationString_ = string.Empty;
		}

		public bool AddPin (APin pin)
		{
			if (!Pins.Contains (pin))
			{
				if (Pins.Count > 0)
				{
					if (Math.Abs (Pins [0].Frequency - pin.Frequency) < 0.0001)
					{
						Pins.Add (pin);
						return true;
					}
				} else
				{
					Pins.Add (pin);
					return true;
				}
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