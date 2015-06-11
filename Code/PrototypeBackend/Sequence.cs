using System;
using ArduinoController;
using System.Security.Cryptography;

namespace PrototypeBackend
{
	public class Sequence
	{
		public ArduinoController.PinType? PinType;
		public string PinLabel;
		public int PinNr;
		public ArduinoController.DPinState PinState = ArduinoController.DPinState.LOW;
		public DateTime DueTime;
		public double Interval;
		public Action PinCmd;

		public Sequence ()
		{
		}

		public Sequence (string label, DateTime time, int pinnr)
		{
			PinLabel = label;
			DueTime = time;
			PinNr = pinnr;
		}

		public static bool operator < (Sequence s1, Sequence s2)
		{
			return s1.DueTime < s2.DueTime;
		}

		public static bool operator > (Sequence s1, Sequence s2)
		{
			return s1.DueTime > s2.DueTime;
		}

		public static bool operator <= (Sequence s1, Sequence s2)
		{
			return s1.DueTime <= s2.DueTime;
		}

		public static bool operator >= (Sequence s1, Sequence s2)
		{
			return s1.DueTime >= s2.DueTime;
		}

		public static bool operator == (Sequence s1, Sequence s2)
		{
			return s1.DueTime == s2.DueTime;
		}

		public static bool operator != (Sequence s1, Sequence s2)
		{
			return s1.DueTime != s2.DueTime;
		}

		public override bool Equals (object obj)
		{
			var seq = obj as Sequence;
			if (seq != null)
			{
				return (seq.PinNr == PinNr
				&& seq.PinLabel.Equals (PinLabel)
				&& seq.DueTime.Equals (DueTime)
				&& seq.PinState.Equals (PinState));
			}
			return false;
		}

		public override int GetHashCode ()
		{
			return base.GetHashCode ();
		}

		public override string ToString ()
		{
			return string.Format ("{0}\t{1}\t{2}\t{3}", PinLabel, PinNr, DueTime, PinState);
		}
	}
}

