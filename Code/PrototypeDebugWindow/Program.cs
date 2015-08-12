using System;
using Gtk;
using GLib;

namespace Frontend
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			GLib.UnhandledExceptionHandler h = new GLib.UnhandledExceptionHandler (OnException);
			GLib.ExceptionManager.UnhandledException += h;

			Application.Init ();
			MainWindow win = new MainWindow ();
			win.Show ();
			Application.Run ();
		}

		public static void OnException (GLib.UnhandledExceptionArgs args)
		{
			Console.Error.WriteLine (args.ExceptionObject);
			args.ExitApplication = true;
		}
	}
}
