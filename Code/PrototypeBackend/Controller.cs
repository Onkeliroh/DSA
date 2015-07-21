using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using PrototypeBackend;

namespace PrototypeBackend
{
	public class Controller
	{
		private Thread signalThread;

		//		private Thread sequenceThread;

		private List<Thread> sequenceThreads = new List<Thread> ();

		public List<IPin> ControllerPins{ private set; get; }

		public List<Scheduler> ControllerSchedulerList { private set; get; }

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
		public EventHandler SchedulerListUpdated;
		public EventHandler<ControllerPinUpdateArgs> PinsUpdated;

		private bool running = true;

		public Controller ()
		{
			ArduinoController.Init ();
			ControllerSchedulerList = new List<Scheduler> ();
			ControllerPins = new List<IPin> ();
			ControlSequences = new List<Sequence> ();

			ArduinoController.NewAnalogValue += OnNewArduinoNewAnalogValue;
			ArduinoController.NewDigitalValue += OnNewArduinoNewDigitalValue;

			signalThread = new Thread (new ThreadStart (Run)){ Name = "controllerThread" };

//			sequenceThread = new Thread (new ThreadStart (ManageSequence)){ Name = "sequenceThread" };

			ArduinoController.OnConnection += ((o, e) => {
				ArduinoController.GetNumberAnalogPins ();
				ArduinoController.GetNumberDigitalPins ();
				ArduinoController.GetVersion ();
				ArduinoController.GetModel ();
			});
		}

		private void OnNewArduinoNewAnalogValue (object sender, ControllerAnalogEventArgs args)
		{	
			if (this.NewAnalogValue != null) {
				this.NewAnalogValue.Invoke (this, args);
			}
		}

		private void OnNewArduinoNewDigitalValue (object sender, ControllerDigitalEventArgs args)
		{
			if (this.NewDigitalValue != null) {
				this.NewDigitalValue.Invoke (this, args);
			}
		}

		public void AddScheduler (Scheduler s)
		{
			if (!ControllerSchedulerList.Contains (s)) {
				ControllerSchedulerList.Add (s);
				ControllerSchedulerList = ControllerSchedulerList.OrderBy (o => o.DueTime).ToList ();
			}

			if (SchedulerListUpdated != null) {
				SchedulerListUpdated.Invoke (this, null);
			}
		}

		public void AddPin (IPin ip)
		{
			if (!ControllerPins.Contains (ip) && ip != null) {
				ControllerPins.Add (ip);
				if (PinsUpdated != null) {
					PinsUpdated.Invoke (this, new ControllerPinUpdateArgs (ip, PinUpdateOperation.Add));
				}
			}
		}

		public void AddSchedulerRange (Scheduler[] s)
		{
			foreach (Scheduler sc in s) {
				if (!ControllerSchedulerList.Contains (sc)) {
					ControllerSchedulerList.Add (sc);
				}
			}
			ControllerSchedulerList = ControllerSchedulerList.OrderBy (o => o.DueTime).ToList ();
			if (SchedulerListUpdated != null) {
				SchedulerListUpdated.Invoke (this, null);
			}
		}

		public void RemoveScheduler (Scheduler s)
		{
			ControllerSchedulerList.Remove (s);
			if (SchedulerListUpdated != null) {
				SchedulerListUpdated.Invoke (this, null);
			}
		}

		public void RemoveSchedulerRange (Scheduler[] s)
		{
			foreach (Scheduler sc in s) {
				ControllerSchedulerList.Remove (sc);
			}
			if (SchedulerListUpdated != null) {
				SchedulerListUpdated.Invoke (this, null);
			}
		}

		public void ClearScheduler ()
		{
			ControllerSchedulerList.Clear ();
			if (SchedulerListUpdated != null) {
				SchedulerListUpdated.Invoke (this, null);
			}
		}

		public void ClearPins (PinType type)
		{
			ControllerPins.RemoveAll (o => o.Type == type);
			if (PinsUpdated != null) {
				PinsUpdated.Invoke (this, new ControllerPinUpdateArgs (null, PinUpdateOperation.Remove));
			}
		}

		public void Stop ()
		{
			sequenceThreads.ForEach (o => o.Abort ());
			running = false;
		}

		public void Start ()
		{

			if (CheckSignals ()) {
				BuildSequenceList ();
				running = true;
				StartTime = DateTime.Now;
				sequenceThreads.ForEach (o => o.Start ());
				signalThread.Start ();
			}
		}

		private void BuildSequenceList ()
		{
			sequenceThreads.Clear ();
			foreach (Sequence seq in ControlSequences) {
				var seqThread = new Thread (
					                new ThreadStart (() => {
						SequenceOperation op = (SequenceOperation)seq.Current ();
						while (seq.Current () != null) {
							op = (SequenceOperation)seq.Current ();
							if (StartTime <= DateTime.Now.Subtract (seq.lastOperation)) {
								//TODO Zeit messen

								ArduinoController.SetPin (seq.Pin.Number, seq.Pin.Mode, op.State);
								seq.Next ();
								Thread.Sleep (op.Duration);
							}
						}	
					}
					                )){ Name = seq.Name + "_Thread" };

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
			//todo sequences berücksichtigen
			while (running) {
				if (ControllerSchedulerList.Count > 0) {
					if (DateTime.Now.Subtract (ControllerSchedulerList [0].DueTime).TotalMilliseconds < 1) {
						if (ControllerSchedulerList [0].Run () == true) {
							ControllerSchedulerList.RemoveAt (0);
						} else {
							ControllerSchedulerList = ControllerSchedulerList.OrderBy (o => o.DueTime).ToList ();
						}
					}
				}
			}
		}

		//		private void ManageSequence ()
		//		{
		//			while (running)
		//			{
		//				foreach (Sequence seq in ControlSequences)
		//				{
		//					if (seq.Current () != null)
		//					{
		//						SequenceOperation op = (SequenceOperation)seq.Current ();
		//						if (StartTime <= DateTime.Now.Subtract (op.Time))
		//						{
		//							Console.Write (DateTime.Now + "\t");
		//							ArduinoController.SetPin (seq.Pin.Number, seq.Pin.Mode, op.State);
		//							seq.Next ();
		//							Thread.Sleep (op.Duration);
		//						}
		//					}
		//				}
		//			}
		//		}

		public int[] GetUsedPins (PrototypeBackend.PinType type)
		{
			return ControllerPins.Where (o => o.Type == type).Select (o => o.Number).ToArray<int> (); 
		}

		public int[] GetUnusedPins (PrototypeBackend.PinType type)
		{
			uint numpins = 0;
			List<int> unusedpins = new List<int> ();

			if (type.Equals (PrototypeBackend.PinType.ANALOG)) {
				numpins = ArduinoController.NumberOfAnalogPins; 
				for (int i = 0; i < numpins; i++) {
					unusedpins.Add (i);
				}
				foreach (APin ip in ControllerPins) {
					Console.WriteLine ("Checking: " + ip);
					if (ip.Type == type) {
						unusedpins.Remove (ip.Number);
					}
				}
			} else if (type.Equals (PrototypeBackend.PinType.DIGITAL)) {
				numpins = ArduinoController.NumberOfDigitalPins;
				for (int i = 0; i < numpins; i++) {
					unusedpins.Add (i);
				}
				foreach (DPin ip in ControllerPins) {
					Console.WriteLine ("Checking: " + ip);
					if (ip.Type == type) {
						unusedpins.Remove (ip.Number);
					}
				}
			}
			return unusedpins.ToArray ();
		}
	}
}

