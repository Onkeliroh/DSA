﻿using System;
using System.Collections.Generic;
using System.Linq;
using PrototypeBackend;

namespace PrototypeBackend
{
	public class BoardConfiguration
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
			}
		}

		private Board board;

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

		#endregion

		#region EventHandler

		public EventHandler<MeasurementCombinationsUpdatedArgs> OnSignalsUpdated;
		public EventHandler<ControllerPinUpdateArgs> OnPinsUpdated;
		public EventHandler<SequencesUpdatedArgs> OnSequencesUpdated;

		#endregion

		public BoardConfiguration ()
		{
			board = null;
			Pins = new List<IPin> ();
			MeasurementCombinations = new List<MeasurementCombination> ();
			Sequences = new List<Sequence> ();
		}

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
			return unusedpins.ToArray<APin> ();
		}

		private DPin[] GetUnusedDigitalPins ()
		{
			var unusedpins = new List<DPin> ();
			var numpins = Board.NumberOfDigitalPins; 
			for (uint i = 0; i < numpins; i++) {
				unusedpins.Add (new DPin () {
					Number = i,
					AnalogNumber = ((Array.IndexOf (Board.HardwareAnalogPins, i) > -1) ? (uint?)Array.IndexOf (Board.HardwareAnalogPins, i) : null),
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
			foreach (Sequence seq in Sequences) {
				if (seq.Pin == pin) {
					return seq;
				}
			}
			return null;
		}

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

		public void AddPin (IPin pin)
		{
			if (!Pins.Contains (pin) && pin != null) {
				Pins.Add (pin);
				Pins = Pins.OrderBy (x => x.Number).ThenBy (x => x.Type).ToList ();
				if (OnPinsUpdated != null) {
					OnPinsUpdated.Invoke (this, new ControllerPinUpdateArgs (pin, UpdateOperation.Add));
				}
			}
		}

		public void AddMeasurementCombination (MeasurementCombination s)
		{
			if (!MeasurementCombinations.Contains (s)) {
				MeasurementCombinations.Add (s);
			}

			if (OnSignalsUpdated != null) {
				OnSignalsUpdated.Invoke (this, new MeasurementCombinationsUpdatedArgs (UpdateOperation.Add, s));
			}
		}

		public void AddSequence (Sequence seq)
		{
			if (!Sequences.Contains (seq)) {
				Sequences.Add (seq);
				if (OnSequencesUpdated != null) {
					OnSequencesUpdated.Invoke (this, new SequencesUpdatedArgs (UpdateOperation.Add, seq));
				}
			}
		}

		#endregion

		#region Set

		public void SetPin (int index, IPin ip)
		{
			if (OnPinsUpdated != null) {
				OnPinsUpdated.Invoke (this, new ControllerPinUpdateArgs (Pins [index], UpdateOperation.Change, ip));
			}
			Pins [index] = ip;
		}

		public void SetMeasurmentCombination (int index, MeasurementCombination s)
		{
			if (s != null) {
				OnSignalsUpdated.Invoke (this, new MeasurementCombinationsUpdatedArgs (UpdateOperation.Change, MeasurementCombinations [index], s));
			}
			MeasurementCombinations [index] = s;
		}

		public void SetSequence (int index, Sequence seq)
		{
			if (OnSequencesUpdated != null) {
				OnSequencesUpdated.Invoke (this, new SequencesUpdatedArgs (UpdateOperation.Change, Sequences [index], seq));
			}
			Sequences [index] = seq;
		}

		#endregion

		#region Remove

		public void RemovePin (string name)
		{
			var result = Pins.Where (o => o.Name == name).ToList<IPin> ();

			if (result.Count > 0) {
				Pins.Remove (result [0]);
				if (OnPinsUpdated != null) {
					OnPinsUpdated.Invoke (this, new ControllerPinUpdateArgs (result [0], UpdateOperation.Remove));
				}
			}
		}

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

		public void RemoveMeasurementCombination (int index)
		{
			var sig = new MeasurementCombination ();
			sig = MeasurementCombinations [index];
			MeasurementCombinations.RemoveAt (index);
		
			if (OnSignalsUpdated != null) {
				OnSignalsUpdated.Invoke (this, new MeasurementCombinationsUpdatedArgs (UpdateOperation.Remove, sig));
			}
		}

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

		public void RemoveMeasurementCombination (MeasurementCombination index)
		{
			if (index != null) {
				if (OnSignalsUpdated != null) {
					OnSignalsUpdated.Invoke (this, new MeasurementCombinationsUpdatedArgs (UpdateOperation.Remove, index));
				}
				MeasurementCombinations.Remove (index);
			}
		}

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

		#endregion

		#region Clear

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

		public void ClearPins ()
		{
			Pins.Clear ();
			if (OnPinsUpdated != null) {
				OnPinsUpdated.Invoke (this, new ControllerPinUpdateArgs (null, UpdateOperation.Clear, null));
			}
		}

		public void ClearMeasurementCombinations ()
		{
			MeasurementCombinations.Clear ();

			if (OnSignalsUpdated != null) {
				OnSignalsUpdated.Invoke (this, new MeasurementCombinationsUpdatedArgs (UpdateOperation.Clear, null));
			}
		}

		public void ClearSequences ()
		{
			Sequences.Clear ();
			if (OnSequencesUpdated != null) {
				OnSequencesUpdated.Invoke (this, new SequencesUpdatedArgs (UpdateOperation.Clear));
			}
		}

		#endregion

		private void CheckPins ()
		{
			foreach (APin pin in AnalogPins) {
				pin.DigitalNumber = board.HardwareAnalogPins [pin.Number];
			}
			foreach (DPin pin in DigitalPins) {
				pin.AnalogNumber = ((Array.IndexOf (Board.HardwareAnalogPins, pin.Number) > -1) ? (uint?)Array.IndexOf (Board.HardwareAnalogPins, pin.Number) : null);
			}
		}
	}
}
