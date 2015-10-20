using System;

namespace PrototypeBackend
{
	/// <summary>
	/// Connection changed arguments.
	/// </summary>
	public class ConnectionChangedArgs : EventArgs
	{
		/// <summary>
		/// Gets a value indicating whether this <see cref="PrototypeBackend.ConnectionChangedArgs"/> is connected.
		/// </summary>
		/// <value><c>true</c> if connected; otherwise, <c>false</c>.</value>
		public bool Connected { get; private set; }

		/// <summary>
		/// Gets the port.
		/// </summary>
		/// <value>The port.</value>
		public string Port { get ; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="PrototypeBackend.ConnectionChangedArgs"/> class.
		/// </summary>
		/// <param name="connected">If set to <c>true</c> connected.</param>
		/// <param name="port">Port.</param>
		public ConnectionChangedArgs (bool connected, string port = null)
		{
			Connected = connected;
			Port = port;
		}
	}

	/// <summary>
	/// Update operations.
	/// </summary>
	public enum UpdateOperation
	{
		Add,
		AddRange,
		Remove,
		Change,
		Clear,
	}

	/// <summary>
	/// Controller pin update arguments.
	/// </summary>
	public class ControllerPinUpdateArgs : EventArgs
	{
		/// <summary>
		/// The original pin.
		/// </summary>
		/// <value>The pin.</value>
		public IPin OldPin { get; private set; }

		/// <summary>
		/// The new pin.
		/// </summary>
		/// <value>The pin2.</value>
		public IPin NewPin { get; private set; }

		/// <summary>
		/// Gets the update operation.
		/// </summary>
		/// <value>The update operation.</value>
		public UpdateOperation UpdateOperation { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="PrototypeBackend.ControllerPinUpdateArgs"/> class.
		/// </summary>
		/// <param name="oldpin">The old pin.</param>
		/// <param name="pinUpdateOperation">Pin update operation.</param>
		/// <param name="newpin">The new pin.</param>
		public ControllerPinUpdateArgs (IPin oldpin, UpdateOperation pinUpdateOperation, IPin newpin = null)
		{
			OldPin = oldpin;
			NewPin = newpin;
			UpdateOperation = pinUpdateOperation;
		}
	}

	/// <summary>
	/// Sequences updated arguments.
	/// </summary>
	public class SequencesUpdatedArgs : EventArgs
	{
		/// <summary>
		/// Gets the old sequence.
		/// </summary>
		/// <value>The old sequence.</value>
		public Sequence OldSeq { get; private set; }

		/// <summary>
		/// Gets the new sequence.
		/// </summary>
		/// <value>The new sequence.</value>
		public Sequence NewSeq { get; private set; }

		/// <summary>
		/// Gets the update operation.
		/// </summary>
		/// <value>The update operation.</value>
		public UpdateOperation UpdateOperation { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="PrototypeBackend.SequencesUpdatedArgs"/> class.
		/// </summary>
		/// <param name="seqUpdateOperation">Seq update operation.</param>
		/// <param name="oldseq">The old sequence.</param>
		/// <param name="newseq">The new sequence.</param>
		public SequencesUpdatedArgs (UpdateOperation seqUpdateOperation, Sequence oldseq = null, Sequence newseq = null)
		{
			OldSeq = oldseq;
			NewSeq = newseq;
			UpdateOperation = seqUpdateOperation;
		}
	}

	/// <summary>
	/// Measurement combinations updated arguments.
	/// </summary>
	public class MeasurementCombinationsUpdatedArgs : EventArgs
	{
		/// <summary>
		/// Gets the old measurementcombination.
		/// </summary>
		/// <value>The old me COM.</value>
		public MeasurementCombination OldMeCom { get; private set; }

		/// <summary>
		/// Gets the new measurementcombination.
		/// </summary>
		/// <value>The new me COM.</value>
		public MeasurementCombination NewMeCom { get; private set; }

		/// <summary>
		/// Gets the update operation.
		/// </summary>
		/// <value>The update operation.</value>
		public UpdateOperation UpdateOperation{ get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="PrototypeBackend.MeasurementCombinationsUpdatedArgs"/> class.
		/// </summary>
		/// <param name="sigUpdateOperation">Sig update operation.</param>
		/// <param name="oldmecom">The old measurementcombination.</param>
		/// <param name="newmecom">The new measurementcombination.</param>
		public MeasurementCombinationsUpdatedArgs (UpdateOperation sigUpdateOperation, MeasurementCombination oldmecom = null, MeasurementCombination newmecom = null)
		{
			OldMeCom = oldmecom;
			NewMeCom = newmecom;
			UpdateOperation = sigUpdateOperation;
		}
	}

	/// <summary>
	/// Board selection arguments.
	/// </summary>
	public class BoardSelectionArgs : EventArgs
	{
		/// <summary>
		/// Gets the board.
		/// </summary>
		/// <value>The board.</value>
		public PrototypeBackend.Board @Board{ get; private set; }

		/// <summary>
		/// Gets the analog reference voltage.
		/// </summary>
		/// <value>The ARE.</value>
		public double AREF { get; private set; }

		/// <summary>
		/// Gets the name of the analog reference voltage options.
		/// </summary>
		/// <value>The name of the analog reference voltage option.</value>
		public string AREFName { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="PrototypeBackend.BoardSelectionArgs"/> class.
		/// </summary>
		/// <param name="board">Board.</param>
		/// <param name="aref">analog reference voltage.</param>
		/// <param name="arefname">analog reference voltage option.</param>
		public BoardSelectionArgs (Board board, double aref = 5, string arefname = "DEFAULT")
		{
			Board = board;
			AREF = aref;
			AREFName = arefname;
		}
	}

	/// <summary>
	/// Communication arguments.
	/// </summary>
	public class CommunicationArgs : EventArgs
	{
		/// <summary>
		/// Gets the message.
		/// </summary>
		/// <value>The message.</value>
		public string Message{ get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="PrototypeBackend.CommunicationArgs"/> class.
		/// </summary>
		/// <param name="msg">Message.</param>
		public CommunicationArgs (string msg)
		{
			Message = msg;
		}
	}

	/// <summary>
	/// New measurement value.
	/// </summary>
	public class NewMeasurementValue
	{
		/// <summary>
		/// The raw value.
		/// </summary>
		public double RAW;

		/// <summary>
		/// The value.
		/// </summary>
		public double Value;

		/// <summary>
		/// The time.
		/// </summary>
		public DateTime Time;
	}
}

