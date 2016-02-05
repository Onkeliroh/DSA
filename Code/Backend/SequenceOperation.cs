using System;

namespace Backend
{
	/// <summary>
	/// Sequence operation.
	/// </summary>
	[Serializable]
	public struct SequenceOperation
	{
		/// <summary>
		/// Gets or sets the digital pin state.
		/// </summary>
		/// <value>The state.</value>
		public DPinState State { get; set; }

		/// <summary>
		/// Gets or sets the duration.
		/// </summary>
		/// <value>The duration.</value>
		public TimeSpan Duration { get; set; }

		/// <summary>
		/// Gets or sets the moment this operation is up.
		/// </summary>
		/// <value>The moment.</value>
		public TimeSpan Moment { get; set; }
	}
}

