using System;
using PrototypeBackend;
using System.Collections.Generic;

namespace PrototypeBackend
{
	public class Scheduler
	{
		public string Label = "";
		public DateTime DueTime;
		public double Interval;

		public int? Repetitions {
			get{ return Repetitions; }
			set {
				Repetitions = value;
				RepetitionsLeft = value;
			}
		}

		public int? RepetitionsLeft;
		public List<IPin> Pins;

		public Scheduler ()
		{
		}

		#region Operators

		public static bool operator < (Scheduler s1, Scheduler s2)
		{
			return s1.DueTime < s2.DueTime;
		}

		public static bool operator > (Scheduler s1, Scheduler s2)
		{
			return s1.DueTime > s2.DueTime;
		}

		public static bool operator <= (Scheduler s1, Scheduler s2)
		{
			return s1.DueTime <= s2.DueTime;
		}

		public static bool operator >= (Scheduler s1, Scheduler s2)
		{
			return s1.DueTime >= s2.DueTime;
		}

		public static bool operator == (Scheduler s1, Scheduler s2)
		{
			return s1.DueTime == s2.DueTime;
		}

		public static bool operator != (Scheduler s1, Scheduler s2)
		{
			return s1.DueTime != s2.DueTime;
		}

		#endregion

		public override bool Equals (object s)
		{
			return DueTime == (s as Scheduler).DueTime;
		}

		public override int GetHashCode ()
		{
			return base.GetHashCode ();
		}

		public override string ToString ()
		{
			string tmp;

			tmp = "Label: " + Label + "\t Interval: " + Interval.ToString () + "\t Repetitions: " + Repetitions; 

			foreach (IPin ip in Pins)
			{
				tmp += "\n\r" + ip.ToString ();
			}

			return tmp;
		}

		public bool Run ()
		{
			foreach (IPin ip in Pins)
			{
				ip.PinCmd ();
			}
			if (Repetitions != null)
			{
				if (RepetitionsLeft > 0)
				{
					DueTime = DueTime.AddMilliseconds (Interval);
					RepetitionsLeft -= 1;
					return false;
				} else
				{
					return true;
				}
			} else
			{
				DueTime = DueTime.AddMilliseconds (Interval);
				return false;
			}
		}
	}
}

