using System;
using System.Collections.Generic;
using Gtk;
using Gdk;
using Cairo;
using PrototypeBackend;
using Rsvg;

namespace Frontend
{
	public enum LabelFormat
	{
		Flat,
		Bold
	}

	public enum BordType
	{
		Line,
		NoLine
	}


	public static class MCUDisplayHelper
	{
		private const int BoldHeight = 26;
		private const int FlatHeight = 18;
		private const int Space = 2;
		private static readonly Cairo.Color BackgroundColor = new Cairo.Color (1, 1, 1, 0);

		//		private static Cairo.ImageSurface MCUSurface = null;
		//		private static string MCUPath = string.Empty;

		public static Cairo.ImageSurface PinLabels (List<IPin> pins, LabelFormat labelformat = LabelFormat.Flat, BordType bordtype = BordType.Line)
		{
			int height = (labelformat == LabelFormat.Flat) ? FlatHeight : BoldHeight;

			var surf = new Cairo.ImageSurface (Cairo.Format.Argb32, 125, height * pins.Count + Space * pins.Count);

			var context = new Cairo.Context (surf);

			context.SetSourceColor (BackgroundColor);
			context.Rectangle (0, 0, surf.Width, surf.Height);
			context.Fill ();

			for (int i = 0; i < pins.Count; i++)
			{
				DrawLabel (context, labelformat, bordtype, pins [i], 0, i * height + i * Space);
			}

			context.Dispose ();

			return surf;
		}

		public static Cairo.ImageSurface GetMCUSurface (string path, int maxWidth = int.MaxValue)
		{
//			if (path != MCUPath || MCUSurface == null)
//			{
			if (path != null && System.IO.File.Exists (path))
			{
				if (!path.Equals (string.Empty))
				{
					try
					{
						var MCUImage = new Rsvg.Handle (path);
						var buf = MCUImage.Pixbuf;

						int height = buf.Height;
						int width = buf.Width;

						if (width > maxWidth)
						{
							int newwidth = maxWidth - 100;
							newwidth = (newwidth < 0) ? 0 : newwidth;
							double scale = (width / 100.0) * newwidth;
							height = (int)(height * scale);
							width = newwidth;

							buf.ScaleSimple (width, height, InterpType.Bilinear);
						}

						var surf = new Cairo.ImageSurface (Cairo.Format.Argb32, width, height);
						var context = new Cairo.Context (surf);

						MCUImage.RenderCairo (context);

						context.Dispose ();
						return surf;
					} catch (Exception ex)
					{
						Console.Error.WriteLine (ex);
					}
				}
			}
			return new Cairo.ImageSurface (Cairo.Format.Argb32, 0, 0);
//			} else
//			{
//				return MCUSurface;
//			}
		}

		public static void DrawLabel (Cairo.Context context, LabelFormat format, BordType bordertype, IPin pin, int xpos, int ypos)
		{
			switch (format)
			{
			case LabelFormat.Flat:
				DrawLabelFlat (context, bordertype, pin, xpos, ypos);
				break;
			case LabelFormat.Bold:
				DrawLabel (context, bordertype, pin, xpos, ypos);
				break;
			}
		}

		private static void DrawLabel (Cairo.Context context, BordType bordertype, IPin pin, int xpos = 0, int ypos = 0)
		{
			const int widht = 100;
			const int height = BoldHeight;
			const int fontsize = 12;
			//Rect
			context.Rectangle (xpos, ypos, widht, 26);
			context.SetSourceColor (BackgroundColor);
			context.Fill ();

			string displaytext = pin.Name;

			if (displaytext.Length > 12)
			{
				displaytext = displaytext.Substring (0, 12);
				displaytext += "...";
			}

			if (bordertype == BordType.Line)
			{
				//Border
				context.SetSourceRGB (0, 0, 0);
				context.LineWidth = .5;
				context.Rectangle (xpos, ypos, widht, height);
				context.Stroke ();
			}

			//ColorFlag
			context.Rectangle (xpos, ypos, 5, height);
			context.SetSourceRGB (pin.PlotColor.Red, pin.PlotColor.Green, pin.PlotColor.Blue);
			context.Fill ();

			//Number
			context.SetSourceColor (new Cairo.Color (0, 0, 0));
			context.SelectFontFace ("Sans", FontSlant.Normal, FontWeight.Bold);
			context.SetFontSize (fontsize);
			context.MoveTo (xpos + 5, ypos + fontsize);
			context.ShowText (pin.DisplayNumberShort);

			context.MoveTo (xpos + 5, ypos + fontsize * 2);
			context.ShowText (displaytext);
		}

		private static void DrawLabelFlat (Cairo.Context context, BordType bordertype, IPin pin, int xpos = 0, int ypos = 0)
		{
			const int width = 120;
			const int height = FlatHeight;
			const int fontsize = 12;
			const int linewidth = 2;

			string displaytext = "";

			displaytext = pin.DisplayNumberShort + " " + pin.Name;

			if (displaytext.Length > 12)
			{
				displaytext = displaytext.Substring (0, 12);
				displaytext += "...";
			}

			//Rect
//			context.Rectangle (xpos, ypos, width, height);
//			context.SetSourceColor (BackgroundColor);

			if (bordertype == BordType.Line)
			{
				//Border
//				context.SetSourceRGB (0, 0, 0);
//				context.LineWidth = .5;
//				context.Rectangle (xpos, ypos, widht, height);
				DrawRoundedRectangle (context, xpos + linewidth, ypos + 1, width - linewidth, height - 1, 5);
				context.SetSourceColor (GdkToCairo (pin.PlotColor));
				context.LineWidth = linewidth;
				context.Stroke ();
			}
//			//ColorFlag
//			context.Rectangle (xpos, ypos, 5, 14);
//			context.SetSourceColor (GdkToCairo (pin.PlotColor));
//			context.Fill ();

			//Number
			context.SetSourceColor (new Cairo.Color (0, 0, 0));
			context.SelectFontFace ("Sans", FontSlant.Normal, FontWeight.Bold);
			context.SetFontSize (fontsize);
			context.MoveTo (xpos + 5, ypos + fontsize + linewidth);
			context.ShowText (displaytext);
		}

		private static Cairo.Color GdkToCairo (Gdk.Color color)
		{
			double r = (double)color.Red / (double)ushort.MaxValue;
			double g = (double)color.Green / (double)ushort.MaxValue;
			double b = (double)color.Blue / (double)ushort.MaxValue;
			 
			return new Cairo.Color (r, g, b);
		}

		private static Cairo.Color RGBToCairo (ushort red, ushort green, ushort blue)
		{
			double r = (double)red / (double)ushort.MaxValue;
			double g = (double)green / (double)ushort.MaxValue;
			double b = (double)blue / (double)ushort.MaxValue;
			 
			return new Cairo.Color (r, g, b);
		}

		private static void DrawRoundedRectangle (Cairo.Context gr, double x, double y, double width, double height, double radius)
		{
			gr.Save ();

			if ((radius > height / 2) || (radius > width / 2))
				radius = min (height / 2, width / 2);

			gr.MoveTo (x, y + radius);
			gr.Arc (x + radius, y + radius, radius, Math.PI, -Math.PI / 2);
			gr.LineTo (x + width - radius, y);
			gr.Arc (x + width - radius, y + radius, radius, -Math.PI / 2, 0);
			gr.LineTo (x + width, y + height - radius);
			gr.Arc (x + width - radius, y + height - radius, radius, 0, Math.PI / 2);
			gr.LineTo (x + radius, y + height);
			gr.Arc (x + radius, y + height - radius, radius, Math.PI / 2, Math.PI);
			gr.ClosePath ();
			gr.Restore ();
		}

		private static void DrawRoundedFlag (Cairo.Context gr, double x, double y, double width, double height, double radius)
		{
			gr.Save ();

			if ((radius > height / 2) || (radius > width / 2))
				radius = min (height / 2, width / 2);

			gr.MoveTo (x, y + radius);
			gr.Arc (x + radius, y + radius, radius, Math.PI, -Math.PI / 2);
			gr.LineTo (x + width, y);
			gr.MoveTo (x + width, y + height);
			gr.LineTo (x + radius, y + height);
			gr.Arc (x + radius, y + height - radius, radius, Math.PI / 2, Math.PI);
			gr.ClosePath ();
			gr.Restore ();
		}

		private static double min (params double[] arr)
		{
			int minp = 0;
			for (int i = 1; i < arr.Length; i++)
				if (arr [i] < arr [minp])
					minp = i;

			return arr [minp];
		}
	}
}