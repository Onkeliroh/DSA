using System;

namespace PrototypeBackend
{
	public class MeasurementDate
	{
		public ArduinoController.PinType? PinType;
		public string PinLabel;
		public string Unit;
		public int PinNr;
		public DateTime DueTime;
		public ArduinoController.Command PinCmd;

		public static bool operator < (MeasurementDate md1, MeasurementDate md2)
		{
			return md1.DueTime < md2.DueTime;
		}

		public static bool operator > (MeasurementDate md1, MeasurementDate md2)
		{
			return md1.DueTime > md2.DueTime;
		}

		public static bool operator >= (MeasurementDate md1, MeasurementDate md2)
		{
			return md1.DueTime >= md2.DueTime;
		}

		public static bool operator <= (MeasurementDate md1, MeasurementDate md2)
		{
			return md1.DueTime <= md2.DueTime;
		}

		public static bool operator == (MeasurementDate md1, MeasurementDate md2)
		{
			return md1.DueTime == md2.DueTime;
		}

		public static bool operator != (MeasurementDate md1, MeasurementDate md2)
		{
			return md1.DueTime != md2.DueTime;
		}

		public override bool Equals (object obj)
		{
			if (obj == null) {
				return false;
			}
			var measurementDate = obj as MeasurementDate;
			if (measurementDate != null) {
				return (measurementDate.DueTime == DueTime
				&& measurementDate.PinCmd == PinCmd
				&& measurementDate.PinNr == PinNr
				&& measurementDate.PinType == PinType
				&& measurementDate.PinLabel.Equals (PinLabel)
				&& measurementDate.Unit.Equals (Unit)
				);
			}
			return false;
		}

		public override int GetHashCode ()
		{
			return base.GetHashCode ();
		}

		public override string ToString ()
		{
			return String.Format ("{0}\t{1}\t{2}\t{3}\t{4}", PinLabel, PinNr, DueTime, PinType, Unit);
		}
	}
}

