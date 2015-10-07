
// This file has been generated by the GUI designer. Do not modify.
namespace PrototypeDebugWindow
{
	public partial class PreferencesDialog
	{
		private global::Gtk.Notebook notebook1;
		
		private global::Gtk.Table table2;
		
		private global::Gtk.Button button5;
		
		private global::Gtk.CheckButton cbtnAutoConnect;
		
		private global::Gtk.CheckButton checkbutton1;
		
		private global::Gtk.Entry entry1;
		
		private global::Gtk.HSeparator hseparator1;
		
		private global::Gtk.Label label4;
		
		private global::Gtk.Label label6;
		
		private global::Gtk.Label label7;
		
		private global::Gtk.Label label8;
		
		private global::Gtk.Label label1;
		
		private global::Gtk.Table table3;
		
		private global::Gtk.Button button6;
		
		private global::Gtk.ComboBox combobox1;
		
		private global::Gtk.Entry entry2;
		
		private global::Gtk.Label label10;
		
		private global::Gtk.Label label9;
		
		private global::Gtk.Label label5;
		
		private global::Gtk.Button buttonCancel;
		
		private global::Gtk.Button buttonOk;

		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget PrototypeDebugWindow.PreferencesDialog
			this.Name = "PrototypeDebugWindow.PreferencesDialog";
			this.WindowPosition = ((global::Gtk.WindowPosition)(4));
			// Internal child PrototypeDebugWindow.PreferencesDialog.VBox
			global::Gtk.VBox w1 = this.VBox;
			w1.Name = "dialog1_VBox";
			w1.BorderWidth = ((uint)(2));
			// Container child dialog1_VBox.Gtk.Box+BoxChild
			this.notebook1 = new global::Gtk.Notebook ();
			this.notebook1.CanFocus = true;
			this.notebook1.Name = "notebook1";
			this.notebook1.CurrentPage = 1;
			this.notebook1.TabPos = ((global::Gtk.PositionType)(0));
			// Container child notebook1.Gtk.Notebook+NotebookChild
			this.table2 = new global::Gtk.Table (((uint)(5)), ((uint)(3)), false);
			this.table2.Name = "table2";
			this.table2.RowSpacing = ((uint)(6));
			this.table2.ColumnSpacing = ((uint)(6));
			this.table2.BorderWidth = ((uint)(6));
			// Container child table2.Gtk.Table+TableChild
			this.button5 = new global::Gtk.Button ();
			this.button5.CanFocus = true;
			this.button5.Name = "button5";
			this.button5.UseUnderline = true;
			global::Gtk.Image w2 = new global::Gtk.Image ();
			w2.Pixbuf = global::Stetic.IconLoader.LoadIcon (this, "gtk-open", global::Gtk.IconSize.Menu);
			this.button5.Image = w2;
			this.table2.Add (this.button5);
			global::Gtk.Table.TableChild w3 = ((global::Gtk.Table.TableChild)(this.table2 [this.button5]));
			w3.LeftAttach = ((uint)(2));
			w3.RightAttach = ((uint)(3));
			w3.XOptions = ((global::Gtk.AttachOptions)(4));
			w3.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.cbtnAutoConnect = new global::Gtk.CheckButton ();
			this.cbtnAutoConnect.CanFocus = true;
			this.cbtnAutoConnect.Name = "cbtnAutoConnect";
			this.cbtnAutoConnect.Label = "";
			this.cbtnAutoConnect.DrawIndicator = true;
			this.cbtnAutoConnect.UseUnderline = true;
			this.table2.Add (this.cbtnAutoConnect);
			global::Gtk.Table.TableChild w4 = ((global::Gtk.Table.TableChild)(this.table2 [this.cbtnAutoConnect]));
			w4.TopAttach = ((uint)(1));
			w4.BottomAttach = ((uint)(2));
			w4.LeftAttach = ((uint)(1));
			w4.RightAttach = ((uint)(2));
			w4.XOptions = ((global::Gtk.AttachOptions)(4));
			w4.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.checkbutton1 = new global::Gtk.CheckButton ();
			this.checkbutton1.CanFocus = true;
			this.checkbutton1.Name = "checkbutton1";
			this.checkbutton1.Label = global::Mono.Unix.Catalog.GetString ("checkbutton1");
			this.checkbutton1.Active = true;
			this.checkbutton1.DrawIndicator = true;
			this.checkbutton1.UseUnderline = true;
			this.table2.Add (this.checkbutton1);
			global::Gtk.Table.TableChild w5 = ((global::Gtk.Table.TableChild)(this.table2 [this.checkbutton1]));
			w5.TopAttach = ((uint)(4));
			w5.BottomAttach = ((uint)(5));
			w5.LeftAttach = ((uint)(1));
			w5.RightAttach = ((uint)(2));
			w5.XOptions = ((global::Gtk.AttachOptions)(4));
			w5.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.entry1 = new global::Gtk.Entry ();
			this.entry1.CanFocus = true;
			this.entry1.Name = "entry1";
			this.entry1.IsEditable = true;
			this.entry1.InvisibleChar = '●';
			this.table2.Add (this.entry1);
			global::Gtk.Table.TableChild w6 = ((global::Gtk.Table.TableChild)(this.table2 [this.entry1]));
			w6.LeftAttach = ((uint)(1));
			w6.RightAttach = ((uint)(2));
			w6.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.hseparator1 = new global::Gtk.HSeparator ();
			this.hseparator1.Name = "hseparator1";
			this.table2.Add (this.hseparator1);
			global::Gtk.Table.TableChild w7 = ((global::Gtk.Table.TableChild)(this.table2 [this.hseparator1]));
			w7.TopAttach = ((uint)(3));
			w7.BottomAttach = ((uint)(4));
			w7.RightAttach = ((uint)(3));
			w7.XOptions = ((global::Gtk.AttachOptions)(4));
			w7.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.label4 = new global::Gtk.Label ();
			this.label4.Name = "label4";
			this.label4.Xalign = 0F;
			this.label4.LabelProp = global::Mono.Unix.Catalog.GetString ("Path to Board Configuration File:");
			this.table2.Add (this.label4);
			global::Gtk.Table.TableChild w8 = ((global::Gtk.Table.TableChild)(this.table2 [this.label4]));
			w8.XOptions = ((global::Gtk.AttachOptions)(4));
			w8.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.label6 = new global::Gtk.Label ();
			this.label6.Name = "label6";
			this.label6.Xalign = 0F;
			this.label6.LabelProp = global::Mono.Unix.Catalog.GetString ("Attempt automatic connection:");
			this.table2.Add (this.label6);
			global::Gtk.Table.TableChild w9 = ((global::Gtk.Table.TableChild)(this.table2 [this.label6]));
			w9.TopAttach = ((uint)(1));
			w9.BottomAttach = ((uint)(2));
			w9.XOptions = ((global::Gtk.AttachOptions)(4));
			w9.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.label7 = new global::Gtk.Label ();
			this.label7.Name = "label7";
			this.label7.Xalign = 0F;
			this.label7.LabelProp = global::Mono.Unix.Catalog.GetString ("Expert mode:");
			this.table2.Add (this.label7);
			global::Gtk.Table.TableChild w10 = ((global::Gtk.Table.TableChild)(this.table2 [this.label7]));
			w10.TopAttach = ((uint)(4));
			w10.BottomAttach = ((uint)(5));
			w10.XOptions = ((global::Gtk.AttachOptions)(4));
			w10.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.label8 = new global::Gtk.Label ();
			this.label8.Name = "label8";
			this.table2.Add (this.label8);
			global::Gtk.Table.TableChild w11 = ((global::Gtk.Table.TableChild)(this.table2 [this.label8]));
			w11.TopAttach = ((uint)(2));
			w11.BottomAttach = ((uint)(3));
			w11.LeftAttach = ((uint)(1));
			w11.RightAttach = ((uint)(2));
			this.notebook1.Add (this.table2);
			// Notebook tab
			this.label1 = new global::Gtk.Label ();
			this.label1.Name = "label1";
			this.label1.LabelProp = global::Mono.Unix.Catalog.GetString ("General");
			this.notebook1.SetTabLabel (this.table2, this.label1);
			this.label1.ShowAll ();
			// Container child notebook1.Gtk.Notebook+NotebookChild
			this.table3 = new global::Gtk.Table (((uint)(3)), ((uint)(3)), false);
			this.table3.Name = "table3";
			this.table3.RowSpacing = ((uint)(6));
			this.table3.ColumnSpacing = ((uint)(6));
			this.table3.BorderWidth = ((uint)(6));
			// Container child table3.Gtk.Table+TableChild
			this.button6 = new global::Gtk.Button ();
			this.button6.CanFocus = true;
			this.button6.Name = "button6";
			this.button6.UseUnderline = true;
			global::Gtk.Image w13 = new global::Gtk.Image ();
			w13.Pixbuf = global::Stetic.IconLoader.LoadIcon (this, "gtk-open", global::Gtk.IconSize.Menu);
			this.button6.Image = w13;
			this.table3.Add (this.button6);
			global::Gtk.Table.TableChild w14 = ((global::Gtk.Table.TableChild)(this.table3 [this.button6]));
			w14.TopAttach = ((uint)(1));
			w14.BottomAttach = ((uint)(2));
			w14.LeftAttach = ((uint)(2));
			w14.RightAttach = ((uint)(3));
			w14.XOptions = ((global::Gtk.AttachOptions)(4));
			w14.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table3.Gtk.Table+TableChild
			this.combobox1 = global::Gtk.ComboBox.NewText ();
			this.combobox1.Name = "combobox1";
			this.table3.Add (this.combobox1);
			global::Gtk.Table.TableChild w15 = ((global::Gtk.Table.TableChild)(this.table3 [this.combobox1]));
			w15.LeftAttach = ((uint)(1));
			w15.RightAttach = ((uint)(3));
			w15.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table3.Gtk.Table+TableChild
			this.entry2 = new global::Gtk.Entry ();
			this.entry2.CanFocus = true;
			this.entry2.Name = "entry2";
			this.entry2.IsEditable = true;
			this.entry2.InvisibleChar = '●';
			this.table3.Add (this.entry2);
			global::Gtk.Table.TableChild w16 = ((global::Gtk.Table.TableChild)(this.table3 [this.entry2]));
			w16.TopAttach = ((uint)(1));
			w16.BottomAttach = ((uint)(2));
			w16.LeftAttach = ((uint)(1));
			w16.RightAttach = ((uint)(2));
			w16.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table3.Gtk.Table+TableChild
			this.label10 = new global::Gtk.Label ();
			this.label10.Name = "label10";
			this.label10.Xalign = 0F;
			this.label10.LabelProp = global::Mono.Unix.Catalog.GetString ("Diagnostics-file path:");
			this.table3.Add (this.label10);
			global::Gtk.Table.TableChild w17 = ((global::Gtk.Table.TableChild)(this.table3 [this.label10]));
			w17.TopAttach = ((uint)(1));
			w17.BottomAttach = ((uint)(2));
			w17.XOptions = ((global::Gtk.AttachOptions)(4));
			w17.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table3.Gtk.Table+TableChild
			this.label9 = new global::Gtk.Label ();
			this.label9.Name = "label9";
			this.label9.Xalign = 0F;
			this.label9.LabelProp = global::Mono.Unix.Catalog.GetString ("Log Level:");
			this.table3.Add (this.label9);
			global::Gtk.Table.TableChild w18 = ((global::Gtk.Table.TableChild)(this.table3 [this.label9]));
			w18.XOptions = ((global::Gtk.AttachOptions)(4));
			w18.YOptions = ((global::Gtk.AttachOptions)(4));
			this.notebook1.Add (this.table3);
			global::Gtk.Notebook.NotebookChild w19 = ((global::Gtk.Notebook.NotebookChild)(this.notebook1 [this.table3]));
			w19.Position = 1;
			// Notebook tab
			this.label5 = new global::Gtk.Label ();
			this.label5.Name = "label5";
			this.label5.LabelProp = global::Mono.Unix.Catalog.GetString ("System Logger");
			this.notebook1.SetTabLabel (this.table3, this.label5);
			this.label5.ShowAll ();
			w1.Add (this.notebook1);
			global::Gtk.Box.BoxChild w20 = ((global::Gtk.Box.BoxChild)(w1 [this.notebook1]));
			w20.Position = 0;
			// Internal child PrototypeDebugWindow.PreferencesDialog.ActionArea
			global::Gtk.HButtonBox w21 = this.ActionArea;
			w21.Name = "dialog1_ActionArea";
			w21.Spacing = 10;
			w21.BorderWidth = ((uint)(5));
			w21.LayoutStyle = ((global::Gtk.ButtonBoxStyle)(4));
			// Container child dialog1_ActionArea.Gtk.ButtonBox+ButtonBoxChild
			this.buttonCancel = new global::Gtk.Button ();
			this.buttonCancel.CanDefault = true;
			this.buttonCancel.CanFocus = true;
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.UseStock = true;
			this.buttonCancel.UseUnderline = true;
			this.buttonCancel.Label = "gtk-cancel";
			this.AddActionWidget (this.buttonCancel, -6);
			global::Gtk.ButtonBox.ButtonBoxChild w22 = ((global::Gtk.ButtonBox.ButtonBoxChild)(w21 [this.buttonCancel]));
			w22.Expand = false;
			w22.Fill = false;
			// Container child dialog1_ActionArea.Gtk.ButtonBox+ButtonBoxChild
			this.buttonOk = new global::Gtk.Button ();
			this.buttonOk.CanDefault = true;
			this.buttonOk.CanFocus = true;
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.UseStock = true;
			this.buttonOk.UseUnderline = true;
			this.buttonOk.Label = "gtk-ok";
			this.AddActionWidget (this.buttonOk, -5);
			global::Gtk.ButtonBox.ButtonBoxChild w23 = ((global::Gtk.ButtonBox.ButtonBoxChild)(w21 [this.buttonOk]));
			w23.Position = 1;
			w23.Expand = false;
			w23.Fill = false;
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.DefaultWidth = 1026;
			this.DefaultHeight = 478;
			this.Show ();
		}
	}
}