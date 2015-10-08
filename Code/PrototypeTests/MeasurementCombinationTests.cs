using System;
using PrototypeBackend;
using NUnit.Framework;

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
	}
}

