using System;
using System.Drawing;
using System.Linq;
using PrototypeBackend;
using Gtk;
using System.ComponentModel.Design;

namespace DigitalPinConfigurationDialog
{
	public partial class DigitalPinConfiguration : Gtk.Dialog
	{
		public DPin Pin {
			get{ return pin; }
			set {
				entryName.Text = value.Name;
				cbColor.Color = new Gdk.Color (value.PlotColor.R, value.PlotColor.G, value.PlotColor.B);
				cbPin.AppendText (value.Number.ToString ());
				cbPin.Active = cbPin.Data.Count - 1;
				pin = value;
			}
		}

		private DPin pin;

		public DigitalPinConfiguration (int[] availablePins, DPin dpin = null)
		{
			this.Build ();
			for (int i = 0; i < availablePins.Length; i++) {
				cbPin.AppendText ("D" + availablePins [i].ToString ());
			}
			if (availablePins.Length > 0) {
				cbPin.Active = 0;
			}
			if (pin != null) {
				Pin = dpin;
			} else {
				pin = null;
			}
				
		}

		protected void OnButtonOkClicked (object sender, EventArgs e)
		{
			if (entryName.Text != "") {
				pin = new DPin () {
					Name = entryName.Text,
					Number = Convert.ToInt32 (cbPin.ActiveText.Remove (0, 1)),
					PlotColor = Color.FromArgb ((byte)cbColor.Alpha, (byte)cbColor.Color.Red, (byte)cbColor.Color.Green, (byte)cbColor.Color.Blue)
				};

				Respond (Gtk.ResponseType.Apply);
			} else {
				var dialog = new MessageDialog (null, DialogFlags.Modal, MessageType.Info, ButtonsType.Ok, "Please enter a Name.", new object[]{ });
				dialog.Run ();
				dialog.Destroy ();
			}
		}

		protected void OnButtonCancelClicked (object sender, EventArgs e)
		{
			Respond (Gtk.ResponseType.Cancel);
		}
	}
}

