using System;

namespace GUIHelper
{
	public struct TimeValue
	{
		public TimeSpan Time{ get; set; }

		public double Value { get; set; }
	}

	public struct DateTimeValue
	{
		public DateTime Time{ get; set; }

		public double Value { get; set; }
	}
}

