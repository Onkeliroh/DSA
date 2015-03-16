using System;
using System.IO.Ports;
using System.Threading;

namespace SerialController{
	public class SerialController : System.IO.Ports.SerialPort {

		#region Members

		private Thread DataReceiveThread;
		#endregion
		
		
		///Constructor
		public SerialController() : base() {}
	
		public SerialController( string name ) : base( name ){}

		
		private void DataReceiveThreadRun()
		{
			while ( this.IsOpen )
			{
				
			}
		}
	}	
}
