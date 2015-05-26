#define Window
using System;
using NPlot;
using NPlot.Gtk;
using Gtk;


namespace NplotTest
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Application.Init ();
			var w = new NPlotConfigTestWindow ("NplotTest");
			w.ShowAll ();
			try{
			Application.Run ();
			}catch(Exception e) {
				Console.Error.WriteLine (e);
			}
		}
	}
}
