using System;

namespace PrototypeBackend
{
	[Serializable]
	public struct SequenceOperation
	{
		public DPinState State { get; set; }

		public TimeSpan Duration { get; set; }

		public TimeSpan Moment { get; set; }
	}
}

