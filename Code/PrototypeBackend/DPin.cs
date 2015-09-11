using System;
using PrototypeBackend;
using System.Xml.Serialization;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PrototypeBackend
{
	[Serializable]
	public class DPin : IPin, ISerializable
	{
		public PinType Type { get; set; }

		public PinMode Mode { get; set; }

		public string Name { get; set; }

		public string DisplayName { get { return string.Format ("{0} (D{1})", Name, Number); } set { } }

		public string DisplayNumber {
			get {
				if (AnalogNumber != -1)
					return string.Format ("D{0} | A{1}", Number, AnalogNumber);
				else
					return string.Format ("D{0}", Number);
			}
			set { }
		}

		public uint Number { get ; set; }

		public uint RealNumber { get { return Number; } set { } }

		public int AnalogNumber { get; set; }

		public bool SDA { get ;	set ; }

		public bool SCL { get ; set ; }

		public bool RX { get; set; }

		public bool TX { get; set; }

		public Gdk.Color PlotColor { get; set; }

		public PrototypeBackend.DPinState State = PrototypeBackend.DPinState.LOW;

		//Constructors

		public DPin ()
		{
			Type = PrototypeBackend.PinType.DIGITAL;
			Mode = PrototypeBackend.PinMode.OUTPUT;
			Name = "";
			Number = 0;
			AnalogNumber = -1;
			PlotColor = Gdk.Color.Zero;
		}

		public DPin (string label, uint pinnr)
		{
			Name = label;
			Number = pinnr;
			Type = PrototypeBackend.PinType.DIGITAL;
			Mode = PrototypeBackend.PinMode.OUTPUT;
			AnalogNumber = -1;
			PlotColor = Gdk.Color.Zero;
		}

		//Methods

		public override bool Equals (object obj)
		{
			var seq = obj as DPin;
			if (seq != null) {
				return (seq.Number == Number)
//				&& seq.Name.Equals (Name)
//				&& seq.State.Equals (State)
//				&& seq.PlotColor.Equals (PlotColor)
				&& seq.Type.Equals (Type);
//				&& seq.Mode.Equals (Mode));
			}
			return false;
		}

		public override int GetHashCode ()
		{
			return base.GetHashCode ();
		}

		public override string ToString ()
		{
			return string.Format ("Name: {0}\tNumber: {1}\tType: {2}", Name, Number, Type);
		}

		public string ToXML ()
		{
			XmlSerializer tmp = new XmlSerializer (typeof(DPin));
			string returnstring = "";
			TextWriter tw = new StreamWriter (returnstring);
			tmp.Serialize (tw, this);
			tw.Close ();
			return returnstring;
		}

		public void Run ()
		{
			switch (Mode) {
			case PrototypeBackend.PinMode.OUTPUT:
				PrototypeBackend.ArduinoController.SetPin (Number, Mode, State);
				break;
			case PrototypeBackend.PinMode.INPUT:
				State = PrototypeBackend.ArduinoController.ReadPin (Number);
				break;
			}
		}

		#region ISerializable implementation

		public void GetObjectData (SerializationInfo info, StreamingContext context)
		{
			info.AddValue ("Type", Type);
			info.AddValue ("Mode", Mode);
			info.AddValue ("Name", Name);
			info.AddValue ("Number", Number);
			info.AddValue ("AnalogNumber", AnalogNumber);
			info.AddValue ("SDA", SDA);
			info.AddValue ("SCL", SCL);
			info.AddValue ("RX", RX);
			info.AddValue ("TX", TX);
			info.AddValue ("RED", uintToByte (PlotColor.Red));
			info.AddValue ("GREEN", uintToByte (PlotColor.Green));
			info.AddValue ("BLUE", uintToByte (PlotColor.Blue));
		}

		//		void  ISerializable.GetObjectData (SerializationInfo info, StreamingContext context)
		public DPin (SerializationInfo info, StreamingContext context)
		{
			Type = (PinType)info.GetByte ("Type");
			Mode = (PinMode)info.GetByte ("Mode");
			Name = info.GetString ("Name");
			Number = info.GetUInt32 ("Number");
			AnalogNumber = info.GetInt32 ("AnalogNumber");
			SDA = info.GetBoolean ("SDA");
			SCL = info.GetBoolean ("SCL");
			RX = info.GetBoolean ("RX");
			TX = info.GetBoolean ("TX");
			PlotColor = new Gdk.Color (info.GetByte ("RED"), info.GetByte ("GREEN"), info.GetByte ("BLUE"));
		}

		#endregion

		public static byte uintToByte (uint val)
		{
			return (byte)(byte.MaxValue / 65535.0 * val);
		}
	}
}

