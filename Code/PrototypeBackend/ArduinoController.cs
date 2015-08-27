using System;
using System.Collections.Generic;
using CommandMessenger;
using CommandMessenger.Transport.Serial;
using PrototypeBackend;
using System.IO.Ports;

namespace PrototypeBackend
{
	#region ENUMS
	public enum Command
	{
		Acknowledge,
		Error,
		Ready,
		SetPinMode,
		SetPinState,
		SetAnalogPin,
		SetPin,
		SetAnalogReference,
		ReadPinMode,
		ReadPinState,
		ReadAnalogPin,
		ReadPinsMode,
		ReadPin,

		ReadPinModeResult,
		ReadPinStateResult,
		ReadAnalogPinResult,
		ReadPinsModeResult,
		ReadPinResult,

		GetVersion,
		GetModel,
		GetNumberDigitalPins,
		GetNumberAnalogPins,
		GetDigitalBitMask,
		GetPinOutputMask,
		GetPinModeMask,
		GetAnalogReference,
		GetAnalogPinNumbers,
		GetSDASCL,
	};

	public enum PinMode
	{
		INPUT,
		OUTPUT}

	;

	public enum DPinState
	{
		LOW,
		HIGH}

	;

	public enum PinType
	{
		DIGITAL,
		ANALOG}

	;
	#endregion

	public static class ArduinoController
	{
		#region Events

		public static event EventHandler<ConnectionChangedArgs> OnConnectionChanged;
		public static event EventHandler<ControllerAnalogEventArgs> NewAnalogValue;
		public static event EventHandler<ControllerDigitalEventArgs> NewDigitalValue;
		public static event EventHandler<CommunicationArgs> OnSendMessage;
		public static event EventHandler<CommunicationArgs> OnReceiveMessage;

		#endregion

		#region Properies and Member

		private static CmdMessenger _cmdMessenger;
		private static Board _board = new Board ();

		public static Board @Board { get { return _board; } set { _board = value; } }

		public static bool AutoConnect {
			get{ return _autoconnect; }
			set {
				_autoconnect = value;
				if (AutoConnectTimer != null)
				{
					if (_autoconnect)
					{
						AutoConnectTimer.Start ();
					} else
					{
						AutoConnectTimer.Abort ();
					}
				}
			}
		}

		private static bool _autoconnect = true;

		private static System.Threading.Thread AutoConnectTimer = null;

		public static bool IsConnected {
			#if FAKESERIAL
			get { return true; }
			private set{ }
			#else
			get;
			private set;
			#endif
		}

		public static string SerialPortName {
			get;
			set;
		}

		public static List<List<float>> AnalogValues {
			get;
			private set;
		}

		public static string Version {
			private set{ _board.Version = value; }
			get{ return _board.Version; }
		}

		public static string MCU {
			private set{ _board.MCU = value; }
			get{ return _board.MCU; }
		}

		public static uint NumberOfDigitalPins {
			private set{ _board.NumberOfDigitalPins = value; }
			get { return _board.NumberOfDigitalPins; }
		}

		public static uint NumberOfAnalogPins {
			private set { _board.NumberOfAnalogPins = value; }
			get{ return _board.NumberOfAnalogPins; }
		}

		public static uint[] HardwareAnalogPinNumbers {
			private set{ _board.HardwareAnalogPins = value; }
			get{ return _board.HardwareAnalogPins; }
		}

		public static UInt32 DigitalBitMask {
			private set;
			get;
		}

		public static UInt32 PinOutputMask {
			private set;
			get;
		}

		public static UInt32 PinModeMask {
			private set;
			get;
		}

		public static Dictionary<string,int> AnalogReferences {
			private set{ _board.AnalogReferences = value; }
			get { return _board.AnalogReferences; }
		}

		#endregion

		public static void Init (uint apins = 6, uint dpins = 20)
		{
			_board = new Board ();
			NumberOfAnalogPins = apins;
			NumberOfDigitalPins = dpins;

			OnConnectionChanged += (sender, e) =>
			{
				if (e.Connected)
				{
					GetVersion ();
					GetModel ();
					GetNumberAnalogPins ();
					GetNumberDigitalPins ();
					GetAnalogPinNumbers ();
					GetSDASCLRXTX ();
				}
			};

			SerialPortName = "";
			#if FAKESERIAL
			IsConnected = true;
			#else
			IsConnected = false;
			#endif

			AutoConnectTimer = new System.Threading.Thread (() =>
			{
				while (!IsConnected)
				{
					foreach (string s in System.IO.Ports.SerialPort.GetPortNames())
					{
						Console.WriteLine ("attemting auto connect to " + s);
						SerialPortName = s;

						Setup (false);
						System.Threading.Thread.Sleep (2000);
						if (IsConnected)
							break;
						Disconnect ();
						Setup (true);
						System.Threading.Thread.Sleep (2000);
						if (IsConnected)
							break;
						Disconnect ();
					}
				}
			});
			if (AutoConnect)
			{
				AutoConnectTimer.Start ();
			}
		}

		public static void Setup (bool Dtr = false)
		{
			if (SerialPortName != null)
			{
				if (_cmdMessenger != null)
				{
					_cmdMessenger.Disconnect ();
					_cmdMessenger.Dispose ();
				}
				_cmdMessenger = new CmdMessenger (new SerialTransport () {
					CurrentSerialSettings = {
						PortName = SerialPortName,
						BaudRate = 115200,
						DataBits = 8,
						Parity = Parity.None,
						DtrEnable = Dtr//bei UNO auf false ändern 
					}
				}, 512, BoardType.Bit32);

				// Attach the callbacks to the Command Messenger
				AttachCommandCallBacks ();

				// Attach to NewLinesReceived for logging purposes
				_cmdMessenger.NewLineReceived += NewLineReceived;

				// Attach to NewLineSent for logging purposes
				_cmdMessenger.NewLineSent += NewLineSent;                       

				#if !FAKESERIAL
				// Start listening
//				IsConnected = _cmdMessenger.Connect ();
				_cmdMessenger.Connect ();
				#endif
			}
		}

		// Exit function
		public static void Exit ()
		{
			#if !FAKESERIAL
			// Stop listening
			AutoConnect = false;
			if (IsConnected)
			{
				_cmdMessenger.Disconnect ();
			}

			// Dispose Command Messenger
			if (_cmdMessenger != null)
			{
				_cmdMessenger.Dispose ();
			}
			#endif
		}

		public static void Disconnect ()
		{
			if (IsConnected)
			{
				IsConnected = false;
				#if !FAKESERIAL
				_cmdMessenger.Disconnect ();
				#endif
				if (OnConnectionChanged != null)
				{
					OnConnectionChanged.Invoke (null, new ConnectionChangedArgs (false, null));
				}
				if (AutoConnect && AutoConnectTimer.ThreadState != System.Threading.ThreadState.Running)
				{
					AutoConnectTimer.Start ();
				}
			}

		}

		public static bool AttemdAutoConnect ()
		{
			foreach (string s in System.IO.Ports.SerialPort.GetPortNames())
			{
				SerialPortName = s;

				Setup (false);
				System.Threading.Thread.Sleep (2000);
//				await System.Threading.Tasks.Task.Delay (1000);
				if (IsConnected)
				{
					return true;
//					break;
				}
				Disconnect ();
				Setup (true);
				System.Threading.Thread.Sleep (2000);
//				await System.Threading.Tasks.Task.Delay (1000);
				if (IsConnected)
				{
					return true;
//					break;
				}
				Disconnect ();
			}
			Disconnect ();
			return false;
		}

		/// Attach command call backs. 
		private static void AttachCommandCallBacks ()
		{
			_cmdMessenger.Attach (OnUnknownCommand);
			_cmdMessenger.Attach ((int)Command.Acknowledge, OnAcknowledge);
			_cmdMessenger.Attach ((int)Command.Error, OnError);
		}


		#region CALLBACKS

		// Called when a received command has no attached function.
		// In a WinForm application, console output gets routed to the output panel of your IDE
		static void OnUnknownCommand (ReceivedCommand arguments)
		{            
			#if DEBUG
			Console.WriteLine (@"Command without attached callback received");
			#endif
		}

		// Callback function that prints that the Arduino has acknowledged
		static void OnAcknowledge (ReceivedCommand arguments)
		{
			#if DEBUG
			Console.WriteLine (@" Arduino is ready");
			#endif
			IsConnected = true;
			if (OnConnectionChanged != null)
			{
				OnConnectionChanged.Invoke (null, new ConnectionChangedArgs (true, SerialPortName));
			}
		}

		// Callback function that prints that the Arduino has experienced an error
		static void OnError (ReceivedCommand arguments)
		{
			#if DEBUG
			Console.WriteLine (@"Arduino has experienced an error");
			#endif
		}

		private static void OnGetVersion (ReceivedCommand args)
		{
			Version = args.ReadStringArg ();
		}

		// Log received line to console
		private static void NewLineReceived (object sender, CommandEventArgs e)
		{
			#if DEBUG
			Console.WriteLine (@"Received > " + e.Command.CommandString ());
			#endif

			if (OnReceiveMessage != null)
			{
				OnReceiveMessage.Invoke (null, new CommunicationArgs (e.Command.CommandString ()));
			}
		}

		// Log sent line to console
		private static void NewLineSent (object sender, CommandEventArgs e)
		{
			#if DEBUG
			Console.WriteLine (DateTime.Now + @": Sent > " + e.Command.CommandString ());
			#endif
			if (OnSendMessage != null)
			{
				OnSendMessage.Invoke (null, new CommunicationArgs (e.Command.CommandString ()));
			}
		}

		#endregion

		#region SETTER

		public static void SetPinMode (int nr, PinMode mode)
		{
			var command = new SendCommand ((int)Command.SetPinMode, nr);
			command.AddArgument ((Int16)mode);
			_cmdMessenger.SendCommand (command);
		}

		public static void SetPinState (int nr, DPinState state)
		{
			var command = new SendCommand ((int)Command.SetPinMode, nr);
			command.AddArgument ((Int16)state);
			_cmdMessenger.SendCommand (command);
		}

		public static void SetPin (int nr, PinMode mode, DPinState state)
		{
			#if !FAKESERIAL
			var command = new SendCommand ((int)Command.SetPin, nr);
			command.AddArgument ((Int16)mode);
			command.AddArgument ((Int16)state);
			lock (_cmdMessenger)
			{
				_cmdMessenger.SendCommand (command, SendQueue.WaitForEmptyQueue, ReceiveQueue.Default);
			}
			#endif
		}

		public static void SetAnalogReference (int AnalogReference)
		{
			_board.AnalogReferenceVoltage = AnalogReference;
			var command = new SendCommand ((int)Command.SetAnalogReference);
			command.AddArgument (AnalogReference);
			_cmdMessenger.SendCommand (command);
		}

		public static void SetAnalogPin (int Pin, int Val)
		{
			var command = new SendCommand ((int)Command.SetAnalogPin, Pin);
			command.AddArgument (Val);
			_cmdMessenger.SendCommand (command);
		}

		#endregion

		#region GETTER

		public static int ReadAnalogPin (int nr)
		{
			return ReadAnalogPin (new int[]{ nr }) [0];
		}

		public static int[] ReadAnalogPin (int[] nr)
		{
			var command = new SendCommand ((int)Command.ReadAnalogPin, (int)Command.ReadAnalogPin, 500);
			command.AddArgument (nr.Length);
			foreach (int i in nr)
			{
				command.AddArgument (i);
			}
			var result = _cmdMessenger.SendCommand (command);
			if (result.Ok)
			{
				int[] results = new int[nr.Length];
				for (int i = 0; i < nr.Length; i++)
				{
					results [i] = result.ReadBinInt32Arg ();
				}
				NewAnalogValue.Invoke (null, new ControllerAnalogEventArgs (nr, results));
				return results;
			}
			return new int[0];
		}

		public static DPinState ReadPin (int nr)
		{
			var command = new SendCommand ((int)Command.ReadPin, (int)Command.ReadPin, 500);
			command.AddArgument (nr);
			var result = _cmdMessenger.SendCommand (command);
			if (result.Ok)
			{
				return (result.ReadBinInt16Arg () == (int)DPinState.HIGH) ? DPinState.HIGH : DPinState.LOW;
			}
			return DPinState.LOW;
		}

		public static void GetVersion ()
		{
			var command = new SendCommand ((int)Command.GetVersion, (int)Command.GetVersion, 1000);
			var returnVal = _cmdMessenger.SendCommand (command);
			if (returnVal.Ok)
			{
				Version = returnVal.ReadStringArg ();
			}
		}

		public static void GetAnalogReference ()
		{
			var command = new SendCommand ((int)Command.GetAnalogReference, (int)Command.GetAnalogReference, 500);
			var returnVal = _cmdMessenger.SendCommand (command);
			if (returnVal.Ok)
			{
				AnalogReferences.Clear ();
				for (int i = 0; i < (returnVal.Arguments.Length / 2); i++)
				{
					AnalogReferences.Add (returnVal.ReadStringArg (), returnVal.ReadInt16Arg ());
				}
			}
		}

		public static void GetModel ()
		{
			var command = new SendCommand ((int)Command.GetModel, (int)Command.GetModel, 1000);
			var returnVal = _cmdMessenger.SendCommand (command);
			if (returnVal.Ok)
			{
//				var tmp = returnVal.ReadStringArg ();
				MCU = returnVal.ReadStringArg ().ToLower ();
//				Console.WriteLine (MCU);
			}
		}

		public static void GetNumberDigitalPins ()
		{
			var command = new SendCommand ((int)Command.GetNumberDigitalPins, (int)Command.GetNumberDigitalPins, 1000);
			var returnVal = _cmdMessenger.SendCommand (command);
			if (returnVal.Ok)
			{
				NumberOfDigitalPins = returnVal.ReadUInt32Arg ();
			} else
			{
				//in case the arduino did not respond
				NumberOfDigitalPins = uint.MaxValue;
			}
		}

		public static void GetNumberAnalogPins ()
		{
			var command = new SendCommand ((int)Command.GetNumberAnalogPins, (int)Command.GetNumberAnalogPins, 1000);
			var returnVal = _cmdMessenger.SendCommand (command);
			if (returnVal.Ok)
			{
				NumberOfAnalogPins = returnVal.ReadUInt32Arg ();
			} else
			{
				NumberOfAnalogPins = uint.MaxValue;
			}
		}

		public static void GetAnalogPinNumbers ()
		{
			var command = new SendCommand ((int)Command.GetAnalogPinNumbers, (int)Command.GetAnalogPinNumbers, 1000);
			var returnVal = _cmdMessenger.SendCommand (command);

			if (returnVal.Ok)
			{
				uint[] tmp = new uint[returnVal.Arguments.Length - 1];
				for (int i = 0; i < returnVal.Arguments.Length - 1; i++)
				{
//					tmp [i] = Convert.ToUInt32 (returnVal.Arguments [i]);

//					uint res;
//					uint.TryParse (returnVal.Arguments [i], out res);
//					tmp [i] = res;

					tmp [i] = returnVal.ReadUInt32Arg ();
				}
				_board.HardwareAnalogPins = tmp;

			} else
			{
				_board.HardwareAnalogPins = null;
			}
		}

		public static void GetDigitalBitMask ()
		{
			var command = new SendCommand ((int)Command.GetDigitalBitMask, (int)Command.GetDigitalBitMask, 1000);
			var returnVal = _cmdMessenger.SendCommand (command);
			if (returnVal.Ok)
			{
				DigitalBitMask = returnVal.ReadBinUInt32Arg ();
			} else
			{
				DigitalBitMask = 0x0;
			}
		}

		public static void GetPinOutputMask ()
		{
			var command = new SendCommand ((int)Command.GetPinOutputMask, (int)Command.GetPinOutputMask, 1000);
			var returnVal = _cmdMessenger.SendCommand (command);
			if (returnVal.Ok)
			{
				PinOutputMask = returnVal.ReadBinUInt32Arg ();
			} else
			{
				PinOutputMask = 0x0;
			}
		}

		public static void GetPinModeMask ()
		{
			var command = new SendCommand ((int)Command.GetPinModeMask, (int)Command.GetPinModeMask, 1000);
			var returnVal = _cmdMessenger.SendCommand (command);
			if (returnVal.Ok)
			{
				PinModeMask = returnVal.ReadBinUInt32Arg ();
			} else
			{
				PinModeMask = 0x0;
			}
		}

		public static void GetSDASCLRXTX ()
		{
			var command = new SendCommand ((int)Command.GetSDASCL, (int)Command.GetSDASCL, 1000);
			var returnVal = _cmdMessenger.SendCommand (command);
			if (returnVal.Ok)
			{
				Board.SDA = returnVal.ReadUInt32Arg ();
				Board.SCL = returnVal.ReadUInt32Arg ();
				Board.RX = returnVal.ReadUInt32Arg ();
				Board.TX = returnVal.ReadUInt32Arg ();
			}
		}

		#endregion
	}

	public class Board
	{
		public uint NumberOfAnalogPins = 0;
		public uint NumberOfDigitalPins = 0;

		public uint[] HardwareAnalogPins { get ; set; }

		public uint SDA = 18;
		public uint SCL = 19;
		public uint RX = 0;
		public uint TX = 1;

		public Dictionary<string,int> AnalogReferences = new Dictionary<string, int> ();

		public double AnalogReferenceVoltage;


		public string Version = "";
		public string MCU = "";
		public string Name = "";
		public string ImageFilePath = "";

		//default with Arduino UNO
		public bool UseDTR = false;

		public Board ()
		{
			AnalogReferences = new Dictionary<string,int> ();
			AnalogReferenceVoltage = 5;
			NumberOfAnalogPins = 6;
			NumberOfDigitalPins = 20;
			HardwareAnalogPins = new uint[]{ 15, 16, 17, 18, 19, 20 };
		}

		public Board (uint numberOfAnalogPins, uint numberOfDigitalPins, uint[] hardwareAnalogPins = null, Dictionary<string,int> analogReferences = null, string name = "", string version = "", string model = "", bool dtr = false)
		{
			this.NumberOfAnalogPins = numberOfAnalogPins;
			this.NumberOfDigitalPins = numberOfDigitalPins;
			if (analogReferences != null)
				this.AnalogReferences = analogReferences;

			if (hardwareAnalogPins != null)
			{
				if (hardwareAnalogPins.Length == numberOfAnalogPins)
				{
					HardwareAnalogPins = hardwareAnalogPins;
				}
			}


			this.Version = version;
			this.MCU = model;
			this.Name = name;
			this.UseDTR = dtr;
		}

		public override string ToString ()
		{
			return String.Format (
				"Name: {0}\n" +
				"Model: {1}\n" +
				"Number of analog Pins: {2}\n" +
				"Number of digital Pins: {3}\n" +
				"Analog reference voltage: {4}\n" +
				"Analog pin hardware numbers: {5}\n" +
				"SDA: {6}\n" +
				"SDC: {7}",
				Name, 
				MCU, 
				NumberOfAnalogPins, 
				NumberOfDigitalPins, 
				AnalogReferenceVoltage,
				NumberOfAnalogPins,
				SDA,
				SCL
			);
		}
	}
}
