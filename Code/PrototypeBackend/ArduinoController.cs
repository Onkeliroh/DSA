using System;
using System.Collections.Generic;
using System.Reflection;
using CommandMessenger;
using CommandMessenger.Transport.Serial;
using MonoDevelop.Components;
using PrototypeBackend;
using System.Xml.Serialization;
using System.IO;

namespace ArduinoController
{
	#region ENUMS
	public enum Command
	{
		Acknowledge,
		Error,
		SetPinMode,
		SetPinState,
		SetAnalogPin,
		SetAnalogPinMode,
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

	public  class ArduinoController
	{
		private CmdMessenger _cmdMessenger;
		private Board _board;

		public event EventHandler<EventArgs> OnConnection;
		public event EventHandler<ControllerAnalogEventArgs> NewAnalogValue;
		public event EventHandler<ControllerDigitalEventArgs> NewDigitalValue;

		public bool IsConnected {
			get;
			private set;
		}

		public string SerialPortName {
			private get;
			set;
		}

		public List<List<float>> AnalogValues {
			get;
			private set;
		}

		public string Version {
			private set{ _board.Version = value; }
			get{ return _board.Version; }
		}

		public string Model {
			private set{ _board.Model = Model; }
			get{ return _board.Model; }
		}

		public uint NumberOfDigitalPins {
			private set{ _board.NumberOfDigitalPins = value; }
			get { return _board.NumberOfDigitalPins; }
		}

		public uint NumberOfAnalogPins {
			private set { _board.NumberOfAnalogPins = value; }
			get{ return _board.NumberOfAnalogPins; }
		}

		public UInt32 DigitalBitMask {
			private set;
			get;
		}

		public UInt32 PinOutputMask {
			private set;
			get;
		}

		public UInt32 PinModeMask {
			private set;
			get;
		}

		public Dictionary<string,int> AnalogReferences {
			private set{ _board.AnalogReferences = value; }
			get { return _board.AnalogReferences; }
		}

		// ------------------ MAIN  ----------------------

		//Constructor
		public ArduinoController () : this (6, 20)
		{
		}

		public ArduinoController (uint AnalogPinsCount, uint DigitalPinsCount)
		{
			_board = new Board ();
			NumberOfAnalogPins = AnalogPinsCount;
			NumberOfDigitalPins = DigitalPinsCount;
			#if FAKESERIAL
			IsConnected = true;
			#else
			IsConnected = false;
			#endif
		}

		// Setup function
		public void Setup ()
		{
			AnalogValues = new List<List<float>> ();
			AnalogReferences = new Dictionary<string, int> ();


			_cmdMessenger = new CmdMessenger (new SerialTransport () {
				CurrentSerialSettings = {
					PortName = SerialPortName,
					BaudRate = 115200,
					DtrEnable = true  //bei UNO auf false ändern 
				}
			}, BoardType.Bit16);

			// Attach the callbacks to the Command Messenger
			AttachCommandCallBacks ();

			// Attach to NewLinesReceived for logging purposes
			_cmdMessenger.NewLineReceived += NewLineReceived;

			// Attach to NewLineSent for logging purposes
			_cmdMessenger.NewLineSent += NewLineSent;                       

			#if !FAKESERIAL
			// Start listening
			IsConnected = _cmdMessenger.Connect ();
			if (IsConnected)
			{
				if (OnConnection != null)
				{
					try
					{
						OnConnection.Invoke (this, null);
					} catch (Exception e)
					{
						Console.WriteLine (e);
					}
				}
			}
			#endif
		}

		// Exit function
		public void Exit ()
		{
			#if !FAKESERIAL
			// Stop listening
			_cmdMessenger.Disconnect ();

			#endif
			// Dispose Command Messenger
			_cmdMessenger.Dispose ();
		}

		public void Disconnect ()
		{
			if (IsConnected)
			{
				IsConnected = false;
				#if !FAKESERIAL
				_cmdMessenger.Disconnect ();
				#endif
			}

		}

		/// Attach command call backs. 
		private void AttachCommandCallBacks ()
		{
			_cmdMessenger.Attach (OnUnknownCommand);
			_cmdMessenger.Attach ((int)Command.Acknowledge, OnAcknowledge);
			_cmdMessenger.Attach ((int)Command.Error, OnError);
			_cmdMessenger.Attach ((int)Command.ReadAnalogPinResult, OnReadAnalogResult);
		}


		// ------------------  CALLBACKS ---------------------

		// Called when a received command has no attached function.
		// In a WinForm application, console output gets routed to the output panel of your IDE
		void OnUnknownCommand (ReceivedCommand arguments)
		{            
			#if DEBUG
			Console.WriteLine (@"Command without attached callback received");
			#endif
		}

		// Callback function that prints that the Arduino has acknowledged
		void OnAcknowledge (ReceivedCommand arguments)
		{
			#if DEBUG
			Console.WriteLine (@" Arduino is ready");
			#endif
		}

		// Callback function that prints that the Arduino has experienced an error
		void OnError (ReceivedCommand arguments)
		{
			#if DEBUG
			Console.WriteLine (@"Arduino has experienced an error");
			#endif
		}

		// Log received line to console
		private void NewLineReceived (object sender, CommandEventArgs e)
		{
			#if DEBUG
			Console.WriteLine (@"Received > " + e.Command.CommandString ());
			#endif
		}

		// Log sent line to console
		private void NewLineSent (object sender, CommandEventArgs e)
		{
			#if DEBUG
			Console.WriteLine (@"Sent > " + e.Command.CommandString ());
			#endif
		}

		public void SetPinMode (int nr, PinMode mode)
		{
			var command = new SendCommand ((int)Command.SetPinMode, nr);
			command.AddArgument ((Int16)mode);
			_cmdMessenger.SendCommand (command);
		}

		public void SetPinState (int nr, DPinState state)
		{
			var command = new SendCommand ((int)Command.SetPinMode, nr);
			command.AddArgument ((Int16)state);
			_cmdMessenger.SendCommand (command);
		}

		public void SetPin (int nr, PinMode mode, DPinState state)
		{
			var command = new SendCommand ((int)Command.SetPin, nr);
			command.AddArgument ((Int16)mode);
			command.AddArgument ((Int16)state);
			_cmdMessenger.SendCommand (command);
		}

		public void ReadAnalogPin (int nr)
		{
			var command = new SendCommand ((int)Command.ReadAnalogPin);
			command.AddArgument (nr);
			_cmdMessenger.SendCommand (command);

		}

		private void OnReadAnalogResult (ReceivedCommand args)
		{
			int pin = (int)args.ReadFloatArg ();
			float val = args.ReadFloatArg ();
			NewAnalogValue.Invoke (this, new ControllerAnalogEventArgs (pin, val));
		}

		public void SetAnalogPinMode (int Pin, PinMode mode)
		{
			var command = new SendCommand ((int)Command.SetAnalogPinMode, Pin);
			command.AddArgument ((Int16)mode);
			_cmdMessenger.SendCommand (command);
		}

		public void SetAnalogPin (int Pin, int Val)
		{
			var command = new SendCommand ((int)Command.SetAnalogPin, Pin);
			command.AddArgument (Val);
			_cmdMessenger.SendCommand (command);
		}

		public void GetVersion ()
		{
			var command = new SendCommand ((int)Command.GetVersion, (int)Command.GetVersion, 1000);
			var returnVal = _cmdMessenger.SendCommand (command);
			if (returnVal.Ok)
			{
				Version = returnVal.ReadStringArg ();
			}
		}

		public void GetAnalogReference ()
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

		private void OnGetVersion (ReceivedCommand args)
		{
			Version = args.ReadStringArg ();
		}

		public void GetModel ()
		{
			var command = new SendCommand ((int)Command.GetModel, (int)Command.GetModel, 1000);
			var returnVal = _cmdMessenger.SendCommand (command);
			if (returnVal.Ok)
			{
				Model = returnVal.ReadBinStringArg ();
			}
		}

		private void OnGetModel (ReceivedCommand args)
		{
			Model = args.ReadBinStringArg ();
		}

		public void GetNumberDigitalPins ()
		{
			var command = new SendCommand ((int)Command.GetNumberDigitalPins, (int)Command.GetNumberDigitalPins, 1000);
			var returnVal = _cmdMessenger.SendCommand (command);
			if (returnVal.Ok)
			{
				NumberOfDigitalPins = returnVal.ReadUInt32Arg ();
			}
		}

		private void OnGetNumberDigitalPins (ReceivedCommand args)
		{
			NumberOfDigitalPins = args.ReadUInt32Arg ();
		}

		public void GetNumberAnalogPins ()
		{
			var command = new SendCommand ((int)Command.GetNumberAnalogPins, (int)Command.GetNumberAnalogPins, 1000);
			var returnVal = _cmdMessenger.SendCommand (command);
			if (returnVal.Ok)
			{
				NumberOfAnalogPins = returnVal.ReadUInt32Arg ();
			}
		}

		private void OnGetNumberAnalogPins (ReceivedCommand args)
		{
			NumberOfAnalogPins = args.ReadUInt32Arg ();
		}

		public void GetDigitalBitMask ()
		{
			var command = new SendCommand ((int)Command.GetDigitalBitMask, (int)Command.GetDigitalBitMask, 1000);
			var returnVal = _cmdMessenger.SendCommand (command);
			if (returnVal.Ok)
			{
				DigitalBitMask = returnVal.ReadBinUInt32Arg ();
			}
		}

		public void OnGetDigitalBitMask (ReceivedCommand args)
		{
			DigitalBitMask = args.ReadBinUInt32Arg ();
		}

		public void GetPinOutputMask ()
		{
			var command = new SendCommand ((int)Command.GetPinOutputMask, (int)Command.GetPinOutputMask, 1000);
			var returnVal = _cmdMessenger.SendCommand (command);
			if (returnVal.Ok)
			{
				PinOutputMask = returnVal.ReadBinUInt32Arg ();
			}
		}

		private void OnGetPinOutputMask (ReceivedCommand args)
		{
			PinOutputMask = args.ReadBinUInt32Arg ();
		}

		public void GetPinModeMask ()
		{
			var command = new SendCommand ((int)Command.GetPinModeMask, (int)Command.GetPinModeMask, 1000);
			var returnVal = _cmdMessenger.SendCommand (command);
			if (returnVal.Ok)
			{
				PinModeMask = returnVal.ReadBinUInt32Arg ();
				Console.WriteLine (PinModeMask);
			}
		}

		private void OnGetPinModeMask (ReceivedCommand args)
		{
			var val = args.ReadUInt32Arg ();
			PinModeMask = val;
		}
	}

	public class Board
	{
		public uint NumberOfAnalogPins = 0;
		public uint NumberOfDigitalPins = 0;
		public Dictionary<string,int> AnalogReferences = new Dictionary<string, int> ();
		public string Version = "";
		public string Model = "";
		public string Name = "";

		public Board ()
		{
		}

		public Board (uint numberOfAnalogPins, uint numberOfDigitalPins, Dictionary<string,int> analogReferences, string version, string model, string name)
		{
			this.NumberOfAnalogPins = numberOfAnalogPins;
			this.NumberOfDigitalPins = numberOfDigitalPins;
			this.AnalogReferences = analogReferences;
			this.Version = version;
			this.Model = model;
			this.Name = name;
		}

		//		public string ToXml ()
		//		{
		//			XmlSerializer tmp = new XmlSerializer (typeof(Board));
		//			string returnstring = "";
		//			TextWriter tw = new StreamWriter (returnstring);
		//			tmp.Serialize (tw, this);
		//			tw.Close ();
		//			return returnstring;
		//		}
	}
}
