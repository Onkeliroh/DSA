using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.IO;
using PrototypeBackend;
using Logger;
using System.Diagnostics;

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

		/// <summary>
		/// Gets the config manager.
		/// </summary>
		/// <value>The config manager.</value>
		public ConfigurationManager ConfigManager { get; private set; }

		/// <summary>
		/// The measurement timer.
		/// </summary>
		private System.Timers.Timer MeasurementTimer;

		/// <summary>
		/// BoardConfiguration
		/// </summary>
		public BoardConfiguration Configuration;

		/// <summary>
		/// The start time.
		/// </summary>
		public DateTime StartTime;

		/// <summary>
		/// The keeper of time. Gets the time elapsed since controller start.
		/// </summary>
		private Stopwatch KeeperOfTime;

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
				if (KeeperOfTime != null)
				{
					return KeeperOfTime.Elapsed;
				} else
				{
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
			ConfigManager = new ConfigurationManager (ConfigurationPath);
			BoardConfigs = ConfigManager.ParseBoards (ConfigManager.GeneralData.Sections ["General"].GetKeyData ("BoardPath").Value);

			LastConfigurationLocations [0] = ConfigManager.GeneralData.Sections ["General"].GetKeyData ("Config1").Value;
			LastConfigurationLocations [1] = ConfigManager.GeneralData.Sections ["General"].GetKeyData ("Config2").Value;
			LastConfigurationLocations [2] = ConfigManager.GeneralData.Sections ["General"].GetKeyData ("Config3").Value;
			LastConfigurationLocations [3] = ConfigManager.GeneralData.Sections ["General"].GetKeyData ("Config4").Value;
			LastConfigurationLocations [4] = ConfigManager.GeneralData.Sections ["General"].GetKeyData ("Config5").Value;

			ConLogger = new InfoLogger (ConfigManager.GeneralData.Sections ["General"].GetKeyData ("DiagnosticsPath").Value, true, false, LogLevel.ERROR);
			ConLogger.LogToFile = true;
			ConLogger.Start ();

			bool ConfigAutoConnect = Convert.ToBoolean (ConfigManager.GeneralData.Sections ["General"] ["AutoConnect"]);

			ArduinoController.AutoConnect = ConfigAutoConnect;
			ArduinoController.Init ();
			ArduinoController.OnReceiveMessage += (sender, e) => ConLogger.Log ("IN < " + e.Message, LogLevel.DEBUG);
			ArduinoController.OnSendMessage += (sender, e) => ConLogger.Log ("OUT > " + e.Message, LogLevel.DEBUG);
			ArduinoController.OnConnectionChanged += ((o, e) =>
			{
				if (e.Connected)
				{
					#if DEBUG
					ConLogger.Log ("Connected to: " + ArduinoController.Board.ToString (), LogLevel.DEBUG);
					#endif
					#if RELEASE
					ConLogger.Log ("Connected to " + ArduinoController.SerialPortName);
					#endif

				} else
				{
					ConLogger.Log ("Disconnected");
				}
			});

			Configuration.OnPinsUpdated += (o, e) =>
			{
				if (e.UpdateOperation == UpdateOperation.Change)
					ConLogger.Log ("Pin Update: [" + e.UpdateOperation + "] " + e.OldPin + " to " + e.NewPin);
				else
					ConLogger.Log ("Pin Update: [" + e.UpdateOperation + "] " + e.OldPin);
			};
			Configuration.OnSequencesUpdated += (o, e) =>
			{
				if (e.UpdateOperation == UpdateOperation.Change)
					ConLogger.Log ("Sequence Update: [" + e.UpdateOperation + "] " + e.OldSeq + " to " + e.OldSeq);
				else
					ConLogger.Log ("Sequence Update: [" + e.UpdateOperation + "] " + e.OldSeq);
			};
			Configuration.OnSignalsUpdated += (o, e) =>
			{
				if (e.UpdateOperation == UpdateOperation.Change)
					ConLogger.Log ("Sequence Update: [" + e.UpdateOperation + "] " + e.OldMeCom + " to " + e.NewMeCom);
				else
					ConLogger.Log ("Sequence Update: [" + e.UpdateOperation + "] " + e.OldMeCom);
			};

//			signalThread = new Thread (new ThreadStart (Run)){ Name = "controllerThread" };

			KeeperOfTime = new Stopwatch ();

			SequencesTimer = new System.Timers.Timer (10);
			SequencesTimer.Elapsed += OnSequenceTimeElapsed;
		}

		/// <summary>
		/// Releases unmanaged resources and performs other cleanup operations before the
		/// <see cref="PrototypeBackend.Controller"/> is reclaimed by garbage collection.
		/// </summary>
		~Controller ()
		{
			if (ConLogger != null)
			{
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
			SequencesTimer.Stop ();
			KeeperOfTime.Stop ();
			WritePreferences ();
		}

		/// <summary>
		/// Writes the preferences.
		/// </summary>
		public void WritePreferences ()
		{
			ConfigManager.GeneralData.Sections ["General"].GetKeyData ("Config1").Value = LastConfigurationLocations [0];
			ConfigManager.GeneralData.Sections ["General"].GetKeyData ("Config2").Value = LastConfigurationLocations [1];
			ConfigManager.GeneralData.Sections ["General"].GetKeyData ("Config3").Value = LastConfigurationLocations [2];
			ConfigManager.GeneralData.Sections ["General"].GetKeyData ("Config4").Value = LastConfigurationLocations [3];
			ConfigManager.GeneralData.Sections ["General"].GetKeyData ("Config5").Value = LastConfigurationLocations [4];

			ConfigManager.SaveGeneralSettings ();
		}

		/// <summary>
		/// Stop Measurement ans Sequencer.
		/// </summary>
		public void Stop ()
		{
			running = false;

			ConLogger.Log ("Controller Stoped", LogLevel.DEBUG);
			SequencesTimer.Stop ();
			MeasurementTimer.Stop ();
			KeeperOfTime.Stop ();
			MeasurementCSVLogger.Stop ();

			if (OnControllerStoped != null)
			{
				OnControllerStoped.Invoke (this, null);
			}
		}

		/// <summary>
		/// Start this controller and measurements.
		/// </summary>
		public void Start ()
		{
			KeeperOfTime.Restart ();

			running = true;

			ArduinoController.SetPinModes (Configuration.AnalogPins.Select (o => o.RealNumber).ToArray<uint> (), Configuration.DigitalPins.Select (o => o.RealNumber).ToArray<uint> ());
			MeasurementPreProcessing ();

			StartTime = DateTime.Now;
			LastCondition = new ushort[]{ 0, 0, 0, 0 };

			SequencesTimer.Start ();
			MeasurementTimer.Start ();

			ConLogger.Log ("Controller Started", LogLevel.DEBUG);
			ConLogger.Log ("Start took: " + KeeperOfTime.ElapsedMilliseconds + "ms", LogLevel.DEBUG);

			if (OnControllerStarted != null)
			{
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

			foreach (Sequence seq in Configuration.Sequences)
			{
				if (seq.GetCurrentState (time) == DPinState.HIGH)
				{
					int arraypos = (int)seq.Pin.Number / 16;
					int shift = (int)seq.Pin.Number % 16;
					int pos = 0x1 << (int)shift;
					conditions [arraypos] = Convert.ToUInt16 (conditions [arraypos] | pos);
				}
			}

			if (LastCondition [0] != conditions [0] || LastCondition [1] != conditions [1] || LastCondition [2] != conditions [2] || LastCondition [3] != conditions [3])
			{
				ArduinoController.SetDigitalOutputPins (conditions);
			}
			LastCondition = conditions;
		}

		//Version1
		/// <summary>
		/// Creates multiple timers acording to the needed measurement frequencies.
		/// </summary>
		private void MeasurementPreProcessing ()
		{
			if (Configuration.AnalogPins.Count > 0)
			{
				#region Build Logger
				MeasurementCSVLogger = new CSVLogger (
					Configuration.GetCSVLogName (),
					true, 
					false,
					Configuration.CSVSaveFolderPath
				);
				MeasurementCSVLogger.Mapping = Configuration.CreateMapping ();
				MeasurementCSVLogger.Separator = SeparatorOptions.GetOption (Configuration.Separator);
				MeasurementCSVLogger.EmptySpaceFilling = Configuration.EmptyValueFilling;
				MeasurementCSVLogger.DateTimeFormat =	FormatOptions.GetFormat (Configuration.TimeFormat);
				MeasurementCSVLogger.WriteHeader (MeasurementCSVLogger.Mapping.Keys.ToList<string> ());
				MeasurementCSVLogger.Start ();
				#endregion

				if (MeasurementTimer != null)
				{
					MeasurementTimer.Dispose ();
				}

				MeasurementTimer = new System.Timers.Timer (10);
				MeasurementTimer.Elapsed += OnMeasurementTimerElapsed;
			}
		}

		/// <summary>
		/// Raised by the <see cref="MeasurementTimer"/>. Collects data from board.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="args">Arguments.</param>
		protected void OnMeasurementTimerElapsed (object sender, System.Timers.ElapsedEventArgs args)
		{
			if (running)
			{
				double time = KeeperOfTime.ElapsedMilliseconds;
				var measurements = Configuration.AnalogPins.Where (o => time % o.Interval <= 10).ToArray ();
				if (measurements.Length > 0)
				{
					var query = measurements.Select (o => o.Number).ToArray ();
					var vals = ArduinoController.ReadAnalogPin (query);

					var now = DateTime.Now;

					for (int i = 0; i < measurements.Length; i++)
					{
						measurements [i].Value = new DateTimeValue (vals [i], now);
					}

					var values = measurements.Select (o => o.Value).ToList ();
					values.AddRange (Configuration.MeasurementCombinations.Select (o => o.Value));

					MeasurementCSVLogger.Log (values);
				}
			} else
			{
				MeasurementTimer.Stop ();
			}
		}

		/// <summary>
		/// Saves the configuration.
		/// </summary>
		/// <returns><c>true</c>, if configuration was saved, <c>false</c> otherwise.</returns>
		/// <param name="path">Path.</param>
		public bool SaveConfiguration (string path = null)
		{
			try
			{
				Stream stream;
				if (path == null)
				{
					stream = File.Open (Configuration.ConfigSavePath, FileMode.Create);
				} else
				{
					stream = File.Open (path, FileMode.Create);
				}
				var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter ();

				BoardConfiguration config = new BoardConfiguration ();
				config = this.Configuration;

				formatter.Serialize (stream, config);

				if (LastConfigurationLocations.Contains (path))
				{
					LastConfigurationLocations.Remove (path);
					LastConfigurationLocations.Reverse ();
					LastConfigurationLocations.Add (path);
					LastConfigurationLocations.Reverse ();
				} else
				{
					LastConfigurationLocations.Reverse ();
					LastConfigurationLocations.Add (path);
					LastConfigurationLocations.Reverse ();
				}
				while (LastConfigurationLocations.Count > 5)
				{
					LastConfigurationLocations.RemoveAt (5);
				}

				stream.Close ();
			} catch (Exception)
			{
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
			if (File.Exists (path))
			{
				try
				{
					Stream stream = File.Open (path, FileMode.Open, FileAccess.Read, FileShare.Write);
					var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter ();

					var config = formatter.Deserialize (stream);

					Configuration = (BoardConfiguration)config;

					stream.Close ();
				
					if (LastConfigurationLocations.Contains (path))
					{
						LastConfigurationLocations.Remove (path);
						LastConfigurationLocations.Reverse ();
						LastConfigurationLocations.Add (path);
						LastConfigurationLocations.Reverse ();
					} else
					{
						LastConfigurationLocations.Reverse ();
						LastConfigurationLocations.Add (path);
						LastConfigurationLocations.Reverse ();
					}

					if (OnOnfigurationLoaded != null)
					{
						OnOnfigurationLoaded.Invoke (this, null);
					}
				} catch (Exception)
				{
					throw;
				}

				return true;
			} else
			{
				return false;
			}
		}
	}
}

