using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using SerialController;
using System.Timers;

namespace SerialControllerTests
{
	class Tests
	{
		static SerialController.SerialController port;

		public static void Main (string[] args)
		{
			port = new SerialController.SerialController ("/dev/ttyACM0", 9600);
			port.DataReceived += HandlePortDataReceived;
			port.ReadTimeout = 100;
			port.Open ();
			var timer = new System.Timers.Timer (500);
			timer.Elapsed += delegate {
				port.Write (new byte[]{ 0x48, 0x47 }, 0, 2);
			};
			timer.Start ();
			while (Console.ReadKey (true).KeyChar != 'x') {
				port.Write ("012");
			}
			timer.Stop ();
			port.Close ();
		}

		static void HandlePortDataReceived (object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
		{
			while (port.BytesToRead > 0) {
				int bt = port.ReadByte ();
				if (bt != '\n') {
					Console.Write (bt + " ");
				} else {
					Console.Write ("\n");
				}
			}
		}
	}
}
