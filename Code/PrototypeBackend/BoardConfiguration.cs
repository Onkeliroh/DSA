using System;
using System.Collections.Generic;
using System.Linq;
using PrototypeBackend;
using System.Runtime.Serialization;

namespace PrototypeBackend
{

	/// <summary>
	/// The BoardConfiguration class.
	/// </summary>
	[Serializable]
	public class BoardConfiguration : ISerializable
	{
		#region Member

		public Board @Board {
			get{ return board; }
			set {
				//TODO versuchen zu verschieben?
				if (board != null) {
					if (value.NumberOfAnalogPins < board.NumberOfAnalogPins) {
						Pins.Where (o => o is APin).ToList ().RemoveAll (x => x.Number >= value.NumberOfAnalogPins);
					}
					if (value.NumberOfDigitalPins < board.NumberOfDigitalPins) {
						Pins.Where (o => o is DPin).ToList ().RemoveAll (x => x.Number >= value.NumberOfDigitalPins);
					}
				}

				board = value;

				CheckPins ();

				if (OnBoardUpdated != null) {
					OnBoardUpdated.Invoke (this, null);
				}
			}
		}

		private Board board = new Board ();

		public APin[] AvailableAnalogPins{ get { return GetUnusedAnalogPins (); } private set { } }

		public DPin[] AvailableDigitalPins{ get { return GetUnusedDigitalPins (); } private set { } }

		public List<IPin> Pins { get; private set; }

		public List<DPin> DigitalPins {
			get{ return Pins.Where (o => (o as DPin) != null).Cast<DPin> ().ToList (); }
			set{ AddPin (value as IPin); }
		}

		public List<APin> AnalogPins {
			get{ return Pins.Where (o => (o as APin) != null).Cast<APin> ().ToList (); }
			set{ AddPin (value as IPin); }
		}

		public List<MeasurementCombination> MeasurementCombinations{ get; private set; }

		public List<Sequence> Sequences{ get; private set; }

		public List<string> SequenceGroups { 
			get { return Sequences.Select (o => o.GroupName).Distinct ().Where (s => !string.IsNullOrEmpty (s) && !string.IsNullOrWhiteSpace (s)).ToList<string> (); }
			private set { }
		}

		public List<IPin> LeftPinLayout {
			get {
				var list = new List<IPin> ();

				foreach (int i in board.PinLayout["LEFT"]) {
					if (Pins.Select (o => o.RealNumber).Contains ((uint)i)) {
						list.Add (Pins.Single (o => o.RealNumber == i));
					}
				}
				return list;
			}
			private set{ }
		}

		public List<IPin> RightPinLayout {
			get {
				var list = new List<IPin> ();

				foreach (int i in board.PinLayout["RIGHT"]) {
					if (Pins.Select (o => o.RealNumber).Contains ((uint)i)) {
						list.Add (Pins.Single (o => o.RealNumber == i));
					}
				}
				return list;
			}
			private set{ }
		}

		public List<IPin> BottomPinLayout {
			get {
				var list = new List<IPin> ();

				foreach (int i in board.PinLayout["BOTTOM"]) {
					if (Pins.Select (o => o.RealNumber).Contains ((uint)i)) {
						list.Add (Pins.Single (o => o.RealNumber == i));
					}
				}
				return list;
			}
			private set{ }
		}

		//Settings

		/// <summary>
		/// Path to the folder where the csv logs will be written
		/// </summary>
		public string CSVSaveFolderPath = string.Empty;
		/// <summary>
		/// Filepath to the configuration savefile 
		/// </summary>
		public string ConfigSavePath = string.Empty;
		public string Separator = "[COMMA]";
		public readonly string EmptyValueFilling = string.Empty;
		public bool UTCTimestamp = false;
		public readonly bool LocalTimestamp = true;
		public string TimeFormat = @"LONG (YYYY-MM-DD hh:mm:ss.ssss)";
		public string[] FileNameConvention = new string[]{ "[DATE]", "[LOCALTIME]", "[EMPTY]" };
		public readonly string FileNameTimeFormat = "{0:HH_mm}";
		public readonly string FileNameDateFormat = "{0:yyyy-MM-dd}";
		public string ValueFormatCultur = "English (United Kingdom)";

		public bool UseMarker = false;
		public bool LogRAWValues = false;

		#endregion

		#region EventHandler

		[NonSerialized]
		public EventHandler<MeasurementCombinationsUpdatedArgs> OnSignalsUpdated;
		[NonSerialized]
		public EventHandler<ControllerPinUpdateArgs> OnPinsUpdated;
		[NonSerialized]
		public EventHandler<SequencesUpdatedArgs> OnSequencesUpdated;
		[NonSerialized]
		public EventHandler OnBoardUpdated;

		#endregion

		/// <summary>
		/// Initializes a new instance of the <see cref="PrototypeBackend.BoardConfiguration"/> class.
		/// </summary>
		public BoardConfiguration ()
		{
			board = new Board ();
			Pins = new List<IPin> ();
			MeasurementCombinations = new List<MeasurementCombination> ();
			Sequences = new List<Sequence> ();
		}

		/// <summary>
		/// Gets the unused analog pins.
		/// </summary>
		/// <returns>The unused analog pins.</returns>
		private APin[] GetUnusedAnalogPins ()
		{
			var unusedpins = new List<APin> ();
			var numpins = Board.NumberOfAnalogPins; 
			for (uint i = 0; i < numpins; i++) {
				unusedpins.Add (new APin () {
					Number = i,
					DigitalNumber = Board.HardwareAnalogPins [i],
					RX = (Board.RX [0] == i),
					TX = (Board.TX [0] == i),
					SDA = (Board.SDA [0] == i),
					SCL = (Board.SCL [0] == i)
				});
			}

			foreach (IPin pin in Pins) {
				if (pin is APin) {
					unusedpins.RemoveAll (o => o.Number == pin.Number);
				} else if (pin is DPin) {
					unusedpins.RemoveAll (o => o.DigitalNumber == pin.Number);
				}
			}
			return unusedpins.OrderBy (o => o.Number).ToArray<APin> ();
		}

		/// <summary>
		/// Gets the unused digital pins.
		/// </summary>
		/// <returns>The unused digital pins.</returns>
		private DPin[] GetUnusedDigitalPins ()
		{
			var unusedpins = new List<DPin> ();
			var numpins = Board.NumberOfDigitalPins; 
			for (uint i = 0; i < numpins; i++) {
				unusedpins.Add (new DPin () {
					Number = i,
					AnalogNumber = ((Array.IndexOf (Board.HardwareAnalogPins, i) > -1) ? Array.IndexOf (Board.HardwareAnalogPins, i) : -1),
					RX = (Board.RX [0] == i),
					TX = (Board.TX [0] == i),
					SDA = (Board.SDA [0] == i),
					SCL = (Board.SCL [0] == i)
				});
			}

			foreach (IPin pin in Pins) {
				if (pin is DPin) {
					unusedpins.RemoveAll (o => o.Number == pin.Number);
				} else if (pin is APin) {
					unusedpins.RemoveAll (o => o.Number == (pin as APin).DigitalNumber);
				}
			}
			return unusedpins.ToArray<DPin> ();
		}

		/// <summary>
		/// Gets the pins without measurement combinations.
		/// </summary>
		/// <returns>The pins without measurement combinations.</returns>
		public APin[] GetPinsWithoutCombinations ()
		{
			var pins = Pins.Where (o => o.Type == PinType.ANALOG).Cast<APin> ().ToList ();

			pins.RemoveAll (o => MeasurementCombinations.Select (mc => mc.Pins.Contains (o)).Any (b => b == true));

			return pins.ToArray ();
		}

		/// <summary>
		/// Gets the digital pins without sequence.
		/// </summary>
		/// <returns>The digital pins without sequence.</returns>
		public DPin[] GetPinsWithoutSequence ()
		{
			var pins = Pins.Where (o => (o as DPin) != null).Cast<DPin> ().ToList ();

			pins.RemoveAll (o => Sequences.Select (seq => seq.Pin == o).Any (b => b == true));

			return pins.ToArray ();
		}

		/// <summary>
		/// Gets the coresponding sequence.
		/// </summary>
		/// <returns>The coresponding sequence.</returns>
		/// <param name="pin">Pin</param>
		public Sequence GetCorespondingSequence (DPin pin)
		{
			foreach (Sequence seq in Sequences) {
				if (seq.Pin == pin) {
					return seq;
				}
			}
			return null;
		}

		/// <summary>
		/// Gets the coresponding combination.
		/// </summary>
		/// <returns>The coresponding combination.</returns>
		/// <param name="pin">Pin</param>
		public MeasurementCombination GetCorespondingCombination (APin pin)
		{
			foreach (MeasurementCombination sig in MeasurementCombinations) {
				if (sig.Pins.Contains (pin)) {
					return sig;
				}
			}
			return null;
		}

		#region Add

		/// <summary>
		/// Adds a pin
		/// </summary>
		/// <param name="pin">Pin</param>
		public void AddPin (IPin pin)
		{
			if (!Pins.Contains (pin)) {
				Pins.Add (pin);
//				Pins = Pins.OrderBy (x => x.RealNumber).ThenBy (x => x.Type).ToList ();
				if (OnPinsUpdated != null) {
					OnPinsUpdated.Invoke (this, new ControllerPinUpdateArgs (pin, UpdateOperation.Add));
				}
			}
		}

		/// <summary>
		/// Adds a pin range.
		/// </summary>
		/// <param name="pins">Pins</param>
		public void AddPinRange (IPin[] pins)
		{
			for (int i = 0; i < pins.Length; i++) {
				if (!Pins.Contains (pins [i]) && pins [i] != null) {
					Pins.Add (pins [i]);
				}
			}
			if (OnPinsUpdated != null) {
				OnPinsUpdated.Invoke (this, new ControllerPinUpdateArgs (null, UpdateOperation.AddRange));
			}
		}

		/// <summary>
		/// Adds a measurement combination.
		/// </summary>
		public void AddMeasurementCombination (MeasurementCombination s)
		{
//			if (!MeasurementCombinations.Contains (s))
//			{
			MeasurementCombinations.Add (s);
//			}

			if (OnSignalsUpdated != null) {
				OnSignalsUpdated.Invoke (this, new MeasurementCombinationsUpdatedArgs (UpdateOperation.Add, s));
			}
		}

		/// <summary>
		/// Adds the sequence.
		/// </summary>
		/// <param name="sequence">Sequence</param>
		public void AddSequence (Sequence sequence)
		{
			if (!Sequences.Contains (sequence)) {
				Sequences.Add (sequence);
				if (OnSequencesUpdated != null) {
					OnSequencesUpdated.Invoke (this, new SequencesUpdatedArgs (UpdateOperation.Add, sequence));
				}
			}
		}

		#endregion

		#region Clone

		/// <summary>
		/// Clones a pin.
		/// </summary>
		/// <returns><c>true</c>, if pin was cloned, <c>false</c> otherwise.</returns>
		/// <param name="pin">Pin.</param>
		public bool ClonePin (IPin pin)
		{
			Console.WriteLine ("Cloning: " + pin);
			if ((pin as APin) != null) {
				if (AvailableAnalogPins.Length != 0) {
					APin newPin = new APin (pin as APin);
					newPin.Number = AvailableAnalogPins [0].Number;
					newPin.DigitalNumber = AvailableAnalogPins [0].DigitalNumber;

					AddPin (newPin);
				} else {
					return false;
				}
			} else if ((pin as DPin) != null) {
				if (AvailableDigitalPins.Length != 0) {
					DPin newPin = new DPin (pin as DPin);
					newPin.Number = AvailableDigitalPins [0].Number;
					newPin.AnalogNumber = AvailableDigitalPins [0].AnalogNumber;

					AddPin (newPin);
				} else {
					return false;
				}
			}
//			CheckPins ();
			return true;
		}

		/// <summary>
		/// Clones a measurement combination.
		/// </summary>
		/// <param name="meCom">Me COM.</param>
		public void CloneMeasurementCombination (MeasurementCombination meCom)
		{
			MeasurementCombination copy = new MeasurementCombination (meCom);
			AddMeasurementCombination (copy);
		}

		/// <summary>
		/// Clones a sequence.
		/// </summary>
		/// <param name="seq">Seq.</param>
		public void CloneSequence (Sequence seq)
		{
			Sequence copy = new Sequence (seq);
			copy.Pin = GetPinsWithoutSequence () [0];
			AddSequence (copy);
		}

		#endregion

		#region Edit

		/// <summary>
		/// Edits a pin.
		/// </summary>
		/// <param name="index">Index.</param>
		/// <param name="ip">Ip.</param>
		public void EditPin (int index, IPin ip)
		{
			if (OnPinsUpdated != null) {
				OnPinsUpdated.Invoke (this, new ControllerPinUpdateArgs (Pins [index], UpdateOperation.Change, ip));
			}
			Pins [index] = ip;
		}

		/// <summary>
		/// Edits the measurment combination.
		/// </summary>
		/// <param name="index">Index.</param>
		/// <param name="s">S.</param>
		public void EditMeasurmentCombination (int index, MeasurementCombination s)
		{
			if (OnSignalsUpdated != null) {
				OnSignalsUpdated.Invoke (this, new MeasurementCombinationsUpdatedArgs (UpdateOperation.Change, MeasurementCombinations [index], s));
			}
			MeasurementCombinations [index] = s;
		}

		/// <summary>
		/// Edits the sequence.
		/// </summary>
		/// <param name="index">Index.</param>
		/// <param name="seq">Seq.</param>
		public void EditSequence (int index, Sequence seq)
		{
			if (OnSequencesUpdated != null) {
				OnSequencesUpdated.Invoke (this, new SequencesUpdatedArgs (UpdateOperation.Change, Sequences [index], seq));
			}
			Sequences [index] = seq;
		}

		#endregion

		#region Remove

		/// <summary>
		/// Removes the pin.
		/// </summary>
		/// <param name="name">Name.</param>
		public void RemovePin (string name)
		{
			var result = Pins.Where (o => o.Name == name).ToList<IPin> ();

			if (result.Count > 0) {
				var pin = result.First ();
				if (pin is DPin) {
					var tmp = GetCorespondingSequence (pin as DPin);
					if (tmp != null)
						RemoveSequence (tmp.Name);
				} else if (pin is APin) {
					var tmp = GetCorespondingCombination (pin as APin);
					if (tmp != null)
						RemoveMeasurementCombination (tmp);
				}
				Pins.Remove (result.First ());
				if (OnPinsUpdated != null) {
					OnPinsUpdated.Invoke (this, new ControllerPinUpdateArgs (result [0], UpdateOperation.Remove));
				}
			}
		}

		/// <summary>
		/// Removes the pin.
		/// </summary>
		/// <param name="index">Index.</param>
		public void RemovePin (int index)
		{
			IPin pin = Pins [index];
			if (pin is DPin) {
				var tmp = GetCorespondingSequence (pin as DPin);
				if (tmp != null)
					RemoveSequence (tmp.Name);
			} else if (pin is APin) {
				var tmp = GetCorespondingCombination (pin as APin);
				if (tmp != null)
					RemoveMeasurementCombination (tmp);
			}
			Pins.RemoveAt (index);
			if (OnPinsUpdated != null) {
				OnPinsUpdated.Invoke (this, new ControllerPinUpdateArgs (pin, UpdateOperation.Remove));
			}
		}

		/// <summary>
		/// Removes the measurement combination.
		/// </summary>
		/// <param name="index">Index.</param>
		public void RemoveMeasurementCombination (int index)
		{
			var sig = new MeasurementCombination ();
			sig = MeasurementCombinations [index];
			MeasurementCombinations.RemoveAt (index);
		
			if (OnSignalsUpdated != null) {
				OnSignalsUpdated.Invoke (this, new MeasurementCombinationsUpdatedArgs (UpdateOperation.Remove, sig));
			}
		}

		/// <summary>
		/// Removes the measurement combination.
		/// </summary>
		/// <param name="index">Index.</param>
		public void RemoveMeasurementCombination (string index)
		{
			if (index != null) {
				var MeCom = new MeasurementCombination ();
				MeCom = MeasurementCombinations.Where (o => o.Name == index).ToList<MeasurementCombination> () [0];
		
				if (OnSignalsUpdated != null) {
					OnSignalsUpdated.Invoke (this, new MeasurementCombinationsUpdatedArgs (UpdateOperation.Remove, MeCom));
				}
				MeasurementCombinations.Remove (MeCom);
			} else {
				throw new ArgumentNullException ();
			}
		}

		/// <summary>
		/// Removes the measurement combination.
		/// </summary>
		/// <param name="index">Index.</param>
		public void RemoveMeasurementCombination (MeasurementCombination index)
		{
			if (index != null) {
				if (OnSignalsUpdated != null) {
					OnSignalsUpdated.Invoke (this, new MeasurementCombinationsUpdatedArgs (UpdateOperation.Remove, index));
				}
				MeasurementCombinations.Remove (index);
			}
		}

		/// <summary>
		/// Removes the sequence.
		/// </summary>
		/// <param name="name">Name.</param>
		public void RemoveSequence (string name)
		{
			if (name != null) {
				var result = Sequences.Where (o => o.Name == name).ToList<Sequence> ();
				if (result.Count > 0) {
					Sequences.Remove (result [0]);
					if (OnSequencesUpdated != null) {
						OnSequencesUpdated.Invoke (this, new SequencesUpdatedArgs (UpdateOperation.Remove, result [0]));
					}
				}
			}
		}

		/// <summary>
		/// Removes the sequence.
		/// </summary>
		/// <param name="index">Index.</param>
		public void RemoveSequence (int index)
		{
			if (index > -1) {
				var seq = new Sequence ();
				seq = Sequences [index];
				Sequences.RemoveAt (index);
				if (OnSequencesUpdated != null) {
					OnSequencesUpdated.Invoke (this, new SequencesUpdatedArgs (UpdateOperation.Remove, seq));
				}
			}
		}

		/// <summary>
		/// Removes the sequence.
		/// </summary>
		/// <param name="index">Index.</param>
		public void RemoveSequence (Sequence index)
		{
			if (index != null) {
				if (OnSequencesUpdated != null) {
					OnSequencesUpdated.Invoke (this, new SequencesUpdatedArgs (UpdateOperation.Remove, index));
				}
				Sequences.Remove (index);
			}
		}

		/// <summary>
		/// Removes the sequence group.
		/// </summary>
		/// <param name="groupname">Groupname.</param>
		public void RemoveSequenceGroup (string groupname)
		{
			Sequences.RemoveAll (o => o.GroupName.Equals (groupname));

			if (OnSequencesUpdated != null) {
				OnSequencesUpdated.Invoke (this, new SequencesUpdatedArgs (UpdateOperation.Remove, null));
			}
		}

		#endregion

		#region Clear

		/// <summary>
		/// Clears the pins.
		/// </summary>
		/// <param name="type">Type.</param>
		public void ClearPins (PinType type)
		{
			Pins.RemoveAll (o => o.Type == type);
			if (OnPinsUpdated != null) {
				OnPinsUpdated.Invoke (this, new ControllerPinUpdateArgs (null, UpdateOperation.Clear));
			}
			if (type == PinType.DIGITAL) {
				ClearSequences ();
			} else if (type == PinType.ANALOG) {
				ClearMeasurementCombinations ();
			}
		}

		/// <summary>
		/// Clears the pins.
		/// </summary>
		public void ClearPins ()
		{
			Pins.Clear ();
			ClearSequences ();
			ClearMeasurementCombinations ();
			if (OnPinsUpdated != null) {
				OnPinsUpdated.Invoke (this, new ControllerPinUpdateArgs (null, UpdateOperation.Clear, null));
			}
		}

		/// <summary>
		/// Clears the measurement combinations.
		/// </summary>
		public void ClearMeasurementCombinations ()
		{
			MeasurementCombinations.Clear ();

			if (OnSignalsUpdated != null) {
				OnSignalsUpdated.Invoke (this, new MeasurementCombinationsUpdatedArgs (UpdateOperation.Clear, null));
			}
		}

		/// <summary>
		/// Clears the sequences.
		/// </summary>
		public void ClearSequences ()
		{
			Sequences.Clear ();
			if (OnSequencesUpdated != null) {
				OnSequencesUpdated.Invoke (this, new SequencesUpdatedArgs (UpdateOperation.Clear));
			}
		}

		#endregion

		/// <summary>
		/// Assignes specific hardware pin number to the pins.
		/// </summary>
		private void CheckPins ()
		{
			foreach (APin pin in AnalogPins) {
				if (pin.Number < board.HardwareAnalogPins.Length) {
					pin.DigitalNumber = board.HardwareAnalogPins [pin.Number];
				}
			}
			foreach (DPin pin in DigitalPins) {
				pin.AnalogNumber = ((Array.IndexOf (Board.HardwareAnalogPins, pin.Number) > -1) ? Array.IndexOf (Board.HardwareAnalogPins, pin.Number) : -1);
			}
		}

		/// <summary>
		/// Gets the name of the CSV log-file-name.
		/// The consists of three parts perviousely set.
		/// </summary>
		/// <returns>The CSV log-file-name.</returns>
		public string GetCSVLogName ()
		{
			string preview = string.Empty;

			foreach (string option in FileNameConvention) {
				switch (option) {
				case "[LOCALTIME]":
					preview += string.Format (FileNameTimeFormat, DateTime.Now);
					preview += "-";
					break;
				case "[UTC TIME]":
					preview += string.Format (FileNameTimeFormat, DateTime.UtcNow);
					preview += "-";
					break;
				case "[DATE]":
					preview += string.Format (FileNameDateFormat, DateTime.Now);
					preview += "-";
					break;
				case "[EMPTY]":
					if (preview.Length > 0) {
						if (preview.Last () == '-') {
							preview.Remove (preview.Length - 1, 1);
						}
					}
					break;
				default:
					preview += option;
					preview += '-';
					break;
				}
			}

			if (preview.Length > 0) {
				if (preview.Last () == '-') {
					preview = preview.Remove (preview.LastIndexOf ('-'), 1);
				}
			}
			preview += ".csv";

			return preview;
		}

		/// <summary>
		/// Creates the parameter mapping for the csv logger
		/// </summary>
		/// <returns>The mapping.</returns>
		public IDictionary<string,int> CreateMapping ()
		{
			var dict = new Dictionary<string,int> ();

			int pos = 0;

			for (int i = pos; i < AnalogPins.Count; i++) {
				dict.Add (AnalogPins [i].DisplayName, i);
				pos++;
			}
			if (MeasurementCombinations.Count > 0) {
				for (int i = 0; i < MeasurementCombinations.Count; i++) {
					dict.Add (MeasurementCombinations [i].Name, i + pos);
				}
			}
			return dict;
		}

		#region ISerializable implementation

		/// <summary>
		/// Gets the object data.
		/// </summary>
		/// <param name="info">Info.</param>
		/// <param name="context">Context.</param>
		public void GetObjectData (SerializationInfo info, StreamingContext context)
		{
			info.AddValue ("Board", board);
			info.AddValue ("Pins", Pins);
			info.AddValue ("MeasurementCombinations", MeasurementCombinations);
			info.AddValue ("Sequences", Sequences);
			info.AddValue ("ConfigSavePath", ConfigSavePath);
			info.AddValue ("CSVSaveFolderPath", CSVSaveFolderPath);
			info.AddValue ("UseMarker", UseMarker);
			info.AddValue ("LogRAWValues", LogRAWValues);

			info.AddValue ("CSVSeparator", Separator);
			info.AddValue ("EmptyValueFilling", EmptyValueFilling);
			info.AddValue ("UTCTimestamp", UTCTimestamp);
			info.AddValue ("LocalTimestamp", LocalTimestamp);
			info.AddValue ("TimeFormat", TimeFormat);
			info.AddValue ("FileNameConvention", FileNameConvention.ToList ());
			info.AddValue ("FileNameTimeFormat", FileNameTimeFormat);
			info.AddValue ("FileNameDateFormat", FileNameDateFormat);
			info.AddValue ("ValueFormatCultur", ValueFormatCultur);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PrototypeBackend.BoardConfiguration"/> class.
		/// </summary>
		/// <param name="info">Info.</param>
		/// <param name="context">Context.</param>
		public BoardConfiguration (SerializationInfo info, StreamingContext context)
		{
			board = new Board ();
			Pins = new List<IPin> ();
			MeasurementCombinations = new List<MeasurementCombination> ();
			Sequences = new List<Sequence> ();

			board = (Board)info.GetValue ("Board", board.GetType ());
			Pins = (List<IPin>)info.GetValue ("Pins", Pins.GetType ());
			MeasurementCombinations = (List<MeasurementCombination>)info.GetValue ("MeasurementCombinations", MeasurementCombinations.GetType ());
			Sequences = (List<Sequence>)info.GetValue ("Sequences", Sequences.GetType ());
			ConfigSavePath = info.GetString ("ConfigSavePath");
			CSVSaveFolderPath = info.GetString ("CSVSaveFolderPath");
			UseMarker = info.GetBoolean ("UseMarker");
			LogRAWValues = info.GetBoolean ("LogRAWValues");

			Separator = info.GetString ("CSVSeparator");
			EmptyValueFilling = info.GetString ("EmptyValueFilling");
			UTCTimestamp = info.GetBoolean ("UTCTimestamp");
			LocalTimestamp = info.GetBoolean ("LocalTimestamp");
			TimeFormat = info.GetString ("TimeFormat");
			FileNameConvention = ((List<string>)info.GetValue ("FileNameConvention", new List<string> ().GetType ())).ToArray<string> ();
			FileNameTimeFormat = info.GetString ("FileNameTimeFormat");
			FileNameDateFormat = info.GetString ("FileNameDateFormat");
			ValueFormatCultur = info.GetString ("ValueFormatCultur");
		}

		#endregion
	}
}

