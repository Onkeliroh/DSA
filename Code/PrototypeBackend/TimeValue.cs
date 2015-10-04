using System;

namespace PrototypeBackend
{
	public struct TimeValue
	{
		public TimeSpan Time{ get; set; }

		public double Value { get; set; }
	}

	public struct DateTimeValue
	{
		public DateTime Time;

		public double Value;

		public DateTimeValue (double value, DateTime time)
		{
			Value = value;
			Time = time;
		}

		public override string ToString ()
		{
			return string.Format ("{0}", Value);
		}
	}
}

