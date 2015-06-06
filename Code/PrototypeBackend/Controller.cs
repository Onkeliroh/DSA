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
			foreach (MeasurementDate MD in md) {
				if (MD != null) {
					controllerMeasurementDateList.RemoveAt (controllerMeasurementDateList.IndexOf (MD));
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
				
			}
		}
	}
}

