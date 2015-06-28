using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using Microsoft.CSharp;
using System.Reflection;
using System.IO;

public class NotDynamicClass
{
	private readonly List<string> values = new List<string> ();

	public void AddValue (string value)
	{
		values.Add (value);
	}

	public void ProcessValues ()
	{
		foreach (var item in values)
		{
			Console.WriteLine (item);
		}
	}
}

class Program
{
	public const int Aint = 42;
	public const int Bint = 3;

	public static void Main ()
	{
		Console.WriteLine ("Please enter a mathematical function:");
		string func = Console.ReadLine ();

		var parts = func.Split (new char[]{ '+', '-', '*', '/', '(', ')' });

		foreach (string s in parts)
		{
			Console.WriteLine (s);
		}

		string tobecompiled = 
			@"using System; public class DynamicClass	{ public static double Main(double a) { return (" + func + @");}}";



		var provider = CodeDomProvider.CreateProvider ("c#");
		var options = new CompilerParameters ();
		var assemblyContainingNotDynamicClass = Path.GetFileName (Assembly.GetExecutingAssembly ().Location);
		options.ReferencedAssemblies.Add (assemblyContainingNotDynamicClass);
		var results = provider.CompileAssemblyFromSource (options, new[] { tobecompiled });
		var resultss = provider.C
		if (results.Errors.Count > 0)
		{
			foreach (var error in results.Errors)
			{
				Console.WriteLine (error);
			}
		} else
		{
			var t = results.CompiledAssembly.GetType ("DynamicClass");
			Func<double,double> dings = (Func<double,double>)Delegate.CreateDelegate (typeof(Func<double,double>), t.GetMethod ("Main"));
			Console.WriteLine (dings (Aint));
//			double result = (double)t.GetMethod ("Main").Invoke (null, null);
//			Console.WriteLine (result);

		}
	}
}
