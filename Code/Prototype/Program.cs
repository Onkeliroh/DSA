using Gtk;
using PrototypeBackend;

namespace Prototype
{
	class MainClass
	{
		public static Controller mainController = new Controller ();

		public static void Main (string[] args)
		{
			InitComponents ();
			
			Application.Init ();
			PrototypeWindow win = new PrototypeWindow ();
			win.Show ();
			Application.Run ();
		}

		private static void InitComponents ()
		{
		}
	}
}
