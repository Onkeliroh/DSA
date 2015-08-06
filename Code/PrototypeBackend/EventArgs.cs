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

		public PinMode PinMode { get; private set; }

		public ControllerDigitalEventArgs (int pinNr, DPinState pinValue, PinMode mode = PinMode.OUTPUT) : this (pinNr, pinValue, DateTime.Now, mode)
		{
		}

		public ControllerDigitalEventArgs (int pinNr, DPinState pinValue, DateTime timeStamp, PinMode mode = PinMode.OUTPUT)
		{
			PinNr = pinNr;
			PinValue = pinValue;
			TimeStamp = timeStamp;
			PinMode = mode;
		}
	}

	public class ConnectionChangedArgs
	{
		public bool Connected { get; private set; }

		public string Port { get ; private set; }

		public ConnectionChangedArgs (bool connected, string port = null)
		{
			Connected = connected;
			Port = port;
		}
	}

	public enum UpdateOperation
	{
		Add,
		Remove,
		Change,
		Clear,
	}

	public class ControllerPinUpdateArgs
	{
		public IPin Pin { get; private set; }

		public UpdateOperation UpdateOperation { get; private set; }

		public PinType Type { get; private set; }

		public ControllerPinUpdateArgs (IPin pin, UpdateOperation pinUpdateOperation, PinType pinType)
		{
			Pin = pin;
			UpdateOperation = pinUpdateOperation;
			Type = pinType;
		}
	}

	public class ControllerSequenceUpdateArgs
	{
		public Sequence Seq { get; private set; }

		public UpdateOperation UpdateOperation { get; private set; }

		public ControllerSequenceUpdateArgs (UpdateOperation seqUpdateOperation, Sequence seq = null)
		{
			Seq = seq;
			UpdateOperation = seqUpdateOperation;
		}
	}
}

