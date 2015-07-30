using System;
using System.Threading;
using System.IO;
using System.Globalization;
using System.Text;
using System.Collections.Generic;

namespace Logger
{
	public sealed class CSVLogger : Logger
	{
		/// <summary>
		/// Default Constructor
		/// </summary>
		public CSVLogger () : this ("logfile.csv", new List<object>{ })
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Sampler.CSVLogger"/> class.
		/// </summary>
		/// <param name="filename">Filename. (May also include a path)</param>
		public CSVLogger (string filename) : this (filename, new  List<object> ())
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Sampler.CSVLogger"/> class.
		/// </summary>
		/// <param name="filename">Filename. (May also include a path)</param>
		/// <param name="header">Header.</param>
		public CSVLogger (string filename, List<object>header) : this (filename, new List<object> (), true, true)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Sampler.CSVLogger"/> class.
		/// </summary>
		/// <param name="filename">Filename.</param>
		/// <param name="header">Header.</param>
		/// <param name="localtime">If set to <c>true</c> a localtime timestamp will be logged.</param>
		/// <param name="utc">If set to <c>true</c> a UTC timestamp will be logged.</param>
		public CSVLogger (string filename, List<object>header, bool localtime, bool utc)
			: this (filename, localtime, utc, "")
		{
			//override the default separator of the Logger class
			WriteHeader (CreateString (header));
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Sampler.CSVLogger"/> class.
		/// </summary>
		/// <param name="filename">Filename.</param>
		/// <param name="header">Header.</param>
		/// <param name="localtime">If set to <c>true</c> a localtime timestamp will be logged.</param>
		/// <param name="utc">If set to <c>true</c> a UTC timestamp will be logged.</param>
		public CSVLogger (string filename, List<string> header, bool localtime, bool utc, string location)
			: this (filename, localtime, utc, location)
		{
			//override the default separator of the Logger class
			WriteHeader (CreateString (header));
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Sampler.CSVLogger"/> class.
		/// </summary>
		/// <param name="filename">Filename.</param>
		/// <param name="localtime">If set to <c>true</c> a localtime timestamp will be logged.</param>
		/// <param name="utc">If set to <c>true</c> a UTC timestamp will be logged.</param>
		/// <param name="location">Location of the log file</param>
		public CSVLogger (string filename, bool localtime, bool utc, string location)
			: base (location + filename)
		{
			Separator = ","; 
			base.LogTimeLocal = localtime;
			base.LogTimeUTC = utc;
		}

		/// <summary>
		/// Creates the string from a list of strings and adds between each strings pair the correct separator.
		/// </summary>
		/// <returns>The string.</returns>
		/// <param name="row">A row of values.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public string CreateString<T> (List<T> row)
		{
			StringBuilder sb = new StringBuilder ();
			bool first = true;
			foreach (T value in row)
			{
				if (!first)
					sb.Append (Separator);
				if (typeof(T) != typeof(string))
					sb.Append (Convert.ToString (value, CultureInfo.NumberFormat));
				else
					sb.Append (value);
				first = false;
			}
			return sb.ToString ();
		}

		/// <summary>
		/// Log the specified row.
		/// </summary>
		/// <param name="row">A row of values.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public void Log<T> (List<T> row)
		{
			Log (GetTimeString () + CreateString (row));
		}


		/// <summary>
		/// Writes the header.
		/// </summary>
		/// <param name="msg">Message.</param>
		public void WriteHeader (List<string>  msg)
		{
			base.WriteHeader (CreateString (msg));
		}
	}
}