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
		private List<DPin> SequencesList = new List<DPin> ();

		public DPin[] Sequences {
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
			foreach (string tmp in Enum.GetNames(typeof(PrototypeBackend.DPinState)))
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
			PrototypeBackend.DPinState State = StringToEnum<PrototypeBackend.DPinState> (cBInitialState.ActiveText); 
			PrototypeBackend.DPinState AltState = (State == PrototypeBackend.DPinState.HIGH) ? PrototypeBackend.DPinState.LOW : PrototypeBackend.DPinState.HIGH;
			if (ePinLabel.Text.Equals (""))
			{
				PinLabel = cBPins.ActiveText;
			} else
			{
				PinLabel = ePinLabel.Text;
			}
	
			//todo erstelle pin config

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

