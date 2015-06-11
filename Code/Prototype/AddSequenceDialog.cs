using System;
using GLib;
using PrototypeBackend;
using System.Collections;
using System.Collections.Generic;
using Gtk;

namespace Prototype
{
	public partial class AddSequenceDialog : Gtk.Dialog
	{
		private List<Sequence> SequencesList = new List<Sequence> ();

		public Sequence[] Sequences {
			private set{ }
			get {
				return SequencesList.ToArray ();
			}
		}

		private int[] Pins_;

		public int[] Pins {
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
			}
		}

		public AddSequenceDialog () : this (new int[0])
		{
		}

		public AddSequenceDialog (int[] pins)
		{
			this.Build ();
			this.Title = "Add Sequence";
			Pins = pins;
			foreach (string tmp in Enum.GetNames(typeof(ArduinoController.DPinState)))
			{
				cBInitialState.AppendText (tmp);
			}
			cBInitialState.Active = 0;
		}

		protected void OnBtnCancelClicked (object sender, EventArgs e)
		{
			Respond (Gtk.ResponseType.Cancel);
		}

		protected void OnBtnAddClicked (object sender, EventArgs e)
		{
			string PinLabel = "";
			ArduinoController.DPinState State = StringToEnum<ArduinoController.DPinState> (cBInitialState.ActiveText); 
			ArduinoController.DPinState AltState = (State == ArduinoController.DPinState.HIGH) ? ArduinoController.DPinState.LOW : ArduinoController.DPinState.HIGH;
			if (ePinLabel.Text.Equals (""))
			{
				PinLabel = cBPins.ActiveText;
			}
			SequencesList.Add (new Sequence () {
				PinLabel = PinLabel,
				PinNr = Convert.ToInt16 (cBPins.ActiveText),
				Interval = sBInititalStateDuration.Value,
				PinType = ArduinoController.PinType.DIGITAL,
				DueTime = DateTime.Now.AddMinutes (2),
				PinState = State
			});
			for (int i = 0; i < (int)sBRepetitions.Value; i++)
			{
				SequencesList.Add (new Sequence () {
					PinLabel = PinLabel,
					PinNr = Convert.ToInt16 (cBPins.ActiveText),
					Interval = sBInititalStateDuration.Value,
					PinType = ArduinoController.PinType.DIGITAL,
					DueTime = SequencesList [0].DueTime.AddMilliseconds (i * sBInititalStateDuration.Value),
					PinState = State
				});
			}
			if (cBAlternating.Active)
			{
				SequencesList.Add (new Sequence () {
					PinLabel = PinLabel,
					PinNr = Convert.ToInt16 (cBPins.ActiveText),
					Interval = sBInititalStateDuration.Value,
					PinType = ArduinoController.PinType.DIGITAL,
					DueTime = DateTime.Now.AddMinutes (2).AddMilliseconds (sBAlternativeStateDuration.Value),
					PinState = AltState
				});
				for (int i = 0; i < (int)sBRepetitions.Value; i++)
				{
					SequencesList.Add (new Sequence () {
						PinLabel = PinLabel,
						PinNr = Convert.ToInt16 (cBPins.ActiveText),
						Interval = sBInititalStateDuration.Value,
						PinType = ArduinoController.PinType.DIGITAL,
						DueTime = SequencesList [0].DueTime.AddMilliseconds (i * sBAlternativeStateDuration.Value),
						PinState = AltState
					});
				}
				
			}


			this.Respond (Gtk.ResponseType.Apply);
		}

		protected void OnCBAlternatingToggled (object sender, EventArgs e)
		{
			sBAlternativeStateDuration.Sensitive = (sender as CheckButton).Active;
		}

		public static T StringToEnum<T> (string name)
		{
			return (T)Enum.Parse (typeof(T), name);
		}
	}
}

