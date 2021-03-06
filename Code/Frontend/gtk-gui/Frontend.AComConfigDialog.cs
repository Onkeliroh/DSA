
// This file has been generated by the GUI designer. Do not modify.
namespace Frontend
{
	public partial class AComConfigDialog
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

		private global::Gtk.Label GtkLabel4;

		private global::Gtk.Table table2;

		private global::Gtk.ColorButton cbColor;

		private global::Gtk.ComboBoxEntry cbeUnit;

		private global::Gtk.Entry entryName;

		private global::Gtk.Entry entryOperation;

		private global::Gtk.Image imageOperation;

		private global::Gtk.Label label2;

		private global::Gtk.Label label3;

		private global::Gtk.Label label4;

		private global::Gtk.Label label5;

		private global::Gtk.Label label6;

		private global::Gtk.Label label7;

		private global::Gtk.Label lblWarning;

		private global::Gtk.SpinButton sbMeanValuesCount;

		private global::Gtk.VSeparator vseparator1;

		private global::Gtk.Button buttonCancel;

		private global::Gtk.Button buttonOk;

		protected virtual void Build()
		{
			global::Stetic.Gui.Initialize(this);
			// Widget Frontend.AComConfigDialog
			this.Name = "Frontend.AComConfigDialog";
			this.WindowPosition = ((global::Gtk.WindowPosition)(4));
			// Internal child Frontend.AComConfigDialog.VBox
			global::Gtk.VBox w1 = this.VBox;
			w1.Name = "dialog1_VBox";
			w1.BorderWidth = ((uint)(2));
			// Container child dialog1_VBox.Gtk.Box+BoxChild
			this.hbox1 = new global::Gtk.HBox();
			this.hbox1.Name = "hbox1";
			this.hbox1.Homogeneous = true;
			this.hbox1.Spacing = 6;
			// Container child hbox1.Gtk.Box+BoxChild
			this.frame1 = new global::Gtk.Frame();
			this.frame1.Name = "frame1";
			this.frame1.ShadowType = ((global::Gtk.ShadowType)(1));
			// Container child frame1.Gtk.Container+ContainerChild
			this.GtkAlignment4 = new global::Gtk.Alignment(0F, 0F, 1F, 1F);
			this.GtkAlignment4.Name = "GtkAlignment4";
			this.GtkAlignment4.LeftPadding = ((uint)(12));
			// Container child GtkAlignment4.Gtk.Container+ContainerChild
			this.table1 = new global::Gtk.Table(((uint)(3)), ((uint)(3)), false);
			this.table1.Name = "table1";
			this.table1.RowSpacing = ((uint)(6));
			this.table1.ColumnSpacing = ((uint)(6));
			// Container child table1.Gtk.Table+TableChild
			this.btnAdd = new global::Gtk.Button();
			this.btnAdd.CanFocus = true;
			this.btnAdd.Name = "btnAdd";
			this.btnAdd.UseUnderline = true;
			this.btnAdd.Label = global::Mono.Unix.Catalog.GetString("Add");
			global::Gtk.Image w2 = new global::Gtk.Image();
			w2.Pixbuf = global::Stetic.IconLoader.LoadIcon(this, "gtk-add", global::Gtk.IconSize.Menu);
			this.btnAdd.Image = w2;
			this.table1.Add(this.btnAdd);
			global::Gtk.Table.TableChild w3 = ((global::Gtk.Table.TableChild)(this.table1[this.btnAdd]));
			w3.TopAttach = ((uint)(1));
			w3.BottomAttach = ((uint)(2));
			w3.LeftAttach = ((uint)(2));
			w3.RightAttach = ((uint)(3));
			w3.XOptions = ((global::Gtk.AttachOptions)(4));
			w3.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.btnRemove = new global::Gtk.Button();
			this.btnRemove.Sensitive = false;
			this.btnRemove.CanFocus = true;
			this.btnRemove.Name = "btnRemove";
			this.btnRemove.UseUnderline = true;
			this.btnRemove.Label = global::Mono.Unix.Catalog.GetString("Remove");
			global::Gtk.Image w4 = new global::Gtk.Image();
			w4.Pixbuf = global::Stetic.IconLoader.LoadIcon(this, "gtk-remove", global::Gtk.IconSize.Menu);
			this.btnRemove.Image = w4;
			this.table1.Add(this.btnRemove);
			global::Gtk.Table.TableChild w5 = ((global::Gtk.Table.TableChild)(this.table1[this.btnRemove]));
			w5.TopAttach = ((uint)(2));
			w5.BottomAttach = ((uint)(3));
			w5.LeftAttach = ((uint)(2));
			w5.RightAttach = ((uint)(3));
			w5.XOptions = ((global::Gtk.AttachOptions)(4));
			w5.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.cbPins = global::Gtk.ComboBox.NewText();
			this.cbPins.Name = "cbPins";
			this.table1.Add(this.cbPins);
			global::Gtk.Table.TableChild w6 = ((global::Gtk.Table.TableChild)(this.table1[this.cbPins]));
			w6.TopAttach = ((uint)(1));
			w6.BottomAttach = ((uint)(3));
			w6.LeftAttach = ((uint)(1));
			w6.RightAttach = ((uint)(2));
			w6.YOptions = ((global::Gtk.AttachOptions)(0));
			// Container child table1.Gtk.Table+TableChild
			this.GtkScrolledWindow = new global::Gtk.ScrolledWindow();
			this.GtkScrolledWindow.Name = "GtkScrolledWindow";
			this.GtkScrolledWindow.ShadowType = ((global::Gtk.ShadowType)(1));
			// Container child GtkScrolledWindow.Gtk.Container+ContainerChild
			this.nvSignal = new global::Gtk.NodeView();
			this.nvSignal.CanFocus = true;
			this.nvSignal.Name = "nvSignal";
			this.nvSignal.HoverSelection = true;
			this.GtkScrolledWindow.Add(this.nvSignal);
			this.table1.Add(this.GtkScrolledWindow);
			global::Gtk.Table.TableChild w8 = ((global::Gtk.Table.TableChild)(this.table1[this.GtkScrolledWindow]));
			w8.RightAttach = ((uint)(3));
			// Container child table1.Gtk.Table+TableChild
			this.label1 = new global::Gtk.Label();
			this.label1.Name = "label1";
			this.label1.LabelProp = global::Mono.Unix.Catalog.GetString("Available Measurements:");
			this.table1.Add(this.label1);
			global::Gtk.Table.TableChild w9 = ((global::Gtk.Table.TableChild)(this.table1[this.label1]));
			w9.TopAttach = ((uint)(1));
			w9.BottomAttach = ((uint)(3));
			w9.XOptions = ((global::Gtk.AttachOptions)(4));
			w9.YOptions = ((global::Gtk.AttachOptions)(0));
			this.GtkAlignment4.Add(this.table1);
			this.frame1.Add(this.GtkAlignment4);
			this.GtkLabel4 = new global::Gtk.Label();
			this.GtkLabel4.Name = "GtkLabel4";
			this.GtkLabel4.LabelProp = global::Mono.Unix.Catalog.GetString("<b>Measurements:</b>");
			this.GtkLabel4.UseMarkup = true;
			this.frame1.LabelWidget = this.GtkLabel4;
			this.hbox1.Add(this.frame1);
			global::Gtk.Box.BoxChild w12 = ((global::Gtk.Box.BoxChild)(this.hbox1[this.frame1]));
			w12.Position = 0;
			// Container child hbox1.Gtk.Box+BoxChild
			this.table2 = new global::Gtk.Table(((uint)(7)), ((uint)(4)), false);
			this.table2.Name = "table2";
			this.table2.RowSpacing = ((uint)(6));
			this.table2.ColumnSpacing = ((uint)(6));
			// Container child table2.Gtk.Table+TableChild
			this.cbColor = new global::Gtk.ColorButton();
			this.cbColor.TooltipMarkup = "This is the color: this signals value colored in the plot.";
			this.cbColor.CanFocus = true;
			this.cbColor.Events = ((global::Gdk.EventMask)(784));
			this.cbColor.Name = "cbColor";
			this.table2.Add(this.cbColor);
			global::Gtk.Table.TableChild w13 = ((global::Gtk.Table.TableChild)(this.table2[this.cbColor]));
			w13.TopAttach = ((uint)(4));
			w13.BottomAttach = ((uint)(5));
			w13.LeftAttach = ((uint)(2));
			w13.RightAttach = ((uint)(4));
			w13.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.cbeUnit = global::Gtk.ComboBoxEntry.NewText();
			this.cbeUnit.TooltipMarkup = "The Unit of the Result.";
			this.cbeUnit.Name = "cbeUnit";
			this.table2.Add(this.cbeUnit);
			global::Gtk.Table.TableChild w14 = ((global::Gtk.Table.TableChild)(this.table2[this.cbeUnit]));
			w14.TopAttach = ((uint)(2));
			w14.BottomAttach = ((uint)(3));
			w14.LeftAttach = ((uint)(2));
			w14.RightAttach = ((uint)(4));
			w14.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.entryName = new global::Gtk.Entry();
			this.entryName.TooltipMarkup = "The name the result will by logged under.";
			this.entryName.CanFocus = true;
			this.entryName.Name = "entryName";
			this.entryName.IsEditable = true;
			this.entryName.InvisibleChar = '●';
			this.table2.Add(this.entryName);
			global::Gtk.Table.TableChild w15 = ((global::Gtk.Table.TableChild)(this.table2[this.entryName]));
			w15.LeftAttach = ((uint)(2));
			w15.RightAttach = ((uint)(4));
			w15.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.entryOperation = new global::Gtk.Entry();
			this.entryOperation.TooltipMarkup = "The Operation you wish to be processed.\n\ne.g:\nGiven analog inputs A0 and A5.\nAddi" +
				"ng those two would require you to type in: \"A0+A5\".";
			this.entryOperation.CanFocus = true;
			this.entryOperation.Name = "entryOperation";
			this.entryOperation.IsEditable = true;
			this.entryOperation.InvisibleChar = '●';
			this.table2.Add(this.entryOperation);
			global::Gtk.Table.TableChild w16 = ((global::Gtk.Table.TableChild)(this.table2[this.entryOperation]));
			w16.TopAttach = ((uint)(1));
			w16.BottomAttach = ((uint)(2));
			w16.LeftAttach = ((uint)(2));
			w16.RightAttach = ((uint)(3));
			w16.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.imageOperation = new global::Gtk.Image();
			this.imageOperation.Name = "imageOperation";
			this.imageOperation.Pixbuf = global::Stetic.IconLoader.LoadIcon(this, "gtk-dialog-warning", global::Gtk.IconSize.Menu);
			this.table2.Add(this.imageOperation);
			global::Gtk.Table.TableChild w17 = ((global::Gtk.Table.TableChild)(this.table2[this.imageOperation]));
			w17.TopAttach = ((uint)(1));
			w17.BottomAttach = ((uint)(2));
			w17.LeftAttach = ((uint)(3));
			w17.RightAttach = ((uint)(4));
			w17.XOptions = ((global::Gtk.AttachOptions)(4));
			w17.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.label2 = new global::Gtk.Label();
			this.label2.Name = "label2";
			this.label2.Xalign = 0F;
			this.label2.LabelProp = global::Mono.Unix.Catalog.GetString("Name:");
			this.table2.Add(this.label2);
			global::Gtk.Table.TableChild w18 = ((global::Gtk.Table.TableChild)(this.table2[this.label2]));
			w18.LeftAttach = ((uint)(1));
			w18.RightAttach = ((uint)(2));
			w18.XOptions = ((global::Gtk.AttachOptions)(4));
			w18.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.label3 = new global::Gtk.Label();
			this.label3.Name = "label3";
			this.label3.Xalign = 0F;
			this.label3.LabelProp = global::Mono.Unix.Catalog.GetString("Operation:");
			this.table2.Add(this.label3);
			global::Gtk.Table.TableChild w19 = ((global::Gtk.Table.TableChild)(this.table2[this.label3]));
			w19.TopAttach = ((uint)(1));
			w19.BottomAttach = ((uint)(2));
			w19.LeftAttach = ((uint)(1));
			w19.RightAttach = ((uint)(2));
			w19.XOptions = ((global::Gtk.AttachOptions)(4));
			w19.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.label4 = new global::Gtk.Label();
			this.label4.Name = "label4";
			this.label4.Xalign = 0F;
			this.label4.LabelProp = global::Mono.Unix.Catalog.GetString("Unit:");
			this.table2.Add(this.label4);
			global::Gtk.Table.TableChild w20 = ((global::Gtk.Table.TableChild)(this.table2[this.label4]));
			w20.TopAttach = ((uint)(2));
			w20.BottomAttach = ((uint)(3));
			w20.LeftAttach = ((uint)(1));
			w20.RightAttach = ((uint)(2));
			w20.XOptions = ((global::Gtk.AttachOptions)(4));
			w20.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.label5 = new global::Gtk.Label();
			this.label5.Name = "label5";
			this.label5.Xalign = 0F;
			this.label5.LabelProp = global::Mono.Unix.Catalog.GetString("Color:");
			this.table2.Add(this.label5);
			global::Gtk.Table.TableChild w21 = ((global::Gtk.Table.TableChild)(this.table2[this.label5]));
			w21.TopAttach = ((uint)(4));
			w21.BottomAttach = ((uint)(5));
			w21.LeftAttach = ((uint)(1));
			w21.RightAttach = ((uint)(2));
			w21.XOptions = ((global::Gtk.AttachOptions)(4));
			w21.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.label6 = new global::Gtk.Label();
			this.label6.Name = "label6";
			this.label6.Xalign = 0F;
			this.label6.LabelProp = global::Mono.Unix.Catalog.GetString("Mean values count:");
			this.table2.Add(this.label6);
			global::Gtk.Table.TableChild w22 = ((global::Gtk.Table.TableChild)(this.table2[this.label6]));
			w22.TopAttach = ((uint)(3));
			w22.BottomAttach = ((uint)(4));
			w22.LeftAttach = ((uint)(1));
			w22.RightAttach = ((uint)(2));
			w22.XOptions = ((global::Gtk.AttachOptions)(4));
			w22.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.label7 = new global::Gtk.Label();
			this.label7.Name = "label7";
			this.table2.Add(this.label7);
			global::Gtk.Table.TableChild w23 = ((global::Gtk.Table.TableChild)(this.table2[this.label7]));
			w23.TopAttach = ((uint)(6));
			w23.BottomAttach = ((uint)(7));
			w23.LeftAttach = ((uint)(2));
			w23.RightAttach = ((uint)(3));
			// Container child table2.Gtk.Table+TableChild
			this.lblWarning = new global::Gtk.Label();
			this.lblWarning.Name = "lblWarning";
			this.lblWarning.LabelProp = global::Mono.Unix.Catalog.GetString("<b>Please note:</b>\nYou have selected measurements, who\'s frequency and/or interv" +
					"al are not compatible.\nIn order to apply the operation, the last available measu" +
					"rement-data will be used.");
			this.lblWarning.UseMarkup = true;
			this.lblWarning.Wrap = true;
			this.table2.Add(this.lblWarning);
			global::Gtk.Table.TableChild w24 = ((global::Gtk.Table.TableChild)(this.table2[this.lblWarning]));
			w24.TopAttach = ((uint)(5));
			w24.BottomAttach = ((uint)(6));
			w24.LeftAttach = ((uint)(1));
			w24.RightAttach = ((uint)(4));
			// Container child table2.Gtk.Table+TableChild
			this.sbMeanValuesCount = new global::Gtk.SpinButton(1D, 100D, 1D);
			this.sbMeanValuesCount.TooltipMarkup = "Amount of values to create a arithmetic mean value from";
			this.sbMeanValuesCount.CanFocus = true;
			this.sbMeanValuesCount.Name = "sbMeanValuesCount";
			this.sbMeanValuesCount.Adjustment.PageIncrement = 10D;
			this.sbMeanValuesCount.ClimbRate = 1D;
			this.sbMeanValuesCount.Numeric = true;
			this.sbMeanValuesCount.Value = 1D;
			this.table2.Add(this.sbMeanValuesCount);
			global::Gtk.Table.TableChild w25 = ((global::Gtk.Table.TableChild)(this.table2[this.sbMeanValuesCount]));
			w25.TopAttach = ((uint)(3));
			w25.BottomAttach = ((uint)(4));
			w25.LeftAttach = ((uint)(2));
			w25.RightAttach = ((uint)(4));
			w25.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.vseparator1 = new global::Gtk.VSeparator();
			this.vseparator1.Name = "vseparator1";
			this.table2.Add(this.vseparator1);
			global::Gtk.Table.TableChild w26 = ((global::Gtk.Table.TableChild)(this.table2[this.vseparator1]));
			w26.BottomAttach = ((uint)(7));
			w26.XOptions = ((global::Gtk.AttachOptions)(4));
			this.hbox1.Add(this.table2);
			global::Gtk.Box.BoxChild w27 = ((global::Gtk.Box.BoxChild)(this.hbox1[this.table2]));
			w27.Position = 1;
			w1.Add(this.hbox1);
			global::Gtk.Box.BoxChild w28 = ((global::Gtk.Box.BoxChild)(w1[this.hbox1]));
			w28.Position = 0;
			// Internal child Frontend.AComConfigDialog.ActionArea
			global::Gtk.HButtonBox w29 = this.ActionArea;
			w29.Name = "dialog1_ActionArea";
			w29.Spacing = 10;
			w29.BorderWidth = ((uint)(5));
			w29.LayoutStyle = ((global::Gtk.ButtonBoxStyle)(4));
			// Container child dialog1_ActionArea.Gtk.ButtonBox+ButtonBoxChild
			this.buttonCancel = new global::Gtk.Button();
			this.buttonCancel.CanDefault = true;
			this.buttonCancel.CanFocus = true;
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.UseStock = true;
			this.buttonCancel.UseUnderline = true;
			this.buttonCancel.Label = "gtk-cancel";
			this.AddActionWidget(this.buttonCancel, -6);
			global::Gtk.ButtonBox.ButtonBoxChild w30 = ((global::Gtk.ButtonBox.ButtonBoxChild)(w29[this.buttonCancel]));
			w30.Expand = false;
			w30.Fill = false;
			// Container child dialog1_ActionArea.Gtk.ButtonBox+ButtonBoxChild
			this.buttonOk = new global::Gtk.Button();
			this.buttonOk.CanDefault = true;
			this.buttonOk.CanFocus = true;
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.UseStock = true;
			this.buttonOk.UseUnderline = true;
			this.buttonOk.Label = "gtk-add";
			this.AddActionWidget(this.buttonOk, -10);
			global::Gtk.ButtonBox.ButtonBoxChild w31 = ((global::Gtk.ButtonBox.ButtonBoxChild)(w29[this.buttonOk]));
			w31.Position = 1;
			w31.Expand = false;
			w31.Fill = false;
			if ((this.Child != null))
			{
				this.Child.ShowAll();
			}
			this.DefaultWidth = 718;
			this.DefaultHeight = 346;
			this.lblWarning.Hide();
			this.Show();
			this.btnRemove.Clicked += new global::System.EventHandler(this.OnBtnRemoveClicked);
			this.btnAdd.Clicked += new global::System.EventHandler(this.OnBtnAddClicked);
			this.sbMeanValuesCount.ValueChanged += new global::System.EventHandler(this.OnSbMeanValuesCountChanged);
			this.entryOperation.Changed += new global::System.EventHandler(this.OnEntryOperationChanged);
			this.entryName.Changed += new global::System.EventHandler(this.OnEntryNameChanged);
			this.cbeUnit.Changed += new global::System.EventHandler(this.OnCbeUnitChanged);
			this.cbColor.ColorSet += new global::System.EventHandler(this.OnCbColorColorSet);
		}
	}
}
