using System;
using PrototypeBackend;
using NUnit.Framework;
using System.Linq;

namespace PrototypeTests
{
	[TestFixture]
	public class MeasurementCombinationTests
	{
		[Test]
		public void MeasurementCombinationTests1 ()
		{
			var mecom = new MeasurementCombination ();
			var pin = new APin (){ Number = 0, RealNumber = 14, Interval = 1000, MeanValuesCount = 1 };

			Assert.AreEqual (0, mecom.Pins.Count);

			mecom.AddPin (pin);
			Assert.AreEqual (1, mecom.Pins.Count);
			Assert.AreEqual (false, mecom.AddPin (pin));

			var mecomcopy = new MeasurementCombination ();
			mecomcopy.AddPin (pin);

			Assert.AreEqual (mecom, mecomcopy);
		}

		[Test]
		public void MeasurementCombinationValueTest1 ()
		{
			var pin1 = new APin ();
			var pin2 = new APin ();
			var meCom1 = new MeasurementCombination ();

			pin1.Slope = 1;
			pin1.Offset = 0;
			pin1.MeanValuesCount = 1;
			pin1.Interval = 1;
			pin1.Number = 0;

			pin2.Slope = 1;
			pin2.Offset = 0;
			pin2.MeanValuesCount = 1;
			pin2.Interval = 1;
			pin2.Number = 1;

			pin1.Value = new DateTimeValue (1, DateTime.Now);
			pin2.Value = new DateTimeValue (1, DateTime.Now);

			meCom1.AddPin (pin1);
			meCom1.AddPin (pin2);

			meCom1.Operation = OperationCompiler.CompileOperation ("A0+A1", meCom1.Pins.Select (o => o.DisplayNumberShort).ToArray ());

			Assert.AreEqual (2, meCom1.Value.Value);
		}

		[Test]
		public void MeasurementCombinationValueTest2 ()
		{
			var pin1 = new APin ();
			var pin2 = new APin ();
			var meCom1 = new MeasurementCombination ();

			pin1.Slope = 1;
			pin1.Offset = 0;
			pin1.MeanValuesCount = 2;
			pin1.Interval = 1;
			pin1.Number = 0;

			pin2.Slope = 1;
			pin2.Offset = 0;
			pin2.MeanValuesCount = 2;
			pin2.Interval = 1;
			pin2.Number = 1;

			pin1.Value = new DateTimeValue (1, DateTime.Now);
			pin1.Value = new DateTimeValue (1, DateTime.Now);
			pin2.Value = new DateTimeValue (1, DateTime.Now);
			pin2.Value = new DateTimeValue (1, DateTime.Now);

			meCom1.AddPin (pin1);
			meCom1.AddPin (pin2);

			meCom1.Operation = OperationCompiler.CompileOperation ("A0+A1", meCom1.Pins.Select (o => o.DisplayNumberShort).ToArray ());

			Assert.AreEqual (2, meCom1.Value.Value);
		}

		[Test]
		public void MeasurementCombinationValueTest3 ()
		{
			var pin1 = new APin ();
			var pin2 = new APin ();
			var meCom1 = new MeasurementCombination ();

			pin1.Slope = 1;
			pin1.Offset = 0;
			pin1.MeanValuesCount = 1;
			pin1.Interval = 1;
			pin1.Number = 0;

			pin2.Slope = 1;
			pin2.Offset = 0;
			pin2.MeanValuesCount = 1;
			pin2.Interval = 1;
			pin2.Number = 1;

			pin1.Value = new DateTimeValue (1, DateTime.Now);
			pin1.Value = new DateTimeValue (1, DateTime.Now);
			pin2.Value = new DateTimeValue (1, DateTime.Now);
			pin2.Value = new DateTimeValue (1, DateTime.Now);


			meCom1.MeanValuesCount = 2;
			meCom1.AddPin (pin1);
			meCom1.AddPin (pin2);

			meCom1.Operation = OperationCompiler.CompileOperation ("A0+A1", meCom1.Pins.Select (o => o.DisplayNumberShort).ToArray ());

			Assert.AreEqual (2, meCom1.Value.Value);
		}

		[Test]
		public void MeasurementCombinationValueTest4 ()
		{
			var pin1 = new APin ();
			var pin2 = new APin ();
			var meCom1 = new MeasurementCombination ();

			pin1.Slope = 1;
			pin1.Offset = 0;
			pin1.MeanValuesCount = 1;
			pin1.Interval = 1;
			pin1.Number = 0;

			pin2.Slope = 1;
			pin2.Offset = 0;
			pin2.MeanValuesCount = 1;
			pin2.Interval = 1;
			pin2.Number = 1;

			pin1.Value = new DateTimeValue (1, DateTime.Now);
			pin1.Value = new DateTimeValue (1, DateTime.Now);
			pin2.Value = new DateTimeValue (1, DateTime.Now);
			pin2.Value = new DateTimeValue (1, DateTime.Now);


			meCom1.MeanValuesCount = 3;
			meCom1.AddPin (pin1);
			meCom1.AddPin (pin2);

			meCom1.Operation = OperationCompiler.CompileOperation ("A0+A1", meCom1.Pins.Select (o => o.DisplayNumberShort).ToArray ());

			Assert.AreEqual (double.NaN, meCom1.Value.Value);
		}

		[Test]
		public void MeasurementCombinationValueTest5 ()
		{
			var pin1 = new APin ();
			var pin2 = new APin ();
			var meCom1 = new MeasurementCombination ();

			pin1.Slope = 1;
			pin1.Offset = 0;
			pin1.MeanValuesCount = 1;
			pin1.Interval = 1;
			pin1.Number = 0;

			pin2.Slope = 1;
			pin2.Offset = 0;
			pin2.MeanValuesCount = 1;
			pin2.Interval = 1;
			pin2.Number = 1;

			pin1.Value = new DateTimeValue (1, DateTime.Now);
			pin1.Value = new DateTimeValue (1, DateTime.Now);
			pin2.Value = new DateTimeValue (1, DateTime.Now);
			pin2.Value = new DateTimeValue (1, DateTime.Now);


			meCom1.MeanValuesCount = 1;
			meCom1.AddPin (pin1);
			meCom1.AddPin (pin2);

			meCom1.Operation = OperationCompiler.CompileOperation ("A0+A1", meCom1.Pins.Select (o => o.DisplayNumberShort).ToArray ());

			Assert.AreEqual (2, meCom1.Value.Value);
		}
	}
}

