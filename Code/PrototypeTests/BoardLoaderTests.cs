using System.IO;
using PrototypeBackend;
using NUnit.Framework;

namespace PrototypeTests
{
	[TestFixture ()]
	public class BoardLoaderTests
	{
		[Test ()]
		public void BoardLoaderParser1Tests ()
		{
			const string filepath = @"BoardLoaderParser1Test.txt";
			TextWriter tw = new StreamWriter (filepath, false);
			tw.WriteLine ("uno.name=Arduino Uno");
			tw.WriteLine ("uno.numberofdigitalpins=20");
			tw.WriteLine ("uno.numberofanalogpins=6");
			tw.WriteLine ("uno.analogreferenceoption=DEFAULT 0");
			tw.WriteLine ("uno.analogreferenceoption=INTERNAL 1");
			tw.WriteLine ("uno.analogreferenceoption=EXTERNAL 3");
			tw.WriteLine ("uno.analogreferencevoltage=4.3");
			tw.Close ();

			Board[] boards = PrototypeBackend.BoardParser.parse (filepath);

			Assert.AreEqual (1, boards.Length);
			Assert.AreEqual ("Arduino Uno", boards [0].Name);
			Assert.AreEqual (20, boards [0].NumberOfDigitalPins);
			Assert.AreEqual (6, boards [0].NumberOfAnalogPins);
			Assert.AreEqual (3, boards [0].AnalogReferences.Count);
			Assert.AreEqual (true, boards [0].AnalogReferences.ContainsKey ("DEFAULT"));
			Assert.AreEqual (true, boards [0].AnalogReferences.ContainsKey ("INTERNAL"));
			Assert.AreEqual (true, boards [0].AnalogReferences.ContainsKey ("EXTERNAL"));
			Assert.AreEqual (4.3, boards [0].AnalogReferenceVoltage, 0.1);

			File.Delete (filepath);
		}

		[Test ()]
		public void BoardLoaderParser2Tests ()
		{
			const string filepath = @"BoardLoaderParser2Test.txt";
			TextWriter tw = new StreamWriter (filepath, false);
			tw.WriteLine ("uno.name=Arduino Uno");
			tw.WriteLine ("uno.numberofdigitalpins=20");
			tw.WriteLine ("uno.numberofanalogpins=6");
			tw.WriteLine ("uno.analogreferenceoption=DEFAULT 0");
			tw.WriteLine ("uno.analogreferenceoption=INTERNAL 1");
			tw.WriteLine ("uno.analogreferenceoption=EXTERNAL 3");
			tw.WriteLine ("#------------------------------");
			tw.WriteLine ("leonardo.name=Arduino Leonardo");
			tw.WriteLine ("leonardo.numberofdigitalpins=20");
			tw.WriteLine ("leonardo.numberofanalogpins=12");
			tw.WriteLine ("leonardo.analogreferenceoption=DEFAULT 0");
			tw.WriteLine ("leonardo.analogreferenceoption=INTERNAL 1");
			tw.WriteLine ("leonardo.analogreferenceoption=EXTERNAL 3");
			tw.Close ();

			Board[] boards = PrototypeBackend.BoardParser.parse (filepath);

			Assert.AreEqual (2, boards.Length);
			Assert.AreEqual ("Arduino Uno", boards [0].Name);
			Assert.AreEqual (20, boards [0].NumberOfDigitalPins);
			Assert.AreEqual (6, boards [0].NumberOfAnalogPins);
			Assert.AreEqual (3, boards [0].AnalogReferences.Count);
			Assert.AreEqual (true, boards [0].AnalogReferences.ContainsKey ("DEFAULT"));
			Assert.AreEqual (true, boards [0].AnalogReferences.ContainsKey ("INTERNAL"));
			Assert.AreEqual (true, boards [0].AnalogReferences.ContainsKey ("EXTERNAL"));
			Assert.AreEqual ("Arduino Leonardo", boards [1].Name);
			Assert.AreEqual (20, boards [1].NumberOfDigitalPins);
			Assert.AreEqual (12, boards [1].NumberOfAnalogPins);
			Assert.AreEqual (3, boards [1].AnalogReferences.Count);
			Assert.AreEqual (true, boards [1].AnalogReferences.ContainsKey ("DEFAULT"));
			Assert.AreEqual (true, boards [1].AnalogReferences.ContainsKey ("INTERNAL"));
			Assert.AreEqual (true, boards [1].AnalogReferences.ContainsKey ("EXTERNAL"));

			File.Delete (filepath);
		}
	}
}

