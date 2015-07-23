using System;
using PrototypeBackend;
using OxyPlot;
using OxyPlot.GtkSharp;
using OxyPlot.Axes;
using Gdk;
using Gtk;
using GUIHelper;


namespace SequenceConfigurationsDialog
{
	public partial class SequenceConfiguration : Gtk.Dialog
	{
		public Sequence PinSequence{ get { return pinSequence; } set { pinSequence = value; } }

		private Sequence pinSequence;

		//Oxyplot----
		private OxyPlot.GtkSharp.PlotView plotView;
		private OxyPlot.PlotModel plotModel;
		private OxyPlot.Series.LineSeries sequenceSeries;

		//NodeView
		private NodeStore nvSequenceOptionsStore = new NodeStore (typeof(SequenceTreeNode));

		public SequenceConfiguration ()
		{
			#region DEBUG
			PinSequence = new Sequence () {
				Pin = new DPin () {
					Name = "Pin of Awesome",
					PlotColor = new Gdk.Color (0xff, 0x04, 0x08),
				}
			};

			#endregion
			this.Build ();

			SetupNodeView ();
			SetupOxyPlot ();
		}

		private void SetupOxyPlot ()
		{
			var XAxis = new OxyPlot.Axes.TimeSpanAxis {
				Position = AxisPosition.Bottom,
				AbsoluteMinimum = -0.1,
				LabelFormatter = x =>
				{
					if ((int)x == 0)
						return "Start";
					return TimeSpanAxis.ToTimeSpan (x).ToString ("c");
				},
				MajorGridlineThickness = 1,
				MajorGridlineStyle = OxyPlot.LineStyle.Solid,
				MinorGridlineColor = OxyPlot.OxyColors.LightGray,
				MinorGridlineStyle = OxyPlot.LineStyle.Dot,
				MinorGridlineThickness = .5
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

			sequenceSeries = new OxyPlot.Series.LineSeries () {
				Color = OxyPlot.OxyColor.FromUInt32 (ColorHelper.RGBAFromGdkColor (pinSequence.Pin.PlotColor))
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
//			((Box.BoxChild)(vboxOptions [plotView])).Expand = true;
//			((Box.BoxChild)(vboxOptions [plotView])).Fill = true;

			plotView.SetSizeRequest (nvSequenceOptions.WidthRequest, this.HeightRequest / 3);

			plotView.ShowAll ();
		}

		private void SetupNodeView ()
		{
			nvSequenceOptions.NodeStore = nvSequenceOptionsStore;
			nvSequenceOptions.AppendColumn (new TreeViewColumn ("Duration", new CellRendererText (), "text", 0));
			nvSequenceOptions.AppendColumn (new TreeViewColumn ("State", new CellRendererText (), "text", 1));

			nvSequenceOptions.Show ();
		}
	}
}

