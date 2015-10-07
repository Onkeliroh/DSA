using System;
using System.Collections;
using System.Collections.Generic;

namespace PrototypeBackend
{
	/// <summary>
	/// A class for storing all DateTime format options.
	/// </summary>
	public static class FormatOptions
	{
		/// <summary>
		/// The datetime format options.
		/// </summary>
		public static readonly Dictionary<string,string> TimeFormatOptions = new Dictionary<string, string> () {
			{ "SHORT (YYYY-MM-DD hh:mm:ss)", "{0:yyyy-MM-dd HH:mm:ss}" },
			{ "LONG (YYYY-MM-DD hh:mm:ss.ssss)", "{0:yyyy-MM-dd HH:mm:ss:ffff}" }
		};

		/// <summary>
		/// Gets the format.
		/// </summary>
		/// <returns>The format.</returns>
		/// <param name="key">Key.</param>
		public static string GetFormat (string key)
		{
			return TimeFormatOptions [key];
		}
	}

	/// <summary>
	/// A class for storing all separator options.
	/// </summary>
	public static class SeparatorOptions
	{
		/// <summary>
		/// The options.
		/// </summary>
		public static readonly Dictionary<string,string> Options = new Dictionary<string, string> () {
			{ "[COMMA]", "," },
			{ "[TAB]", "\t" },
			{ "[SPACE]", " " },
			{ "[SEMICOLON]", ";" },
			{ "[PIPE]", "|" }
		};

		/// <summary>
		/// Gets the option.
		/// </summary>
		/// <returns>The option.</returns>
		/// <param name="key">Key.</param>
		public static string GetOption (string key)
		{
			return Options [key];
		}
	}
}
