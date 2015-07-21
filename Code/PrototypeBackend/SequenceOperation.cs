using System;

namespace PrototypeBackend
{
	public struct SequenceOperation
	{
		public DPinState State { get; set; }

		public TimeSpan Duration { get; set; }
	}
}

