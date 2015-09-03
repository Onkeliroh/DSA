using System;
using System.Drawing;
using System.Linq;
using PrototypeBackend;
using Gtk;

namespace AnalogPinConfigurationDialog
{
	public partial class AnalogPinConfiguration : Gtk.Dialog
	{
		public APin Pin {
			get{ return pin; }
			set {
				entryName.Text = value.Name;
				cbColor.Color = value.PlotColor;
				cbPin.InsertText (0, value.DisplayNumber);
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
				sbFrequency.Value = value.Frequency;
				sbInterval.Value = value.Interval;

				pin = value;
			}
		}

		private APin pin;

		private APin[] AvailablePins;

		public AnalogPinConfiguration (APin[] availablePins, APin apin = null, Gtk.Window parent = null)
			: base ("Analog Pin Configuration", parent, Gtk.DialogFlags.Modal, new object[0])
		{
			this.Build ();


			if (apin != null)
			{
				AvailablePins = new APin[availablePins.Length + 1];
				Array.Copy (availablePins, AvailablePins, availablePins.Length);
				AvailablePins [availablePins.Length] = apin;
			} else
			{
				AvailablePins = availablePins;
			}


			sbFrequency.Adjustment.Lower = double.MinValue;
			sbFrequency.Adjustment.Upper = double.MaxValue;
			sbSlope.Adjustment.Lower = double.MinValue;
			sbSlope.Adjustment.Upper = double.MaxValue;
			sbOffset.Adjustment.Lower = double.MinValue;
			sbOffset.Adjustment.Upper = double.MaxValue;
			sbInterval.Adjustment.Upper = int.MaxValue;

			cbColor.Color = GUIHelper.ColorHelper.GetRandomGdkColor ();

			for (int i = 0; i < availablePins.Length; i++)
			{
				cbPin.AppendText (availablePins [i].DisplayNumber);
			}

			if (apin != null)
			{
				Pin = apin;
			} else
			{
				pin = new APin ();
			}

			if (availablePins.Length > 0)
			{
				cbPin.Active = 0;
			}
		}

		protected void OnButtonOkClicked (object sender, EventArgs e)
		{
			pin.Name = entryName.Text;
			pin.Number = AvailablePins.Where (o => o.DisplayNumber == cbPin.ActiveText).ToList () [0].Number;
			pin.PlotColor = cbColor.Color;
			pin.Unit = cbUnit.ActiveText;
			pin.Slope = sbSlope.Value;
			pin.Offset = sbOffset.Value;
			pin.Frequency = sbFrequency.Value;
			pin.Interval = sbInterval.ValueAsInt;

			Respond (Gtk.ResponseType.Apply);
		}

		protected void OnEntryNameChanged (object sender, EventArgs e)
		{
			if (pin != null)
			{
				pin.Name = entryName.Text;
			}
		}

		protected void OnCbPinChanged (object sender, EventArgs e)
		{
			if (pin != null)
			{
				var selector = AvailablePins.Where (o => o.DisplayNumber == cbPin.ActiveText).ToList () [0];
				pin.Number = selector.Number;
				pin.DigitalNumber = selector.DigitalNumber;
			}
		}

		protected void OnCbColorClicked (object sender, EventArgs e)
		{
			if (pin != null)
			{
				pin.PlotColor = cbColor.Color;
			}
		}

		protected void OnCbUnitChanged (object sender, EventArgs e)
		{
			if (pin != null)
			{
				pin.Unit = cbUnit.ActiveText;	
			}
		}

		protected void OnSbSlopeChanged (object sender, EventArgs e)
		{
			if (pin != null)
			{
				pin.Slope = sbSlope.Value;
			}	
		}

		protected void OnSbOffsetChanged (object sender, EventArgs e)
		{
			if (pin != null)
			{
				pin.Offset = sbOffset.Value;
			}
		}

		protected void OnSbFrequencyChanged (object sender, EventArgs e)
		{
			if (pin != null)
			{
				pin.Frequency = sbFrequency.Value;	
			}
		}

		protected void OnSbIntervalChanged (object sender, EventArgs e)
		{
			if (pin != null)
			{
				pin.Interval = sbInterval.ValueAsInt;
			}
		}
	}
}

