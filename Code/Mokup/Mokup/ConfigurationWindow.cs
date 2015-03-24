using System;
using Gtk;

namespace Mokup
{
	public partial class ConfigurationWindow : Gtk.Window
	{
		public ConfigurationWindow () :
			base (Gtk.WindowType.Toplevel)
		{
			this.Build ();
			InitializeComponents ();

			this.DeleteEvent += new global::Gtk.DeleteEventHandler (OnDelete);
			this.quitAction.Activated += new System.EventHandler (OnDelete);
			this.aboutAction.Activated += new System.EventHandler (delegate {
				About.Show ();
			});

			Maximize ();
		}

		private void OnDelete (object obj, EventArgs args)
		{
			this.Destroy ();
		}

		private void InitializeComponents ()
		{
		}
	}
}

