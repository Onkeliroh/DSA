using System;
using System.IO.Ports;
using System.Collections.Generic;

namespace Utility
{
	public static class Serial
	{
		public static SerialPort port = new SerialPort();
		public static Queue<string> Incomming = new Queue<string>();

		public static string[] GetPorts()
		{
			return SerialPort.GetPortNames ();	
		}

		public static void ConnectToPort( string portname, int baut )
		{
			try{
				port = new SerialPort(portname,baut);
				port.PortName = portname;
				port.BaudRate = baut;
				port.ReadBufferSize = 8192;
				port.Parity = Parity.None;
				port.StopBits = StopBits.One;
				port.DataBits = 8;
				port.Handshake = Handshake.None;

				//attach handlers
				port.DataReceived += Receive;
				port.Disposed += Disposed;

				port.Open();
			}
			catch (Exception) {
				throw;
			}
			Console.WriteLine ("Successfully connected!");
		}

		private static void Receive( object sender, SerialDataReceivedEventArgs e )
		{
			Console.WriteLine ("serial inc");
			Incomming.Enqueue(((SerialPort)sender).ReadExisting());
			//todo evtl. loggen

		}

		private static void Disposed( object sender, EventArgs e )
		{
			//todo keine ahnung was hier hin kommt
		}
	}
}

