using System;
using System.Drawing;
using PrototypeBackend;
using Gtk;
using GUIHelper;

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
					cbeUnit.Active = cbeUnit.Data.Count - 1;
				}
					
				analogSignal = value;
			}
		}

		private Signal analogSignal;

		private Gtk.NodeStore SignalStore = new NodeStore (typeof(SignalTreeNode));

		public SignalConfigurationDialog ()
		{
			this.Build ();

			nvSignal.NodeStore = SignalStore;
			nvSignal.AppendColumn (new TreeViewColumn ("Name(Pin)", new CellRendererText (), "text", 0));
			nvSignal.AppendColumn (new TreeViewColumn ("Add/Remove", new CellRendererText (), "text", 1));
		}

		private void DrawNodeView ()
		{
//			SignalStore.AddNode(new SignalTreeNode())
			
		}
	}
}

