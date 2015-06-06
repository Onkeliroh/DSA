using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;

namespace PrototypeBackend
{
	public class Controller
	{
		public EventHandler AnalogDataAvailable;

		private Thread controllerThread;

		public List<MeasurementDate> controllerMeasurementDateList{ private set; get; }

		public List<List<double>> analogList;
		public List<List<ArduinoController.DPinState>> digitalList;

		public EventHandler<ControllerAnalogEventArgs> NewAnalogValue;
		public EventHandler<ControllerDigitalEventArgs> NewDigitalValue;

		private bool running = true;

		public Controller ()
		{
			controllerThread = new Thread (new ThreadStart (Run)){ Name = "controllerThread" };
			controllerThread.Start ();

			controllerMeasurementDateList = new List<MeasurementDate> ();
		}

		public void AddMeasurementDate( MeasurementDate md )
		{
			controllerMeasurementDateList.Add (md);
			controllerMeasurementDateList = controllerMeasurementDateList.OrderBy (o => o.dueTime).ToList ();
		}

		public void AddMeasurementDateRange( MeasurementDate[] md)
		{
			controllerMeasurementDateList.AddRange (md);
			controllerMeasurementDateList = controllerMeasurementDateList.OrderBy (o => o.dueTime).ToList ();
		}

		public void RemoveMeasurementDate( MeasurementDate md)
		{
			controllerMeasurementDateList.RemoveAt (controllerMeasurementDateList.IndexOf (md));
		}

		public void RemoveMeasurementDateRange( MeasurementDate[] md)
		{
			int pos;
			foreach (MeasurementDate MD in md) {
				pos = controllerMeasurementDateList.IndexOf (MD);
				if (pos != -1 && pos >= 0 && pos < controllerMeasurementDateList.Count) {
					controllerMeasurementDateList.RemoveAt (pos);
				}
			}
		}
			
		public void Stop()
		{
			running = false;
		}

		private void Run()
		{
			while (running) {
				if (controllerMeasurementDateList.Count > 0)
				{
					if ( controllerMeasurementDateList[0].dueTime.Subtract(DateTime.Now).TotalMilliseconds < 100 )
					{
						Thread.Sleep (90);
						controllerMeasurementDateList[0].pinCmd();
					}
				}
			}
		}
	}
}

