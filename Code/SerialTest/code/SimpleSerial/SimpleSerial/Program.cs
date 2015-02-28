using System;
using System.IO;
using Utility;


namespace SimpleSerial
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			if (Config ()) {
				bool exit = false;
				while (!exit) {
					while (Serial.Incomming.Count > 0)
						Console.WriteLine (DateTime.Now + "\t" + Serial.Incomming.Dequeue ());

					//				System.Threading.Thread.Sleep (100);

					if (Console.KeyAvailable)
						exit = true;
				}
			}
		}

		public static bool Config()
		{
			Console.WriteLine ("Select one of serialports to listen to. Please enter the number.");
			string[] names = Utility.Serial.GetPorts ();
			for ( int i = 0; i < names.Length; i++)
			{
				Console.WriteLine(i+".\t" + names[i]);
			}

			int selection = Convert.ToInt32(Console.ReadLine());

			return Serial.ConnectToPort ( names[selection], 9600);
		}
	}
}
