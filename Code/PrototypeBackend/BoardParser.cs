using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PrototypeBackend;

namespace PrototypeBackend
{
	public static class BoardParser
	{
		private const char Comment = '#';
		private const char BoardSeparator = '-';

		public static Board[] parse (string path)
		{
			Board[] Boards = new Board[0];
			try
			{
				if (File.Exists (path))
				{
					TextReader tr = new StreamReader (path);
					Boards = getBoards (tr);

					tr.Close ();
				} else
				{
					throw new FileNotFoundException ();
				}
			} catch (Exception e)
			{
				Console.Error.WriteLine (e);
			}
			return Boards;
		}

		private static Board[] getBoards (TextReader tr)
		{
			Dictionary<string,Board> Boards = new Dictionary<string,Board> ();
			string line;
			while ((line = tr.ReadLine ()) != null)
			{
				if (line [0].Equals (BoardSeparator))
				{
					continue;
				} else if (line.Equals (string.Empty))
				{
					continue;
				} else if (line [0].Equals (Comment))
				{
					continue;
				} else
				{
					string[] args = line.Split (new char[]{ '.' }, 2);
					string key = args [0];
					if (!Boards.ContainsKey (args [0]))
					{
						Boards.Add (key, new Board ());
					}
					args = args [1].Split ('=');
					if (args [0].Equals ("name"))
					{
						Boards [key].Name = args [1];
					} else if (args [0].Equals ("numberofdigitalpins"))
					{
						Boards [key].NumberOfDigitalPins = Convert.ToUInt16 (args [1]);
					} else if (args [0].Equals ("numberofanalogpins"))
					{
						Boards [key].NumberOfAnalogPins = Convert.ToUInt16 (args [1]);
					} else if (args [0].Equals ("analogreferenceoption"))
					{
						string[] references = args [1].Split (' ');
						Boards [key].AnalogReferences.Add (references [0], Convert.ToInt16 (references [1]));
					} else if (args [0].Equals ("mcu"))
					{
						Boards [key].Model = args [1];
					} else if (args [0].Equals ("analogreferencevoltage"))
					{
						if (args.Length > 2)
						{
							Boards [key].AnalogReferenceVoltage = Convert.ToDouble (args [1] + '.' + args [2]);
						} else
						{
							Boards [key].AnalogReferenceVoltage = Convert.ToDouble (args [1]);
						}
					}
				}
			}
			return Boards.Values.ToArray<Board> ();
		}
	}
}

