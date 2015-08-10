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
				cbColor.Color = value.PlotColor;
				cbPin.InsertText (0, "D" + value.Number.ToString ());
				cbPin.Active = 0;
				pin = value;
			}
		}

		private DPin pin;

		public DigitalPinConfiguration (int[] availablePins, DPin dpin = null, Gtk.Window parent = null)
			: base ("Digital Pin Configuration", parent, Gtk.DialogFlags.Modal, new object[0])
		{
			this.Build ();

			this.FocusChain = new Widget[]{ entryName, cbPin, cbColor, buttonOk, buttonCancel };

			if (dpin != null)
			{
				Pin = dpin;
			} else
			{
				pin = new DPin ();
			}

			for (int i = 0; i < availablePins.Length; i++)
			{
				cbPin.AppendText ("D" + availablePins [i].ToString ());
			}
			if (availablePins.Length > 0)
			{
				cbPin.Active = 0;
			}
		}

		[GLib.ConnectBeforeAttribute]
		protected void OnButtonOkClicked (object sender, EventArgs e)
		{
			pin.Name = entryName.Text;
			pin.Number = Convert.ToInt32 (cbPin.ActiveText.Remove (0, 1));
			pin.PlotColor = cbColor.Color;

			Respond (Gtk.ResponseType.Apply);
		}

		protected void OnButtonCancelClicked (object sender, EventArgs e)
		{
			Respond (Gtk.ResponseType.Cancel);
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
			try
			{
				if (pin != null)
				{
					pin.Number = Convert.ToInt32 (cbPin.ActiveText.Remove (0, 1));
				}
			} catch (Exception ee)
			{
			}
		}

		protected void OnCbColorColorSet (object sender, EventArgs e)
		{
			pin.PlotColor = cbColor.Color;
		}
	}
}

