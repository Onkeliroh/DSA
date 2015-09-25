using System;
using System.Linq;
using PrototypeBackend;
using Gtk;
using GUIHelper;
using System.Text.RegularExpressions;

namespace MeasurementCombinationDialog
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

		private string HintList = "";

		#endregion

		public MeasurementCombinationDialog (APin[] pins, MeasurementCombination signal = null, APin pin = null, Gtk.Window parent = null, string[] units = null)
			: base ("Signal Configuration", parent, Gtk.DialogFlags.Modal, new object[0])
		{
			this.Build ();

			APins = pins;

			cbColor.Color = GUIHelper.ColorHelper.GetRandomGdkColor ();

			if (signal == null)
			{
				Combination_ = new MeasurementCombination ();
				Combination_.Color = cbColor.Color;
			} else
			{
				Combination = signal;
				if (!string.IsNullOrEmpty (Combination.OperationString))
				{
					CompileOperation ();
				} else
				{
					SetWarning ();
				}
			}
	
			if (pin != null)
			{
				Combination_.AddPin (pin);
			}

			BuildUnits (units);
			SetupNodeView ();
			DrawNodeView ();
			UpdateCBPins ();
			SetApplyButton ();

			entryOperation.Activated += (sender, e) => CompileOperation ();
			entryOperation.FocusOutEvent += (o, args) => CompileOperation ();

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
					store.AppendValues (new object[]{ pin.Name + "(A" + pin.Number + ")", pin.Period });
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

		private void BuildUnits (string[] units)
		{
			if (units != null)
			{
				for (int i = 0; i < units.Length; i++)
				{
					if (!cbeUnit.Data.Contains (units [i]))
					{
						cbeUnit.AppendText (units [i]);
					}
				}
			}
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

			SetApplyButton ();
		}

		private bool CheckMeasurementsOnFrequency ()
		{
			foreach (APin i in Combination_.Pins)
			{
				foreach (APin j in Combination_.Pins)
				{
					if (i.Period - j.Period > 0.00000000000000000000001)
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

		private void CompileOperation ()
		{
			try
			{
				if (Combination_ != null)
				{
					var tmp = PrototypeBackend.OperationCompiler.CompileOperation (entryOperation.Text, Combination_.Pins.Select (o => "A" + o.Number.ToString ()).ToArray ());
					Combination_.Operation = tmp;
				}
			} catch (Exception ex)
			{
				Console.Error.WriteLine (ex);
			}

			if (Combination_.Operation != null)
			{
				Combination_.OperationString = entryOperation.Text;
			}
			SetApplyButton ();
			SetWarning ();
		}

		private void SetWarning ()
		{
			if (Combination_.Operation == null)
			{
				imageOperation.Pixbuf = global::Stetic.IconLoader.LoadIcon (this, "gtk-dialog-warning", global::Gtk.IconSize.Menu);

			} else
			{
				Combination_.OperationString = entryOperation.Text;
				imageOperation.Pixbuf = global::Stetic.IconLoader.LoadIcon (this, "gtk-apply", global::Gtk.IconSize.Menu);
			}
		}

		private void SetApplyButton ()
		{
			bool sensitive = false;
			string hint = "";
			if (Combination_.Pins.Count != 0)
			{
				sensitive = true;
			} else
			{
				hint += "- Please select at least one measurement signal\n";
				sensitive = false;
			}

			if (Combination_.Operation != null)
			{
				sensitive = true;
			} else
			{
				hint += "- Please enter a valid operation\n";
				sensitive = false;
			}

			buttonOk.Sensitive = sensitive;
			buttonOk.TooltipText = hint;
		}

		#region On...Stuff

		[GLib.ConnectBeforeAttribute]
		protected void OnSignalButtonPress (object o, ButtonPressEventArgs args)
		{
			if (args.Event.Button == 3)
			{
				Menu m = new Menu ();

				MenuItem deleteItem = new MenuItem ("Delete this measurementsignal");
				deleteItem.ButtonPressEvent += (obj, e) =>
				{
					APinSignalDialogTreeNode node = ((o as NodeView).NodeSelection.SelectedNode as APinSignalDialogTreeNode);
					Combination.Pins.RemoveAt (node.Index);
					DrawNodeView ();
					UpdateCBPins ();
					SetApplyButton ();
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
			SetApplyButton ();
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
			CompileOperation ();
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

