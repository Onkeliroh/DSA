﻿using NUnit.Framework;
using System;
using PrototypeBackend;

namespace PrototypeTests
{
	[TestFixture ()]
	public class ControllerTests
	{
		[Test ()]
		public void AddTest ()
		{
			var tmp = new PrototypeBackend.Controller ();

			Assert.AreEqual (0, tmp.controllerMeasurementDateList.Count);

			tmp.AddMeasurementDate (new PrototypeBackend.MeasurementDate () {
				dueTime = new DateTime(1),
				pinNr = 0,
				pinType = ArduinoController.PinType.ANALOG
			});
			Assert.AreEqual (1, tmp.controllerMeasurementDateList.Count);
			tmp.AddMeasurementDate (new PrototypeBackend.MeasurementDate () {
				dueTime = new DateTime(3),
				pinNr = 0,
				pinType = ArduinoController.PinType.ANALOG
			});
			Assert.AreEqual (2, tmp.controllerMeasurementDateList.Count);

			tmp.AddMeasurementDate (new PrototypeBackend.MeasurementDate () {
				dueTime = new DateTime(42),
				pinNr = 0,
				pinType = ArduinoController.PinType.ANALOG
			});
			Assert.AreEqual (3, tmp.controllerMeasurementDateList.Count);

			tmp.AddMeasurementDate (new PrototypeBackend.MeasurementDate () {
				dueTime = new DateTime(2),
				pinNr = 0,
				pinType = ArduinoController.PinType.ANALOG
			});
			Assert.AreEqual (4, tmp.controllerMeasurementDateList.Count);

			Assert.AreEqual (new DateTime(1), tmp.controllerMeasurementDateList [0].dueTime);
			Assert.AreEqual (new DateTime(2), tmp.controllerMeasurementDateList [1].dueTime);
			Assert.AreEqual (new DateTime(3), tmp.controllerMeasurementDateList [2].dueTime);
			Assert.AreEqual (new DateTime(42), tmp.controllerMeasurementDateList [3].dueTime);
		}

		[Test ()]
		public void AddRangeTest ()
		{
			var tmp = new PrototypeBackend.Controller ();

			var dates = new MeasurementDate[5];
			dates [0] = new MeasurementDate (){ dueTime = new DateTime(2) };
			dates [1] = new MeasurementDate (){ dueTime = new DateTime(4) };
			dates [2] = new MeasurementDate (){ dueTime = new DateTime(1) };
			dates [3] = new MeasurementDate (){ dueTime = new DateTime(42) };
			dates [4] = new MeasurementDate (){ dueTime = new DateTime(88) };

			tmp.AddMeasurementDateRange (dates);
			Assert.AreEqual (5, tmp.controllerMeasurementDateList.Count);

			Assert.AreEqual (new DateTime(1), tmp.controllerMeasurementDateList [0].dueTime);
			Assert.AreEqual (new DateTime(2), tmp.controllerMeasurementDateList [1].dueTime);
			Assert.AreEqual (new DateTime(4), tmp.controllerMeasurementDateList [2].dueTime);
			Assert.AreEqual (new DateTime(42), tmp.controllerMeasurementDateList [3].dueTime);
			Assert.AreEqual (new DateTime(88), tmp.controllerMeasurementDateList [4].dueTime);
		}

		[Test ()]
		public void RemoveTest ()
		{
			var tmp = new PrototypeBackend.Controller ();

			var dates = new MeasurementDate[5];
			dates [0] = new MeasurementDate (){ dueTime = new DateTime(2) };
			dates [1] = new MeasurementDate (){ dueTime = new DateTime(4) };
			dates [2] = new MeasurementDate (){ dueTime = new DateTime(1) };
			dates [3] = new MeasurementDate (){ dueTime = new DateTime(42) };
			dates [4] = new MeasurementDate (){ dueTime = new DateTime(88) };

			tmp.AddMeasurementDateRange (dates);
			Assert.AreEqual (5, tmp.controllerMeasurementDateList.Count);

			tmp.controllerMeasurementDateList.Remove (dates [1]);
			Assert.AreEqual (4, tmp.controllerMeasurementDateList.Count);

			Assert.AreEqual (new DateTime(1), tmp.controllerMeasurementDateList [0].dueTime);
			Assert.AreEqual (new DateTime(2), tmp.controllerMeasurementDateList [1].dueTime);
			Assert.AreEqual (new DateTime(42), tmp.controllerMeasurementDateList [2].dueTime);
			Assert.AreEqual (new DateTime(88), tmp.controllerMeasurementDateList [3].dueTime);
		}

		[Test ()]
		public void RemoveRangeTest ()
		{
			var tmp = new PrototypeBackend.Controller ();

			var dates = new MeasurementDate[5];
			dates [0] = new MeasurementDate (){ dueTime = new DateTime(2) };
			dates [1] = new MeasurementDate (){ dueTime = new DateTime(4) };
			dates [2] = new MeasurementDate (){ dueTime = new DateTime(1) };
			dates [3] = new MeasurementDate (){ dueTime = new DateTime(42) };
			dates [4] = new MeasurementDate (){ dueTime = new DateTime(88) };

			tmp.AddMeasurementDateRange (dates);
			Assert.AreEqual (5, tmp.controllerMeasurementDateList.Count);

			var deletedates = new MeasurementDate[4];
			deletedates [0] = dates [1];
			deletedates [1] = dates [3];
			deletedates [2] = dates [4];
			deletedates [3] = new MeasurementDate (){ dueTime = new DateTime(43) };

			tmp.RemoveMeasurementDateRange (deletedates);
			Assert.AreEqual (2, tmp.controllerMeasurementDateList.Count);

			Assert.AreEqual (new DateTime(1), tmp.controllerMeasurementDateList [0].dueTime);
			Assert.AreEqual (new DateTime(2), tmp.controllerMeasurementDateList [1].dueTime);
		}
	}
}
