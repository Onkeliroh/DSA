using System;
using System.IO;
using PrototypeBackend;

namespace PrototypeManualTests
{
	class MainClass
	{
		public static void Main (string[] args)
		{
//			RunAddTest ();
			RunBoardParserTest ();

		}

		public static void RunAddTest ()
		{
			var controller = new Controller ();

			controller.AddScheduler (new Scheduler (){ DueTime = new DateTime (1) });
		}

		public static void RunBoardParserTest ()
		{
			const string filepath = @"BoardLoaderParser1Test.txt";
			TextWriter tw = new StreamWriter (filepath, false);
			tw.WriteLine ("uno.name=Arduino Uno");
			tw.WriteLine ("uno.numberofdigitalpins=20");
			tw.WriteLine ("uno.numberofanalogpins=6");
			tw.WriteLine ("uno.analogreference=DEFAULT 0");
			tw.WriteLine ("uno.analogreference=INTERNAL 1");
			tw.WriteLine ("uno.analogreference=EXTERNAL 3");
			tw.Close ();

			Board[] boards = BoardParser.parse (filepath);
			Console.WriteLine (boards.Length);
		}
	}
}
