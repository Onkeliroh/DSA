using NUnit.Framework;
using System.IO;
using Ini;

namespace INIFileLoaderTest
{
	[TestFixture ()]
	public class Test
	{
		public const string FilePath = "testfile.ini";

		[Test ()]
		public void ConstructorTest ()
		{
			var Loader = new IniFile (FilePath);

			Assert.AreEqual (FilePath, Loader.path);
		}

		[Test ()]
		public void WriteTest ()
		{
			var Loader = new IniFile (FilePath);

			Loader.IniWriteValue ("Section1", "WriteTest", "Test1");
			Loader.IniWriteValue ("Section2", "WriteTest", "Test2");

			Assert.AreEqual (true, File.Exists (FilePath));

			Assert.AreEqual ("Test1", Loader.IniReadValue ("Section1", "WriteTest"));
			Assert.AreEqual ("Test2", Loader.IniReadValue ("Section2", "WriteTest"));
		}
	}
}

