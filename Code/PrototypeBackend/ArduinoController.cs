﻿using System;
using System.Collections.Generic;
using System.Reflection;
using CommandMessenger;
using CommandMessenger.Transport.Serial;
using MonoDevelop.Components;
using PrototypeBackend;
using System.Xml.Serialization;
using System.IO;

namespace PrototypeBackend
{
	#region ENUMS
	public enum Command
	{
		Acknowledge,
		Error,
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
		private static CmdMessenger _cmdMessenger;
		private static Board _board = new Board ();

		public static event EventHandler<EventArgs> OnConnection;
		public static event EventHandler<ControllerAnalogEventArgs> NewAnalogValue;
		public static event EventHandler<ControllerDigitalEventArgs> NewDigitalValue;

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
			private get;
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

		public static string Model {
			private set{ _board.Model = Model; }
			get{ return _board.Model; }
		}

		public static uint NumberOfDigitalPins {
			private set{ _board.NumberOfDigitalPins = value; }
			get { return _board.NumberOfDigitalPins; }
		}

		public static uint NumberOfAnalogPins {
			private set { _board.NumberOfAnalogPins = value; }
			get{ return _board.NumberOfAnalogPins; }
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

		// ------------------ MAIN  ----------------------

		public static void Init (uint apins = 6, uint dpins = 20)
		{
			_board = new Board ();
			NumberOfAnalogPins = apins;
			NumberOfDigitalPins = dpins;
			#if FAKESERIAL
			IsConnected = true;
			#else
			IsConnected = false;
			#endif
		}

		// Setup function
		public static void Setup ()
		{
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
						OnConnection.Invoke (null, null);
					} catch (Exception e)
					{
						Console.WriteLine (e);
					}
				}
			}
			#endif
		}

		// Exit function
		public static void Exit ()
		{
			#if !FAKESERIAL
			// Stop listening
			_cmdMessenger.Disconnect ();

			#endif
			// Dispose Command Messenger
			_cmdMessenger.Dispose ();
		}

		public static void Disconnect ()
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
		private static void AttachCommandCallBacks ()
		{
			_cmdMessenger.Attach (OnUnknownCommand);
			_cmdMessenger.Attach ((int)Command.Acknowledge, OnAcknowledge);
			_cmdMessenger.Attach ((int)Command.Error, OnError);
		}


		// ------------------  CALLBACKS ---------------------

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
		}

		// Callback function that prints that the Arduino has experienced an error
		static void OnError (ReceivedCommand arguments)
		{
			#if DEBUG
			Console.WriteLine (@"Arduino has experienced an error");
			#endif
		}

		// Log received line to console
		private static void NewLineReceived (object sender, CommandEventArgs e)
		{
			#if DEBUG
			Console.WriteLine (@"Received > " + e.Command.CommandString ());
			#endif
		}

		// Log sent line to console
		private static void NewLineSent (object sender, CommandEventArgs e)
		{
			#if DEBUG
			Console.WriteLine (@"Sent > " + e.Command.CommandString ());
			#endif
		}

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
			var command = new SendCommand ((int)Command.SetPin, nr);
			command.AddArgument ((Int16)mode);
			command.AddArgument ((Int16)state);
			_cmdMessenger.SendCommand (command);
		}

		public static void SetAnalogReference (int AnalogReference)
		{
			_board.AnalogReference = AnalogReference;
			var command = new SendCommand ((int)Command.SetAnalogReference);
			command.AddArgument (AnalogReference);
			_cmdMessenger.SendCommand (command);
		}

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

		public static void SetAnalogPin (int Pin, int Val)
		{
			var command = new SendCommand ((int)Command.SetAnalogPin, Pin);
			command.AddArgument (Val);
			_cmdMessenger.SendCommand (command);
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

		private static void OnGetVersion (ReceivedCommand args)
		{
			Version = args.ReadStringArg ();
		}

		public static void GetModel ()
		{
			var command = new SendCommand ((int)Command.GetModel, (int)Command.GetModel, 1000);
			var returnVal = _cmdMessenger.SendCommand (command);
			if (returnVal.Ok)
			{
				Model = returnVal.ReadBinStringArg ();
			}
		}

		private static void OnGetModel (ReceivedCommand args)
		{
			Model = args.ReadBinStringArg ();
		}

		public static void GetNumberDigitalPins ()
		{
			var command = new SendCommand ((int)Command.GetNumberDigitalPins, (int)Command.GetNumberDigitalPins, 1000);
			var returnVal = _cmdMessenger.SendCommand (command);
			if (returnVal.Ok)
			{
				NumberOfDigitalPins = returnVal.ReadUInt32Arg ();
			}
		}

		private static void OnGetNumberDigitalPins (ReceivedCommand args)
		{
			NumberOfDigitalPins = args.ReadUInt32Arg ();
		}

		public static void GetNumberAnalogPins ()
		{
			var command = new SendCommand ((int)Command.GetNumberAnalogPins, (int)Command.GetNumberAnalogPins, 1000);
			var returnVal = _cmdMessenger.SendCommand (command);
			if (returnVal.Ok)
			{
				NumberOfAnalogPins = returnVal.ReadUInt32Arg ();
			}
		}

		private static void OnGetNumberAnalogPins (ReceivedCommand args)
		{
			NumberOfAnalogPins = args.ReadUInt32Arg ();
		}

		public static void GetDigitalBitMask ()
		{
			var command = new SendCommand ((int)Command.GetDigitalBitMask, (int)Command.GetDigitalBitMask, 1000);
			var returnVal = _cmdMessenger.SendCommand (command);
			if (returnVal.Ok)
			{
				DigitalBitMask = returnVal.ReadBinUInt32Arg ();
			}
		}

		public static void OnGetDigitalBitMask (ReceivedCommand args)
		{
			DigitalBitMask = args.ReadBinUInt32Arg ();
		}

		public static void GetPinOutputMask ()
		{
			var command = new SendCommand ((int)Command.GetPinOutputMask, (int)Command.GetPinOutputMask, 1000);
			var returnVal = _cmdMessenger.SendCommand (command);
			if (returnVal.Ok)
			{
				PinOutputMask = returnVal.ReadBinUInt32Arg ();
			}
		}

		private static void OnGetPinOutputMask (ReceivedCommand args)
		{
			PinOutputMask = args.ReadBinUInt32Arg ();
		}

		public static void GetPinModeMask ()
		{
			var command = new SendCommand ((int)Command.GetPinModeMask, (int)Command.GetPinModeMask, 1000);
			var returnVal = _cmdMessenger.SendCommand (command);
			if (returnVal.Ok)
			{
				PinModeMask = returnVal.ReadBinUInt32Arg ();
				Console.WriteLine (PinModeMask);
			}
		}

		private static void OnGetPinModeMask (ReceivedCommand args)
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

		public int AnalogReference {
			get{ return AnalogReference_; }
			set {
				AnalogReference_ = value;
				
			}
		}

		private int AnalogReference_;
		public string Version = "";
		public string Model = "";
		public string Name = "";

		//default with Arduino UNO
		public bool UseDTR = false;

		public Board ()
		{
			AnalogReferences = new Dictionary<string,int> ();
			NumberOfAnalogPins = 6;
			NumberOfDigitalPins = 20;
		}

		public Board (uint numberOfAnalogPins, uint numberOfDigitalPins, Dictionary<string,int> analogReferences, string name = "", string version = "", string model = "", bool dtr = false)
		{
			this.NumberOfAnalogPins = numberOfAnalogPins;
			this.NumberOfDigitalPins = numberOfDigitalPins;
			this.AnalogReferences = analogReferences;
			this.Version = version;
			this.Model = model;
			this.Name = name;
			this.UseDTR = dtr;
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
