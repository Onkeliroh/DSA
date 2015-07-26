using System;
using Gtk;

public partial class MainWindow: Gtk.Window
{
	public MainWindow () : base (Gtk.WindowType.Toplevel)
	{
		Build ();

		Gtk.ListStore store = new ListStore (typeof(ComboBoxEntry));
		treeview1 = new TreeView (store);

		var box = new ComboBoxEntry (new string[]{ "test1", "test2", "test3" });

		var rend = new CellRendererCombo ();
		var column = new TreeViewColumn ();

		column.PackStart (rend, true);
//		column.AddAttribute(rend,)


		ShowAll ();
	}

	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}
}

public class CBClass : TreeViewColumn
{
	
}
