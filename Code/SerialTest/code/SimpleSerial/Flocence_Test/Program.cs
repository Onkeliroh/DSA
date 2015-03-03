using Gtk;


namespace FlocenceTest
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Application.Init ();
			System.Console.WriteLine ("Start");
			var window = new PlotClass ();
			window.ShowAll ();
			Application.Run ();
			System.Console.WriteLine ("Stop");

		}
	}
}
