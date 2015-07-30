using System;
using Gtk;

namespace PrototypeDebugWindow
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Application.Init ();
			DebugWindow win = new DebugWindow ();
			win.Show ();
			Application.Run ();
		}
	}
}
