using System;
using PrototypeBackend;
using System.Xml.Serialization;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PrototypeBackend
{
	/// <summary>
	/// A class managing analog pins.
	/// </summary>
	[Serializable]
	public class DPin : IPin, ISerializable
	{
		/// <summary>
		/// Gets or sets the type.
		/// </summary>
		/// <value>The type.</value>
		public PinType Type { get; set; }

		/// <summary>
		/// Gets or sets the mode.
		/// </summary>
		/// <value>The mode.</value>
		public PinMode Mode { get; set; }

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name.</value>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the display name.
		/// </summary>
		/// <value>The display name.</value>
		public string DisplayName { get { return string.Format ("{0} (D{1})", Name, Number); } set { } }

		/// <summary>
		/// Gets or sets the display number.
		/// </summary>
		/// <value>The display number.</value>
		public string DisplayNumber {
			get {
				if (AnalogNumber != -1)
					return string.Format ("D{0} | A{1}", Number.ToString ().PadLeft (2, ' '), AnalogNumber);
				else
					return string.Format ("D{0}", Number.ToString ().PadLeft (2, ' '));
			}
			set { }
		}

		/// <summary>
		/// Gets or sets the display number short.
		/// </summary>
		/// <value>The display number short.</value>
		public string DisplayNumberShort { get { return string.Format ("D{0}", Number.ToString ().PadLeft (2, ' ')); } set { } }

		/// <summary>
		/// Gets or sets the number.
		/// </summary>
		/// <value>The number.</value>
		public uint Number { get ; set; }

		/// <summary>
		/// Gets or sets the real number.
		/// </summary>
		/// <value>The real number.</value>
		public uint RealNumber { get { return Number; } set { } }

		/// <summary>
		/// Gets or sets the analog number.
		/// </summary>
		/// <value>The analog number.</value>
		public int AnalogNumber { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="PrototypeBackend.DPin"/> is SDA enabled.
		/// </summary>
		/// <value><c>true</c> if SDA enabled; otherwise, <c>false</c>.</value>
		public bool SDA { get ;	set ; }

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="PrototypeBackend.DPin"/> is SCL enabled.
		/// </summary>
		/// <value><c>true</c> if SCL enabled; otherwise, <c>false</c>.</value>
		public bool SCL { get ; set ; }

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="PrototypeBackend.DPin"/> is RX enabled.
		/// </summary>
		/// <value><c>true</c> if RX enabled; otherwise, <c>false</c>.</value>
		public bool RX { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="PrototypeBackend.DPin"/> is TX enabled.
		/// </summary>
		/// <value><c>true</c> if TX enabled; otherwise, <c>false</c>.</value>
		public bool TX { get; set; }

		/// <summary>
		/// Gets or sets the color of the pin values in the plot and elsewhere.
		/// </summary>
		/// <value>The color of the pin.</value>
		public Gdk.Color PlotColor { get; set; }

		/// <summary>
		/// The state.
		/// </summary>
		public PrototypeBackend.DPinState State = PrototypeBackend.DPinState.LOW;

		//Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="PrototypeBackend.DPin"/> class.
		/// </summary>
		public DPin ()
		{
			Type = PrototypeBackend.PinType.DIGITAL;
			Mode = PrototypeBackend.PinMode.OUTPUT;
			Name = "";
			Number = 0;
			AnalogNumber = -1;
			PlotColor = Gdk.Color.Zero;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PrototypeBackend.DPin"/> class.
		/// </summary>
		/// <param name="name">Name.</param>
		/// <param name="pinnr">Pinnr.</param>
		public DPin (string name, uint pinnr)
		{
			Name = name;
			Number = pinnr;
			Type = PrototypeBackend.PinType.DIGITAL;
			Mode = PrototypeBackend.PinMode.OUTPUT;
			AnalogNumber = -1;
			PlotColor = Gdk.Color.Zero;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PrototypeBackend.DPin"/> class.
		/// </summary>
		/// <param name="copy">Copy.</param>
		public DPin (DPin copy) : base ()
		{
			Name = copy.Name;
			Number = copy.Number;
			AnalogNumber = copy.AnalogNumber;
			PlotColor = copy.PlotColor;
		}

		/// <summary>
		/// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="PrototypeBackend.DPin"/>.
		/// </summary>
		/// <param name="obj">The <see cref="System.Object"/> to compare with the current <see cref="PrototypeBackend.DPin"/>.</param>
		/// <returns><c>true</c> if the specified <see cref="System.Object"/> is equal to the current
		/// <see cref="PrototypeBackend.DPin"/>; otherwise, <c>false</c>.</returns>
		public override bool Equals (object obj)
		{
			var seq = obj as DPin;
			if (seq != null)
			{
				return (seq.Number == Number)
				&& seq.Name.Equals (Name)
				&& seq.State.Equals (State)
				&& seq.PlotColor.Equals (PlotColor)
				&& seq.Type.Equals (Type)
				&& seq.Mode.Equals (Mode);
			}
			return false;
		}

		/// <summary>
		/// Serves as a hash function for a <see cref="PrototypeBackend.DPin"/> object.
		/// </summary>
		/// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a hash table.</returns>
		public override int GetHashCode ()
		{
			return base.GetHashCode ();
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="PrototypeBackend.DPin"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="PrototypeBackend.DPin"/>.</returns>
		public override string ToString ()
		{
			return string.Format ("Name: {0}\tNumber: {1}\tType: {2}", Name, Number, Type);
		}

		#region ISerializable implementation

		/// <summary>
		/// Gets the object data.
		/// </summary>
		/// <param name="info">Info.</param>
		/// <param name="context">Context.</param>
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

		/// <summary>
		/// Initializes a new instance of the <see cref="PrototypeBackend.DPin"/> class.
		/// </summary>
		/// <param name="info">Info.</param>
		/// <param name="context">Context.</param>
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

		/// <summary>
		/// Uints to byte.
		/// This method is used to parse colors form one framework to another.
		/// </summary>
		/// <returns>The to byte.</returns>
		/// <param name="val">Value.</param>
		public static byte uintToByte (uint val)
		{
			return (byte)(byte.MaxValue / 65535.0 * val);
		}
	}
}

