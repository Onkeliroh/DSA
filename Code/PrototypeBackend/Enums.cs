using System;
using System.Collections;
using System.Collections.Generic;

namespace PrototypeBackend
{
	public static class TimeFormatOptions
	{
		public static readonly Dictionary<string,string> FormatOptions = new Dictionary<string, string> () {
			{ "SHORT (YYYY-MM-DD hh:mm:ss)", "YYYY-MM-DD HH:mm:ss" },
			{ "LONG (YYYY-MM-DD hh:mm:ss.ssss)", "YYYY-MM-DD HH:mm:ss:ffff" }
		};

		public static string GetFormat (string key)
		{
			return FormatOptions [key];
		}
	}

	public static class SeparatorOptions
	{
		public static readonly Dictionary<string,string> Options = new Dictionary<string, string> () {
			{ "[TAB]", "\t" },
			{ "[SPACE]", " " },
			{ "[SEMICOLON]", ";" },
			{ "[COMMA]", "," },
			{ "[PIPE]", "|" }
		};

		public static string GetOption (string key)
		{
			return Options [key];
		}
	}
}
