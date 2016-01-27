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

	/// <summary>
	/// Sequence.
	/// </summary>
	[Serializable]
	public class Sequence : ISerializable
	{
		/// <summary>
		/// Gets or sets the pin.
		/// </summary>
		/// <value>The pin.</value>
		public DPin Pin { get; set; }

		/// <summary>
		/// The name of the group.
		/// </summary>
		public string GroupName;

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name.</value>
		public string Name { get; set; }

		/// <summary>
		/// Gets the color.
		/// </summary>
		/// <value>The color.</value>
		public Gdk.Color Color { get { return Pin.PlotColor; } private set { } }

		/// <summary>
		/// Gets or sets the <see cref="SequenceOperation"/>s.
		/// </summary>
		/// <value>The chain.</value>
		public List<SequenceOperation> Chain { get; set; }

		/// <summary>
		/// Gets the runtime.
		/// </summary>
		/// <value>The runtime.</value>
		public TimeSpan Runtime { 
			get { 
				return TimeSpan.FromTicks (Chain.Select (x => x.Duration.Ticks).ToList ().Sum ()); 
			}
			private set{ }
		}

		/// <summary>
		/// Gets the cycle.
		/// Determines the number of repetitions done.
		/// </summary>
		/// <value>The cycle.</value>
		public int Cycle { get; private set; }

		/// <summary>
		/// Gets the current operation.
		/// </summary>
		/// <value>The current operation.</value>
		public int CurrentOperation { get; private set; }

		/// <summary>
		/// Gets the state of the current.
		/// </summary>
		/// <value>The state of the current.</value>
		public SequenceState CurrentState { get; private set; }

		/// <summary>
		/// Gets the last operation.
		/// </summary>
		/// <value>The last operation.</value>
		public TimeSpan LastOperation{ get; private set; }

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
			LastOperation = new TimeSpan (0);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PrototypeBackend.Sequence"/> class.
		/// </summary>
		/// <param name="copy">Copy.</param>
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
				LastOperation += Chain [CurrentOperation].Duration;

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

		/// <summary>
		/// Gets the sequence state depending on the given time.
		/// </summary>
		/// <returns>The current state.</returns>
		/// <param name="milli">time in milliseconds</param>
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

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="PrototypeBackend.Sequence"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="PrototypeBackend.Sequence"/>.</returns>
		public override string ToString ()
		{
			string res = String.Format ("Name: {0}\t[Pin: {1}]", Name, Pin);
			return res;
		}

		/// <summary>
		/// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="PrototypeBackend.Sequence"/>.
		/// </summary>
		/// <param name="obj">The <see cref="System.Object"/> to compare with the current <see cref="PrototypeBackend.Sequence"/>.</param>
		/// <returns><c>true</c> if the specified <see cref="System.Object"/> is equal to the current
		/// <see cref="PrototypeBackend.Sequence"/>; otherwise, <c>false</c>.</returns>
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

		/// <summary>
		/// Returns a more detailed <see cref="System.String"/> that represents the current <see cref="PrototypeBackend.Sequence"/>.
		/// </summary>
		/// <returns>A more detailed <see cref="System.String"/> that represents the current <see cref="PrototypeBackend.Sequence"/>.</returns>
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

		/// <summary>
		/// Gets the object data.
		/// </summary>
		/// <param name="info">Info.</param>
		/// <param name="context">Context.</param>
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

		/// <summary>
		/// Initializes a new instance of the <see cref="PrototypeBackend.Sequence"/> class.
		/// </summary>
		/// <param name="info">Info.</param>
		/// <param name="context">Context.</param>
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

		/// <summary>
		/// Uints to byte.
		/// This method is used to parse colors form one framework to another.
		/// </summary>
		/// <returns>The to byte.</returns>
		/// <param name="val">Value.</param>
		public static byte uintToByte (uint val)
		{
			return (byte)(byte.MaxValue / 65535.0 * val);
		}
	}
}