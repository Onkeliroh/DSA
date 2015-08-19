using System;
using System.Text;

namespace Logger
{
	public enum LogLevel
	{
		DEBUG,
		INFO,
		WARNING,
		ERROR,
	}

	public class InfoLogger : Logger
	{
		//LogLevel
		private LogLevel _LogLevel = LogLevel.INFO;

		private bool loglevel{ get; set; }

		public bool LogToFile = true;

		public EventHandler<LogMessageArgs> NewMessage;


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
			: base (location + filename, localtime, utc)
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
			if (NewMessage != null)
			{
				NewMessage.Invoke (this, new LogMessageArgs (msg, DateTime.Now, LogLevel.INFO));
			}
		}

		/// <summary>
		/// Log the specified msg depending on the LogLevel.
		/// </summary>
		/// <param name="msg">Message.</param>
		/// <param name="loglvl">Loglvl.</param>
		public void Log (string msg, LogLevel loglvl)
		{
			if (LogToFile)
			{
				base.Log (GetTimeString () + "[" + loglvl + "]" + Separator + msg);
			}
			if (NewMessage != null)
			{
				NewMessage.Invoke (this, new LogMessageArgs (msg, DateTime.Now, loglvl));
			}
		}
	}
}

