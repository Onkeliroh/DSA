using Gtk;
using Cairo;
using Rsvg;

public partial class MainWindow: Gtk.Window
{
	public MainWindow () : base (Gtk.WindowType.Toplevel)
	{
		Build ();

		drawingarea1.ExposeEvent += Draw;
	}

	public void Draw (object sender, ExposeEventArgs args)
	{
		Context context = Gdk.CairoHelper.Create (this.drawingarea1.GdkWindow);

		var dings = new Handle ("arduino_uno.svg");

		dings.RenderCairo (context);
	}

	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}
}
