using System;
using Gtk;
using Gdk;

namespace Prototype
{
	[System.ComponentModel.ToolboxItem (true)]
	public partial class SignalPinConfigBox : Gtk.Bin
	{
		public bool UseAsInput{ private set; get; }

		public string SignalPinName { private set; get; }

		public string LabelText {
			set { this.lblFrame.Text = value; }
			get{ return this.lblFrame.Text; }
		}

		public EventHandler UseAsInputToggled;

		public SignalPinConfigBox ()
		{
			Build ();
		}

		protected void OnCbSignalPinUseAsInputToggled (object sender, EventArgs e)
		{
			UseAsInput = (sender as CheckButton).Active;
			entrySignalPinName.Sensitive = UseAsInput;
			entryUnit.Sensitive = UseAsInput;
			spMultiplier.Sensitive = UseAsInput;
			spOffset.Sensitive = UseAsInput;
		}

		protected void OnEntrySignalPinNameChanged (object sender, EventArgs e)
		{
			SignalPinName = (sender as Entry).Text;
		}
	}
}

