using System;
using System.Linq;
using PrototypeBackend;
using Gtk;
using GUIHelper;
using System.Text.RegularExpressions;

namespace MeasurementCombinationDialog
{
	/// <summary>
	/// Measurement combination dialog.
	/// </summary>
	public partial class MeasurementCombinationDialog : Gtk.Dialog
	{
		#region Memeber

		/// <summary>
		/// Gets or sets the combination.
		/// </summary>
		/// <value>The combination.</value>
		public MeasurementCombination Combination {
			get{ return Combination_; }
			set {
				entryName.Text = value.Name;
				entryOperation.Text = value.OperationString;
				cbColor.Color = value.Color;
				sbMeanValuesCount.Value = value.MeanValuesCount;

				if (value.Unit != null && !cbeUnit.Data.Contains (value.Unit))
				{
					cbeUnit.InsertText (0, value.Unit);
					cbeUnit.Active = 0;
				} 
					
				Combination_ = value;
			}
		}

		/// <summary>
		/// The combination.
		/// </summary>
		private MeasurementCombination Combination_;

		/// <summary>
		/// The active node.
		/// </summary>
		private APin ActiveNode = null;

		/// <summary>
		/// The available APins
		/// </summary>
		private APin[] APins;

		/// <summary>
		/// The signal store.
		/// </summary>
		private Gtk.NodeStore SignalStore = new NodeStore (typeof(APinSignalDialogTreeNode));

		/// <summary>
		/// The compile timer.
		/// </summary>
		private System.Timers.Timer CompileTimer = new System.Timers.Timer (1000);

		#endregion

		/// <summary>
		/// Initializes a new instance of the <see cref="MeasurementCombinationDialog.MeasurementCombinationDialog"/> class.
		/// </summary>
		/// <param name="pins">Pins.</param>
		/// <param name="signal">Signal.</param>
		/// <param name="pin">Pin.</param>
		/// <param name="parent">Parent.</param>
		/// <param name="units">Units.</param>
		public MeasurementCombinationDialog (APin[] pins, MeasurementCombination signal = null, APin pin = null, Gtk.Window parent = null, string[] units = null)
			: base ("Signal Configuration", parent, Gtk.DialogFlags.Modal, new object[0])
		{
			this.Build ();

			sbMeanValuesCount.Adjustment.Upper = int.MaxValue;
			sbMeanValuesCount.Adjustment.Lower = 1;

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

			entryOperation.Activated += (sender, e) =>
			{
				if (!CompileTimer.Enabled)
				{
					CompileTimer.Start ();
				}
			};
			entryOperation.FocusInEvent += (sender, e) =>
			{
				if (!CompileTimer.Enabled)
				{
					CompileTimer.Start ();
				}
			};
//			entryOperation.Activated += (sender, e) => CompileOperation ();
//			entryOperation.FocusOutEvent += (o, args) => CompileOperation ();

			CompileTimer.Elapsed += CompileTimerElapsed;
		}

		/// <summary>
		/// Raised by the <see cref="CompileTimer"/>
		/// </summary>
		/// <param name="obj">Object.</param>
		/// <param name="args">Arguments.</param>
		private void CompileTimerElapsed (object obj, System.Timers.ElapsedEventArgs args)
		{
			CompileOperation ();
		}

		/// <summary>
		/// Setups the nodeview.
		/// </summary>
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

		/// <summary>
		/// Draws the nodeview.
		/// </summary>
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

		/// <summary>
		/// Updates the pins combobox.
		/// </summary>
		private void UpdateCBPins ()
		{
			var store = new Gtk.ListStore (typeof(string), typeof(double));

			foreach (APin pin in APins)
			{
				if (!Combination_.Pins.Contains (pin))
				{
					// Analysis disable once CompareOfFloatsByEqualityOperator
					store.AppendValues (new object[]{ pin.Name + "(A" + pin.Number + ")", pin.Interval });
				}
			}
			cbPins.Model = store;
			if (cbPins.Cells.Length > 0)
			{
				cbPins.Active = 0;
			}

			if (!CheckMeasurementsOnInterval ())
			{
				lblWarning.Visible = true;
			} else
			{
				lblWarning.Visible = false;
			}

			cbPins.ShowAll ();
		}

		/// <summary>
		/// Fills the unit combobox with options.
		/// </summary>
		/// <param name="units">Units.</param>
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

		/// <summary>
		/// Adds the pin to the combination and updates the nessesary widgets.
		/// </summary>
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

		/// <summary>
		/// Checks the interval of the selected pins on whether their interval ist equal or different.
		/// </summary>
		/// <returns><c>true</c>, if intervals are equal, <c>false</c> otherwise.</returns>
		private bool CheckMeasurementsOnInterval ()
		{
			foreach (APin i in Combination_.Pins)
			{
				foreach (APin j in Combination_.Pins)
				{
					if (i.Interval - j.Interval > 0.00000000000000000000001)
					{
						return false;
					}
				}
			}
			return true;
		}

		/// <summary>
		/// Gets the pins.
		/// </summary>
		/// <returns>The pins.</returns>
		/// <param name="index">Index.</param>
		private APin GetPins (int index)
		{
			foreach (APin pin in APins)
			{
				if (pin.Number == index)
					return pin;
			}
			return null;
		}

		/// <summary>
		/// Compiles the operation.
		/// </summary>
		private void CompileOperation ()
		{
			try
			{
				if (Combination_ != null)
				{
					var tmp = PrototypeBackend.OperationCompiler.CompileOperation (
						          entryOperation.Text,
						          Combination_.Pins.Select (o => "A" + o.Number.ToString ()).ToArray ()
					          );
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

		/// <summary>
		/// Sets the warning label.
		/// </summary>
		private void SetWarning ()
		{
			if (Combination_.Operation == null)
			{
				imageOperation.Pixbuf = global::Stetic.IconLoader.LoadIcon (this, "gtk-dialog-warning", global::Gtk.IconSize.Menu);

			} else
			{
				Combination_.OperationString = entryOperation.Text;
				imageOperation.Pixbuf = global::Stetic.IconLoader.LoadIcon (this, "gtk-apply", global::Gtk.IconSize.Menu);
				CompileTimer.Stop ();
			}
		}

		/// <summary>
		/// Sets the apply button on whether it is sensitive or not and if not adds a tooltip containing why not.
		/// </summary>
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

		/// <summary>
		/// Creates a popup menu
		/// </summary>
		/// <param name="o">O.</param>
		/// <param name="args">Arguments.</param>
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

		/// <summary>
		/// Removes a selected pin from the combination.
		/// </summary>
		/// <param name="o">O.</param>
		/// <param name="args">Arguments.</param>
		[GLib.ConnectBeforeAttribute]
		protected void OnSignalKeyPress (object o, KeyPressEventArgs args)
		{
			if (args.Event.Key == Gdk.Key.Delete)
			{
				Combination.Pins.RemoveAt (((o as NodeView).NodeSelection.SelectedNode as SequenceOperationTreeNode).Index);
				DrawNodeView ();
			}
		}

		/// <summary>
		/// Sets every combination member.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		[GLib.ConnectBeforeAttribute]
		protected void OnButtonOkClicked (object sender, EventArgs e)
		{
			Combination.Name = entryName.Text;
			Combination.Unit = cbeUnit.ActiveText;
			Combination.Color = cbColor.Color;
			Combination.OperationString = entryOperation.Text;
		}

		/// <summary>
		/// Raises the button cancel clicked event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		protected void OnButtonCancelClicked (object sender, EventArgs e)
		{
			Respond (ResponseType.Cancel);
		}

		/// <summary>
		/// Adds a selected pin to the combination.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		protected void OnBtnAddClicked (object sender, EventArgs e)
		{
			AddPin ();
		}

		/// <summary>
		/// Removes a selected pin from the combination.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
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

		/// <summary>
		///	Sets the combinations name.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		protected void OnEntryNameChanged (object sender, EventArgs e)
		{
			if (Combination_ != null)
			{
				Combination_.Name = entryName.Text;
			}
		}

		/// <summary>
		/// Sets the combinations unit. 
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		protected void OnCbeUnitChanged (object sender, EventArgs e)
		{
			if (Combination_ != null)
			{
				Combination_.Unit = cbeUnit.ActiveText;
			}
		}

		/// <summary>
		/// Sets the combinations color. 
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		protected void OnCbColorColorSet (object sender, EventArgs e)
		{
			if (Combination_ != null)
			{
				Combination_.Color = cbColor.Color;
			}
		}

		/// <summary>
		/// Compiles the operation. 
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		protected void OnEntryOperationChanged (object sender, EventArgs e)
		{
			CompileOperation ();
		}

		/// <summary>
		/// Sets the number of mean values needed. 
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		protected void OnSbMeanValuesCountChanged (object sender, EventArgs e)
		{
			if (Combination_ != null)
			{
				Combination_.MeanValuesCount = sbMeanValuesCount.ValueAsInt;
			}
		}

		#endregion
	}
}

