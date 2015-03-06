using System;
using System.Collections;
using NPlot;
using NPlot.Gtk;
using Gtk;
using System.Collections.Generic;

namespace NplotTest
{
	public class NPlotConfigTestWindow : Gtk.Window
	{
		#region Components

		Button QuitBtn;
		Button NewPlotBtn;
		Button TimePlotBtn;
		ComboBox Plots;
		NPlot.Gtk.PlotSurface2D Plot;

		VBox MainLayout;
		HBox ButtonBar;
		Table ConfigTable;

		#endregion

		#region Members

		System.Timers.Timer TimePlotTimer;

		List<double> values;
		#endregion

		public NPlotConfigTestWindow (string Title) : base (Title)
		{
			InitializeComponents ();

			TimePlotTimer = new System.Timers.Timer (500);
			TimePlotTimer.Elapsed += new System.Timers.ElapsedEventHandler (OnTimerTick);

			values = new List<double> ();
		}

		private void OnDelete (object obj, EventArgs e)
		{
			Application.Quit ();
		}

		void InitializeComponents ()
		{
			QuitBtn = new Button ();
			QuitBtn.Label = "Quit";
			QuitBtn.Clicked += new System.EventHandler (OnDelete);

			NewPlotBtn = new Button ();
			NewPlotBtn.Label = "New Plot";
			NewPlotBtn.Clicked += new EventHandler (CreatePlot);

			TimePlotBtn = new Button ("TimePlot");
			TimePlotBtn.Clicked += new EventHandler (TimePlot);

			MainLayout = new VBox ();

			//ebene 1
			Plot = new NPlot.Gtk.PlotSurface2D ();

			//ebene 2
			Plots = new ComboBox (new String[] {
				"LinePlot",
				"PointPlot",
				"StepPlot",
				"CandlePlot",
				"BarPlot",
				"ImagePlot"
			});

			//ebene 3
			ButtonBar = new HBox (true, 3);

			ButtonBar.Add (NewPlotBtn);
			ButtonBar.Add (TimePlotBtn);
			ButtonBar.Add (QuitBtn);

			ConfigTable = new Table (3, 2, true);
			ConfigTable.Attach (new Label ("Plots"), 0, 1, 0, 1);
			ConfigTable.Attach (Plots, 1, 2, 0, 1);
			ConfigTable.Attach (new Label ("Color"), 0, 1, 1, 2);
			MainLayout.Add (Plot);
			MainLayout.Add (ConfigTable);
			MainLayout.Add (ButtonBar);

			this.Add (MainLayout);

			//this.Si

			this.DeleteEvent += new global::Gtk.DeleteEventHandler (OnDelete);
		}

		private void CreatePlot (object obj, EventArgs e)
		{
			this.Plot.Clear ();
			this.Plot.Title = "Test Plot";


			//gen values
			Random rgen = new Random ();

			double[] values = new double[rgen.Next (10, 1000)];

			for (int i = 0; i < values.Length; i++) {
				values [i] = rgen.Next (0, 50);
			}
			//end gen values

			StepPlot plottype = new StepPlot ();

			plottype.DataSource = values;

			Plot.Add (plottype);

			Plot.Legend = new Legend ();

			Plot.ShowAll ();
			Plot.Refresh ();
		}

		private void TimePlot (object obj, EventArgs e)
		{
			if (TimePlotTimer.Enabled) {
				TimePlotTimer.Stop ();
				TimePlotTimer.Enabled = false;
			} else {
				TimePlotTimer.Enabled = true;
				TimePlotTimer.Start ();
			}
		}

		private void OnTimerTick (object obj, System.Timers.ElapsedEventArgs e){
			GenValues ();
			ShowPlot ();
		}

		private void GenValues()
		{
			Random rgen = new Random ();
			values.Add (rgen.NextDouble());
		}

		private void ShowPlot ()
		{
			this.Plot.Clear ();
			this.Plot.Title = "Test Plot";

			AddPlotData ();

			Plot.Legend = new Legend ();

			Plot.ShowAll ();
			Plot.Refresh ();
		}

		private void AddPlotData()
		{
			dynamic surface;
						switch (Plots.Active) {
			case 0:
				surface = new LinePlot ();
				break;
			case 1:
				surface = new PointPlot ();
				break;
			case 2:
				surface = new StepPlot ();
				break;
			case 4:
				surface = new CandlePlot ();
				break;
			case 5:
				//surface = new ImagePlot ();
				//break;
			default:
				surface = new PointPlot ();
				break;
			}
			surface.DataSource = values.GetRange (0, values.Count - 1);
			Plot.Add (surface);
		}
	}
}
