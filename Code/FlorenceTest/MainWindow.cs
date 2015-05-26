using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Florence;
using System.Threading;
using System.Threading.Tasks;

using Florence.GtkSharp;
using Florence;

using Gtk;
using System.Runtime.InteropServices;

public partial class MainWindow: Gtk.Window
{
	private Florence.GtkSharp.PlotWidget FlorenceWidget;

	public MainWindow () : base (Gtk.WindowType.Toplevel)
	{
		Build ();

		InitializeComponents ();
	}

	private void InitializeComponents()
	{	
		FlorenceWidget = new Florence.GtkSharp.PlotWidget ();

		framePlot.Add (FlorenceWidget);

		FlorenceWidget.ButtonPressEvent += (o, e) => Console.WriteLine ("Click");
	}

	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}

	protected void OnBtnGOClicked (object sender, EventArgs e)
	{
	}
}
