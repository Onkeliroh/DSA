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

		public List<IPin> ControllerPins{ private set; get; }

		public List<MeasurementCombination> ControllerMeasurementCombinations { private set; get; }

		public List<Sequence> ControllerSequences;

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

		public int[] AvailableAnalogPins {
			private set{ }
			get {
				return GetUnusedPins (PinType.ANALOG);
			}
		}

		public int[] AvailableDigitalPins {
			private set{ }
			get {
				return GetUnusedPins (PinType.DIGITAL); 
			}
		}

		public Board[] BoardConfigs;

		public EventHandler<ControllerAnalogEventArgs> NewAnalogValue;
		public EventHandler<ControllerDigitalEventArgs> NewDigitalValue;
		public EventHandler<MeasurementCombinationsUpdatedArgs> SignalsUpdated;
		public EventHandler<ControllerPinUpdateArgs> PinsUpdated;
		public EventHandler<SequencesUpdatedArgs> SequencesUpdated;
		public EventHandler OnControllerStarted;
		public EventHandler OnControllerStoped;

		private bool running = false;

		public bool IsRunning { get { return running; } private set { } }

		public Controller ()
		{
			ConfigManager = new ConfigurationManager ("/home/onkeliroh/Bachelorarbeit/Resources/Config.ini");
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

			ControllerMeasurementCombinations = new List<MeasurementCombination> ();
			ControllerPins = new List<IPin> ();
			ControllerSequences = new List<Sequence> ();

			bool ConfigAutoConnect = Convert.ToBoolean (ConfigManager.GeneralData.Sections ["General"] ["AutoConnect"]);

			ArduinoController.AutoConnect = ConfigAutoConnect;
			ArduinoController.Init ();
			ArduinoController.NewAnalogValue += OnNewArduinoNewAnalogValue;
			ArduinoController.NewDigitalValue += OnNewArduinoNewDigitalValue;
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

			signalThread = new Thread (new ThreadStart (Run)){ Name = "controllerThread" };

			TimeKeeper = new Stopwatch ();
		}

		~Controller ()
		{
			ConLogger.Stop ();
		}

		#region EventDelegates

		private void OnNewArduinoNewAnalogValue (object sender, ControllerAnalogEventArgs args)
		{	
			if (this.NewAnalogValue != null)
			{
				this.NewAnalogValue.Invoke (this, args);
			}
		}

		private void OnNewArduinoNewDigitalValue (object sender, ControllerDigitalEventArgs args)
		{
			if (this.NewDigitalValue != null)
			{
				this.NewDigitalValue.Invoke (this, args);
			}
		}

		#endregion

		#region Add

		public void AddPin (IPin ip)
		{
			if (!ControllerPins.Contains (ip) && ip != null)
			{
				ControllerPins.Add (ip);
				ControllerPins = ControllerPins.OrderBy (x => x.Number).ThenBy (x => x.Type).ToList ();
				if (PinsUpdated != null)
				{
					PinsUpdated.Invoke (this, new ControllerPinUpdateArgs (ip, UpdateOperation.Add, ip.Type));
				}
				ConLogger.Log ("Added Pin: " + ip, LogLevel.DEBUG);
			}
		}

		public void AddMeasurementCombination (MeasurementCombination s)
		{
			if (!ControllerMeasurementCombinations.Contains (s))
			{
				ControllerMeasurementCombinations.Add (s);
			}

			if (SignalsUpdated != null)
			{
				SignalsUpdated.Invoke (this, new MeasurementCombinationsUpdatedArgs (UpdateOperation.Add, s));
			}
		}

		public void AddSequence (Sequence seq)
		{
			if (!ControllerSequences.Contains (seq))
			{
				if (seq.Name.Equals (string.Empty))
				{
					seq.Name = "Squence(" + seq.Pin.Name + " D" + seq.Pin.Number + " )";
				}
				ConLogger.Log ("Added Sequence: " + seq, LogLevel.DEBUG);
				ControllerSequences.Add (seq);
				if (SequencesUpdated != null)
				{
					SequencesUpdated.Invoke (this, new SequencesUpdatedArgs (UpdateOperation.Add, seq));
				}
				BuildSequenceList ();
			}
		}

		#endregion

		#region Set

		public void SetPin (int index, IPin ip)
		{
			if (index >= 0 && index < ControllerPins.Count)
			{
				ConLogger.Log ("Changed Pin from: " + ControllerPins [index] + " to " + ip, LogLevel.DEBUG);
				if (PinsUpdated != null)
				{
					PinsUpdated.Invoke (this, new ControllerPinUpdateArgs (ControllerPins [index], UpdateOperation.Change, ip.Type));
				}
				ControllerPins [index] = ip;
				if (PinsUpdated != null)
				{
					PinsUpdated.Invoke (this, new ControllerPinUpdateArgs (ip, UpdateOperation.Change, ip.Type));
				}
			} else
			{
				throw new IndexOutOfRangeException ();
			}
		}

		public void SetMeasurmentCombination (int index, MeasurementCombination s)
		{
			if (index >= 0 && index < ControllerMeasurementCombinations.Count)
			{
				AddMeasurementCombination (s);
			} else
			{
				if (s != null)
				{
					SignalsUpdated.Invoke (this, new MeasurementCombinationsUpdatedArgs (UpdateOperation.Change, s));
				}
				ConLogger.Log ("Changed Measurement Combination from: " + ControllerMeasurementCombinations [index] + " to " + s, LogLevel.DEBUG);
				ControllerMeasurementCombinations [index] = s;
			}
		}

		public void SetSequence (int index, Sequence seq)
		{
			if (index >= 0 && index < ControllerSequences.Count)
			{
				ConLogger.Log ("Changed Sequence from: " + ControllerSequences [index] + " to " + seq, LogLevel.DEBUG);
				if (SequencesUpdated != null)
				{
					SequencesUpdated.Invoke (this, new SequencesUpdatedArgs (UpdateOperation.Change, ControllerSequences [index]));
				}
				ControllerSequences [index] = seq;
				BuildSequenceList ();
			}
		}

		#endregion

		#region Remove

		public void RemovePin (string name)
		{
			var result = ControllerPins.Where (o => o.Name == name).ToList<IPin> ();

			if (result.Count > 0)
			{
				ControllerPins.Remove (result [0]);
				if (PinsUpdated != null)
				{
					PinsUpdated.Invoke (this, new ControllerPinUpdateArgs (result [0], UpdateOperation.Remove, result [0].Type));
				}
				ConLogger.Log ("Removed Pin: " + result [0], LogLevel.DEBUG);
			}
		}

		public void RemovePin (int index)
		{
			if (index >= 0 && index < ControllerPins.Count)
			{
				if (PinsUpdated != null)
				{
					PinsUpdated.Invoke (this, new ControllerPinUpdateArgs (ControllerPins [index], UpdateOperation.Remove, ControllerPins [index].Type));
				}
				ConLogger.Log ("Removed Pin: " + ControllerPins [index], LogLevel.DEBUG);
				ControllerPins.RemoveAt (index);
			}
		}

		public void RemoveMeasurementCombination (int index)
		{
			var sig = new MeasurementCombination ();
			sig = ControllerMeasurementCombinations [index];
			ControllerMeasurementCombinations.RemoveAt (index);

			ConLogger.Log ("Removed Measurement Combination: " + sig, LogLevel.DEBUG);
			if (SignalsUpdated != null)
			{
				SignalsUpdated.Invoke (this, new MeasurementCombinationsUpdatedArgs (UpdateOperation.Remove, sig));
			}
		}

		public void RemoveSequence (string name)
		{
			var result = ControllerSequences.Where (o => o.Name == name).ToList<Sequence> ();
			if (result.Count > 0)
			{
				ConLogger.Log ("Removed Sequence: " + result [0], LogLevel.DEBUG);
				ControllerSequences.Remove (result [0]);
				if (SequencesUpdated != null)
				{
					SequencesUpdated.Invoke (this, new SequencesUpdatedArgs (UpdateOperation.Remove, result [0]));
				}
			}
		}

		public void RemoveSequence (int index)
		{
			if (index >= 0 && index < ControllerSequences.Count)
			{
				ConLogger.Log ("Removed Sequence: " + ControllerSequences [index], LogLevel.DEBUG);
				var seq = new Sequence ();
				seq = ControllerSequences [index];
				ControllerSequences.RemoveAt (index);
				if (SequencesUpdated != null)
				{
					SequencesUpdated.Invoke (this, new SequencesUpdatedArgs (UpdateOperation.Remove, seq));
				}
			}
		}

		#endregion

		#region Clear

		public void ClearScheduler ()
		{
			ControllerMeasurementCombinations.Clear ();
			if (SignalsUpdated != null)
			{
				SignalsUpdated.Invoke (this, null);
			}
		}

		public void ClearPins (PinType type)
		{
			ControllerPins.RemoveAll (o => o.Type == type);
			ConLogger.Log ("Cleared Pins", LogLevel.DEBUG);
			if (PinsUpdated != null)
			{
				PinsUpdated.Invoke (this, new ControllerPinUpdateArgs (null, UpdateOperation.Clear, type));
			}
		}

		public void ClearPins ()
		{
			ControllerPins.Clear ();
		}

		public void ClearMeasurementCombinations ()
		{
			ControllerMeasurementCombinations.Clear ();

			if (SignalsUpdated != null)
			{
				SignalsUpdated.Invoke (this, new MeasurementCombinationsUpdatedArgs (UpdateOperation.Clear, null));
			}
		}

		public void ClearSequences ()
		{
			ControllerSequences.Clear ();
			if (SequencesUpdated != null)
			{
				SequencesUpdated.Invoke (this, new SequencesUpdatedArgs (UpdateOperation.Clear));
			}
		}

		#endregion

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
//			sequenceThreads.ForEach (x => x.Wait (new CancellationToken (true)));
//			BuildSequenceList ();
		}

		public void Start ()
		{
			TimeKeeper.Restart ();

			if (CheckSignals ())
			{
				running = true;
				BuildSequenceList ();
				StartTime = DateTime.Now;
				sequenceThreads.ForEach (o => o.Start ());
//				signalThread.Start ();
				ConLogger.Log ("Controller Started", LogLevel.DEBUG);
			}
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
			foreach (Sequence seq in ControllerSequences)
			{
				var seqThread = new Thread (
					                () =>
					{
						var op = seq.Current ();
						while (seq.CurrentState != SequenceState.Done && running && op != null)
						{
							ArduinoController.SetPin (seq.Pin.Number, seq.Pin.Mode, ((SequenceOperation)op).State);
							Thread.Sleep (((SequenceOperation)op).Duration);
							op = seq.Next ();
						}	
						ConLogger.Log (seq.Name + "exiting", LogLevel.DEBUG);
						seq.Reset ();
					});
				seqThread.Priority = ThreadPriority.Highest;
				seqThread.Name = seq.Name + "(" + seq.Pin + ")";
				sequenceThreads.Add (seqThread);
			}
		}

		public bool CheckSignals ()
		{
			//TODO implement
			return true;
		}

		private void Run ()
		{
			//TODO signal verarbeitung
		}

		#region IntelMethods

		public int[] GetUsedPins (PinType type)
		{
			return ControllerPins.Where (o => o.Type == type).Select (o => o.Number).ToArray<int> (); 
		}

		public DPin[] GetDPinsWithoutSequence ()
		{
			var pins = ControllerPins.Where (o => o.Type == PinType.DIGITAL).ToList<IPin> ();

			pins.RemoveAll (o => ControllerSequences.Select (s => s.Pin).Contains (o));

			DPin[] array = new DPin[pins.Count ];
			for (int i = 0; i < array.Length; i++)
			{
				array [i] = (pins [i] as DPin);
			}

			return array;
		}

		public APin[] GetApinsWithoutCombination ()
		{
			var pins = ControllerPins.Where (o => o.Type == PinType.ANALOG).ToList<IPin> ();

			APin[] array = new APin[pins.Count];

			for (int i = 0; i < array.Length; i++)
			{
				array [i] = (pins [i] as APin);
			}
			return array;
		}

		public int[] GetUnusedPins (PrototypeBackend.PinType type)
		{
			uint numpins = 0;
			List<int> unusedpins = new List<int> ();

			if (type.Equals (PrototypeBackend.PinType.ANALOG))
			{
				numpins = ArduinoController.NumberOfAnalogPins; 
				for (int i = 0; i < numpins; i++)
				{
					unusedpins.Add (i);
				}
			} else if (type.Equals (PrototypeBackend.PinType.DIGITAL))
			{
				numpins = ArduinoController.NumberOfDigitalPins;
				for (int i = 0; i < numpins; i++)
				{
					unusedpins.Add (i);
				}
			}

			foreach (IPin ip in ControllerPins)
			{
				if (ip.Type == type)
				{
					unusedpins.Remove (ip.Number);
				}
			}

			return unusedpins.ToArray ();
		}

		public MeasurementCombination GetCorespondingCombination (APin pin)
		{
			foreach (MeasurementCombination sig in ControllerMeasurementCombinations)
			{
				if (sig.Pins.Contains (pin))
				{
					return sig;
				}
			}
			return null;
		}

		public Sequence GetCorespondingSequence (DPin pin)
		{
			foreach (Sequence seq in ControllerSequences)
			{
				if (seq.Pin == pin)
				{
					return seq;
				}
			}
			return null;
		}

		#endregion
	}
}

