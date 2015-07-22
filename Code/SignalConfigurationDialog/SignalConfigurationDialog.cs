using System;
using System.Drawing;
using PrototypeBackend;
using Gtk;

namespace SignalConfigurationDialog
{
	public partial class SignalConfigurationDialog : Gtk.Dialog
	{
		public Signal AnalogSignal {
			get{ return analogSignal; }
			set {
				entryName.Text = value.SignalName;
				entryOperation.Text = value.SignalOperationString;
				if (value.Unit != null)
				{
					cbeUnit.AppendText (value.Unit);
					cbeUnit.Activate = cbeUnit.Data.Count - 1;
				}
					
				analogSignal = value;
			}
		}

		private Signal analogSignal;

		private Gtk.NodeStore SignalStore = 

		public SignalConfigurationDialog ()
		{
			this.Build ();

			nvSignal.NodeStore
		}

		private DrawNodeView()
		{
			
		}
	}
}

