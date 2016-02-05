using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Linq;


namespace Backend
{
	/// <summary>
	///	The Board class. Storing inforamtions concerning boards like the Arduino UNO 
	/// </summary>
	[Serializable]
	public class Board : ISerializable
	{
		#region Member

		/// <summary>
		/// The number of analog pins.
		/// </summary>
		public uint NumberOfAnalogPins = 0;

		/// <summary>
		/// The number of digital pins.
		/// </summary>
		public uint NumberOfDigitalPins = 0;

		/// <summary>
		/// Gets or sets the hardware analog pins.
		/// </summary>
		/// <value>The hardware analog pins.</value>
		public uint[] HardwareAnalogPins { get ; set; }

		/// <summary>
		/// The addresses of SDA enabled pins.
		/// </summary>
		public uint[] SDA;

		/// <summary>
		/// The addresses of SCL enabled pins.
		/// </summary>
		public uint[] SCL;

		/// <summary>
		/// The addresses of RX enabled pins.
		/// </summary>
		public uint[] RX;

		/// <summary>
		/// The addresses of TX enabled pins.
		/// </summary>
		public uint[] TX;

		/// <summary>
		/// The analog reference options and default voltage values.
		/// </summary>
		public Dictionary<string,double> AnalogReferences = new Dictionary<string, double> ();

		/// <summary>
		/// The analog reference voltage.
		/// </summary>
		public double AnalogReferenceVoltage = 5;

		/// <summary>
		/// The type of the analog reference voltage.
		/// </summary>
		public string AnalogReferenceVoltageType = "DEFAULT";

		/// <summary>
		/// The MCU.
		/// </summary>
		public string MCU = "";
		/// <summary>
		/// The name of the board (e.g. Arduino UNO)
		/// </summary>
		public string Name = "";
		/// <summary>
		/// The image file path.
		/// </summary>
		public string ImageFilePath = "";
		/// <summary>
		/// The pin layout stores whether a pin is on the left, right or bottom of a board.
		/// </summary>
		public Dictionary<string,List<int>> PinLayout = new Dictionary<string, List<int>> ();
		/// <summary>
		/// The pin location in pixels acording to the image stored in the <paramref name="ImageFilePath"/>.
		/// </summary>
		public Dictionary<int,Point> PinLocation = new Dictionary<int,Point> ();

		/// <summary>
		/// Determines the use of a DTR controll signal for communication. (Arduino UNO = false)
		/// </summary>
		public bool UseDTR = false;

		#endregion

		#region Methods

		/// <summary>
		/// Initializes a new instance of the <see cref="PrototypeBackend.Board"/> class.
		/// </summary>
		public Board ()
		{
			AnalogReferences = new Dictionary<string,double> ();
			AnalogReferenceVoltage = 5;
			NumberOfAnalogPins = 6;
			NumberOfDigitalPins = 20;
			HardwareAnalogPins = new uint[]{ 14, 15, 16, 17, 18, 19 };
			SDA = new uint[]{ 18 };
			SCL = new uint[]{ 19 };
			RX = new uint[]{ 0 };
			TX = new uint[]{ 1 };
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PrototypeBackend.Board"/> class.
		/// </summary>
		/// <param name="numberOfAnalogPins">Number of analog pins.</param>
		/// <param name="numberOfDigitalPins">Number of digital pins.</param>
		/// <param name="hardwareAnalogPins">Hardware analog pins.</param>
		/// <param name="analogReferences">Analog references.</param>
		/// <param name="name">Name.</param>
		/// <param name="model">Model.</param>
		/// <param name="dtr">If set to <c>true</c> dtr.</param>
		public Board (uint numberOfAnalogPins, uint numberOfDigitalPins, uint[] hardwareAnalogPins = null, Dictionary<string,double> analogReferences = null, string name = "", string model = "", bool dtr = false)
		{
			this.NumberOfAnalogPins = numberOfAnalogPins;
			this.NumberOfDigitalPins = numberOfDigitalPins;
			if (analogReferences != null)
				this.AnalogReferences = analogReferences;

			if (hardwareAnalogPins != null)
			{
				if (hardwareAnalogPins.Length == numberOfAnalogPins)
				{
					HardwareAnalogPins = hardwareAnalogPins;
				}
			}

			this.MCU = model;
			this.Name = name;
			this.UseDTR = dtr;
		}

		/// <summary>
		/// Transforms the raw arduino 10Bit voltage values to a voltage values under concideration of the analog reference voltage.
		/// </summary>
		/// <returns>The voltage value.</returns>
		/// <param name="rawVal">10Bit voltage value</param>
		public double RAWToVolt (object rawVal)
		{
			return (Convert.ToDouble (rawVal) / 1023.0) * AnalogReferenceVoltage;
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="PrototypeBackend.Board"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="PrototypeBackend.Board"/>.</returns>
		public override string ToString ()
		{
			return String.Format (
				"Name: {0}\n" +
				"Model: {1}\n" +
				"Number of analog Pins: {2}\n" +
				"Number of digital Pins: {3}\n" +
				"Analog reference voltage: {4}\n" +
				"Analog pin hardware numbers: {5}\n" +
				"SDA: {6}\n" +
				"SDC: {7}",
				Name, 
				MCU, 
				NumberOfAnalogPins, 
				NumberOfDigitalPins, 
				AnalogReferenceVoltage,
				NumberOfAnalogPins,
				SDA,
				SCL
			);
		}

		#endregion

		#region ISerializable implementation

		/// <summary>
		/// Gets the object data.
		/// </summary>
		/// <param name="info">Info.</param>
		/// <param name="context">Context.</param>
		public void GetObjectData (SerializationInfo info, StreamingContext context)
		{
			info.AddValue ("NumberOfAnalogPins", NumberOfAnalogPins);
			info.AddValue ("NumberOfDigitalPins", NumberOfDigitalPins);
			info.AddValue ("HardwareAnalogPins", HardwareAnalogPins.ToList ());
			info.AddValue ("SDA", SDA.ToList ());
			info.AddValue ("SCL", SCL.ToList ());
			info.AddValue ("RX", RX.ToList ());
			info.AddValue ("TX", TX.ToList ());
			info.AddValue ("AnalogReferences", AnalogReferences);
			info.AddValue ("AnalogReferenceVoltage", AnalogReferenceVoltage);
			info.AddValue ("AnalogReferenceVoltageType", AnalogReferenceVoltageType);
			info.AddValue ("MCU", MCU);
			if (PinLayout.ContainsKey ("LEFT"))
			{
				info.AddValue ("PinLayoutLeft", PinLayout ["LEFT"]);
			} else
			{
				info.AddValue ("PinLayoutLeft", new List<int> ());
			}
			if (PinLayout.ContainsKey ("RIGHT"))
			{
				info.AddValue ("PinLayoutRight", PinLayout ["RIGHT"]);
			} else
			{
				info.AddValue ("PinLayoutRight", new List<int> ());
			}
			if (PinLayout.ContainsKey ("BOTTOM"))
			{
				info.AddValue ("PinLayoutBottom", PinLayout ["BOTTOM"]);
			} else
			{
				info.AddValue ("PinLayoutBottom", new List<int> ());
			}
			info.AddValue ("PinLocation", PinLocation);
			info.AddValue ("ImageFilePath", ImageFilePath);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PrototypeBackend.Board"/> class.
		/// </summary>
		/// <param name="info">Info.</param>
		/// <param name="context">Context.</param>
		public Board (SerializationInfo info, StreamingContext context)
		{
			NumberOfAnalogPins = info.GetUInt32 ("NumberOfAnalogPins");
			NumberOfDigitalPins = info.GetUInt32 ("NumberOfDigitalPins");
			HardwareAnalogPins = ((List<uint>)info.GetValue ("HardwareAnalogPins", new List<uint> ().GetType ())).ToArray ();
			SDA = ((List<uint>)info.GetValue ("SDA", new List<uint> ().GetType ())).ToArray ();
			SCL = ((List<uint>)info.GetValue ("SCL", new List<uint> ().GetType ())).ToArray ();
			RX = ((List<uint>)info.GetValue ("RX", new List<uint> ().GetType ())).ToArray ();
			TX = ((List<uint>)info.GetValue ("TX", new List<uint> ().GetType ())).ToArray ();
			AnalogReferences = (Dictionary<string,double>)info.GetValue ("AnalogReferences", AnalogReferences.GetType ());
			AnalogReferenceVoltage = info.GetDouble ("AnalogReferenceVoltage");
			this.AnalogReferenceVoltageType = "";
			this.AnalogReferenceVoltageType = info.GetString ("AnalogReferenceVoltageType");
			MCU = info.GetString ("MCU");
			PinLayout = new Dictionary<string, List<int>> ();
			PinLayout.Add ("LEFT", ((List<int>)info.GetValue ("PinLayoutLeft", new List<int> ().GetType ())));
			PinLayout.Add ("RIGHT", ((List<int>)info.GetValue ("PinLayoutRight", new List<int> ().GetType ())));
			PinLayout.Add ("BOTTOM", ((List<int>)info.GetValue ("PinLayoutBottom", new List<int> ().GetType ())));
			PinLocation = (Dictionary<int,Point>)info.GetValue ("PinLocation", PinLocation.GetType ());

			ImageFilePath = info.GetString ("ImageFilePath");
		}

		#endregion
	}
}