
// This file has been generated by the GUI designer. Do not modify.
namespace Prototype
{
	public partial class SignalPinConfigBox
	{
		private global::Gtk.Frame frame1;
		
		private global::Gtk.Alignment GtkAlignment;
		
		private global::Gtk.Table table1;
		
		private global::Gtk.Button btnConfirm;
		
		private global::Gtk.CheckButton cbSignalPinUseAsInput;
		
		private global::Gtk.Entry entrySignalPinName;
		
		private global::Gtk.ComboBoxEntry entryUnit;
		
		private global::Gtk.Label lblMultiplier;
		
		private global::Gtk.Label lblOffset;
		
		private global::Gtk.Label lblSignalPinName;
		
		private global::Gtk.Label lblUnit;
		
		private global::Gtk.SpinButton spMultiplier;
		
		private global::Gtk.SpinButton spOffset;
		
		private global::Gtk.VSeparator vseparator1;
		
		private global::Gtk.Label lblFrame;

		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget Prototype.SignalPinConfigBox
			global::Stetic.BinContainer.Attach (this);
			this.Name = "Prototype.SignalPinConfigBox";
			// Container child Prototype.SignalPinConfigBox.Gtk.Container+ContainerChild
			this.frame1 = new global::Gtk.Frame ();
			this.frame1.Name = "frame1";
			this.frame1.ShadowType = ((global::Gtk.ShadowType)(1));
			// Container child frame1.Gtk.Container+ContainerChild
			this.GtkAlignment = new global::Gtk.Alignment (0F, 0F, 1F, 1F);
			this.GtkAlignment.Name = "GtkAlignment";
			this.GtkAlignment.LeftPadding = ((uint)(12));
			// Container child GtkAlignment.Gtk.Container+ContainerChild
			this.table1 = new global::Gtk.Table (((uint)(6)), ((uint)(4)), false);
			this.table1.Name = "table1";
			this.table1.RowSpacing = ((uint)(6));
			this.table1.ColumnSpacing = ((uint)(6));
			// Container child table1.Gtk.Table+TableChild
			this.btnConfirm = new global::Gtk.Button ();
			this.btnConfirm.Sensitive = false;
			this.btnConfirm.CanFocus = true;
			this.btnConfirm.Name = "btnConfirm";
			this.btnConfirm.UseUnderline = true;
			this.btnConfirm.Label = global::Mono.Unix.Catalog.GetString ("Confirm");
			this.table1.Add (this.btnConfirm);
			global::Gtk.Table.TableChild w1 = ((global::Gtk.Table.TableChild)(this.table1 [this.btnConfirm]));
			w1.TopAttach = ((uint)(5));
			w1.BottomAttach = ((uint)(6));
			w1.RightAttach = ((uint)(4));
			w1.XOptions = ((global::Gtk.AttachOptions)(4));
			w1.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.cbSignalPinUseAsInput = new global::Gtk.CheckButton ();
			this.cbSignalPinUseAsInput.CanFocus = true;
			this.cbSignalPinUseAsInput.Name = "cbSignalPinUseAsInput";
			this.cbSignalPinUseAsInput.Label = global::Mono.Unix.Catalog.GetString ("Use this Pin as Input");
			this.cbSignalPinUseAsInput.DrawIndicator = true;
			this.cbSignalPinUseAsInput.UseUnderline = true;
			this.table1.Add (this.cbSignalPinUseAsInput);
			global::Gtk.Table.TableChild w2 = ((global::Gtk.Table.TableChild)(this.table1 [this.cbSignalPinUseAsInput]));
			w2.RightAttach = ((uint)(2));
			w2.XOptions = ((global::Gtk.AttachOptions)(4));
			w2.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.entrySignalPinName = new global::Gtk.Entry ();
			this.entrySignalPinName.Sensitive = false;
			this.entrySignalPinName.CanFocus = true;
			this.entrySignalPinName.Name = "entrySignalPinName";
			this.entrySignalPinName.IsEditable = true;
			this.entrySignalPinName.InvisibleChar = '●';
			this.table1.Add (this.entrySignalPinName);
			global::Gtk.Table.TableChild w3 = ((global::Gtk.Table.TableChild)(this.table1 [this.entrySignalPinName]));
			w3.TopAttach = ((uint)(1));
			w3.BottomAttach = ((uint)(2));
			w3.LeftAttach = ((uint)(1));
			w3.RightAttach = ((uint)(2));
			w3.XOptions = ((global::Gtk.AttachOptions)(4));
			w3.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.entryUnit = global::Gtk.ComboBoxEntry.NewText ();
			this.entryUnit.AppendText (global::Mono.Unix.Catalog.GetString ("C"));
			this.entryUnit.AppendText (global::Mono.Unix.Catalog.GetString ("F"));
			this.entryUnit.AppendText (global::Mono.Unix.Catalog.GetString ("K"));
			this.entryUnit.AppendText (global::Mono.Unix.Catalog.GetString ("V"));
			this.entryUnit.AppendText (global::Mono.Unix.Catalog.GetString ("A"));
			this.entryUnit.AppendText (global::Mono.Unix.Catalog.GetString ("Pa"));
			this.entryUnit.Sensitive = false;
			this.entryUnit.Name = "entryUnit";
			this.table1.Add (this.entryUnit);
			global::Gtk.Table.TableChild w4 = ((global::Gtk.Table.TableChild)(this.table1 [this.entryUnit]));
			w4.TopAttach = ((uint)(4));
			w4.BottomAttach = ((uint)(5));
			w4.LeftAttach = ((uint)(1));
			w4.RightAttach = ((uint)(2));
			w4.XOptions = ((global::Gtk.AttachOptions)(4));
			w4.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.lblMultiplier = new global::Gtk.Label ();
			this.lblMultiplier.Name = "lblMultiplier";
			this.lblMultiplier.Xalign = 0F;
			this.lblMultiplier.LabelProp = global::Mono.Unix.Catalog.GetString ("Multiplier:");
			this.table1.Add (this.lblMultiplier);
			global::Gtk.Table.TableChild w5 = ((global::Gtk.Table.TableChild)(this.table1 [this.lblMultiplier]));
			w5.TopAttach = ((uint)(2));
			w5.BottomAttach = ((uint)(3));
			w5.XOptions = ((global::Gtk.AttachOptions)(4));
			w5.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.lblOffset = new global::Gtk.Label ();
			this.lblOffset.Name = "lblOffset";
			this.lblOffset.Xalign = 0F;
			this.lblOffset.LabelProp = global::Mono.Unix.Catalog.GetString ("Offset:");
			this.table1.Add (this.lblOffset);
			global::Gtk.Table.TableChild w6 = ((global::Gtk.Table.TableChild)(this.table1 [this.lblOffset]));
			w6.TopAttach = ((uint)(3));
			w6.BottomAttach = ((uint)(4));
			w6.XOptions = ((global::Gtk.AttachOptions)(4));
			w6.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.lblSignalPinName = new global::Gtk.Label ();
			this.lblSignalPinName.Name = "lblSignalPinName";
			this.lblSignalPinName.LabelProp = global::Mono.Unix.Catalog.GetString ("Signal Label:");
			this.table1.Add (this.lblSignalPinName);
			global::Gtk.Table.TableChild w7 = ((global::Gtk.Table.TableChild)(this.table1 [this.lblSignalPinName]));
			w7.TopAttach = ((uint)(1));
			w7.BottomAttach = ((uint)(2));
			w7.XOptions = ((global::Gtk.AttachOptions)(4));
			w7.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.lblUnit = new global::Gtk.Label ();
			this.lblUnit.Name = "lblUnit";
			this.lblUnit.Xalign = 0F;
			this.lblUnit.LabelProp = global::Mono.Unix.Catalog.GetString ("Unit:");
			this.table1.Add (this.lblUnit);
			global::Gtk.Table.TableChild w8 = ((global::Gtk.Table.TableChild)(this.table1 [this.lblUnit]));
			w8.TopAttach = ((uint)(4));
			w8.BottomAttach = ((uint)(5));
			w8.XOptions = ((global::Gtk.AttachOptions)(4));
			w8.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.spMultiplier = new global::Gtk.SpinButton (-9999, 9999, 1);
			this.spMultiplier.Sensitive = false;
			this.spMultiplier.CanFocus = true;
			this.spMultiplier.Name = "spMultiplier";
			this.spMultiplier.Adjustment.PageIncrement = 10;
			this.spMultiplier.ClimbRate = 1;
			this.spMultiplier.Numeric = true;
			this.spMultiplier.Value = 1;
			this.table1.Add (this.spMultiplier);
			global::Gtk.Table.TableChild w9 = ((global::Gtk.Table.TableChild)(this.table1 [this.spMultiplier]));
			w9.TopAttach = ((uint)(2));
			w9.BottomAttach = ((uint)(3));
			w9.LeftAttach = ((uint)(1));
			w9.RightAttach = ((uint)(2));
			w9.XOptions = ((global::Gtk.AttachOptions)(4));
			w9.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.spOffset = new global::Gtk.SpinButton (-9999, 9999, 1);
			this.spOffset.Sensitive = false;
			this.spOffset.CanFocus = true;
			this.spOffset.Name = "spOffset";
			this.spOffset.Adjustment.PageIncrement = 10;
			this.spOffset.ClimbRate = 1;
			this.spOffset.Numeric = true;
			this.table1.Add (this.spOffset);
			global::Gtk.Table.TableChild w10 = ((global::Gtk.Table.TableChild)(this.table1 [this.spOffset]));
			w10.TopAttach = ((uint)(3));
			w10.BottomAttach = ((uint)(4));
			w10.LeftAttach = ((uint)(1));
			w10.RightAttach = ((uint)(2));
			w10.XOptions = ((global::Gtk.AttachOptions)(4));
			w10.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.vseparator1 = new global::Gtk.VSeparator ();
			this.vseparator1.Name = "vseparator1";
			this.table1.Add (this.vseparator1);
			global::Gtk.Table.TableChild w11 = ((global::Gtk.Table.TableChild)(this.table1 [this.vseparator1]));
			w11.BottomAttach = ((uint)(5));
			w11.LeftAttach = ((uint)(2));
			w11.RightAttach = ((uint)(3));
			w11.XOptions = ((global::Gtk.AttachOptions)(4));
			w11.YOptions = ((global::Gtk.AttachOptions)(4));
			this.GtkAlignment.Add (this.table1);
			this.frame1.Add (this.GtkAlignment);
			this.lblFrame = new global::Gtk.Label ();
			this.lblFrame.Name = "lblFrame";
			this.lblFrame.Xalign = 0F;
			this.lblFrame.LabelProp = global::Mono.Unix.Catalog.GetString ("<b>GtkFrame</b>");
			this.lblFrame.UseMarkup = true;
			this.frame1.LabelWidget = this.lblFrame;
			this.Add (this.frame1);
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.Hide ();
			this.entrySignalPinName.Changed += new global::System.EventHandler (this.OnEntrySignalPinNameChanged);
			this.cbSignalPinUseAsInput.Toggled += new global::System.EventHandler (this.OnCbSignalPinUseAsInputToggled);
		}
	}
}
