using System;
using PrototypeBackend;

namespace PrototypeManualTests
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			RunAddTest ();

		}

		public static void RunAddTest ()
		{
			var controller = new Controller ();

			controller.AddScheduler (new Scheduler (){ DueTime = new DateTime (1) });
		}
	}
}
