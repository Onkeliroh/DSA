using System;
using System.Linq;
using PrototypeBackend;
using OxyPlot;
using OxyPlot.GtkSharp;
using OxyPlot.Axes;
using Gdk;
using Gtk;
using GUIHelper;
using GLib;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Configuration;
using System.Collections.ObjectModel;


namespace SequenceConfigurationsDialog
{
	public partial class SequenceConfiguration : Gtk.Dialog
	{

		#region Member

		public Sequence PinSequence{ get { return pinSequence; } set { pinSequence = value; } }

		private Sequence pinSequence;

		//		private List<SequenceOperation> SeqOps = new List<SequenceOperation> ();

		private DPin[] DPins;

		private DPin selectedPin;

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
		private OxyPlot.GtkSharp.PlotView plotView;
		private OxyPlot.PlotModel plotModel;
		private OxyPlot.Series.LineSeries sequenceSeries;
		private LinearAxis XAxis;

		//NodeView
		private NodeStore nvSequenceOptionsStore;

		#endregion

		public SequenceConfiguration (DPin[] Pins, Sequence seq = null)
		{
			this.Build ();

			SetupNodeView ();
			SetupOxyPlot ();

			if (seq != null)
			{
				PinSequence = seq;

				DisplaySequenceInfos ();
			} else
			{
				PinSequence = new Sequence ();
			}

			DPins = Pins;

			//no DPin no Sequence
			if (DPins.Length > 0)
			{
				for (int i = 0; i < DPins.Length; i++)
				{
					cbPin.AppendText (string.Format ("{0}(D{1})", DPins [i].Name, DPins [i].Number));
				}
				cbPin.Active = 0;
			} else
			{
				buttonOk.Sensitive = false;
				buttonOk.TooltipText = "You need to first setup at least one digital Pin to define a Sequence.";
			}
		}

		private void SetupOxyPlot ()
		{
			XAxis = new OxyPlot.Axes.LinearAxis {
				Key = "X",
				Position = AxisPosition.Bottom,
				AbsoluteMinimum = 0.0,
				LabelFormatter = x =>
				{
					if (x <= 0.0)
					{
						return "Start";
					}
					return x.ToString ();// TimeSpanAxis.ToTimeSpan (x).ToString ("c");
				},
				MajorGridlineThickness = 1,
				MajorGridlineStyle = OxyPlot.LineStyle.Solid,
				MinorGridlineColor = OxyPlot.OxyColors.LightGray,
				MinorGridlineStyle = OxyPlot.LineStyle.Dot,
				MinorGridlineThickness = .5,
				MinorStep = TimeSpan.FromSeconds (10).Ticks,
				MajorStep = TimeSpan.FromSeconds (30).Ticks,
			};

			var YAxis = new OxyPlot.Axes.LinearAxis {
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
				MajorGridlineColor = OxyPlot.OxyColors.Gray,
				MajorGridlineThickness = 1,
				MajorGridlineStyle = OxyPlot.LineStyle.Solid,
			};

			sequenceSeries = new OxyPlot.Series.StairStepSeries () {
				DataFieldX = "Time",
				DataFieldY = "Value",
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
			nvSequenceOptions.AppendColumn (new TreeViewColumn ("Duration", new CellRendererText (), "text", 0));
			nvSequenceOptions.AppendColumn (new TreeViewColumn ("State", new CellRendererText (), "text", 1));

			nvSequenceOptions.ButtonPressEvent += new ButtonPressEventHandler (OnSequenceOptionsButtonPress);
			nvSequenceOptions.RowActivated += (o, args) =>
			{
				var node = ((o as NodeView).NodeSelection.SelectedNode as SequenceOperationTreeNode);
				ActiveNode = node;
				sbDays.Value = node.SeqOp.Duration.Days;
				sbHours.Value = node.SeqOp.Duration.Hours;
				sbMinutes.Value = node.SeqOp.Duration.Minutes;
				sbSeconds.Value = node.SeqOp.Duration.Seconds;
				sbMilliSec.Value = node.SeqOp.Duration.Milliseconds;

				btnRemoveOperation.Sensitive = true;
			};

			nvSequenceOptions.Show ();
		}

		private void DisplaySequenceInfos ()
		{
			btnRemoveOperation.Sensitive = false;

			nvSequenceOptionsStore.Clear ();
			for (int i = 0; i < PinSequence.Chain.Count; i++)
			{
				nvSequenceOptions.NodeStore.AddNode (new SequenceOperationTreeNode (PinSequence.Chain [i], i));
			}
			nvSequenceOptions.QueueDraw ();
			DisplayPlot ();
		}

		private void DisplayPlot ()
		{
			//TODO implement


			var current = new TimeSpan (0);
			var data = new Collection<TimeValue> ();
			for (int i = 0; i < PinSequence.Chain.Count; i++)
			{
				Console.Write (current.Ticks);
				data.Add (new TimeValue (){ Time = current, Value = ((PinSequence.Chain [i].State == DPinState.HIGH) ? 1 : 0) });
				current = current.Add (PinSequence.Chain [i].Duration);
				Console.Write ("\t" + current.Ticks + "\n");
				data.Add (new TimeValue (){ Time = current, Value = ((PinSequence.Chain [i].State == DPinState.HIGH) ? 1 : 0) });
			}

			sequenceSeries = new OxyPlot.Series.LineSeries () {
				DataFieldX = "Time",
				DataFieldY = "Value",
				ItemsSource = data,
				Color = OxyPlot.OxyColor.FromUInt32 (ColorHelper.RGBAFromGdkColor (selectedPin.PlotColor))
			};

			plotView.Model.Series.Clear ();
			plotView.Model.Series.Add (sequenceSeries);
			plotView.InvalidatePlot (true);
			plotView.Model.InvalidatePlot (true);
			plotView.ShowAll ();
		}

		[GLib.ConnectBeforeAttribute]
		protected void OnSequenceOptionsButtonPress (object o, ButtonPressEventArgs args)
		{
			if (args.Event.Button == 3) /* right click */
			{
				Menu m = new Menu ();

				MenuItem deleteItem = new MenuItem ("Delete this SequenceOperation");
				deleteItem.ButtonPressEvent += (obj, e) =>
				{
					SequenceOperationTreeNode node = ((o as NodeView).NodeSelection.SelectedNode as SequenceOperationTreeNode);
//					Console.WriteLine (node.Index);
					PinSequence.Chain.RemoveAt (node.Index);
					DisplaySequenceInfos ();
				};
				m.Add (deleteItem);
				m.ShowAll ();
				m.Popup ();
			}
			
		}

		protected void OnCbPinChanged (object sender, EventArgs e)
		{
			int nr = 0;
			var reg = Regex.Match (cbPin.ActiveText, @"\(D([0-9]+)\)");
			reg = Regex.Match (reg.Value, @"\d");
			if (reg.Success)
			{
				nr = Convert.ToInt32 (reg.Value);
			
				for (int i = 0; i < DPins.Length; i++)
				{
					if (DPins [i].Number == nr)
					{
						selectedPin = DPins [i];
						sequenceSeries.Color = OxyPlot.OxyColor.FromUInt32 (ColorHelper.RGBAFromGdkColor (DPins [i].PlotColor));
						plotView.ShowAll ();
						break;
					}
				}
			}
		}

		protected void OnButtonOkClicked (object sender, EventArgs e)
		{
			pinSequence = new Sequence {
				Name = entryName.Text,
				Pin = selectedPin,
				Repetitions = (rbRepeateContinously.Active) ? -1 : sbRadioBtnStopAfter.ValueAsInt,
			};
				
			Respond (ResponseType.Apply);
		}

		protected void OnBtnApplyOperationClicked (object sender, EventArgs e)
		{
			AddOperation (new SequenceOperation () {
				Duration = this.Duration,
				State = (cbState.ActiveText == "HIGH") ? DPinState.HIGH : DPinState.LOW,
			});
			cbState.Active = (cbState.Active == 0) ? 1 : 0;
		}

		protected void OnBtnRemoveOperationClicked (object sender, EventArgs e)
		{
			PinSequence.Chain.RemoveAt (ActiveNode.Index);
			DisplaySequenceInfos ();
			btnRemoveOperation.Sensitive = false;
		}

		private void AddOperation (SequenceOperation SeqOp)
		{
			pinSequence.Chain.Add (SeqOp);
			XAxis.AbsoluteMaximum = pinSequence.Chain.Sum (o => o.Duration.TotalMilliseconds);
			DisplaySequenceInfos ();
		}
	}

	struct TimeValue
	{
		public TimeSpan Time{ get; set; }

		public double Value { get; set; }
	}
}

