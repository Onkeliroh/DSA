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
				cbColor.Color = new Gdk.Color (value.PlotColor.R, value.PlotColor.G, value.PlotColor.B);
				cbPin.AppendText ("A" + value.Number.ToString ());
				cbPin.Active = cbPin.Data.Count - 1;

				if (!cbUnit.Data.Contains (value.Unit))
				{
					cbUnit.AppendText (value.Unit);
					cbUnit.Active = cbUnit.Data.Count - 1;
				}

				sbSlope.Value = value.Slope;
				sbOffset.Value = value.Offset;
				sbFrequency.Value = value.Frequency;
				sbInterval.Value = value.Interval;

				pin = value;
			}
		}

		private APin pin;

		public AnalogPinConfiguration (int[] availablePins, APin apin = null)
		{
			this.Build ();

			sbFrequency.Adjustment.Lower = double.MinValue;
			sbFrequency.Adjustment.Upper = double.MaxValue;
			sbSlope.Adjustment.Lower = double.MinValue;
			sbSlope.Adjustment.Upper = double.MaxValue;
			sbOffset.Adjustment.Lower = double.MinValue;
			sbOffset.Adjustment.Upper = double.MaxValue;
			sbInterval.Adjustment.Upper = int.MaxValue;

			for (int i = 0; i < availablePins.Length; i++)
			{
				cbPin.AppendText ("A" + availablePins [i].ToString ());
			}

			if (availablePins.Length > 0)
			{
				cbPin.Active = 0;
			}

			if (apin != null)
			{
				Pin = apin;
			} else
			{
				pin = apin;
			}
		}

		protected void OnButtonOkClicked (object sender, EventArgs e)
		{
			if (entryName.Text != "")
			{
				pin = new APin () {
					Name = entryName.Text,
					Number = Convert.ToInt32 (cbPin.ActiveText.Remove (0, 1)),
					PlotColor = Color.FromArgb ((byte)cbColor.Alpha, (byte)cbColor.Color.Red, (byte)cbColor.Color.Green, (byte)cbColor.Color.Blue),

					Unit = cbUnit.ActiveText,
					Slope = sbSlope.Value,
					Offset = sbOffset.Value,
					Frequency = sbFrequency.Value,
					Interval = sbInterval.ValueAsInt,
				};

				Respond (Gtk.ResponseType.Apply);
			} else
			{
				var dialog = new MessageDialog (null, DialogFlags.Modal, MessageType.Info, ButtonsType.Ok, "Please enter a Name.", new object[]{ });
				dialog.Run ();
				dialog.Destroy ();
			}
		}
	}
}

