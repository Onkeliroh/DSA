using System;
using System.Collections.Generic;
using PrototypeBackend;
using Gtk;
using Gdk;
using System.Linq;

namespace MCUWidget
{
	[System.ComponentModel.ToolboxItem (true)]
	public partial class MCUWidget : Gtk.Bin
	{
		public string MCUImagepath{ get { return (SelectedBoard != null) ? SelectedBoard.ImageFilePath : null; } private set { } }

		public Board[] Boards {
			get { return _Boards; }
			set {
				_Boards = value;
				var store = new ListStore (typeof(string));
				foreach (Board b in _Boards) {
					store.AppendValues (new object[]{ b.Name });
				}
				cbBoardType.Model = store;
				cbBoardType.Show ();

//				if (OnBoardSelected != null) {
//					OnBoardSelected.Invoke (this, null);
//				}
			}
		}

		public Board SelectedBoard { get { return (cbBoardType.Active != -1) ? _Boards [cbBoardType.Active] : null; } private set { } }

		public int SelectedARef;

		public int LastActive = -1;

		private Board[] _Boards;

		public ListStore AREFTypes { get; set; }

		public EventHandler<BoardSelectionArgs> OnBoardSelected;

		public MCUWidget ()
		{
			this.Build ();
			SelectedBoard = null;

			this.drawingarea1.ExposeEvent += Draw;
		}

		public void Select (string mcu)
		{
			for (int i = 0; i < _Boards.Length; i++) {
				if (_Boards [i].MCU != "") {
					if (_Boards [i].MCU.ToLower () == mcu.ToLower ()) {
						cbBoardType.Active = i;
						break;
					}
				}
			}
		}

		public void SelectAREF (string AREFType)
		{
			if (SelectedBoard.AnalogReferences.ContainsKey (AREFType)) {
				SelectedBoard.AnalogReferenceVoltageType = AREFType;
				cbAREF.Active = SelectedBoard.AnalogReferences.Keys.ToList ().IndexOf (AREFType);
			}
		}

		public void SetAREF (double AREF)
		{
			SelectedBoard.AnalogReferenceVoltage = AREF;
			sbAREFExternal.Value = AREF;
		}

		#region Drawing

		void Draw (object o, ExposeEventArgs args)
		{
			var context = CairoHelper.Create (this.drawingarea1.GdkWindow);
			context.SetSource (Compose ());
			context.Paint ();
			var MCUImage = MCUSurface ();
			context.SetSource (
				MCUImage,
				this.drawingarea1.Allocation.Width / 2 - MCUImage.Width / 2,
				this.drawingarea1.Allocation.Height / 3 - MCUImage.Height / 2
			);
			context.Paint ();
			SetSizeRequest (MCUImage.Width, MCUImage.Height + 200);
		}

		private Cairo.ImageSurface  Compose (params Pixbuf[] Bufs)
		{
			var surf = new Cairo.ImageSurface (Cairo.Format.Argb32, 100, 100);
//			var context = new Cairo.Context (surf);
//			context.SetSourceColor (new Cairo.Color (255, 0, 0));
//			context.Rectangle (new Cairo.Rectangle (0, 0, 50, 50));
//			context.Fill ();

			return surf;
		}

		protected Cairo.ImageSurface MCUSurface ()
		{
//			#if !WIN
			if (MCUImagepath != null && System.IO.File.Exists (MCUImagepath)) {
				if (!MCUImagepath.Equals (string.Empty)) {
					try {
						var MCUImage = new Rsvg.Handle (MCUImagepath);
						var buf = MCUImage.Pixbuf;
						var surf = new Cairo.ImageSurface (Cairo.Format.Argb32, buf.Width, buf.Height);
						var context = new Cairo.Context (surf);

						MCUImage.RenderCairo (context);
						return surf;
					} catch (Exception ex) {
						Console.Error.WriteLine (ex);
					}
				}
			}
//			#endif
			return new Cairo.ImageSurface (Cairo.Format.Argb32, 0, 0);

		}

		private Cairo.ImageSurface MCULabelLeft ()
		{
			return new Cairo.ImageSurface (Cairo.Format.ARGB32, 0, 0);
		}

		#endregion

		protected void OnCbBoardTypeChanged (object sender, EventArgs e)
		{
			//TODO englisch prüfen
			if (LastActive != cbBoardType.Active && LastActive != -1) {
				//TODO auf unterschied prüfen. sonst ignorieren
				var dialog = new MessageDialog (this.Toplevel as Gtk.Window, DialogFlags.Modal, MessageType.Info, ButtonsType.YesNo,
					             "The Board Type was changed. If you procede parts of your configuration could get lost, due to incompatibility with the new Board Type.\n Do you wish to procede?");
				dialog.Response += (o, args) => {
					if (args.ResponseId == ResponseType.Yes) {
						LastActive = cbBoardType.Active;
						UpdateARef ();
					} else {
						cbBoardType.Active = LastActive;
					}
				};
				dialog.Run ();
				dialog.Destroy ();
			} else {
				LastActive = cbBoardType.Active;
				UpdateARef ();
			}

			drawingarea1.QueueDraw ();

			if (OnBoardSelected != null) {
				OnBoardSelected.Invoke (this, new BoardSelectionArgs (SelectedBoard));
			}
		}

		private void UpdateARef ()
		{
			if (SelectedBoard != null) {
				var store = new ListStore (typeof(string));

				foreach (string key in SelectedBoard.AnalogReferences.Keys) {
					store.AppendValues (new object[]{ key });
				}

				cbAREF.Model = store;

				if (SelectedBoard.AnalogReferenceVoltage != -1 && SelectedBoard.AnalogReferences.ContainsValue (SelectedBoard.AnalogReferenceVoltage)) {
					int index = SelectedBoard.AnalogReferences.Values.ToList ().IndexOf (SelectedBoard.AnalogReferenceVoltage);

					cbAREF.Active = index;
				}

				cbAREF.Show ();
			}
		}

		protected void OnCbAREFChanged (object sender, EventArgs e)
		{
			if (cbAREF.ActiveText == "EXTERNAL") {
				sbAREFExternal.Sensitive = true;
			} else {
				sbAREFExternal.Sensitive = false;
			}
			if (SelectedBoard != null) {
				if (!sbAREFExternal.Sensitive) {
					SelectedBoard.AnalogReferenceVoltage = SelectedBoard.AnalogReferences.ElementAt (cbAREF.Active).Value;
					sbAREFExternal.Value = SelectedBoard.AnalogReferenceVoltage;
				} else {
					SelectedBoard.AnalogReferenceVoltage = sbAREFExternal.Value;
				}
			}

			if (OnBoardSelected != null) {
				OnBoardSelected.Invoke (
					this,
					new BoardSelectionArgs (SelectedBoard, SelectedBoard.AnalogReferenceVoltage, cbAREF.ActiveText)
				);
			}
		}
	}
}
