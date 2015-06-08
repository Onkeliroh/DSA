using System;
using Gtk;
using Prototype;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.GtkSharp;
using OxyPlot.Series;
using System.ComponentModel;

public partial class PrototypeWindow: Gtk.Window
{
	#region Member

	#endregion


	#region Window Stuff

	public PrototypeWindow () : base (Gtk.WindowType.Toplevel)
	{
		Build ();
		InitComponents ();
	}

	private void InitComponents ()
	{
		foreach (string s in System.IO.Ports.SerialPort.GetPortNames()) {
		}


		(this.UIManager.GetWidget ("/menubar1") as MenuBar).Insert (new MenuItem ("Test"), 5);
		(this.UIManager.GetWidget ("/menubar1") as MenuBar).Show ();
	}

	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		MainClass.mainController.Stop ();
		MainClass.mainController.ArduinoController.Disconnect ();
		Application.Quit ();
	}

	protected void OnQuitActionActivated (object sender, EventArgs e)
	{
		OnDeleteEvent (sender, null);
	}

	protected void OnAboutActionActivated (object sender, EventArgs e)
	{
		var about = new AboutDialog () {
			AllowGrow = false,
			Authors = new string[]{ "Daniel Pollack" },
		};
		about.Run ();
		about.Destroy ();
	}

	protected void OnBtnNewConfigClicked (object sender, EventArgs e)
	{
		hboxGreetings.Visible = false;
		tableConnection.Visible = true;

		OnBtnRefreshClicked (this, null);
	}

	protected void OnBtnRefreshClicked (object sender, EventArgs e)
	{
		(cbConnectPorts.Model as ListStore).Clear ();
		foreach (string s in System.IO.Ports.SerialPort.GetPortNames()) {
			cbConnectPorts.AppendText (s);	
		}
		cbConnectPorts.Active = 0;
	}

	protected void OnBtnConnectClicked (object sender, EventArgs e)
	{
		MainClass.mainController.ArduinoController.SerialPortName = cbConnectPorts.ActiveText;
		MainClass.mainController.ArduinoController.Setup ();
		if (MainClass.mainController.ArduinoController.IsConnected) {
			CreateConfigInterface ();
			tableConfig.Visible = true;
			tableConnection.Visible = false;
			hboxGreetings.Visible = false;
			disconnectAction.Sensitive = true;
			vboxPlot.Visible = false;
		} else {
			//Todo What else?
		}
	}

	protected void OnBtnConfigRunClicked (object sender, EventArgs e)
	{
		tableConfig.Visible = false;
		tableConnection.Visible = false;
		hboxGreetings.Visible = false;
		vboxPlot.Visible = true;
		CleatePlotInterface ();
	}

	protected void OnDisconnectActionActivated (object sender, EventArgs e)
	{
		if (MainClass.mainController.ArduinoController.IsConnected) {
			MainClass.mainController.ArduinoController.Disconnect ();
		}
	}

	protected void OnBtnConfigBackClicked (object sender, EventArgs e)
	{
		tableConfig.Visible = false;
		tableConnection.Visible = true;
		hboxGreetings.Visible = false;
		vboxPlot.Visible = false;
		disconnectAction.Sensitive = false;
	}

	protected void OnBtnPlotBackClicked (object sender, EventArgs e)
	{
		tableConfig.Visible = true;
		vboxPlot.Visible = false;
		tableConnection.Visible = false;
		hboxGreetings.Visible = false;
		disconnectAction.Sensitive = false;
	}

	#endregion

	#region Logic

	private void CreateConfigInterface ()
	{
		Gtk.NodeStore ScheduleNodeStore = new NodeStore (typeof(ScheduleNode));
		ScheduleNodeStore.AddNode (new ScheduleNode ("Test1"));
		ScheduleNodeStore.AddNode (new ScheduleNode ("Test2"));

		Gtk.NodeView ScheduleNodeview = new NodeView (ScheduleNodeStore);

		Schedulehbox.Add (ScheduleNodeview);

		ScheduleNodeview.AppendColumn ("Label", new Gtk.CellRendererText (), "text", 0);
		ScheduleNodeview.AppendColumn ("Pin", new Gtk.CellRendererText (), "text", 1);
		ScheduleNodeview.AppendColumn ("Time", new Gtk.CellRendererText (), "text", 2);

		ScheduleNodeview.Show ();
		ShowAll ();
	}

	private void CleatePlotInterface ()
	{
		if ((vboxPlot.Children [0] as PlotView) == null) {
			var pv = new OxyPlot.GtkSharp.PlotView ();
			vboxPlot.PackStart (pv);
			(vboxPlot [pv] as VBox.BoxChild).Position = 0;

			var pm = new OxyPlot.PlotModel (){ PlotType = OxyPlot.PlotType.Cartesian };
			var xAxis =	new OxyPlot.Axes.LinearAxis {
				Position = AxisPosition.Bottom,
				Minimum = -10,
				Maximum = 0,
				MinimumPadding = 0,
				MaximumPadding = 0,
				MajorGridlineColor = OxyPlot.OxyColors.Gray,
				MajorGridlineThickness = .5,
				MajorGridlineStyle = OxyPlot.LineStyle.Solid
			};

			var yAxis = new LinearAxis {
				Position = AxisPosition.Left,
				Minimum = 0,
				Maximum = 5,
				AbsoluteMaximum = 5.1,
				AbsoluteMinimum = -0.1,
				MaximumPadding = 5,
				MinimumPadding = 5,
				IsPanEnabled = false,
				IsZoomEnabled = false,
				MajorGridlineColor = OxyPlot.OxyColors.Gray,
				MajorGridlineThickness = .5,
				MajorGridlineStyle = OxyPlot.LineStyle.Solid,
			};

			pm.Axes.Add (xAxis);
			pm.Axes.Add (yAxis);

			pm.Series.Add (CreateSeriesSuitableForDecimation ());

			pv.Model = pm;

			pv.Show ();
		}
	}

	private static LineSeries CreateSeriesSuitableForDecimation ()
	{
		var s1 = new LineSeries ();

		int n = 20000;
		for (int i = 0; i < n; i++) {
			s1.Points.Add (new DataPoint ((double)i / n, Math.Sin (i)));
		}

		return s1;
	}

	#endregion
}

namespace Prototype
{
	class ScheduleNode : Gtk.TreeNode
	{
		[Gtk.TreeNodeValue (Column = 0)]
		public string Label;
		[Gtk.TreeNodeValue (Column = 1)]
		public string Pin;
		[Gtk.TreeNodeValue (Column = 2)]
		public DateTime Time;

		public ScheduleNode (string label) : this (label, "UNKNOWN", DateTime.Now)
		{
		}

		public ScheduleNode (string label, string pin, DateTime time)
		{
			Label = label;
			Pin = pin;
			Time = time;
		}
	}
}