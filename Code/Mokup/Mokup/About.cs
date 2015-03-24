using System;

namespace Mokup
{
	public static class About
	{
		static Gtk.AboutDialog ADialog = new  Gtk.AboutDialog ();

		static About ()
		{
		}

		public static void Show ()
		{
			ADialog = new Gtk.AboutDialog ();
			ADialog.ProgramName = "MicroLog";
			ADialog.Version = "0.0.1";
			ADialog.Authors = new string[]{ "Daniel Pollack" };
			ADialog.Run ();
			ADialog.Destroy ();
		}
	}
}

