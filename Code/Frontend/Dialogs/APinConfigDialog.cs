using System;
using System.Linq;
using PrototypeBackend;
using Gtk;
using GLib;
using System.Collections.Generic;

namespace Frontend
{
	public partial class APinConfigDialog : Gtk.Dialog
	{
		/// <summary>
		/// Gets or sets the pin and every nessesary widgets state.
		/// </summary>
		/// <value>The pin.</value>
		public APin Pin {
			get{ return pin; }
			set {
				entryName.Text = value.Name;
				cbColor.Color = value.PlotColor;
				cbPin.Active = 0;

				try
				{
					if (!cbUnit.Data.Contains (value.Unit))
					{
						cbUnit.InsertText (0, value.Unit);
						cbUnit.Active = 0;
					}
				} catch
				{
				}

				sbSlope.Value = value.Slope;
				sbOffset.Value = value.Offset;
				sbMeanValuesCount.Value = value.MeanValuesCount;

				sbDays.Value = TimeSpan.FromMilliseconds (value.Interval).Days;
				sbHours.Value = TimeSpan.FromMilliseconds (value.Interval).Hours;
				sbMinutes.Value = TimeSpan.FromMilliseconds (value.Interval).Minutes;
				sbSeconds.Value = TimeSpan.FromMilliseconds (value.Interval).Seconds;
				sbMilliSec.Value = TimeSpan.FromMilliseconds (value.Interval).Milliseconds;

				pin = value;
			}
		}

		/// <summary>
		/// The pin.
		/// </summary>
		private APin pin;

		/// <summary>
		/// The available pins.
		/// </summary>
		private APin[] AvailablePins;

		/// <summary>
		/// List of all provided units.
		/// </summary>
		private List<string> Units = new List<string> ();

		/// <summary>
		/// Initializes a new instance of the <see cref="AnalogPinConfigurationDialog.AnalogPinConfiguration"/> class.
		/// </summary>
		/// <param name="availablePins">Available pins.</param>
		/// <param name="apin">Apin.</param>
		/// <param name="parent">Parent.</param>
		public APinConfigDialog (APin[] availablePins, APin apin = null, Gtk.Window parent = null, List<string> units = null)
			: base ("Analog Input - Dialog", parent, Gtk.DialogFlags.Modal, new object[0])
		{
			this.Build ();


			if (apin != null)
			{
				AvailablePins = new APin[availablePins.Length + 1];
				Array.Copy (availablePins, AvailablePins, availablePins.Length);
				AvailablePins [availablePins.Length] = apin;

				buttonOk.Label = "Apply";
				buttonOk.Image = new Image (Gtk.Stock.Apply, IconSize.Button);
			} else
			{
				AvailablePins = availablePins;
			}


			sbSlope.Adjustment.Lower = double.MinValue;
			sbSlope.Adjustment.Upper = double.MaxValue;
			sbOffset.Adjustment.Lower = double.MinValue;
			sbOffset.Adjustment.Upper = double.MaxValue;
			sbMeanValuesCount.Adjustment.Upper = int.MaxValue;
			sbMeanValuesCount.Adjustment.Lower = 1;


			for (int i = 0; i < AvailablePins.Length; i++)
			{
				cbPin.AppendText (AvailablePins [i].DisplayNumber);
			}

			if (apin != null)
			{
				Pin = new APin (apin);
			} else
			{
				if (AvailablePins.Length > 0)
				{
					pin = AvailablePins [0];
				} else
				{
					pin = new APin ();
				}
				pin.PlotColor = GUIHelper.ColorHelper.GetRandomGdkColor ();
				cbColor.Color = pin.PlotColor;
			}

			if (AvailablePins.Length > 0)
			{
				if (apin != null)
				{
					cbPin.Active = AvailablePins.Length - 1;
				} else
				{
					cbPin.Active = 0;
				}
			} else
			{
				buttonOk.Sensitive = false;
				buttonOk.TooltipText = "There are no more available pins to configure.";
			}

			Units = units;
			ListStore store = new ListStore (typeof(string));
			Units.ForEach (o => store.AppendValues (new object[]{ o }));
			cbUnit.Model = store;
			if (!string.IsNullOrEmpty (pin.Unit))
			{
				if (Units.Contains (pin.Unit))
				{
					cbUnit.Active = Array.IndexOf (Units.ToArray (), pin.Unit);
				} else
				{
					store.AppendValues (new string[]{ pin.Unit });
					cbUnit.Active = Units.Count;
				}
			} else
			{
				cbUnit.Active = Array.IndexOf (Units.ToArray (), "V");
			}

			BindEvents ();
		}

		/// <summary>
		/// Binds the events.
		/// </summary>
		private void BindEvents ()
		{
			entryName.Changed += OnEntryNameChanged;	
			cbPin.Changed += OnCbPinChanged;
			cbColor.ColorSet += OnCbColorColorSet;
			cbUnit.Changed += OnCbUnitChanged;
			sbSlope.ValueChanged += OnSbSlopeChanged;
			sbOffset.ValueChanged += OnSbOffsetChanged;
			sbMeanValuesCount.ValueChanged += OnSbMeanValuesCountChanged;

			//time Events
			sbDays.ValueChanged += OnTimeChanged;
			sbHours.ValueChanged += OnTimeChanged;
			sbMinutes.ValueChanged += OnTimeChanged;
			sbSeconds.ValueChanged += OnTimeChanged;
			sbMilliSec.ValueChanged += OnTimeChanged;
		}

		/// <summary>
		/// Sets the pins name. 
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		protected void OnEntryNameChanged (object sender, EventArgs e)
		{
			if (pin != null)
			{
				pin.Name = entryName.Text;
			}
		}

		/// <summary>
		/// Changes the pin. 
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		protected void OnCbPinChanged (object sender, EventArgs e)
		{
			if (pin != null)
			{
				var selector = AvailablePins.Single (o => o.DisplayNumber == cbPin.ActiveText);
				pin.Number = selector.Number;
				pin.DigitalNumber = selector.DigitalNumber;
				pin.RealNumber = selector.RealNumber;
			}
		}

		/// <summary>
		/// Sets the color.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		protected void OnCbUnitChanged (object sender, EventArgs e)
		{
			if (pin != null)
			{
				pin.Unit = cbUnit.ActiveText;	
			}
		}

		/// <summary>
		/// Sets the slope.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		protected void OnSbSlopeChanged (object sender, EventArgs e)
		{
			if (pin != null)
			{
				pin.Slope = sbSlope.Value;
			}	
		}

		/// <summary>
		/// Sets the offset.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		protected void OnSbOffsetChanged (object sender, EventArgs e)
		{
			if (pin != null)
			{
				pin.Offset = sbOffset.Value;
			}
		}

		/// <summary>
		/// Sets the amount of values needed for building the mean value.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		protected void OnSbMeanValuesCountChanged (object sender, EventArgs e)
		{
			if (pin != null)
			{
				pin.MeanValuesCount = Convert.ToInt32 (sbMeanValuesCount.ValueAsInt);
			}
		}

		/// <summary>
		/// Sets the color of the pin.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		protected void OnCbColorColorSet (object sender, EventArgs e)
		{
			pin.PlotColor = cbColor.Color;
		}

		/// <summary>
		/// Sets the interval of the pin.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		protected void OnTimeChanged (object sender, EventArgs e)
		{
			pin.Interval = Convert.ToInt32 (
				new TimeSpan (
					sbDays.ValueAsInt,
					sbHours.ValueAsInt,
					sbMinutes.ValueAsInt,
					sbSeconds.ValueAsInt,
					sbMilliSec.ValueAsInt
				).TotalMilliseconds
			);
		}
	}
}

