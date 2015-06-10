using System;
using System.Collections.Generic;
using PrototypeBackend;

namespace Prototype
{
	public partial class AddScheduleDialog : Gtk.Dialog
	{
		private List<PrototypeBackend.MeasurementDate> DatesList = new List<PrototypeBackend.MeasurementDate> ();

		public PrototypeBackend.MeasurementDate[] Dates { 
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
				cBPins.Show ();
			}
			get{ return Pins_; }
		}

		public int Pin{ private set; get; }

		public string PinLabel{ private set; get; }

		public double Multiplier { private set; get; }

		public double Offset { private set; get; }

		public string Unit { private set; get; }

		public AddScheduleDialog () : this (new int[0])
		{
		}

		public AddScheduleDialog (int[] pins)
		{
			this.Build ();
			Pins = pins;
		}

		protected void OnEPinLabelChanged (object sender, EventArgs e)
		{
			PinLabel = ePinLabel.Text;
		}

		protected void OnCBPinsChanged (object sender, EventArgs e)
		{
			Pin = Convert.ToInt16 (cBPins.ActiveText);
		}

		protected void OnSBMultiplierChangeValue (object o, Gtk.ChangeValueArgs args)
		{
			Multiplier = sBMultiplier.Value;
		}

		protected void OnSBOffsetValueChanged (object sender, EventArgs e)
		{
			Offset = sBOffset.Value;
		}

		protected void OnEUUnitChanged (object sender, EventArgs e)
		{
			Unit = eUUnit.ActiveText;
		}

		protected void OnBtnAddClicked (object sender, EventArgs e)
		{
			DatesList.Add (new PrototypeBackend.MeasurementDate () {
				PinType = ArduinoController.PinType.ANALOG, 
				PinNr = Pin, 
				PinCmd = ArduinoController.Command.ReadAnalogPin,
				DueTime = DateTime.Now.AddMinutes (2),
				PinLabel = PinLabel
			});

			for (int i = 0; i < sBRepetitions.Value; i++)
			{
				DatesList.Add (new PrototypeBackend.MeasurementDate () {
					PinType = ArduinoController.PinType.ANALOG, 
					PinNr = Pin, 
					PinCmd = ArduinoController.Command.ReadAnalogPin,
					DueTime = DatesList [0].DueTime.AddMinutes (i + 1),
					PinLabel = PinLabel
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

