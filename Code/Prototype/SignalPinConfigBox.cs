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
			set {
				this.lblFrame.Text = value;
				this.LabelText = value;
			}
			get{ return this.LabelText; }
		}

		public EventHandler UseAsInputToggled;

		public SignalPinConfigBox ()
		{
			this.Build ();
		}

		protected void OnCbSignalPinUseAsInputToggled (object sender, EventArgs e)
		{
			UseAsInput = (sender as CheckButton).Active;
			lblSignalPinName.Sensitive = UseAsInput;
			entrySignalPinName.Sensitive = UseAsInput;
		}

		protected void OnEntrySignalPinNameChanged (object sender, EventArgs e)
		{
			SignalPinName = (sender as Entry).Text;
		}
	}
}

