﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace PrototypeBackend
{
	public class Signal
	{
		#region Member

		public List<APin> Pins;

		public string SignalName{ get; private set; }

		public System.Drawing.Color SignalColor { get; set; }

		private Func<double[],double> SignalOperation { get; set; }

		public double SignalValue {
			get {
				if (SignalOperation != null)
				{
					return (SignalOperation (Pins.Select (o => o.PinValue).ToArray ()));
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
				SignalOperation = SignalOperationCompiler.CompileOperation (value, Pins.Select (o => o.PinLabel).ToArray ());
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
			SignalColor = System.Drawing.Color.Blue;
			SignalOperation = null;
			SignalOperationString_ = string.Empty;
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