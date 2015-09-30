using System;
using System.Collections;
using System.Collections.Generic;

namespace PrototypeBackend
{
	public static class TimeFormatOptions
	{
		public static readonly Dictionary<string,string> FormatOptions = new Dictionary<string, string> () {
			{ "SHORT", "HH:mm:ss" },
			{ "LONG", "HH:mm:ss:ffff" }
		};

		public static string GetFormat (string key)
		{
			return FormatOptions [key];
		}
	}

	public static class SeparatorOptions
	{
		public static readonly Dictionary<string,string> Options = new Dictionary<string, string> () {
			{ "[TAB]","\t" },
			{ "[SPACE]"," " },
			{ "[SEMICOLON]",";" },
			{ "[COMMA]","," },
			{ "[PIPE]","|" }
		};

		public static string GetOption (string key)
		{
			if (Options.ContainsKey (key))
			{
				return Options [key];
			} else
			{
				return key;
			}
		}
	}
}
