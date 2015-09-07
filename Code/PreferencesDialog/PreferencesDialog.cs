using System;
using PrototypeBackend;
using PrototypeBackend;

namespace PreferencesDialog
{
	public partial class PreferencesDialog : Gtk.Dialog
	{
		public PreferencesDialog (Gtk.Window parent = null, Controller con = null) : base ("", parent, Gtk.DialogFlags.Modal)
		{
			this.Build ();
		}
	}
}

