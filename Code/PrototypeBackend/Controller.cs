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

		private List<Task> sequenceThreads = new List<Task> ();

		public List<IPin> ControllerPins{ private set; get; }

		public List<Signal> ControllerSignals { private set; get; }

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
		public EventHandler<SignalsUpdatedArgs> SignalsUpdated;
		public EventHandler<ControllerPinUpdateArgs> PinsUpdated;
		public EventHandler<SequencesUpdatedArgs> SequencesUpdated;

		private bool running = false;

		public bool IsRunning { get { return running; } private set { } }

		public Controller ()
		{
			#if DEBUG
			ConfigManager = new ConfigurationManager ("/home/onkeliroh/Bachelorarbeit/Resources/Config.ini");
			BoardConfigs = ConfigManager.ParseBoards (ConfigManager.GeneralData.Sections ["General"].GetKeyData ("BoardPath").Value);
			ConLogger = new InfoLogger (ConfigManager.GeneralData.Sections ["General"].GetKeyData ("DiagnosticsPath").Value, true, false, LogLevel.DEBUG);
			ConLogger.LogToFile = true;
			ConLogger.Start ();
			#else
			ConLogger = new InfoLogger ("PrototypeBackendLog.txt", true, false, LogLevel.DEBUG);
			ConLogger.LogToFile = true;
			ConfigManager = new ConfigurationManager ();
			#endif

			ControllerSignals = new List<Signal> ();
			ControllerPins = new List<IPin> ();
			ControllerSequences = new List<Sequence> ();

			bool ConfigAutoConnect = Convert.ToBoolean (ConfigManager.GeneralData.Sections ["General"] ["AutoConnect"]);

			ArduinoController.AutoConnect = ConfigAutoConnect;
			ArduinoController.Init ();
			ArduinoController.NewAnalogValue += OnNewArduinoNewAnalogValue;
			ArduinoController.NewDigitalValue += OnNewArduinoNewDigitalValue;
			ArduinoController.OnConnectionChanged += ((o, e) =>
			{
				if (e.Connected)
				{
					#if DEBUG
					ConLogger.Log ("Connected to: " + ArduinoController.Board.ToString (), LogLevel.DEBUG);
					#endif
					#if RELEASE
					ConLogger.Log("Connected to " + ArduinoController.SerialPortName);
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

		public void AddSignal (Signal s)
		{
			if (!ControllerSignals.Contains (s))
			{
				if (s.SignalName == null || s.SignalName.Equals (string.Empty))
				{
					s.SignalName = "Signal" + (ControllerSignals.Count + 1);
				}
				ControllerSignals.Add (s);
			}

			if (SignalsUpdated != null)
			{
				SignalsUpdated.Invoke (this, new SignalsUpdatedArgs (UpdateOperation.Add, s));
			}
		}

		public void AddSignalRange (Signal[] s)
		{
			foreach (Signal sc in s)
			{
				if (!ControllerSignals.Contains (sc))
				{
					ControllerSignals.Add (sc);
				}
			}
			if (SignalsUpdated != null)
			{
				SignalsUpdated.Invoke (this, null);
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

		public void SetSignal (int index, Signal s)
		{
			if (!ControllerSignals.Contains (s))
			{
				AddSignal (s);
			} else
			{
				if (s != null)
				{
					SignalsUpdated.Invoke (this, new SignalsUpdatedArgs (UpdateOperation.Change, s));
				}
				ConLogger.Log ("Changed Signal from: " + ControllerPins [index] + " to " + s, LogLevel.DEBUG);
				ControllerSignals [index] = s;
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
				if (SequencesUpdated != null)
				{
					SequencesUpdated.Invoke (this, new SequencesUpdatedArgs (UpdateOperation.Change, ControllerSequences [index]));
				}
			}
		}

		#endregion

		#region Remove

		public void RemoveScheduler (Signal s)
		{
			ControllerSignals.Remove (s);
			if (SignalsUpdated != null)
			{
				SignalsUpdated.Invoke (this, null);
			}
		}

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

		public void RemoveSignal (int index)
		{
			if (index >= 0 && index < ControllerSignals.Count)
			{
				ConLogger.Log ("Removed Sequence: " + ControllerSignals [index], LogLevel.DEBUG);
				var sig = new Signal ();
				sig = ControllerSignals [index];
				ControllerSignals.RemoveAt (index);
				if (SignalsUpdated != null)
				{
					SignalsUpdated.Invoke (this, new SignalsUpdatedArgs (UpdateOperation.Remove, sig));
				}
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

		public void RemoveSchedulerRange (Signal[] s)
		{
			foreach (Signal sc in s)
			{
				ControllerSignals.Remove (sc);
			}
			if (SignalsUpdated != null)
			{
				SignalsUpdated.Invoke (this, null);
			}
		}

		#endregion

		#region Clear

		public void ClearScheduler ()
		{
			ControllerSignals.Clear ();
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

		public void ClearSignals ()
		{
			ControllerSignals.Clear ();

			if (SignalsUpdated != null)
			{
				SignalsUpdated.Invoke (this, new SignalsUpdatedArgs (UpdateOperation.Clear, null));
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

		public void Stop ()
		{
			running = false;

			while (sequenceThreads.Any (o => o.Status != TaskStatus.RanToCompletion))
			{
			}

			sequenceThreads.Clear ();

			ConLogger.Log ("Controller Stoped", LogLevel.DEBUG);
			ConLogger.Stop ();
			TimeKeeper.Stop ();
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
		}

		public System.Threading.ThreadState State ()
		{
			if (running)
			{
				return System.Threading.ThreadState.Running;
			} else
			{
				return System.Threading.ThreadState.Unstarted;
			}
		}

		private void BuildSequenceList ()
		{
			var sw = new Stopwatch ();
			sw.Start ();
			sequenceThreads.Clear ();
			foreach (Sequence seq in ControllerSequences)
			{
				var seqThread = new Task (
					                () =>
					{
						while (seq.Current () != null && running)
						{
							SequenceOperation op = (SequenceOperation)seq.Current ();
							if (StartTime <= DateTime.Now.Subtract (seq.lastOperation))
							{
								//TODO Zeit messen

								ArduinoController.SetPin (seq.Pin.Number, seq.Pin.Mode, op.State);
								seq.Next ();
								Thread.Sleep (op.Duration);
							}
						}	
						seq.Reset ();
						bool res = sequenceThreads.Any (o => o.Status != TaskStatus.RanToCompletion);
						running = !res;
					}
					, TaskCreationOptions.AttachedToParent);
				sequenceThreads.Add (seqThread);
			}
			ConLogger.Log ("BuildSequenceList took " + sw.ElapsedMilliseconds + "ms", LogLevel.DEBUG);
			sw.Stop ();
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

		#region IntelMethos

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

		public APin[] GetApinsWithoutSingal ()
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

		public Signal GetCorespondingSignal (APin pin)
		{
			foreach (Signal sig in ControllerSignals)
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

