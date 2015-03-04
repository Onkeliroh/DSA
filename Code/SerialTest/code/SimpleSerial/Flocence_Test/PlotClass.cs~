using System;
using Florence;
using Florence.GtkSharp;
using Gtk;

namespace FlorenceTest
{
	public class PlotClass : Gtk.Window
	{
		private PlotWidget plotwidget;
		private InteractivePlotSurface2D surface;
		private IDemo currentSample;
		private Button quitBtn;


		public PlotClass () : base("Flocence Test")
		{
			InitializeComponents ();

			surface = new InteractivePlotSurface2D ();
			plotwidget.InteractivePlotSurface2D = surface;
		}

		void InitializeComponents ()
		{
			plotwidget = new PlotWidget ();
			quitBtn = new Button ();
			quitBtn.Label = "Quit";
			quitBtn.Clicked += new System.EventHandler (quitBtn_click);

			SetSizeRequest (400, 400);

			Gtk.VBox layout = new VBox (false,1);
			Add (layout);
			layout.Add (plotwidget);
			layout.Add (quitBtn);

			//wichtig um das fenster via X zu schliessen und die instanz zu beenden
			this.DeleteEvent += new global::Gtk.DeleteEventHandler (this.OnDeleteEvent);
		}

		private void OnDeleteEvent(object obj, DeleteEventArgs e)
		{
			Application.Quit ();
		}

		void quitBtn_click (object sender, EventArgs e)
		{
			Application.Quit ();
		}


		private void ShowMeSomething()
		{

			Type currentType = typeof(PlotDataSet);
			currentSample = Activator.CreateInstance (currentType);
			currentSample.CreatePlot (surface);

			ShowAll ();
		}
	}
}

