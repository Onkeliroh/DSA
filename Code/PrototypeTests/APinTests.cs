using System;

using PrototypeBackend;
using NUnit.Framework;

namespace PrototypeTests
{
	[TestFixture ()]
	public class APinTests
	{
		[Test ()]
		public void APinConstructorTests ()
		{
			var pin = new APin ();

			Assert.AreEqual (PrototypeBackend.PinType.ANALOG, pin.PinType);
			Assert.AreEqual (PrototypeBackend.PinMode.INPUT, pin.PinMode);

			pin.PinColor = System.Drawing.Color.Red;
			Assert.AreEqual (System.Drawing.Color.Red, pin.PinColor);
		}

		[Test ()]
		public void APinEqualsTest ()
		{
			var pin1 = new APin ();
			var pin2 = new APin ();

			Assert.AreEqual (true, pin1.Equals (pin2));

			pin1.PinColor = System.Drawing.Color.Green;
			pin2.PinColor = System.Drawing.Color.Yellow;

			Assert.AreEqual (false, pin1.Equals (pin2));
		}

		[Test ()]
		public void APinValuesTest ()
		{
			var pin1 = new APin ();
			pin1.Values.AddRange (new double[]{ 42, 15, 174, 245, 3456.3456 });
			Assert.AreEqual (3456.3456, pin1.PinValue, 0.001);
			pin1.Interval = 5;
			Assert.AreEqual ((42 + 15 + 174 + 245 + 3456.3456) / 5.0, pin1.PinValue, 0.001);

			pin1.Values.Clear ();
			Assert.AreEqual (double.NaN, pin1.PinValue);

			pin1.Values.AddRange (new double[]{ 1, 2, 3, double.NaN, 5 });
			Assert.AreEqual (5, pin1.Interval);
			Assert.AreEqual ((1 + 2 + 3 + 5) / 5.0, pin1.PinValue, 0.1);
		}
	}
}

