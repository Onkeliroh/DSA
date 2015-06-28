using System;
using NUnit.Framework;
using PrototypeBackend;
using System.Drawing;

namespace PrototypeTests
{
	[TestFixture ()]
	public class DPinTests
	{
		[Test ()]
		public void DPinConstructorTests ()
		{
			var pin = new DPin ();

			Assert.AreEqual (PrototypeBackend.PinType.DIGITAL, pin.PinType);
			Assert.AreEqual (PrototypeBackend.PinMode.OUTPUT, pin.PinMode);

			Assert.AreEqual (Color.Blue, pin.PinColor);
			pin.PinColor = System.Drawing.Color.Red;
			Assert.AreEqual (Color.Red, pin.PinColor);
		}

		[Test ()]
		public void DPinEqualsTest ()
		{
			var pin1 = new DPin ();
			var pin2 = new DPin ();

			Assert.AreEqual (true, pin1.Equals (pin2));

			pin1.PinColor = Color.LimeGreen;

			Assert.AreEqual (false, pin1.Equals (pin2));
		}
	}
}

