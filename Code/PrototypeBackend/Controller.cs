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

		public static List<List<double>> analogList;
		public static List<List<ArduinoController.DPinState>> digitalList;

		public int[] AvailableAnalogPins {
			private set{ }
			get {
				return GetUnusedPins (ArduinoController.PinType.ANALOG);
			}
		}

		public EventHandler<ControllerAnalogEventArgs> NewAnalogValue;
		public EventHandler<ControllerDigitalEventArgs> NewDigitalValue;
		public EventHandler MeasurementDateListUpdated;

		private bool running = true;

		public ArduinoController.ArduinoController ArduinoController_ = new ArduinoController.ArduinoController ();

		public Controller ()
		{
			controllerMeasurementDateList = new List<MeasurementDate> ();

			ArduinoController_.NewAnalogValue += OnNewArduinoNewAnalogValue;
			ArduinoController_.NewDigitalValue += OnNewArduinoNewDigitalValue;

			controllerThread = new Thread (new ThreadStart (Run)){ Name = "controllerThread" };
			controllerThread.Start ();
		}

		private void OnNewArduinoNewAnalogValue (object sender, ControllerAnalogEventArgs args)
		{
			this.NewAnalogValue.Invoke (this, args);
		}

		private void OnNewArduinoNewDigitalValue (object sender, ControllerDigitalEventArgs args)
		{
			this.NewDigitalValue.Invoke (this, args);
		}

		public void AddMeasurementDate (MeasurementDate md)
		{
			controllerMeasurementDateList.Add (md);
			controllerMeasurementDateList = controllerMeasurementDateList.OrderBy (o => o.DueTime).ToList ();
			MeasurementDateListUpdated.Invoke (this, null);
		}

		public void AddMeasurementDateRange (MeasurementDate[] md)
		{
			controllerMeasurementDateList.AddRange (md);
			controllerMeasurementDateList = controllerMeasurementDateList.OrderBy (o => o.DueTime).ToList ();
			MeasurementDateListUpdated.Invoke (this, null);
		}

		public void RemoveMeasurementDate (MeasurementDate md)
		{
			controllerMeasurementDateList.RemoveAt (controllerMeasurementDateList.IndexOf (md));
			MeasurementDateListUpdated.Invoke (this, null);
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
			MeasurementDateListUpdated.Invoke (this, null);
		}

		public void ClearMeasurementDate ()
		{
			controllerMeasurementDateList.Clear ();
			MeasurementDateListUpdated.Invoke (this, null);
		}

		public void Stop ()
		{
			running = false;
		}

		private void Run ()
		{
			while (running)
			{
				if (controllerMeasurementDateList.Count > 0)
				{
					if (controllerMeasurementDateList [0].DueTime.Subtract (DateTime.Now).TotalMilliseconds < 10)
					{
						#if DEBUG
						Console.WriteLine (controllerMeasurementDateList [0].ToString ());
						#endif
						switch (controllerMeasurementDateList [0].PinCmd)
						{
						case ArduinoController.Command.ReadAnalogPin:
							ArduinoController_.ReadAnalogPin (controllerMeasurementDateList [0].PinNr);
							break;
						}
						controllerMeasurementDateList.RemoveAt (0);
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

