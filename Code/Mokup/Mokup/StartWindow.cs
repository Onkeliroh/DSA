using System;
using Gtk;

namespace Mokup
{
	public partial class StartWindow : Gtk.Window
	{
		public StartWindow () :
			base (Gtk.WindowType.Toplevel)
		{
			this.Build ();
			CreateObjects ();

			this.DeleteEvent += new global::Gtk.DeleteEventHandler (OnDelete);
			this.quitAction.Activated += new System.EventHandler (OnDelete);
			this.aboutAction.Activated += new System.EventHandler (delegate {
				About.Show ();
			});
		}

		private void OnDelete (object obj, EventArgs args)
		{
			Application.Quit ();
		}

		private void OnConfigBtnClick (object obj, EventArgs args)
		{
			OpenConfigurationWindow ();
		}

		private void CreateObjects ()
		{
			Gtk.TreeViewColumn SessionNameColumn = new Gtk.TreeViewColumn ();
			SessionNameColumn.Title = "Name";
			SessionNameColumn.AddAttribute (new Gtk.CellRendererText (), "text", 0);

			Gtk.TreeViewColumn SessionPathColumn = new Gtk.TreeViewColumn ();
			SessionPathColumn.Title = "Path";
			SessionNameColumn.AddAttribute (new Gtk.CellRendererText (), "text", 1);

			this.treeViewSessions.AppendColumn (SessionNameColumn);
			this.treeViewSessions.AppendColumn (SessionPathColumn);
			treeViewSessions.Model = new Gtk.ListStore (typeof(string), typeof(string));
			ShowAll ();
		}

		private void OpenConfigurationWindow ()
		{
			OpenConfigurationWindow ("");
		}

		private void OpenConfigurationWindow (string path)
		{
			new ConfigurationWindow ().Show ();
		}
	}
}

