
// This file has been generated by the GUI designer. Do not modify.
namespace SequenceConfigurationsDialog
{
	public partial class SequenceConfiguration
	{
		private global::Gtk.HBox hbox2;
		
		private global::Gtk.Table table3;
		
		private global::Gtk.ComboBox cbPin;
		
		private global::Gtk.Entry entryName;
		
		private global::Gtk.Frame frame1;
		
		private global::Gtk.Alignment GtkAlignment4;
		
		private global::Gtk.Table table4;
		
		private global::Gtk.Label label5;
		
		private global::Gtk.RadioButton rbRepeateContinously;
		
		private global::Gtk.RadioButton rbStopAfter;
		
		private global::Gtk.SpinButton sbRadioBtnStopAfter;
		
		private global::Gtk.Label GtkLabel2;
		
		private global::Gtk.HSeparator hseparator2;
		
		private global::Gtk.Label label2;
		
		private global::Gtk.Label label3;
		
		private global::Gtk.VSeparator vseparator1;
		
		private global::Gtk.VBox vboxOptions;
		
		private global::Gtk.ScrolledWindow GtkScrolledWindow;
		
		private global::Gtk.NodeView nvSequenceOptions;
		
		private global::Gtk.Frame frame2;
		
		private global::Gtk.Alignment GtkAlignment2;
		
		private global::Gtk.Table table5;
		
		private global::Gtk.Button btnApplyOperation;
		
		private global::Gtk.Button btnRemoveOperation;
		
		private global::Gtk.ComboBox cbState;
		
		private global::Gtk.Label label7;
		
		private global::Gtk.Label label8;
		
		private global::Gtk.Table table2;
		
		private global::Gtk.Label label4;
		
		private global::Gtk.Label label6;
		
		private global::Gtk.Label lblHours;
		
		private global::Gtk.Label lblMilliSec;
		
		private global::Gtk.Label lblSeconds;
		
		private global::Gtk.SpinButton sbDays;
		
		private global::Gtk.SpinButton sbHours;
		
		private global::Gtk.SpinButton sbMilliSec;
		
		private global::Gtk.SpinButton sbMinutes;
		
		private global::Gtk.SpinButton sbSeconds;
		
		private global::Gtk.Label GtkLabel5;
		
		private global::Gtk.HSeparator hseparator1;
		
		private global::Gtk.Button buttonCancel;
		
		private global::Gtk.Button buttonOk;

		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget SequenceConfigurationsDialog.SequenceConfiguration
			this.Name = "SequenceConfigurationsDialog.SequenceConfiguration";
			this.WindowPosition = ((global::Gtk.WindowPosition)(4));
			// Internal child SequenceConfigurationsDialog.SequenceConfiguration.VBox
			global::Gtk.VBox w1 = this.VBox;
			w1.Name = "dialog1_VBox";
			w1.BorderWidth = ((uint)(2));
			// Container child dialog1_VBox.Gtk.Box+BoxChild
			this.hbox2 = new global::Gtk.HBox ();
			this.hbox2.Name = "hbox2";
			this.hbox2.Spacing = 6;
			// Container child hbox2.Gtk.Box+BoxChild
			this.table3 = new global::Gtk.Table (((uint)(6)), ((uint)(2)), false);
			this.table3.Name = "table3";
			this.table3.RowSpacing = ((uint)(6));
			this.table3.ColumnSpacing = ((uint)(6));
			// Container child table3.Gtk.Table+TableChild
			this.cbPin = global::Gtk.ComboBox.NewText ();
			this.cbPin.Name = "cbPin";
			this.table3.Add (this.cbPin);
			global::Gtk.Table.TableChild w2 = ((global::Gtk.Table.TableChild)(this.table3 [this.cbPin]));
			w2.TopAttach = ((uint)(1));
			w2.BottomAttach = ((uint)(2));
			w2.LeftAttach = ((uint)(1));
			w2.RightAttach = ((uint)(2));
			w2.XOptions = ((global::Gtk.AttachOptions)(4));
			w2.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table3.Gtk.Table+TableChild
			this.entryName = new global::Gtk.Entry ();
			this.entryName.CanFocus = true;
			this.entryName.Name = "entryName";
			this.entryName.IsEditable = true;
			this.entryName.InvisibleChar = '●';
			this.table3.Add (this.entryName);
			global::Gtk.Table.TableChild w3 = ((global::Gtk.Table.TableChild)(this.table3 [this.entryName]));
			w3.LeftAttach = ((uint)(1));
			w3.RightAttach = ((uint)(2));
			w3.XOptions = ((global::Gtk.AttachOptions)(4));
			w3.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table3.Gtk.Table+TableChild
			this.frame1 = new global::Gtk.Frame ();
			this.frame1.Name = "frame1";
			this.frame1.ShadowType = ((global::Gtk.ShadowType)(1));
			// Container child frame1.Gtk.Container+ContainerChild
			this.GtkAlignment4 = new global::Gtk.Alignment (0F, 0F, 1F, 1F);
			this.GtkAlignment4.Name = "GtkAlignment4";
			this.GtkAlignment4.LeftPadding = ((uint)(12));
			// Container child GtkAlignment4.Gtk.Container+ContainerChild
			this.table4 = new global::Gtk.Table (((uint)(2)), ((uint)(3)), false);
			this.table4.Name = "table4";
			this.table4.RowSpacing = ((uint)(6));
			this.table4.ColumnSpacing = ((uint)(6));
			// Container child table4.Gtk.Table+TableChild
			this.label5 = new global::Gtk.Label ();
			this.label5.Name = "label5";
			this.label5.LabelProp = global::Mono.Unix.Catalog.GetString ("Cycles");
			this.table4.Add (this.label5);
			global::Gtk.Table.TableChild w4 = ((global::Gtk.Table.TableChild)(this.table4 [this.label5]));
			w4.TopAttach = ((uint)(1));
			w4.BottomAttach = ((uint)(2));
			w4.LeftAttach = ((uint)(2));
			w4.RightAttach = ((uint)(3));
			w4.XOptions = ((global::Gtk.AttachOptions)(4));
			w4.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table4.Gtk.Table+TableChild
			this.rbRepeateContinously = new global::Gtk.RadioButton (global::Mono.Unix.Catalog.GetString ("Repeate continously"));
			this.rbRepeateContinously.CanFocus = true;
			this.rbRepeateContinously.Name = "rbRepeateContinously";
			this.rbRepeateContinously.DrawIndicator = true;
			this.rbRepeateContinously.UseUnderline = true;
			this.rbRepeateContinously.Group = new global::GLib.SList (global::System.IntPtr.Zero);
			this.table4.Add (this.rbRepeateContinously);
			global::Gtk.Table.TableChild w5 = ((global::Gtk.Table.TableChild)(this.table4 [this.rbRepeateContinously]));
			w5.RightAttach = ((uint)(3));
			w5.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table4.Gtk.Table+TableChild
			this.rbStopAfter = new global::Gtk.RadioButton (global::Mono.Unix.Catalog.GetString ("Stop after"));
			this.rbStopAfter.CanFocus = true;
			this.rbStopAfter.Name = "rbStopAfter";
			this.rbStopAfter.DrawIndicator = true;
			this.rbStopAfter.UseUnderline = true;
			this.rbStopAfter.Group = this.rbRepeateContinously.Group;
			this.table4.Add (this.rbStopAfter);
			global::Gtk.Table.TableChild w6 = ((global::Gtk.Table.TableChild)(this.table4 [this.rbStopAfter]));
			w6.TopAttach = ((uint)(1));
			w6.BottomAttach = ((uint)(2));
			w6.XOptions = ((global::Gtk.AttachOptions)(4));
			w6.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table4.Gtk.Table+TableChild
			this.sbRadioBtnStopAfter = new global::Gtk.SpinButton (1, 100, 1);
			this.sbRadioBtnStopAfter.CanFocus = true;
			this.sbRadioBtnStopAfter.Name = "sbRadioBtnStopAfter";
			this.sbRadioBtnStopAfter.Adjustment.PageIncrement = 10;
			this.sbRadioBtnStopAfter.ClimbRate = 1;
			this.sbRadioBtnStopAfter.Numeric = true;
			this.sbRadioBtnStopAfter.Value = 1;
			this.table4.Add (this.sbRadioBtnStopAfter);
			global::Gtk.Table.TableChild w7 = ((global::Gtk.Table.TableChild)(this.table4 [this.sbRadioBtnStopAfter]));
			w7.TopAttach = ((uint)(1));
			w7.BottomAttach = ((uint)(2));
			w7.LeftAttach = ((uint)(1));
			w7.RightAttach = ((uint)(2));
			w7.XOptions = ((global::Gtk.AttachOptions)(4));
			w7.YOptions = ((global::Gtk.AttachOptions)(4));
			this.GtkAlignment4.Add (this.table4);
			this.frame1.Add (this.GtkAlignment4);
			this.GtkLabel2 = new global::Gtk.Label ();
			this.GtkLabel2.Name = "GtkLabel2";
			this.GtkLabel2.Xalign = 1F;
			this.GtkLabel2.LabelProp = global::Mono.Unix.Catalog.GetString ("End of Sequence:");
			this.GtkLabel2.UseMarkup = true;
			this.frame1.LabelWidget = this.GtkLabel2;
			this.table3.Add (this.frame1);
			global::Gtk.Table.TableChild w10 = ((global::Gtk.Table.TableChild)(this.table3 [this.frame1]));
			w10.TopAttach = ((uint)(3));
			w10.BottomAttach = ((uint)(6));
			w10.RightAttach = ((uint)(2));
			w10.XOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table3.Gtk.Table+TableChild
			this.hseparator2 = new global::Gtk.HSeparator ();
			this.hseparator2.Name = "hseparator2";
			this.table3.Add (this.hseparator2);
			global::Gtk.Table.TableChild w11 = ((global::Gtk.Table.TableChild)(this.table3 [this.hseparator2]));
			w11.TopAttach = ((uint)(2));
			w11.BottomAttach = ((uint)(3));
			w11.RightAttach = ((uint)(2));
			w11.XOptions = ((global::Gtk.AttachOptions)(4));
			w11.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table3.Gtk.Table+TableChild
			this.label2 = new global::Gtk.Label ();
			this.label2.Name = "label2";
			this.label2.Xalign = 0F;
			this.label2.LabelProp = global::Mono.Unix.Catalog.GetString ("Name:");
			this.table3.Add (this.label2);
			global::Gtk.Table.TableChild w12 = ((global::Gtk.Table.TableChild)(this.table3 [this.label2]));
			w12.XOptions = ((global::Gtk.AttachOptions)(4));
			w12.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table3.Gtk.Table+TableChild
			this.label3 = new global::Gtk.Label ();
			this.label3.Name = "label3";
			this.label3.Xalign = 0F;
			this.label3.LabelProp = global::Mono.Unix.Catalog.GetString ("Digital Pin:");
			this.table3.Add (this.label3);
			global::Gtk.Table.TableChild w13 = ((global::Gtk.Table.TableChild)(this.table3 [this.label3]));
			w13.TopAttach = ((uint)(1));
			w13.BottomAttach = ((uint)(2));
			w13.XOptions = ((global::Gtk.AttachOptions)(4));
			w13.YOptions = ((global::Gtk.AttachOptions)(4));
			this.hbox2.Add (this.table3);
			global::Gtk.Box.BoxChild w14 = ((global::Gtk.Box.BoxChild)(this.hbox2 [this.table3]));
			w14.Position = 0;
			w14.Expand = false;
			w14.Fill = false;
			// Container child hbox2.Gtk.Box+BoxChild
			this.vseparator1 = new global::Gtk.VSeparator ();
			this.vseparator1.Name = "vseparator1";
			this.hbox2.Add (this.vseparator1);
			global::Gtk.Box.BoxChild w15 = ((global::Gtk.Box.BoxChild)(this.hbox2 [this.vseparator1]));
			w15.Position = 1;
			w15.Expand = false;
			w15.Fill = false;
			// Container child hbox2.Gtk.Box+BoxChild
			this.vboxOptions = new global::Gtk.VBox ();
			this.vboxOptions.Name = "vboxOptions";
			this.vboxOptions.Spacing = 6;
			// Container child vboxOptions.Gtk.Box+BoxChild
			this.GtkScrolledWindow = new global::Gtk.ScrolledWindow ();
			this.GtkScrolledWindow.HeightRequest = 180;
			this.GtkScrolledWindow.Name = "GtkScrolledWindow";
			this.GtkScrolledWindow.ShadowType = ((global::Gtk.ShadowType)(1));
			// Container child GtkScrolledWindow.Gtk.Container+ContainerChild
			this.nvSequenceOptions = new global::Gtk.NodeView ();
			this.nvSequenceOptions.CanFocus = true;
			this.nvSequenceOptions.Name = "nvSequenceOptions";
			this.GtkScrolledWindow.Add (this.nvSequenceOptions);
			this.vboxOptions.Add (this.GtkScrolledWindow);
			global::Gtk.Box.BoxChild w17 = ((global::Gtk.Box.BoxChild)(this.vboxOptions [this.GtkScrolledWindow]));
			w17.Position = 0;
			// Container child vboxOptions.Gtk.Box+BoxChild
			this.frame2 = new global::Gtk.Frame ();
			this.frame2.Name = "frame2";
			this.frame2.ShadowType = ((global::Gtk.ShadowType)(1));
			// Container child frame2.Gtk.Container+ContainerChild
			this.GtkAlignment2 = new global::Gtk.Alignment (0F, 0F, 1F, 1F);
			this.GtkAlignment2.Name = "GtkAlignment2";
			this.GtkAlignment2.LeftPadding = ((uint)(12));
			// Container child GtkAlignment2.Gtk.Container+ContainerChild
			this.table5 = new global::Gtk.Table (((uint)(2)), ((uint)(3)), false);
			this.table5.Name = "table5";
			this.table5.RowSpacing = ((uint)(6));
			this.table5.ColumnSpacing = ((uint)(10));
			// Container child table5.Gtk.Table+TableChild
			this.btnApplyOperation = new global::Gtk.Button ();
			this.btnApplyOperation.CanFocus = true;
			this.btnApplyOperation.Name = "btnApplyOperation";
			this.btnApplyOperation.UseUnderline = true;
			this.btnApplyOperation.Label = global::Mono.Unix.Catalog.GetString ("Apply");
			global::Gtk.Image w18 = new global::Gtk.Image ();
			w18.Pixbuf = global::Stetic.IconLoader.LoadIcon (this, "gtk-apply", global::Gtk.IconSize.Menu);
			this.btnApplyOperation.Image = w18;
			this.table5.Add (this.btnApplyOperation);
			global::Gtk.Table.TableChild w19 = ((global::Gtk.Table.TableChild)(this.table5 [this.btnApplyOperation]));
			w19.LeftAttach = ((uint)(2));
			w19.RightAttach = ((uint)(3));
			w19.XOptions = ((global::Gtk.AttachOptions)(4));
			w19.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table5.Gtk.Table+TableChild
			this.btnRemoveOperation = new global::Gtk.Button ();
			this.btnRemoveOperation.Sensitive = false;
			this.btnRemoveOperation.CanFocus = true;
			this.btnRemoveOperation.Name = "btnRemoveOperation";
			this.btnRemoveOperation.UseUnderline = true;
			this.btnRemoveOperation.Label = global::Mono.Unix.Catalog.GetString ("Remove");
			global::Gtk.Image w20 = new global::Gtk.Image ();
			w20.Pixbuf = global::Stetic.IconLoader.LoadIcon (this, "gtk-remove", global::Gtk.IconSize.Menu);
			this.btnRemoveOperation.Image = w20;
			this.table5.Add (this.btnRemoveOperation);
			global::Gtk.Table.TableChild w21 = ((global::Gtk.Table.TableChild)(this.table5 [this.btnRemoveOperation]));
			w21.TopAttach = ((uint)(1));
			w21.BottomAttach = ((uint)(2));
			w21.LeftAttach = ((uint)(2));
			w21.RightAttach = ((uint)(3));
			w21.XOptions = ((global::Gtk.AttachOptions)(4));
			w21.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table5.Gtk.Table+TableChild
			this.cbState = global::Gtk.ComboBox.NewText ();
			this.cbState.AppendText (global::Mono.Unix.Catalog.GetString ("HIGH"));
			this.cbState.AppendText (global::Mono.Unix.Catalog.GetString ("LOW"));
			this.cbState.Name = "cbState";
			this.cbState.Active = 0;
			this.table5.Add (this.cbState);
			global::Gtk.Table.TableChild w22 = ((global::Gtk.Table.TableChild)(this.table5 [this.cbState]));
			w22.TopAttach = ((uint)(1));
			w22.BottomAttach = ((uint)(2));
			w22.LeftAttach = ((uint)(1));
			w22.RightAttach = ((uint)(2));
			w22.XOptions = ((global::Gtk.AttachOptions)(4));
			w22.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table5.Gtk.Table+TableChild
			this.label7 = new global::Gtk.Label ();
			this.label7.Name = "label7";
			this.label7.Xalign = 0F;
			this.label7.LabelProp = global::Mono.Unix.Catalog.GetString ("State:");
			this.table5.Add (this.label7);
			global::Gtk.Table.TableChild w23 = ((global::Gtk.Table.TableChild)(this.table5 [this.label7]));
			w23.TopAttach = ((uint)(1));
			w23.BottomAttach = ((uint)(2));
			w23.XOptions = ((global::Gtk.AttachOptions)(4));
			w23.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table5.Gtk.Table+TableChild
			this.label8 = new global::Gtk.Label ();
			this.label8.Name = "label8";
			this.label8.Xalign = 0F;
			this.label8.LabelProp = global::Mono.Unix.Catalog.GetString ("Duration:");
			this.table5.Add (this.label8);
			global::Gtk.Table.TableChild w24 = ((global::Gtk.Table.TableChild)(this.table5 [this.label8]));
			w24.XOptions = ((global::Gtk.AttachOptions)(4));
			w24.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table5.Gtk.Table+TableChild
			this.table2 = new global::Gtk.Table (((uint)(1)), ((uint)(10)), false);
			this.table2.Name = "table2";
			this.table2.RowSpacing = ((uint)(6));
			this.table2.ColumnSpacing = ((uint)(6));
			// Container child table2.Gtk.Table+TableChild
			this.label4 = new global::Gtk.Label ();
			this.label4.Name = "label4";
			this.label4.LabelProp = global::Mono.Unix.Catalog.GetString ("Minutes:");
			this.table2.Add (this.label4);
			global::Gtk.Table.TableChild w25 = ((global::Gtk.Table.TableChild)(this.table2 [this.label4]));
			w25.LeftAttach = ((uint)(4));
			w25.RightAttach = ((uint)(5));
			w25.XOptions = ((global::Gtk.AttachOptions)(4));
			w25.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.label6 = new global::Gtk.Label ();
			this.label6.Name = "label6";
			this.label6.LabelProp = global::Mono.Unix.Catalog.GetString ("Days:");
			this.table2.Add (this.label6);
			global::Gtk.Table.TableChild w26 = ((global::Gtk.Table.TableChild)(this.table2 [this.label6]));
			w26.XOptions = ((global::Gtk.AttachOptions)(4));
			w26.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.lblHours = new global::Gtk.Label ();
			this.lblHours.Name = "lblHours";
			this.lblHours.LabelProp = global::Mono.Unix.Catalog.GetString ("Hours:");
			this.table2.Add (this.lblHours);
			global::Gtk.Table.TableChild w27 = ((global::Gtk.Table.TableChild)(this.table2 [this.lblHours]));
			w27.LeftAttach = ((uint)(2));
			w27.RightAttach = ((uint)(3));
			w27.XOptions = ((global::Gtk.AttachOptions)(4));
			w27.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.lblMilliSec = new global::Gtk.Label ();
			this.lblMilliSec.Name = "lblMilliSec";
			this.lblMilliSec.LabelProp = global::Mono.Unix.Catalog.GetString ("Millisec.:");
			this.table2.Add (this.lblMilliSec);
			global::Gtk.Table.TableChild w28 = ((global::Gtk.Table.TableChild)(this.table2 [this.lblMilliSec]));
			w28.LeftAttach = ((uint)(8));
			w28.RightAttach = ((uint)(9));
			w28.XOptions = ((global::Gtk.AttachOptions)(4));
			w28.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.lblSeconds = new global::Gtk.Label ();
			this.lblSeconds.Name = "lblSeconds";
			this.lblSeconds.LabelProp = global::Mono.Unix.Catalog.GetString ("Seconds:");
			this.table2.Add (this.lblSeconds);
			global::Gtk.Table.TableChild w29 = ((global::Gtk.Table.TableChild)(this.table2 [this.lblSeconds]));
			w29.LeftAttach = ((uint)(6));
			w29.RightAttach = ((uint)(7));
			w29.XOptions = ((global::Gtk.AttachOptions)(4));
			w29.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.sbDays = new global::Gtk.SpinButton (0, 111111, 1);
			this.sbDays.CanFocus = true;
			this.sbDays.Name = "sbDays";
			this.sbDays.Adjustment.PageIncrement = 10;
			this.sbDays.ClimbRate = 1;
			this.sbDays.Numeric = true;
			this.table2.Add (this.sbDays);
			global::Gtk.Table.TableChild w30 = ((global::Gtk.Table.TableChild)(this.table2 [this.sbDays]));
			w30.LeftAttach = ((uint)(1));
			w30.RightAttach = ((uint)(2));
			w30.XOptions = ((global::Gtk.AttachOptions)(4));
			w30.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.sbHours = new global::Gtk.SpinButton (0, 24, 1);
			this.sbHours.CanFocus = true;
			this.sbHours.Name = "sbHours";
			this.sbHours.Adjustment.PageIncrement = 10;
			this.sbHours.ClimbRate = 1;
			this.sbHours.Numeric = true;
			this.table2.Add (this.sbHours);
			global::Gtk.Table.TableChild w31 = ((global::Gtk.Table.TableChild)(this.table2 [this.sbHours]));
			w31.LeftAttach = ((uint)(3));
			w31.RightAttach = ((uint)(4));
			w31.XOptions = ((global::Gtk.AttachOptions)(4));
			w31.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.sbMilliSec = new global::Gtk.SpinButton (0, 1000, 1);
			this.sbMilliSec.CanFocus = true;
			this.sbMilliSec.Name = "sbMilliSec";
			this.sbMilliSec.Adjustment.PageIncrement = 10;
			this.sbMilliSec.ClimbRate = 1;
			this.sbMilliSec.Numeric = true;
			this.table2.Add (this.sbMilliSec);
			global::Gtk.Table.TableChild w32 = ((global::Gtk.Table.TableChild)(this.table2 [this.sbMilliSec]));
			w32.LeftAttach = ((uint)(9));
			w32.RightAttach = ((uint)(10));
			w32.XOptions = ((global::Gtk.AttachOptions)(4));
			w32.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.sbMinutes = new global::Gtk.SpinButton (0, 60, 1);
			this.sbMinutes.CanFocus = true;
			this.sbMinutes.Name = "sbMinutes";
			this.sbMinutes.Adjustment.PageIncrement = 10;
			this.sbMinutes.ClimbRate = 1;
			this.sbMinutes.Numeric = true;
			this.table2.Add (this.sbMinutes);
			global::Gtk.Table.TableChild w33 = ((global::Gtk.Table.TableChild)(this.table2 [this.sbMinutes]));
			w33.LeftAttach = ((uint)(5));
			w33.RightAttach = ((uint)(6));
			w33.XOptions = ((global::Gtk.AttachOptions)(4));
			w33.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.sbSeconds = new global::Gtk.SpinButton (0, 60, 1);
			this.sbSeconds.CanFocus = true;
			this.sbSeconds.Name = "sbSeconds";
			this.sbSeconds.Adjustment.PageIncrement = 10;
			this.sbSeconds.ClimbRate = 1;
			this.sbSeconds.Numeric = true;
			this.table2.Add (this.sbSeconds);
			global::Gtk.Table.TableChild w34 = ((global::Gtk.Table.TableChild)(this.table2 [this.sbSeconds]));
			w34.LeftAttach = ((uint)(7));
			w34.RightAttach = ((uint)(8));
			w34.XOptions = ((global::Gtk.AttachOptions)(4));
			w34.YOptions = ((global::Gtk.AttachOptions)(4));
			this.table5.Add (this.table2);
			global::Gtk.Table.TableChild w35 = ((global::Gtk.Table.TableChild)(this.table5 [this.table2]));
			w35.LeftAttach = ((uint)(1));
			w35.RightAttach = ((uint)(2));
			w35.XOptions = ((global::Gtk.AttachOptions)(4));
			w35.YOptions = ((global::Gtk.AttachOptions)(4));
			this.GtkAlignment2.Add (this.table5);
			this.frame2.Add (this.GtkAlignment2);
			this.GtkLabel5 = new global::Gtk.Label ();
			this.GtkLabel5.Name = "GtkLabel5";
			this.GtkLabel5.LabelProp = global::Mono.Unix.Catalog.GetString ("Add/Modify Operation");
			this.frame2.LabelWidget = this.GtkLabel5;
			this.vboxOptions.Add (this.frame2);
			global::Gtk.Box.BoxChild w38 = ((global::Gtk.Box.BoxChild)(this.vboxOptions [this.frame2]));
			w38.Position = 1;
			w38.Expand = false;
			w38.Fill = false;
			// Container child vboxOptions.Gtk.Box+BoxChild
			this.hseparator1 = new global::Gtk.HSeparator ();
			this.hseparator1.Name = "hseparator1";
			this.vboxOptions.Add (this.hseparator1);
			global::Gtk.Box.BoxChild w39 = ((global::Gtk.Box.BoxChild)(this.vboxOptions [this.hseparator1]));
			w39.Position = 2;
			w39.Expand = false;
			w39.Fill = false;
			this.hbox2.Add (this.vboxOptions);
			global::Gtk.Box.BoxChild w40 = ((global::Gtk.Box.BoxChild)(this.hbox2 [this.vboxOptions]));
			w40.Position = 2;
			w1.Add (this.hbox2);
			global::Gtk.Box.BoxChild w41 = ((global::Gtk.Box.BoxChild)(w1 [this.hbox2]));
			w41.Position = 0;
			// Internal child SequenceConfigurationsDialog.SequenceConfiguration.ActionArea
			global::Gtk.HButtonBox w42 = this.ActionArea;
			w42.Name = "dialog1_ActionArea";
			w42.Spacing = 10;
			w42.BorderWidth = ((uint)(5));
			w42.LayoutStyle = ((global::Gtk.ButtonBoxStyle)(4));
			// Container child dialog1_ActionArea.Gtk.ButtonBox+ButtonBoxChild
			this.buttonCancel = new global::Gtk.Button ();
			this.buttonCancel.CanDefault = true;
			this.buttonCancel.CanFocus = true;
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.UseStock = true;
			this.buttonCancel.UseUnderline = true;
			this.buttonCancel.Label = "gtk-cancel";
			this.AddActionWidget (this.buttonCancel, -6);
			global::Gtk.ButtonBox.ButtonBoxChild w43 = ((global::Gtk.ButtonBox.ButtonBoxChild)(w42 [this.buttonCancel]));
			w43.Expand = false;
			w43.Fill = false;
			// Container child dialog1_ActionArea.Gtk.ButtonBox+ButtonBoxChild
			this.buttonOk = new global::Gtk.Button ();
			this.buttonOk.CanDefault = true;
			this.buttonOk.CanFocus = true;
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.UseStock = true;
			this.buttonOk.UseUnderline = true;
			this.buttonOk.Label = "gtk-ok";
			this.AddActionWidget (this.buttonOk, -5);
			global::Gtk.ButtonBox.ButtonBoxChild w44 = ((global::Gtk.ButtonBox.ButtonBoxChild)(w42 [this.buttonOk]));
			w44.Position = 1;
			w44.Expand = false;
			w44.Fill = false;
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.DefaultWidth = 996;
			this.DefaultHeight = 598;
			this.Show ();
			this.cbPin.Changed += new global::System.EventHandler (this.OnCbPinChanged);
			this.btnRemoveOperation.Clicked += new global::System.EventHandler (this.OnBtnRemoveOperationClicked);
			this.btnApplyOperation.Clicked += new global::System.EventHandler (this.OnBtnApplyOperationClicked);
			this.buttonOk.Clicked += new global::System.EventHandler (this.OnButtonOkClicked);
		}
	}
}
