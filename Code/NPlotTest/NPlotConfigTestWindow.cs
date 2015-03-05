using System;
using NPlot;
using NPlot.Gtk;
using Gtk;

namespace NplotTest
{
	public class NPlotConfigTestWindow : Gtk.Window
	{
		#region Components
		Button QuitBtn;
		Button RefreshBtn;
		ComboBox Plots;
		NPlot.Gtk.PlotSurface2D Plot;

		VBox MainLayout;
		HBox ButtonBar;
		Table ConfigTable;
		#endregion



		public NPlotConfigTestWindow(string Title) : base(Title)
		{
			InitializeComponents ();
		}

		private void OnDelete(object obj, EventArgs e)
		{
			Application.Quit ();
		}

		void InitializeComponents()
		{
			QuitBtn = new Button ();
			QuitBtn.Label = "Quit";
			QuitBtn.Clicked += new System.EventHandler (OnDelete);

			RefreshBtn = new Button ();
			RefreshBtn.Label = "Refresh";
			RefreshBtn.Clicked += new EventHandler (CreatePlot); //todo

			MainLayout = new VBox ();

			//ebene 1
			Plot = new NPlot.Gtk.PlotSurface2D ();

			//ebene 2
			Plots = new ComboBox(new String[]{
				"LinePlot",
				"PointPlot",
				"StepPlot",
				"CandlePlot",
				"BarPlot",
				"ImagePlot"
			});

			//ebene 3
			ButtonBar = new HBox (true, 3);

			ButtonBar.Add (RefreshBtn);
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

		private void CreatePlot( object obj, EventArgs e)
		{
			this.Plot.Clear ();
			this.Plot.Title = "Test Plot";


			//gen values
			Random rgen = new Random ();

			double[] values = new double[rgen.Next(10,1000)];

			for ( int i = 0; i < values.Length; i++)
			{
				values[i] = rgen.Next (0, 50);
			}
			//end gen values

			//LinePlot plottype = new LinePlot ();
			//PointPlot plottype = new PointPlot();
			StepPlot plottype = new StepPlot();

//			switch (Plots.Active)
//			{
//			case 0:
//				LinePlot plottype = new LinePlot ();
//				break;
//			case 1:
//				PointPlot plottype = new PointPlot ();
//				break;
//			case 2:
//				plottype = new StepPlot ();
//				break;
//			case 3:
//				plottype = new CandlePlot ();
//				break;
//			case 4:
//				//Plot.Add (new Bar ());
//				break;
//			case 5:
//				plottype = new ImagePlot ();
//				break;
//			default:
//				plottype = new LinePlot ();
//			}

			plottype.DataSource = values;

			Plot.Add (plottype);

			Plot.Legend = new Legend ();

			Plot.ShowAll ();
			Plot.Refresh ();
		}

	}
}
