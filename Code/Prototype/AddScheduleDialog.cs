using System;
using System.Collections.Generic;
using PrototypeBackend;
using Gtk;

namespace Prototype
{
	public partial class AddScheduleDialog : Gtk.Dialog
	{
		private List<PrototypeBackend.APin> DatesList = new List<PrototypeBackend.APin> ();

		public PrototypeBackend.APin[] Dates { 
			private set { } 
			get { return DatesList.ToArray (); } 
		}

		private int[] Pins_;

		public int []Pins {
			set {
				Pins_ = new int[(value as int[]).Length];
				Pins_ = (value as int[]);
				foreach (int i in Pins_)
				{
					cBPins.AppendText (i.ToString ());
				}
				if (Pins_.Length > 0)
				{
					cBPins.Active = 0;
				}
				cBPins.Show ();
			}
			get{ return Pins_; }
		}

		public AddScheduleDialog () : this (new int[0])
		{
		}

		public AddScheduleDialog (int[] pins)
		{
			this.Build ();
			this.Title = "Add Schedule";
			Pins = pins;
		}


		protected void OnBtnAddClicked (object sender, EventArgs e)
		{
			string PinLabel = "";
			if (ePinLabel.Text.Equals (""))
			{
				PinLabel = cBPins.ActiveText;
			}
			DatesList.Add (new PrototypeBackend.APin () {
				Type = PrototypeBackend.PinType.ANALOG, 
				Number = Convert.ToInt16 (cBPins.ActiveText), 
//				PinCmd = ArduinoController.Command.ReadAnalogPin,
//				DueTime = DateTime.Now.AddMinutes (2),
				Name = PinLabel,
			});

			for (int i = 0; i < sBRepetitions.Value; i++)
			{
				DatesList.Add (new PrototypeBackend.APin () {
					Type = PrototypeBackend.PinType.ANALOG, 
					Number = Convert.ToInt16 (cBPins.ActiveText), 
//					PinCmd = ArduinoController.Command.ReadAnalogPin,
//					DueTime = DatesList [0].DueTime.AddMinutes (i + 1),
					Name = PinLabel
				});
			}

			this.Respond (Gtk.ResponseType.Apply);
		}

		protected void OnBtnCancelClicked (object sender, EventArgs e)
		{
			this.Respond (Gtk.ResponseType.Cancel);
		}
	}
}

