﻿using System;

namespace Mokup
{
	public partial class MeassureWindow : Gtk.Window
	{
		public MeassureWindow () :
			base (Gtk.WindowType.Toplevel)
		{
			this.Build ();

			this.DeleteEvent += new global::Gtk.DeleteEventHandler (OnDelete);
			this.quitAction.Activated += new EventHandler (OnDelete);
			this.aboutAction.Activated += new EventHandler (delegate {
				About.Show ();
			});

			InitializeComponents ();

			ShowAll ();
		}

		private void OnDelete (Object obj, EventArgs args)
		{
			this.Destroy ();
		}

		void InitializeComponents ()
		{
			this.ContentHBox.PackEnd (new CollapseFrame ());
		}
	}
}

