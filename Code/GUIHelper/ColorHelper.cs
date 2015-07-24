﻿using System;
using Gdk;

namespace PrototypeBackend
{
	public static class ColorHelper
	{
		public static byte uintToByte (uint val)
		{
			return (byte)(byte.MaxValue / 65535.0 * val);
		}

		public static uint RGBAFromGdkColor (Gdk.Color c)
		{
			return (uint)(
			    uintToByte (c.Red) << 24 |
			    uintToByte (c.Green) << 16 |
			    uintToByte (c.Blue) << 8
			);
		}

		public static Pixbuf ColoredPixbuf (Gdk.Color c, int w = 24, int h = 24)
		{
			var pixbuf = new Pixbuf (Colorspace.Rgb, false, 8, w, h); 
			pixbuf.Fill (RGBAFromGdkColor (c));
			return pixbuf;
		}
	}
}
