using System;
using System.IO;
using IniParser;
using IniParser.Model;
using System.Collections.Generic;

namespace PrototypeBackend
{
	/// <summary>
	/// Manages the ini-configuration files, reads and writes informations.
	/// </summary>
	public class ConfigurationManager
	{
		/// <summary>
		/// The general data extracted form the main config file.
		/// </summary>
		public IniParser.Model.IniData GeneralData = new IniParser.Model.IniData ();

		/// <summary>
		/// Systemspecific path to the user directory.
		/// </summary>
		public string UserFolder = "";

		/// <summary>
		/// Initializes a new instance of the <see cref="PrototypeBackend.ConfigurationManager"/> class.
		/// </summary>
		/// <param name="UserFolderPath">User folder path.</param>
		public ConfigurationManager (string UserFolderPath = null)
		{
			if (UserFolderPath == null) {
					
				//Linux|Mac
				if (Environment.OSVersion.Platform == PlatformID.Unix) {
					UserFolder = Environment.GetFolderPath (Environment.SpecialFolder.UserProfile);
					UserFolder += @"/.config/micrologger/Config.ini";
				}
				//Windows
				else {
					UserFolder = Environment.GetFolderPath (Environment.SpecialFolder.ApplicationData);
					UserFolder += @"\micrologger\Config.ini";
					Console.WriteLine (UserFolder);
				}
			} else {
				UserFolder = UserFolderPath;
			}

			if (File.Exists (UserFolder)) {
				GeneralData = ParseSettings (UserFolder);
			} else {
				throw new FileNotFoundException ();
			}
		}

		/// <summary>
		/// Saves the general settings.
		/// </summary>
		public void SaveGeneralSettings ()
		{
			if (UserFolder != null) {
				var Parser = new FileIniDataParser ();
				Parser.WriteFile (UserFolder, GeneralData, System.Text.Encoding.UTF8);
			}
		}

		/// <summary>
		/// Parses the settings.
		/// </summary>
		/// <returns>The settings.</returns>
		/// <param name="Path">Path.</param>
		public IniData ParseSettings (string Path)
		{
			if (File.Exists (Path)) {
				var Parser = new FileIniDataParser ();
				return Parser.ReadFile (Path);
			}
			return null;
		}

		/// <summary>
		/// Parses the boards.
		/// </summary>
		/// <returns>The boards.</returns>
		/// <param name="Path">Path.</param>
		public static Board[] ParseBoards (StreamReader Path)
		{

			var Parser = new IniParser.FileIniDataParser ();
			IniData Data = Parser.ReadData (Path);
			var Boards = new System.Collections.Generic.List<Board> ();
			foreach (SectionData sd in Data.Sections) {
				try {
					Boards.Add (new Board () {
						Name = sd.Keys.GetKeyData ("Name").Value,
						NumberOfAnalogPins = Convert.ToUInt32 (sd.Keys.GetKeyData ("NumberOfAnalogPins").Value),
						NumberOfDigitalPins = Convert.ToUInt32 (sd.Keys.GetKeyData ("NumberOfDigitalPins").Value),
						MCU = sd.Keys.GetKeyData ("MCU").Value,
						ImageFilePath = sd.Keys.GetKeyData ("ImagePath").Value,
						SDA = StringToArray (sd.Keys.GetKeyData ("SDA").Value),
						SCL = StringToArray (sd.Keys.GetKeyData ("SCL").Value),
						RX = StringToArray (sd.Keys.GetKeyData ("RX").Value),
						TX = StringToArray (sd.Keys.GetKeyData ("TX").Value),
						UseDTR = Convert.ToBoolean (sd.Keys.GetKeyData ("DTR").Value),
						HardwareAnalogPins = StringToArray (sd.Keys.GetKeyData ("HWAPinsAddrs").Value),
						AnalogReferences = StringToARefDict (sd.Keys.GetKeyData ("AREF").Value),
						PinLayout = StringToLayout (sd.Keys.GetKeyData ("PinLeft").Value, sd.Keys.GetKeyData ("PinRight").Value, sd.Keys.GetKeyData ("PinBottom").Value),
						PinLocation = StringToPinPlacement (sd.Keys.GetKeyData ("PinPosition").Value)
					});
				} catch (Exception ex) {
					Console.WriteLine (ex);
				}

			}
			return Boards.ToArray ();
		}

		/// <summary>
		/// Strings to array.
		/// </summary>
		/// <returns>The to array.</returns>
		/// <param name="str">String.</param>
		private static uint[] StringToArray (string str)
		{
			var stra = str.Split (new char[]{ ',', ';', '|' }, StringSplitOptions.RemoveEmptyEntries);
			var ints = new System.Collections.Generic.List<uint> ();
			foreach (string s in stra) {
				uint i;
				uint.TryParse (s, out i);
				ints.Add (i);
			}
			return ints.ToArray ();
		}

		/// <summary>
		/// Strings to analog reference dictionary.
		/// </summary>
		/// <returns>The analog reference dictionary.</returns>
		/// <param name="str">String.</param>
		private static Dictionary<string,double> StringToARefDict (string str)
		{
			var stra = str.Split (new char[]{ ',', ';', '|' }, StringSplitOptions.RemoveEmptyEntries);
			var res = new Dictionary<string,double> ();

			foreach (string s in stra) {
				var pair = s.Split (new char[]{ ' ' }, StringSplitOptions.RemoveEmptyEntries);
				res.Add (pair [0], Convert.ToDouble (pair [1]));
			}
			return res;
		}

		/// <summary>
		/// Strings to layout.
		/// </summary>
		/// <returns>The to layout.</returns>
		/// <param name="left">Left.</param>
		/// <param name="right">Right.</param>
		/// <param name="bottom">Bottom.</param>
		private static Dictionary<string,List<int>> StringToLayout (string left, string right, string bottom)
		{
			var dict = new Dictionary<string,List<int>> ();
			dict.Add ("LEFT", StringToPin (left));
			dict.Add ("RIGHT", StringToPin (right));
			dict.Add ("BOTTOM", StringToPin (bottom));

			return dict;
		}

		/// <summary>
		/// Strings to pin.
		/// </summary>
		/// <returns>The to pin.</returns>
		/// <param name="str">String.</param>
		private static List<int> StringToPin (string str)
		{
			var pairs = str.Split (new char[]{ ',' }, StringSplitOptions.RemoveEmptyEntries);
			var dict = new List<int> ();

			foreach (string s in pairs) {
				dict.Add (Convert.ToInt32 (s));
			}
			return dict;
		}

		/// <summary>
		/// Strings to pin placement.
		/// </summary>
		/// <returns>The to pin placement.</returns>
		/// <param name="str">String.</param>
		private static Dictionary<int,Point> StringToPinPlacement (string str)
		{
			var pins = str.Split (new char[]{ ';' }, StringSplitOptions.RemoveEmptyEntries);
			var dict = new Dictionary<int,Point> ();

			foreach (string s in pins) {
				var pair = s.Split (new char[]{ ':', ',' }, StringSplitOptions.RemoveEmptyEntries);

				Point p = new Point (Convert.ToDouble (pair [1]), Convert.ToDouble (pair [2]));
				dict.Add (Convert.ToInt32 (pair [0]), p);
			}
			return dict;
		}
	}


	/// <summary>
	/// A simple class to store x and y coordinates.
	/// </summary>
	[Serializable]
	public struct Point
	{
		/// <summary>
		/// The x.
		/// </summary>
		public double x;
		/// <summary>
		/// The y.
		/// </summary>
		public double y;

		/// <summary>
		/// Initializes a new instance of the <see cref="PrototypeBackend.Point"/> struct.
		/// </summary>
		/// <param name="X">X.</param>
		/// <param name="Y">Y.</param>
		public Point (double X, double Y)
		{
			x = X;
			y = Y;
		}
	}
}

