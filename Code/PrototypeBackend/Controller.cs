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

		public EventHandler<ControllerAnalogEventArgs> NewAnalogValue;
		public EventHandler<ControllerDigitalEventArgs> NewDigitalValue;

		private bool running = true;

		public ArduinoController.ArduinoController ArduinoController = new ArduinoController.ArduinoController ();

		public Controller ()
		{
			controllerThread = new Thread (new ThreadStart (Run)){ Name = "controllerThread" };
			controllerThread.Start ();

			controllerMeasurementDateList = new List<MeasurementDate> ();

			ArduinoController.NewAnalogValue += OnNewArduinoNewAnalogValue;
			ArduinoController.NewDigitalValue += OnNewArduinoNewDigitalValue;
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
			controllerMeasurementDateList = controllerMeasurementDateList.OrderBy (o => o.dueTime).ToList ();
		}

		public void AddMeasurementDateRange (MeasurementDate[] md)
		{
			controllerMeasurementDateList.AddRange (md);
			controllerMeasurementDateList = controllerMeasurementDateList.OrderBy (o => o.dueTime).ToList ();
		}

		public void RemoveMeasurementDate (MeasurementDate md)
		{
			controllerMeasurementDateList.RemoveAt (controllerMeasurementDateList.IndexOf (md));
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
					if (controllerMeasurementDateList [0].dueTime.Subtract (DateTime.Now).TotalMilliseconds < 100)
					{
						Thread.Sleep (90);
						controllerMeasurementDateList [0].pinCmd ();
						controllerMeasurementDateList.RemoveAt (0);
					}
				}
				Thread.Sleep (10);
			}
		}
	}
}

