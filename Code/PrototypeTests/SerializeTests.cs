using System;
using System.Linq;
using System.IO;
using System.Runtime;
using System.Runtime.Serialization;
using PrototypeBackend;
using NUnit.Framework;

namespace PrototypeTests
{
	[TestFixture]
	public class SerializeTests
	{
		public MemoryStream MemStream;
		public System.Runtime.Serialization.Formatters.Binary.BinaryFormatter Formator;

		[TestFixtureSetUp]
		public void Init ()
		{
			Formator = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter ();
		}

		[SetUp]
		public void SetUp ()
		{
			MemStream = new MemoryStream ();			
		}

		[Test]
		public void SerializeDPin ()
		{
			DPin TestPin = new DPin () {
				Number = 42,
				Name = "TestPin",
				PlotColor = GUIHelper.ColorHelper.GetRandomGdkColor (),
			};

			Formator.Serialize (MemStream, TestPin);

			MemStream.Seek (0, SeekOrigin.Begin);

			DPin TestPinClone = (DPin)Formator.Deserialize (MemStream);

			Assert.AreEqual (TestPin, TestPinClone);
		}

		[Test]
		public void SerializeAPin ()
		{
			APin TestPin = new APin () {
				Number = 42,
				Name = "TestPin",
				PlotColor = GUIHelper.ColorHelper.GetRandomGdkColor (),
			};

			Formator.Serialize (MemStream, TestPin);

			MemStream.Seek (0, SeekOrigin.Begin);

			APin TestPinClone = (APin)Formator.Deserialize (MemStream);

			Assert.AreEqual (TestPin, TestPinClone);
		}

		[Test]
		public void SerializeSequenceOperation ()
		{
			SequenceOperation SeqOp = new SequenceOperation () {
				Duration = TimeSpan.FromSeconds (42),
				State = DPinState.HIGH
			};

			Formator.Serialize (MemStream, SeqOp);

			MemStream.Seek (0, SeekOrigin.Begin);

			SequenceOperation SeqOpClone = (SequenceOperation)Formator.Deserialize (MemStream);

			Assert.AreEqual (SeqOp, SeqOpClone);
		}

		[Test]
		public void SerializeSequence ()
		{
			DPin TestPin = new DPin () {
				Number = 42,
				Name = "Testor the Destroyer"
			};

			Sequence Seq = new Sequence () {
				Pin = TestPin,
				Chain = new System.Collections.Generic.List<SequenceOperation> () {
					new SequenceOperation () {
						Duration = TimeSpan.FromSeconds (42),
						State = DPinState.HIGH
					},
					new SequenceOperation () {
						Duration = TimeSpan.FromSeconds (42),
						State = DPinState.LOW
					}
				},
				Repetitions = -1
			};

			Formator.Serialize (MemStream, Seq);
			MemStream.Seek (0, SeekOrigin.Begin);
			Sequence SeqEvilClone = (Sequence)Formator.Deserialize (MemStream);

			Console.WriteLine (Seq.ToStringLong ());
			Console.WriteLine (SeqEvilClone.ToStringLong ());

			Assert.AreEqual (Seq, SeqEvilClone);
		}

		[Test]
		public void SerializeMeasurementCombination ()
		{
			APin oPinionAtor = new APin () {
				Number = 42,
			};

			MeasurementCombination MeCom = new MeasurementCombination () {
				Pins = new System.Collections.Generic.List<APin> (){ oPinionAtor },
				OperationString = "A42"
			};

			Formator.Serialize (MemStream, MeCom);
			MemStream.Seek (0, SeekOrigin.Begin);
			MeasurementCombination MeComCopy = (MeasurementCombination)Formator.Deserialize (MemStream);

			Assert.AreEqual (MeCom, MeComCopy);
		}

		[Test]
		public void SerializeBoardConfigTest1 ()
		{
			var conf = new BoardConfiguration ();
			conf.AddPin (new APin (){ Number = 42 });

			Formator.Serialize (MemStream, conf);
			MemStream.Seek (0, SeekOrigin.Begin);

			var confClone = (BoardConfiguration)Formator.Deserialize (MemStream);

			Assert.AreEqual (conf.Pins [0], confClone.Pins [0]);

		}

		[Test]
		public void SerializeBoardConfigTest2 ()
		{
			var conf = new BoardConfiguration ();
			conf.Board.AnalogReferenceVoltageType = "INTERNAL";
			conf.Board.AnalogReferenceVoltage = 4.24;

			var pin = new APin (){ Number = 42 };

			var MeCom = new MeasurementCombination ();
			MeCom.AddPin (pin);


			conf.AddPin (pin);
			conf.AddMeasurementCombination (MeCom);


			Formator.Serialize (MemStream, conf);
			MemStream.Seek (0, SeekOrigin.Begin);

			var confClone = (BoardConfiguration)Formator.Deserialize (MemStream);

			Assert.AreEqual (conf.Pins [0], confClone.Pins [0]);
			Assert.AreEqual (conf.MeasurementCombinations [0], confClone.MeasurementCombinations [0]);
			Assert.AreEqual (4.24, conf.Board.AnalogReferenceVoltage, 0.000000001);
			Assert.AreEqual (4.24, confClone.Board.AnalogReferenceVoltage, 0.000000001);
			Assert.AreEqual ("INTERNAL", conf.Board.AnalogReferenceVoltageType);
			Assert.AreEqual ("INTERNAL", confClone.Board.AnalogReferenceVoltageType);
			Assert.AreSame (conf.Pins [0], conf.MeasurementCombinations [0].Pins [0]);
			Assert.AreSame (confClone.Pins [0], confClone.MeasurementCombinations [0].Pins [0]);

			conf.Pins [0].Name = "Dulli";
			Assert.AreEqual (conf.Pins [0].Name, conf.MeasurementCombinations [0].Pins [0].Name);
			Assert.AreEqual (19, confClone.AvailableDigitalPins.Length);
			Assert.AreEqual (6, confClone.AvailableAnalogPins.Length);

			conf.ClearPins ();
			Assert.AreEqual (0, conf.Pins.Count);
			Assert.AreEqual (0, conf.MeasurementCombinations.Count);
		}
	}
}

