using System;
using NUnit.Framework;
using PrototypeBackend;

namespace PrototypeTests
{
	[TestFixture]
	public class SequenceTests
	{
		[Test]
		public void ConstructorTest ()
		{
			Sequence seq = new Sequence ();

			Assert.AreEqual (new DPin (), seq.Pin);
			Assert.AreEqual ("", seq.Name);
			Assert.AreEqual (seq.Pin.PlotColor, seq.Color);
			Assert.AreEqual (0, seq.Chain.Count);
			Assert.AreEqual (0, seq.Repetitions);
			Assert.AreEqual (0, seq.Cycle);
		}

		[Test]
		public void SetTest ()
		{
			Sequence seq = new Sequence () {
				Pin = new DPin () {
					Name = "Pin of Awesome",
					Number = 13,
					PlotColor = new Gdk.Color (0, 255, 255),
					State = DPinState.HIGH
				},
				Name = "Sequence of Awesome",
				Repetitions = 3,
			};

			Assert.AreNotEqual (null, seq.Pin);
			Assert.AreEqual ("Sequence of Awesome", seq.Name);
			Assert.AreEqual (0, seq.Chain.Count);
			Assert.AreEqual (3, seq.Repetitions);
			Assert.AreEqual (0, seq.Cycle);
		}

		[Test]
		public void AddTest ()
		{
			Sequence seq = new Sequence () {
				Pin = new DPin () {
					Name = "Pin of Awesome",
					Number = 13,
					PlotColor = GUIHelper.ColorHelper.SystemColorToGdkColor (System.Drawing.Color.Yellow),
					State = DPinState.HIGH
				},
				Name = "Sequence of Awesome",
				Repetitions = 3,
			};

			seq.AddSequenceOperation (new SequenceOperation () {
				State = DPinState.HIGH,
				Duration = TimeSpan.FromSeconds (30)
			});

			Assert.AreEqual (1, seq.Chain.Count);

			Assert.AreEqual (0, seq.Cycle);
			seq.Next ();
			Assert.AreEqual (1, seq.Cycle);
			seq.Next ();
			Assert.AreEqual (2, seq.Cycle);
			seq.Next ();
			Assert.AreEqual (3, seq.Cycle);

			seq.Reset ();
			Assert.AreEqual (0, seq.Cycle);
		}

		[Test]
		public void CurrentTest ()
		{
			Sequence seq = new Sequence () {
				Pin = new DPin () {
					Name = "Pin of Awesome",
					Number = 13,
					PlotColor = GUIHelper.ColorHelper.SystemColorToGdkColor (System.Drawing.Color.Yellow),
					State = DPinState.HIGH
				},
				Name = "Sequence of Awesome",
				Repetitions = 3,
			};

			seq.AddSequenceOperation (new SequenceOperation () {
				State = DPinState.HIGH,
				Duration = TimeSpan.FromSeconds (30)
			});
			seq.AddSequenceOperation (new SequenceOperation () {
				State = DPinState.LOW,
				Duration = TimeSpan.FromSeconds (30)
			});

			Assert.AreEqual (2, seq.Chain.Count);

			Assert.AreEqual (0, seq.Cycle);
			Assert.AreEqual (DPinState.HIGH, ((SequenceOperation)seq.Current ()).State);
			Assert.AreEqual (0, seq.Cycle);
			Assert.AreEqual (DPinState.LOW, ((SequenceOperation)seq.Next ()).State);
			Assert.AreEqual (0, seq.Cycle);
			Assert.AreEqual (DPinState.HIGH, ((SequenceOperation)seq.Next ()).State);
			Assert.AreEqual (1, seq.Cycle);
			Assert.AreEqual (DPinState.LOW, ((SequenceOperation)seq.Next ()).State);
			Assert.AreEqual (1, seq.Cycle);

			seq.Reset ();
			Assert.AreEqual (0, seq.Cycle);
		}

		[Test]
		public void GetCurrentStateTest ()
		{
					
		}

		[Test]
		public void GroupTest ()
		{
			var conf = new BoardConfiguration ();

			for (uint i = 0; i < 10; i++)
			{
				conf.AddPin (new DPin () {
					Number = i
				});
				conf.AddSequence (new Sequence () {
					Pin = conf.DigitalPins [(int)i],
					GroupName = i.ToString ()
				});
			}

			Assert.AreEqual (10, conf.SequenceGroups.Count);

			conf.RemoveSequence (5);
			Assert.AreEqual (9, conf.SequenceGroups.Count);

			conf.Sequences [2].GroupName = conf.Sequences [0].GroupName;
			Assert.AreEqual (8, conf.SequenceGroups.Count);
		}
	}
}

