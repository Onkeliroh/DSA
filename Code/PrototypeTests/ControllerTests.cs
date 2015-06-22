using NUnit.Framework;
using System;
using PrototypeBackend;
using System.Threading;

namespace PrototypeTests
{
	[TestFixture ()]
	public class ControllerTests
	{
		[Test ()]
		public void AddTestScheduler ()
		{
			var tmp = new Controller ();

			Assert.AreEqual (0, tmp.controllerSchedulerList.Count);

			var sched = new Scheduler (){ DueTime = new DateTime (1) };
			tmp.AddScheduler (sched);
			Assert.AreEqual (1, tmp.controllerSchedulerList.Count);

			tmp.AddScheduler (new PrototypeBackend.Scheduler () {
				DueTime = new DateTime (42),
			});
			Assert.AreEqual (2, tmp.controllerSchedulerList.Count);

			tmp.AddScheduler (new Scheduler () {
				DueTime = new DateTime (3),
			});

			Assert.AreEqual (new DateTime (1), tmp.controllerSchedulerList [0].DueTime);
			Assert.AreEqual (new DateTime (3), tmp.controllerSchedulerList [1].DueTime);
			Assert.AreEqual (new DateTime (42), tmp.controllerSchedulerList [2].DueTime);

			tmp.AddScheduler (new Scheduler () {
				DueTime = new DateTime (13),
			});

			Assert.AreEqual (new DateTime (1), tmp.controllerSchedulerList [0].DueTime);
			Assert.AreEqual (new DateTime (3), tmp.controllerSchedulerList [1].DueTime);
			Assert.AreEqual (new DateTime (13), tmp.controllerSchedulerList [2].DueTime);
			Assert.AreEqual (new DateTime (42), tmp.controllerSchedulerList [3].DueTime);
		}

		[Test ()]
		public void AddScheduleRangeTest ()
		{
			var tmp = new PrototypeBackend.Controller ();

			var dates = new Scheduler[4];
			dates [0] = new Scheduler (){ DueTime = new DateTime (4) };
			dates [1] = new Scheduler (){ DueTime = new DateTime (1) };
			dates [2] = new Scheduler (){ DueTime = new DateTime (88) };
			dates [3] = new Scheduler (){ DueTime = new DateTime (42) };

			tmp.AddSchedulerRange (dates);
			Assert.AreEqual (4, tmp.controllerSchedulerList.Count);

			Assert.AreEqual (true, tmp.controllerSchedulerList [0].DueTime.Equals (new DateTime (1)));
			Assert.AreEqual (new DateTime (4), tmp.controllerSchedulerList [1].DueTime);
			Assert.AreEqual (new DateTime (42), tmp.controllerSchedulerList [2].DueTime);
			Assert.AreEqual (new DateTime (88), tmp.controllerSchedulerList [3].DueTime);
		}

		[Test ()]
		public void RemoveSchedulerTest ()
		{
			var tmp = new PrototypeBackend.Controller ();

			var dates = new Scheduler[5];
			dates [0] = new Scheduler (){ DueTime = new DateTime (2) };
			dates [1] = new Scheduler (){ DueTime = new DateTime (4) };
			dates [2] = new Scheduler (){ DueTime = new DateTime (1) };
			dates [3] = new Scheduler (){ DueTime = new DateTime (42) };
			dates [4] = new Scheduler (){ DueTime = new DateTime (88) };

			tmp.AddSchedulerRange (dates);
			Assert.AreEqual (5, tmp.controllerSchedulerList.Count);

			tmp.RemoveScheduler (dates [1]);
			Assert.AreEqual (4, tmp.controllerSchedulerList.Count);

			Assert.AreEqual (new DateTime (1), tmp.controllerSchedulerList [0].DueTime);
			Assert.AreEqual (new DateTime (2), tmp.controllerSchedulerList [1].DueTime);
			Assert.AreEqual (new DateTime (42), tmp.controllerSchedulerList [2].DueTime);
			Assert.AreEqual (new DateTime (88), tmp.controllerSchedulerList [3].DueTime);
		}

		[Test ()]
		public void RemoveSchedulerRangeTest ()
		{
			var tmp = new PrototypeBackend.Controller ();

			var dates = new Scheduler[5];
			dates [0] = new Scheduler (){ DueTime = new DateTime (2) };
			dates [1] = new Scheduler (){ DueTime = new DateTime (4) };
			dates [2] = new Scheduler (){ DueTime = new DateTime (1) };
			dates [3] = new Scheduler (){ DueTime = new DateTime (42) };
			dates [4] = new Scheduler (){ DueTime = new DateTime (88) };

			tmp.AddSchedulerRange (dates);
			Assert.AreEqual (5, tmp.controllerSchedulerList.Count);

			var deletedates = new Scheduler[4];
			deletedates [0] = dates [1];
			deletedates [1] = dates [3];
			deletedates [2] = dates [4];
			deletedates [3] = new Scheduler (){ DueTime = new DateTime (43) };

			tmp.RemoveSchedulerRange (deletedates);
			Assert.AreEqual (2, tmp.controllerSchedulerList.Count);

			Assert.AreEqual (new DateTime (1), tmp.controllerSchedulerList [0].DueTime);
			Assert.AreEqual (new DateTime (2), tmp.controllerSchedulerList [1].DueTime);
		}

		[Test ()]
		public void EventTest ()
		{
			var tmp = new PrototypeBackend.Controller ();
			bool received = false;
			int pin = -1;
			double val = -1;
			tmp.NewAnalogValue += (o, e) =>
			{
				received = true;
				pin = e.PinNr;
				val = e.PinValue;
			};

			tmp.NewAnalogValue.Invoke (null, new ControllerAnalogEventArgs (42, 42));
			Assert.AreEqual (true, received);
			Assert.AreEqual (42, pin);
			Assert.AreEqual (42, val);
		}

		//		[Test ()]
		//		public void QueueTest ()
		//		{
		//			var tmp = new PrototypeBackend.Controller ();
		//
		//			bool action = false;
		//
		//			tmp.AddMeasurementDate (new MeasurementDate () {
		//				DueTime = DateTime.Now.AddSeconds (1),
		//				PinNr = 42,
		//				PinType = ArduinoController.PinType.DIGITAL,
		//				PinCmd = () =>
		//				{
		//					action = true;
		//				}
		//			});
		//
		//			Thread.Sleep (2000);
		//			Assert.AreEqual (true, action);
		//		}

		[Test ()]
		public void GetUsedPinsTest ()
		{
			var tmp = new Controller ();

			var pins = tmp.GetUsedPins (PrototypeBackend.PinType.ANALOG);

			Assert.AreEqual (pins.Length, 0);

			tmp.AddPin (new APin () {
				PinNr = 0,
			});

			pins = tmp.GetUsedPins (PrototypeBackend.PinType.ANALOG);
			Assert.AreEqual (pins.Length, 1);
			Assert.AreEqual (pins [0], 0);

			tmp.AddPin (new DPin () {
				PinNr = 42
			});

			tmp.AddPin (new DPin () {
				PinNr = 13
			});

			pins = tmp.GetUsedPins (PrototypeBackend.PinType.ANALOG);
			Assert.AreEqual (pins.Length, 1);
			Assert.AreEqual (pins [0], 0);

			pins = tmp.GetUsedPins (PrototypeBackend.PinType.DIGITAL);
			Assert.AreEqual (pins.Length, 2);
			Assert.AreEqual (pins [0], 42);
			Assert.AreEqual (pins [1], 13);
		}

		[Test ()]
		public void GetUnusedPinsTest ()
		{
			var tmp = new Controller ();

			var pins = tmp.GetUnusedPins (PrototypeBackend.PinType.ANALOG);
			Assert.AreEqual (pins.Length, 6);

//			tmp.AddPin (new MeasurementData () {
//				PinNr = 0,
//			});
//
//			pins = tmp.GetUnusedPins (ArduinoController.PinType.ANALOG);
//			Assert.AreEqual (pins.Length, 5);
//			Assert.AreEqual (pins [0], 1);
//
//			tmp.AddPin (new Sequence () {
//				PinNr = 42
//			});
//
//			tmp.AddPin (new Sequence () {
//				PinNr = 13
//			});
//
//			pins = tmp.GetUnusedPins (ArduinoController.PinType.ANALOG);
//			Assert.AreEqual (pins.Length, 5);
//			Assert.AreEqual (pins [0], 1);
//
//			pins = tmp.GetUnusedPins (ArduinoController.PinType.DIGITAL);
//			Assert.AreEqual (pins.Length, 18);
//			Assert.AreEqual (pins [0], 0);
//			Assert.AreEqual (pins [1], 1);
		}
	}
}

