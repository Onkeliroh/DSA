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
		public InfoLogger ConLogger { get; private set; }

		public ConfigurationManager ConfigManager { get; private set; }

		private Thread signalThread;

		//		private Thread sequenceThread;

		private List<Thread> sequenceThreads = new List<Thread> ();

		public BoardConfiguration Configuration;

		public DateTime StartTime;

		public TimeSpan TimePassed { 
			get {
				if (TimeKeeper != null) {
					return TimeKeeper.Elapsed;
				} else {
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

		public Controller ()
		{
			Configuration = new BoardConfiguration ();

//			ConfigManager = new ConfigurationManager ("/home/onkeliroh/Bachelorarbeit/Resources/Config.ini");
			ConfigManager = new ConfigurationManager ();
			#if DEBUG
			BoardConfigs = ConfigManager.ParseBoards (ConfigManager.GeneralData.Sections ["General"].GetKeyData ("BoardPath").Value);
			ConLogger = new InfoLogger (ConfigManager.GeneralData.Sections ["General"].GetKeyData ("DiagnosticsPath").Value, true, false, LogLevel.DEBUG);
			ConLogger.LogToFile = true;
			ConLogger.DateTimeFormat = "{0:mm:ss.fffff}";
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
			ArduinoController.OnConnectionChanged += ((o, e) => {
				if (e.Connected) {
					#if DEBUG
					ConLogger.Log ("Connected to: " + ArduinoController.Board.ToString (), LogLevel.DEBUG);
					#endif
					#if RELEASE
					ConLogger.Log ("Connected to " + ArduinoController.SerialPortName);
					#endif

				} else {
					ConLogger.Log ("Disconnected");
				}
			});

			Configuration.OnPinsUpdated += (o, e) => {
				if (e.UpdateOperation == UpdateOperation.Change)
					ConLogger.Log ("Pin Update: [" + e.UpdateOperation + "] " + e.Pin + " to " + e.Pin2);
				else
					ConLogger.Log ("Pin Update: [" + e.UpdateOperation + "] " + e.Pin);
			};

			Configuration.OnSequencesUpdated += (o, e) => {
				if (e.UpdateOperation == UpdateOperation.Change)
					ConLogger.Log ("Sequence Update: [" + e.UpdateOperation + "] " + e.Seq + " to " + e.Seq);
				else
					ConLogger.Log ("Sequence Update: [" + e.UpdateOperation + "] " + e.Seq);

				BuildSequenceList ();
			};
			Configuration.OnSignalsUpdated += (o, e) => {
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

			if (OnControllerStoped != null) {
				OnControllerStoped.Invoke (this, null);
			}
		}

		public void Start ()
		{
			TimeKeeper.Restart ();

			running = true;
			BuildSequenceList ();
			StartTime = DateTime.Now;
			sequenceThreads.ForEach (o => o.Start ());
			ConLogger.Log ("Controller Started", LogLevel.DEBUG);
			ConLogger.Log ("Start took: " + TimeKeeper.ElapsedMilliseconds + "ms", LogLevel.DEBUG);

			if (OnControllerStarted != null) {
				OnControllerStarted.Invoke (this, null);
			}
		}

		private void BuildSequenceList ()
		{
			sequenceThreads.Clear ();
			GC.Collect ();
			foreach (Sequence seq in Configuration.Sequences) {
				var seqThread = new Thread (
					                () => {
						var logger = new InfoLogger (ConfigManager.GeneralData.Sections ["General"].GetKeyData ("DiagnosticsPath").Value + seq.Pin.Number, true, false);
						logger.LogToFile = true;
						logger.Start ();
						Stopwatch sw = new Stopwatch ();
						sw.Start ();

						uint pin = seq.Pin.Number;
						PinMode mode = seq.Pin.Mode;
						var op = seq.Current ();
						while (seq.CurrentState != SequenceState.Done && running && op != null) {
							logger.Log (sw.ElapsedMilliseconds.ToString () + "; begin");
							ArduinoController.SetPin (pin, mode, ((SequenceOperation)op).State);
							logger.Log (sw.ElapsedMilliseconds.ToString () + "; after send");
							Thread.Sleep (((SequenceOperation)op).Duration);
							logger.Log (sw.ElapsedMilliseconds.ToString () + "; after sleep");
							op = seq.Next ();
							logger.Log (sw.ElapsedMilliseconds.ToString () + "; after next");
						}	
						ConLogger.Log (seq.Name + "exiting", LogLevel.DEBUG);
						seq.Reset ();
						logger.Stop ();
					});
				seqThread.Priority = ThreadPriority.Highest;
				seqThread.Name = seq.Name + "(" + seq.Pin + ")";
				sequenceThreads.Add (seqThread);
			}
		}

	}
}

