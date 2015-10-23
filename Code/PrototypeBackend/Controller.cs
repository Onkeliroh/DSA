using Logger;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using PrototypeBackend;
using PrototypeBackend.Properties;
using System.Text;
using System.Globalization;


namespace PrototypeBackend
{
	public class Controller
	{
		#region Member

		/// <summary>
		/// Gets the gernal purpose logger.
		/// </summary>
		/// <value>The general purpose logger.</value>
		public InfoLogger ConLogger { get; private set; }

		/// <summary>
		/// The measurement CSV logger.
		/// </summary>
		private CSVLogger MeasurementCSVLogger;

		public LogLevel LoggerLevel {
			get{ return PrototypeBackend.Properties.Settings.Default.LogLevel; }
			set {
				PrototypeBackend.Properties.Settings.Default.LogLevel = value;
				PrototypeBackend.Properties.Settings.Default.Save ();
			}
		}

		public bool LogToFile {
			get{ return PrototypeBackend.Properties.Settings.Default.LogToFile; }
			set {
				PrototypeBackend.Properties.Settings.Default.LogToFile = value;
				PrototypeBackend.Properties.Settings.Default.Save ();
			}
		}

		public string LogFilePath {
			get{ return PrototypeBackend.Properties.Settings.Default.LogFilePath; }
			set {
				PrototypeBackend.Properties.Settings.Default.LogFilePath = value;
				PrototypeBackend.Properties.Settings.Default.Save ();
			}
		}

		/// <summary>
		/// Gets the config manager.
		/// </summary>
		/// <value>The config manager.</value>
		public ConfigurationManager ConfigManager { get; private set; }

		/// <summary>
		/// The measurement timer.
		/// </summary>
		//		private System.Timers.Timer MeasurementTimer;
		private System.Threading.Timer MeasurementTimer;


		/// <summary>
		/// BoardConfiguration
		/// </summary>
		public BoardConfiguration Configuration;

		/// <summary>
		/// The start time.
		/// </summary>
		public DateTime StartTime = DateTime.Now;

		/// <summary>
		/// The keeper of time. Gets the time elapsed since controller start.
		/// </summary>
		private Stopwatch KeeperOfTime = new Stopwatch ();

		/// <summary>
		/// The sequences timer.
		/// </summary>
		private System.Timers.Timer SequencesTimer;

		/// <summary>
		/// The last condition send by the <paramref name="SequencesTimer"/>. This is to minimize the send packages.
		/// </summary>
		private UInt16[] LastCondition = new UInt16[4]{ 0, 0, 0, 0 };

		/// <summary>
		/// Gets the elapsed time.
		/// </summary>
		/// <value>The elapsed time.</value>
		public TimeSpan TimeElapsed { 
			get {
				if (KeeperOfTime != null) {
					return KeeperOfTime.Elapsed;
				} else {
					return new TimeSpan (0);
				}
			}
			private set{ }
		}

		/// <summary>
		/// Array of all available boards. Parsed from BoardConfigFile
		/// </summary>
		public Board[] BoardConfigs;

		/// <summary>
		///	The 5 last used configurations.
		/// </summary>
		public List<string> LastConfigurationLocations = new List<string> () {
			string.Empty,
			string.Empty,
			string.Empty,
			string.Empty,
			string.Empty
		};

		#region Events

		/// <summary>
		/// Raised when controller started.
		/// </summary>
		public EventHandler OnControllerStarted;
		/// <summary>
		/// Raised when controller stoped.
		/// </summary>
		public EventHandler OnControllerStoped;
		/// <summary>
		/// Raised when onfiguration loaded.
		/// </summary>
		public EventHandler OnOnfigurationLoaded;

		#endregion

		/// <summary>
		///	Determines whether the controller timers shall keep running or not. 
		/// </summary>
		private bool running = false;

		/// <summary>
		/// Gets a value indicating whether this controller is running.
		/// </summary>
		/// <value><c>true</c> if this controller is running; otherwise, <c>false</c>.</value>
		public bool IsRunning { get { return running; } private set { } }

		#endregion

		/// <summary>
		/// Initializes a new instance of the <see cref="PrototypeBackend.Controller"/> class.
		/// </summary>
		/// <param name="ConfigurationPath">Configuration path.</param>
		public Controller (string ConfigurationPath = null)
		{
			Configuration = new BoardConfiguration ();
			using (MemoryStream memstream = new MemoryStream (Encoding.ASCII.GetBytes (Resources.Boards))) {
				using (StreamReader str = new StreamReader (memstream)) {
					BoardConfigs = ConfigurationManager.ParseBoards (str);
				}
			}

			LastConfigurationLocations [0] = Properties.Settings.Default.Config1;
			LastConfigurationLocations [1] = Properties.Settings.Default.Config2;
			LastConfigurationLocations [2] = Properties.Settings.Default.Config3;
			LastConfigurationLocations [3] = Properties.Settings.Default.Config4;
			LastConfigurationLocations [4] = Properties.Settings.Default.Config5;

			ConLogger = new InfoLogger (Resources.LogFileName, true, false, Settings.Default.LogLevel, Settings.Default.LogFilePath);
			ConLogger.LogToFile = Settings.Default.LogToFile; 
			ConLogger.Start ();


			ArduinoController.AutoConnect = Settings.Default.AutoConnect; 
			ArduinoController.Init ();
			ArduinoController.OnReceiveMessage += (sender, e) => ConLogger.Log ("IN < " + e.Message, LogLevel.DEBUG);
			ArduinoController.OnSendMessage += (sender, e) => ConLogger.Log ("OUT > " + e.Message, LogLevel.DEBUG);
			ArduinoController.OnConnectionChanged += ((o, e) => {
				if (e.Connected) {
					ConLogger.Log ("Connected to: " + ArduinoController.Board.ToString (), LogLevel.INFO);
				} else {
					ConLogger.Log ("Disconnected", LogLevel.INFO);
				}
			});

			Configuration.OnPinsUpdated += (o, e) => {
				if (e.UpdateOperation == UpdateOperation.Change)
					ConLogger.Log ("Pin Update: [" + e.UpdateOperation + "] " + e.OldPin + " to " + e.NewPin);
				else
					ConLogger.Log ("Pin Update: [" + e.UpdateOperation + "] " + e.OldPin);
			};
			Configuration.OnSequencesUpdated += (o, e) => {
				if (e.UpdateOperation == UpdateOperation.Change)
					ConLogger.Log ("Sequence Update: [" + e.UpdateOperation + "] " + e.OldSeq + " to " + e.OldSeq);
				else
					ConLogger.Log ("Sequence Update: [" + e.UpdateOperation + "] " + e.OldSeq);
			};
			Configuration.OnSignalsUpdated += (o, e) => {
				if (e.UpdateOperation == UpdateOperation.Change)
					ConLogger.Log ("Sequence Update: [" + e.UpdateOperation + "] " + e.OldMeCom + " to " + e.NewMeCom);
				else
					ConLogger.Log ("Sequence Update: [" + e.UpdateOperation + "] " + e.OldMeCom);
			};
		}

		/// <summary>
		/// Releases unmanaged resources and performs other cleanup operations before the
		/// <see cref="PrototypeBackend.Controller"/> is reclaimed by garbage collection.
		/// </summary>
		~Controller ()
		{
			if (ConLogger != null) {
				ConLogger.Stop ();
			}
		}

		/// <summary>
		/// Stops controller and referenced timers. Writes the preferences.
		/// </summary>
		public void Quit ()
		{
			running = false;

			ConLogger.Stop ();
			if (SequencesTimer != null)
				SequencesTimer.Stop ();
			KeeperOfTime.Stop ();
			WritePreferences ();
		}

		public void LoadLastConfigAndConnect ()
		{
			if (!string.IsNullOrEmpty (Properties.Settings.Default.Config1)) {
				OpenConfiguration (Properties.Settings.Default.Config1);
			}	
			if (!string.IsNullOrEmpty (Properties.Settings.Default.LastConnectedPort)) {
				ArduinoController.SerialPortName = Properties.Settings.Default.LastConnectedPort;
				ArduinoController.Setup (Configuration.Board.UseDTR);
			}
		}

		/// <summary>
		/// Writes the preferences.
		/// </summary>
		public void WritePreferences ()
		{
			Properties.Settings.Default.Config1 = LastConfigurationLocations [0];
			Properties.Settings.Default.Config2 = LastConfigurationLocations [1];
			Properties.Settings.Default.Config3 = LastConfigurationLocations [2];
			Properties.Settings.Default.Config4 = LastConfigurationLocations [3];
			Properties.Settings.Default.Config5 = LastConfigurationLocations [4];
			Properties.Settings.Default.Save ();
		}

		/// <summary>
		/// Stop Measurement ans Sequencer.
		/// </summary>
		public void Stop ()
		{
			running = false;

			ConLogger.Log ("Controller Stoped", LogLevel.DEBUG);
			SequencesTimer.Stop ();
			if (MeasurementTimer != null) {
				try {
					MeasurementTimer.Dispose ();
					lock (MeasurementCSVLogger) {					
						MeasurementCSVLogger.Stop ();
					}
				} catch (Exception) {
				}
			}

			KeeperOfTime.Stop ();

			if (OnControllerStoped != null) {
				OnControllerStoped.Invoke (this, null);
			}
		}

		/// <summary>
		/// Start this controller and measurements.
		/// </summary>
		public void Start ()
		{
			//Save the port, so that next time the connection may be automaticly established
			Properties.Settings.Default.LastConnectedPort = ArduinoController.SerialPortName;
			Properties.Settings.Default.Save ();

			KeeperOfTime.Restart ();

			running = true;

			ArduinoController.SetPinModes (Configuration.AnalogPins.Select (o => o.RealNumber).ToArray<uint> (), Configuration.DigitalPins.Select (o => o.RealNumber).ToArray<uint> ());

			StartTime = DateTime.Now;
			LastCondition = new ushort[]{ 0, 0, 0, 0 };

			SequencesTimer = new System.Timers.Timer (10);
			SequencesTimer.Elapsed += OnSequenceTimeElapsed;
			SequencesTimer.Start ();

			MeasurementPreProcessing ();
			MeasurementTimer = new System.Threading.Timer (new TimerCallback (OnMeasurementTimerTick), null, 0, 10);

			ConLogger.Log ("Controller Started", LogLevel.DEBUG);
			ConLogger.Log ("Start took: " + KeeperOfTime.ElapsedMilliseconds + "ms", LogLevel.DEBUG);

			if (OnControllerStarted != null) {
				OnControllerStarted.Invoke (this, null);
			}
		}

		/// <summary>
		/// Raised by the <paramref name="SequencesTimer"/> elapsed event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="args">Arguments.</param>
		private void OnSequenceTimeElapsed (object sender, System.Timers.ElapsedEventArgs args)
		{
			double time = KeeperOfTime.ElapsedMilliseconds;

			UInt16[] conditions = new UInt16[4];
			conditions [0] = 0x0;
			conditions [1] = 0x0;
			conditions [2] = 0x0;
			conditions [3] = 0x0;

			foreach (Sequence seq in Configuration.Sequences) {
				if (seq.GetCurrentState (time) == DPinState.HIGH) {
					int arraypos = (int)seq.Pin.Number / 16;
					int shift = (int)seq.Pin.Number % 16;
					int pos = 0x1 << (int)shift;
					conditions [arraypos] = Convert.ToUInt16 (conditions [arraypos] | pos);
				}
			}

			if (LastCondition [0] != conditions [0] || LastCondition [1] != conditions [1] || LastCondition [2] != conditions [2] || LastCondition [3] != conditions [3]) {
				ArduinoController.SetDigitalOutputPins (conditions);
			}
			LastCondition = conditions;
		}

		/// <summary>
		/// Creates multiple timers acording to the needed measurement frequencies.
		/// </summary>
		private void MeasurementPreProcessing ()
		{
			if (Configuration.AnalogPins.Count > 0) {
				#region Build Logger
				MeasurementCSVLogger = new CSVLogger (
					Configuration.GetCSVLogName (),
					new List<string> (),
					true, 
					false,
					Configuration.CSVSaveFolderPath
				);
				MeasurementCSVLogger.CultureInfo = CultureInfo.GetCultures (CultureTypes.AllCultures).Single (o => o.EnglishName == Configuration.ValueFormatCultur);
				MeasurementCSVLogger.Mapping = Configuration.CreateMapping ();
				MeasurementCSVLogger.Separator = SeparatorOptions.GetOption (Configuration.Separator);
				MeasurementCSVLogger.EmptySpaceFilling = Configuration.EmptyValueFilling;
				MeasurementCSVLogger.DateTimeFormat =	FormatOptions.GetFormat (Configuration.TimeFormat);
				MeasurementCSVLogger.WriteHeader (MeasurementCSVLogger.Mapping.Keys.ToList<string> ());
				MeasurementCSVLogger.Start ();
				#endregion

				if (MeasurementTimer != null) {
					MeasurementTimer.Dispose ();
				}

//				MeasurementTimer = new System.Timers.Timer (10);
//				MeasurementTimer.Elapsed += OnMeasurementTimerElapsed;
			}
		}

		/// <summary>
		/// Raised by the <see cref="MeasurementTimer"/>. Collects data from board.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="args">Arguments.</param>
		private void OnMeasurementTimerTick (object state)
		{
			try {
				if (running) {
					double time = KeeperOfTime.ElapsedMilliseconds;

					var analogPins = Configuration.AnalogPins.Where (o => time % o.Interval <= 10).ToArray ();
					if (analogPins.Length > 0) {
						var query = analogPins.Select (o => o.Number).ToArray ();
						var vals = ArduinoController.ReadAnalogPin (query);

						var now = DateTime.Now;

						for (int i = 0; i < analogPins.Length; i++) {
							analogPins [i].Value = new DateTimeValue (vals [i], now);
						}

						var analogPinValues = analogPins.Select (o => o.Value.Value).ToList<double> ();
						var analogPinValuesNames = analogPins.ToList ().Select (o => o.DisplayName).ToList ();

						var MeComValues = Configuration.MeasurementCombinations
						.Select (o => o.Value.Value)
						.Where (o => !double.IsNaN (o))
						.ToList <double> ();
						var MeComValuesNames = Configuration.MeasurementCombinations
						.Where (o => !double.IsNaN (o.Value.Value))
						.Select (o => o.DisplayName)
						.ToList ();

						var names = analogPinValuesNames;
						names.AddRange (MeComValuesNames);
						var values = analogPinValues;
						values.AddRange (MeComValues);

						MeasurementCSVLogger.Log<double> (names, values);
					}
				} else {
					System.Threading.Timer t = (System.Threading.Timer)state;
					t.Dispose ();
				}
			} catch (Exception) {
			}
		}

		/// <summary>
		/// Saves the configuration.
		/// </summary>
		/// <returns><c>true</c>, if configuration was saved, <c>false</c> otherwise.</returns>
		/// <param name="path">Path.</param>
		public bool SaveConfiguration (string path = null)
		{
			try {
				Stream stream;
				if (path == null) {
					stream = File.Open (Configuration.ConfigSavePath, System.IO.FileMode.Create);
				} else {
					stream = File.Open (path, System.IO.FileMode.Create);
				}
				var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter ();

				BoardConfiguration config = new BoardConfiguration ();
				config = this.Configuration;

				formatter.Serialize (stream, config);

				if (LastConfigurationLocations.Contains (path)) {
					LastConfigurationLocations.Remove (path);
					LastConfigurationLocations.Reverse ();
					LastConfigurationLocations.Add (path);
					LastConfigurationLocations.Reverse ();
				} else {
					LastConfigurationLocations.Reverse ();
					LastConfigurationLocations.Add (path);
					LastConfigurationLocations.Reverse ();
				}
				while (LastConfigurationLocations.Count > 5) {
					LastConfigurationLocations.RemoveAt (5);
				}

				stream.Close ();
			} catch (Exception) {
				throw;
			} 
			return true;
		}

		/// <summary>
		/// Opens the configuration.
		/// </summary>
		/// <returns><c>true</c>, if configuration was opened, <c>false</c> otherwise.</returns>
		/// <param name="path">Path.</param>
		public bool OpenConfiguration (string path)
		{
			if (File.Exists (path)) {
				try {
					Stream stream = File.Open (path, System.IO.FileMode.Open, FileAccess.Read, FileShare.Write);
					var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter ();

					var config = formatter.Deserialize (stream);

					Configuration = (BoardConfiguration)config;

					stream.Close ();
				
					if (LastConfigurationLocations.Contains (path)) {
						LastConfigurationLocations.Remove (path);
						LastConfigurationLocations.Reverse ();
						LastConfigurationLocations.Add (path);
						LastConfigurationLocations.Reverse ();
					} else {
						LastConfigurationLocations.Reverse ();
						LastConfigurationLocations.Add (path);
						LastConfigurationLocations.Reverse ();
					}
					WritePreferences ();

					if (OnOnfigurationLoaded != null) {
						OnOnfigurationLoaded.Invoke (this, null);
					}
				} catch (Exception) {
					throw;
				}

				return true;
			} else {
				return false;
			}
		}
	}
}

