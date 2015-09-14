using System;
using NUnit.Framework;
using PrototypeBackend;

namespace PrototypeTests
{
	[TestFixture ()]
	public class SignalTests
	{
		[Test ()]
		public void SignalConstructorTest ()
		{
			MeasurementCombination signal = new MeasurementCombination ();
			signal.Color = GUIHelper.ColorHelper.SystemColorToGdkColor (System.Drawing.Color.Blue);

			Assert.AreEqual (0, signal.Pins.Count);
			Assert.AreEqual (string.Empty, signal.Name);
			Assert.AreEqual (GUIHelper.ColorHelper.SystemColorToGdkColor (System.Drawing.Color.Blue), signal.Color);
			Assert.AreEqual (string.Empty, signal.OperationString);
		}

		[Test ()]
		public void SignalPinsTest ()
		{
			MeasurementCombination signal = new MeasurementCombination ();

			signal.Pins.Add (new APin () {
				Name = "Temp1",
				Number = 0
			});

			signal.OperationString = "A0";
			signal.Operation = PrototypeBackend.OperationCompiler.CompileOperation (
				signal.OperationString, 
				new string[]{ "A0" }
			);

			Assert.AreEqual (1, signal.Pins.Count);
			signal.Pins [0].Values.Add (new DateTimeValue (42, DateTime.Now));

			string func = "Temp1";
			signal.OperationString = func;
			Assert.AreEqual (42, signal.Value.Value);
		}
	}
}

