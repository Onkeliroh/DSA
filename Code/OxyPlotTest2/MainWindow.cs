using System;
using Gtk;

using OxyPlot;
using OxyPlot.GtkSharp;
using System.ComponentModel;
using OxyPlot.Series;
using GLib;
using System.Timers;

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
			timeSeries = new LineSeries { Title = "timed Series" };

			var plotModel = new PlotModel {
				Title = "timed Plot",
				PlotType = PlotType.Cartesian,
				Background = OxyPlot.OxyColors.White
			};

			plotModel.Series.Add (timeSeries);
			plotModel.InvalidatePlot (true);

			plotView.Model = plotModel;

			plotTimer.Elapsed += (object senderer, ElapsedEventArgs es) =>
			{
				timeSeries.Points.Add (new DataPoint (DateTime.Now.Second, new Random ().NextDouble ()));
				plotModel.InvalidatePlot (true);
				Console.WriteLine (timeSeries.Points.Count);
			};

			plotTimer.Start ();
		}
	}
}
