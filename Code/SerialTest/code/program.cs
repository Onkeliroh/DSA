using System;
using System.IO.Ports;

namespace SerialTest
{
	class MainClass
	{
		public static void Main (string[] args)
		{
//			System.Console.WriteLine("Hello World!");
			ShowSerialPorts();
		}

		public static void ShowSerialPorts ()
		{
//			SerialPort _serialPort = new SerialPort();
			string[]tmp = SerialPort.GetPortNames();
			foreach ( string s in tmp )
				Console.WriteLine(s);
		}
	}
}
