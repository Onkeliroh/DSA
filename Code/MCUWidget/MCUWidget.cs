using System;
using System.Collections.Generic;
using PrototypeBackend;
using Gtk;
using Gdk;

namespace MCUWidget
{
	[System.ComponentModel.ToolboxItem (true)]
	public partial class MCUWidget : Gtk.Bin
	{
		public string MCUImagepath{ get; set; }

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

				if (OnBoardSelected != null) {
					OnBoardSelected.Invoke (this, null);
				}
			}
		}

		public Board SelectedBoard;
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
			if (SelectedBoard != null) {
				var dialog = new MessageDialog (this.Toplevel as Gtk.Window, DialogFlags.Modal, MessageType.Info, ButtonsType.YesNo,
					             "The Board Type was changed. If you procede parts of your configuration could get lost, due to incompatibility with the new Board Type.\n Do you wish to procede?");
				dialog.Response += (o, args) => {
					if (args.ResponseId == ResponseType.Yes) {
						MCUImagepath = Boards [cbBoardType.Active].ImageFilePath;
						drawingarea1.QueueDraw ();

						if (OnBoardSelected != null) {
							if (cbBoardType.Active != -1)
								OnBoardSelected.Invoke (this, new BoardSelectionArgs (Boards [cbBoardType.Active]));
							else
								OnBoardSelected.Invoke (this, null);
						}
					}
				};
				dialog.Run ();
				dialog.Destroy ();
			} else {
				SelectedBoard = Boards [cbBoardType.Active];
			}
		}
	}
}

