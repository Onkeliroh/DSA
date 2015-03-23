using System;

namespace Mokup
{
	public partial class MeassureWindow : Gtk.Window
	{
		public MeassureWindow () :
			base (Gtk.WindowType.Toplevel)
		{
			this.Build ();
		}
	}
}

