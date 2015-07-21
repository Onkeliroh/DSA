using System;
using System.Drawing;
using PrototypeBackend;

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

		public DigitalPinConfiguration ()
		{
			this.Build ();
		}

		protected void OnButtonOkClicked (object sender, EventArgs e)
		{
			pin = new DPin () {
				Name = entryName.Text,
				Number = Convert.ToInt32 (cbPin.ActiveText),
				PlotColor = Color.FromArgb (cbColor.Alpha, cbColor.Color.Red, cbColor.Color.Green, cbColor.Color.Blue)
			};

			Respond (Gtk.ResponseType.Apply);
		}

		protected void OnButtonCancelClicked (object sender, EventArgs e)
		{
			Respond (Gtk.ResponseType.Cancel);
		}
	}
}

