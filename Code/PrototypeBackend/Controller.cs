using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;

namespace PrototypeBackend
{
	public class Controller
	{
		private Thread controllerThread;

		public List<MeasurementDate> controllerMeasurementDateList{ private set; get; }

		public List<Sequence> controllerSequenceDateList{ private set; get; }

		public static List<List<double>> analogList;
		public static List<List<ArduinoController.DPinState>> digitalList;

		public int[] AvailableAnalogPins {
			private set{ }
			get {
				return GetUnusedPins (ArduinoController.PinType.ANALOG);
			}
		}

		public int[] AvailableDigitalPins {
			private set{ }
			get {
				return GetUnusedPins (ArduinoController.PinType.DIGITAL); 
			}
		}

		public EventHandler<ControllerAnalogEventArgs> NewAnalogValue;
		public EventHandler<ControllerDigitalEventArgs> NewDigitalValue;
		public EventHandler MeasurementDateListUpdated;
		public EventHandler SequenceDateListUpdated;

		private bool running = true;

		public ArduinoController.ArduinoController ArduinoController_ = new ArduinoController.ArduinoController ();

		public Controller ()
		{
			controllerMeasurementDateList = new List<MeasurementDate> ();
			controllerSequenceDateList = new List<Sequence> ();

			ArduinoController_.NewAnalogValue += OnNewArduinoNewAnalogValue;
			ArduinoController_.NewDigitalValue += OnNewArduinoNewDigitalValue;

			controllerThread = new Thread (new ThreadStart (Run)){ Name = "controllerThread" };
			controllerThread.Start ();
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

		public void AddMeasurementDate (MeasurementDate md)
		{
			controllerMeasurementDateList.Add (md);
			controllerMeasurementDateList = controllerMeasurementDateList.OrderBy (o => o.DueTime).ToList ();
			if (MeasurementDateListUpdated != null)
			{
				MeasurementDateListUpdated.Invoke (this, null);
			}
		}

		public void AddSequenceDate (Sequence seq)
		{
			controllerSequenceDateList.Add (seq);
			controllerSequenceDateList = controllerSequenceDateList.OrderBy (o => o.DueTime).ToList ();
			if (SequenceDateListUpdated != null)
			{
				SequenceDateListUpdated.Invoke (this, null);
			}
		}

		public void AddMeasurementDateRange (MeasurementDate[] md)
		{
			controllerMeasurementDateList.AddRange (md);
			controllerMeasurementDateList = controllerMeasurementDateList.OrderBy (o => o.DueTime).ToList ();
			if (MeasurementDateListUpdated != null)
			{
				MeasurementDateListUpdated.Invoke (this, null);
			}
		}

		public void AddSequenceDateRange (Sequence[] seq)
		{
			controllerSequenceDateList.AddRange (seq);
			controllerSequenceDateList = controllerSequenceDateList.OrderBy (o => o.DueTime).ToList ();
			if (SequenceDateListUpdated != null)
			{
				SequenceDateListUpdated.Invoke (this, null);
			}
		}

		public void RemoveMeasurementDate (MeasurementDate md)
		{
			controllerMeasurementDateList.RemoveAt (controllerMeasurementDateList.IndexOf (md));
			if (MeasurementDateListUpdated != null)
			{
				MeasurementDateListUpdated.Invoke (this, null);
			}
		}

		public void RemoveSequenceDate (Sequence seq)
		{
			controllerSequenceDateList.RemoveAt (controllerSequenceDateList.IndexOf (seq));
			if (SequenceDateListUpdated != null)
			{
				SequenceDateListUpdated.Invoke (this, null);
			}
		}

		public void RemoveMeasurementDate (int position)
		{
			controllerMeasurementDateList.RemoveAt (position);
			if (MeasurementDateListUpdated != null)
			{
				MeasurementDateListUpdated.Invoke (this, null);
			}
		}

		public void RemoveSequenceDate (int position)
		{
			controllerSequenceDateList.RemoveAt (position);
			if (SequenceDateListUpdated != null)
			{
				SequenceDateListUpdated.Invoke (this, null);
			}
		}

		public void RemoveMeasurementDateRange (MeasurementDate[] md)
		{
			int pos;
			foreach (MeasurementDate MD in md)
			{
				pos = controllerMeasurementDateList.IndexOf (MD);
				if (pos != -1 && pos >= 0 && pos < controllerMeasurementDateList.Count)
				{
					controllerMeasurementDateList.RemoveAt (pos);
				}
			}
			if (MeasurementDateListUpdated != null)
			{
				MeasurementDateListUpdated.Invoke (this, null);
			}
		}

		public void RemoveSequenceDateRange (Sequence[] seq)
		{
			int pos;
			foreach (Sequence SEQ in seq)
			{
				pos = controllerSequenceDateList.IndexOf (SEQ);
				if (pos != -1 && pos >= 0 && pos < controllerSequenceDateList.Count)
				{
					controllerSequenceDateList.RemoveAt (pos);
				}
			}
			if (SequenceDateListUpdated != null)
			{
				SequenceDateListUpdated.Invoke (this, null);
			}
		}

		public void ClearMeasurementDate ()
		{
			controllerMeasurementDateList.Clear ();
			if (MeasurementDateListUpdated != null)
			{
				MeasurementDateListUpdated.Invoke (this, null);
			}
		}

		public void ClearSequenceDate ()
		{
			controllerSequenceDateList.Clear ();
			if (SequenceDateListUpdated != null)
			{
				SequenceDateListUpdated.Invoke (this, null);
			}
		}

		public void Stop ()
		{
			running = false;
		}

		private void Run ()
		{
			//todo sequences berücksichtigen
			while (running)
			{
				if (controllerMeasurementDateList.Count > 0)
				{
					if (controllerMeasurementDateList [0].DueTime.Subtract (DateTime.Now).TotalMilliseconds < 10)
					{
						#if DEBUG
						Console.WriteLine (DateTime.Now + ":\t" + controllerMeasurementDateList [0].ToString ());
						#endif
						#if !FAKESERIAL
						switch (controllerMeasurementDateList [0].PinCmd)
						{
						case ArduinoController.Command.ReadAnalogPin:
							ArduinoController_.ReadAnalogPin (controllerMeasurementDateList [0].PinNr);
							break;
						}
						#endif
						RemoveMeasurementDate (0);
					}
				}
				Thread.Sleep (10);
			}
		}

		private int[] GetUsedPins (ArduinoController.PinType type)
		{
			List<int> pins = new List<int> ();


			foreach (MeasurementDate md in this.controllerMeasurementDateList)
			{
				if (md.PinType == type)
				{
					if (!pins.Contains (md.PinNr))
					{
						pins.Add (md.PinNr);
					}
				}
			}
			return pins.ToArray ();
		}

		private int[] GetUnusedPins (ArduinoController.PinType type)
		{
			List<int> pins = new List<int> ();

			if (ArduinoController.PinType.ANALOG == type)
			{
				for (int i = 0; i < ArduinoController_.NumberOfAnalogPins; i++)
				{
					pins.Add (i);
				}
			} else if (ArduinoController.PinType.DIGITAL == type)
			{
				for (int i = 0; i < ArduinoController_.NumberOfDigitalPins; i++)
				{
					pins.Add (i);
				}
			}

			foreach (MeasurementDate md in this.controllerMeasurementDateList)
			{
				if (md.PinType == type)
				{
					if (pins.Contains (md.PinNr))
					{
						pins.Remove (md.PinNr);
					}
				}
			}

			return pins.ToArray ();
		}
	}
}

