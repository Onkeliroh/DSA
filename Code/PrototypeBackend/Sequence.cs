using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Gdk;

namespace PrototypeBackend
{
	public enum SequenceState
	{
		New,
		Running,
		Done,
	}

	[Serializable]
	public class Sequence : ISerializable
	{
		public DPin Pin { get; set; }

		public string GroupName;

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
			GroupName = "";
			Chain = new List<SequenceOperation> ();
			Repetitions = 0;
			Cycle = 0;
			CurrentOperation = 0;
			CurrentState = SequenceState.New;
			lastOperation = new TimeSpan (0);
		}

		public Sequence (Sequence copy) : base ()
		{
			Pin = new DPin ();
			Name = copy.Name;
			Chain = new List<SequenceOperation> (copy.Chain);
			Repetitions = copy.Repetitions;
		}

		/// <summary>
		/// Adds asequence operation.
		/// </summary>
		/// <param name="dps">digital pin state to be set</param>
		/// <param name="starttime">Time after the last operation or start</param>
		public void AddSequenceOperation (DPinState dps, TimeSpan duration)
		{
			Chain.Add (new SequenceOperation () {
				State = dps,
				Duration = duration,
				Moment = new TimeSpan (Chain.Sum (o => o.Duration.Ticks))
			});
		}

		/// <summary>
		/// Adds a sequence operation.
		/// </summary>
		/// <param name="seqop">Sequenceoperation to be added</param>
		public void AddSequenceOperation (SequenceOperation seqop)
		{
			seqop.Moment = new TimeSpan (Chain.Sum (o => o.Duration.Ticks));
			Chain.Add (seqop);
		}

		/// <summary>
		/// Adds the sequence operation range.
		/// </summary>
		/// <param name="seqops">Seqops.</param>
		public void AddSequenceOperationRange (SequenceOperation[] seqops)
		{
			for (int i = 0; i < seqops.Length; i++)
			{
				AddSequenceOperation (seqops [i]);
			}
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
			CurrentState = SequenceState.New;
		}

		/// <summary>
		/// Returns the next Operation to be done
		/// </summary>
		public SequenceOperation? Next ()
		{
			CurrentOperation += 1;

			//one cycle is finished -> start new cycle
			if (CurrentOperation == Chain.Count)
			{
				CurrentOperation = 0;
				Cycle += 1;
			}

			//if sequence is done
			if (CurrentState == SequenceState.Done || ((Cycle > Repetitions || Chain.Count == 0) && Repetitions != -1))
			{
				CurrentState = SequenceState.Done;
				return  null;
			} else
			{
				CurrentState = SequenceState.Running;
				lastOperation += Chain [CurrentOperation].Duration;

				SequenceOperation op = Chain [CurrentOperation];
				return op;
			}
		}

		/// <summary>
		/// Returns the current SequenceOperation.
		/// </summary>
		public SequenceOperation? Current ()
		{
			if (Chain.Count > 0)
			{
				return Chain [CurrentOperation];
			} else
			{
				return null;
			}
		}

		public DPinState GetCurrentState (double milli)
		{
			int multiplier = 0;
			if (milli >= Runtime.TotalMilliseconds)
			{
				multiplier = (int)(System.Math.Floor (milli / Runtime.TotalMilliseconds));
				milli -= multiplier * Runtime.TotalMilliseconds;
			}
 			
			SequenceOperation op = new SequenceOperation ();
			if (Chain.Count > 0)
			{
				if (multiplier >= Repetitions && Repetitions != -1 && multiplier != 0)
				{
					return Chain.Last ().State;
				} else
				{
					op = Chain [0];
					foreach (SequenceOperation seqop in Chain)
					{
						if (seqop.Moment.TotalMilliseconds == milli)
						{
							return seqop.State;
						} else if (seqop.Moment.TotalMilliseconds < milli)
						{
							if ((milli - seqop.Moment.TotalMilliseconds) < (milli - op.Moment.TotalMilliseconds))
							{
								op = seqop;	
							}

						}
					}
					return op.State;
				}
			}
			return DPinState.LOW;
		}

		public override string ToString ()
		{
			string res = String.Format ("Name: {0}\t[Pin: {1}]", Name, Pin);
			return res;
		}

		public override bool Equals (object obj)
		{
			var seq = obj as Sequence;
			if (seq != null)
			{
				return( 
				    this.Pin.Equals (seq.Pin) &&
				    this.Chain.SequenceEqual (seq.Chain) &&
				    this.Name.Equals (seq.Name) &&
				    this.Repetitions.Equals (seq.Repetitions));
			}
			return false;
		}

		public string ToStringLong ()
		{
			string res = String.Format ("Name: {0}\n[Pin: {1}]\nColor {2}\tRepetitions {3}", Name, Pin, Color, Repetitions);
			res += "\nOperations:";
			foreach (SequenceOperation seqop in Chain)
			{
				res += string.Format ("\nDuration: {0}\tState: {1}", seqop.Duration, seqop.State);
			}
			return res;
		}

		#region ISerializable implementation

		public void GetObjectData (SerializationInfo info, StreamingContext context)
		{
			info.AddValue ("Pin", Pin);
			info.AddValue ("Name", Name);
			info.AddValue ("GroupName", GroupName);
			info.AddValue ("RED", uintToByte (Color.Red));
			info.AddValue ("GREEN", uintToByte (Color.Green));
			info.AddValue ("BLUE", uintToByte (Color.Blue));
			info.AddValue ("Chain", Chain);
			info.AddValue ("Repetitions", Repetitions);
		}

		public Sequence (SerializationInfo info, StreamingContext context)
		{
			Pin = new DPin ();
			Pin = (DPin)info.GetValue ("Pin", Pin.GetType ());

			Name = info.GetString ("Name");

			GroupName = info.GetString ("GroupName");

			Chain = new List<SequenceOperation> ();
			Chain = (List<SequenceOperation>)info.GetValue ("Chain", Chain.GetType ());

			Repetitions = info.GetInt32 ("Repetitions");

			Color = new Gdk.Color (info.GetByte ("RED"), info.GetByte ("GREEN"), info.GetByte ("BLUE"));
		}

		#endregion

		public static byte uintToByte (uint val)
		{
			return (byte)(byte.MaxValue / 65535.0 * val);
		}
	}
}