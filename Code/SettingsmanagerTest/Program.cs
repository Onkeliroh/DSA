using System;
using System.Configuration;

namespace SettingsmanagerTest
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Console.WriteLine (ConfigurationManager.AppSettings.Count);
			Console.WriteLine (ConfigurationManager.AppSettings ["Testkey"]);

			Console.WriteLine ((ConfigurationManager.GetSection ("sampleSection") as System.Configuration.NameValueConfigurationCollection).Count);

//			ConfigurationManager.AppSettings.Set ("Testkeyy", "fünf");
		}
	}
}
