using System;
using System.Collections.Generic;
using CommandMessenger;
using CommandMessenger.Transport.Serial;
using PrototypeBackend;
using System.IO.Ports;

namespace PrototypeBackend
{
	#region ENUMS
	/// <summary>
	/// Arduino Controller Commands
	/// </summary>
	public enum Command
	{
		Acknowledge,
		Error,
		Ready,
		Alive,
		SetPinMode,
		SetPinState,
		SetDigitalOutputPins,
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

	/// <summary>
	/// Pin modes.
	/// </summary>
	public enum PinMode
	{
		INPUT,
		OUTPUT}
	;

	/// <summary>
	/// DPin states.
	/// </summary>
	public enum DPinState
	{
		LOW,
		HIGH}
	;

	/// <summary>
	/// Pin types.
	/// </summary>
	public enum PinType
	{
		DIGITAL,
		ANALOG}
	;

	#endregion

	/// <summary>
	/// Arduino controller class. Managing the communication.
	/// </summary>
	public static class ArduinoController
	{
		#region Events

		/// <summary>
		/// Occurs when on connection changed.
		/// </summary>
		public static event EventHandler<ConnectionChangedArgs> OnConnectionChanged;

		/// <summary>
		/// Occurs when on send message.
		/// </summary>
		public static event EventHandler<CommunicationArgs> OnSendMessage;

		/// <summary>
		/// Occurs when on receive message.
		/// </summary>
		public static event EventHandler<CommunicationArgs> OnReceiveMessage;

		#endregion

		#region Properies and Member

		private static CmdMessenger _cmdMessenger;

		private static Board _board = new Board ();

		/// <summary>
		/// Gets or sets the board.
		/// </summary>
		/// <value>The board.</value>
		public static Board @Board { get { return _board; } set { _board = value; } }

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="PrototypeBackend.ArduinoController"/> auto connect.
		/// </summary>
		/// <value><c>true</c> if auto connect; otherwise, <c>false</c>.</value>
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

		//TODO kommentieren
		private static bool _autoconnect = true;

		private static System.Threading.Thread AutoConnectTimer = null;


		private static DateTime LastCommunication = DateTime.Now;

		private const double CommunicationTimeout = 2500;

		private static System.Threading.Timer ConnectionWatchdog = null;

		/// <summary>
		/// Gets a value indicating is connected.
		/// </summary>
		/// <value><c>true</c> if is connected; otherwise, <c>false</c>.</value>
		public static bool IsConnected {
			get { return isConnected; }
			private set {
				if (value != isConnected)
				{
					isConnected = value;
					if (OnConnectionChanged != null)
					{
						OnConnectionChanged.Invoke (null, new ConnectionChangedArgs (value, SerialPortName));
					}
				}
			}
		}

		/// <summary>
		/// Indication on whether a serial connetion is enabled or not.
		/// </summary>
		private static bool isConnected;

		/// <summary>
		/// Gets or sets the name of the serial port.
		/// </summary>
		/// <value>The name of the serial port.</value>
		public static string SerialPortName {
			get;
			set;
		}

		/// <summary>
		/// Gets the MCU name.
		/// </summary>
		/// <value>The MCU name.</value>
		public static string MCU {
			private set{ _board.MCU = value; }
			get{ return _board.MCU; }
		}

		/// <summary>
		/// Gets the number of digital pins available on the connected MCU.
		/// </summary>
		/// <value>The number of digital pins.</value>
		public static uint NumberOfDigitalPins {
			private set{ _board.NumberOfDigitalPins = value; }
			get { return _board.NumberOfDigitalPins; }
		}

		/// <summary>
		/// Gets the number of analog pins available on the connected MCU.
		/// </summary>
		/// <value>The number of analog pins.</value>
		public static uint NumberOfAnalogPins {
			private set { _board.NumberOfAnalogPins = value; }
			get{ return _board.NumberOfAnalogPins; }
		}

		/// <summary>
		/// Gets the analog references.
		/// </summary>
		/// <value>The analog references.</value>
		public static Dictionary<string,double> AnalogReferences {
			private set{ _board.AnalogReferences = value; }
			get { return _board.AnalogReferences; }
		}

		#endregion

		/// <summary>
		/// Init the arduinocontroller.
		/// </summary>
		/// <param name="apins">Number of analog pins</param>
		/// <param name="dpins">Number of digital pins</param>
		public static void Init (uint apins = 6, uint dpins = 20)
		{
			_board = new Board ();
			NumberOfAnalogPins = apins;
			NumberOfDigitalPins = dpins;

			OnConnectionChanged += (sender, e) =>
			{
				if (e.Connected)
				{
					GetModel ();
					GetNumberAnalogPins ();
					GetNumberDigitalPins ();
					GetAnalogPinNumbers ();
					GetSDASCL ();
				}
			};

			SerialPortName = "";
			#if FAKESERIAL
			IsConnected = true;
			#else
			isConnected = false;
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
						System.Threading.Thread.Sleep (200);
						if (IsConnected)
							break;
						Disconnect ();
						Setup (true);
						System.Threading.Thread.Sleep (200);
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

		/// <summary>
		/// Sets up a conntection to a specified serial port.
		/// </summary>
		/// <param name="Dtr">If <c>true</c> use dtr for transmition.</param>
		public static bool Setup (bool Dtr = false)
		{
			if (SerialPortName != null)
			{
				if (_cmdMessenger != null)
				{
					_cmdMessenger.Disconnect ();
					_cmdMessenger.Dispose ();
				}
				if (ConnectionWatchdog != null)
				{
					ConnectionWatchdog.Dispose ();
				}

				_cmdMessenger = new CmdMessenger (new SerialTransport () {
					CurrentSerialSettings = {
						PortName = SerialPortName,
						BaudRate = 115200,
						DataBits = 8,
						Parity = Parity.None,
						DtrEnable = Dtr
					}
				}, 512, BoardType.Bit32);

				// Attach the callbacks to the Command Messenger
				AttachCommandCallBacks ();

				// Attach to NewLinesReceived for logging purposes
				_cmdMessenger.NewLineReceived += NewLineReceived;

				// Attach to NewLineSent for logging purposes
				_cmdMessenger.NewLineSent += NewLineSent;

				// Start listening
				IsConnected = _cmdMessenger.Connect ();
				if (!IsConnected)
				{
					SerialPortName = string.Empty;
				} else
				{
					ConnectionWatchdog = new System.Threading.Timer (new System.Threading.TimerCallback (ConnectionWatchdogCallback), null, 0, 1000);
				}
				return IsConnected;
			}
			return false;
		}

		/// <summary>
		/// Disconnects and disposes instance.
		/// </summary>
		public static void Exit ()
		{
			// Stop listening
			AutoConnect = false;
			if (IsConnected)
			{
				// Dispose Command Messenger
				if (_cmdMessenger != null)
				{
					_cmdMessenger.Disconnect ();
					_cmdMessenger.Dispose ();
				}
			}

			if (ConnectionWatchdog != null)
			{
				ConnectionWatchdog.Dispose ();
			}
		}

		/// <summary>
		/// Disconnect this instance.
		/// </summary>
		public static void Disconnect ()
		{
			if (IsConnected)
			{
				IsConnected = false;
				_cmdMessenger.Disconnect ();

				if (ConnectionWatchdog != null)
				{
					ConnectionWatchdog.Dispose ();
				}

				if (AutoConnect && AutoConnectTimer.ThreadState != System.Threading.ThreadState.Running)
				{
					AutoConnectTimer.Start ();
				}
			}

		}

		/// <summary>
		/// Attemds to auto connect to a serial port.
		/// </summary>
		/// <returns><c>true</c>, if auto connect was successfull, <c>false</c> otherwise.</returns>
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

		/// <summary>
		/// Attachs the command call backs.
		/// </summary>
		private static void AttachCommandCallBacks ()
		{
			_cmdMessenger.Attach (OnUnknownCommand);
			_cmdMessenger.Attach ((int)Command.Acknowledge, OnAcknowledge);
			_cmdMessenger.Attach ((int)Command.Error, OnError);
		}

		#region CALLBACKS

		/// <summary>
		/// Raised when unknown command is received
		/// </summary>
		/// <param name="arguments">Arguments.</param>
		static void OnUnknownCommand (ReceivedCommand arguments)
		{
			#if DEBUG
			Console.WriteLine (@"Command without attached callback received");
			#endif
			LastCommunication = DateTime.Now;
		}

		/// <summary>
		/// Raised when acknowledge was received.
		/// </summary>
		/// <param name="arguments">Arguments.</param>
		static void OnAcknowledge (ReceivedCommand arguments)
		{
			#if DEBUG
			Console.WriteLine (@" Arduino is ready");
			#endif
			IsConnected = true;
//			if (OnConnectionChanged != null) {
//				OnConnectionChanged.Invoke (null, new ConnectionChangedArgs (true, SerialPortName));
//			}
			LastCommunication = DateTime.Now;
		}

		/// <summary>
		/// Raised when error was received.
		/// </summary>
		/// <param name="arguments">Arguments.</param>
		static void OnError (ReceivedCommand arguments)
		{
			#if DEBUG
			Console.WriteLine (@"Arduino has experienced an error");
			#endif
			LastCommunication = DateTime.Now;
		}

		/// <summary>
		/// Raised when a new line was received.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		private static void NewLineReceived (object sender, CommandEventArgs e)
		{
			#if DEBUG
			Console.WriteLine (@"Received > " + e.Command.CommandString ());
			#endif

			if (OnReceiveMessage != null)
			{
				OnReceiveMessage.Invoke (null, new CommunicationArgs (e.Command.CommandString ()));
			}
			LastCommunication = DateTime.Now;
		}

		/// <summary>
		/// Raised when a new line was send.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		private static void NewLineSent (object sender, CommandEventArgs e)
		{
			#if DEBUG
			Console.WriteLine (DateTime.Now + @": Sent > " + e.Command.CommandString ());
			#endif
			if (OnSendMessage != null)
			{
				OnSendMessage.Invoke (null, new CommunicationArgs (e.Command.CommandString ()));
			}
			LastCommunication = DateTime.Now;
		}

		#endregion

		#region SETTER

		/// <summary>
		/// Sets the pin modes.
		/// </summary>
		/// <param name="inputs">Inputs.</param>
		/// <param name="outputs">Outputs.</param>
		public static void SetPinModes (uint[] inputs, uint[]outputs)
		{
			for (int i = 0; i < inputs.Length; i++)
			{
				SetPinMode (inputs [i], PinMode.INPUT);
			}
			for (int i = 0; i < outputs.Length; i++)
			{
				SetPinMode (outputs [i], PinMode.OUTPUT);
			}
		}

		/// <summary>
		/// Sets the pin mode.
		/// </summary>
		/// <param name="nr">Nr.</param>
		/// <param name="mode">Mode.</param>
		public static void SetPinMode (uint nr, PinMode mode)
		{
			#if !FAKESERIAL
			var command = new SendCommand ((int)Command.SetPinMode, nr);
			command.AddArgument ((Int16)mode);
			_cmdMessenger.SendCommand (command);
			#endif
		}

		/// <summary>
		/// Set the digital output pins.
		/// </summary>
		/// <param name="conditions">The conditions are packed as follows: conditions[0] == pins[0-15], conditions[1] = pins[16,23],...</param>
		public static void SetDigitalOutputPins (UInt16[] conditions)
		{
			var command = new SendCommand ((int)Command.SetDigitalOutputPins);
			for (int i = 0; i < conditions.Length; i++)
			{
				command.AddArgument (conditions [i]);
			}
			_cmdMessenger.SendCommand (command);
		}

		/// <summary>
		/// Set the digital output pins.
		/// </summary>
		/// <param name="conditions">The conditions are packed as follows: conditions[0] == pins[0-31], conditions[1] = pins[32,63]</param>
		public static void SetDigitalOutputPins (params uint[] conditions)
		{
			UInt16[] parts = new UInt16[conditions.Length * 2];

			int pos = 0;
			while (pos < conditions.Length)
			{
				parts [pos] = (UInt16)conditions [pos];
				parts [pos + 1] = (UInt16)(conditions [pos] >> 16);
				pos += 2;
			}

			SetDigitalOutputPins (parts);
		}

		/// <summary>
		/// Sets the digital output pins.
		/// </summary>
		/// <param name="conditions">Conditions (LSB = Pin0)</param>
		public static void SetDigitalOutputPins (UInt64 conditions)
		{
			var byteparts = BitConverter.GetBytes (conditions);
			UInt16[] parts = new UInt16 [4];
			parts [0] = BitConverter.ToUInt16 (byteparts, 0);
			parts [1] = BitConverter.ToUInt16 (byteparts, 2);
			parts [2] = BitConverter.ToUInt16 (byteparts, 4);
			parts [3] = BitConverter.ToUInt16 (byteparts, 6);

			SetDigitalOutputPins (parts);
		}

		/// <summary>
		/// Sets the state of the pin.
		/// </summary>
		/// <param name="nr">Nr.</param>
		/// <param name="state">State.</param>
		public static void SetPinState (uint nr, DPinState state)
		{
			var command = new SendCommand ((int)Command.SetPinState, (int)Command.SetPinState, 50);
			command.AddArgument (nr);
			command.AddArgument ((Int16)state);
			var ret = _cmdMessenger.SendCommand (command);
			if (!ret.Ok)
			{
				Console.Error.WriteLine ("SetPinState " + nr + " " + state + " failed");
				LastCommunication = DateTime.Now;
			}
		}

		/// <summary>
		/// Sets the pin.
		/// </summary>
		/// <param name="nr">Nr.</param>
		/// <param name="mode">Mode.</param>
		/// <param name="state">State.</param>
		public static void SetPin (uint nr, PinMode mode, DPinState state)
		{
			#if !FAKESERIAL
			var command = new SendCommand ((int)Command.SetPin, (int)Command.SetPin, 100);
			command.AddArgument ((UInt16)nr);
			command.AddArgument ((Int16)mode);
			command.AddArgument ((Int16)state);
			var ret = _cmdMessenger.SendCommand (command);
			if (ret.Ok)
			{
				if (!(nr == (uint)ret.ReadInt32Arg () && (Int16)mode == ret.ReadInt16Arg () && (Int16)state == ret.ReadInt16Arg ()))
				{
					Console.Error.WriteLine (DateTime.Now.ToString ("HH:mm:ss tt zz") + "\t" + nr + "\t" + mode + "\t" + state);
				}
				LastCommunication = DateTime.Now;
			}
			#endif
		}

		/// <summary>
		/// Sets the analog reference voltage.
		/// </summary>
		/// <param name="AnalogReference">Analog reference.</param>
		public static void SetAnalogReference (int AnalogReference)
		{
			_board.AnalogReferenceVoltage = AnalogReference;
			var command = new SendCommand ((int)Command.SetAnalogReference);
			command.AddArgument (AnalogReference);
			_cmdMessenger.SendCommand (command);
		}

		/// <summary>
		/// Sets the analog pin.
		/// </summary>
		/// <param name="Pin">Pin.</param>
		/// <param name="Val">Value.</param>
		public static void SetAnalogPin (int Pin, int Val)
		{
			var command = new SendCommand ((int)Command.SetAnalogPin, Pin);
			command.AddArgument (Val);
			_cmdMessenger.SendCommand (command);
		}

		#endregion

		#region GETTER

		/// <summary>
		/// Reads analog pin.
		/// </summary>
		/// <returns>The analog pin.</returns>
		/// <param name="nr">Nr.</param>
		public static double ReadAnalogPin (uint nr)
		{
			return ReadAnalogPin (new uint[]{ nr }) [0];
		}

		/// <summary>
		/// Reads analog pins.
		/// </summary>
		/// <returns>The analog pin.</returns>
		/// <param name="nr">Nr.</param>
		public static double[] ReadAnalogPin (uint[] nr)
		{
			var command = new SendCommand ((int)Command.ReadAnalogPin, (int)Command.ReadAnalogPin, 100);
			command.AddArgument (nr.Length);
			foreach (int i in nr)
			{
				command.AddArgument (i);
			}
			double[] results = new double[nr.Length];
			var result = _cmdMessenger.SendCommand (command);
			if (result.Ok)
			{
				try
				{
					for (int i = 0; i < nr.Length; i++)
					{
						results [i] = Board.RAWToVolt (result.ReadFloatArg ());
					}
				} catch (Exception ex)
				{
					Console.Error.WriteLine (ex);
				}
				LastCommunication = DateTime.Now;
				return results;
			} else
			{
				for (int i = 0; i < results.Length; i++)
				{
					results [i] = double.NaN;
				}
				return results;
			}
		}

		/// <summary>
		/// Reads DPin state.
		/// </summary>
		/// <returns>The pin.</returns>
		/// <param name="nr">Nr.</param>
		public static DPinState ReadDPinState (uint nr)
		{
			var command = new SendCommand ((int)Command.ReadPin, (int)Command.ReadPin, 500);
			command.AddArgument (nr);
			var result = _cmdMessenger.SendCommand (command);
			if (result.Ok)
			{
				LastCommunication = DateTime.Now;
				return (result.ReadBinInt16Arg () == (int)DPinState.HIGH) ? DPinState.HIGH : DPinState.LOW;
			}
			return DPinState.LOW;
		}

		/// <summary>
		/// Gets the analog reference options.
		/// </summary>
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
				LastCommunication = DateTime.Now;
			}
		}

		/// <summary>
		/// Gets the MCU.
		/// </summary>
		public static void GetModel ()
		{
			var command = new SendCommand ((int)Command.GetModel, (int)Command.GetModel, 1000);
			var returnVal = _cmdMessenger.SendCommand (command);
			if (returnVal.Ok)
			{
				MCU = returnVal.ReadStringArg ().ToLower ();
				LastCommunication = DateTime.Now;
			}
		}

		/// <summary>
		/// Gets the number digital pins.
		/// </summary>
		public static void GetNumberDigitalPins ()
		{
			var command = new SendCommand ((int)Command.GetNumberDigitalPins, (int)Command.GetNumberDigitalPins, 1000);
			var returnVal = _cmdMessenger.SendCommand (command);
			if (returnVal.Ok)
			{
				NumberOfDigitalPins = returnVal.ReadUInt32Arg ();
				LastCommunication = DateTime.Now;
			} else
			{
				//in case the arduino did not respond
				NumberOfDigitalPins = uint.MaxValue;
			}
		}

		/// <summary>
		/// Gets the number analog pins.
		/// </summary>
		public static void GetNumberAnalogPins ()
		{
			var command = new SendCommand ((int)Command.GetNumberAnalogPins, (int)Command.GetNumberAnalogPins, 1000);
			var returnVal = _cmdMessenger.SendCommand (command);
			if (returnVal.Ok)
			{
				NumberOfAnalogPins = returnVal.ReadUInt32Arg ();
				LastCommunication = DateTime.Now;
			} else
			{
				NumberOfAnalogPins = uint.MaxValue;
			}
		}

		/// <summary>
		/// Gets the hardware pin numbers of analog pins.
		/// </summary>
		public static void GetAnalogPinNumbers ()
		{
			var command = new SendCommand ((int)Command.GetAnalogPinNumbers, (int)Command.GetAnalogPinNumbers, 1000);
			var returnVal = _cmdMessenger.SendCommand (command);

			if (returnVal.Ok)
			{
				uint[] tmp = new uint[returnVal.Arguments.Length - 1];
				for (int i = 0; i < returnVal.Arguments.Length - 1; i++)
				{
					tmp [i] = returnVal.ReadUInt32Arg ();
				}
				_board.HardwareAnalogPins = tmp;
				LastCommunication = DateTime.Now;
			} else
			{
				_board.HardwareAnalogPins = null;
			}
		}

		/// <summary>
		/// Gets the pin numbers of SDA abd SCL enabled pins.
		/// </summary>
		public static void GetSDASCL ()
		{
			var command = new SendCommand ((int)Command.GetSDASCL, (int)Command.GetSDASCL, 1000);
			var returnVal = _cmdMessenger.SendCommand (command);
			if (returnVal.Ok)
			{
				Board.SDA = new uint[]{ returnVal.ReadUInt32Arg () };
				Board.SCL = new uint[]{ returnVal.ReadUInt32Arg () };

				LastCommunication = DateTime.Now;
			}
		}

		#endregion

		/// <summary>
		/// the watchdog callback to keep track of the connection status and possible disconnects.
		/// </summary>
		/// <param name="sender">Sender.</param>
		private static void ConnectionWatchdogCallback (object sender)
		{
			if ((DateTime.Now.Subtract (LastCommunication).TotalMilliseconds > CommunicationTimeout) && IsConnected)
			{
				var command = new SendCommand ((int)Command.Alive, (int)Command.Alive, 1000);
				var returnVal = _cmdMessenger.SendCommand (command);

				Console.Write ("ping");

				if (returnVal.Ok)
				{
					LastCommunication = DateTime.Now;
					Console.Write (" -> OK\n");
				} else
				{
					IsConnected = false;
					Console.Error.Write (" -> FAIL\n");
				}
			}
		}
	}
}
