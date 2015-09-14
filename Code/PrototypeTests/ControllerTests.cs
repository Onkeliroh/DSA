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
		//		[Test ()]
		//		public void AddTestScheduler ()
		//		{
		//			var tmp = new Controller ();
		//
		//			Assert.AreEqual (0, tmp.ControllerSignals.Count);
		//
		//			var sched = new Scheduler (){ DueTime = new DateTime (1) };
		//			tmp.AddScheduler (sched);
		//			Assert.AreEqual (1, tmp.ControllerSignals.Count);
		//
		//			tmp.AddScheduler (new PrototypeBackend.Scheduler () {
		//				DueTime = new DateTime (42),
		//			});
		//			Assert.AreEqual (2, tmp.ControllerSignals.Count);
		//
		//			tmp.AddScheduler (new Scheduler () {
		//				DueTime = new DateTime (3),
		//			});
		//
		//			Assert.AreEqual (new DateTime (1), tmp.ControllerSignals [0].DueTime);
		//			Assert.AreEqual (new DateTime (3), tmp.ControllerSignals [1].DueTime);
		//			Assert.AreEqual (new DateTime (42), tmp.ControllerSignals [2].DueTime);
		//
		//			tmp.AddScheduler (new Scheduler () {
		//				DueTime = new DateTime (13),
		//			});
		//
		//			Assert.AreEqual (new DateTime (1), tmp.ControllerSignals [0].DueTime);
		//			Assert.AreEqual (new DateTime (3), tmp.ControllerSignals [1].DueTime);
		//			Assert.AreEqual (new DateTime (13), tmp.ControllerSignals [2].DueTime);
		//			Assert.AreEqual (new DateTime (42), tmp.ControllerSignals [3].DueTime);
		//		}
		//
		//		[Test ()]
		//		public void AddScheduleRangeTest ()
		//		{
		//			var tmp = new PrototypeBackend.Controller ();
		//
		//			var dates = new Scheduler[4];
		//			dates [0] = new Scheduler (){ DueTime = new DateTime (4) };
		//			dates [1] = new Scheduler (){ DueTime = new DateTime (1) };
		//			dates [2] = new Scheduler (){ DueTime = new DateTime (88) };
		//			dates [3] = new Scheduler (){ DueTime = new DateTime (42) };
		//
		//			tmp.AddSchedulerRange (dates);
		//			Assert.AreEqual (4, tmp.ControllerSignals.Count);
		//
		//			Assert.AreEqual (true, tmp.ControllerSignals [0].DueTime.Equals (new DateTime (1)));
		//			Assert.AreEqual (new DateTime (4), tmp.ControllerSignals [1].DueTime);
		//			Assert.AreEqual (new DateTime (42), tmp.ControllerSignals [2].DueTime);
		//			Assert.AreEqual (new DateTime (88), tmp.ControllerSignals [3].DueTime);
		//		}
		//
		//		[Test ()]
		//		public void RemoveSchedulerTest ()
		//		{
		//			var tmp = new PrototypeBackend.Controller ();
		//
		//			var dates = new Scheduler[5];
		//			dates [0] = new Scheduler (){ DueTime = new DateTime (2) };
		//			dates [1] = new Scheduler (){ DueTime = new DateTime (4) };
		//			dates [2] = new Scheduler (){ DueTime = new DateTime (1) };
		//			dates [3] = new Scheduler (){ DueTime = new DateTime (42) };
		//			dates [4] = new Scheduler (){ DueTime = new DateTime (88) };
		//
		//			tmp.AddSchedulerRange (dates);
		//			Assert.AreEqual (5, tmp.ControllerSignals.Count);
		//
		//			tmp.RemoveScheduler (dates [1]);
		//			Assert.AreEqual (4, tmp.ControllerSignals.Count);
		//
		//			Assert.AreEqual (new DateTime (1), tmp.ControllerSignals [0].DueTime);
		//			Assert.AreEqual (new DateTime (2), tmp.ControllerSignals [1].DueTime);
		//			Assert.AreEqual (new DateTime (42), tmp.ControllerSignals [2].DueTime);
		//			Assert.AreEqual (new DateTime (88), tmp.ControllerSignals [3].DueTime);
		//		}
		//
		//		[Test ()]
		//		public void RemoveSchedulerRangeTest ()
		//		{
		//			var tmp = new PrototypeBackend.Controller ();
		//
		//			var dates = new Scheduler[5];
		//			dates [0] = new Scheduler (){ DueTime = new DateTime (2) };
		//			dates [1] = new Scheduler (){ DueTime = new DateTime (4) };
		//			dates [2] = new Scheduler (){ DueTime = new DateTime (1) };
		//			dates [3] = new Scheduler (){ DueTime = new DateTime (42) };
		//			dates [4] = new Scheduler (){ DueTime = new DateTime (88) };
		//
		//			tmp.AddSchedulerRange (dates);
		//			Assert.AreEqual (5, tmp.ControllerSignals.Count);
		//
		//			var deletedates = new Scheduler[4];
		//			deletedates [0] = dates [1];
		//			deletedates [1] = dates [3];
		//			deletedates [2] = dates [4];
		//			deletedates [3] = new Scheduler (){ DueTime = new DateTime (43) };
		//
		//			tmp.RemoveSchedulerRange (deletedates);
		//			Assert.AreEqual (2, tmp.ControllerSignals.Count);
		//
		//			Assert.AreEqual (new DateTime (1), tmp.ControllerSignals [0].DueTime);
		//			Assert.AreEqual (new DateTime (2), tmp.ControllerSignals [1].DueTime);
		//		}

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
		[Ignore]
		public void SequenceThread ()
		{
//			var con = new Controller ();
//
//
//			DPin[] dpins = new DPin[10];
//			for (int i = 0; i < dpins.Length; i++)
//			{
//				dpins [i] = new DPin ("", i);
//			}
//
//			Sequence[] seqs = new Sequence[10];
//			for (int i = 0; i < seqs.Length; i++)
//			{
//				seqs [i] = new Sequence ();
//				seqs [i].Pin = dpins [i];
//				seqs [i].AddSequenceOperation (DPinState.HIGH, TimeSpan.FromSeconds (0), TimeSpan.FromSeconds (i * 10));
//			}
//
//			con.ControlSequences.AddRange (seqs);
//			#if FAKESERIAL
//			con.Start ();
//			Thread.Sleep (1000);
//			foreach (Sequence seq in seqs)
//			{
//				Assert.AreEqual (1, seq.Cycle);
//				Assert.AreEqual (1, seq.Chain.Count);
//			}
//			con.Stop ();
//			#endif
		}

		[Test]
		[Ignore]
		public void CheckSequenceTest ()
		{
//			var con = new Controller ();
//			var seq = new Sequence ();
//			seq.AddSequenceOperation (new SequenceOperation () {
//				Time = TimeSpan.FromSeconds (10),
//				Duration = TimeSpan.FromSeconds (5),
//				State = DPinState.HIGH
//			});
//			seq.AddSequenceOperation (new SequenceOperation () {
//				Time = TimeSpan.FromSeconds (1),
//				Duration = TimeSpan.FromSeconds (5),
//				State = DPinState.HIGH
//			});
//
//			Assert.AreEqual (2, seq.Chain.Count);
//			Assert.AreEqual (TimeSpan.FromSeconds (1), seq.Chain [0].Time);
//			Assert.AreEqual (TimeSpan.FromSeconds (10), seq.Chain [1].Time);
//
//			con.ControlSequences.Add (seq);
//
//			Assert.AreEqual (true, con.CheckSequences ());
//
//			Assert.AreEqual (TimeSpan.FromSeconds (0), con.ControlSequences [0].Chain [0].Time);
//			Assert.AreEqual (TimeSpan.FromSeconds (6), con.ControlSequences [0].Chain [2].Time);
//
//
//			Console.WriteLine (con.ControlSequences [0].ToString ());
		}

		[Test]
		[Ignore]
		public void CheckSequenceTest2 ()
		{
//			var con = new Controller ();
//			var seq = new Sequence ();
//			seq.AddSequenceOperation (new SequenceOperation () {
//				Time = TimeSpan.FromSeconds (10),
//				Duration = TimeSpan.FromSeconds (5),
//				State = DPinState.HIGH
//			});
//			seq.AddSequenceOperation (new SequenceOperation () {
//				Time = TimeSpan.FromSeconds (1),
//				Duration = TimeSpan.FromSeconds (10),
//				State = DPinState.HIGH
//			});
//			con.ControlSequences.Add (seq);
//
//			Assert.AreNotEqual (true, con.CheckSequences ());
		}
	}
}

