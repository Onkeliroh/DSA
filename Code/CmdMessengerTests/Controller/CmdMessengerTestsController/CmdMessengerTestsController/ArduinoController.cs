using System;
using CommandMessenger;
using CommandMessenger.Transport.Serial;
using System.Collections.Generic;

namespace ArduinoController
{
	enum Command
	{
		Acknowledge,
		Error,
		SetPinMode,
		SetPinState,
		SetAnalogPin,
		SetAnalogPinMode,
		SetPin,
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

	public class ArduinoController
	{
		private CmdMessenger _cmdMessenger;

		public bool IsConnected {
			get;
			private set;
		}

		public string SerialPortName {
			private get;
			set;
		}

		public ArduinoController ()
		{
			IsConnected = false;
		}

		public List<List<float>> AnalogValues {
			get;
			private set;
		}

		public string Version {
			private set;
			get;
		}

		public string Model {
			private set;
			get;
		}

		public int NumberOfDigitalPins {
			private set;
			get;
		}

		public int NumberOfAnalogPins {
			private set;
			get;
		}

		public UInt32 DigitalBitMask {
			private set;
			get;
		}

		// ------------------ MAIN  ----------------------

		// Setup function
		public void Setup ()
		{
			AnalogValues = new List<List<float>> ();

			_cmdMessenger = new CmdMessenger (new SerialTransport () {
				CurrentSerialSettings = {
					PortName = SerialPortName,
					BaudRate = 115200,
					DtrEnable = false
				}
			}, BoardType.Bit16);

			// Attach the callbacks to the Command Messenger
			AttachCommandCallBacks ();

			// Attach to NewLinesReceived for logging purposes
			_cmdMessenger.NewLineReceived += NewLineReceived;

			// Attach to NewLineSent for logging purposes
			_cmdMessenger.NewLineSent += NewLineSent;                       

			// Start listening
			IsConnected = _cmdMessenger.Connect ();
		}

		// Exit function
		public void Exit ()
		{
			// Stop listening
			_cmdMessenger.Disconnect ();

			// Dispose Command Messenger
			_cmdMessenger.Dispose ();
		}

		public void Disconnect ()
		{
			IsConnected = false;
			_cmdMessenger.Disconnect ();
		}

		/// Attach command call backs. 
		private void AttachCommandCallBacks ()
		{
			_cmdMessenger.Attach (OnUnknownCommand);
			_cmdMessenger.Attach ((int)Command.Acknowledge, OnAcknowledge);
			_cmdMessenger.Attach ((int)Command.Error, OnError);
			_cmdMessenger.Attach ((int)Command.ReadAnalogPinResult, OnReadAnalogResult);
			_cmdMessenger.Attach ((int)Command.GetVersion, OnGetVersion);
			_cmdMessenger.Attach ((int)Command.GetModel, OnGetModel);
			_cmdMessenger.Attach ((int)Command.GetNumberDigitalPins, OnGetNumberDigitalPins);
			_cmdMessenger.Attach ((int)Command.GetNumberAnalogPins, OnGetNumberAnalogPins);
			_cmdMessenger.Attach ((int)Command.GetDigitalBitMask, OnGetDigitalBitMask);
		}


		// ------------------  CALLBACKS ---------------------

		// Called when a received command has no attached function.
		// In a WinForm application, console output gets routed to the output panel of your IDE
		void OnUnknownCommand (ReceivedCommand arguments)
		{            
			Console.WriteLine (@"Command without attached callback received");
		}

		// Callback function that prints that the Arduino has acknowledged
		void OnAcknowledge (ReceivedCommand arguments)
		{
			Console.WriteLine (@" Arduino is ready");
		}

		// Callback function that prints that the Arduino has experienced an error
		void OnError (ReceivedCommand arguments)
		{
			Console.WriteLine (@"Arduino has experienced an error");
		}

		// Log received line to console
		private void NewLineReceived (object sender, CommandEventArgs e)
		{
			Console.WriteLine (@"Received > " + e.Command.CommandString ());
		}

		// Log sent line to console
		private void NewLineSent (object sender, CommandEventArgs e)
		{
			Console.WriteLine (@"Sent > " + e.Command.CommandString ());
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

			if (AnalogValues.Count < nr)
			{
				while (AnalogValues.Count <= nr)
				{
					AnalogValues.Add (new List<float> ());
				}
			}
		}

		private void OnReadAnalogResult (ReceivedCommand args)
		{
			int pin = (int)args.ReadFloatArg ();
			float val = args.ReadFloatArg ();

			AnalogValues [pin].Add (val);
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
			var command = new SendCommand ((int)Command.GetVersion);
			_cmdMessenger.SendCommand (command);
		}

		private void OnGetVersion (ReceivedCommand args)
		{
			Version = args.ReadStringArg ();
		}

		public void GetModel ()
		{
			var command = new SendCommand ((int)Command.GetModel);
			_cmdMessenger.SendCommand (command);
		}

		private void OnGetModel (ReceivedCommand args)
		{
			Model = args.ReadStringArg ();
		}

		public void GetNumberDigitalPins ()
		{
			var command = new SendCommand ((int)Command.GetNumberDigitalPins);
			_cmdMessenger.SendCommand (command);
		}

		private void OnGetNumberDigitalPins (ReceivedCommand args)
		{
			NumberOfDigitalPins = args.ReadInt32Arg ();
		}

		public void GetNumberAnalogPins ()
		{
			var command = new SendCommand ((int)Command.GetNumberAnalogPins);
			_cmdMessenger.SendCommand (command);
		}

		private void OnGetNumberAnalogPins (ReceivedCommand args)
		{
			NumberOfAnalogPins = args.ReadInt32Arg ();
		}

		public void GetDigitalBitMask ()
		{
			var command = new SendCommand ((int)Command.GetDigitalBitMask);
			_cmdMessenger.SendCommand (command);
		}

		public void OnGetDigitalBitMask (ReceivedCommand args)
		{
			DigitalBitMask = args.ReadBinUInt32Arg ();
		}
	}
}
