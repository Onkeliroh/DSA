using System;

namespace PrototypeBackend
{
	/// <summary>
	/// Time value.
	/// </summary>
	public struct TimeValue
	{
		/// <summary>
		/// Gets or sets the time.
		/// </summary>
		/// <value>The time.</value>
		public TimeSpan Time{ get; set; }

		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		/// <value>The value.</value>
		public double Value { get; set; }
	}

	/// <summary>
	/// Date time value.
	/// </summary>
	public struct DateTimeValue
	{
		/// <summary>
		/// The time.
		/// </summary>
		public double Time;

		/// <summary>
		/// The value.
		/// </summary>
		public double Value;

		/// <summary>
		/// Initializes a new instance of the <see cref="PrototypeBackend.DateTimeValue"/> struct.
		/// </summary>
		/// <param name="value">Value.</param>
		/// <param name="time">Time.</param>
		public DateTimeValue (double value, DateTime time)
		{
			Value = value;
			Time = time.ToOADate ();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PrototypeBackend.DateTimeValue"/> struct.
		/// </summary>
		/// <param name="value">Value.</param>
		/// <param name="time">Time.</param>
		public DateTimeValue (double value, double time)
		{
			Value = value;
			Time = time;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PrototypeBackend.DateTimeValue"/> struct.
		/// </summary>
		/// <param name="copy">Copy.</param>
		public DateTimeValue (DateTimeValue copy)
		{
			Value = copy.Value;
			Time = copy.Time;
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="PrototypeBackend.DateTimeValue"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="PrototypeBackend.DateTimeValue"/>.</returns>
		public override string ToString ()
		{
			return string.Format ("{0}", Value);
		}
	}
}

