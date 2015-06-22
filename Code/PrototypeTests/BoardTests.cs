using System;
using PrototypeBackend;
using NUnit.Framework;

namespace PrototypeTests
{
	[TestFixture ()]
	public class BoardTests
	{
		[Test ()]
		public void BoardXmlTests ()
		{
			var tmp = new PrototypeBackend.Board ();
		}

		[Test ()]
		public void ConstructorTest ()
		{
			var tmp = new Board ();
			Assert.AreEqual (6, tmp.NumberOfAnalogPins);
			Assert.AreEqual (20, tmp.NumberOfDigitalPins);
			Assert.AreEqual (0, tmp.AnalogReference);
		}

		[Test ()]
		public void Constructor2Test ()
		{
			var tmp = new Board (42, 43, null, "TestBoard", "10", "SuperModel", true);
			Assert.AreEqual (42, tmp.NumberOfAnalogPins);
			Assert.AreEqual (43, tmp.NumberOfDigitalPins);
			Assert.AreEqual ("TestBoard", tmp.Name);
			Assert.AreEqual ("10", tmp.Version);
			Assert.AreEqual ("SuperModel", tmp.Model);
			Assert.AreEqual (true, tmp.UseDTR);
		}
	}
}

