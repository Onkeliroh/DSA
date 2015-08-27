using System;
using PrototypeBackend;

namespace PreferencesDialog
{
	public partial class PreferencesDialog : Gtk.Dialog
	{
		public PreferencesDialog (Gtk.Window parent = null) : base ("", parent, Gtk.DialogFlags.Modal)
		{
			this.Build ();
		}
	}
}

