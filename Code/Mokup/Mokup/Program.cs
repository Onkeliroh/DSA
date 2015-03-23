using System;
using Gtk;

namespace Mokup
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Application.Init ();
			StartWindow win = new StartWindow ();
			win.Show ();
			Application.Run ();
		}
	}
}
