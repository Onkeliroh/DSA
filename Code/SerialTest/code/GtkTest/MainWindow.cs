using System;
using Gtk;
using System.ComponentModel;
using System.Runtime.InteropServices.WindowsRuntime;

public partial class MainWindow: Gtk.Window
{
	public MainWindow () : base (Gtk.WindowType.Toplevel)
	{
		Build ();
	}

	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}

	protected void OnDelete (object sender, EventArgs e)
	{
		Application.Quit ();
	}
}
