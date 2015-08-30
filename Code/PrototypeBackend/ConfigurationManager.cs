using System;
using System.IO;
using IniParser;
using IniParser.Model;

namespace PrototypeBackend
{
	public class ConfigurationManager
	{
		public IniParser.Model.IniData GeneralData = new IniParser.Model.IniData ();

		public string UserFolder { get; set; }

		public ConfigurationManager (string UserFolderPath = null)
		{
			if (UserFolderPath == null) {
					
				//Linux|Mac
				if (Environment.OSVersion.Platform == PlatformID.Unix) {
					UserFolder = Environment.GetFolderPath (Environment.SpecialFolder.UserProfile);
					UserFolder += @"/.config/.micrologger";
				}
				//Windows
				else {
					UserFolder = Environment.GetFolderPath (Environment.SpecialFolder.ApplicationData);
					UserFolder += @"\micrologger";
				}
			} else {
				UserFolder = UserFolderPath;
			}

			GeneralData = ParseSettings (UserFolder);
		}

		public void SaveGeneralSettings ()
		{
			if (UserFolder != null) {
				var Parser = new FileIniDataParser ();
				Parser.WriteFile (UserFolder, GeneralData, System.Text.Encoding.UTF8);
			}
		}

		public IniData ParseSettings (string Path)
		{
			if (File.Exists (Path)) {
				var Parser = new FileIniDataParser ();
				return Parser.ReadFile (Path);
			}
			return null;
		}

		public Board[] ParseBoards (string Path)
		{
			if (File.Exists (Path)) {
				var Parser = new IniParser.FileIniDataParser ();
				IniData Data = Parser.ReadFile (Path);
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
							HardwareAnalogPins = StringToArray (sd.Keys.GetKeyData ("HWAPinsAddrs").Value)
						});
					} catch (Exception ex) {
						Console.WriteLine (ex);
					}

				}
				return Boards.ToArray ();
			}
			return null;
		}

		private uint[] StringToArray (string str)
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
	}
}

