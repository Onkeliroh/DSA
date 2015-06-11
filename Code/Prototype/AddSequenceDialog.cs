using System;
using GLib;
using PrototypeBackend;
using System.Collections.Generic;
using Gtk;

namespace Prototype
{
	public partial class AddSequenceDialog : Gtk.Dialog
	{
		private List<Sequence> SequencesList = new List<Sequence>();

		public Sequence[] Sequences{
			private set{ }
			get{
				return SequencesList.ToArray ();
			}
		}

		private int[] Pins_;

		public int[] Pins{
			set{
				Pins_ = new int[(value as int[]).Length];
				Pins_ = (value as int[]);
				foreach (int i in Pins_) {
					cBPins.AppendText (i.ToString());
				}
				if (Pins_.Length > 0) {
					cBPins.Active = 0;
				}
			}
		}

		public AddSequenceDialog ()
		{
			this.Build ();
		}

		protected void OnBtnCancelClicked (object sender, EventArgs e)
		{
			Respond (Gtk.ResponseType.Cancel);
		}

		protected void OnBtnAddClicked (object sender, EventArgs e)
		{
			Respond (Gtk.ResponseType.Apply);
		}

		protected void OnCBAlternatingToggled (object sender, EventArgs e)
		{
			sBAlternativeStateDuration.Sensitive = (sender as CheckButton).Active;
		}
	}
}

