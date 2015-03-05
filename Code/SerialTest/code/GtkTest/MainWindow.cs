﻿using System;
using Gtk;

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

	protected void OnDialogInfoActionActivated (object sender, EventArgs e)
	{
		var AboutDialogThing = new AboutDialog ();
		AboutDialogThing.Version = System.Reflection.Assembly.GetExecutingAssembly ().GetName ().Version.ToString();
		AboutDialogThing.Authors = new []{"Daniel Pollack"};
		AboutDialogThing.Run ();//run
		AboutDialogThing.Destroy ();//after close destroy
	}
}