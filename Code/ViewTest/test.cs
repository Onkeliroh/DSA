using System;
using Gtk;

class Program
{
	private static Gtk.TreeView treeview = null;

	static void OnEdited (object sender, Gtk.EditedArgs args)
	{
		Gtk.TreeSelection selection = treeview.Selection;
		Gtk.TreeIter iter;
		selection.GetSelected (out iter);

		treeview.Model.SetValue (iter, 1, args.NewText); // the CellRendererText
	}

	static void Main (string[] args)
	{
		Gtk.Application.Init ();

		Gtk.Window window = new Window ("TreeView ComboTest");
		window.WidthRequest = 200;
		window.HeightRequest = 150;

		Gtk.ListStore treeModel = new ListStore (typeof(string), typeof(string));
		treeview = new TreeView (treeModel);

		// Values to be chosen in the ComboBox
		Gtk.ListStore comboModel = new ListStore (typeof(string));
		Gtk.ComboBox comboBox = new ComboBox (comboModel);
		comboBox.AppendText ("<Please select>");
		comboBox.AppendText ("A");
		comboBox.AppendText ("B");
		comboBox.AppendText ("C");
		comboBox.Active = 0;

		Gtk.TreeViewColumn comboCol = new TreeViewColumn ();
		Gtk.CellRendererCombo comboCell = new CellRendererCombo ();
		comboCol.Title = "Combo Column";
		comboCol.PackStart (comboCell, true);
		comboCol.AddAttribute (comboCell, "text", 1);
		comboCell.Editable = true;
		comboCell.Edited += OnEdited;
		comboCell.TextColumn = 0;
		comboCell.Text = comboBox.ActiveText;
		comboCell.Model = comboModel;
		comboCell.WidthChars = 20;

		Gtk.TreeViewColumn valueCol = new TreeViewColumn ();
		Gtk.CellRendererText valueCell = new CellRendererText ();
		valueCol.Title = "Value";
		valueCol.PackStart (valueCell, true);
		valueCol.AddAttribute (valueCell, "text", 1);
		valueCol.Visible = false;

		treeview.AppendColumn (comboCol);
		treeview.AppendColumn (valueCol);

		// Append the values used for the tests
		treeModel.AppendValues ("comboBox1", "<Please select>"); // the string value setted for the first column does not appear.
		treeModel.AppendValues ("comboBox2", "<Please select>");
		treeModel.AppendValues ("comboBox3", "<Please select>");

		window.Add (treeview);
		window.ShowAll ();
		Gtk.Application.Run ();
	}
}
