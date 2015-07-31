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

		private Thread signalThread;

		//		private Thread sequenceThread;

		private List<Task> sequenceThreads = new List<Task> ();

		public List<IPin> ControllerPins{ private set; get; }

		public List<Signal> ControllerSignalList { private set; get; }

		public List<Sequence> ControlSequences;

		private DateTime StartTime;

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

		public EventHandler<ControllerAnalogEventArgs> NewAnalogValue;
		public EventHandler<ControllerDigitalEventArgs> NewDigitalValue;
		public EventHandler SignalListUpdated;
		public EventHandler<ControllerPinUpdateArgs> PinsUpdated;
		public EventHandler<ControllerSequenceUpdateArgs> SequencesUpdated;

		private bool running = false;

		public bool IsRunning { get { return running; } private set { } }

		public Controller ()
		{
			ConLogger = new InfoLogger ("PrototypeBackendLog.txt", true, false, LogLevel.DEBUG);
			ConLogger.LogToFile = false;

			ControllerSignalList = new List<Signal> ();
			ControllerPins = new List<IPin> ();
			ControlSequences = new List<Sequence> ();

			ArduinoController.Init ();
			ArduinoController.NewAnalogValue += OnNewArduinoNewAnalogValue;
			ArduinoController.NewDigitalValue += OnNewArduinoNewDigitalValue;
			ArduinoController.OnConnection += ((o, e) =>
			{
				ArduinoController.GetNumberAnalogPins ();
				ArduinoController.GetNumberDigitalPins ();
				ArduinoController.GetVersion ();
				ArduinoController.GetModel ();
			});

			signalThread = new Thread (new ThreadStart (Run)){ Name = "controllerThread" };
		}

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

		public void AddSignal (Signal s)
		{
			if (!ControllerSignalList.Contains (s))
			{
				ControllerSignalList.Add (s);
			}

			if (SignalListUpdated != null)
			{
				SignalListUpdated.Invoke (this, null);
			}
		}

		public void AddPin (IPin ip)
		{
			if (!ControllerPins.Contains (ip) && ip != null)
			{
				ControllerPins.Add (ip);
				if (PinsUpdated != null)
				{
					PinsUpdated.Invoke (this, new ControllerPinUpdateArgs (ip, UpdateOperation.Add, ip.Type));
				}
				ConLogger.Log ("Added Pin: " + ip, LogLevel.DEBUG);
			}
		}

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

		public void AddSequence (Sequence seq)
		{
			if (!ControlSequences.Contains (seq))
			{
				ConLogger.Log ("Added Sequence: " + seq, LogLevel.DEBUG);
				ControlSequences.Add (seq);
				if (SequencesUpdated != null)
				{
					SequencesUpdated.Invoke (this, new ControllerSequenceUpdateArgs (UpdateOperation.Add, seq));
				}
			}
		}

		public void SetSequence (int index, Sequence seq)
		{
			if (index >= 0 && index < ControlSequences.Count)
			{
				ConLogger.Log ("Changed Sequence from: " + ControlSequences [index] + " to " + seq, LogLevel.DEBUG);
				if (SequencesUpdated != null)
				{
					SequencesUpdated.Invoke (this, new ControllerSequenceUpdateArgs (UpdateOperation.Change, ControlSequences [index]));
				}
				ControlSequences [index] = seq;
				if (SequencesUpdated != null)
				{
					SequencesUpdated.Invoke (this, new ControllerSequenceUpdateArgs (UpdateOperation.Change, ControlSequences [index]));
				}
			}
		}

		public void AddSignalRange (Signal[] s)
		{
			foreach (Signal sc in s)
			{
				if (!ControllerSignalList.Contains (sc))
				{
					ControllerSignalList.Add (sc);
				}
			}
			if (SignalListUpdated != null)
			{
				SignalListUpdated.Invoke (this, null);
			}
		}

		public void RemoveScheduler (Signal s)
		{
			ControllerSignalList.Remove (s);
			if (SignalListUpdated != null)
			{
				SignalListUpdated.Invoke (this, null);
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

		public void RemoveSequence (string name)
		{
			var result = ControlSequences.Where (o => o.Name == name).ToList<Sequence> ();
			if (result.Count > 0)
			{
				if (SequencesUpdated != null)
				{
					SequencesUpdated.Invoke (this, new ControllerSequenceUpdateArgs (UpdateOperation.Remove, result [0]));
				}
				ConLogger.Log ("Removed Sequence: " + result [0], LogLevel.DEBUG);
				ControlSequences.Remove (result [0]);
			}
		}

		public void RemoveSequence (int index)
		{
			if (index >= 0 && index < ControlSequences.Count)
			{
				if (SequencesUpdated != null)
				{
					SequencesUpdated.Invoke (this, new ControllerSequenceUpdateArgs (UpdateOperation.Remove, ControlSequences [index]));
				}
				ConLogger.Log ("Removed Sequence: " + ControlSequences [index], LogLevel.DEBUG);
				ControlSequences.RemoveAt (index);
			}
		}

		public void RemoveSchedulerRange (Signal[] s)
		{
			foreach (Signal sc in s)
			{
				ControllerSignalList.Remove (sc);
			}
			if (SignalListUpdated != null)
			{
				SignalListUpdated.Invoke (this, null);
			}
		}

		public void ClearScheduler ()
		{
			ControllerSignalList.Clear ();
			if (SignalListUpdated != null)
			{
				SignalListUpdated.Invoke (this, null);
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

		public void Stop ()
		{
			running = false;

			while (sequenceThreads.Any (o => o.Status != TaskStatus.RanToCompletion))
			{
			}

			sequenceThreads.Clear ();

			ConLogger.Log ("Controller Stoped", LogLevel.DEBUG);
		}

		public void Start ()
		{
			var sw = new Stopwatch ();
			sw.Start ();
			if (CheckSignals ())
			{
				running = true;
				BuildSequenceList ();
				StartTime = DateTime.Now;
				sequenceThreads.ForEach (o => o.Start ());
//				signalThread.Start ();
				ConLogger.Log ("Controller Started", LogLevel.DEBUG);
			}
			ConLogger.Log ("Start took: " + sw.ElapsedMilliseconds + "ms", LogLevel.DEBUG);
			sw.Stop ();
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
			foreach (Sequence seq in ControlSequences)
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

		public int[] GetUsedPins (PinType type)
		{
			return ControllerPins.Where (o => o.Type == type).Select (o => o.Number).ToArray<int> (); 
		}

		public DPin[] GetDPinsWithoutSequence ()
		{
			var pins = ControllerPins.Where (o => o.Type == PinType.DIGITAL).ToList<IPin> ();

			pins.RemoveAll (o => ControlSequences.Select (s => s.Pin).Contains (o));

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
	}
}

