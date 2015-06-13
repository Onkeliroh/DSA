using System;
using Gtk;

public partial class MainWindow: Gtk.Window
{
	public MainWindow () : base (Gtk.WindowType.Toplevel)
	{
		Build ();

		label1.ButtonPressEvent += (o, e) =>
		{
			Console.WriteLine (e);
		};
	}

	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}

	protected void OnLabel1DragMotion (object o, DragMotionArgs args)
	{
		Console.WriteLine ("DragMotion:\t" + args.ToString ());
	}

	protected void OnLabel1DragBegin (object o, DragBeginArgs args)
	{
		Console.WriteLine ("DragBegin:\t" + args.ToString ());
	}

	protected void OnLabel1KeyPressEvent (object o, KeyPressEventArgs args)
	{
		Console.WriteLine ("KeypressEvent:\t" + args.ToString ());
	}

	protected void OnSpinbutton1ChangeValue (object o, EventArgs args)
	{
		var child = (fixed1 [eventbox1] as Gtk.Fixed.FixedChild);
		fixed1.Move (eventbox1, spinbutton1.ValueAsInt, child.Y);
	}

	protected void OnLabel1WidgetEvent (object o, WidgetEventArgs args)
	{
		Console.WriteLine (DateTime.Now + "\tLabel\t" + args.Event.Type);
	}

	protected void OnDrawingarea1WidgetEvent (object o, WidgetEventArgs args)
	{
		Console.WriteLine (DateTime.Now + "\tDrawingArea\t" + args.Event.Type);
	}

	protected void OnFixed1WidgetEvent (object o, WidgetEventArgs args)
	{
		Console.WriteLine (DateTime.Now + "\tFixed\t" + args.Event.Type);
	}
}
