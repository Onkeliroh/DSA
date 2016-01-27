using System;
using NUnit.Framework;
using PrototypeBackend;
using Gdk;

namespace PrototypeTests
{
	[TestFixture ()]
	public class DPinTests
	{
		[Test ()]
		public void DPinConstructorTests ()
		{
			var pin = new DPin ();

			Assert.AreEqual (PrototypeBackend.PinType.DIGITAL, pin.Type);
			Assert.AreEqual (PrototypeBackend.PinMode.OUTPUT, pin.Mode);

			Assert.AreEqual (Color.Zero, pin.PlotColor);
			pin.PlotColor = new Gdk.Color (255, 0, 0);
			Assert.AreEqual (new Gdk.Color (255, 0, 0), pin.PlotColor);
		}

		[Test ()]
		public void DPinEqualsTest ()
		{
			var pin1 = new DPin ();
			var pin2 = new DPin ();

			Assert.AreEqual (true, pin1.Equals (pin2));

			pin1.PlotColor = new Color (0, 255, 0);

			Assert.AreEqual (false, pin1.Equals (pin2));
		}
	}
}

