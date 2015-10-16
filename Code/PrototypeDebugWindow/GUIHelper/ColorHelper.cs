using System;
using System.Drawing;
using Gdk;
using OxyPlot;

namespace GUIHelper
{
	/// <summary>
	///	A collection of helping methods for dealing with the mass of different color classes. 
	/// </summary>
	public static class ColorHelper
	{
		/// <summary>
		/// Translates a unsigned integer to a byte value.
		/// </summary>
		/// <returns>The byte.</returns>
		/// <param name="val">Value.</param>
		public static byte uintToByte (uint val)
		{
			return (byte)(byte.MaxValue / 65535.0 * val);
		}

		/// <summary>
		/// Transforms a Gdk color into a unsigned integer value;
		/// </summary>
		/// <returns>The unsigned integer.</returns>
		/// <param name="c">The Gdk color.</param>
		public static uint RGBAFromGdkColor (Gdk.Color c)
		{
			return (uint)(
			    uintToByte (c.Red) << 24 |
			    uintToByte (c.Green) << 16 |
			    uintToByte (c.Blue) << 8
			);
		}

		/// <summary>
		/// Transforms a Gdk color into a unsigned integer value;
		/// </summary>
		/// <returns>The unsigned integer.</returns>
		/// <param name="c">The Gdk color.</param>
		public static uint ARGBFromGdkColor (Gdk.Color c)
		{
			return (uint)(
			    0x0f << 24 |
			    uintToByte (c.Red) << 16 |
			    uintToByte (c.Green) << 8 |
			    uintToByte (c.Blue)
			);
		}

		/// <summary>
		///	Translates a Gdk color to a OxyColor 
		/// </summary>
		/// <returns>The OxyColor</returns>
		/// <param name="c">The Gdk color</param>
		public static OxyColor GdkColorToOxyColor (Gdk.Color c)
		{
			return OxyPlot.OxyColor.FromArgb (
				255, 
				ColorHelper.uintToByte (c.Red),
				ColorHelper.uintToByte (c.Green),
				ColorHelper.uintToByte (c.Blue));
		}

		/// <summary>
		///	Creates a Pixbuf based on the given parameters. 
		/// </summary>
		/// <returns>The pixbuf.</returns>
		/// <param name="c">The color</param>
		/// <param name="w">The width.</param>
		/// <param name="h">The height.</param>
		public static Pixbuf ColoredPixbuf (Gdk.Color c, int w = 24, int h = 24)
		{
			var pixbuf = new Pixbuf (Colorspace.Rgb, false, 8, w, h); 
			pixbuf.Fill (RGBAFromGdkColor (c));
			return pixbuf;
		}

		/// <summary>
		///	Creates a random Gdk color 
		/// </summary>
		/// <returns>The random Gdk color.</returns>
		public static Gdk.Color GetRandomGdkColor ()
		{
			var rng = new Random ();
			return new Gdk.Color ((byte)rng.Next (), (byte)rng.Next (), (byte)rng.Next ());
		}

		/// <summary>
		/// Translated a System.Drawing color to a Gdk color.
		/// </summary>
		/// <returns>The Gdk color.</returns>
		/// <param name="color">The System.Drawing color.</param>
		public static Gdk.Color SystemColorToGdkColor (System.Drawing.Color color)
		{
			return new Gdk.Color (color.R, color.G, color.B);
		}
	}
}

