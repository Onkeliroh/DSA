
// This file has been generated by the GUI designer. Do not modify.
namespace AnalogPinConfigurationDialog
{
	public partial class AnalogPinConfiguration
	{
		private global::Gtk.Table table3;
		
		private global::Gtk.ColorButton cbColor;
		
		private global::Gtk.ComboBox cbPin;
		
		private global::Gtk.ComboBoxEntry cbUnit;
		
		private global::Gtk.Entry entryName;
		
		private global::Gtk.HSeparator hseparator1;
		
		private global::Gtk.Label label1;
		
		private global::Gtk.Label label2;
		
		private global::Gtk.Label label3;
		
		private global::Gtk.Label label4;
		
		private global::Gtk.Label label5;
		
		private global::Gtk.Label label6;
		
		private global::Gtk.Label lblColor;
		
		private global::Gtk.Label lblName;
		
		private global::Gtk.Label lblPin;
		
		private global::Gtk.SpinButton sbFrequency;
		
		private global::Gtk.SpinButton sbInterval;
		
		private global::Gtk.SpinButton sbOffset;
		
		private global::Gtk.SpinButton sbSlope;
		
		private global::Gtk.Button buttonCancel;
		
		private global::Gtk.Button buttonOk;

		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget AnalogPinConfigurationDialog.AnalogPinConfiguration
			this.Name = "AnalogPinConfigurationDialog.AnalogPinConfiguration";
			this.Title = global::Mono.Unix.Catalog.GetString ("Analog Input Configuration Dialog");
			this.WindowPosition = ((global::Gtk.WindowPosition)(4));
			// Internal child AnalogPinConfigurationDialog.AnalogPinConfiguration.VBox
			global::Gtk.VBox w1 = this.VBox;
			w1.Name = "dialog1_VBox";
			w1.BorderWidth = ((uint)(2));
			// Container child dialog1_VBox.Gtk.Box+BoxChild
			this.table3 = new global::Gtk.Table (((uint)(9)), ((uint)(3)), false);
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
			w2.RightAttach = ((uint)(3));
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
			w3.RightAttach = ((uint)(3));
			w3.XOptions = ((global::Gtk.AttachOptions)(4));
			w3.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table3.Gtk.Table+TableChild
			this.cbUnit = global::Gtk.ComboBoxEntry.NewText ();
			this.cbUnit.Name = "cbUnit";
			this.table3.Add (this.cbUnit);
			global::Gtk.Table.TableChild w4 = ((global::Gtk.Table.TableChild)(this.table3 [this.cbUnit]));
			w4.TopAttach = ((uint)(4));
			w4.BottomAttach = ((uint)(5));
			w4.LeftAttach = ((uint)(1));
			w4.RightAttach = ((uint)(3));
			w4.XOptions = ((global::Gtk.AttachOptions)(4));
			w4.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table3.Gtk.Table+TableChild
			this.entryName = new global::Gtk.Entry ();
			this.entryName.CanFocus = true;
			this.entryName.Name = "entryName";
			this.entryName.IsEditable = true;
			this.entryName.InvisibleChar = '●';
			this.table3.Add (this.entryName);
			global::Gtk.Table.TableChild w5 = ((global::Gtk.Table.TableChild)(this.table3 [this.entryName]));
			w5.LeftAttach = ((uint)(1));
			w5.RightAttach = ((uint)(3));
			w5.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table3.Gtk.Table+TableChild
			this.hseparator1 = new global::Gtk.HSeparator ();
			this.hseparator1.Name = "hseparator1";
			this.table3.Add (this.hseparator1);
			global::Gtk.Table.TableChild w6 = ((global::Gtk.Table.TableChild)(this.table3 [this.hseparator1]));
			w6.TopAttach = ((uint)(3));
			w6.BottomAttach = ((uint)(4));
			w6.RightAttach = ((uint)(3));
			w6.XOptions = ((global::Gtk.AttachOptions)(4));
			w6.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table3.Gtk.Table+TableChild
			this.label1 = new global::Gtk.Label ();
			this.label1.Name = "label1";
			this.label1.Xalign = 0F;
			this.label1.LabelProp = global::Mono.Unix.Catalog.GetString ("Unit:");
			this.table3.Add (this.label1);
			global::Gtk.Table.TableChild w7 = ((global::Gtk.Table.TableChild)(this.table3 [this.label1]));
			w7.TopAttach = ((uint)(4));
			w7.BottomAttach = ((uint)(5));
			w7.XOptions = ((global::Gtk.AttachOptions)(4));
			w7.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table3.Gtk.Table+TableChild
			this.label2 = new global::Gtk.Label ();
			this.label2.Name = "label2";
			this.label2.Xalign = 0F;
			this.label2.LabelProp = global::Mono.Unix.Catalog.GetString ("Slope:");
			this.table3.Add (this.label2);
			global::Gtk.Table.TableChild w8 = ((global::Gtk.Table.TableChild)(this.table3 [this.label2]));
			w8.TopAttach = ((uint)(5));
			w8.BottomAttach = ((uint)(6));
			w8.XOptions = ((global::Gtk.AttachOptions)(4));
			w8.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table3.Gtk.Table+TableChild
			this.label3 = new global::Gtk.Label ();
			this.label3.Name = "label3";
			this.label3.Xalign = 0F;
			this.label3.LabelProp = global::Mono.Unix.Catalog.GetString ("Offset:");
			this.table3.Add (this.label3);
			global::Gtk.Table.TableChild w9 = ((global::Gtk.Table.TableChild)(this.table3 [this.label3]));
			w9.TopAttach = ((uint)(6));
			w9.BottomAttach = ((uint)(7));
			w9.XOptions = ((global::Gtk.AttachOptions)(4));
			w9.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table3.Gtk.Table+TableChild
			this.label4 = new global::Gtk.Label ();
			this.label4.Name = "label4";
			this.label4.Xalign = 0F;
			this.label4.LabelProp = global::Mono.Unix.Catalog.GetString ("Frequency:");
			this.table3.Add (this.label4);
			global::Gtk.Table.TableChild w10 = ((global::Gtk.Table.TableChild)(this.table3 [this.label4]));
			w10.TopAttach = ((uint)(7));
			w10.BottomAttach = ((uint)(8));
			w10.XOptions = ((global::Gtk.AttachOptions)(4));
			w10.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table3.Gtk.Table+TableChild
			this.label5 = new global::Gtk.Label ();
			this.label5.Name = "label5";
			this.label5.Xalign = 0F;
			this.label5.LabelProp = global::Mono.Unix.Catalog.GetString ("Interval:");
			this.table3.Add (this.label5);
			global::Gtk.Table.TableChild w11 = ((global::Gtk.Table.TableChild)(this.table3 [this.label5]));
			w11.TopAttach = ((uint)(8));
			w11.BottomAttach = ((uint)(9));
			w11.XOptions = ((global::Gtk.AttachOptions)(4));
			w11.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table3.Gtk.Table+TableChild
			this.label6 = new global::Gtk.Label ();
			this.label6.Name = "label6";
			this.label6.Xalign = 0F;
			this.label6.LabelProp = global::Mono.Unix.Catalog.GetString ("s");
			this.table3.Add (this.label6);
			global::Gtk.Table.TableChild w12 = ((global::Gtk.Table.TableChild)(this.table3 [this.label6]));
			w12.TopAttach = ((uint)(7));
			w12.BottomAttach = ((uint)(8));
			w12.LeftAttach = ((uint)(2));
			w12.RightAttach = ((uint)(3));
			w12.XOptions = ((global::Gtk.AttachOptions)(6));
			w12.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table3.Gtk.Table+TableChild
			this.lblColor = new global::Gtk.Label ();
			this.lblColor.Name = "lblColor";
			this.lblColor.Xalign = 0F;
			this.lblColor.LabelProp = global::Mono.Unix.Catalog.GetString ("Color:");
			this.table3.Add (this.lblColor);
			global::Gtk.Table.TableChild w13 = ((global::Gtk.Table.TableChild)(this.table3 [this.lblColor]));
			w13.TopAttach = ((uint)(2));
			w13.BottomAttach = ((uint)(3));
			w13.XOptions = ((global::Gtk.AttachOptions)(4));
			w13.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table3.Gtk.Table+TableChild
			this.lblName = new global::Gtk.Label ();
			this.lblName.Name = "lblName";
			this.lblName.Xalign = 0F;
			this.lblName.LabelProp = global::Mono.Unix.Catalog.GetString ("Name:");
			this.table3.Add (this.lblName);
			global::Gtk.Table.TableChild w14 = ((global::Gtk.Table.TableChild)(this.table3 [this.lblName]));
			w14.XOptions = ((global::Gtk.AttachOptions)(4));
			w14.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table3.Gtk.Table+TableChild
			this.lblPin = new global::Gtk.Label ();
			this.lblPin.Name = "lblPin";
			this.lblPin.Xalign = 0F;
			this.lblPin.LabelProp = global::Mono.Unix.Catalog.GetString ("Pin:");
			this.table3.Add (this.lblPin);
			global::Gtk.Table.TableChild w15 = ((global::Gtk.Table.TableChild)(this.table3 [this.lblPin]));
			w15.TopAttach = ((uint)(1));
			w15.BottomAttach = ((uint)(2));
			w15.XOptions = ((global::Gtk.AttachOptions)(4));
			w15.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table3.Gtk.Table+TableChild
			this.sbFrequency = new global::Gtk.SpinButton (0, 100, 1);
			this.sbFrequency.CanFocus = true;
			this.sbFrequency.Name = "sbFrequency";
			this.sbFrequency.Adjustment.PageIncrement = 10;
			this.sbFrequency.ClimbRate = 1;
			this.sbFrequency.Digits = ((uint)(2));
			this.sbFrequency.Numeric = true;
			this.sbFrequency.Value = 1;
			this.table3.Add (this.sbFrequency);
			global::Gtk.Table.TableChild w16 = ((global::Gtk.Table.TableChild)(this.table3 [this.sbFrequency]));
			w16.TopAttach = ((uint)(7));
			w16.BottomAttach = ((uint)(8));
			w16.LeftAttach = ((uint)(1));
			w16.RightAttach = ((uint)(2));
			w16.XOptions = ((global::Gtk.AttachOptions)(4));
			w16.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table3.Gtk.Table+TableChild
			this.sbInterval = new global::Gtk.SpinButton (1, 100, 1);
			this.sbInterval.CanFocus = true;
			this.sbInterval.Name = "sbInterval";
			this.sbInterval.Adjustment.PageIncrement = 10;
			this.sbInterval.ClimbRate = 1;
			this.sbInterval.Numeric = true;
			this.sbInterval.Value = 1;
			this.table3.Add (this.sbInterval);
			global::Gtk.Table.TableChild w17 = ((global::Gtk.Table.TableChild)(this.table3 [this.sbInterval]));
			w17.TopAttach = ((uint)(8));
			w17.BottomAttach = ((uint)(9));
			w17.LeftAttach = ((uint)(1));
			w17.RightAttach = ((uint)(3));
			w17.XOptions = ((global::Gtk.AttachOptions)(4));
			w17.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table3.Gtk.Table+TableChild
			this.sbOffset = new global::Gtk.SpinButton (0, 100, 1);
			this.sbOffset.CanFocus = true;
			this.sbOffset.Name = "sbOffset";
			this.sbOffset.Adjustment.PageIncrement = 10;
			this.sbOffset.ClimbRate = 1;
			this.sbOffset.Digits = ((uint)(10));
			this.sbOffset.Numeric = true;
			this.table3.Add (this.sbOffset);
			global::Gtk.Table.TableChild w18 = ((global::Gtk.Table.TableChild)(this.table3 [this.sbOffset]));
			w18.TopAttach = ((uint)(6));
			w18.BottomAttach = ((uint)(7));
			w18.LeftAttach = ((uint)(1));
			w18.RightAttach = ((uint)(3));
			w18.XOptions = ((global::Gtk.AttachOptions)(4));
			w18.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table3.Gtk.Table+TableChild
			this.sbSlope = new global::Gtk.SpinButton (0, 100, 1);
			this.sbSlope.CanFocus = true;
			this.sbSlope.Name = "sbSlope";
			this.sbSlope.Adjustment.PageIncrement = 10;
			this.sbSlope.ClimbRate = 1;
			this.sbSlope.Digits = ((uint)(10));
			this.sbSlope.Numeric = true;
			this.sbSlope.Value = 1;
			this.table3.Add (this.sbSlope);
			global::Gtk.Table.TableChild w19 = ((global::Gtk.Table.TableChild)(this.table3 [this.sbSlope]));
			w19.TopAttach = ((uint)(5));
			w19.BottomAttach = ((uint)(6));
			w19.LeftAttach = ((uint)(1));
			w19.RightAttach = ((uint)(3));
			w19.XOptions = ((global::Gtk.AttachOptions)(4));
			w19.YOptions = ((global::Gtk.AttachOptions)(4));
			w1.Add (this.table3);
			global::Gtk.Box.BoxChild w20 = ((global::Gtk.Box.BoxChild)(w1 [this.table3]));
			w20.Position = 0;
			// Internal child AnalogPinConfigurationDialog.AnalogPinConfiguration.ActionArea
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
			this.buttonOk.Label = "gtk-apply";
			this.AddActionWidget (this.buttonOk, -10);
			global::Gtk.ButtonBox.ButtonBoxChild w23 = ((global::Gtk.ButtonBox.ButtonBoxChild)(w21 [this.buttonOk]));
			w23.Position = 1;
			w23.Expand = false;
			w23.Fill = false;
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.DefaultWidth = 276;
			this.DefaultHeight = 308;
			this.Show ();
			this.sbSlope.Changed += new global::System.EventHandler (this.OnSbSlopeChanged);
			this.sbOffset.Changed += new global::System.EventHandler (this.OnSbOffsetChanged);
			this.sbInterval.Changed += new global::System.EventHandler (this.OnSbIntervalChanged);
			this.sbFrequency.Changed += new global::System.EventHandler (this.OnSbFrequencyChanged);
			this.entryName.Changed += new global::System.EventHandler (this.OnEntryNameChanged);
			this.cbUnit.Changed += new global::System.EventHandler (this.OnCbUnitChanged);
			this.cbPin.Changed += new global::System.EventHandler (this.OnCbPinChanged);
			this.cbColor.Clicked += new global::System.EventHandler (this.OnCbColorClicked);
			this.buttonOk.Clicked += new global::System.EventHandler (this.OnButtonOkClicked);
		}
	}
}
