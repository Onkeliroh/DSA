using System;
using System.IO;
using IniParser;
using IniParser.Model;
using System.Collections.Generic;

namespace PrototypeBackend
{
	public class ConfigurationManager
	{
		public IniParser.Model.IniData GeneralData = new IniParser.Model.IniData ();

		public string UserFolder = "";

		public ConfigurationManager (string UserFolderPath = null)
		{
			if (UserFolderPath == null)
			{
					
				//Linux|Mac
				if (Environment.OSVersion.Platform == PlatformID.Unix)
				{
					UserFolder = Environment.GetFolderPath (Environment.SpecialFolder.UserProfile);
					UserFolder += @"/.config/micrologger/Config.ini";
				}
				//Windows
				else
				{
					UserFolder = Environment.GetFolderPath (Environment.SpecialFolder.ApplicationData);
					UserFolder += @"\micrologger\Config.ini";
					Console.WriteLine (UserFolder);
				}
			} else
			{
				UserFolder = UserFolderPath;
			}

			if (File.Exists (UserFolder))
			{
				GeneralData = ParseSettings (UserFolder);
			} else
			{
				throw new FileNotFoundException ();
			}
		}

		public void SaveGeneralSettings ()
		{
			if (UserFolder != null)
			{
				var Parser = new FileIniDataParser ();
				Parser.WriteFile (UserFolder, GeneralData, System.Text.Encoding.UTF8);
			}
		}

		public IniData ParseSettings (string Path)
		{
			if (File.Exists (Path))
			{
				var Parser = new FileIniDataParser ();
				return Parser.ReadFile (Path);
			}
			return null;
		}

		public Board[] ParseBoards (string Path)
		{
			if (Path != null && !Path.Equals (string.Empty))
			{
				if (File.Exists (Path))
				{
					var Parser = new IniParser.FileIniDataParser ();
					IniData Data = Parser.ReadFile (Path);
					var Boards = new System.Collections.Generic.List<Board> ();
					foreach (SectionData sd in Data.Sections)
					{
						try
						{
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
								PinLayout = StringToLayout (sd.Keys.GetKeyData ("PinLeft").Value, sd.Keys.GetKeyData ("PinRight").Value, sd.Keys.GetKeyData ("PinBottom").Value)
							});
						} catch (Exception ex)
						{
							Console.WriteLine (ex);
						}

					}
					return Boards.ToArray ();
				}
			}
			return new Board[]{ };
		}

		private uint[] StringToArray (string str)
		{
			var stra = str.Split (new char[]{ ',', ';', '|' }, StringSplitOptions.RemoveEmptyEntries);
			var ints = new System.Collections.Generic.List<uint> ();
			foreach (string s in stra)
			{
				uint i;
				uint.TryParse (s, out i);
				ints.Add (i);
			}
			return ints.ToArray ();
		}

		private Dictionary<string,double> StringToARefDict (string str)
		{
			var stra = str.Split (new char[]{ ',', ';', '|' }, StringSplitOptions.RemoveEmptyEntries);
			var res = new Dictionary<string,double> ();

			foreach (string s in stra)
			{
				var pair = s.Split (new char[]{ ' ' }, StringSplitOptions.RemoveEmptyEntries);
				res.Add (pair [0], Convert.ToDouble (pair [1]));
			}
			return res;
		}

		private Dictionary<string,List<int>> StringToLayout (string left, string right, string bottom)
		{
			var dict = new Dictionary<string,List<int>> ();
			dict.Add ("LEFT", StringToPin (left));
			dict.Add ("RIGHT", StringToPin (right));
			dict.Add ("BOTTOM", StringToPin (bottom));

			return dict;
		}

		private List<int> StringToPin (string str)
		{
			var pairs = str.Split (new char[]{ ',' }, StringSplitOptions.RemoveEmptyEntries);
			var dict = new List<int> ();

			foreach (string s in pairs)
			{
				dict.Add (Convert.ToInt32 (s));
			}
			return dict;
		}
	}
}

