using System;
using Gtk;
using Gdk;

namespace MCUWidget
{
	[System.ComponentModel.ToolboxItem (true)]
	public partial class MCUWidget : Gtk.Bin
	{
		public string MCUImagepath{ get; set; }

		public ListStore BoardTypes{ get; set; }

		public ListStore AREFTypes { get; set; }

		public MCUWidget ()
		{
			this.Build ();

//			MCUImagepath = "/home/onkeliroh/Bachelorarbeit/Resources/arduino_uno.svg";

			this.drawingarea1.ExposeEvent += Draw;
		}

		public void Redraw ()
		{
			
		}

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
			if (MCUImagepath != null)
			{
				if (!MCUImagepath.Equals (string.Empty))
				{
					try
					{
						var MCUImage = new Rsvg.Handle (MCUImagepath);
						var buf = MCUImage.Pixbuf;
						var surf = new Cairo.ImageSurface (Cairo.Format.Argb32, buf.Width, buf.Height);
						var context = new Cairo.Context (surf);

						MCUImage.RenderCairo (context);
						return surf;
					} catch (Exception ex)
					{
						Console.Error.WriteLine (ex);
					}
				}
			}
			return new Cairo.ImageSurface (Cairo.Format.Argb32, 0, 0);
		}

		private Cairo.ImageSurface MCULabelLeft ()
		{
			return new Cairo.ImageSurface (Cairo.Format.ARGB32, 0, 0);	
		}
	}
}

