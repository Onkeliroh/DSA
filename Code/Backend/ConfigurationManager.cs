using System;
using System.IO;
using IniParser;
using IniParser.Model;
using System.Collections.Generic;

namespace Backend
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
						SDA = ConfigHelper.StringToArray (sd.Keys.GetKeyData ("SDA").Value),
						SCL = ConfigHelper.StringToArray (sd.Keys.GetKeyData ("SCL").Value),
						RX = ConfigHelper.StringToArray (sd.Keys.GetKeyData ("RX").Value),
						TX = ConfigHelper.StringToArray (sd.Keys.GetKeyData ("TX").Value),
						UseDTR = Convert.ToBoolean (sd.Keys.GetKeyData ("DTR").Value),
						HardwareAnalogPins = ConfigHelper.StringToArray (sd.Keys.GetKeyData ("HWAPinsAddrs").Value),
						AnalogReferences = ConfigHelper.StringToARefDict (sd.Keys.GetKeyData ("AREF").Value),
						PinLayout = ConfigHelper.StringToLayout (sd.Keys.GetKeyData ("PinLeft").Value, sd.Keys.GetKeyData ("PinRight").Value, sd.Keys.GetKeyData ("PinBottom").Value),
						PinLocation = ConfigHelper.StringToPinPlacement (sd.Keys.GetKeyData ("PinPosition").Value)
					});
				} catch (Exception ex) {
					Console.WriteLine (ex);
				}

			}
			return Boards.ToArray ();
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

