﻿using System;
using System.Linq;
using System.Collections.Generic;
using Gdk;

namespace PrototypeBackend
{
	public enum SequenceState
	{
		New,
		Running,
		Done,
	}

	public class Sequence
	{
		public DPin Pin { get; set; }

		public string Name { get; set; }

		public Gdk.Color Color { get { return Pin.PlotColor; } private set { } }

		public List<SequenceOperation> Chain { get; set; }

		public TimeSpan Runtime { 
			get { 
				return TimeSpan.FromTicks (Chain.Select (x => x.Duration.Ticks).ToList ().Sum ()); 
			}
			private set{ }
		}

		public int Cycle { get; private set; }

		public int CurrentOperation { get; private set; }

		public SequenceState CurrentState { get; private set; }

		public TimeSpan lastOperation{ get; private set; }

		/// <summary>
		/// Gets or sets the repetitions.
		/// 0 = one cycle
		/// -1 = neverending
		/// </summary>
		/// <value>The repetitions.</value>
		public int Repetitions { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="PrototypeBackend.Sequence"/> class.
		/// </summary>
		public Sequence ()
		{
			Pin = new DPin ();
			Name = "";
			Chain = new List<SequenceOperation> ();
			Repetitions = 0;
			Cycle = 0;
			CurrentOperation = 0;
			CurrentState = SequenceState.New;
			lastOperation = new TimeSpan (0);
		}

		/// <summary>
		/// Adds asequence operation.
		/// </summary>
		/// <param name="dps">digital pin state to be set</param>
		/// <param name="starttime">Time after the last operation or start</param>
		public void AddSequenceOperation (DPinState dps, TimeSpan duration)
		{
			Chain.Add (new SequenceOperation (){ State = dps, Duration = duration });
		}

		/// <summary>
		/// Adds a sequence operation.
		/// </summary>
		/// <param name="seqop">Sequenceoperation to be added</param>
		public void AddSequenceOperation (SequenceOperation seqop)
		{
			Chain.Add (seqop);
		}

		public void AddSequenceOperationRange (SequenceOperation[] seqops)
		{
			Chain.AddRange (seqops);
		}

		/// <summary>
		/// Removes the last operation.
		/// </summary>
		public void RemoveLastOperation ()
		{
			Chain.Remove (Chain.Last ());
		}

		public void Reset ()
		{
			Cycle = 0;
			CurrentOperation = 0;
		}

		/// <summary>
		/// Returns the next Operation to be done
		/// </summary>
		public SequenceOperation? Next ()
		{
			lastOperation += Chain [CurrentOperation].Duration;
			if (Cycle > Repetitions || Chain.Count == 0)
			{
				CurrentState = SequenceState.Done;
				return  null;
			}
			CurrentOperation += 1;
			if (CurrentOperation == Chain.Count)
			{
				CurrentOperation = 0;
				Cycle += 1;
			}
			SequenceOperation op = Chain [CurrentOperation];
			return op;
		}

		/// <summary>
		/// Returns the current SequenceOperation.
		/// </summary>
		public SequenceOperation? Current ()
		{
			if (Cycle > Repetitions || Chain.Count == 0)
			{
				CurrentState = SequenceState.Done;
				return  null;
			}
			SequenceOperation op = Chain [CurrentOperation];
			CurrentState = SequenceState.Running;
			return op;
		}

		public override string ToString ()
		{
			string res = String.Format ("Name: {0}\t[Pin: {1}]", Name, Pin);
			return res;
		}

		public string ToStringLong ()
		{
			string res = String.Format ("Name: {0}\t[Pin: {1}]", Name, Pin);
			res += "\nOperations:";
			foreach (SequenceOperation seqop in Chain)
			{
				res += string.Format ("\nDuration: {0}\tState: {1}", seqop.Duration, seqop.State);
			}
			return res;
		}
	}
}
