using System;
using Gtk;

public partial class MainWindow: Gtk.Window
{
	public MainWindow () : base (Gtk.WindowType.Toplevel)
	{
		Build ();
		InitVis ();
	}

	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}

	public void InitVis ()
	{
		var model = new OxyPlot.PlotModel () {
			Title = "SideView",
			PlotType = OxyPlot.PlotType.XY,
			Background = OxyPlot.OxyColors.White
		};

		var view = new OxyPlot.GtkSharp.PlotView (){ Model = model };

		model.Axes.Add (new OxyPlot.Axes.DateTimeAxis (OxyPlot.Axes.AxisPosition.Bottom, DateTime.Now, DateTime.Now.AddMinutes (3), "Time", "mm:ss"));
		model.Axes.Add (new OxyPlot.Axes.LinearAxis () {
			Position = OxyPlot.Axes.AxisPosition.Left,
			Minimum = -0.5,
			Maximum = 1.5,
			Title = "State",
			MaximumPadding = 5,
			MinimumPadding = 5,
			MinorStep = 1,
			MajorStep = 1,
			IsPanEnabled = false,
			IsZoomEnabled = false
		});

		model.Series.Add (new OxyPlot.Series.RectangleBarSeries ());

		vboxView1.Add (view);
		(vboxView1 [view] as VBox.BoxChild).Expand = true;
		(vboxView1 [view] as VBox.BoxChild).Position = 0;

		ShowAll ();
	}

	protected void OnButton1Clicked (object sender, EventArgs e)
	{
		var model = (vboxView1.Children [0] as OxyPlot.GtkSharp.PlotView).Model;

		(model.Series [0] as OxyPlot.Series.RectangleBarSeries).Items.Add (
			new OxyPlot.Series.RectangleBarItem (
				DateTime.Now.AddSeconds (30).ToOADate (),
				1,
				DateTime.Now.AddMinutes (2).ToOADate (), 
				1
			)
		);

		ShowAll ();
	}
}
