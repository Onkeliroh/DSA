using System;
using Florence;
using Florence.GtkSharp;
using Gtk;

namespace FlocenceTest
{
	public class PlotClass : Gtk.Window
	{
		private PlotWidget plotwidget;
		private Button quitBtn;
		public PlotClass () : base("Flocence Test")
		{
			InitializeComponents ();

			InteractivePlotSurface2D surface = new InteractivePlotSurface2D ();
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

		}

		void quitBtn_click (object sender, EventArgs e)
		{
			Application.Quit ();
		}
	}
}

