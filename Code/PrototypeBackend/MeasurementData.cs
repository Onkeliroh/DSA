using System;

namespace PrototypeBackend
{
	public class MeasurementData : IPin
	{
		public ArduinoController.PinType? PinType { get; set; }

		public ArduinoController.PinMode? PinMode { get; set; }

		public string PinLabel { get; set; }

		public string Unit { get; set; }

		public int PinNr{ get; set; }

		public int? RelativCurrentPin { get; set; }

		public Action PinCmd{ get; set; }

		public override bool Equals (object obj)
		{
			if (obj != null)
			{
				if (obj is MeasurementData)
				{
					return (obj as MeasurementData).PinType == PinType &&
					(obj as MeasurementData).PinMode == PinMode &&
					(obj as MeasurementData).PinLabel.Equals (PinLabel) &&
					(obj as MeasurementData).Unit.Equals (Unit) &&
					(obj as MeasurementData).PinNr.Equals (PinNr);
				}
			}
			return false;
		}

		public override int GetHashCode ()
		{
			return base.GetHashCode ();
		}


		public override string ToString ()
		{
			return String.Format ("Label: {0}\tNumber: {1}\tPinType: {2}\tUnit: {3}", PinLabel, PinNr, PinType, Unit);
		}
	}
}

