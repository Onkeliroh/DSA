using System;
using Gtk;
using PrototypeBackend;
using System.Linq;

namespace PrototypeDebugWindow
{
	public partial class MainWindow : Gtk.Window
	{
		public MainWindow () :
			base (Gtk.WindowType.Toplevel)
		{
			this.Build ();
			InitComponents ();

			this.Maximize ();
		}

		private void InitComponents ()
		{
			ArduinoController.Init ();
			ArduinoController.OnConnectionChanged += (object sender, EventArgs e) =>
			{
				if (ArduinoController.IsConnected)
				{
					lblConnectionStatus.Text = "connected";
					ImageConnectionStatus.Pixbuf = global::Stetic.IconLoader.LoadIcon (this, "gtk-connect", global::Gtk.IconSize.Menu);
				} else
				{
					lblConnectionStatus.Text = "<b>NOT</b> connected";
					lblConnectionStatus.UseMarkup = true;
					ImageConnectionStatus.Pixbuf = global::Stetic.IconLoader.LoadIcon (this, "gtk-disconnect", global::Gtk.IconSize.Menu);
				}
			};

			BuildMenu ();
		}

		private void BuildMenu ()
		{
			MenuBar mbar = (this.UIManager.GetWidget ("/menubarMain") as MenuBar);

			//FileMenu
			Menu filemenu = new Menu ();
			MenuItem file = new MenuItem ("File");
			file.Submenu = filemenu;
			MenuItem exit = new MenuItem ("Exit");
			exit.Activated += (sender, e) => OnDeleteEvent (null, null);
			filemenu.Append (exit);
			mbar.Append (file);

			//ConnectionMenu
			Menu connectionmenu = new Menu ();
			MenuItem connection = new MenuItem ("Connection");
			connection.Submenu = connectionmenu;
			Menu portmenu = new Menu ();
			MenuItem port = new MenuItem ("Port");
			connectionmenu.Append (port);
			port.Submenu = portmenu;

			port.Activated += (object sender, EventArgs e) =>
			{
				foreach (MenuItem mi in portmenu.AllChildren)
				{
					portmenu.Remove (mi);
				}
				foreach (String s in System.IO.Ports.SerialPort.GetPortNames())
				{
					CheckMenuItem portname = new CheckMenuItem (s);
					portname.Toggled += (object senderer, EventArgs ee) =>
					{
						if ((senderer as CheckMenuItem).Active)
						{
							ArduinoController.SerialPortName = ((senderer as CheckMenuItem).Child as Label).Text;
							ArduinoController.Setup ();
						} else
						{
							lblConnectionStatus.Text = "<b>NOT</b> connected";
							lblConnectionStatus.UseMarkup = true;
						}
					};
					portmenu.Append (portname);
				}
				portmenu.ShowAll ();
			};


			mbar.Append (connection);
			mbar.ShowAll ();
			
		}

		protected void OnDeleteEvent (object obj, DeleteEventArgs a)
		{
			ArduinoController.Disconnect ();
			Application.Quit ();
		}
	}
}

