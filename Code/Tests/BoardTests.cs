using System;
using PrototypeBackend;
using NUnit.Framework;

namespace PrototypeTests
{
	[TestFixture ()]
	public class BoardTests
	{
		[Test ()]
		public void ConstructorTest ()
		{
			var tmp = new Board ();
			Assert.AreEqual (6, tmp.NumberOfAnalogPins);
			Assert.AreEqual (20, tmp.NumberOfDigitalPins);
			Assert.AreEqual (5, tmp.AnalogReferenceVoltage);
		}

		[Test ()]
		public void Constructor2Test ()
		{
			var tmp = new Board (42, 43) {
				Name = "TestBoard",
				MCU = "SuperModel",
				UseDTR = true
			};
			Assert.AreEqual (42, tmp.NumberOfAnalogPins);
			Assert.AreEqual (43, tmp.NumberOfDigitalPins);
			Assert.AreEqual ("TestBoard", tmp.Name);
			Assert.AreEqual ("SuperModel", tmp.MCU);
			Assert.AreEqual (true, tmp.UseDTR);
		}
	}
}

