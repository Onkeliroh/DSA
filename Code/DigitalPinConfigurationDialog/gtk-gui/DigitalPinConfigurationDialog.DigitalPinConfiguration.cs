
// This file has been generated by the GUI designer. Do not modify.
namespace DigitalPinConfigurationDialog
{
	public partial class DigitalPinConfiguration
	{
		private global::Gtk.Table table3;
		
		private global::Gtk.ColorButton cbColor;
		
		private global::Gtk.ComboBox cbPin;
		
		private global::Gtk.Entry entryName;
		
		private global::Gtk.Label lblColor;
		
		private global::Gtk.Label lblName;
		
		private global::Gtk.Label lblPin;
		
		private global::Gtk.Button buttonCancel;
		
		private global::Gtk.Button buttonOk;

		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget DigitalPinConfigurationDialog.DigitalPinConfiguration
			this.Name = "DigitalPinConfigurationDialog.DigitalPinConfiguration";
			this.Title = global::Mono.Unix.Catalog.GetString ("Digital Output Configuration Dialog");
			this.WindowPosition = ((global::Gtk.WindowPosition)(4));
			// Internal child DigitalPinConfigurationDialog.DigitalPinConfiguration.VBox
			global::Gtk.VBox w1 = this.VBox;
			w1.Name = "dialog1_VBox";
			w1.BorderWidth = ((uint)(2));
			// Container child dialog1_VBox.Gtk.Box+BoxChild
			this.table3 = new global::Gtk.Table (((uint)(3)), ((uint)(2)), false);
			this.table3.Name = "table3";
			this.table3.RowSpacing = ((uint)(6));
			this.table3.ColumnSpacing = ((uint)(6));
			// Container child table3.Gtk.Table+TableChild
			this.cbColor = new global::Gtk.ColorButton ();
			this.cbColor.CanFocus = true;
			this.cbColor.Events = ((global::Gdk.EventMask)(784));
			this.cbColor.Name = "cbColor";
			this.table3.Add (this.cbColor);
			global::Gtk.Table.TableChild w2 = ((global::Gtk.Table.TableChild)(this.table3 [this.cbColor]));
			w2.TopAttach = ((uint)(2));
			w2.BottomAttach = ((uint)(3));
			w2.LeftAttach = ((uint)(1));
			w2.RightAttach = ((uint)(2));
			w2.XOptions = ((global::Gtk.AttachOptions)(4));
			w2.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table3.Gtk.Table+TableChild
			this.cbPin = global::Gtk.ComboBox.NewText ();
			this.cbPin.Name = "cbPin";
			this.table3.Add (this.cbPin);
			global::Gtk.Table.TableChild w3 = ((global::Gtk.Table.TableChild)(this.table3 [this.cbPin]));
			w3.TopAttach = ((uint)(1));
			w3.BottomAttach = ((uint)(2));
			w3.LeftAttach = ((uint)(1));
			w3.RightAttach = ((uint)(2));
			w3.XOptions = ((global::Gtk.AttachOptions)(4));
			w3.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table3.Gtk.Table+TableChild
			this.entryName = new global::Gtk.Entry ();
			this.entryName.CanFocus = true;
			this.entryName.Name = "entryName";
			this.entryName.IsEditable = true;
			this.entryName.InvisibleChar = '●';
			this.table3.Add (this.entryName);
			global::Gtk.Table.TableChild w4 = ((global::Gtk.Table.TableChild)(this.table3 [this.entryName]));
			w4.LeftAttach = ((uint)(1));
			w4.RightAttach = ((uint)(2));
			w4.XOptions = ((global::Gtk.AttachOptions)(4));
			w4.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table3.Gtk.Table+TableChild
			this.lblColor = new global::Gtk.Label ();
			this.lblColor.Name = "lblColor";
			this.lblColor.Xalign = 0F;
			this.lblColor.LabelProp = global::Mono.Unix.Catalog.GetString ("Color:");
			this.table3.Add (this.lblColor);
			global::Gtk.Table.TableChild w5 = ((global::Gtk.Table.TableChild)(this.table3 [this.lblColor]));
			w5.TopAttach = ((uint)(2));
			w5.BottomAttach = ((uint)(3));
			w5.XOptions = ((global::Gtk.AttachOptions)(4));
			w5.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table3.Gtk.Table+TableChild
			this.lblName = new global::Gtk.Label ();
			this.lblName.Name = "lblName";
			this.lblName.Xalign = 0F;
			this.lblName.LabelProp = global::Mono.Unix.Catalog.GetString ("Name:");
			this.table3.Add (this.lblName);
			global::Gtk.Table.TableChild w6 = ((global::Gtk.Table.TableChild)(this.table3 [this.lblName]));
			w6.XOptions = ((global::Gtk.AttachOptions)(4));
			w6.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table3.Gtk.Table+TableChild
			this.lblPin = new global::Gtk.Label ();
			this.lblPin.Name = "lblPin";
			this.lblPin.Xalign = 0F;
			this.lblPin.LabelProp = global::Mono.Unix.Catalog.GetString ("Pin:");
			this.table3.Add (this.lblPin);
			global::Gtk.Table.TableChild w7 = ((global::Gtk.Table.TableChild)(this.table3 [this.lblPin]));
			w7.TopAttach = ((uint)(1));
			w7.BottomAttach = ((uint)(2));
			w7.XOptions = ((global::Gtk.AttachOptions)(4));
			w7.YOptions = ((global::Gtk.AttachOptions)(4));
			w1.Add (this.table3);
			global::Gtk.Box.BoxChild w8 = ((global::Gtk.Box.BoxChild)(w1 [this.table3]));
			w8.Position = 0;
			w8.Expand = false;
			w8.Fill = false;
			// Internal child DigitalPinConfigurationDialog.DigitalPinConfiguration.ActionArea
			global::Gtk.HButtonBox w9 = this.ActionArea;
			w9.Name = "dialog1_ActionArea";
			w9.Spacing = 10;
			w9.BorderWidth = ((uint)(1));
			w9.LayoutStyle = ((global::Gtk.ButtonBoxStyle)(4));
			// Container child dialog1_ActionArea.Gtk.ButtonBox+ButtonBoxChild
			this.buttonCancel = new global::Gtk.Button ();
			this.buttonCancel.CanDefault = true;
			this.buttonCancel.CanFocus = true;
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.UseStock = true;
			this.buttonCancel.UseUnderline = true;
			this.buttonCancel.Label = "gtk-cancel";
			this.AddActionWidget (this.buttonCancel, -6);
			global::Gtk.ButtonBox.ButtonBoxChild w10 = ((global::Gtk.ButtonBox.ButtonBoxChild)(w9 [this.buttonCancel]));
			w10.Expand = false;
			w10.Fill = false;
			// Container child dialog1_ActionArea.Gtk.ButtonBox+ButtonBoxChild
			this.buttonOk = new global::Gtk.Button ();
			this.buttonOk.CanDefault = true;
			this.buttonOk.CanFocus = true;
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.UseStock = true;
			this.buttonOk.UseUnderline = true;
			this.buttonOk.Label = "gtk-apply";
			this.AddActionWidget (this.buttonOk, -10);
			global::Gtk.ButtonBox.ButtonBoxChild w11 = ((global::Gtk.ButtonBox.ButtonBoxChild)(w9 [this.buttonOk]));
			w11.Position = 1;
			w11.Expand = false;
			w11.Fill = false;
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.DefaultWidth = 210;
			this.DefaultHeight = 130;
			this.Show ();
			this.entryName.Changed += new global::System.EventHandler (this.OnEntryNameChanged);
			this.cbPin.Changed += new global::System.EventHandler (this.OnCbPinChanged);
			this.cbColor.ColorSet += new global::System.EventHandler (this.OnCbColorColorSet);
			this.buttonOk.Clicked += new global::System.EventHandler (this.OnButtonOkClicked);
		}
	}
}
