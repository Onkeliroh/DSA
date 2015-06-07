using System;

namespace PrototypeBackend
{
	public class MeasurementDate
	{
		public ArduinoController.PinType? pinType;
		public int pinNr;
		public DateTime dueTime;
		public Action pinCmd;

		public static bool operator < (MeasurementDate md1, MeasurementDate md2)
		{
			return md1.dueTime < md2.dueTime;
		}

		public static bool operator > (MeasurementDate md1, MeasurementDate md2)
		{
			return md1.dueTime > md2.dueTime;
		}

		public static bool operator >= (MeasurementDate md1, MeasurementDate md2)
		{
			return md1.dueTime >= md2.dueTime;
		}

		public static bool operator <= (MeasurementDate md1, MeasurementDate md2)
		{
			return md1.dueTime <= md2.dueTime;
		}

		public static bool operator == (MeasurementDate md1, MeasurementDate md2)
		{
			return md1.dueTime == md2.dueTime;
		}

		public static bool operator != (MeasurementDate md1, MeasurementDate md2)
		{
			return md1.dueTime != md2.dueTime;
		}

		public override bool Equals (object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (obj is MeasurementDate)
			{
				return ((obj as MeasurementDate).dueTime == dueTime
				&& (obj as MeasurementDate).pinCmd == pinCmd
				&& (obj as MeasurementDate).pinNr == pinNr
				&& (obj as MeasurementDate).pinType == pinType);
			}
			return false;
		}

		public override int GetHashCode ()
		{
			return base.GetHashCode ();
		}
	}
}

