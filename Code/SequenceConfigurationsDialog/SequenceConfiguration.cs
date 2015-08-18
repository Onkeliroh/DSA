using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using Gtk;
using GUIHelper;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.GtkSharp;
using PrototypeBackend;


namespace SequenceConfigurationsDialog
{
	public partial class SequenceConfiguration : Gtk.Dialog
	{

		#region Member

		public Sequence PinSequence {
			get { return pinSequence; }
			set {
				entryName.Text = value.Name;
				cbPin.InsertText (0, string.Format ("{0}(D{1})", value.Pin.Name, value.Pin.Number));
				if (value.Repetitions == -1)
				{
					rbRepeateContinously.Active = true;
				} else
				{
					rbStopAfter.Active = true;
					sbRadioBtnStopAfter.Value = value.Repetitions;
				}

				pinSequence = value;

//				DisplaySequenceInfos ();
			}
		}

		private Sequence pinSequence;

		private DPin[] DPins;

		private DPin selectedPin{ get { return pinSequence.Pin; } set { pinSequence.Pin = value; } }

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

		public SequenceConfiguration (DPin[] pins, Sequence seq = null, DPin RefPin = null, Gtk.Window parent = null)
			: base ("Sequence Configuration", parent, Gtk.DialogFlags.Modal, new object[0])
		{
			this.Build ();

			SetupNodeView ();
			SetupOxyPlot ();

			DPins = pins;

			//no DPin no Sequence
			if (DPins.Length > 0)
			{
				for (int i = 0; i < DPins.Length; i++)
				{
					cbPin.AppendText (string.Format ("{0}(D{1})", DPins [i].Name, DPins [i].Number));
				}
			}

			if (seq != null)
			{
				PinSequence = seq;
			} else
			{
				pinSequence = new Sequence ();
			}
			if (RefPin == null)
			{
				cbPin.Active = 0;
			} else
			{
				cbPin.Active = pins.ToList ().IndexOf (RefPin);
			}
			DisplaySequenceInfos ();
		}

		private void SetupOxyPlot ()
		{
			XAxis = new LinearAxis {
				Key = "X",
				Position = AxisPosition.Bottom,
				AbsoluteMinimum = TimeSpan.FromSeconds (0).Ticks,
				LabelFormatter = x =>
				{
					if (x <= TimeSpan.FromSeconds (0).Ticks)
					{
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

		private void SetupNodeView ()
		{
			nvSequenceOptionsStore = new NodeStore (typeof(SequenceOperationTreeNode));

			nvSequenceOptions.NodeStore = nvSequenceOptionsStore;
			nvSequenceOptions.AppendColumn (new TreeViewColumn ("Time", new CellRendererText (), "text", 0));
			nvSequenceOptions.AppendColumn (new TreeViewColumn ("Duration", new CellRendererText (), "text", 1));
			nvSequenceOptions.AppendColumn (new TreeViewColumn ("State", new CellRendererText (), "text", 2));

			nvSequenceOptions.ButtonPressEvent += new ButtonPressEventHandler (OnSequenceOptionsButtonPress);
			nvSequenceOptions.KeyPressEvent += new KeyPressEventHandler (OnSequenceOptionsKeyPress);
			nvSequenceOptions.RowActivated += (o, args) =>
			{
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

		private void SwitchToAddBtn ()
		{
			btnApplyOperation.Label = "Add";
			btnApplyOperation.RenderIcon ("gtk-add", IconSize.Button, "");
		}

		private void SwitchToApplyBtn ()
		{
			btnApplyOperation.Label = "Apply";
			btnApplyOperation.RenderIcon ("gtk-apply", IconSize.Button, "");
		}

		private void DisplaySequenceInfos ()
		{
			if (pinSequence != null)
			{
				btnRemoveOperation.Sensitive = false;

				nvSequenceOptionsStore.Clear ();
				TimeSpan totalTime = new TimeSpan (0);
				for (int i = 0; i < PinSequence.Chain.Count; i++)
				{
					nvSequenceOptions.NodeStore.AddNode (new SequenceOperationTreeNode (PinSequence.Chain [i], i, totalTime.Ticks));
					totalTime = totalTime.Add (PinSequence.Chain [i].Duration);
				}
				nvSequenceOptions.QueueDraw ();
				DisplayPlot ();
			}
		}

		private void DisplayPlot ()
		{
			if (pinSequence != null)
			{
				plotView.Model.Series.Clear ();

				var current = new TimeSpan (0);
				var data = new Collection<GUIHelper.TimeValue> ();
				for (int i = 0; i < pinSequence.Chain.Count; i++)
				{
					data.Add (new GUIHelper.TimeValue () {
						Time = current,
						Value = ((pinSequence.Chain [i].State == DPinState.HIGH) ? 1 : 0)
					});
					current = current.Add (pinSequence.Chain [i].Duration);
					data.Add (new GUIHelper.TimeValue () {
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

				if ((rbRepeateContinously.Active || (rbStopAfter.Active && sbRadioBtnStopAfter.ValueAsInt > 1)) && data.Count > 0)
				{
					var repeateData = new Collection<TimeValue> ();
					repeateData.Add (data.Last ());
					repeateData.Add (
						new TimeValue {
							Time = data.Last ().Time,
							Value = ((pinSequence.Chain [0].State == DPinState.HIGH) ? 1 : 0)
						});
					repeateData.Add (
						new TimeValue {
							Time = data.Last ().Time.Add (pinSequence.Chain [0].Duration),
							Value = ((pinSequence.Chain [0].State == DPinState.HIGH) ? 1 : 0)
						});
					repeateSeries.ItemsSource = repeateData;
					plotView.Model.Series.Add (repeateSeries);
				}

				plotView.Model.Series.Add (sequenceSeries);
				plotView.InvalidatePlot (true);
				plotView.Model.InvalidatePlot (true);
				plotView.ShowAll ();
			}
		}

		[GLib.ConnectBeforeAttribute]
		protected void OnSequenceOptionsButtonPress (object o, ButtonPressEventArgs args)
		{
			if (args.Event.Button == 3)
			{ /* right click */
				Menu m = new Menu ();

				MenuItem deleteItem = new MenuItem ("Delete this SequenceOperation");
				deleteItem.ButtonPressEvent += (obj, e) =>
				{
					SequenceOperationTreeNode node = ((o as NodeView).NodeSelection.SelectedNode as SequenceOperationTreeNode);
					PinSequence.Chain.RemoveAt (node.Index);
					DisplaySequenceInfos ();
				};
				m.Add (deleteItem);
				m.ShowAll ();
				m.Popup ();
			}
		}

		[GLib.ConnectBeforeAttribute]
		protected void OnSequenceOptionsKeyPress (object o, KeyPressEventArgs args)
		{
			if (args.Event.Key == Gdk.Key.Delete)
			{
				PinSequence.Chain.RemoveAt (((o as NodeView).NodeSelection.SelectedNode as SequenceOperationTreeNode).Index);
				DisplaySequenceInfos ();
			}
		}

		protected void OnCbPinChanged (object sender, EventArgs e)
		{
			try
			{
				if (cbPin.ActiveText != null)
				{
					if (cbPin.ActiveText.Length > 0)
					{
						int nr = 0;
						var reg = Regex.Match (cbPin.ActiveText, @"\(D([0-9]+)\)");
						reg = Regex.Match (reg.Value, @"\d+");
						if (reg.Success)
						{
							nr = Convert.ToInt32 (reg.Value);

							for (int i = 0; i < DPins.Length; i++)
							{
								if (DPins [i].Number == nr)
								{
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
			} catch (Exception ex)
			{
			}
		}

		protected void OnButtonOkClicked (object sender, EventArgs e)
		{
			pinSequence.Name = entryName.Text;
			pinSequence.Pin = selectedPin;
			pinSequence.Repetitions = (rbRepeateContinously.Active) ? -1 : sbRadioBtnStopAfter.ValueAsInt;

			Respond (ResponseType.Apply);
		}

		protected void OnButtonCancelClicked (object sender, EventArgs e)
		{
			Respond (ResponseType.Cancel);
		}

		protected void OnBtnApplyOperationClicked (object sender, EventArgs e)
		{
			var op = new SequenceOperation () {
				Duration = this.Duration,
				State = (cbState.ActiveText == "HIGH") ? DPinState.HIGH : DPinState.LOW,
			};
			if (ActiveNode == null)
			{
				AddOperation (op);
				cbState.Active = (cbState.Active == 0) ? 1 : 0;
			} else
			{
				pinSequence.Chain [ActiveNode.Index] = op;
				DisplaySequenceInfos ();
				SwitchToAddBtn ();
			}
		}

		protected void OnBtnRemoveOperationClicked (object sender, EventArgs e)
		{
			if (ActiveNode != null)
			{
				PinSequence.Chain.RemoveAt (ActiveNode.Index);
				DisplaySequenceInfos ();
				btnRemoveOperation.Sensitive = false;
				ActiveNode = null;
			}
		}

		protected void OnRbRepeateContinouslyToggled (object sender, EventArgs e)
		{
			DisplayPlot ();
		}

		protected void OnRbStopAfterToggled (object sender, EventArgs e)
		{
			DisplayPlot ();
		}

		protected void OnSbRadioBtnStopAfterValueChanged (object sender, EventArgs e)
		{
			DisplayPlot ();
		}

		private void AddOperation (SequenceOperation SeqOp)
		{
			pinSequence.Chain.Add (SeqOp);
			XAxis.AbsoluteMaximum = pinSequence.Chain.Sum (o => o.Duration.TotalMilliseconds);
			DisplaySequenceInfos ();
		}

		protected void OnSbRadioBtnStopAfterChanged (object sender, EventArgs e)
		{
			rbStopAfter.Active = true;
			DisplayPlot ();
		}
	}

	//	struct TimeValue
	//	{
	//		public TimeSpan Time{ get; set; }
	//
	//		public double Value { get; set; }
	//	}
}

