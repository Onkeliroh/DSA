using System;
using Gtk;

using OxyPlot;
using OxyPlot.GtkSharp;
using System.ComponentModel;
using OxyPlot.Series;
using GLib;
using System.Timers;
using OxyPlot.Axes;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Runtime.CompilerServices;
using Gdk;

public partial class MainWindow: Gtk.Window
{
	private OxyPlot.GtkSharp.PlotView plotView;
	private PlotView multiPlotView ;

	private Timer plotTimer;
	private Timer multiPlotTimer = new Timer();

	public LineSeries timeSeries;


	public MainWindow () : base (Gtk.WindowType.Toplevel)
	{
		Build ();

		InitComponents ();
	}

	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}

	protected void OnBtnExitClicked (object sender, EventArgs e)
	{
		Application.Quit ();
	}

	private void InitComponents ()
	{
		//Plot stuff

		plotView = new PlotView();
		plotView.SetSizeRequest (400, 400);
		plotView.Visible = true;
		plotView.InvalidatePlot (true);

		vboxMain.PackStart (plotView);
		((Box.BoxChild)(vboxMain [plotView])).Position = 0;
		((Box.BoxChild)(vboxMain [plotView])).Expand = true;

		this.SetSizeRequest (600, 600);

		//End Plot stuff

		plotTimer = new Timer (500);

		this.ShowAll ();
	}

	private void DrawSinglePlot()
	{
		var plotModel = new PlotModel {
			Title = "TestPlot",
			Subtitle = "",
			PlotType = PlotType.Cartesian,
			Background = OxyColors.White
		};
		plotModel.InvalidatePlot (true);

		var tmpSeries = new LineSeries ();
		var rand = new Random ();
		for (int i = 0; i < 100; i++)
		{
			tmpSeries.Points.Add (new DataPoint (i, rand.NextDouble ()));
		}

		plotModel.Series.Add (tmpSeries);

		plotView.Model = plotModel;

		this.ShowAll ();
	}

	public System.Func<double,double> Funktion = (a) => a + 1;

	protected void OnBtnCenterPlotClicked (object sender, EventArgs e)
	{
		plotView.Model.ResetAllAxes ();

	}

	protected void OnBtnTimedPlotClicked (object sender, EventArgs e)
	{
		//stop timer
		if (plotTimer.Enabled)
		{
			plotTimer.Stop ();
		} 
		//start timer
		else if (!plotTimer.Enabled)
		{
			var plotModel = new PlotModel {
				Title = "timed Plot",
				PlotType = PlotType.Cartesian,
				Background = OxyPlot.OxyColors.White,
			};

			for (int i = 0; i < spinbuttonNumberOfSeries.Value; i++) {
				plotModel.Series.Add (new LineSeries(){Title = i.ToString(), Smooth = true});
			}


			var xAxis =	new LinearAxis {
				Position = AxisPosition.Bottom,
				Minimum = 0,
				Maximum = 100,
				MinimumPadding = 0,
				MaximumPadding = 0,
				MajorGridlineColor = OxyPlot.OxyColors.Gray,
				MajorGridlineThickness = .5,
				MajorGridlineStyle = OxyPlot.LineStyle.Solid
			};


//			var xAxis = new DateTimeAxis {
//				Position = AxisPosition.Bottom,
//				Minimum = DateTimeAxis.ToDouble(DateTime.Now),
//				Maximum = DateTimeAxis.ToDouble(DateTime.Now.AddSeconds(10)),
//				MajorStep = DateTimeAxis.ToDouble( new TimeSpan(0,0,1) ),
//				StringFormat = "mm:ss"
//			};

			plotModel.Axes.Add ( xAxis );

			var yAxis = new LinearAxis {
				Position = AxisPosition.Left,
				Minimum = 10,
				Maximum = 1,
				AbsoluteMaximum = 10.1,
				AbsoluteMinimum = -0.1,
				MaximumPadding = 5,
				MinimumPadding = 5,
				IsPanEnabled = false,
				IsZoomEnabled = false,
				MajorGridlineColor = OxyPlot.OxyColors.Gray,
				MajorGridlineThickness = .5,
				MajorGridlineStyle = OxyPlot.LineStyle.Solid,
			};
			plotModel.Axes.Add (yAxis);

			plotView.Model = plotModel;

			int iterator = 0;

			plotTimer.Elapsed += (object senderer, ElapsedEventArgs es) =>
			{
				Random rand = new Random();
				foreach (Series s in plotView.Model.Series)
				{
					(s as LineSeries).Points.Add (new DataPoint (iterator, rand.NextDouble () * 10 ));
				}
				iterator++;
				double panStep = xAxis.Transform(-1 + xAxis.Offset);
				xAxis.Pan(panStep);
				plotModel.InvalidatePlot (true);
			};

			plotTimer.Start ();
		}
	}

	protected void OnSpinbuttonNumberOfSeriesChangeValue (object o, ChangeValueArgs args)
	{
		if (plotView.Model.Series.Count > (o as SpinButton).Value) {
			while (plotView.Model.Series.Count > (o as SpinButton).Value) {
				plotView.Model.Series.RemoveAt (plotView.Model.Series.Count - 1);
			}
		} else if (plotView.Model.Series.Count < (o as SpinButton).Value) {
			while (plotView.Model.Series.Count > (o as SpinButton).Value) {
				plotView.Model.Series.Add(new LineSeries());
			}
		}

	}

	protected void OnCheckbuttonSmoothPlotToggled (object sender, EventArgs e)
	{
		foreach (Series s in plotView.Model.Series) {
			(s as LineSeries).Smooth = (sender as CheckButton).Active;
		}
	}

	protected void OnCheckbuttonMarkerToggleToggled (object sender, EventArgs e)
	{
		if ((sender as CheckButton).Active) {
			foreach (Series s in plotView.Model.Series) {
				(s as LineSeries).MarkerType = MarkerType.Cross;
				(s as LineSeries).MarkerStroke = OxyColors.Red;
			}
		} else {
			foreach (Series s in plotView.Model.Series) {
				(s as LineSeries).MarkerType = MarkerType.None;
			}

		}
	}

	protected void OnBtnSinglePlotClicked (object sender, EventArgs e)
	{
		DrawSinglePlot ();
	}

	protected void OnBtnStartStopMultiplotClicked (object sender, EventArgs e)
	{
		if (multiPlotTimer.Enabled) {

			multiPlotTimer.Stop ();

			multiPlotView.Model = null;
			multiPlotView.Unrealize ();

			vboxMultiPlot.Remove (multiPlotView);
		} else {
			//BEGIN Setup
			multiPlotView = new PlotView();
			multiPlotView.InvalidatePlot (true);

			vboxMultiPlot.PackStart (multiPlotView, true, true, 0);
			(vboxMultiPlot [multiPlotView] as VBox.BoxChild).Position = 0;


			List<LineSeries> Serieses = new List<LineSeries> ();

			for (int i = 0; i < spinbuttonNumberOfMultiplots.Value; i++) {
				Serieses.Add(new LineSeries {
					Title = "A"+i.ToString()
				});
			}

			var multiPlotTotalModel = new PlotModel {
				Title = "multiplot Total View",
				PlotType = PlotType.Cartesian,
				LegendPlacement = LegendPlacement.Outside,
				LegendPosition = LegendPosition.RightMiddle};

			var xAxis = new LinearAxis {
				Position = AxisPosition.Bottom,
				Minimum = -10,
				Maximum = 0,
				MajorGridlineStyle = OxyPlot.LineStyle.Solid,
				MajorGridlineColor = OxyPlot.OxyColors.Gray,
				MajorGridlineThickness = .5,
	};

			var yAxis = new LinearAxis {
				Position = AxisPosition.Left,
				Maximum = 10,
				Minimum = 0,
				AbsoluteMaximum = 10,
				AbsoluteMinimum = 0,
				IsZoomEnabled = false,
				IsPanEnabled = false,
				MajorGridlineStyle = OxyPlot.LineStyle.Solid,
				MajorGridlineColor = OxyPlot.OxyColors.Gray,
				MajorGridlineThickness = .5,
				Key = "Total"
			};

			multiPlotTotalModel.Axes.Add (xAxis);
			multiPlotTotalModel.Axes.Add (yAxis);

			foreach (LineSeries ls in Serieses) {
				multiPlotTotalModel.Series.Add (ls);
			}

			if (checkbuttonDetailedPlots.Active) {
				for (int i = 0; i < spinbuttonNumberOfMultiplots.Value; i++) {

					var Model = new PlotModel {
					};
					Model.Series.Add (Serieses[0]);

					var View = new PlotView ();
					View.Model = Model;

					vboxMultiPlot.PackStart (View);
					(vboxMultiPlot [View] as VBox.BoxChild).Position = i + 1;
				}
			}

			multiPlotView.Model = multiPlotTotalModel;
			//END Setup

			//BEGIN TIMER Stuff
			multiPlotTimer = new Timer(500);
			int iterator = 0;

			multiPlotTimer.Elapsed += (senderer, ee) => {
				var rand = new Random();
				foreach (LineSeries ls in multiPlotView.Model.Series)
				{
					ls.Points.Add(
						new DataPoint(iterator, rand.NextDouble() * 10)
					);
				}
				iterator++;
				multiPlotView.Model.PanAllAxes(xAxis.Transform(-1 + xAxis.Offset),0);
				multiPlotView.Model.ZoomAllAxes(1);
				multiPlotTotalModel.InvalidatePlot (true);
			};

			multiPlotTimer.Start ();

			this.ShowAll ();
			//END TIMER Stuff
		}
	}
}
