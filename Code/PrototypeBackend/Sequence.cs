using System;
using System.Linq;

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

		public System.Drawing.Color Color { get; set; }

		public System.Collections.Generic.List<SequenceOperation> Chain { get; set; }

		public int Cycle { get; private set; }

		public int CurrentOperation { get; private set; }

		public SequenceState CurrentState { get; private set; }

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
			Pin = null;
			Name = "";
			Color = System.Drawing.Color.Empty;
			Chain = new System.Collections.Generic.List<SequenceOperation> ();
			Repetitions = 0;
			Cycle = 0;
			CurrentOperation = 0;
			CurrentState = SequenceState.New;
		}

		/// <summary>
		/// Adds asequence operation.
		/// </summary>
		/// <param name="dps">digital pin state to be set</param>
		/// <param name="starttime">Time after the last operation or start</param>
		public void AddSequenceOperation (DPinState dps, TimeSpan starttime, TimeSpan duration)
		{
			Chain.Add (new SequenceOperation (){ State = dps, Time = starttime, Duration = duration });
		}

		/// <summary>
		/// Adds a sequence operation.
		/// </summary>
		/// <param name="seqop">Sequenceoperation to be added</param>
		public void AddSequenceOperation (SequenceOperation seqop)
		{
			Chain.Add (seqop);
			Chain = Chain.OrderBy (o => o.Time).ToList ();
//			Chain.Reverse ();
		}

		public void AddSequenceOperationRange (SequenceOperation[] seqops)
		{
			Chain.AddRange (seqops);
			Chain = Chain.OrderBy (o => o.Time).ToList ();
//			Chain.Reverse ();
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
			string res = String.Format ("Pin: {0}\tName: {1}", Pin, Name);
			foreach (SequenceOperation seqop in Chain)
			{
				res += string.Format ("\nStartTime: {0}\tDuration: {1}\tState: {2}", seqop.Time, seqop.Duration, seqop.State);
			}
			return res;
		}
	}
}
