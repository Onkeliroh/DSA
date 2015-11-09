using System;
using System.Linq;
using PrototypeBackend;
using Gtk;

namespace Frontend
{
	public partial class DPinConfigDialog : Gtk.Dialog
	{
		/// <summary>
		/// Gets or sets the pin and every widget state acordingly.
		/// </summary>
		/// <value>The pin.</value>
		public DPin Pin {
			get{ return pin; }
			set {
				entryName.Text = value.Name;
				cbColor.Color = value.PlotColor;
				cbPin.InsertText (0, value.DisplayNumber);
				cbPin.Active = 0;
				pin = value;
			}
		}

		/// <summary>
		/// The pin.
		/// </summary>
		private DPin pin;

		/// <summary>
		/// The available pins.
		/// </summary>
		private DPin[] AvailablePins;

		/// <summary>
		/// Initializes a new instance of the <see cref="DigitalPinConfigurationDialog.DigitalPinConfiguration"/> class.
		/// </summary>
		/// <param name="availablePins">Available pins.</param>
		/// <param name="dpin">Dpin.</param>
		/// <param name="parent">Parent.</param>
		public DPinConfigDialog (DPin[] availablePins, DPin dpin = null, Gtk.Window parent = null)
			: base ("Digital Output - Dialog", parent, Gtk.DialogFlags.Modal, new object[0])
		{
			this.Build ();

			this.FocusChain = new Widget[]{ entryName, cbPin, cbColor, buttonOk, buttonCancel };

			if (dpin != null)
			{
				AvailablePins = new DPin[availablePins.Length + 1];
				Array.Copy (availablePins, AvailablePins, availablePins.Length);
				AvailablePins [availablePins.Length] = dpin;
			} else
			{
				AvailablePins = availablePins;
			}

			if (dpin != null)
			{
				Pin = dpin;
			} else
			{
				if (AvailablePins.Length > 0)
				{
					pin = AvailablePins [0];
				}
				pin.PlotColor = GUIHelper.ColorHelper.GetRandomGdkColor ();
				cbColor.Color = pin.PlotColor;
			}

			for (int i = 0; i < availablePins.Length; i++)
			{
				cbPin.AppendText (availablePins [i].DisplayNumber);
			}
			if (availablePins.Length > 0)
			{
				cbPin.Active = 0;
			} else
			{
				buttonOk.Sensitive = false;
				buttonOk.TooltipText = "There are no more Pins left to configure.";
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
		}

		/// <summary>
		/// Raises the button ok clicked event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		[GLib.ConnectBeforeAttribute]
		[Obsolete]
		protected void OnButtonOkClicked (object sender, EventArgs e)
		{
			pin.Name = entryName.Text;
//			pin.Number = AvailablePins.Where (o => o.DisplayNumber == cbPin.ActiveText).ToList () [0].Number;
			pin.PlotColor = cbColor.Color;
//			pin.RealNumber = AvailablePins.Single (o => o.DisplayNumber == cbPin.ActiveText).RealNumber;
		}

		/// <summary>
		/// Raises the button cancel clicked event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		[Obsolete]
		protected void OnButtonCancelClicked (object sender, EventArgs e)
		{
			Respond (Gtk.ResponseType.Cancel);
		}

		/// <summary>
		/// Raises the entry name changed event.
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
		/// Raises the cb pin changed event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		protected void OnCbPinChanged (object sender, EventArgs e)
		{
			if (pin != null)
			{
				pin.RealNumber = AvailablePins.ToList ().Single (o => o.DisplayNumber == cbPin.ActiveText).RealNumber;
				pin.Number = AvailablePins.ToList ().Single (o => o.DisplayNumber.Equals (cbPin.ActiveText)).Number;
			}
		}

		/// <summary>
		/// Raises the cb color color set event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		protected void OnCbColorColorSet (object sender, EventArgs e)
		{
			pin.PlotColor = cbColor.Color;
		}
	}
}

