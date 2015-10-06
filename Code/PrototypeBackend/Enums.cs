using System;
using System.Collections;
using System.Collections.Generic;

namespace PrototypeBackend
{
	public static class FormatOptions
	{
		public static readonly Dictionary<string,string> TimeFormatOptions = new Dictionary<string, string> () {
			{ "SHORT (YYYY-MM-DD hh:mm:ss)", "{0:yyyy-MM-dd HH:mm:ss}" },
			{ "LONG (YYYY-MM-DD hh:mm:ss.ssss)", "{0:yyyy-MM-dd HH:mm:ss:ffff}" }
		};

		public static string GetFormat (string key)
		{
			return TimeFormatOptions [key];
		}
	}

	public static class SeparatorOptions
	{
		public static readonly Dictionary<string,string> Options = new Dictionary<string, string> () {
			{ "[COMMA]", "," },
			{ "[TAB]", "\t" },
			{ "[SPACE]", " " },
			{ "[SEMICOLON]", ";" },
			{ "[PIPE]", "|" }
		};

		public static string GetOption (string key)
		{
			return Options [key];
		}
	}
}
