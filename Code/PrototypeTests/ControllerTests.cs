using NUnit.Framework;
using System;
using System.Linq;
using PrototypeBackend;
using System.Security.Cryptography;

namespace PrototypeTests
{
	[TestFixture ()]
	public class ControllerTests
	{
		[Test ()]
		public void GetUsedPinsTest ()
		{
			var tmp = new Controller ();

			var pins = tmp.Configuration.AnalogPins;

			Assert.AreEqual (pins.Count, 0);

			tmp.Configuration.AddPin (new APin () {
				Number = 0,
			});

			pins = tmp.Configuration.AnalogPins;
			Assert.AreEqual (1, pins.Count);
			Assert.AreEqual (0, pins [0].Number);

			tmp.Configuration.AddPin (new DPin () {
				Number = 42
			});

			tmp.Configuration.AddPin (new DPin () {
				Number = 13
			});

			pins = tmp.Configuration.AnalogPins;
			Assert.AreEqual (1, pins.Count);
			Assert.AreEqual (0, pins [0].Number);

			var dpins = tmp.Configuration.DigitalPins;
			Assert.AreEqual (2, dpins.Count);
			Assert.AreEqual (13, dpins [0].Number);
			Assert.AreEqual (42, dpins [1].Number);
		}

		[Test ()]
		public void GetUnusedPinsTest ()
		{
			var tmp = new Controller ();

			var pins = tmp.Configuration.AvailableAnalogPins;
			Assert.AreEqual (pins.Count (), 6);

			tmp.Configuration.AddPin (new APin () {
				Number = 0,
			});

			pins = tmp.Configuration.AvailableAnalogPins;
			Assert.AreEqual (5, pins.Count ());
			Assert.AreEqual (1, pins [0].Number);

			tmp.Configuration.AddPin (
				new DPin () {
					Name = "Ding of Awesome",
					Number = 13
				}
			);

			tmp.Configuration.AddPin (
				new DPin () {
					Name = "Pin of Doom",
					Number = 12
				}
			);

			pins = tmp.Configuration.GetPinsWithoutCombinations ();
			Assert.AreEqual (1, pins.Count ());
			Assert.AreEqual (pins [0].Number, 0);

			var dpins = tmp.Configuration.GetPinsWithoutSequence ();
			Assert.AreEqual (2, dpins.Count ());
			Assert.AreEqual (12, dpins [0].Number);
			Assert.AreEqual (13, dpins [1].Number);
		}

		[Test]
		public void GetUnusedPinsTest2 ()
		{
			var con = new Controller ();
			con.Configuration.AddPin (new DPin (){ Name = "Pin1", Number = 0 });

			Assert.AreEqual (19, con.Configuration.AvailableDigitalPins.Length);
			Assert.AreEqual (1, con.Configuration.AvailableDigitalPins [0].Number);

		}

		[Test]
		public void CSVMappingTest ()
		{
			var con = new BoardConfiguration ();
			con.AddPin (new APin (){ Number = 1, Name = "Pin1" });
			con.AddPin (new APin (){ Number = 2, Name = "Pin2" });
			con.AddPin (new APin (){ Number = 3, Name = "Pin3" });

			var res = con.CreateMapping ();

			Assert.AreEqual (3, res.Keys.Count);
			Assert.AreEqual (0, res [con.Pins [0].DisplayName]);
			Assert.AreEqual (1, res [con.Pins [1].DisplayName]);
			Assert.AreEqual (2, res [con.Pins [2].DisplayName]);

			con.AddMeasurementCombination (new MeasurementCombination () {
				Pins = new System.Collections.Generic.List<APin> (){ con.Pins [0] as APin, con.Pins [1] as APin },
				Name = "MeCom"
			});

			res = con.CreateMapping ();

			Assert.AreEqual (4, res.Keys.Count);
			Assert.AreEqual (3, res [con.MeasurementCombinations [0].DisplayName]);
		}
	}
}

