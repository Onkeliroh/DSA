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

		public IPin Pin2 { get; private set; }

		public UpdateOperation UpdateOperation { get; private set; }

		public ControllerPinUpdateArgs (IPin pin, UpdateOperation pinUpdateOperation, IPin pin2 = null)
		{
			Pin = pin;
			Pin2 = pin2;
			UpdateOperation = pinUpdateOperation;
		}
	}

	public class SequencesUpdatedArgs
	{
		public Sequence Seq { get; private set; }

		public Sequence Seq2 { get; private set; }

		public UpdateOperation UpdateOperation { get; private set; }

		public SequencesUpdatedArgs (UpdateOperation seqUpdateOperation, Sequence seq = null, Sequence seq2 = null)
		{
			Seq = seq;
			Seq2 = seq2;
			UpdateOperation = seqUpdateOperation;
		}
	}

	public class MeasurementCombinationsUpdatedArgs
	{
		public MeasurementCombination MC { get; private set; }

		public MeasurementCombination MC2 { get; private set; }

		public UpdateOperation UpdateOperation{ get; private set; }

		public MeasurementCombinationsUpdatedArgs (UpdateOperation sigUpdateOperation, MeasurementCombination mc = null, MeasurementCombination mc2 = null)
		{
			MC = mc;
			MC2 = mc2;
			UpdateOperation = sigUpdateOperation;
		}
	}

	public class BoardSelectionArgs
	{
		public PrototypeBackend.Board @Board{ get; private set; }

		public BoardSelectionArgs (Board board)
		{
			Board = board;
		}
	}

	public class CommunicationArgs
	{
		public string Message{ get; private set; }

		public CommunicationArgs (string msg)
		{
			Message = msg;
		}
	}
}

