using System;
using System.IO.Ports;
using System.Collections.Generic;

namespace Utility
{
	public static class Serial
	{
		public static SerialPort Port = new SerialPort();
		public static Queue<string> Incomming = new Queue<string>();

		public static string[] GetPorts()
		{
			return SerialPort.GetPortNames ();	
		}

		public static bool ConnectToPort( string portname, int baut )
		{
			try{
			Port = new SerialPort (portname, baut);
			Port.PortName = portname;
			Port.BaudRate = baut;
			Port.ReadBufferSize = 8192;
			Port.Parity = Parity.None;
			Port.StopBits = StopBits.One;
			Port.DataBits = 8;
			Port.Handshake = Handshake.None;
			Port.DataReceived += Receive;
			Port.Open ();
			}
			catch (Exception e) {
				Console.Error.WriteLine (e);
				return false;
			}
			Console.WriteLine ("Successfully connected!");
			return true;
		}

		private static void Receive( object sender, SerialDataReceivedEventArgs e )
		{
			Console.WriteLine ("serial inc");
			string inc = ((SerialPort)sender).ReadExisting ();
			Incomming.Enqueue(inc);
			//todo evtl. loggen

		}

		private static void Disposed( object sender, EventArgs e )
		{
			//todo keine ahnung was hier hin kommt
		}
	}
}

