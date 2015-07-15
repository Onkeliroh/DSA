using System;
using System.Linq;

namespace PrototypeBackend
{
	public class Sequence
	{
		public DPin Pin { get; set; }

		public string Name { get; set; }

		public System.Drawing.Color Color { get; set; }

		public System.Collections.Generic.List<SequenceOperation> Chain { get; set; }

		public int Cycle { get; private set; }

		public int CurrentOperation { get; private set; }

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
			if (Cycle > Repetitions)
			{
				return  null;
			}
			SequenceOperation op = Chain [CurrentOperation];
			CurrentOperation += 1;
			if (CurrentOperation == Chain.Count)
			{
				CurrentOperation = 0;
				Cycle += 1;
			}
			return op;
		}
	}
}
