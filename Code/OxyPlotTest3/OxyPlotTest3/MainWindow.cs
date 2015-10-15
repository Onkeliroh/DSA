using System;
using Gtk;
using OxyPlot;
using OxyPlot.GtkSharp;
using OxyPlot.Axes;
using OxyPlot.Series;
using System.Linq;
using System.Timers;

public partial class MainWindow: Gtk.Window
{
	private PlotView view;
	private PlotModel model;
	private LinearAxis YAxis;
	private DateTimeAxis XAxis;
	Timer timer = new Timer (1000);

	private double prevoffset;


	public MainWindow () : base (Gtk.WindowType.Toplevel)
	{
		Build ();
		Init ();

		this.Maximize ();

		ShowAll ();

		Log ("Done Loading");
	}

	private void Init ()
	{
		YAxis = new LinearAxis () {
			AbsoluteMaximum = 1.0,
			AbsoluteMinimum = 0.0,
			IsPanEnabled = false,
			IsZoomEnabled = false,
			Key = "YAxis",
			Position = OxyPlot.Axes.AxisPosition.Left
		};

		XAxis = new DateTimeAxis () {
			IntervalType = DateTimeIntervalType.Hours,
			IsZoomEnabled = true,
			Position = OxyPlot.Axes.AxisPosition.Bottom,
		};

		model = new PlotModel () {
			PlotType = PlotType.XY,
		};

		model.Axes.Add (XAxis);
		model.Axes.Add (YAxis);

		view = new PlotView () {
			Model = model,
			HeightRequest = 200,
			WidthRequest = 300
		};

		vboxMain.PackStart (view, true, true, 0);
		(vboxMain [view] as Box.BoxChild).Position = 0;

		view.QueueDraw ();
	}

	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}

	private void Log (string s)
	{
		tvLog.Buffer.InsertAtCursor ("\n" + s);
		Console.WriteLine (s);
		tvLog.QueueDraw ();
	}

	#region Operationen

	protected void OnButton1Clicked (object sender, EventArgs e)
	{
		var series = new OxyPlot.Series.LineSeries () {
			DataFieldX = "X",
			DataFieldY = "Y"
		};

		var now = DateTime.Now;
		var rng = new Random ();
		var time = now;
		double value = 0;
		for (int i = 0; i < 100; i++) {
			time = now.AddMinutes (i);
			value = rng.NextDouble ();
			series.Points.Add (new DataPoint (time.ToOADate (), value));
		}

		model.Series.Clear ();
		model.Series.Add (series);
		model.InvalidatePlot (true);
		view.QueueDraw ();
	}


	protected void OnButton2Clicked (object sender, EventArgs e)
	{
		if (timer.Enabled) {
			timer.Stop ();
			timer = new Timer (1000);
		} else {
			var rng = new Random ();

			var series = new OxyPlot.Series.LineSeries ();

			timer.Elapsed += (o, args) => {
				series.Points.Add (new DataPoint (DateTime.Now.ToOADate (), rng.NextDouble ()));	
				view.InvalidatePlot (true);
				view.QueueDraw ();

				if (series.Points.Count == 1) {
//					XAxis.Pan (series.Points.Last ().X);
				} else if (series.Points.Count > 2) {
//					XAxis.Pan (series.Points [series.Points.Count - 1].X - series.Points [series.Points.Count - 2].X);
					XAxis.Pan (new ScreenPoint (XAxis.Transform (series.Points.Last ().X), 0), new ScreenPoint (XAxis.Transform (series.Points [series.Points.Count - 2].X), 0));

					Log ((XAxis.Offset - prevoffset).ToString () +
					"\t" +
					(XAxis.InverseTransform (series.Points [series.Points.Count - 1].X) - XAxis.InverseTransform (series.Points [series.Points.Count - 2].X)).ToString () +
					"\t" +
					(series.Points [series.Points.Count - 1].X - series.Points [series.Points.Count - 2].X).ToString ()
					);
				}
//				Log (string.Format ("{0}\t{1}", series.Points.Last ().X, XAxis.Offset));
				prevoffset = XAxis.Offset;
			};

			model.Series.Clear ();
			model.Series.Add (series);

			timer.Start ();
		}
	}

	#endregion
}

public class DateTimeValue
{
	public double X;
	public double Y;

	public DateTimeValue (double time, double value)
	{
		X = time;
		Y = value;
	}
}
