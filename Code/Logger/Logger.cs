using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace Logger
{
	public class Logger
	{
		#region Config

		private StreamWriter LogWriter;

		private string FileName_;

		public string FileName {
			get{ return FileName_; }
			set {
				if (value.Length > 0)
					FileName_ = value;
			}
		}

		public string FilePath{ get; private set; }


		public Encoding FileEncoding = Encoding.UTF8;

		private Thread LogThread;

		public ThreadState State {
			private set{ }
			get {
				return LogThread.ThreadState;
			}
		}

		public bool disposed {
			private set;
			get;
		}

		private Queue<string> LogQueue = new Queue<string> ();

		private string DateTimeFormat_ = "{0:yyyy-MM-dd-HH:mm:ss}";

		public string DateTimeFormat {
			get{ return DateTimeFormat_; }
			set{ DateTimeFormat_ = value; }
		}

		/// <summary>
		/// Singals wheather the Logger is processing new messages or not.
		/// </summary>
		public bool IsLogging {
			get{ return IsLogging_; } 
			private set{ }
		}

		private bool IsLogging_ = true;

		public string Linebreak = "\r\n";

		public string Separator = ",";

		private DateTime LastLog_ = DateTime.Now;

		private int MaxLog_ = 1000;

		/// <summary>
		/// Sets the maximum number if lines to be queued before the next writing to the file
		/// </summary>
		public int MaxLog {
			get{ return MaxLog; }
			set {
				MaxLog_ = (value < 10) ? 10 : value;
			}
		}

		private int MaxLogAge_ = 1000;

		/// <summary>
		/// Sets the maximum timespan(in milliseconds) between the last file writing and the next file writing
		/// </summary>
		public int MaxLogAge {
			get{ return MaxLogAge_; }
			set {
				MaxLogAge_ = (value < 10) ? 10 : value;
			}
		}

		private int RefreshRate_ = 100;

		/// <summary>
		/// Sets the frequency for the main thread to check and write new logs in milliseconds.
		/// </summary>
		public int RefreshRate {
			get{ return RefreshRate_; }
			set {
				RefreshRate_ = (value < 10) ? 10 : value;
			}
		}


		public bool LogTimeLocal = false;

		public bool LogTimeUTC = false;

		/// <summary>
		/// CultureInfo is set to en-US, because of the common value display
		/// </summary>
		public CultureInfo CultureInfo{ get; set; }

		#endregion

		/// <summary>
		/// Initializes a new instance of the <see cref="SamplerLogger.Logger"/> class.
		/// </summary>
		public Logger ()
		{
			CultureInfo = new CultureInfo ("en-US");

			LogThread = new Thread (new ThreadStart (this.Run));
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Sampler.Logger"/> class.
		/// </summary>
		/// <param name="filename">Name of the new Log file. May also include a path.</param>
		public Logger (string filename, bool local = true, bool utc = false) : this ()
		{
			FileName_ = filename;
			LogTimeLocal = local;
			LogTimeUTC = utc;
		}

		public Logger (string filename, string filepath = "", bool local = true, bool utc = false) : this ()
		{
			FileName_ = filename;
			FilePath = filepath;
			LogTimeLocal = local;
			LogTimeUTC = utc;
		}

		/// <summary>
		/// Releases unmanaged resources and performs other cleanup operations before the <see cref="Sampler.Logger"/> is
		/// reclaimed by garbage collection. The LogQueue will be emptied.
		/// </summary>
		~Logger ()
		{           
			Stop ();
		}

		public void Init ()
		{
			if (FileName_.Length > 0) {
				try {
					if (!string.IsNullOrEmpty (FilePath)) {
						if (!System.IO.Directory.Exists (FilePath)) {
							System.IO.Directory.CreateDirectory (FilePath);
						}
					}
					string path = "";
					if (!string.IsNullOrEmpty (FilePath)) {
						path += FilePath;
						if (Environment.OSVersion.Platform == PlatformID.Unix) {
							path += @"/";
						} else if (Environment.OSVersion.Platform == PlatformID.MacOSX) {
							path += @"/";	
						} else if (Environment.OSVersion.Platform == PlatformID.Win32NT) {
							path += @"\";
						}
					}
					path += FileName_;

					if (File.Exists (path)) {
						LogWriter = new StreamWriter (new FileStream (path, FileMode.Append, FileAccess.Write, FileShare.Write), FileEncoding);
					} else {
						LogWriter = new StreamWriter (new FileStream (path, FileMode.CreateNew, FileAccess.Write, FileShare.Write), FileEncoding);
					}

					LogThread.Name = FileName_ + "_thread";
				} catch (Exception) {
					throw;
				}
			} else {
				throw new ArgumentNullException ("FileName", "There is no filename.");
			}
		}

		/// <summary>
		/// Starts the internal thread for logging to the file.
		/// </summary>
		public void Start ()
		{
			try {
				Init ();
				IsLogging_ = true;
				LogThread.Start ();
			} catch {
				throw;
			}
		}

		/// <summary>
		/// Starts the internal thread for logging to the file.
		/// </summary>
		/// <param name="filename">Name and path of the Logfile</param>
		public void Start (string filename)
		{
			FileName_ = filename;
			try {
				Start ();
			} catch {
				throw;
			}
		}

		/// <summary>
		/// Stop the internal thread for logging to the file. 
		/// Therefore all pending LogQueue items will be written befor stopping.
		/// </summary>
		public void Stop ()
		{
			LogToFile ();
			while (LogQueue.Count != 0) {
				Thread.Sleep (10);
			}
			LogThread.Abort ();

			LogWriter.Close ();
		}

		/// <summary>
		/// Joins the thread with the calling thread.
		/// </summary>
		public void Join ()
		{
			LogThread.Join ();
		}

		/// <summary>
		/// Determines whether the internal thread is alive.
		/// </summary>
		/// <returns><c>true</c> if this instance is alive; otherwise, <c>false</c>.</returns>
		public bool IsAlive ()
		{
			return LogThread.IsAlive;
		}

		/// <summary>
		/// Method of the internal thread. checks frequently for the nessasity of a flush.
		/// </summary>
		protected void Run ()
		{
			while (IsLogging_ || LogQueue.Count > 0) {
				if (LogQueue.Count >= MaxLog_ || DoLog ()) {
					LastLog_ = DateTime.Now;
					LogToFile ();
				}
				Thread.Sleep (RefreshRate_);
			}
			LogWriter.Close (); //close stream so that the file is free to use elsewhere
		}

		/// <summary>
		/// Generates a string with, depending on the settings, timestamps and the correct separator for easy use.
		/// </summary>
		/// <returns>The time string.</returns>
		protected string GetTimeString ()
		{
			StringBuilder sb = new StringBuilder ();
			if (LogTimeLocal) {
				sb.Append (String.Format (DateTimeFormat, DateTimeOffset.Now));
				sb.Append (Separator);
			}
			if (LogTimeUTC) {
				sb.Append (String.Format (DateTimeFormat, DateTimeOffset.UtcNow));
				sb.Append (Separator);
			}
			return sb.ToString (); 
		}

		/// <summary>
		/// Logs a specified msg.
		/// </summary>
		/// <param name="msg">Message</param>
		public virtual void Log (string msg)
		{
			if (IsLogging_) {
				lock (LogQueue) {
					LogQueue.Enqueue (msg);
				}
			}
		}

		/// <summary>
		/// Queues a header to the logqueue only if a filename was provided and the destination file 
		/// apperars to be empty or non existend.
		/// </summary>
		/// <param name="Header">Header</param>
		public virtual void WriteHeader (string Header)
		{
			bool wirteHeader = true;
			//is there a designated file?
			if (FileName_ != String.Empty) {
				//does it allready exist
				if (System.IO.File.Exists (FileName_)) {
					//is it filled with stuff and probably already a header?
					if (System.IO.File.ReadAllLines (FileName_).Length == 0) {
						wirteHeader = false;
					}
				} 

				if (wirteHeader) {
					var sb = new StringBuilder ();
					if (LogTimeLocal)
						sb.Append ("LocalTime" + Separator);
					if (LogTimeUTC)
						sb.Append ("UTC" + Separator);
					sb.Append (Header);
					LogQueue.Enqueue (sb.ToString ());
				}
			}
		}

		/// <summary>
		/// Checks weather it is time to initiate a new flush or not. 
		/// Depending on either the last time or the amount of logs collected.
		/// </summary>
		/// <returns><c>true</c>, if log should be done, <c>false</c> otherwise.</returns>
		protected virtual bool DoLog ()
		{
			TimeSpan LogAge = DateTime.Now - LastLog_;
			if (LogAge.TotalMilliseconds >= MaxLogAge_) {
				return true;
			} else {
				return false;
			}
		}

		/// <summary>
		/// Logs to file.
		/// </summary>
		protected virtual void LogToFile ()
		{
			lock (LogWriter) {
				while (this.LogQueue.Count > 0) {
					lock (LogQueue) {
						LogWriter.Write (this.LogQueue.Dequeue () + Linebreak);
						LogWriter.Flush ();
					}
				}
			}
		}
	}
}

