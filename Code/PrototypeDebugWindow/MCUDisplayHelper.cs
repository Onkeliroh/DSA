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
		private const int FlatHeight = 14;
		private const int Space = 3;
		private static readonly Cairo.Color BackgroundColor = new Cairo.Color (1, 1, 1);

		public static Cairo.ImageSurface PinLabels (List<IPin> pins, LabelFormat labelformat = LabelFormat.Flat, BordType bordtype = BordType.Line)
		{
			int height = (labelformat == LabelFormat.Flat) ? FlatHeight : BoldHeight;

			var surf = new Cairo.ImageSurface (Cairo.Format.Rgb24, 100, height * pins.Count + Space * pins.Count);

			var context = new Cairo.Context (surf);

			context.SetSourceColor (BackgroundColor);
			context.Rectangle (0, 0, surf.Width, surf.Height);
			context.Fill ();

			for (int i = 0; i < pins.Count; i++)
			{
				DrawLabel (context, labelformat, bordtype, pins [i], 0, i * height + i * Space);
			}

			return surf;
		}

		public static Cairo.ImageSurface MCUSurface (string path, int maxWidth = int.MaxValue)
		{
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
						return surf;
					} catch (Exception ex)
					{
						Console.Error.WriteLine (ex);
					}
				}
			}
			return new Cairo.ImageSurface (Cairo.Format.Argb32, 0, 0);
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
			context.Color = new Cairo.Color (0, 0, 0);
			context.SelectFontFace ("Sans", FontSlant.Normal, FontWeight.Bold);
			context.SetFontSize (fontsize);
			TextExtents te = context.TextExtents (pin.DisplayNumberShort);
			context.MoveTo (xpos + 5, ypos + fontsize);
			context.ShowText (pin.DisplayNumberShort);

			context.MoveTo (xpos + 5, ypos + fontsize * 2);
			context.ShowText (displaytext);
		}

		private static void DrawLabelFlat (Cairo.Context context, BordType bordertype, IPin pin, int xpos = 0, int ypos = 0)
		{
			const int widht = 100;
			const int height = FlatHeight;
			const int fontsize = 12;

			string displaytext = "";

			displaytext = pin.DisplayNumberShort + " " + pin.Name;

			if (displaytext.Length > 12)
			{
				displaytext = displaytext.Substring (0, 12);
				displaytext += "...";
			}

			//Rect
			context.Rectangle (xpos, ypos, widht, height);
			context.SetSourceColor (BackgroundColor);
			context.Fill ();

			if (bordertype == BordType.Line)
			{
				//Border
				context.SetSourceRGB (0, 0, 0);
				context.LineWidth = .5;
				context.Rectangle (xpos, ypos, widht, height);
				context.Stroke ();
			}
			//ColorFlag
			context.Rectangle (xpos, ypos, 5, 14);
			context.SetSourceColor (GdkToCairo (pin.PlotColor));
			context.Fill ();

			//Number
			context.SetSourceColor (new Cairo.Color (0, 0, 0));
			context.SelectFontFace ("Sans", FontSlant.Normal, FontWeight.Bold);
			context.SetFontSize (fontsize);
			context.MoveTo (xpos + 5, ypos + fontsize);
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
	}
}