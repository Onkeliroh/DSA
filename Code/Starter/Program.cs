using System;
using System.Collections.Generic;
using System.Linq;
using ArgumentParser;
using ArgumentParser.Arguments;
using Gtk;
using Frontend;

namespace Starter
{
	class MainClass
	{
		public static readonly IArgument HelpShort = new ArgumentParser.Arguments.POSIX.POSIXShortFlag ('h', "print help", 0, 0, null, 0);
		public static readonly IArgument HelpLong = new ArgumentParser.Arguments.POSIX.POSIXLongFlag ("help", "print help");
		public static readonly IArgument ConfigFileShort = new ArgumentParser.Arguments.POSIX.POSIXShortArgument ('c', "configuration file path", defaultValue: "/home/onkeliroh/Bachelorarbeit/Resources/Config.ini");

		private static readonly IArgument[] arguments = {
			HelpShort,
			HelpLong,
			ConfigFileShort,
		};

		public static void Main (string[] args)
		{
			var ret = Parser.GetParameters (args, new ParserOptions (ParameterTokenStyle.POSIX), arguments);
			var matchedParameters = ret.OfType<ParameterPair> ().Where (o => o.Matched == true);
			var matchedFlags = ret.OfType<FlagPair> ().Where (o => o.Matched == true);

			try
			{
				if (matchedFlags.Any (o => o.Key == HelpShort.Key))
				{
					PrintHelp ();
				} else if (matchedFlags.Any (o => o.Key == HelpLong.Key))
				{
					PrintHelp ();
				} else if (matchedParameters.Any (o => o.Key == ConfigFileShort.Key))
				{
					RunWindow (null);
				} else
				{
					RunWindow (ConfigFileShort.DefaultValue as string);
				}
			} catch (Exception ex)
			{
				Console.Error.WriteLine (ex);
			}
		}

		private static void RunWindow (string ConfigPath)
		{
			try
			{
				Application.Init ();
				MainWindow win = new MainWindow (new PrototypeBackend.Controller (ConfigPath));
				win.Show ();
				Application.Run ();
			} catch (Exception ex)
			{
				Console.Error.WriteLine (ex);
			}
		}

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
