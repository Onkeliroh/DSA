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
			} else
			{
				PinLabel = ePinLabel.Text;
			}
	
			if (cBAlternating.Active)
			{

				SequencesList.Add (new Sequence () {
					PinLabel = PinLabel,
					PinNr = Convert.ToInt16 (cBPins.ActiveText),
//					Interval = sBInititalStateDuration.Value,
					PinType = ArduinoController.PinType.DIGITAL,
//					DueTime = DateTime.Now,
					PinState = State,
					PinCmd = () => MainClass.mainController.ArduinoController_.SetPin (Convert.ToInt16 (cBPins.ActiveText), ArduinoController.PinMode.OUTPUT, State)
				});
				if (cBAlternating.Active)
				{
					SequencesList.Add (new Sequence () {
						PinLabel = PinLabel,
						PinNr = Convert.ToInt16 (cBPins.ActiveText),
//						Interval = sBInititalStateDuration.Value,
						PinType = ArduinoController.PinType.DIGITAL,
//						DueTime = SequencesList [0].DueTime.AddMilliseconds (sBAlternativeStateDuration.Value),
						PinState = AltState,
						PinCmd = () => MainClass.mainController.ArduinoController_.SetPin (Convert.ToInt16 (cBPins.ActiveText), ArduinoController.PinMode.OUTPUT, AltState)
					});
				}
				for (int i = 1; i <= (int)sBRepetitions.Value; i++)
				{
					SequencesList.Add (new Sequence () {
						PinLabel = PinLabel,
						PinNr = Convert.ToInt16 (cBPins.ActiveText),
//						Interval = sBInititalStateDuration.Value,
						PinType = ArduinoController.PinType.DIGITAL,
//						DueTime = SequencesList [SequencesList.Count - 1].DueTime.AddMilliseconds (sBInititalStateDuration.Value),
						PinState = State,
						PinCmd = () => MainClass.mainController.ArduinoController_.SetPin (Convert.ToInt16 (cBPins.ActiveText), ArduinoController.PinMode.OUTPUT, State)
					});
					SequencesList.Add (new Sequence () {
						PinLabel = PinLabel,
						PinNr = Convert.ToInt16 (cBPins.ActiveText),
//						Interval = sBInititalStateDuration.Value,
						PinType = ArduinoController.PinType.DIGITAL,
//						DueTime = SequencesList [SequencesList.Count - 1].DueTime.AddMilliseconds (sBAlternativeStateDuration.Value),
						PinState = AltState,
						PinCmd = () => MainClass.mainController.ArduinoController_.SetPin (Convert.ToInt16 (cBPins.ActiveText), ArduinoController.PinMode.OUTPUT, AltState)
					});
				}
			} else
			{
				SequencesList.Add (new Sequence () {
					PinLabel = PinLabel,
					PinNr = Convert.ToInt16 (cBPins.ActiveText),
//					Interval = sBInititalStateDuration.Value,
					PinType = ArduinoController.PinType.DIGITAL,
//					DueTime = DateTime.Now,
					PinState = State,
					PinCmd = () => MainClass.mainController.ArduinoController_.SetPin (Convert.ToInt16 (cBPins.ActiveText), ArduinoController.PinMode.OUTPUT, State)
				});
				for (int i = 0; i < (int)sBRepetitions.Value; i++)
				{
					SequencesList.Add (new Sequence () {
						PinLabel = PinLabel,
						PinNr = Convert.ToInt16 (cBPins.ActiveText),
//						Interval = sBInititalStateDuration.Value,
						PinType = ArduinoController.PinType.DIGITAL,
//						DueTime = SequencesList [i].DueTime.AddMilliseconds (sBInititalStateDuration.Value),
						PinState = State,
						PinCmd = () => MainClass.mainController.ArduinoController_.SetPin (Convert.ToInt16 (cBPins.ActiveText), ArduinoController.PinMode.OUTPUT, State)
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

