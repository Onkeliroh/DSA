
// This file has been generated by the GUI designer. Do not modify.
namespace SignalConfigurationDialog
{
	public partial class SignalConfigurationDialog
	{
		private global::Gtk.HBox hbox1;
		
		private global::Gtk.Frame frame1;
		
		private global::Gtk.Alignment GtkAlignment4;
		
		private global::Gtk.Table table1;
		
		private global::Gtk.Button btnAdd;
		
		private global::Gtk.Button btnRemove;
		
		private global::Gtk.ComboBox cbPins;
		
		private global::Gtk.ScrolledWindow GtkScrolledWindow;
		
		private global::Gtk.NodeView nvSignal;
		
		private global::Gtk.Label label1;
		
		private global::Gtk.Label GtkLabel2;
		
		private global::Gtk.Table table2;
		
		private global::Gtk.ColorButton cbColor;
		
		private global::Gtk.ComboBoxEntry cbeUnit;
		
		private global::Gtk.Entry entry5;
		
		private global::Gtk.Entry entryName;
		
		private global::Gtk.Entry entryOperation;
		
		private global::Gtk.HSeparator hseparator2;
		
		private global::Gtk.Label label2;
		
		private global::Gtk.Label label3;
		
		private global::Gtk.Label label4;
		
		private global::Gtk.Label label5;
		
		private global::Gtk.Label label6;
		
		private global::Gtk.Label label7;
		
		private global::Gtk.Label label8;
		
		private global::Gtk.VSeparator vseparator1;
		
		private global::Gtk.Button buttonCancel;
		
		private global::Gtk.Button buttonOk;

		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget SignalConfigurationDialog.SignalConfigurationDialog
			this.Name = "SignalConfigurationDialog.SignalConfigurationDialog";
			this.WindowPosition = ((global::Gtk.WindowPosition)(4));
			// Internal child SignalConfigurationDialog.SignalConfigurationDialog.VBox
			global::Gtk.VBox w1 = this.VBox;
			w1.Name = "dialog1_VBox";
			w1.BorderWidth = ((uint)(2));
			// Container child dialog1_VBox.Gtk.Box+BoxChild
			this.hbox1 = new global::Gtk.HBox ();
			this.hbox1.Name = "hbox1";
			this.hbox1.Homogeneous = true;
			this.hbox1.Spacing = 6;
			// Container child hbox1.Gtk.Box+BoxChild
			this.frame1 = new global::Gtk.Frame ();
			this.frame1.Name = "frame1";
			this.frame1.ShadowType = ((global::Gtk.ShadowType)(1));
			// Container child frame1.Gtk.Container+ContainerChild
			this.GtkAlignment4 = new global::Gtk.Alignment (0F, 0F, 1F, 1F);
			this.GtkAlignment4.Name = "GtkAlignment4";
			this.GtkAlignment4.LeftPadding = ((uint)(12));
			// Container child GtkAlignment4.Gtk.Container+ContainerChild
			this.table1 = new global::Gtk.Table (((uint)(3)), ((uint)(3)), false);
			this.table1.Name = "table1";
			this.table1.RowSpacing = ((uint)(6));
			this.table1.ColumnSpacing = ((uint)(6));
			// Container child table1.Gtk.Table+TableChild
			this.btnAdd = new global::Gtk.Button ();
			this.btnAdd.CanFocus = true;
			this.btnAdd.Name = "btnAdd";
			this.btnAdd.UseUnderline = true;
			this.btnAdd.Label = global::Mono.Unix.Catalog.GetString ("Add");
			global::Gtk.Image w2 = new global::Gtk.Image ();
			w2.Pixbuf = global::Stetic.IconLoader.LoadIcon (this, "gtk-add", global::Gtk.IconSize.Menu);
			this.btnAdd.Image = w2;
			this.table1.Add (this.btnAdd);
			global::Gtk.Table.TableChild w3 = ((global::Gtk.Table.TableChild)(this.table1 [this.btnAdd]));
			w3.TopAttach = ((uint)(1));
			w3.BottomAttach = ((uint)(2));
			w3.LeftAttach = ((uint)(2));
			w3.RightAttach = ((uint)(3));
			w3.XOptions = ((global::Gtk.AttachOptions)(4));
			w3.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.btnRemove = new global::Gtk.Button ();
			this.btnRemove.Sensitive = false;
			this.btnRemove.CanFocus = true;
			this.btnRemove.Name = "btnRemove";
			this.btnRemove.UseUnderline = true;
			this.btnRemove.Label = global::Mono.Unix.Catalog.GetString ("Remove");
			global::Gtk.Image w4 = new global::Gtk.Image ();
			w4.Pixbuf = global::Stetic.IconLoader.LoadIcon (this, "gtk-remove", global::Gtk.IconSize.Menu);
			this.btnRemove.Image = w4;
			this.table1.Add (this.btnRemove);
			global::Gtk.Table.TableChild w5 = ((global::Gtk.Table.TableChild)(this.table1 [this.btnRemove]));
			w5.TopAttach = ((uint)(2));
			w5.BottomAttach = ((uint)(3));
			w5.LeftAttach = ((uint)(2));
			w5.RightAttach = ((uint)(3));
			w5.XOptions = ((global::Gtk.AttachOptions)(4));
			w5.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.cbPins = global::Gtk.ComboBox.NewText ();
			this.cbPins.AppendText (global::Mono.Unix.Catalog.GetString ("kfludfludlfdludlc"));
			this.cbPins.Name = "cbPins";
			this.cbPins.Active = 0;
			this.table1.Add (this.cbPins);
			global::Gtk.Table.TableChild w6 = ((global::Gtk.Table.TableChild)(this.table1 [this.cbPins]));
			w6.TopAttach = ((uint)(1));
			w6.BottomAttach = ((uint)(2));
			w6.LeftAttach = ((uint)(1));
			w6.RightAttach = ((uint)(2));
			w6.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.GtkScrolledWindow = new global::Gtk.ScrolledWindow ();
			this.GtkScrolledWindow.Name = "GtkScrolledWindow";
			this.GtkScrolledWindow.ShadowType = ((global::Gtk.ShadowType)(1));
			// Container child GtkScrolledWindow.Gtk.Container+ContainerChild
			this.nvSignal = new global::Gtk.NodeView ();
			this.nvSignal.CanFocus = true;
			this.nvSignal.Name = "nvSignal";
			this.GtkScrolledWindow.Add (this.nvSignal);
			this.table1.Add (this.GtkScrolledWindow);
			global::Gtk.Table.TableChild w8 = ((global::Gtk.Table.TableChild)(this.table1 [this.GtkScrolledWindow]));
			w8.RightAttach = ((uint)(3));
			// Container child table1.Gtk.Table+TableChild
			this.label1 = new global::Gtk.Label ();
			this.label1.Name = "label1";
			this.label1.LabelProp = global::Mono.Unix.Catalog.GetString ("Available Inputs:");
			this.table1.Add (this.label1);
			global::Gtk.Table.TableChild w9 = ((global::Gtk.Table.TableChild)(this.table1 [this.label1]));
			w9.TopAttach = ((uint)(1));
			w9.BottomAttach = ((uint)(2));
			w9.XOptions = ((global::Gtk.AttachOptions)(4));
			w9.YOptions = ((global::Gtk.AttachOptions)(4));
			this.GtkAlignment4.Add (this.table1);
			this.frame1.Add (this.GtkAlignment4);
			this.GtkLabel2 = new global::Gtk.Label ();
			this.GtkLabel2.Name = "GtkLabel2";
			this.GtkLabel2.LabelProp = global::Mono.Unix.Catalog.GetString ("<b>Analog Inputs:</b>");
			this.GtkLabel2.UseMarkup = true;
			this.frame1.LabelWidget = this.GtkLabel2;
			this.hbox1.Add (this.frame1);
			global::Gtk.Box.BoxChild w12 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.frame1]));
			w12.Position = 0;
			// Container child hbox1.Gtk.Box+BoxChild
			this.table2 = new global::Gtk.Table (((uint)(7)), ((uint)(4)), false);
			this.table2.Name = "table2";
			this.table2.RowSpacing = ((uint)(6));
			this.table2.ColumnSpacing = ((uint)(6));
			// Container child table2.Gtk.Table+TableChild
			this.cbColor = new global::Gtk.ColorButton ();
			this.cbColor.TooltipMarkup = "This is the color: this signals value colored in the plot.";
			this.cbColor.CanFocus = true;
			this.cbColor.Events = ((global::Gdk.EventMask)(784));
			this.cbColor.Name = "cbColor";
			this.table2.Add (this.cbColor);
			global::Gtk.Table.TableChild w13 = ((global::Gtk.Table.TableChild)(this.table2 [this.cbColor]));
			w13.TopAttach = ((uint)(3));
			w13.BottomAttach = ((uint)(4));
			w13.LeftAttach = ((uint)(2));
			w13.RightAttach = ((uint)(4));
			w13.XOptions = ((global::Gtk.AttachOptions)(4));
			w13.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.cbeUnit = global::Gtk.ComboBoxEntry.NewText ();
			this.cbeUnit.TooltipMarkup = "The Unit of the Result.";
			this.cbeUnit.Name = "cbeUnit";
			this.table2.Add (this.cbeUnit);
			global::Gtk.Table.TableChild w14 = ((global::Gtk.Table.TableChild)(this.table2 [this.cbeUnit]));
			w14.TopAttach = ((uint)(2));
			w14.BottomAttach = ((uint)(3));
			w14.LeftAttach = ((uint)(2));
			w14.RightAttach = ((uint)(4));
			w14.XOptions = ((global::Gtk.AttachOptions)(4));
			w14.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.entry5 = new global::Gtk.Entry ();
			this.entry5.TooltipMarkup = "The resulting frequency of this signal. Depending on the used analog inputs frequ" +
			"encies.";
			this.entry5.CanFocus = true;
			this.entry5.Name = "entry5";
			this.entry5.IsEditable = false;
			this.entry5.InvisibleChar = '●';
			this.table2.Add (this.entry5);
			global::Gtk.Table.TableChild w15 = ((global::Gtk.Table.TableChild)(this.table2 [this.entry5]));
			w15.TopAttach = ((uint)(5));
			w15.BottomAttach = ((uint)(6));
			w15.LeftAttach = ((uint)(2));
			w15.RightAttach = ((uint)(3));
			w15.XOptions = ((global::Gtk.AttachOptions)(4));
			w15.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.entryName = new global::Gtk.Entry ();
			this.entryName.TooltipMarkup = "The name the result will by logged under.";
			this.entryName.CanFocus = true;
			this.entryName.Name = "entryName";
			this.entryName.IsEditable = true;
			this.entryName.InvisibleChar = '●';
			this.table2.Add (this.entryName);
			global::Gtk.Table.TableChild w16 = ((global::Gtk.Table.TableChild)(this.table2 [this.entryName]));
			w16.LeftAttach = ((uint)(2));
			w16.RightAttach = ((uint)(4));
			w16.XOptions = ((global::Gtk.AttachOptions)(4));
			w16.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.entryOperation = new global::Gtk.Entry ();
			this.entryOperation.TooltipMarkup = "The Operation you wish to be processed.\n\ne.g:\nGiven analog inputs A0 and A5.\nAddi" +
			"ng those two would require you to type in: \"A0+A5\".\nAlternatively you could you " +
			"the inputs name. (e.g. TemperatureSensor0 + TemperatureSensor5)";
			this.entryOperation.CanFocus = true;
			this.entryOperation.Name = "entryOperation";
			this.entryOperation.IsEditable = true;
			this.entryOperation.InvisibleChar = '●';
			this.table2.Add (this.entryOperation);
			global::Gtk.Table.TableChild w17 = ((global::Gtk.Table.TableChild)(this.table2 [this.entryOperation]));
			w17.TopAttach = ((uint)(1));
			w17.BottomAttach = ((uint)(2));
			w17.LeftAttach = ((uint)(2));
			w17.RightAttach = ((uint)(4));
			w17.XOptions = ((global::Gtk.AttachOptions)(4));
			w17.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.hseparator2 = new global::Gtk.HSeparator ();
			this.hseparator2.Name = "hseparator2";
			this.table2.Add (this.hseparator2);
			global::Gtk.Table.TableChild w18 = ((global::Gtk.Table.TableChild)(this.table2 [this.hseparator2]));
			w18.TopAttach = ((uint)(4));
			w18.BottomAttach = ((uint)(5));
			w18.LeftAttach = ((uint)(1));
			w18.RightAttach = ((uint)(4));
			w18.XOptions = ((global::Gtk.AttachOptions)(4));
			w18.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.label2 = new global::Gtk.Label ();
			this.label2.Name = "label2";
			this.label2.Xalign = 0F;
			this.label2.LabelProp = global::Mono.Unix.Catalog.GetString ("Name:");
			this.table2.Add (this.label2);
			global::Gtk.Table.TableChild w19 = ((global::Gtk.Table.TableChild)(this.table2 [this.label2]));
			w19.LeftAttach = ((uint)(1));
			w19.RightAttach = ((uint)(2));
			w19.XOptions = ((global::Gtk.AttachOptions)(4));
			w19.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.label3 = new global::Gtk.Label ();
			this.label3.Name = "label3";
			this.label3.Xalign = 0F;
			this.label3.LabelProp = global::Mono.Unix.Catalog.GetString ("Operation:");
			this.table2.Add (this.label3);
			global::Gtk.Table.TableChild w20 = ((global::Gtk.Table.TableChild)(this.table2 [this.label3]));
			w20.TopAttach = ((uint)(1));
			w20.BottomAttach = ((uint)(2));
			w20.LeftAttach = ((uint)(1));
			w20.RightAttach = ((uint)(2));
			w20.XOptions = ((global::Gtk.AttachOptions)(4));
			w20.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.label4 = new global::Gtk.Label ();
			this.label4.Name = "label4";
			this.label4.Xalign = 0F;
			this.label4.LabelProp = global::Mono.Unix.Catalog.GetString ("Unit:");
			this.table2.Add (this.label4);
			global::Gtk.Table.TableChild w21 = ((global::Gtk.Table.TableChild)(this.table2 [this.label4]));
			w21.TopAttach = ((uint)(2));
			w21.BottomAttach = ((uint)(3));
			w21.LeftAttach = ((uint)(1));
			w21.RightAttach = ((uint)(2));
			w21.XOptions = ((global::Gtk.AttachOptions)(4));
			w21.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.label5 = new global::Gtk.Label ();
			this.label5.Name = "label5";
			this.label5.Xalign = 0F;
			this.label5.LabelProp = global::Mono.Unix.Catalog.GetString ("Color:");
			this.table2.Add (this.label5);
			global::Gtk.Table.TableChild w22 = ((global::Gtk.Table.TableChild)(this.table2 [this.label5]));
			w22.TopAttach = ((uint)(3));
			w22.BottomAttach = ((uint)(4));
			w22.LeftAttach = ((uint)(1));
			w22.RightAttach = ((uint)(2));
			w22.XOptions = ((global::Gtk.AttachOptions)(4));
			w22.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.label6 = new global::Gtk.Label ();
			this.label6.Name = "label6";
			this.label6.Xalign = 0F;
			this.label6.LabelProp = global::Mono.Unix.Catalog.GetString ("Collective Frequency:");
			this.table2.Add (this.label6);
			global::Gtk.Table.TableChild w23 = ((global::Gtk.Table.TableChild)(this.table2 [this.label6]));
			w23.TopAttach = ((uint)(5));
			w23.BottomAttach = ((uint)(6));
			w23.LeftAttach = ((uint)(1));
			w23.RightAttach = ((uint)(2));
			w23.XOptions = ((global::Gtk.AttachOptions)(4));
			w23.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.label7 = new global::Gtk.Label ();
			this.label7.Name = "label7";
			this.label7.LabelProp = global::Mono.Unix.Catalog.GetString ("s");
			this.table2.Add (this.label7);
			global::Gtk.Table.TableChild w24 = ((global::Gtk.Table.TableChild)(this.table2 [this.label7]));
			w24.TopAttach = ((uint)(5));
			w24.BottomAttach = ((uint)(6));
			w24.LeftAttach = ((uint)(3));
			w24.RightAttach = ((uint)(4));
			w24.XOptions = ((global::Gtk.AttachOptions)(4));
			w24.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.label8 = new global::Gtk.Label ();
			this.label8.Name = "label8";
			this.table2.Add (this.label8);
			global::Gtk.Table.TableChild w25 = ((global::Gtk.Table.TableChild)(this.table2 [this.label8]));
			w25.TopAttach = ((uint)(6));
			w25.BottomAttach = ((uint)(7));
			w25.LeftAttach = ((uint)(2));
			w25.RightAttach = ((uint)(3));
			// Container child table2.Gtk.Table+TableChild
			this.vseparator1 = new global::Gtk.VSeparator ();
			this.vseparator1.Name = "vseparator1";
			this.table2.Add (this.vseparator1);
			global::Gtk.Table.TableChild w26 = ((global::Gtk.Table.TableChild)(this.table2 [this.vseparator1]));
			w26.BottomAttach = ((uint)(7));
			w26.XOptions = ((global::Gtk.AttachOptions)(4));
			this.hbox1.Add (this.table2);
			global::Gtk.Box.BoxChild w27 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.table2]));
			w27.Position = 1;
			w27.Expand = false;
			w27.Fill = false;
			w1.Add (this.hbox1);
			global::Gtk.Box.BoxChild w28 = ((global::Gtk.Box.BoxChild)(w1 [this.hbox1]));
			w28.Position = 0;
			// Internal child SignalConfigurationDialog.SignalConfigurationDialog.ActionArea
			global::Gtk.HButtonBox w29 = this.ActionArea;
			w29.Name = "dialog1_ActionArea";
			w29.Spacing = 10;
			w29.BorderWidth = ((uint)(5));
			w29.LayoutStyle = ((global::Gtk.ButtonBoxStyle)(4));
			// Container child dialog1_ActionArea.Gtk.ButtonBox+ButtonBoxChild
			this.buttonCancel = new global::Gtk.Button ();
			this.buttonCancel.CanDefault = true;
			this.buttonCancel.CanFocus = true;
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.UseStock = true;
			this.buttonCancel.UseUnderline = true;
			this.buttonCancel.Label = "gtk-cancel";
			this.AddActionWidget (this.buttonCancel, -6);
			global::Gtk.ButtonBox.ButtonBoxChild w30 = ((global::Gtk.ButtonBox.ButtonBoxChild)(w29 [this.buttonCancel]));
			w30.Expand = false;
			w30.Fill = false;
			// Container child dialog1_ActionArea.Gtk.ButtonBox+ButtonBoxChild
			this.buttonOk = new global::Gtk.Button ();
			this.buttonOk.CanDefault = true;
			this.buttonOk.CanFocus = true;
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.UseStock = true;
			this.buttonOk.UseUnderline = true;
			this.buttonOk.Label = "gtk-apply";
			this.AddActionWidget (this.buttonOk, -10);
			global::Gtk.ButtonBox.ButtonBoxChild w31 = ((global::Gtk.ButtonBox.ButtonBoxChild)(w29 [this.buttonOk]));
			w31.Position = 1;
			w31.Expand = false;
			w31.Fill = false;
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.DefaultWidth = 676;
			this.DefaultHeight = 300;
			this.Show ();
		}
	}
}
