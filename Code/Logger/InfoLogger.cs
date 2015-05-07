using System;
using System.Text;

namespace SamplerLogger
{
	public enum LogLevel
	{
		DEBUG,
		INFO,
		WARNING,
		ERROR,
	}

	//	public readonly string[] LogLevelNames = new string[]{
	//		"DEBUG",
	//		"ÏNFO",
	//		"WARNING",
	//		"ERROR",
	//	}
	
	public class InfoLogger : Logger
	{
		//LogLevel
		private LogLevel _LogLevel = LogLevel.INFO;

		private bool loglevel{ get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Sampler.InfoLogger"/> class.
		/// </summary>
		/// <param name="filename">Filename.</param>
		public InfoLogger (string filename) : this (filename, true, false)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Sampler.InfoLogger"/> class.
		/// </summary>
		/// <param name="filename">Filename.</param>
		/// <param name="localtime">If set to <c>true</c> a localtime timestamp will be logged.</param>
		/// <param name="utc">If set to <c>true</c> a UTC timestamp will be logged.</param>
		public InfoLogger (string filename, bool localtime, bool utc)
			: this (filename, localtime, utc, LogLevel.INFO)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Sampler.InfoLogger"/> class.
		/// </summary>
		/// <param name="filename">Filename.</param>
		/// <param name="localtime">If set to <c>true</c> a localtime timestamp will be logged.</param>
		/// <param name="utc">If set to <c>true</c> a UTC timestamp will be logged.</param>
		/// <param name="loglvl">The default LogLevel. Determens to mimium Level for a message to be logged.</param>
		public InfoLogger (string filename, bool localtime, bool utc, LogLevel loglvl)
			: this (filename, localtime, utc, loglvl, "")
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Sampler.InfoLogger"/> class.
		/// </summary>
		/// <param name="filename">Filename.</param>
		/// <param name="localtime">If set to <c>true</c> a localtime timestamp will be logged.</param>
		/// <param name="utc">If set to <c>true</c> a UTC timestamp will be logged.</param>
		/// <param name="loglvl">The default LogLevel. Determens to mimium Level for a message to be logged.</param>
		/// <param name="location">Location.</param>
		public InfoLogger (string filename, bool localtime, bool utc, LogLevel loglvl, string location)
			: base (location + filename)
		{
			this._LogLevel = loglvl;
			Separator = "\t";
			base.LogTimeUTC = false;
		}

		/// <summary>
		/// Sets the log level.
		/// </summary>
		/// <param name="loglvl">Loglvl.</param>
		public void setLogLevel (LogLevel loglvl)
		{
			this._LogLevel = loglvl;
		}

		/// <summary>
		/// Gets the log level.
		/// </summary>
		/// <returns>The log level.</returns>
		public LogLevel getLogLevel ()
		{
			return this._LogLevel;
		}

		/// <summary>
		/// Log the specified msg.
		/// </summary>
		/// <param name="msg">Message.</param>
		public override void Log (string msg)
		{
			Log (msg, LogLevel.INFO);
		}

		/// <summary>
		/// Log the specified msg depending on the LogLevel.
		/// </summary>
		/// <param name="msg">Message.</param>
		/// <param name="loglvl">Loglvl.</param>
		public void Log (string msg, LogLevel loglvl)
		{
			base.Log (GetTimeString () + "[" + getLogLevelName (loglvl) + "]" + Separator + msg);
		}

		/// <summary>
		/// Translates numerical values to human readable strings for better a understanding of the log file.
		/// </summary>
		/// <param name="level"> loglevel value</param>
		/// <returns> name of the loglevel value  (e.g. 0 = DEBUG)</returns>
		private string getLogLevelName (LogLevel level)
		{
			switch (level)
			{
			case LogLevel.DEBUG:
				return "DEBUG";
			case LogLevel.INFO:
				return "INFO";
			case LogLevel.WARNING:
				return "WARNING";
			case LogLevel.ERROR:
				return "ERROR";
			default:
				return "";
			}
		}
	
	}
}

