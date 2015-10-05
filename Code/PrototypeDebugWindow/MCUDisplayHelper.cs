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

	public enum LabelPosition
	{
		Left,
		Right,
		Bottom
	}

	public enum BordType
	{
		Line,
		NoLine
	}


	public static class MCUDisplayHelper
	{
		public const int BoldHeight = 26;
		public const int FlatHeight = 17;
		public const int LabelWidth = 120;
		public const int LabelBorderWeight = 2;
		public const int LabelFontSize = 12;
		public const int Space = 2;
		public static double shiftX = 0;
		public static double shiftY = 0;
		private static double MCUImageXZero = 0;
		private static double MCUImageYZero = 0;
		public static Dictionary<int,PrototypeBackend.Point> PinLocations = new Dictionary<int,PrototypeBackend.Point> ();

		private static readonly Cairo.Color BackgroundColor = new Cairo.Color (1, 1, 1, 0);

		//		private static Cairo.ImageSurface MCUSurface = null;
		//		private static string MCUPath = string.Empty;

		public static void SetMCUSurface (Cairo.Context context, string path, int maxWidth = int.MaxValue)
		{
			if (path != null && System.IO.File.Exists (path))
			{
				if (!path.Equals (string.Empty))
				{
					try
					{
						ImageSurface surf;
						if (path.Contains (".svg"))
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

							surf = new Cairo.ImageSurface (Cairo.Format.Argb32, width, height);
							var svgcontext = new Cairo.Context (surf);

							MCUImage.RenderCairo (svgcontext);

						} else
						{
							surf = new Cairo.ImageSurface (path);
						}
						MCUImageXZero = shiftX - surf.Width / 2;
						MCUImageYZero = shiftY - surf.Height / 2;

						context.SetSource (
							surf,
							MCUImageXZero,
							MCUImageYZero
						);
						context.Paint ();
					} catch (Exception ex)
					{
						Console.Error.WriteLine (ex);
					}
				}
			}
		}

		public static void SetPinLabels (Cairo.Context context, List<IPin> pins, int xpos, int ypos, LabelPosition labelposition, LabelFormat labelformat = LabelFormat.Flat, BordType bordtype = BordType.Line)
		{
			int height = (labelformat == LabelFormat.Flat) ? FlatHeight : BoldHeight;

			for (int i = 0; i < pins.Count; i++)
			{
				DrawLabel (context, labelformat, bordtype, labelposition, pins [i], xpos, ypos + (i * height + i * Space));
			}
		}

		public static void DrawLabel (Cairo.Context context, LabelFormat format, BordType bordertype, LabelPosition labelposition, IPin pin, int xpos, int ypos)
		{
			switch (format)
			{
			case LabelFormat.Flat:
				DrawLabelFlat (context, bordertype, labelposition, pin, xpos, ypos);
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
			context.SetSourceRGBA (BackgroundColor.R, BackgroundColor.G, BackgroundColor.B, BackgroundColor.A);
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
			context.SetSourceRGB (0, 0, 0);
			context.SelectFontFace ("Sans", FontSlant.Normal, FontWeight.Bold);
			context.SetFontSize (fontsize);
			context.MoveTo (xpos + 5, ypos + fontsize);
			context.ShowText (pin.DisplayNumberShort);

			context.MoveTo (xpos + 5, ypos + fontsize * 2);
			context.ShowText (displaytext);
		}

		private static void DrawLabelFlat (Cairo.Context context, BordType bordertype, LabelPosition labelposition, IPin pin, int xpos = 0, int ypos = 0)
		{
			string displaytext = "";
			var color = GdkToCairo (pin.PlotColor);

			displaytext = pin.DisplayNumberShort + " " + pin.Name;

			if (displaytext.Length > 12)
			{
				displaytext = displaytext.Substring (0, 12);
				displaytext += "...";
			}

			if (bordertype == BordType.Line)
			{
				DrawRoundedRectangle (context, xpos, ypos, LabelWidth - LabelBorderWeight, FlatHeight, 5);
				context.SetSourceRGBA (color.R, color.G, color.B, color.A);
				context.LineWidth = LabelBorderWeight;
				context.Stroke ();
			}

			//PinToLabelLine
			int xposlabelline = 0;
			int yposlabelline = 0;
			switch (labelposition)
			{
			case LabelPosition.Left:
				xposlabelline = xpos + LabelWidth;
				yposlabelline = ypos + (FlatHeight / 2);
				break;
			case LabelPosition.Right:
				xposlabelline = xpos;
				yposlabelline = ypos + (FlatHeight / 2);
				break;
			case LabelPosition.Bottom:
				xpos = xpos + LabelWidth / 2;
				yposlabelline = ypos;
				break;
			default:
				break;
			}

			if (PinLocations.ContainsKey ((int)pin.RealNumber))
			{
				DrawLines (
					context,
					xposlabelline,
					yposlabelline,
					(int)(MCUImageXZero + PinLocations [(int)pin.RealNumber].x), 
					(int)(MCUImageYZero + PinLocations [(int)pin.RealNumber].y),
					color
				);
			}

			//Number
			context.SetSourceRGB (0, 0, 0);
			context.SelectFontFace ("Sans", FontSlant.Normal, FontWeight.Bold);
			context.SetFontSize (LabelFontSize);
			context.MoveTo (xpos + 5, ypos + LabelFontSize + LabelBorderWeight);
			context.ShowText (displaytext);
		}

		private static void DrawLines (Cairo.Context context, int xStart, int yStart, int xEnd, int yEnd, Cairo.Color color)
		{
			context.Save ();
			context.SetSourceRGBA (color.R, color.G, color.B, color.A);
			context.MoveTo (xStart, yStart);
			context.LineTo (xEnd, yEnd);
			context.ClosePath ();
			context.Restore ();
			context.LineWidth = 1;
			context.Stroke ();
		}

		#region Helperly

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

		#endregion
	}
}