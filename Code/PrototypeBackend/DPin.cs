using System;
using PrototypeBackend;
using System.Xml.Serialization;
using System.IO;
using System.Collections.Generic;

namespace PrototypeBackend
{
	public class DPin : IPin
	{
		public PinType Type { get; set; }

		public PinMode Mode { get; set; }

		public string Name { get; set; }

		public string DisplayName { get { return string.Format ("{0} (D{1})", Name, Number); } set { } }

		public string DisplayNumber {
			get {
				if (AnalogNumber != null)
					return string.Format ("D{0} | A{1}", Number, AnalogNumber);
				else
					return string.Format ("D{0}", Number);
			}
			set { }
		}

		public uint Number { get ; set; }

		public uint RealNumber { get { return Number; } set { } }

		public uint? AnalogNumber { get; set; }

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
			AnalogNumber = null;
			PlotColor = Gdk.Color.Zero;
		}

		public DPin (string label, uint pinnr)
		{
			Name = label;
			Number = pinnr;
			Type = PrototypeBackend.PinType.DIGITAL;
			Mode = PrototypeBackend.PinMode.OUTPUT;
			AnalogNumber = null;
			PlotColor = Gdk.Color.Zero;
		}

		//Methods

		public override bool Equals (object obj)
		{
			var seq = obj as DPin;
			if (seq != null)
			{
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
			switch (Mode)
			{
			case PrototypeBackend.PinMode.OUTPUT:
				PrototypeBackend.ArduinoController.SetPin (Number, Mode, State);
				break;
			case PrototypeBackend.PinMode.INPUT:
				State = PrototypeBackend.ArduinoController.ReadPin (Number);
				break;
			}
		}
	}
}

