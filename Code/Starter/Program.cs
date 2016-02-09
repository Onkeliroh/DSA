using System;
using System.Collections.Generic;
using System.Linq;
using ArgumentParser;
using ArgumentParser.Arguments;
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
		/// <summary>
		/// The help argument short version.
		/// </summary>
		public static readonly IArgument HelpShort = new ArgumentParser.Arguments.POSIX.POSIXShortFlag ('h', "print help", 0, 0, null, 0);
		/// <summary>
		/// The help argument long version.
		/// </summary>
		public static readonly IArgument HelpLong = new ArgumentParser.Arguments.POSIX.POSIXLongFlag ("help", "print help");
		/// <summary>
		/// The configuration file argument short version.
		/// </summary>
		public static readonly IArgument ConfigFileShort = new ArgumentParser.Arguments.POSIX.POSIXShortArgument ('c', "configuration file path");
		/// <summary>
		/// The verbose flag. <b>true</b> = verbose, <b>false</b> = NOT verbose
		/// </summary>
		public static readonly IArgument VerboseShort = new ArgumentParser.Arguments.POSIX.POSIXShortFlag ('v', "verpose output", 0, 0, null, 0);

		private static bool verbose = false;

		/// <summary>
		/// Array of all arguments
		/// </summary>
		private static readonly IArgument[] arguments = {
//			HelpShort,
//			HelpLong,
//			ConfigFileShort,
			VerboseShort,
		};

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

			var ret = Parser.GetParameters (args, new ParserOptions (ParameterTokenStyle.POSIX), arguments);
			var matchedParameters = ret.OfType<ParameterPair> ().Where (o => o.Matched == true);
			var matchedFlags = ret.OfType<FlagPair> ().Where (o => o.Matched == true);

			try
			{
				if (matchedFlags.Any (o => o.Key == VerboseShort.Key))
				{
					verbose = true;	
				}

//				if (matchedFlags.Any (o => o.Key == HelpShort.Key)) {
//					PrintHelp ();
//				} else if (matchedFlags.Any (o => o.Key == HelpLong.Key)) {
//					PrintHelp ();
//				} else if (matchedParameters.Any (o => o.Key == ConfigFileShort.Key)) {
//					RunWindow (matchedParameters.Single (o => o.Argument == ConfigFileShort).Values.ToList () [0] as string);
//				} else {
//					RunWindow (System.Environment.CurrentDirectory + "/Config.ini");
//				}
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
				MainWindow win = new MainWindow (con, verbose);
				win.SetGtkTheme (Resources.gtkrc);
				win.Show ();
				Application.Run ();
			} catch (Exception ex)
			{
				Console.Error.WriteLine (ex);
			}
		}

		/// <summary>
		/// Prints the help.
		/// </summary>
		private static void PrintHelp ()
		{
			Console.WriteLine ("Key | Description | Default Value");
			foreach (IArgument a in arguments)
			{
				Console.WriteLine (string.Format ("{0} | {1} | {2}", a.Key, a.Description, a.DefaultValue));
			}
		}
	}
}
