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

			Assert.AreEqual (null, seq.Pin);
			Assert.AreEqual ("", seq.Name);
			Assert.AreEqual (System.Drawing.Color.Empty, seq.Color);
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
				Color = new Gdk.Color (0, 0, 255),
			};

			Assert.AreNotEqual (null, seq.Pin);
			Assert.AreEqual ("Sequence of Awesome", seq.Name);
			Assert.AreEqual (System.Drawing.Color.AliceBlue, seq.Color);
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
					PlotColor = System.Drawing.Color.Yellow,
					State = DPinState.HIGH
				},
				Name = "Sequence of Awesome",
				Repetitions = 3,
				Color = System.Drawing.Color.AliceBlue
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
					PlotColor = System.Drawing.Color.Yellow,
					State = DPinState.HIGH
				},
				Name = "Sequence of Awesome",
				Repetitions = 3,
				Color = System.Drawing.Color.AliceBlue
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
	}
}

