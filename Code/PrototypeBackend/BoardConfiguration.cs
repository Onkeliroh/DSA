using System;
using System.Collections.Generic;
using System.Linq;
using PrototypeBackend;
using System.Runtime.Serialization;

namespace PrototypeBackend
{

	[Serializable]
	public class BoardConfiguration : ISerializable
	{
		#region Member

		public Board @Board {
			get{ return board; }
			set {
				//TODO versuchen zu verschieben?
				if (board != null)
				{
					if (value.NumberOfAnalogPins < board.NumberOfAnalogPins)
					{
						Pins.Where (o => o is APin).ToList ().RemoveAll (x => x.Number >= value.NumberOfAnalogPins);
					}
					if (value.NumberOfDigitalPins < board.NumberOfDigitalPins)
					{
						Pins.Where (o => o is DPin).ToList ().RemoveAll (x => x.Number >= value.NumberOfDigitalPins);
					}
				}

				board = value;

				CheckPins ();

				if (OnBoardUpdated != null)
				{
					OnBoardUpdated.Invoke (this, null);
				}
			}
		}

		private Board board = new Board ();

		public APin[] AvailableAnalogPins{ get { return GetUnusedAnalogPins (); } private set { } }

		public DPin[] AvailableDigitalPins{ get { return GetUnusedDigitalPins (); } private set { } }

		public List<IPin> Pins { get; private set; }

		public List<DPin> DigitalPins {
			get{ return Pins.Where (o => o.Type == PinType.DIGITAL).Cast<DPin> ().ToList (); }
			set{ AddPin (value as IPin); }
		}

		public List<APin> AnalogPins {
			get{ return Pins.Where (o => o.Type == PinType.ANALOG).Cast<APin> ().ToList (); }
			set{ AddPin (value as IPin); }
		}

		public List<MeasurementCombination> MeasurementCombinations{ get; private set; }

		public List<Sequence> Sequences{ get; private set; }


		//Settings

		/// <summary>
		/// Path to the folder where the csv logs will be written
		/// </summary>
		public string CSVSaveFolderPath = string.Empty;
		/// <summary>
		/// Filepath to the configuration savefile 
		/// </summary>
		public string ConfigSavePath = string.Empty;
		public string Separator = ";";
		public string EmptyValueFilling = string.Empty;
		public bool UTCTimestamp = false;
		public bool LocalTimestamp = true;
		public string TimeFormat = "{0:HH:mm:ss.ff}";
		public string[] FileNameConvention = new string[]{ "Time", "Empty", "Empty" };

		public string LogFilePath = Environment.GetFolderPath (Environment.SpecialFolder.UserProfile) + @"/micrologger/";

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

		public BoardConfiguration ()
		{
			board = new Board ();
			Pins = new List<IPin> ();
			MeasurementCombinations = new List<MeasurementCombination> ();
			Sequences = new List<Sequence> ();
		}

		private APin[] GetUnusedAnalogPins ()
		{
			var unusedpins = new List<APin> ();
			var numpins = Board.NumberOfAnalogPins; 
			for (uint i = 0; i < numpins; i++)
			{
				unusedpins.Add (new APin () {
					Number = i,
					DigitalNumber = Board.HardwareAnalogPins [i],
					RX = (Board.RX [0] == i),
					TX = (Board.TX [0] == i),
					SDA = (Board.SDA [0] == i),
					SCL = (Board.SCL [0] == i)
				});
			}

			foreach (IPin pin in Pins)
			{
				if (pin is APin)
				{
					unusedpins.RemoveAll (o => o.Number == pin.Number);
				} else if (pin is DPin)
				{
					unusedpins.RemoveAll (o => o.DigitalNumber == pin.Number);
				}
			}
			return unusedpins.OrderBy (o => o.Number).ToArray<APin> ();
		}

		private DPin[] GetUnusedDigitalPins ()
		{
			var unusedpins = new List<DPin> ();
			var numpins = Board.NumberOfDigitalPins; 
			for (uint i = 0; i < numpins; i++)
			{
				unusedpins.Add (new DPin () {
					Number = i,
					AnalogNumber = ((Array.IndexOf (Board.HardwareAnalogPins, i) > -1) ? Array.IndexOf (Board.HardwareAnalogPins, i) : -1),
					RX = (Board.RX [0] == i),
					TX = (Board.TX [0] == i),
					SDA = (Board.SDA [0] == i),
					SCL = (Board.SCL [0] == i)
				});
			}

			foreach (IPin pin in Pins)
			{
				if (pin is DPin)
				{
					unusedpins.RemoveAll (o => o.Number == pin.Number);
				} else if (pin is APin)
				{
					unusedpins.RemoveAll (o => o.Number == (pin as APin).DigitalNumber);
				}
			}
			return unusedpins.ToArray<DPin> ();
		}

		public APin[] GetPinsWithoutCombinations ()
		{
			var pins = Pins.Where (o => o.Type == PinType.ANALOG).Cast<APin> ().ToList ();

			pins.RemoveAll (o => MeasurementCombinations.Select (mc => mc.Pins.Contains (o)).Any (b => b == true));

			return pins.ToArray ();
		}

		public DPin[] GetPinsWithoutSequence ()
		{
			var pins = Pins.Where (o => o.Type == PinType.DIGITAL).Cast<DPin> ().ToList ();

			pins.RemoveAll (o => Sequences.Select (seq => seq.Pin == o).Any (b => b == true));

			return pins.ToArray ();
		}

		public Sequence GetCorespondingSequence (DPin pin)
		{
			foreach (Sequence seq in Sequences)
			{
				if (seq.Pin == pin)
				{
					return seq;
				}
			}
			return null;
		}

		public MeasurementCombination GetCorespondingCombination (APin pin)
		{
			foreach (MeasurementCombination sig in MeasurementCombinations)
			{
				if (sig.Pins.Contains (pin))
				{
					return sig;
				}
			}
			return null;
		}

		#region Add

		public void AddPin (IPin pin)
		{
			if (!Pins.Contains (pin) && pin != null)
			{
				Pins.Add (pin);
				Pins = Pins.OrderBy (x => x.RealNumber).ThenBy (x => x.Type).ToList ();
				if (OnPinsUpdated != null)
				{
					OnPinsUpdated.Invoke (this, new ControllerPinUpdateArgs (pin, UpdateOperation.Add));
				}
			}
		}

		public void AddPinRange (IPin[] pins)
		{
			for (int i = 0; i < pins.Length; i++)
			{
				if (!Pins.Contains (pins [i]) && pins [i] != null)
				{
					Pins.Add (pins [i]);
				}
			}
			if (OnPinsUpdated != null)
			{
				OnPinsUpdated.Invoke (this, new ControllerPinUpdateArgs (null, UpdateOperation.AddRange));
			}
		}

		public void AddMeasurementCombination (MeasurementCombination s)
		{
			if (!MeasurementCombinations.Contains (s))
			{
				MeasurementCombinations.Add (s);
			}

			if (OnSignalsUpdated != null)
			{
				OnSignalsUpdated.Invoke (this, new MeasurementCombinationsUpdatedArgs (UpdateOperation.Add, s));
			}
		}

		public void AddSequence (Sequence seq)
		{
			if (!Sequences.Contains (seq))
			{
				Sequences.Add (seq);
				if (OnSequencesUpdated != null)
				{
					OnSequencesUpdated.Invoke (this, new SequencesUpdatedArgs (UpdateOperation.Add, seq));
				}
			}
		}

		#endregion

		#region Set

		public void SetPin (int index, IPin ip)
		{
			if (OnPinsUpdated != null)
			{
				OnPinsUpdated.Invoke (this, new ControllerPinUpdateArgs (Pins [index], UpdateOperation.Change, ip));
			}
			Pins [index] = ip;
		}

		public void SetMeasurmentCombination (int index, MeasurementCombination s)
		{
			if (s != null)
			{
				OnSignalsUpdated.Invoke (this, new MeasurementCombinationsUpdatedArgs (UpdateOperation.Change, MeasurementCombinations [index], s));
			}
			MeasurementCombinations [index] = s;
		}

		public void SetSequence (int index, Sequence seq)
		{
			if (OnSequencesUpdated != null)
			{
				OnSequencesUpdated.Invoke (this, new SequencesUpdatedArgs (UpdateOperation.Change, Sequences [index], seq));
			}
			Sequences [index] = seq;
		}

		#endregion

		#region Remove

		public void RemovePin (string name)
		{
			var result = Pins.Where (o => o.Name == name).ToList<IPin> ();

			if (result.Count > 0)
			{
				var pin = result.First ();
				if (pin is DPin)
				{
					var tmp = GetCorespondingSequence (pin as DPin);
					if (tmp != null)
						RemoveSequence (tmp.Name);
				} else if (pin is APin)
				{
					var tmp = GetCorespondingCombination (pin as APin);
					if (tmp != null)
						RemoveMeasurementCombination (tmp);
				}
				Pins.Remove (result.First ());
				if (OnPinsUpdated != null)
				{
					OnPinsUpdated.Invoke (this, new ControllerPinUpdateArgs (result [0], UpdateOperation.Remove));
				}
			}
		}

		public void RemovePin (int index)
		{
			IPin pin = Pins [index];
			if (pin is DPin)
			{
				var tmp = GetCorespondingSequence (pin as DPin);
				if (tmp != null)
					RemoveSequence (tmp.Name);
			} else if (pin is APin)
			{
				var tmp = GetCorespondingCombination (pin as APin);
				if (tmp != null)
					RemoveMeasurementCombination (tmp);
			}
			Pins.RemoveAt (index);
			if (OnPinsUpdated != null)
			{
				OnPinsUpdated.Invoke (this, new ControllerPinUpdateArgs (pin, UpdateOperation.Remove));
			}
		}

		public void RemoveMeasurementCombination (int index)
		{
			var sig = new MeasurementCombination ();
			sig = MeasurementCombinations [index];
			MeasurementCombinations.RemoveAt (index);
		
			if (OnSignalsUpdated != null)
			{
				OnSignalsUpdated.Invoke (this, new MeasurementCombinationsUpdatedArgs (UpdateOperation.Remove, sig));
			}
		}

		public void RemoveMeasurementCombination (string index)
		{
			if (index != null)
			{
				var MeCom = new MeasurementCombination ();
				MeCom = MeasurementCombinations.Where (o => o.Name == index).ToList<MeasurementCombination> () [0];
		
				if (OnSignalsUpdated != null)
				{
					OnSignalsUpdated.Invoke (this, new MeasurementCombinationsUpdatedArgs (UpdateOperation.Remove, MeCom));
				}
				MeasurementCombinations.Remove (MeCom);
			} else
			{
				throw new ArgumentNullException ();
			}
		}

		public void RemoveMeasurementCombination (MeasurementCombination index)
		{
			if (index != null)
			{
				if (OnSignalsUpdated != null)
				{
					OnSignalsUpdated.Invoke (this, new MeasurementCombinationsUpdatedArgs (UpdateOperation.Remove, index));
				}
				MeasurementCombinations.Remove (index);
			}
		}

		public void RemoveSequence (string name)
		{
			if (name != null)
			{
				var result = Sequences.Where (o => o.Name == name).ToList<Sequence> ();
				if (result.Count > 0)
				{
					Sequences.Remove (result [0]);
					if (OnSequencesUpdated != null)
					{
						OnSequencesUpdated.Invoke (this, new SequencesUpdatedArgs (UpdateOperation.Remove, result [0]));
					}
				}
			}
		}

		public void RemoveSequence (int index)
		{
			if (index > -1)
			{
				var seq = new Sequence ();
				seq = Sequences [index];
				Sequences.RemoveAt (index);
				if (OnSequencesUpdated != null)
				{
					OnSequencesUpdated.Invoke (this, new SequencesUpdatedArgs (UpdateOperation.Remove, seq));
				}
			}
		}

		public void RemoveSequence (Sequence index)
		{
			if (index != null)
			{
				if (OnSequencesUpdated != null)
				{
					OnSequencesUpdated.Invoke (this, new SequencesUpdatedArgs (UpdateOperation.Remove, index));
				}
				Sequences.Remove (index);
			}
		}

		#endregion

		#region Clear

		public void ClearPins (PinType type)
		{
			Pins.RemoveAll (o => o.Type == type);
			if (OnPinsUpdated != null)
			{
				OnPinsUpdated.Invoke (this, new ControllerPinUpdateArgs (null, UpdateOperation.Clear));
			}
			if (type == PinType.DIGITAL)
			{
				ClearSequences ();
			} else if (type == PinType.ANALOG)
			{
				ClearMeasurementCombinations ();
			}
		}

		public void ClearPins ()
		{
			Pins.Clear ();
			ClearSequences ();
			ClearMeasurementCombinations ();
			if (OnPinsUpdated != null)
			{
				OnPinsUpdated.Invoke (this, new ControllerPinUpdateArgs (null, UpdateOperation.Clear, null));
			}
		}

		public void ClearMeasurementCombinations ()
		{
			MeasurementCombinations.Clear ();

			if (OnSignalsUpdated != null)
			{
				OnSignalsUpdated.Invoke (this, new MeasurementCombinationsUpdatedArgs (UpdateOperation.Clear, null));
			}
		}

		public void ClearSequences ()
		{
			Sequences.Clear ();
			if (OnSequencesUpdated != null)
			{
				OnSequencesUpdated.Invoke (this, new SequencesUpdatedArgs (UpdateOperation.Clear));
			}
		}

		#endregion

		private void CheckPins ()
		{
			foreach (APin pin in AnalogPins)
			{
				pin.DigitalNumber = board.HardwareAnalogPins [pin.Number];
			}
			foreach (DPin pin in DigitalPins)
			{
				pin.AnalogNumber = ((Array.IndexOf (Board.HardwareAnalogPins, pin.Number) > -1) ? Array.IndexOf (Board.HardwareAnalogPins, pin.Number) : -1);
			}
		}

		#region ISerializable implementation

		public void GetObjectData (SerializationInfo info, StreamingContext context)
		{
			info.AddValue ("Board", board);
			info.AddValue ("Pins", Pins);
			info.AddValue ("MeasurementCombinations", MeasurementCombinations);
			info.AddValue ("Sequences", Sequences);
			info.AddValue ("ConfigSavePath", ConfigSavePath);
			info.AddValue ("LogFilePath", LogFilePath);
			info.AddValue ("UseMarker", UseMarker);
			info.AddValue ("LogRAWValues", LogRAWValues);

			info.AddValue ("CSVSeparator", Separator);
			info.AddValue ("EmptyValueFilling", EmptyValueFilling);
			info.AddValue ("UTCTimestamp", UTCTimestamp);
			info.AddValue ("LocalTimestamp", LocalTimestamp);
			info.AddValue ("TimeFormat", TimeFormat);
			info.AddValue ("FileNameConvention", FileNameConvention.ToList ());
		}

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
			LogFilePath = info.GetString ("LogFilePath");
			UseMarker = info.GetBoolean ("UseMarker");
			LogRAWValues = info.GetBoolean ("LogRAWValues");

			Separator = info.GetString ("CSVSeparator");
			EmptyValueFilling = info.GetString ("EmptyValueFilling");
			UTCTimestamp = info.GetBoolean ("UTCTimestamp");
			LocalTimestamp = info.GetBoolean ("LocalTimestamp");
			TimeFormat = info.GetString ("TimeFormat");
			FileNameConvention = ((List<string>)info.GetValue ("FileNameConvention", new List<string> ().GetType ())).ToArray<string> ();
		}

		#endregion
	}
}

