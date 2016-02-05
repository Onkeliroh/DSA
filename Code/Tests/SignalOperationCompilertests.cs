using System;
using Backend;
using NUnit.Framework;

namespace PrototypeTests
{
	[TestFixture ()]
	public class SignalOperationCompilerTests
	{
		[Test ()]
		public void SignalOperationCompilerTest1 ()
		{
			string func = "val1";
			string[] paramnames = new string[]{ "val1" };
			double[] paramvalues = new double[]{ 42 };

			var method = OperationCompiler.CompileOperation (func, paramnames);

			Assert.AreNotEqual (null, method);

			Assert.AreEqual (42, method (paramvalues));
		}

		[Test ()]
		public void SignalOperationCompilerTest2 ()
		{
			string func = "2+3";
			var method = OperationCompiler.CompileOperation (func, new string[]{ });
			Assert.AreNotEqual (null, method);
			Assert.AreEqual (5, method (new double[]{ }));
		}

		[Test ()]
		public void SignalOperationCompilerTest3 ()
		{
			string func = "val1 + val2 / 5";
			var method = OperationCompiler.CompileOperation (func, new string[]{ "val1", "val2" });
			Assert.AreNotEqual (null, method);
			Assert.AreEqual (43 + 4 / 5.0, method (new double[]{ 43, 4 }), 0.0001);
		}

		[Test ()]
		public void SignalOperationCompilerMultiTest1 ()
		{
			string func1 = "val1";
			string func2 = "val1 + 0.25 * 13 + val2";
			string[] valuesnames = new string[]{ "val1", "val2" };

			double[] values = new double[]{ 24.4, 20.9 };
			var method1 = OperationCompiler.CompileOperation (func1, valuesnames);
			var method2 = OperationCompiler.CompileOperation (func2, valuesnames);

			Assert.AreNotEqual (null, method1);
			Assert.AreNotEqual (null, method2);

			Assert.AreEqual (24.4, method1 (values));
			Assert.AreEqual (24.4 + 0.25 * 13 + 20.9, method2 (values));
		}
	}
}

