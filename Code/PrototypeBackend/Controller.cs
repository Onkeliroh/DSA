using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
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


		//		private Thread sequenceThread;

		private List<Thread> sequenceThreads = new List<Thread> ();
		private List<System.Timers.Timer> measurementTimers = new List<System.Timers.Timer> ();

		public BoardConfiguration Configuration;

		public DateTime StartTime;

		public TimeSpan TimePassed { 
			get {
				if (TimeKeeper != null)
				{
					return TimeKeeper.Elapsed;
				} else
				{
					return new TimeSpan (0);
				}
			}
			private set{ }
		}

		private Stopwatch TimeKeeper;

		public Board[] BoardConfigs;

		public EventHandler OnControllerStarted;
		public EventHandler OnControllerStoped;

		private bool running = false;

		public bool IsRunning { get { return running; } private set { } }

		#endregion

		public Controller (string ConfigurationPath = null)
		{
			Configuration = new BoardConfiguration ();

			ConfigManager = new ConfigurationManager (ConfigurationPath);
			#if DEBUG
			BoardConfigs = ConfigManager.ParseBoards (ConfigManager.GeneralData.Sections ["General"].GetKeyData ("BoardPath").Value);
			ConLogger = new InfoLogger (ConfigManager.GeneralData.Sections ["General"].GetKeyData ("DiagnosticsPath").Value, true, false, LogLevel.DEBUG);
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

				BuildSequenceList ();
			};
			Configuration.OnSignalsUpdated += (o, e) =>
			{
				if (e.UpdateOperation == UpdateOperation.Change)
					ConLogger.Log ("Sequence Update: [" + e.UpdateOperation + "] " + e.MC + " to " + e.MC2);
				else
					ConLogger.Log ("Sequence Update: [" + e.UpdateOperation + "] " + e.MC);
			};

//			signalThread = new Thread (new ThreadStart (Run)){ Name = "controllerThread" };

			TimeKeeper = new Stopwatch ();
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

			sequenceThreads.Clear ();

			ConLogger.Stop ();
			TimeKeeper.Stop ();
		}

		/// <summary>
		/// Stop Measurement ans Sequencer.
		/// </summary>
		public void Stop ()
		{
			running = false;

			ConLogger.Log ("Controller Stoped", LogLevel.DEBUG);
			TimeKeeper.Stop ();

			if (OnControllerStoped != null)
			{
				OnControllerStoped.Invoke (this, null);
			}
		}

		public void Start ()
		{
			TimeKeeper.Restart ();

			running = true;

			ArduinoController.SetPinModes (Configuration.AnalogPins.Select (o => o.DigitalNumber).ToArray<uint> (), Configuration.DigitalPins.Select (o => o.Number).ToArray<uint> ());
			BuildSequenceList ();
			MeasurementPreProcessing ();
			StartTime = DateTime.Now;
			sequenceThreads.ForEach (o => o.Start ());
			measurementTimers.ForEach (o => o.Start ());
			ConLogger.Log ("Controller Started", LogLevel.DEBUG);
			ConLogger.Log ("Start took: " + TimeKeeper.ElapsedMilliseconds + "ms", LogLevel.DEBUG);

			if (OnControllerStarted != null)
			{
				OnControllerStarted.Invoke (this, null);
			}
		}

		private void BuildSequenceList ()
		{
			sequenceThreads.Clear ();
			GC.Collect ();
			foreach (Sequence seq in Configuration.Sequences)
			{
				var seqThread = new Thread (
					                () =>
					{
						Stopwatch sw = new Stopwatch ();
						sw.Start ();

						uint pin = seq.Pin.Number;
						PinMode mode = seq.Pin.Mode;
						var op = seq.Current ();
						while (seq.CurrentState != SequenceState.Done && running && op != null)
						{
							ArduinoController.SetPinState (pin, ((SequenceOperation)op).State);
							Thread.Sleep (((SequenceOperation)op).Duration);
							op = seq.Next ();
						}	
						seq.Reset ();
					});
				seqThread.Priority = ThreadPriority.Highest;
				seqThread.Name = seq.Name + "(" + seq.Pin + ")";
				sequenceThreads.Add (seqThread);
			}
		}

		//Version1
		private void MeasurementPreProcessing ()
		{

			#region Build Logger
			//build logger
			var logger = new CSVLogger (
				             string.Format (
					             "{0}_{1}.csv", 
					             DateTime.Now.ToShortTimeString (), 
					             "csv"),
				             true, 
				             false,
				             Configuration.LogFilePath
			             );
			logger.Mapping = CreateMapping ();
			logger.DateTimeFormat = "{0:mm:ss.ffff}"; 
			logger.FileName = 
				"/home/onkeliroh/Bachelorarbeit/Resources/Logs/" +
			string.Format ("{0}_{1}.csv", DateTime.Now.ToShortTimeString (), "csv");
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

							//pass values to logger
							var keys = new List<string> ();
							keys.AddRange (query.Select (x => x.DisplayName));
							keys.AddRange (combquery.Select (x => x.DisplayName));

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

		/// <summary>
		/// Creates the parameter mapping for the csv logger
		/// </summary>
		/// <returns>The mapping.</returns>
		private IDictionary<string,int> CreateMapping ()
		{
			var dict = new Dictionary<string,int> ();

			for (int i = 0; i < Configuration.AnalogPins.Count; i++)
			{
				dict.Add (Configuration.AnalogPins [i].DisplayName, i);
			}
			for (int i = Configuration.AnalogPins.Count; i < (Configuration.MeasurementCombinations.Count + Configuration.Pins.Count); i++)
			{
				dict.Add (Configuration.MeasurementCombinations [i].Name, i);
			}
			return dict;
		}

	}
}

