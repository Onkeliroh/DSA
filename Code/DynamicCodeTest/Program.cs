using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using Microsoft.CSharp;
using System.Reflection;
using System.IO;
using System.Net;
using System.Linq;

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
	public static Dictionary<string,double> parameter_= new Dictionary<string,double>();

	public static void Main ()
	{
		EnterParameter ();

		Console.WriteLine ("\n---------------------------------------\n");

		Console.WriteLine ("Please enter a mathematical function:");
		string func = Console.ReadLine ();

		string tobecompiled = 
			@"using System; public class DynamicClass	{ public static double Main(double[] parameters) { return (  function  );}}";


		int pos = 0;
		foreach (string s in parameter_.Keys.ToArray()) {
			Console.WriteLine (s);
			func=func.Replace(s,"parameters["+pos+"]");
			pos++;
		}


//		func = func.Replace ("val2", "parameters[0]");
//		func = func.Replace("val1","parameters[1]");
		tobecompiled = tobecompiled.Replace ("function", func);

		Console.WriteLine (tobecompiled);

		var provider = CodeDomProvider.CreateProvider ("c#");

		var options = new CompilerParameters ();
		var assemblyContainingNotDynamicClass = Path.GetFileName (Assembly.GetExecutingAssembly ().Location);
		options.ReferencedAssemblies.Add (assemblyContainingNotDynamicClass);
		var results = provider.CompileAssemblyFromSource (options, new[] { tobecompiled });
		if (results.Errors.Count > 0)
		{
			foreach (var error in results.Errors)
			{
				Console.WriteLine (error);
			}
		} else
		{
			var t = results.CompiledAssembly.GetType ("DynamicClass");
			Func<double[],double> dings = (Func<double[],double>)Delegate.CreateDelegate (typeof(Func<double[],double>), t.GetMethod ("Main"));
//			Console.WriteLine (dings (new double[]{Aint,Bint}));
			Console.WriteLine(dings(parameter_.Values.ToArray<double>()));

		}
	}

	public static void EnterParameter()
	{
		Console.WriteLine ("Please enter a parameter name:");
		string name = Console.ReadLine ();
		Console.WriteLine ("Please enter a value for this parameter:");
		double value = Convert.ToDouble(Console.ReadLine ());

		parameter_.Add (name, value);

		Console.WriteLine ("\nDo you wish to enter another value?:");
		if (Console.Read () == 'y') {
			EnterParameter ();
		}
	}
}
