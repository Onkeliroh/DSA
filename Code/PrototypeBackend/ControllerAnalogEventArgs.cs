using System;

namespace PrototypeBackend
{
	public class ControllerAnalogEventArgs
	{
		public int[] PinNr { get; set; }

		public int[] PinValue { get; private set; }

		public DateTime TimeStamp { get; private set; }

		public ControllerAnalogEventArgs (int[] pinNr, int[] pinValue) : this (pinNr, pinValue, DateTime.Now)
		{
		}

		public ControllerAnalogEventArgs (int[] pinNr, int[] pinValue, DateTime timeStamp)
		{
			PinNr = pinNr;
			PinValue = pinValue;
			TimeStamp = timeStamp;
		}
	}

	public class ControllerDigitalEventArgs
	{
		public int PinNr { get; private set; }

		public double PinValue { get; private set; }

		public DateTime TimeStamp { get; private set; }

		public ControllerDigitalEventArgs (int pinNr, double pinValue) : this (pinNr, pinValue, DateTime.Now)
		{
		}

		public ControllerDigitalEventArgs (int pinNr, double pinValue, DateTime timeStamp)
		{
			PinNr = pinNr;
			PinValue = pinValue;
			TimeStamp = timeStamp;
		}
	}
}

