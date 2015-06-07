using Gtk;
using PrototypeBackend;

namespace Prototype
{
	class MainClass
	{
		public static ArduinoController.ArduinoController arduinoController{ private set; get; }

		public static PrototypeBackend.Controller mainController = new Controller ();

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
			arduinoController = new ArduinoController.ArduinoController ();
		}
	}
}
