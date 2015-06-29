using System;
using NUnit.Framework;
using PrototypeBackend;

namespace PrototypeTests
{
	[TestFixture ()]
	public class SignalTests
	{
		[Test ()]
		public void SignalConstructorTest ()
		{
			Signal signal = new Signal ();

			Assert.AreEqual (0, signal.Pins.Count);
			Assert.AreEqual (string.Empty, signal.SignalName);
			Assert.AreEqual (System.Drawing.Color.Blue, signal.SignalColor);
			Assert.AreEqual (string.Empty, signal.SignalOperationString);
		}

		[Test ()]
		public void SignalPinsTest ()
		{
			Signal signal = new Signal ();

			signal.Pins.Add (new APin () {
				PinLabel = "Temp1"
			});

			Assert.AreEqual (1, signal.Pins.Count);
			signal.Pins [0].Values.Add (42);

			string func = "Temp1";
			signal.SignalOperationString = func;
			Assert.AreEqual (42, signal.SignalValue);
		}
	}
}

