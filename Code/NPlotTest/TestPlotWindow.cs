﻿#define Window
using System;
using NPlot;
using NPlot.Gtk;
using Gtk;


namespace NplotTest
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Application.Init ();

			#if Window
			TestPlotWindow w = new TestPlotWindow ("NPlotTest");

			BuildPlot(w.plot);

			w.plot.Show();

			#else
			Window w = new Window ("NPlot Test");
			w.DeleteEvent += delegate {
			Application.Quit();
			};

			NPlot.Gtk.PlotSurface2D plot = new NPlot.Gtk.PlotSurface2D ();

			BuildPlot (plot);

			plot.Show ();

			w.Add (plot);
			#endif

			w.ShowAll ();

			Application.Run ();
		}

		private static void BuildPlot(IPlotSurface2D plot)
		{
			//leeren
			plot.Clear ();

			plot.Title = "Test Plot";

			//plot werte
			double[] values = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 };

			LinePlot lp = new LinePlot ();

			lp.DataSource = values;
			lp.Label = "LInePlot Label";

			plot.Add (lp);


			plot.Legend = new Legend ();
			plot.Legend.NeverShiftAxes = true;
			plot.Legend.HorizontalEdgePlacement = Legend.Placement.Outside;
			plot.Legend.VerticalEdgePlacement = Legend.Placement.Outside;
			plot.Legend.XOffset = 10;
			plot.Legend.YOffset = 15;
		}
	}

	public class TestPlotWindow : Gtk.Window
	{
		private Button QuitBtn;
		public NPlot.Gtk.PlotSurface2D plot;

		public TestPlotWindow(string Title) : base(Title)
		{
			InitializeComponents ();
		}

		void InitializeComponents ()
		{
			QuitBtn = new Button ();
			QuitBtn.Label = "Quit";
			QuitBtn.Clicked += new System.EventHandler (delegate {
				Application.Quit ();
			});

			plot = new NPlot.Gtk.PlotSurface2D ();

			Gtk.VBox layout = new VBox ();

			Add (layout);

			layout.Add (plot);
			layout.Add (QuitBtn);

			SetSizeRequest(400,400);

			this.DeleteEvent += new global::Gtk.DeleteEventHandler (delegate (object obj, DeleteEventArgs e) {
				Application.Quit ();
			});
		}
	}
}
