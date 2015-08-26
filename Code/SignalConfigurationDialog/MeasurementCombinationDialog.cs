using System;
using System.Drawing;
using PrototypeBackend;
using Gtk;
using GUIHelper;
using System.Text.RegularExpressions;

namespace SignalConfigurationDialog
{
	public partial class MeasurementCombinationDialog : Gtk.Dialog
	{
		#region Memeber

		public MeasurementCombination Combination {
			get{ return Combination_; }
			set {
				entryName.Text = value.Name;
				entryOperation.Text = value.OperationString;
				cbColor.Color = value.Color;
				sbInterval.Value = value.Interval;

				if (value.Unit != null && !cbeUnit.Data.Contains (value.Unit))
				{
					cbeUnit.InsertText (0, value.Unit);
					cbeUnit.Active = 0;
				}
					
				Combination_ = value;
			}
		}

		private MeasurementCombination Combination_;

		private APin ActiveNode = null;

		private APin[] APins;

		private Gtk.NodeStore SignalStore = new NodeStore (typeof(APinSignalDialogTreeNode));

		#endregion

		public MeasurementCombinationDialog (APin[] pins, MeasurementCombination signal = null, APin pin = null, Gtk.Window parent = null)
			: base ("Signal Configuration", parent, Gtk.DialogFlags.Modal, new object[0])
		{
			this.Build ();

			APins = pins;

			if (signal == null)
			{
				Combination_ = new MeasurementCombination ();
			} else
			{
				Combination = signal;
			}
	
			if (pin != null)
			{
				Combination_.AddPin (pin);
			}

			SetupNodeView ();
			DrawNodeView ();
			UpdateCBPins ();
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


		private void DrawNodeView ()
		{
			nvSignal.NodeStore.Clear ();
			btnRemove.Sensitive = false;
			for (int i = 0; i < Combination_.Pins.Count; i++)
			{
				nvSignal.NodeStore.AddNode (new APinSignalDialogTreeNode (Combination_.Pins [i], i));
			}
			nvSignal.QueueDraw ();
		}

		private void UpdateCBPins ()
		{
			var store = new Gtk.ListStore (typeof(string), typeof(double));

			foreach (APin pin in APins)
			{
				if (!Combination_.Pins.Contains (pin))
				{
					// Analysis disable once CompareOfFloatsByEqualityOperator
					store.AppendValues (new object[]{ pin.Name + "(A" + pin.Number + ")", pin.Frequency });
				}
			}
			cbPins.Model = store;
			if (cbPins.Cells.Length > 0)
			{
				cbPins.Active = 0;
			}

			if (!CheckMeasurementsOnFrequency ())
			{
				lblWarning.Visible = true;
			} else
			{
				lblWarning.Visible = false;
			}

			cbPins.ShowAll ();
		}

		private void AddPin ()
		{
			//if one item is selected
			if (cbPins.Active != -1)
			{
				var reg = Regex.Match (cbPins.ActiveText, @"\(A([0-9]+)\)");
				reg = Regex.Match (reg.Value, @"\d+");
				if (reg.Success)
				{
					Combination_.AddPin (GetPins (Convert.ToInt32 (reg.Value)));
				}
			}

			UpdateCBPins ();

			DrawNodeView ();
		}

		private bool CheckMeasurementsOnFrequency ()
		{
			foreach (APin i in Combination_.Pins)
			{
				foreach (APin j in Combination_.Pins)
				{
					if (i.Frequency - j.Frequency > 0.00000000000000000000001)
					{
						return false;
					}
				}
			}
			return true;
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

		#region On...Stuff

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
					Combination.Pins.RemoveAt (node.Index);
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
				Combination.Pins.RemoveAt (((o as NodeView).NodeSelection.SelectedNode as SequenceOperationTreeNode).Index);
				DrawNodeView ();
			}
		}

		[GLib.ConnectBeforeAttribute]
		protected void OnButtonOkClicked (object sender, EventArgs e)
		{
			Combination.Name = entryName.Text;
			Combination.Unit = cbeUnit.ActiveText;
			Combination.Color = cbColor.Color;
			Combination.OperationString = entryOperation.Text;

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
				Combination_.Pins.Remove (ActiveNode);
				btnRemove.Sensitive = false;
				DrawNodeView ();
			}
			UpdateCBPins ();
		}

		protected void OnEntryNameChanged (object sender, EventArgs e)
		{
			if (Combination_ != null)
			{
				Combination_.Name = entryName.Text;
			}
		}

		protected void OnCbeUnitChanged (object sender, EventArgs e)
		{
			if (Combination_ != null)
			{
				Combination_.Unit = cbeUnit.ActiveText;
			}
		}

		protected void OnCbColorColorSet (object sender, EventArgs e)
		{
			if (Combination_ != null)
			{
				Combination_.Color = cbColor.Color;
			}
		}

		protected void OnEntryOperationChanged (object sender, EventArgs e)
		{
			try
			{
				if (Combination_ != null)
				{
					Combination_.OperationString = entryOperation.Text;
				}
			} catch (Exception ex)
			{
				Console.Error.WriteLine (ex);
			}
		}

		protected void OnSbIntervalChangeValue (object o, ChangeValueArgs args)
		{
			if (Combination_ != null)
			{
				Combination_.Interval = sbInterval.ValueAsInt;
			}
		}

		#endregion
	}
}

