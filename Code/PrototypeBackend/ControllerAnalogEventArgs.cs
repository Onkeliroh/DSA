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

		public DPinState PinValue { get; private set; }

		public DateTime TimeStamp { get; private set; }

		public ControllerDigitalEventArgs (int pinNr, DPinState pinValue) : this (pinNr, pinValue, DateTime.Now)
		{
		}

		public ControllerDigitalEventArgs (int pinNr, DPinState pinValue, DateTime timeStamp)
		{
			PinNr = pinNr;
			PinValue = pinValue;
			TimeStamp = timeStamp;
		}
	}

	public enum PinUpdateOperation
	{
		Add,
		Remove
	}

	public class ControllerPinUpdateArgs
	{
		public IPin Pin { get; private set; }

		public PinUpdateOperation UpdateOperation { get; private set; }

		public ControllerPinUpdateArgs (IPin pin, PinUpdateOperation pinUpdateOperation)
		{
			Pin = pin;
			UpdateOperation = pinUpdateOperation;
		}
	}
}

