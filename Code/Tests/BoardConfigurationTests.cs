using System;
using NUnit.Framework;
using Backend;
using System.Collections.Generic;

namespace PrototypeTests
{
	[TestFixture]
	public class BoardConfigurationTests
	{
		BoardConfiguration conf;

		public void Setup ()
		{
			conf = new BoardConfiguration ();

			for (int i = 0; i < 5; i++)
			{
				conf.AddPin (new APin (){ Number = (uint)i });
				conf.AddPin (new DPin (){ Number = (uint)i });
			}
		}

		public void SetupSequence ()
		{
			conf = new BoardConfiguration ();
			conf.AddPin (new DPin (){ Number = 0 });
			conf.AddPin (new DPin (){ Number = 1 });

			conf.AddSequence (new Sequence () { Pin = conf.DigitalPins [0],
				Chain = new List<SequenceOperation> () {
					new SequenceOperation () {
						Duration = TimeSpan.FromSeconds (1),
						State = DPinState.HIGH
					},
					new SequenceOperation () {
						Duration = TimeSpan.FromSeconds (1),
						State = DPinState.HIGH
					}
				}
			});
		}

		public void SetupMeasurementCombination ()
		{
			conf = new BoardConfiguration ();
			conf.AddPin (new APin (){ Number = 0 });
			conf.AddPin (new APin (){ Number = 1 });

			conf.AddMeasurementCombination (new MeasurementCombination () {
				Pins = new List<APin> (){ conf.AnalogPins [0], conf.AnalogPins [1] },
			});
		}

		[Test]
		public void APinCloneTest ()
		{
			Setup ();

			Assert.AreEqual (5, conf.AnalogPins.Count);

			conf.ClonePin (conf.AnalogPins [3]);
			Assert.AreEqual (6, conf.AnalogPins.Count);
			Assert.AreEqual (5, conf.AnalogPins [5].Number);
		}

		[Test]
		public void DPinCloneTest ()
		{
			Setup ();
			Assert.AreEqual (5, conf.DigitalPins.Count);

			conf.ClonePin (conf.DigitalPins [3]);
			Assert.AreEqual (6, conf.DigitalPins.Count);
			Assert.AreEqual (5, conf.DigitalPins [5].Number);
		}

		[Test]
		public void APinCloneTest2 ()
		{
			string name = "TestPin";

			conf = new BoardConfiguration ();
			conf.AddPin (new APin () {
				Number = 5,
				Name = name
			});


			conf.ClonePin (conf.AnalogPins [0]);

			Assert.AreEqual (2, conf.AnalogPins.Count);
			Assert.AreEqual (name, conf.AnalogPins [0].Name);
			Assert.AreEqual (name, conf.AnalogPins [1].Name);

			conf.AnalogPins [0].Name = string.Empty;
			Assert.AreEqual (name, conf.AnalogPins [1].Name);
		}

		[Test]
		public void DPinCloneTest2 ()
		{
			string name = "TestPin";

			conf = new BoardConfiguration ();
			conf.AddPin (new DPin () {
				Number = 5,
				Name = name
			});


			conf.ClonePin (conf.DigitalPins [0]);

			Assert.AreEqual (2, conf.DigitalPins.Count);
			Assert.AreEqual (name, conf.DigitalPins [0].Name);
			Assert.AreEqual (name, conf.DigitalPins [1].Name);

			conf.DigitalPins [0].Name = string.Empty;
			Assert.AreEqual (name, conf.DigitalPins [1].Name);
		}

		[Test]
		public void SequenceCloneTest ()
		{
			SetupSequence ();

			Assert.AreEqual (1, conf.Sequences.Count);
			conf.CloneSequence (conf.Sequences [0]);
			Assert.AreEqual (2, conf.Sequences.Count);

			Assert.AreEqual (conf.DigitalPins [1], conf.Sequences [1].Pin);
			Assert.AreEqual (conf.Sequences [0].Chain, conf.Sequences [1].Chain);
			Assert.AreNotSame (conf.Sequences [0], conf.Sequences [1]);

			conf.DigitalPins [0].Name = "DickButt";
			Assert.AreNotEqual (conf.Sequences [1].Pin.Name, conf.DigitalPins [0].Name);
		}

		[Test]
		public void MeasurementCombinationCloneTest ()
		{
			SetupMeasurementCombination ();

			Assert.AreEqual (1, conf.MeasurementCombinations.Count);
			conf.CloneMeasurementCombination (conf.MeasurementCombinations [0]);
			Assert.AreEqual (2, conf.MeasurementCombinations.Count);

			Assert.AreEqual (0, conf.MeasurementCombinations [1].Pins [0].Number);
			Assert.AreEqual (1, conf.MeasurementCombinations [1].Pins [1].Number);
		}
	}
}

