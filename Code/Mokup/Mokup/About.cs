using System;

namespace Mokup
{
	public static class About
	{
		static Gtk.AboutDialog ADialog = new  Gtk.AboutDialog ();

		static About ()
		{
			ADialog.Title = "Programm Titel";
			ADialog.Authors = new string[]{ "Daniel Pollack" };
		}

		public static void Show ()
		{
			ADialog.ShowAll ();
		}
	}
}

