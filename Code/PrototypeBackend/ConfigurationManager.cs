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
			if (UserFolderPath == null)
			{
					
				//Linux|Mac
				if (Environment.OSVersion.Platform == PlatformID.Unix)
				{
					UserFolder = Environment.GetFolderPath (Environment.SpecialFolder.UserProfile);
					UserFolder += @"/.config/.micrologger";
				}
				//Windows
				else
				{
					UserFolder = Environment.GetFolderPath (Environment.SpecialFolder.ApplicationData);
					UserFolder += @"\micrologger";
				}
			} else
			{
				UserFolder = UserFolderPath;
			}

			GeneralData = ParseSettings (UserFolder);
		}

		public void SaveGeneralSettings ()
		{
			if (UserFolder != null)
			{
				var Parser = new IniParser.FileIniDataParser ();
				Parser.WriteFile (UserFolder, GeneralData, System.Text.Encoding.UTF8);
			}
		}

		public IniData ParseSettings (string Path)
		{
			if (File.Exists (Path))
			{
				var Parser = new IniParser.FileIniDataParser ();
				return Parser.ReadFile (Path);
			}
			return null;
		}

		public Board[] ParseBoards (string Path, ref string[] ImgPaths)
		{
			if (File.Exists (Path))
			{
				var Parser = new IniParser.FileIniDataParser ();
				IniData Data = Parser.ReadFile (Path);
				var Boards = new System.Collections.Generic.List<Board> ();
				var Imgs = new System.Collections.Generic.List<string> ();
				foreach (SectionData sd in Data.Sections)
				{
					Boards.Add (new Board () {
						Name = sd.Keys.GetKeyData ("Name").Value,
						NumberOfAnalogPins = Convert.ToUInt32 (sd.Keys.GetKeyData ("NumberOfAnalogPins").Value),
						NumberOfDigitalPins = Convert.ToUInt32 (sd.Keys.GetKeyData ("NumberOfDigitalPins").Value),
						MCU = sd.Keys.GetKeyData ("MCU").Value
					});
					Imgs.Add (sd.Keys ["ImagePath"]);

				}
				ImgPaths = Imgs.ToArray ();
				return Boards.ToArray ();
			}
			return null;
		}
	}
}

