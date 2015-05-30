using System;
using System.Collections.Generic;

using System.Timers;
using Gtk;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.GtkSharp;
using OxyPlot.Series;

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

		plotTimer = new Timer (500);

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

			for (int i = 0; i < spinbuttonNumberOfSeries.Value; i++)
			{
				plotModel.Series.Add (new LineSeries (){ Title = i.ToString (), Smooth = true });
			}


			var xAxis =	new LinearAxis {
				Position = AxisPosition.Bottom,
				Minimum = -10,
				Maximum = 0,
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

			plotTimer.Elapsed += (object senderer, ElapsedEventArgs es) =>
			{
				Random rand = new Random ();
				foreach (Series s in plotView.Model.Series)
				{
					(s as LineSeries).Points.Add (new DataPoint (iterator, rand.NextDouble () * 10));
				}
				iterator++;
				double panStep = xAxis.Transform (-1 + xAxis.Offset);
				xAxis.Pan (panStep);
				plotModel.InvalidatePlot (true);
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

				//damit alle x achsen gleich sind
//				xAxis.AxisChanged += (object senderer, AxisChangedEventArgs ee) =>
//				{
//					foreach (PlotView pv in multiPlotViews)
//					{
//						if (!pv.Model.Axes [0].Equals ((senderer as LinearAxis)))
//						{
//							pv.Model.Axes [0].Pan ((senderer as LinearAxis).Transform ((senderer as LinearAxis).Offset));
//						}
//					}
//				};

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
