// *** ArduinoController ***

// This example expands the SendandReceiveArguments example. The PC will now sends commands to the Arduino when the trackbar 
// is pulled. Every TrackBarChanged events will queue a message to the Arduino to set the blink speed of the 
// internal / pin 13 LED
// 
// This example shows how to :
// - use in combination with WinForms
// - use in combination with ZedGraph
// - send queued commands
// - Use the CollapseCommandStrategy

using System;
using CommandMessenger;
using CommandMessenger.Queue;
using CommandMessenger.Transport.Serial;
using Gtk;
using System.Collections.Generic;

namespace ArduinoController
{
	enum Command
	{
		Acknowledge,
		// Command to acknowledge a received command
		Error,
		// Command to message that an error has occurred
		SetPin,
		// Command to set Mode and state of a digital Pin
		ReadAnalog,
		// Command to read a analog Pin
		ReadAnalogResult,
		// Return for ReadAnalog command
	};

	public enum DPinMode
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
		// This class (kind of) contains presentation logic, and domain model.
		// ChartForm.cs contains the view components

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
			_cmdMessenger.Disconnect ();
		}

		/// Attach command call backs. 
		private void AttachCommandCallBacks ()
		{
			_cmdMessenger.Attach (OnUnknownCommand);
			_cmdMessenger.Attach ((int)Command.Acknowledge, OnAcknowledge);
			_cmdMessenger.Attach ((int)Command.Error, OnError);
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

		public void SetPin (int nr, DPinMode mode, DPinState state)
		{
			var command = new SendCommand ((int)Command.SetPin, nr);
			command.AddArgument ((Int16)mode);
			command.AddArgument ((Int16)state);
			_cmdMessenger.SendCommand (command);
		}

		public void ReadAnalogPin (int nr)
		{
			var command = new SendCommand ((int)Command.ReadAnalog);
			command.AddArgument (nr);
			var commandreturn = _cmdMessenger.SendCommand (command);

			if (commandreturn.Ok) {
				if (AnalogValues.Count < nr) {
					while (AnalogValues.Count <= nr) {
						AnalogValues.Add (new List<float> ());
					}
				}
				var val = commandreturn.ReadFloatArg ();
				Console.WriteLine (nr + "\t" + val);
				AnalogValues [nr].Add (val);
			}
		}
	}
}
