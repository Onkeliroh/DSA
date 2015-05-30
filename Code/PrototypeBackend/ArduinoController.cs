﻿using System;
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
		GetPinOutputMask,
		GetPinModeMask,
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

		public uint NumberOfDigitalPins {
			private set;
			get;
		}

		public uint NumberOfAnalogPins {
			private set;
			get;
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

		// ------------------ MAIN  ----------------------

		// Setup function
		public void Setup ()
		{
			AnalogValues = new List<List<float>> ();

			_cmdMessenger = new CmdMessenger (new SerialTransport () {
				CurrentSerialSettings = {
					PortName = SerialPortName,
					BaudRate = 115200,
					DtrEnable = true  //bei UNO ändern 
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
//			_cmdMessenger.Attach ((int)Command.GetVersion, OnGetVersion);
//			_cmdMessenger.Attach ((int)Command.GetModel, OnGetModel);
//			_cmdMessenger.Attach ((int)Command.GetNumberDigitalPins, OnGetNumberDigitalPins);
//			_cmdMessenger.Attach ((int)Command.GetNumberAnalogPins, OnGetNumberAnalogPins);
//			_cmdMessenger.Attach ((int)Command.GetDigitalBitMask, OnGetDigitalBitMask);
//			_cmdMessenger.Attach ((int)Command.GetPinOutputMask, OnGetPinModeMask);
//			_cmdMessenger.Attach ((int)Command.GetPinModeMask, OnGetPinModeMask);
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
			var command = new SendCommand ((int)Command.GetVersion, (int)Command.GetVersion, 1000);
			var returnVal = _cmdMessenger.SendCommand (command);
			if (returnVal.Ok)
			{
				Version = returnVal.ReadStringArg ();
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
				NumberOfAnalogPins = returnVal.ReadBinUInt32Arg ();
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
//			_cmdMessenger.SendCommand (command);
		}

		private void OnGetPinModeMask (ReceivedCommand args)
		{
			var val = args.ReadUInt32Arg ();
			PinModeMask = val;
		}
	}
}