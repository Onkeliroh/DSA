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
	}
}

