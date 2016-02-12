using System;
using NUnit.Framework;
using Backend;
using System.Threading;
using System.IO;

namespace PrototypeTests
{
	[TestFixture]	
	public class ConnectionsTests
	{
		[Test]
		[Ignore ("duno")]
		public void ConnectionNOTAcknowledgeTest ()
		{
			ArduinoController.SerialPortName = System.IO.Ports.SerialPort.GetPortNames () [0];
			Console.WriteLine ("Connecting to " + ArduinoController.SerialPortName);
			ArduinoController.Setup (false);

			Assert.AreEqual (true, ArduinoController.IsConnected);

			Thread.Sleep (3000);

			Assert.AreEqual (false, ArduinoController.IsConnected);
		}
	}
}

