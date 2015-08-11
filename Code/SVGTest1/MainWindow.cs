using Gtk;
using Gdk;
using Cairo;
using Rsvg;
using System.IO;

public partial class MainWindow: Gtk.Window
{
	public MainWindow () : base (Gtk.WindowType.Toplevel)
	{
		Build ();

//		drawingarea1.ExposeEvent += Draw1;
	}


	public void Draw1 (object sender, ExposeEventArgs args)
	{
		Context context = Gdk.CairoHelper.Create (this.drawingarea1.GdkWindow);
		context.Rotate (0.45);

		var dings = new Handle ("arduino_uno.svg");

		dings.RenderCairo (context);
		context.Paint ();
	}

	public void Draw2 (object sender, ExposeEventArgs args)
	{
		Gdk.Pixbuf buf = new Rsvg.Handle ("arduino_uno.svg").Pixbuf;

		buf = buf.ScaleSimple ((int)(buf.Width / 1), (int)(buf.Height / 1), Gdk.InterpType.Bilinear);

		const string bufbuf = "bufbuf";
		buf.Save (bufbuf, "png");

		var img = new ImageSurface (bufbuf);
		File.Delete (bufbuf);

		var context = CairoHelper.Create (this.drawingarea1.GdkWindow);

		context.SetSource (new SurfacePattern (img));

		context.Paint ();
	}

	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}

	protected void OnCombobox1Changed (object sender, System.EventArgs e)
	{
//		var context = CairoHelper.Create (this.drawingarea1.GdkWindow);
//		context.SetSourceRGB (255, 255, 255);
//		context.Fill ();
//		context.Paint ();

		switch ((sender as ComboBox).ActiveText)
		{
		case "Draw1":
			this.ExposeEvent -= Draw2;
			this.ExposeEvent += Draw1;
			break;
		case "Draw2":
			this.ExposeEvent -= Draw1;
			this.ExposeEvent += Draw2;
			break;
		}
		ShowAll ();
	}
}
