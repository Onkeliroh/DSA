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
	}
}

