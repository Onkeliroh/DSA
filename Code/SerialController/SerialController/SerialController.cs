using System;
using System.ComponentModel;
using System.IO.Ports;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.IO;

namespace SerialController{
	public class SerialController : System.IO.Ports.SerialPort {

		#region Members
		int fd;
		FieldInfo disposedFieldInfo;
		object data_received;
		//private Thread DataReceiveThread;
		#endregion
		
		
		#region Constructor
		public SerialController() : base() {}
	
		public SerialController(IContainer container) : base(container){}

		public SerialController( string portName ) : base( portName ){}

		public SerialController( string portName, int baudRate ) : base ( portName,baudRate ){}

		public SerialController( string portName, int baudRate, Parity parity ) : base ( portName, baudRate, parity ){}

		public SerialController( string portName, int baudRate, Parity parity, int dataBits) : base ( portName, baudRate, parity, dataBits){}

		public SerialController( string portName, int baudRate, Parity parity, int dataBits, StopBits stopBits ) : base ( portName, baudRate, parity, dataBits, stopBits ){}
		#endregion

		public new void Open ()
		{
			base.Open ();

			if( IsWindows == false )
			{
				FieldInfo fieldInfo = BaseStream.GetType ().GetField ("fd", BindingFlags.Instance | BindingFlags.NonPublic);
				fd = (int)fieldInfo.GetValue (BaseStream);
				disposedFieldInfo = BaseStream.GetType ().GetField ("disposed", BindingFlags.Instance | BindingFlags.NonPublic);
				fieldInfo = typeof(SerialPort).GetField ("data_received", BindingFlags.Instance | BindingFlags.NonPublic);
				data_received = fieldInfo.GetValue (this);

				new System.Threading.Thread (new System.Threading.ThreadStart (this.EventThreadFunction)).Start ();
			}
		}

		static bool IsWindows
		{
			get{
				PlatformID id = Environment.OSVersion.Platform;
				return id == PlatformID.Win32Windows || id == PlatformID.Win32NT;
			}
		}

		private void EventThreadFunction()
		{
			do {
				try {
					var _stream = BaseStream;
					if (_stream == null) {
						return;
					}
					if (Poll (_stream, ReadTimeout)) {
						OnDataReceived (null);
					}
				} catch {
					return;
				}
			} while(IsOpen);
		}

		void OnDataReceived (SerialDataReceivedEventArgs args)
		{
			SerialDataReceivedEventHandler handler = (SerialDataReceivedEventHandler)Events [data_received];

			if ( handler != null)
			{
				handler (this, args);
			}
		}

		[DllImport ("MonoPosixHelper", SetLastError = true)]
		static extern bool poll_serial (int fd, out int error, int timeout);

		private bool Poll(Stream stream, int timeout)
		{
			CheckDisposed (stream);
			if ( IsOpen == false)
			{
				throw new Exception ("port is closed");
			}
			int error;

			bool poll_result = poll_serial (fd, out error, ReadTimeout);
			if ( error == -1)
			{
				ThrowIOException ();
			}
			return poll_result;
		}

		[DllImport ("libc")]
		static extern IntPtr strerror (int errnum);

		static void ThrowIOException ()
		{
			int errnum = Marshal.GetLastWin32Error ();
			string error_message = Marshal.PtrToStringAnsi (strerror (errnum));
			throw new IndexOutOfRangeException (error_message);
		}

		void CheckDisposed (Stream stream)
		{
			bool disposed = (bool)disposedFieldInfo.GetValue (stream);
			if (disposed)
			{
				throw new ObjectDisposedException (stream.GetType ().FullName);
			}
		}

		private void DataReceiveThreadRun()
		{
			while ( this.IsOpen )
			{
				
			}
		}
	}	
}
