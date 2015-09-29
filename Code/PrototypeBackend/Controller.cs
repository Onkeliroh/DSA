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

		public InfoLogger ConLogger { get; private set; }

		public ConfigurationManager ConfigManager { get; private set; }

		private List<System.Timers.Timer> measurementTimers = new List<System.Timers.Timer> ();

		public BoardConfiguration Configuration;

		public DateTime StartTime;
		private Stopwatch KeeperOfTime;
		private System.Timers.Timer SequencesTimer;
		private UInt16[] LastCondition = new UInt16[4]{ 0, 0, 0, 0 };


		public TimeSpan TimePassed { 
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


		public Board[] BoardConfigs;
		public List<string> LastConfigurationLocations = new List<string> () {
			string.Empty,
			string.Empty,
			string.Empty,
			string.Empty,
			string.Empty
		};

		public EventHandler OnControllerStarted;
		public EventHandler OnControllerStoped;
		public EventHandler OnOnfigurationLoaded;

		private bool running = false;

		public bool IsRunning { get { return running; } private set { } }

		#endregion

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

			#if DEBUG
			ConLogger = new InfoLogger (ConfigManager.GeneralData.Sections ["General"].GetKeyData ("DiagnosticsPath").Value, true, false, LogLevel.ERROR);
			ConLogger.LogToFile = true;
//			ConLogger.DateTimeFormat = "{0:mm:ss.fffff}";
			ConLogger.Start ();
			#else
			ConLogger = new InfoLogger (
				ConfigManager.GeneralData.Sections ["General"].GetKeyData ("DiagnosticsPath").Value, 
				true, 
				false
			);
			ConLogger.LogToFile = Convert.ToBoolean (ConfigManager.GeneralData.Sections ["General"].GetKeyData ("LogToFile"));
			ConfigManager = new ConfigurationManager ();
			#endif

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
					ConLogger.Log ("Pin Update: [" + e.UpdateOperation + "] " + e.Pin + " to " + e.Pin2);
				else
					ConLogger.Log ("Pin Update: [" + e.UpdateOperation + "] " + e.Pin);
			};
			Configuration.OnSequencesUpdated += (o, e) =>
			{
				if (e.UpdateOperation == UpdateOperation.Change)
					ConLogger.Log ("Sequence Update: [" + e.UpdateOperation + "] " + e.Seq + " to " + e.Seq);
				else
					ConLogger.Log ("Sequence Update: [" + e.UpdateOperation + "] " + e.Seq);
			};
			Configuration.OnSignalsUpdated += (o, e) =>
			{
				if (e.UpdateOperation == UpdateOperation.Change)
					ConLogger.Log ("Sequence Update: [" + e.UpdateOperation + "] " + e.MC + " to " + e.MC2);
				else
					ConLogger.Log ("Sequence Update: [" + e.UpdateOperation + "] " + e.MC);
			};

//			signalThread = new Thread (new ThreadStart (Run)){ Name = "controllerThread" };

			KeeperOfTime = new Stopwatch ();

			SequencesTimer = new System.Timers.Timer (1);
			SequencesTimer.Elapsed += OnSequenceTimeElapsed;
		}

		~Controller ()
		{
			ConLogger.Stop ();
		}

		/// <summary>
		/// Closes and stops all Member
		/// </summary>
		public void Quit ()
		{
			running = false;

			ConLogger.Stop ();
			SequencesTimer.Stop ();
			KeeperOfTime.Stop ();
			WritePreferences ();
		}

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
			KeeperOfTime.Stop ();

			if (OnControllerStoped != null)
			{
				OnControllerStoped.Invoke (this, null);
			}
		}

		public void Start ()
		{
			KeeperOfTime.Restart ();

			running = true;

			ArduinoController.SetPinModes (Configuration.AnalogPins.Select (o => o.DigitalNumber).ToArray<uint> (), Configuration.DigitalPins.Select (o => o.Number).ToArray<uint> ());
			MeasurementPreProcessing ();

			StartTime = DateTime.Now;
			LastCondition = new ushort[]{ 0, 0, 0, 0 };
			SequencesTimer.Start ();

			measurementTimers.ForEach (o => o.Start ());

			ConLogger.Log ("Controller Started", LogLevel.DEBUG);
			ConLogger.Log ("Start took: " + KeeperOfTime.ElapsedMilliseconds + "ms", LogLevel.DEBUG);

			if (OnControllerStarted != null)
			{
				OnControllerStarted.Invoke (this, null);
			}
		}

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

//					Console.WriteLine ("arraypos:{0}\tshift:{1}\tpos:{2}\tresult:{3}",
//						arraypos, 
//						shift, 
//						Convert.ToString (pos, 2).PadLeft (16, '0'), 
//						Convert.ToString (conditions [arraypos], 2).PadLeft (16, '0')
//					);
				}
			}

			if (LastCondition [0] != conditions [0] || LastCondition [1] != conditions [1] || LastCondition [2] != conditions [2] || LastCondition [3] != conditions [3])
			{
				ArduinoController.SetDigitalOutputPins (conditions);
				LastCondition = conditions;
			}
		}

		//Version1
		private void MeasurementPreProcessing ()
		{
			if (Configuration.AnalogPins.Count > 0)
			{

				#region Build Logger
				//build logger
				var logger = new CSVLogger (
					             Configuration.GetCSVLogName (),
					             true, 
					             false,
					             Configuration.CSVSaveFolderPath
				             );
				logger.Mapping = Configuration.CreateMapping ();
				logger.Separator = Configuration.Separator;
				logger.EmptySpaceFilling = Configuration.EmptyValueFilling;
				logger.DateTimeFormat = Configuration.TimeFormat;
				logger.WriteHeader (logger.Mapping.Keys.ToList<string> ());
				logger.Start ();
				#endregion

				//remove old timer
				measurementTimers.ForEach (o => o.Dispose ());
				measurementTimers.Clear ();

				var list = new List<APin> ();
				list = Configuration.AnalogPins.OrderBy (o => o.Period).ToList<APin> ();
				var comblist = new List<MeasurementCombination> ();
				comblist = Configuration.MeasurementCombinations;

				while (list.Count > 0)
				{
					//take every pin with the same period as the first one
					var query = list.Where (o => o.Period == list.First ().Period).ToList<APin> ();
					var combquery = comblist.Where (o => o.Period == query.First ().Period).ToList<MeasurementCombination> ();

					//pass values to logger
					var keys = new List<string> ();
					keys.AddRange (query.Select (x => x.DisplayName));
					keys.AddRange (combquery.Select (x => x.DisplayName));

					if (query.Count > 0)
					{
						//remove every pin with as certain period. so that it can not be added again
						list.RemoveAll (o => o.Period == query.First ().Period);
						comblist.RemoveAll (o => o.Period == query.First ().Period);

						var timer = new System.Timers.Timer (query.First ().Period);
						timer.Elapsed += (o, e) =>
						{
							//as long as running is true: collect data. otherwise go to sleep
							if (running)
							{
								var vals = ArduinoController.ReadAnalogPin (query.Select (x => x.Number).ToArray<uint> ());

								//in order to append to all values the same time stamp, otherwise every individual timestamp would be a little bit of
								var now = DateTime.Now; //what time is it?

								for (int i = 0; i < vals.Length; i++)
								{
									//add values, scaled to the AREF to their pin
									query [i].Value = new DateTimeValue () {
										Value = Configuration.Board.RAWToVolt (vals [i]),
										Time = now 
									};
								}

								var values = new List<DateTimeValue> ();
								values.AddRange (query.Select (x => x.Value));
								values.AddRange (combquery.Select (x => x.Value));

								logger.Log (keys, values);
							} else
							{
								logger.Stop ();
								(o as System.Timers.Timer).Stop ();
							}
						};
						measurementTimers.Add (timer);
					}
				}
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
		}
	}
}

