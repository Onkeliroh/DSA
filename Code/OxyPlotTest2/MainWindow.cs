using System;
using Gtk;

using OxyPlot;
using OxyPlot.GtkSharp;
using System.ComponentModel;
using OxyPlot.Series;
using GLib;

public partial class MainWindow: Gtk.Window
{
	private OxyPlot.GtkSharp.PlotView plotView;


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

	private void InitComponents()
	{
		var plotModel = new PlotModel {Title="TestPlot",Subtitle="",PlotType=PlotType.Cartesian,Background=OxyColors.White};

		plotModel.Series.Add (new FunctionSeries (Funktion, 0, 1, .1, "a+b"));

		plotView = new PlotView { Model = plotModel };
		plotView.SetSizeRequest (400, 400);
		plotView.Visible = true;

		vboxMain.PackStart (plotView);

		this.SetSizeRequest (600, 600);
		this.ShowAll ();
	}

	public System.Func<double,double> Funktion = (a) => a + 1;
}
