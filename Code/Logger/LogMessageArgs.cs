using System;
using System.Text;

namespace Logger
{
	public class LogMessageArgs : EventArgs
	{
		public DateTime Time { get; private set; }

		public string Message { get; private set; }

		public LogLevel Level { get; private set; }

		public LogMessageArgs (string message, object time = null, LogLevel level = LogLevel.INFO)
		{
			if (time != null) {
				Time = (DateTime)time;
			} else {
				Time = DateTime.Now;
			}
			Level = level;
			Message = message;
		}
	}


	
}

