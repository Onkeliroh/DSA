using System;

using PrototypeBackend;
using NUnit.Framework;
using Gdk;

namespace PrototypeTests
{
	[TestFixture ()]
	public class APinTests
	{
		[Test ()]
		public void APinConstructorTests ()
		{
			var pin = new APin ();

			Assert.AreEqual (PrototypeBackend.PinType.ANALOG, pin.Type);
			Assert.AreEqual (PrototypeBackend.PinMode.INPUT, pin.Mode);

			pin.PlotColor = new Gdk.Color (255, 0, 0);
			Assert.AreEqual (new Gdk.Color (255, 0, 0), pin.PlotColor);
		}

		[Test ()]
		public void APinEqualsTest ()
		{
			var pin1 = new APin ();
			var pin2 = new APin ();

			Assert.AreEqual (true, pin1.Equals (pin2));

			pin1.PlotColor = new Color (0, 255, 0);
			pin2.PlotColor = new Color (0, 255, 255);

			Assert.AreEqual (false, pin1.Equals (pin2));
		}

		[Test ()]
		[NUnit.Framework.Ignore]
		public void APinValuesTest ()
		{
//			var pin1 = new APin ();
//			pin1.Values.AddRange (new double[]{ 42, 15, 174, 245, 3456.3456 });
//			Assert.AreEqual (3456.3456, pin1.Value, 0.001);
//			pin1.Interval = 5;
//			Assert.AreEqual ((42 + 15 + 174 + 245 + 3456.3456) / 5.0, pin1.Value, 0.001);
//
//			pin1.Values.Clear ();
//			Assert.AreEqual (double.NaN, pin1.Value);
//
//			pin1.Values.AddRange (new double[]{ 1, 2, 3, double.NaN, 5 });
//			Assert.AreEqual (5, pin1.Interval);
//			Assert.AreEqual ((1 + 2 + 3 + 5) / 5.0, pin1.Value, 0.1);
		}
	}
}

