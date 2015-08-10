using System;
using System.Drawing;
using PrototypeBackend;
using Gtk;
using GUIHelper;
using System.Text.RegularExpressions;

namespace SignalConfigurationDialog
{
	public partial class SignalConfigurationDialog : Gtk.Dialog
	{
		#region Memeber

		public Signal AnalogSignal {
			get{ return analogSignal; }
			set {
				entryName.Text = value.SignalName;
				entryOperation.Text = value.SignalOperationString;
				cbColor.Color = value.SignalColor;

				if (value.Unit != null && !cbeUnit.Data.Contains (value.Unit))
				{
					cbeUnit.InsertText (0, value.Unit);
					cbeUnit.Active = 0;
				}
					
				analogSignal = value;
			}
		}

		private Signal analogSignal;

		private APin ActiveNode = null;

		private APin[] APins;

		private Gtk.NodeStore SignalStore = new NodeStore (typeof(APinSignalDialogTreeNode));

		#endregion

		public SignalConfigurationDialog (APin[] pins, Signal signal = null, Gtk.Window parent = null)
			: base ("Signal Configuration", parent, Gtk.DialogFlags.Modal, new object[0])
		{
			this.Build ();

			APins = pins;

			if (signal == null)
			{
				analogSignal = new Signal ();
			} else
			{
				AnalogSignal = signal;
			}

			SetupNodeView ();
			DrawNodeView ();
			UpdateCBPins ();
//			ShowAll ();
		}

		private void SetupNodeView ()
		{
			nvSignal.NodeStore = SignalStore;
			nvSignal.AppendColumn (new TreeViewColumn ("Name(Pin)", new CellRendererText (), "text", 0));
			nvSignal.AppendColumn (new TreeViewColumn ("Frequency", new CellRendererText (), "text", 1));
			nvSignal.AppendColumn (new TreeViewColumn ("Interval", new CellRendererText (), "text", 2));

			nvSignal.ButtonPressEvent += new ButtonPressEventHandler (OnSignalButtonPress);
			nvSignal.KeyPressEvent += new KeyPressEventHandler (OnSignalKeyPress);
			nvSignal.RowActivated += (o, args) =>
			{
				var node = ((o as NodeView).NodeSelection.SelectedNode as APinSignalDialogTreeNode).Pin;
				ActiveNode = node;

				btnRemove.Sensitive = true;
			};
		}

		[GLib.ConnectBeforeAttribute]
		protected void OnSignalButtonPress (object o, ButtonPressEventArgs args)
		{
			if (args.Event.Button == 3)
			{
				Menu m = new Menu ();

				MenuItem deleteItem = new MenuItem ("Delete this SequenceOperation");
				deleteItem.ButtonPressEvent += (obj, e) =>
				{
					APinSignalDialogTreeNode node = ((o as NodeView).NodeSelection.SelectedNode as APinSignalDialogTreeNode);
					AnalogSignal.Pins.RemoveAt (node.Index);
					DrawNodeView ();
					UpdateCBPins ();
				};
				m.Add (deleteItem);
				m.ShowAll ();
				m.Popup ();
			}
		}

		[GLib.ConnectBeforeAttribute]
		protected void OnSignalKeyPress (object o, KeyPressEventArgs args)
		{
			if (args.Event.Key == Gdk.Key.Delete)
			{
				AnalogSignal.Pins.RemoveAt (((o as NodeView).NodeSelection.SelectedNode as SequenceOperationTreeNode).Index);
				DrawNodeView ();
			}
		}

		private void DrawNodeView ()
		{
			nvSignal.NodeStore.Clear ();
			btnRemove.Sensitive = false;
			for (int i = 0; i < analogSignal.Pins.Count; i++)
			{
				nvSignal.NodeStore.AddNode (new APinSignalDialogTreeNode (analogSignal.Pins [i], i));
			}
			nvSignal.QueueDraw ();
		}

		private void UpdateCBPins ()
		{
			var store = new Gtk.ListStore (typeof(string), typeof(double));

			foreach (APin pin in APins)
			{
				if (!analogSignal.Pins.Contains (pin))
				{
					// Analysis disable once CompareOfFloatsByEqualityOperator
					if (AnalogSignal.Frequency != -1)
					{
						if (Math.Abs (pin.EffectiveFrequency - AnalogSignal.Frequency) < 0.0001 && !nvSignal.NodeStore.Data.Contains (pin))
						{
							store.AppendValues (new object[]{ pin.Name + "(" + pin.Number + ")", pin.EffectiveFrequency });
						}
					} else
					{
						store.AppendValues (new object[]{ pin.Name + "(" + pin.Number + ")", pin.EffectiveFrequency });
					}
				}
			}
			cbPins.Model = store;
			if (cbPins.Cells.Length > 0)
			{
				cbPins.Active = 0;
			}
			cbPins.ShowAll ();
		}

		private void AddPin ()
		{
			//if one item is selected
			if (cbPins.Active != -1)
			{
				var reg = Regex.Match (cbPins.ActiveText, @"\(([0-9]+)\)");
				reg = Regex.Match (reg.Value, @"\d+");
				if (reg.Success)
				{
					analogSignal.AddPin (GetPins (Convert.ToInt32 (reg.Value)));
				}
			}

			UpdateCBPins ();

			DrawNodeView ();
		}

		private APin GetPins (int index)
		{
			foreach (APin pin in APins)
			{
				if (pin.Number == index)
					return pin;
			}
			return null;
		}

		[GLib.ConnectBeforeAttribute]
		protected void OnButtonOkClicked (object sender, EventArgs e)
		{
			AnalogSignal.SignalName = entryName.Text;
			AnalogSignal.Unit = cbeUnit.ActiveText;
			AnalogSignal.SignalColor = cbColor.Color;
			AnalogSignal.SignalOperationString = entryOperation.Text;

			Respond (ResponseType.Apply);
		}

		protected void OnButtonCancelClicked (object sender, EventArgs e)
		{
			Respond (ResponseType.Cancel);
		}

		protected void OnBtnAddClicked (object sender, EventArgs e)
		{
			AddPin ();
		}

		protected void OnBtnRemoveClicked (object sender, EventArgs e)
		{
			if (ActiveNode != null)
			{
				analogSignal.Pins.Remove (ActiveNode);
				btnRemove.Sensitive = false;
				DrawNodeView ();
			}
			UpdateCBPins ();
		}

		protected void OnEntryNameChanged (object sender, EventArgs e)
		{
			if (analogSignal != null)
			{
				analogSignal.SignalName = entryName.Text;
			}
		}

		protected void OnCbeUnitChanged (object sender, EventArgs e)
		{
			if (analogSignal != null)
			{
				analogSignal.Unit = cbeUnit.ActiveText;
			}
		}

		protected void OnCbColorColorSet (object sender, EventArgs e)
		{
			if (analogSignal != null)
			{
				analogSignal.SignalColor = cbColor.Color;
			}
		}

		protected void OnEntryOperationChanged (object sender, EventArgs e)
		{
			try
			{
				if (analogSignal != null)
				{
					analogSignal.SignalOperationString = entryOperation.Text;
				}
			} catch (Exception ex)
			{
				Console.Error.WriteLine (ex);
			}
		}
	}
}

