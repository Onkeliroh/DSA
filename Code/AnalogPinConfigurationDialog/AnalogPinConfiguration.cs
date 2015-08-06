using System;
using System.Drawing;
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
				cbPin.InsertText (0, "A" + value.Number.ToString ());
				cbPin.Active = 0;

				if (!cbUnit.Data.Contains (value.Unit)) {
					cbUnit.InsertText (0, value.Unit);
					cbUnit.Active = 0;
				}

				sbSlope.Value = value.Slope;
				sbOffset.Value = value.Offset;
				sbFrequency.Value = value.Frequency;
				sbInterval.Value = value.Interval;

				pin = value;
			}
		}

		private APin pin;

		public AnalogPinConfiguration (int[] availablePins, APin apin = null, Gtk.Window parent = null)
			: base ("Analog Pin Configuration", parent, Gtk.DialogFlags.Modal, new object[0])
		{
			this.Build ();

			sbFrequency.Adjustment.Lower = double.MinValue;
			sbFrequency.Adjustment.Upper = double.MaxValue;
			sbSlope.Adjustment.Lower = double.MinValue;
			sbSlope.Adjustment.Upper = double.MaxValue;
			sbOffset.Adjustment.Lower = double.MinValue;
			sbOffset.Adjustment.Upper = double.MaxValue;
			sbInterval.Adjustment.Upper = int.MaxValue;

			for (int i = 0; i < availablePins.Length; i++) {
				cbPin.AppendText ("A" + availablePins [i].ToString ());
			}

			if (availablePins.Length > 0) {
				cbPin.Active = 0;
			}

			if (apin != null) {
				Pin = apin;
			} else {
				pin = new APin ();
			}
		}

		protected void OnButtonOkClicked (object sender, EventArgs e)
		{
//			pin = new APin () {
//				Name = (entryName.Text == null) ? " " : entryName.Text,
//				Number = Convert.ToInt32 (cbPin.ActiveText.Remove (0, 1)),
//				PlotColor = cbColor.Color,
//
//				Unit = cbUnit.ActiveText,
//				Slope = sbSlope.Value,
//				Offset = sbOffset.Value,
//				Frequency = sbFrequency.Value,
//				Interval = sbInterval.ValueAsInt,
//			};
			pin.Name = entryName.Text;
			pin.Number = Convert.ToInt32 (cbPin.ActiveText.Remove (0, 1));
			pin.PlotColor = cbColor.Color;
			pin.Unit = cbUnit.ActiveText;
			pin.Slope = sbSlope.Value;
			pin.Offset = sbOffset.Value;
			pin.Frequency = sbFrequency.Value;
			pin.Interval = sbInterval.ValueAsInt;

			Respond (Gtk.ResponseType.Apply);
		}
	}
}

