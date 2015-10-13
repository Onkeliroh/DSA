using System;
using System.Collections.Generic;
using Gtk;
using Gdk;
using Cairo;
using PrototypeBackend;
using PrototypeDebugWindow.Properties;
using System.IO;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;
using System.Drawing.Imaging;

namespace Frontend
{
	/// <summary>
	/// Label format options. 
	/// </summary>
	public enum LabelFormat
	{
		Flat,
		Bold
	}

	/// <summary>
	/// Label position options. 
	/// </summary>
	public enum LabelPosition
	{
		Left,
		Right,
		Bottom
	}

	/// <summary>
	/// Border type options 
	/// </summary>
	public enum BorderType
	{
		Line,
		NoLine
	}


	/// <summary>
	/// draws the board image. 
	/// </summary>
	public static class MCUDisplayHelper
	{
		/// <summary>
		/// The height of a bold label.
		/// </summary>
		public const int BoldHeight = 26;
		/// <summary>
		/// The height of a flat label.
		/// </summary>
		public const int FlatHeight = 17;
		/// <summary>
		/// The width of the label.
		/// </summary>
		public const int LabelWidth = 120;
		/// <summary>
		/// The label border weight.
		/// </summary>
		public const int LabelBorderWeight = 2;
		/// <summary>
		/// The size of the label font.
		/// </summary>
		public const int LabelFontSize = 12;
		/// <summary>
		/// The space between labels.
		/// </summary>
		public const int Space = 2;
		/// <summary>
		/// x middle of drawing area.
		/// </summary>
		public static double ShiftX = 0;
		/// <summary>
		/// y middle of drawing area.
		/// </summary>
		public static double ShiftY = 0;

		/// <summary>
		/// The MCU image upper left corner x value.
		/// </summary>
		private static double MCUImageXZero = 0;
		/// <summary>
		/// The MCU image upper left corner x value.
		/// </summary>
		private static double MCUImageYZero = 0;
		/// <summary>
		/// The pin locations in pixels.
		/// </summary>
		public static Dictionary<int,PrototypeBackend.Point> PinLocations = new Dictionary<int,PrototypeBackend.Point> ();

		/// <summary>
		/// The color of the background.
		/// </summary>
		private static readonly Cairo.Color BackgroundColor = new Cairo.Color (1, 1, 1, 0);

		/// <summary>
		/// Draws the Boards picture. 
		/// </summary>
		/// <param name="context">Context.</param>
		/// <param name="path">Path.</param>
		/// <param name="maxWidth">Max width.</param>
		public static void SetMCUSurface (Cairo.Context context, string path, int maxWidth = int.MaxValue)
		{
			try {
				var surf =	GetImage (path);
				MCUImageXZero = ShiftX - surf.Width / 2;
				MCUImageYZero = ShiftY - surf.Height / 2;
				context.SetSource (
					surf,
					MCUImageXZero,
					MCUImageYZero
				);
				context.Paint ();
				surf.Dispose ();
			} catch (Exception ex) {
				Console.Error.WriteLine (ex);
			}
		}

		/// <summary>
		///	Draws the labels. 
		/// </summary>
		/// <param name="context">Context.</param>
		/// <param name="pins">Pins.</param>
		/// <param name="xpos">Xpos.</param>
		/// <param name="ypos">Ypos.</param>
		/// <param name="labelposition">Labelposition.</param>
		/// <param name="labelformat">Labelformat.</param>
		/// <param name="bordtype">Bordtype.</param>
		public static void SetPinLabels (Cairo.Context context, List<IPin> pins, int xpos, int ypos, LabelPosition labelposition, LabelFormat labelformat = LabelFormat.Flat, BorderType bordtype = BorderType.Line)
		{
			int height = (labelformat == LabelFormat.Flat) ? FlatHeight : BoldHeight;

			for (int i = 0; i < pins.Count; i++) {
				DrawLabel (context, labelformat, bordtype, labelposition, pins [i], xpos, ypos + (i * height + i * Space));
			}
		}

		/// <summary>
		/// Draws the label.
		/// </summary>
		/// <param name="context">Context.</param>
		/// <param name="format">Format.</param>
		/// <param name="bordertype">Bordertype.</param>
		/// <param name="labelposition">Labelposition.</param>
		/// <param name="pin">Pin.</param>
		/// <param name="xpos">Xpos.</param>
		/// <param name="ypos">Ypos.</param>
		public static void DrawLabel (Cairo.Context context, LabelFormat format, BorderType bordertype, LabelPosition labelposition, IPin pin, int xpos, int ypos)
		{
			switch (format) {
			case LabelFormat.Flat:
				DrawLabelFlat (context, bordertype, labelposition, pin, xpos, ypos);
				break;
			case LabelFormat.Bold:
				DrawLabel (context, bordertype, pin, xpos, ypos);
				break;
			}
		}

		/// <summary>
		/// Draws the label.
		/// </summary>
		/// <param name="context">Context.</param>
		/// <param name="bordertype">Bordertype.</param>
		/// <param name="pin">Pin.</param>
		/// <param name="xpos">Xpos.</param>
		/// <param name="ypos">Ypos.</param>
		private static void DrawLabel (Cairo.Context context, BorderType bordertype, IPin pin, int xpos = 0, int ypos = 0)
		{
			const int widht = 100;
			const int height = BoldHeight;
			const int fontsize = 12;
			//Rect
			context.Rectangle (xpos, ypos, widht, 26);
			context.SetSourceRGBA (BackgroundColor.R, BackgroundColor.G, BackgroundColor.B, BackgroundColor.A);
			context.Fill ();

			string displaytext = pin.Name;

			if (displaytext.Length > 12) {
				displaytext = displaytext.Substring (0, 12);
				displaytext += "...";
			}

			if (bordertype == BorderType.Line) {
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

		/// <summary>
		/// Draws the label flat.
		/// </summary>
		/// <param name="context">Context.</param>
		/// <param name="bordertype">Bordertype.</param>
		/// <param name="labelposition">Labelposition.</param>
		/// <param name="pin">Pin.</param>
		/// <param name="xpos">Xpos.</param>
		/// <param name="ypos">Ypos.</param>
		private static void DrawLabelFlat (Cairo.Context context, BorderType bordertype, LabelPosition labelposition, IPin pin, int xpos = 0, int ypos = 0)
		{
			string displaytext = "";
			var color = GdkToCairo (pin.PlotColor);

			displaytext = pin.DisplayNumberShort + " " + pin.Name;

			if (displaytext.Length > 12) {
				displaytext = displaytext.Substring (0, 12);
				displaytext += "...";
			}

			if (bordertype == BorderType.Line) {
				DrawRoundedRectangle (context, xpos, ypos, LabelWidth - LabelBorderWeight, FlatHeight, 5);
				context.SetSourceRGBA (color.R, color.G, color.B, color.A);
				context.LineWidth = LabelBorderWeight;
				context.Stroke ();
			}

			//PinToLabelLine
			int xposlabelline = 0;
			int yposlabelline = 0;
			switch (labelposition) {
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

			if (PinLocations.ContainsKey ((int)pin.RealNumber)) {
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

		/// <summary>
		/// Draws the lines.
		/// </summary>
		/// <param name="context">Context.</param>
		/// <param name="xStart">X start.</param>
		/// <param name="yStart">Y start.</param>
		/// <param name="xEnd">X end.</param>
		/// <param name="yEnd">Y end.</param>
		/// <param name="color">Color.</param>
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

		private static Cairo.ImageSurface GetImage (string ImageName)
		{
			System.Drawing.Bitmap bitmap = (System.Drawing.Bitmap)Resources.ResourceManager.GetObject (ImageName);

//			using (MemoryStream stream = new MemoryStream ()) {
//				bitmap.Save (stream, ImageFormat.Png);
//				var strider = bitmap.LockBits (new System.Drawing.Rectangle (0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
//				var surf = new ImageSurface (stream.ToArray (), Format.ARGB32, strider.Width, strider.Height, strider.Stride);
//				return surf;
//			}

			bitmap.Save ("image", ImageFormat.Png);
			var surf = new ImageSurface ("image");
		
			File.Delete ("image");
			bitmap.Dispose ();
			return surf;
		}

		/// <summary>
		/// Converts a Gdk color to a Cairo color. 
		/// </summary>
		/// <returns>The to cairo.</returns>
		/// <param name="color">Color.</param>
		private static Cairo.Color GdkToCairo (Gdk.Color color)
		{
			double r = (double)color.Red / (double)ushort.MaxValue;
			double g = (double)color.Green / (double)ushort.MaxValue;
			double b = (double)color.Blue / (double)ushort.MaxValue;
			 
			return new Cairo.Color (r, g, b);
		}

		/// <summary>
		/// Creates a Cairo color from rgb.
		/// </summary>
		/// <returns>The to cairo.</returns>
		/// <param name="red">Red.</param>
		/// <param name="green">Green.</param>
		/// <param name="blue">Blue.</param>
		private static Cairo.Color RGBToCairo (ushort red, ushort green, ushort blue)
		{
			double r = (double)red / (double)ushort.MaxValue;
			double g = (double)green / (double)ushort.MaxValue;
			double b = (double)blue / (double)ushort.MaxValue;
			 
			return new Cairo.Color (r, g, b);
		}

		/// <summary>
		/// Draws the rounded rectangle.
		/// </summary>
		/// <param name="gr">Gr.</param>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <param name="width">Width.</param>
		/// <param name="height">Height.</param>
		/// <param name="radius">Radius.</param>
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

		/// <summary>
		/// Finds the minimum in a given array. 
		/// </summary>
		/// <param name="arr">Arr.</param>
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