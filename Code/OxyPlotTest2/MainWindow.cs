﻿using System;
using System.Collections.Generic;

using System.Timers;
using Gtk;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.GtkSharp;
using OxyPlot.Series;
using System.IO;

public partial class MainWindow: Gtk.Window
{
	private OxyPlot.GtkSharp.PlotView plotView;
	private List<PlotView> multiPlotViews = new List<PlotView> ();

	private Random rand = new Random ();

	private Timer plotTimer;
	private Timer multiPlotTimer = new Timer ();

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

	protected void OnKeyPressEvent(object sender, KeyPressEventArgs e)
	{
		Console.WriteLine (e.Event.Key + "\t" + e.Event.State);
		if (e.Event.Key.Equals (Gdk.Key.c) && e.Event.State.Equals(Gdk.ModifierType.ControlMask|Gdk.ModifierType.Mod2Mask)){
			Console.WriteLine ("Saving plot");
			var stream = File.Create ("plot.png");
			var pngExporter = new PngExporter ();
			pngExporter.Export (plotView.Model, stream);
			stream.Close ();
		}
	}

	private void InitComponents ()
	{
		//Plot stuff

		plotView = new PlotView ();
		plotView.SetSizeRequest (400, 400);
		plotView.Visible = true;
		plotView.InvalidatePlot (true);

		vboxMain.PackStart (plotView);
		((Box.BoxChild)(vboxMain [plotView])).Position = 0;
		((Box.BoxChild)(vboxMain [plotView])).Expand = true;

		this.SetSizeRequest (600, 600);

		//End Plot stuff

		plotTimer = new Timer (1000);

		this.ShowAll ();
	}

	private void DrawSinglePlot ()
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

	private void DrawSingleTimePlot ()
	{
		var model = new PlotModel {
			Title = "Single Time Plot",
			PlotType = PlotType.XY,
			Background = OxyColors.White
		};
		model.InvalidatePlot (true);

		model.Axes.Add (
			new DateTimeAxis {
				IntervalType = DateTimeIntervalType.Seconds,
				MajorGridlineStyle = LineStyle.Solid,
				Position = AxisPosition.Bottom
			}
		);

		var dt = DateTime.Now;

		var series = new LineSeries ();
		var rand = new Random ();
		for (int i = 0; i < 100; i++)
		{
			series.Points.Add (DateTimeAxis.CreateDataPoint (dt.AddSeconds (i), rand.NextDouble () * 10));
		}

		model.Series.Add (series);

		plotView.Model = model;

		this.ShowAll ();
	}

	private void DrawNaNTestPlot ()
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

		for (int i = 1; i < (int)(rand.NextDouble () * 10); i++)
		{
			int pos = rand.Next () % 100;
			tmpSeries.Points [pos] = new DataPoint (pos, double.NaN);
		}

		plotModel.Series.Add (tmpSeries);

		plotView.Model = plotModel;

		this.ShowAll ();
	}

	private void DrawStairStepSinglePlot ()
	{
		var plotModel = new PlotModel {
			Title = "TestPlot",
			Subtitle = "",
			PlotType = PlotType.Cartesian,
			Background = OxyColors.White
		};
		plotModel.InvalidatePlot (true);

		var tmpSeries = new StairStepSeries ();
		var rand = new Random ();

		for (int i = 0; i < 100; i++) {
			tmpSeries.Points.Add(new DataPoint(i,rand.Next()%2));
		}
		plotModel.Series.Add( tmpSeries );
		plotView.Model = plotModel;

		this.ShowAll ();
	}

	private void DrawStairStepSinglePlotCustomAxes ()
	{
		var plotModel = new PlotModel {
			Title = "TestPlot",
			Subtitle = "",
			PlotType = PlotType.Cartesian,
			Background = OxyColors.White
		};
		plotModel.InvalidatePlot (true);

		plotModel.Axes.Add (
			new LinearAxis () {
				Position = AxisPosition.Left,
				Minimum = 0,
				Maximum = 1,
				LabelFormatter = x => ((int)x == 0)?"LOW":"HIGH",
				IsPanEnabled = false,
				IsZoomEnabled = false,
				AbsoluteMaximum = 1.1,
				AbsoluteMinimum = -0.1,
				MinorStep = 1,
				MajorStep = 1
			});

		var tmpSeries = new StairStepSeries ();
		var rand = new Random ();

		for (int i = 0; i < 100; i++) {
			tmpSeries.Points.Add(new DataPoint(i,rand.Next()%2));
		}
		plotModel.Series.Add( tmpSeries );
		plotView.Model = plotModel;

		this.ShowAll ();
	}

	private void DrawStairStepDualPlotCustomAxes()
	{
		var plotModel = new PlotModel {
			Title = "TestPlot",
			Subtitle = "",
			PlotType = PlotType.XY,
			Background = OxyColors.White
		};
		plotModel.InvalidatePlot (true);

		#region yAxes
		plotModel.Axes.Add (
			new LinearAxis () {
				Key = "Stair1",
				Title = "Stair1",
				StartPosition =0.1,
				EndPosition = 0.45,
				Position = AxisPosition.Left,
				Minimum = 0,
				Maximum = 1,
				LabelFormatter = x => ((int)x == 0)?"LOW":"HIGH",
				IsPanEnabled = false,
				IsZoomEnabled = false,
				AbsoluteMaximum = 1.1,
				AbsoluteMinimum = -0.1,
				MinorStep = 1,
				MajorStep = 1
			});
		plotModel.Axes.Add (
			new LinearAxis () {
				Key = "Stair2",
				Title = "Stair2",
				StartPosition = 0.5,
				EndPosition = 0.95,
				Position = AxisPosition.Left,
				Minimum = 0,
				Maximum = 1,
				LabelFormatter = x => ((int)x == 0)?"LOW":"HIGH",
				IsPanEnabled = false,
				IsZoomEnabled = false,
				AbsoluteMaximum = 1.1,
				AbsoluteMinimum = -0.1,
				MinorStep = 1,
				MajorStep = 1
			});
		#endregion

		plotModel.Axes.Add (new DateTimeAxis {
				IntervalType = DateTimeIntervalType.Seconds,
				MajorGridlineStyle = LineStyle.Solid,
				Position = AxisPosition.Bottom	
			}
		);

		var dt = DateTime.Now;
		var tmpSeries = new StairStepSeries (){YAxisKey = "Stair1"};
		var rand = new Random ();

		for (int i = 0; i < 100; i++) {
			tmpSeries.Points.Add(DateTimeAxis.CreateDataPoint(dt.AddSeconds(i),rand.Next()%2));
		}
		plotModel.Series.Add( tmpSeries );

		tmpSeries = new StairStepSeries (){YAxisKey = "Stair2"};

		for (int i = 0; i < 100; i++) {
			tmpSeries.Points.Add(DateTimeAxis.CreateDataPoint(dt.AddSeconds(i),rand.Next()%2));
		}
		plotModel.Series.Add( tmpSeries );
		plotView.Model = plotModel;

		this.ShowAll ();
	}

	protected void OnBtnCenterPlotClicked (object sender, EventArgs e)
	{
		plotView.Model.ResetAllAxes ();
		plotView.InvalidatePlot (true);
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
				PlotType = PlotType.XY,
				Background = OxyPlot.OxyColors.White,
			};

			var plotSeries = new LineSeries ();
			plotModel.Series.Add (plotSeries);

			var xAxis = new DateTimeAxis {
				Position = AxisPosition.Bottom,
//				Minimum = DateTimeAxis.ToDouble (DateTime.Now),
//				Maximum = DateTimeAxis.ToDouble (DateTime.Now.AddSeconds (10)),
				IntervalType = DateTimeIntervalType.Seconds,
			};

			plotModel.Axes.Add (xAxis);

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
			var dt = DateTime.Now;

			Random rand = new Random ();
			plotTimer.Elapsed += (object senderer, ElapsedEventArgs es) =>
			{
				plotSeries.Points.Add (
					DateTimeAxis.CreateDataPoint (dt.AddSeconds (iterator), rand.NextDouble () * 100)
				);
				iterator++;
//				if (iterator > 10)
//				{
//					double panStep = xAxis.Transform (-1 + xAxis.Offset);
//					xAxis.Pan (panStep);
//				}
				Console.WriteLine (xAxis.Offset + "\t" + dt.AddSeconds (iterator).ToOADate () + "\t" + xAxis.Position + "\t" + xAxis.InverseTransform (xAxis.ScreenMin.X));
				plotModel.InvalidatePlot (true);
				plotView.InvalidatePlot (true);
			};

			plotTimer.Start ();
		}
	}

	protected void OnSpinbuttonNumberOfSeriesChangeValue (object o, ChangeValueArgs args)
	{
		if (plotView.Model.Series.Count > (o as SpinButton).Value)
		{
			while (plotView.Model.Series.Count > (o as SpinButton).Value)
			{
				plotView.Model.Series.RemoveAt (plotView.Model.Series.Count - 1);
			}
		} else if (plotView.Model.Series.Count < (o as SpinButton).Value)
		{
			while (plotView.Model.Series.Count > (o as SpinButton).Value)
			{
				plotView.Model.Series.Add (new LineSeries ());
			}
		}

	}

	protected void OnCheckbuttonSmoothPlotToggled (object sender, EventArgs e)
	{
		foreach (Series s in plotView.Model.Series)
		{
			(s as LineSeries).Smooth = (sender as CheckButton).Active;
		}
	}

	protected void OnCheckbuttonMarkerToggleToggled (object sender, EventArgs e)
	{
		if ((sender as CheckButton).Active)
		{
			foreach (Series s in plotView.Model.Series)
			{
				(s as LineSeries).MarkerType = MarkerType.Cross;
				(s as LineSeries).MarkerStroke = OxyColors.Red;
			}
		} else
		{
			foreach (Series s in plotView.Model.Series)
			{
				(s as LineSeries).MarkerType = MarkerType.None;
			}

		}
	}

	protected void OnBtnSinglePlotClicked (object sender, EventArgs e)
	{
		DrawSinglePlot ();
	}

	protected void OnBtnSingleTimePlotClicked (object sender, EventArgs e)
	{
		DrawSingleTimePlot ();		
	}

	protected void OnBtnNaNTestClicked (object sender, EventArgs e)
	{
		DrawNaNTestPlot ();
	}

	protected void OnBtnStairStepSinglePlotClicked (object sender, EventArgs e)
	{
		DrawStairStepSinglePlot ();
	}

	protected void OnBtnStairStepSingleCustomAxesClicked (object sender, EventArgs e)
	{
		DrawStairStepSinglePlotCustomAxes ();
	}

	protected void OnBtnStairStempDualPlotCustomAxesClicked (object sender, EventArgs e)
	{
		DrawStairStepDualPlotCustomAxes ();
	}

	protected void OnBtnStartStopMultiplotClicked (object sender, EventArgs e)
	{
		if (multiPlotTimer.Enabled)
		{

			multiPlotTimer.Stop ();

			foreach (PlotView pv in multiPlotViews)
			{
				pv.Model = null;
				pv.Unrealize ();

				vboxMultiPlot.Remove (pv);
			}
		} else
		{
			#region Setup
			multiPlotViews.Clear ();
			for (int plotnr = 0; plotnr <= spinbuttonNumberOfMultiplots.Value; plotnr++)
			{
				if (!checkbuttonDetailedPlots.Active && plotnr != 0)
				{
					break;
				}
				var multiPlotView = new PlotView ();
				multiPlotView.InvalidatePlot (true);

				vboxMultiPlot.PackStart (multiPlotView, true, true, 0);
				(vboxMultiPlot [multiPlotView] as VBox.BoxChild).Position = plotnr;

				var multiPlotTotalModel = new PlotModel {
					Title = "A" + (plotnr - 1).ToString (),
					PlotType = PlotType.Cartesian,
					LegendPlacement = LegendPlacement.Outside,
					LegendPosition = LegendPosition.RightMiddle
				};

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
				};

				multiPlotTotalModel.Axes.Add (xAxis);
				multiPlotTotalModel.Axes.Add (yAxis);

				if (plotnr != 0)
				{
					multiPlotTotalModel.Series.Add (new LineSeries { Title = "A" + (plotnr - 1).ToString () });
				} else
				{
					multiPlotTotalModel.Title = "Total";
					for (int i = 0; i < spinbuttonNumberOfMultiplots.Value; i++)
					{
						multiPlotTotalModel.Series.Add (new LineSeries { Title = "A" + i.ToString () });
					}
				}

				multiPlotTotalModel.InvalidatePlot (true);
				multiPlotView.Model = multiPlotTotalModel;
				multiPlotViews.Add (multiPlotView);
			}
			#endregion

			#region BEGIN TIMER Stuff
			multiPlotTimer = new Timer (500);
			int iterator = 0;

			multiPlotTimer.Elapsed += (senderer, ee) =>
			{
				try
				{
					for (int i = 1; i <= multiPlotViews [0].Model.Series.Count; i++)
					{
						double val = rand.NextDouble () * 10;
						(multiPlotViews [0].Model.Series [i - 1] as LineSeries).Points.Add (
							new DataPoint (iterator, val)
						);
						if (checkbuttonDetailedPlots.Active)
						{
							(multiPlotViews [i].Model.Series [0] as LineSeries).Points.Add (
								new DataPoint (iterator, val)
							);
//						(multiPlotViews [i].Model.Series [0] as LineSeries).Color = (multiPlotViews [0].Model.Series [i - 1] as LineSeries).Color;
							multiPlotViews [i].Model.Axes [0].Pan (multiPlotViews [i].Model.Axes [0].Transform (-1 + multiPlotViews [i].Model.Axes [0].Offset));
						}
					}
					multiPlotViews [0].Model.Axes [0].Pan (multiPlotViews [0].Model.Axes [0].Transform (-1 + multiPlotViews [0].Model.Axes [0].Offset));
					foreach (PlotView pv in multiPlotViews)
					{
						pv.Model.InvalidatePlot (true);
					}
					iterator++;
				} catch (Exception exp)
				{
					Console.Error.WriteLine (exp);
				}
			};

			multiPlotTimer.Start ();

			this.ShowAll ();
			#endregion
		}
	}
}
