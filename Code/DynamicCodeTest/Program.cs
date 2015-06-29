using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

using NUnit.Framework;


class Program
{
	public static Dictionary<string,double> parameter_ = new Dictionary<string,double> ();

	public static List<Func<double[],double>> methods = new List<Func<double[], double>> ();

	public static void Main ()
	{
		EnterParameter ();
	
		EnterFunction ();
	}

	public static void EnterFunction ()
	{
		Console.WriteLine ("\n---------------------------------------\n");

		Console.WriteLine ("Please enter a mathematical function:");
		string func = Console.ReadLine ();


		var res = CompileCode (func);
		if (res != null)
		{
			methods.Add (res);
		} else
		{
			Console.WriteLine ("You have entered a false function. Please pay attention towards a proper syntax and only use variables pervisously defined by yourselfe.");
		}

		if (methods.Count > 0)
		{
			Console.WriteLine ("Result: " + methods.Last () (parameter_.Values.ToArray ()));
		} 
		Console.WriteLine ("\nDo you wish to enter another function?:");
		if (Console.Read () == 'y')
		{
			Console.WriteLine ();
			EnterFunction ();
		}

	}

	public static Func<double[],double> CompileCode (string func)
	{

		string tobecompiled = 
			@"using System; 
			public class DynamicClass	
			{ 
				public static double Main(double[] parameters) 
				{
					try{
						return ( function );
					} 
					catch(Exception e)
					{
						Console.Error.WriteLine(e);
					}
					return double.NaN;
				}
			}";



		//replace all parameter strings with their representation in the array
		int pos = 0;
		foreach (string s in parameter_.Keys.ToArray())
		{
			func = func.Replace (s, "parameters[" + pos + "]");
			pos++;
		}

		//add a forced conversion to double after each operator
		//this is becaus of the way c sharp handles values without floating points
		var parts = Regex.Split (func, @"(?<=[+,-,/,*])");
		func = @"(double)" + parts [0];
		for (int i = 1; i < parts.Length; i++)
		{
			func += @"(double)" + parts [i];
		}	


		tobecompiled = tobecompiled.Replace ("function", func);

		#if DEBUG
		Console.WriteLine (tobecompiled);
		#endif

		var provider = CodeDomProvider.CreateProvider ("c#");

		var options = new CompilerParameters ();
		var assemblyContainingNotDynamicClass = Path.GetFileName (Assembly.GetExecutingAssembly ().Location);
		options.ReferencedAssemblies.Add (assemblyContainingNotDynamicClass);
		var results = provider.CompileAssemblyFromSource (options, new[] { tobecompiled });

		//if there were no errors while compiling
		if (results.Errors.Count > 0)
		{
			foreach (var error in results.Errors)
			{
//				Console.WriteLine (error);
			}
		} else
		{
			//extract class and method
			var t = results.CompiledAssembly.GetType ("DynamicClass");
			return (Func<double[],double>)Delegate.CreateDelegate (typeof(Func<double[],double>), t.GetMethod ("Main"));
		}
		return null;
	}

	public static void EnterParameter ()
	{
		Console.WriteLine ("Please enter a parameter name:");
		string name = Console.ReadLine ();
		Console.WriteLine ("Please enter a value for this parameter:");
		double value = Convert.ToDouble (Console.ReadLine ());

		parameter_.Add (name, value);

		Console.WriteLine ("\nDo you wish to enter another value?:");
		if (Console.Read () == 'y')
		{
			Console.WriteLine ();
			EnterParameter ();
		}
	}
}
