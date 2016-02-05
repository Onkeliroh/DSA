using System;
using System.Text.RegularExpressions;
using System.CodeDom.Compiler;
using System.IO;
using System.Reflection;

namespace Backend
{
	/// <summary>
	/// Operation compiler.
	/// </summary>
	public static class OperationCompiler
	{
		/// <summary>
		/// Compiles a mathematical operation given by a string and constrained by the available parameters.
		/// </summary>
		/// <returns>The operation.</returns>
		/// <param name="func">The arithmetic expression</param>
		/// <param name="parameterNames">Array of all parameter by name</param>
		public static Func<double[],double> CompileOperation (string func, string[] parameterNames)
		{
			//the  blueprint for the class, wich will be compiled
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

			//replace parameternames with representation in the array
			int pos = 0;
//			func = func.Replace ("A", "");
//			func = func.Replace ("a", "");
			foreach (string s in parameterNames)
			{
				var value = s.Replace (" ", "");
				func = func.Replace (value, "parameters[" + pos + "]");
				pos++;
			}

			//add a fored conversion to double, after each operator
			//this is because of the way c# handles values without floating points
			var parts = Regex.Split (func, @"(?<=[+,-,*,/])");
			func = @"(double)" + parts [0];
			for (int i = 1; i < parts.Length; i++)
			{
				func += @"(double)" + parts [i];
			}

			tobecompiled = tobecompiled.Replace ("function", func);

			var provider = CodeDomProvider.CreateProvider ("c#");

			var options = new CompilerParameters ();
			var assemblyContainingNotDynamicClass = Path.GetFileName (Assembly.GetExecutingAssembly ().Location);
			options.ReferencedAssemblies.Add (assemblyContainingNotDynamicClass);
			var results = provider.CompileAssemblyFromSource (options, new[] { tobecompiled });

			//if there were no errors while compiling
			if (results.Errors.Count > 0)
			{
				#if DEBUG
				foreach (var error in results.Errors)
				{
					Console.WriteLine (error);
				}
				#endif
			} else
			{
				//extract class and method
				var t = results.CompiledAssembly.GetType ("DynamicClass");
				return (Func<double[],double>)Delegate.CreateDelegate (typeof(Func<double[],double>), t.GetMethod ("Main"));
			}
			return null;
		}
	}
}

