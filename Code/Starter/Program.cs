using System;
using System.Linq;
using Gtk;
using Frontend;
using Starter.Properties;

namespace Starter
{
	/// <summary>
	/// Main class.
	/// </summary>
	static class MainClass
	{
		public static bool IsVerbose = false;

		/// <summary>
		/// The entry point of the program, where the program control starts and ends.
		/// </summary>
		/// <param name="args">The command-line arguments.</param>
		public static void Main (string[] args)
		{
			if (System.Diagnostics.Process.GetProcessesByName (System.IO.Path.GetFileNameWithoutExtension (System.Reflection.Assembly.GetEntryAssembly ().Location)).Count () > 1)
			{
				Console.WriteLine ("Another instance was found. Exiting now.");
				return;
			}

			try
			{
				RunWindow ();
			} catch (Exception ex)
			{
				Console.Error.WriteLine (ex);
			}
			Console.WriteLine ("Finished");
		}

		/// <summary>
		/// Starts the interface and controller.
		/// </summary>
		/// <param name="ConfigPath">Config path.</param>
		private static void RunWindow (string ConfigPath = null)
		{
			try
			{
				Application.Init ();

				Gtk.Rc.ParseString (Resources.gtkrc);

				var con = new Backend.Controller (ConfigPath);
				MainWindow win = new MainWindow (con, IsVerbose);
				win.SetGtkTheme (Resources.gtkrc);
				win.Show ();
				Application.Run ();
			} catch (Exception ex)
			{
				Console.Error.WriteLine (ex);
			}
		}
	}
}
