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

public partial class MainWindow: Gtk.Window
{
	private OxyPlot.GtkSharp.PlotView plotView;

	private Timer plotTimer;

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
//		plotModel.Series.Add (new FunctionSeries (Funktion, 0, 1, .1, "a+b"));

		plotView = new PlotView { Model = plotModel };
		plotView.SetSizeRequest (400, 400);
		plotView.Visible = true;
		plotView.InvalidatePlot (true);

		vboxMain.PackStart (plotView);
		((Box.BoxChild)(vboxMain [plotView])).Position = 0;

		this.SetSizeRequest (600, 600);

		//End Plot stuff

		plotTimer = new Timer (500);

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


//			var xAxis =	new LinearAxis {
//				Position = AxisPosition.Bottom,
//				Minimum = 0,
//				Maximum = 100,
//				MinimumPadding = 0,
//				MaximumPadding = 0,
//				MajorGridlineColor = OxyPlot.OxyColors.Gray,
//				MajorGridlineThickness = .5,
//				MajorGridlineStyle = LineStyle.Solid
//			};

			var dt = new DateTime (2015, 1, 1);
			var xAxis = new DateTimeAxis {
				Position = AxisPosition.Bottom,
				Minimum = DateTimeAxis.ToDouble(dt),
				Maximum = DateTimeAxis.ToDouble(dt.AddMinutes(1)),
				IntervalType = DateTimeIntervalType.Seconds,
				MajorGridlineStyle = LineStyle.Solid,
//				Angle = 90,
				StringFormat = "dd:HH:mm:ss",
//				MajorStep = 1.0 / 24 / 2, // 1/24 = 1 hour, 1/24/2 = 30 minutes
				IsZoomEnabled = true,
				MaximumPadding = 0,
				MinimumPadding = 0,
				TickStyle = TickStyle.None
			};

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
				MajorGridlineStyle = LineStyle.Solid,
			};
			plotModel.Axes.Add (yAxis);

			plotView.Model = plotModel;

			int iterator = 0;

			plotTimer.Elapsed += (object senderer, ElapsedEventArgs es) =>
			{
				Random rand = new Random();
				foreach (Series s in plotView.Model.Series)
				{
//					(s as LineSeries).Points.Add (new DataPoint (iterator, rand.NextDouble () * 10 ));
					(s as LineSeries).Points.Add (DateTimeAxis.CreateDataPoint( dt.AddSeconds(1), rand.NextDouble() * 10 ));
					dt = dt.AddSeconds(1).AddMilliseconds(Math.Floor(rand.NextDouble()*100));
				}
				iterator++;
				double panStep = xAxis.Transform(-1 + xAxis.Offset);
//				xAxis.Pan(panStep);
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
}
