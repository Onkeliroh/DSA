using System;
using System.Linq;
using PrototypeBackend;
using GUIHelper;
using OxyPlot.GtkSharp;
using OxyPlot;
using OxyPlot.Axes;
using Gtk;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace Frontend
{
	public partial class SeqConfigDialog : Gtk.Dialog
	{
		#region Member

		/// <summary>
		/// Gets or sets the pin sequence and every nessesary widgets state.
		/// </summary>
		/// <value>The pin sequence.</value>
		public Sequence PinSequence {
			get { return pinSequence; }
			set {
				entryName.Text = value.Name;
				cbPin.InsertText (0, string.Format ("{0}(D{1})", value.Pin.Name, value.Pin.Number));
				if (value.Repetitions == -1) {
					rbRepeateContinously.Active = true;
				} else {
					rbStopAfter.Active = true;
					sbRadioBtnStopAfter.Value = value.Repetitions;
				}

				pinSequence = value;
			}
		}

		/// <summary>
		/// The pin sequence.
		/// </summary>
		private Sequence pinSequence;

		/// <summary>
		/// The available DPins.
		/// </summary>
		private DPin[] DPins;

		/// <summary>
		/// Gets or sets the selected pin.
		/// </summary>
		/// <value>The selected pin.</value>
		private DPin selectedPin{ get { return pinSequence.Pin; } set { pinSequence.Pin = value; } }

		/// <summary>
		/// Gets or sets the duration.
		/// </summary>
		/// <value>The duration.</value>
		public TimeSpan Duration {
			get {
				return new TimeSpan (sbDays.ValueAsInt, sbHours.ValueAsInt, sbMinutes.ValueAsInt, sbSeconds.ValueAsInt, sbMilliSec.ValueAsInt);
			}
			set {
				sbDays.Value = value.Days;
				sbHours.Value = value.Hours;
				sbMinutes.Value = value.Minutes;
				sbSeconds.Value = value.Seconds;
				sbMilliSec.Value = value.Milliseconds;
			}
		}

		/// <summary>
		/// The active node.
		/// </summary>
		private SequenceOperationTreeNode ActiveNode = null;

		//Oxyplot----
		private PlotView plotView;
		private PlotModel plotModel;
		private OxyPlot.Series.LineSeries sequenceSeries;
		private OxyPlot.Series.LineSeries repeateSeries;
		private LinearAxis XAxis;

		//NodeView
		private NodeStore nvSequenceOptionsStore;

		#endregion

		/// <summary>
		/// Initializes a new instance of the <see cref="SequenceConfigurationsDialog.SequenceConfiguration"/> class.
		/// </summary>
		/// <param name="pins">Pins.</param>
		/// <param name="groups">Groups.</param>
		/// <param name="seq">Seq.</param>
		/// <param name="RefPin">Reference pin.</param>
		/// <param name="parent">Parent.</param>
		public SeqConfigDialog (DPin[] pins, List<string> groups, Sequence seq = null, DPin RefPin = null, Gtk.Window parent = null)
			: base ("Sequence Configuration", parent, Gtk.DialogFlags.Modal, new object[0])
		{
			this.Build ();

			DPins = pins;

			//no DPin no Sequence
			if (DPins.Length > 0) {
				for (int i = 0; i < DPins.Length; i++) {
					cbPin.AppendText (string.Format ("{0}(D{1})", DPins [i].Name, DPins [i].Number));
				}
			}
			SetupNodeView ();
			SetupOxyPlot ();

			if (seq != null) {
				PinSequence = seq;
			} else {
				pinSequence = new Sequence ();
			}
			if (RefPin == null) {
				cbPin.Active = 0;
			} else {
				cbPin.Active = pins.ToList ().IndexOf (RefPin);
			}

			SetupGroups (groups);
			DisplaySequenceInfos ();
		}

		/// <summary>
		/// Setups the oxyplot.
		/// </summary>
		private void SetupOxyPlot ()
		{
			XAxis = new LinearAxis {
				Key = "X",
				Position = AxisPosition.Bottom,
				AbsoluteMinimum = TimeSpan.FromSeconds (0).Ticks,
				LabelFormatter = x => {
					if (x <= TimeSpan.FromSeconds (0).Ticks) {
						return "Start";
					}
					return string.Format ("+{0}", TimeSpan.FromSeconds (x).ToString ("c"));
				},
				MajorGridlineThickness = 1,
				MajorGridlineStyle = LineStyle.Solid,
				MinorGridlineColor = OxyColors.LightGray,
				MinorGridlineStyle = LineStyle.Dot,
				MinorGridlineThickness = .5,
			};

			var YAxis = new LinearAxis {
				Position = AxisPosition.Left,
				Minimum = -0.1,
				Maximum = 1.1,
				LabelFormatter = x => ((int)x == 0) ? "LOW" : "HIGH",
				IsPanEnabled = false,
				IsZoomEnabled = false,
				AbsoluteMaximum = 1.1,
				AbsoluteMinimum = -0.1,
				MinorStep = 1,
				MajorStep = 1,
			};

			sequenceSeries = new OxyPlot.Series.StairStepSeries () {
				DataFieldX = "Time",
				DataFieldY = "Value",
			};

			repeateSeries = new OxyPlot.Series.LineSeries () {
				DataFieldX = "Time",
				DataFieldY = "Value",
				StrokeThickness = 2,
				LineStyle = LineStyle.Dot
			};

			plotModel = new PlotModel {
				PlotType = PlotType.XY,
				Background = OxyPlot.OxyColors.White,
			};
			plotModel.Axes.Add (YAxis);
			plotModel.Axes.Add (XAxis);
			plotModel.Series.Add (sequenceSeries);
			plotView = new PlotView (){ Name = "", Model = plotModel };

			vboxOptions.Add (plotView);
			((Box.BoxChild)(vboxOptions [plotView])).Position = 2;

			plotView.SetSizeRequest (nvSequenceOptions.WidthRequest, this.HeightRequest / 3);

			plotView.ShowAll ();
		}

		/// <summary>
		/// Setups the nodeview.
		/// </summary>
		private void SetupNodeView ()
		{
			nvSequenceOptionsStore = new NodeStore (typeof(SequenceOperationTreeNode));

			nvSequenceOptions.NodeStore = nvSequenceOptionsStore;
			nvSequenceOptions.AppendColumn (new TreeViewColumn ("Time", new CellRendererText (), "text", 0));
			nvSequenceOptions.AppendColumn (new TreeViewColumn ("Duration", new CellRendererText (), "text", 1));
			nvSequenceOptions.AppendColumn (new TreeViewColumn ("State", new CellRendererText (), "text", 2));

			nvSequenceOptions.ButtonPressEvent += new ButtonPressEventHandler (OnSequenceOptionsButtonPress);
			nvSequenceOptions.KeyPressEvent += new KeyPressEventHandler (OnSequenceOptionsKeyPress);
			nvSequenceOptions.RowActivated += (o, args) => {
				var node = ((o as NodeView).NodeSelection.SelectedNode as SequenceOperationTreeNode);
				ActiveNode = node;
				sbDays.Value = node.SeqOp.Duration.Days;
				sbHours.Value = node.SeqOp.Duration.Hours;
				sbMinutes.Value = node.SeqOp.Duration.Minutes;
				sbSeconds.Value = node.SeqOp.Duration.Seconds;
				sbMilliSec.Value = node.SeqOp.Duration.Milliseconds;
				cbState.Active = (node.SeqOp.State == DPinState.HIGH) ? 0 : 1;

				btnRemoveOperation.Sensitive = true;

				SwitchToApplyBtn ();
			};

			nvSequenceOptions.Show ();
		}

		/// <summary>
		/// Setups the groups.
		/// </summary>
		/// <param name="groups">Groups.</param>
		private void SetupGroups (List<string> groups)
		{
			foreach (string s in groups) {
				cbeGroups.AppendText (s);
			}

			if (!string.IsNullOrEmpty (pinSequence.GroupName) && !string.IsNullOrWhiteSpace (pinSequence.GroupName)) {
				cbeGroups.Active = groups.IndexOf (pinSequence.GroupName);
			}

			cbeGroups.Changed += (object sender, EventArgs e) => {
				if (pinSequence != null) {
					pinSequence.GroupName = cbeGroups.ActiveText;
				}
			};
		}

		/// <summary>
		/// changes the apperance of the operations apply button.
		/// </summary>
		private void SwitchToAddBtn ()
		{
			btnApplyOperation.Label = "Add";
			btnApplyOperation.RenderIcon ("gtk-add", IconSize.Button, "");
		}

		/// <summary>
		/// changes the apperance of the operations apply button.
		/// </summary>
		private void SwitchToApplyBtn ()
		{
			btnApplyOperation.Label = "Apply";
			btnApplyOperation.RenderIcon ("gtk-apply", IconSize.Button, "");
		}

		/// <summary>
		/// Displays the sequence infos.
		/// </summary>
		private void DisplaySequenceInfos ()
		{
			if (pinSequence != null) {
				btnRemoveOperation.Sensitive = false;

				nvSequenceOptionsStore.Clear ();
				TimeSpan totalTime = new TimeSpan (0);
				for (int i = 0; i < PinSequence.Chain.Count; i++) {
					nvSequenceOptions.NodeStore.AddNode (new SequenceOperationTreeNode (PinSequence.Chain [i], i, totalTime.Ticks));
					totalTime = totalTime.Add (PinSequence.Chain [i].Duration);
				}
				nvSequenceOptions.QueueDraw ();
				DisplayPlot ();
			}
		}

		/// <summary>
		/// Displaies the plot.
		/// </summary>
		private void DisplayPlot ()
		{
			if (pinSequence != null) {
				plotView.Model.Series.Clear ();

				var current = new TimeSpan (0);
				var data = new Collection<TimeValue> ();
				for (int i = 0; i < pinSequence.Chain.Count; i++) {
					data.Add (new TimeValue () {
						Time = current,
						Value = ((pinSequence.Chain [i].State == DPinState.HIGH) ? 1 : 0)
					});
					current = current.Add (pinSequence.Chain [i].Duration);
					data.Add (new TimeValue () {
						Time = current,
						Value = ((pinSequence.Chain [i].State == DPinState.HIGH) ? 1 : 0)
					});
				}

				sequenceSeries = new OxyPlot.Series.LineSeries () {
					DataFieldX = "Time",
					DataFieldY = "Value",
					ItemsSource = data,
					StrokeThickness = 2,
					Color = ColorHelper.GdkColorToOxyColor (selectedPin.PlotColor)
				};

				repeateSeries.Color = ColorHelper.GdkColorToOxyColor (selectedPin.PlotColor);

				//next Cycle Tease
				//				if ((rbRepeateContinously.Active || (rbStopAfter.Active && sbRadioBtnStopAfter.ValueAsInt > 1)) && data.Count > 0)
				//				{
				//					var repeateData = new Collection<TimeValue> ();
				//					repeateData.Add (data.Last ());
				//					repeateData.Add (
				//						new TimeValue {
				//							Time = data.Last ().Time,
				//							Value = ((pinSequence.Chain [0].State == DPinState.HIGH) ? 1 : 0)
				//						});
				//					repeateData.Add (
				//						new TimeValue {
				//							Time = data.Last ().Time.Add (pinSequence.Chain [0].Duration),
				//							Value = ((pinSequence.Chain [0].State == DPinState.HIGH) ? 1 : 0)
				//						});
				//					repeateSeries.ItemsSource = repeateData;
				//					plotView.Model.Series.Add (repeateSeries);
				//				}

				plotView.Model.Series.Add (sequenceSeries);
				plotView.InvalidatePlot (true);
				plotView.Model.InvalidatePlot (true);
				plotView.ShowAll ();
			}
		}

		/// <summary>
		/// Creates a popup menu.
		/// </summary>
		/// <param name="o">O.</param>
		/// <param name="args">Arguments.</param>
		[GLib.ConnectBeforeAttribute]
		protected void OnSequenceOptionsButtonPress (object o, ButtonPressEventArgs args)
		{
			if (args.Event.Button == 3) { /* right click */
				Menu m = new Menu ();

				MenuItem deleteItem = new MenuItem ("Delete this SequenceOperation");
				deleteItem.ButtonPressEvent += (obj, e) => {
					SequenceOperationTreeNode node = ((o as NodeView).NodeSelection.SelectedNode as SequenceOperationTreeNode);
					PinSequence.Chain.RemoveAt (node.Index);
					DisplaySequenceInfos ();
				};
				m.Add (deleteItem);
				m.ShowAll ();
				m.Popup ();
			}
		}

		/// <summary>
		/// Removes a selected operation.
		/// </summary>
		/// <param name="o">O.</param>
		/// <param name="args">Arguments.</param>
		[GLib.ConnectBeforeAttribute]
		protected void OnSequenceOptionsKeyPress (object o, KeyPressEventArgs args)
		{
			if (args.Event.Key == Gdk.Key.Delete) {
				PinSequence.Chain.RemoveAt (((o as NodeView).NodeSelection.SelectedNode as SequenceOperationTreeNode).Index);
				DisplaySequenceInfos ();
			}
		}

		/// <summary>
		/// Changes the pin.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		protected void OnCbPinChanged (object sender, EventArgs e)
		{
			try {
				if (cbPin.ActiveText != null && selectedPin != null) {
					if (cbPin.ActiveText.Length > 0) {
						int nr = 0;
						var reg = Regex.Match (cbPin.ActiveText, @"\(D([0-9]+)\)");
						reg = Regex.Match (reg.Value, @"\d+");
						if (reg.Success) {
							nr = Convert.ToInt32 (reg.Value);

							for (int i = 0; i < DPins.Length; i++) {
								if (DPins [i].Number == nr) {
									selectedPin = DPins [i];
									sequenceSeries.Color = ColorHelper.GdkColorToOxyColor (selectedPin.PlotColor);
									repeateSeries.Color = ColorHelper.GdkColorToOxyColor (selectedPin.PlotColor);
									plotView.InvalidatePlot (true);
									plotView.ShowAll ();
									break;
								}
							}
						}
					}
				}
				// Analysis disable once EmptyGeneralCatchClause
			} catch (Exception ex) {
			}
		}

		/// <summary>
		/// Sets every member.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		[GLib.ConnectBeforeAttribute]
		protected void OnButtonOkClicked (object sender, EventArgs e)
		{
			pinSequence.Name = entryName.Text;
			pinSequence.Pin = selectedPin;
			pinSequence.Repetitions = (rbRepeateContinously.Active) ? -1 : sbRadioBtnStopAfter.ValueAsInt;
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
		/// Adds or changes a operation.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		protected void OnBtnApplyOperationClicked (object sender, EventArgs e)
		{
			var op = new SequenceOperation () {
				Duration = this.Duration,
				State = (cbState.ActiveText == "HIGH") ? DPinState.HIGH : DPinState.LOW,
			};
			if (ActiveNode == null) {
				AddOperation (op);
				cbState.Active = (cbState.Active == 0) ? 1 : 0;
			} else {
				pinSequence.Chain [ActiveNode.Index] = op;
				DisplaySequenceInfos ();
				SwitchToAddBtn ();
			}
		}

		/// <summary>
		/// Removes a operation.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		protected void OnBtnRemoveOperationClicked (object sender, EventArgs e)
		{
			if (ActiveNode != null) {
				PinSequence.Chain.RemoveAt (ActiveNode.Index);
				DisplaySequenceInfos ();
				btnRemoveOperation.Sensitive = false;
				ActiveNode = null;
			}
		}

		/// <summary>
		/// Updates the plot.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		protected void OnRbRepeateContinouslyToggled (object sender, EventArgs e)
		{
			DisplayPlot ();
		}

		/// <summary>
		/// Updates the plot.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		protected void OnRbStopAfterToggled (object sender, EventArgs e)
		{
			DisplayPlot ();
		}

		/// <summary>
		/// Updates the plot.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		protected void OnSbRadioBtnStopAfterValueChanged (object sender, EventArgs e)
		{
			DisplayPlot ();
		}

		/// <summary>
		/// Adds a operation.
		/// </summary>
		/// <param name="SeqOp">Seq op.</param>
		private void AddOperation (SequenceOperation SeqOp)
		{
			pinSequence.AddSequenceOperation (SeqOp);
			XAxis.AbsoluteMaximum = pinSequence.Chain.Sum (o => o.Duration.TotalMilliseconds);
			DisplaySequenceInfos ();
		}

		/// <summary>
		/// Updates the plot.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		protected void OnSbRadioBtnStopAfterChanged (object sender, EventArgs e)
		{
			rbStopAfter.Active = true;
			DisplayPlot ();
		}
	}
}

