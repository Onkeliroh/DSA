using System;
using System.Drawing;
using PrototypeBackend;
using Gtk;
using GUIHelper;

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
				if (value.Unit != null && !cbeUnit.Data.Contains (value.Unit)) {
					cbeUnit.AppendText (value.Unit);
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

			if (signal == null) {
				analogSignal = new Signal ();
			} else {
				analogSignal = signal;
			}

			UpdateCBPins ();
			SetupNodeView ();
			DrawNodeView ();
			ShowAll ();
		}

		private void SetupNodeView ()
		{
			nvSignal.NodeStore = SignalStore;
			nvSignal.AppendColumn (new TreeViewColumn ("Name(Pin)", new CellRendererText (), "text", 0));
			nvSignal.AppendColumn (new TreeViewColumn ("Frequency", new CellRendererText (), "text", 1));
			nvSignal.AppendColumn (new TreeViewColumn ("Interval", new CellRendererText (), "text", 2));

			nvSignal.ButtonPressEvent += new ButtonPressEventHandler (OnSignalButtonPress);
			nvSignal.KeyPressEvent += new KeyPressEventHandler (OnSignalKeyPress);
			nvSignal.RowActivated += (o, args) => {
				var node = ((o as NodeView).NodeSelection.SelectedNode as APinSignalDialogTreeNode).Pin;
				ActiveNode = node;

				btnRemove.Sensitive = true;
			};
		}

		[GLib.ConnectBeforeAttribute]
		protected void OnSignalButtonPress (object o, ButtonPressEventArgs args)
		{
			if (args.Event.Button == 3) {
				Menu m = new Menu ();

				MenuItem deleteItem = new MenuItem ("Delete this SequenceOperation");
				deleteItem.ButtonPressEvent += (obj, e) => {
					SequenceOperationTreeNode node = ((o as NodeView).NodeSelection.SelectedNode as SequenceOperationTreeNode);
					AnalogSignal.Pins.RemoveAt (node.Index);
					DrawNodeView ();
				};
				m.Add (deleteItem);
				m.ShowAll ();
				m.Popup ();
			}
		}

		[GLib.ConnectBeforeAttribute]
		protected void OnSignalKeyPress (object o, KeyPressEventArgs args)
		{
			if (args.Event.Key == Gdk.Key.Delete) {
				AnalogSignal.Pins.RemoveAt (((o as NodeView).NodeSelection.SelectedNode as SequenceOperationTreeNode).Index);
				DrawNodeView ();
			}
		}

		private void DrawNodeView ()
		{
			nvSignal.NodeStore.Clear ();
			btnRemove.Sensitive = false;
			for (int i = 0; i < analogSignal.Pins.Count; i++) {
				nvSignal.NodeStore.AddNode (new APinSignalDialogTreeNode (analogSignal.Pins [i], i));
			}
			nvSignal.QueueDraw ();
		}

		private void UpdateCBPins ()
		{
			cbPins = new ComboBox ();
//			cbPins.Clear ();

			foreach (APin pin in APins) {
				// Analysis disable once CompareOfFloatsByEqualityOperator
				if (AnalogSignal.Frequency != -1) {
					if (Math.Abs (pin.EffectiveFrequency - AnalogSignal.Frequency) < 0.0001 && !nvSignal.NodeStore.Data.Contains (pin)) {
						cbPins.AppendText (pin.Name + "(" + pin.Number + ")");
					}
				} else {
					cbPins.AppendText (pin.Name + "(" + pin.Number + ")");
				}
			}
			if (cbPins.Data.Count > 0) {
				cbPins.Active = 0;
			}
			cbPins.ShowAll ();
		}

		private void AddPin ()
		{
//			analogSignal.AddPin (APins [cbPins.Active]);

			UpdateCBPins ();

			DrawNodeView ();
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
			if (ActiveNode != null) {
				AnalogSignal.Pins.Remove (ActiveNode);
				btnRemove.Sensitive = false;
				DrawNodeView ();
			}
		}
	}
}

