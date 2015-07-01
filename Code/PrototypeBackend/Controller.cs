using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using PrototypeBackend;

namespace PrototypeBackend
{
	public class Controller
	{
		private Thread controllerThread;

		public List<IPin> controllerPins{ private set; get; }

		public List<Scheduler> controllerSchedulerList { private set; get; }

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
		public EventHandler PinsUpdated;

		private bool running = true;

		public Controller ()
		{
			ArduinoController.Init ();
			controllerSchedulerList = new List<Scheduler> ();
			controllerPins = new List<IPin> ();

			ArduinoController.NewAnalogValue += OnNewArduinoNewAnalogValue;
			ArduinoController.NewDigitalValue += OnNewArduinoNewDigitalValue;

			controllerThread = new Thread (new ThreadStart (Run)){ Name = "controllerThread" };
			controllerThread.Start ();

			ArduinoController.OnConnection += ((o, e) =>
			{
				ArduinoController.GetNumberAnalogPins ();
				ArduinoController.GetNumberDigitalPins ();
				ArduinoController.GetVersion ();
				ArduinoController.GetModel ();
			});
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

		public void AddScheduler (Scheduler s)
		{
			if (!controllerSchedulerList.Contains (s))
			{
				controllerSchedulerList.Add (s);
				controllerSchedulerList = controllerSchedulerList.OrderBy (o => o.DueTime).ToList ();
			}

			if (SchedulerListUpdated != null)
			{
				SchedulerListUpdated.Invoke (this, null);
			}
		}

		public void AddPin (IPin ip)
		{
			if (!controllerPins.Contains (ip))
			{
				controllerPins.Add (ip);
			}
			if (PinsUpdated != null)
			{
				PinsUpdated.Invoke (this, null);
			}
		}

		public void AddSchedulerRange (Scheduler[] s)
		{
			foreach (Scheduler sc in s)
			{
				if (!controllerSchedulerList.Contains (sc))
				{
					controllerSchedulerList.Add (sc);
				}
			}
			controllerSchedulerList = controllerSchedulerList.OrderBy (o => o.DueTime).ToList ();
			if (SchedulerListUpdated != null)
			{
				SchedulerListUpdated.Invoke (this, null);
			}
		}

		public void RemoveScheduler (Scheduler s)
		{
			controllerSchedulerList.Remove (s);
			if (SchedulerListUpdated != null)
			{
				SchedulerListUpdated.Invoke (this, null);
			}
		}

		public void RemoveSchedulerRange (Scheduler[] s)
		{
			foreach (Scheduler sc in s)
			{
				controllerSchedulerList.Remove (sc);
			}
			if (SchedulerListUpdated != null)
			{
				SchedulerListUpdated.Invoke (this, null);
			}
		}

		public void ClearScheduler ()
		{
			controllerSchedulerList.Clear ();
			if (SchedulerListUpdated != null)
			{
				SchedulerListUpdated.Invoke (this, null);
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
				if (controllerSchedulerList.Count > 0)
				{
					if (DateTime.Now.Subtract (controllerSchedulerList [0].DueTime).TotalMilliseconds < 10)
					{
						if (controllerSchedulerList [0].Run () == true)
						{
							controllerSchedulerList.RemoveAt (0);
						} else
						{
							controllerSchedulerList = controllerSchedulerList.OrderBy (o => o.DueTime).ToList ();
						}
					}
				}
			}
		}

		public int[] GetUsedPins (PrototypeBackend.PinType type)
		{
			List<int> pins = new List<int> ();


			foreach (IPin cp in this.controllerPins)
			{
				if (cp.Type == type)
				{
					pins.Add (cp.Number);
				}
			}
			if (pins.Count > 0)
			{
				return pins.ToArray ();
			}
			return new int[0];
		}

		public int[] GetUnusedPins (PrototypeBackend.PinType type)
		{
			uint numpins = 0;
			if (type.Equals (PrototypeBackend.PinType.ANALOG))
			{
				numpins = ArduinoController.NumberOfAnalogPins; 
			} else if (type.Equals (PrototypeBackend.PinType.DIGITAL))
			{
				numpins = ArduinoController.NumberOfDigitalPins;
			}

			List<int> unusedpins = new List<int> ();

			for (int i = 0; i < numpins; i++)
			{
				unusedpins.Add (i);
			}

			foreach (IPin ip in controllerPins)
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

