using System;
using Gtk;
using PrototypeBackend;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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
					if (ArduinoController.SerialPortName.Equals (s))
					{
						portname.Toggle ();
					}

					portname.Toggled += (object senderer, EventArgs ee) =>
					{
						if ((senderer as CheckMenuItem).Active)
						{
							ArduinoController.SerialPortName = ((senderer as CheckMenuItem).Child as Label).Text;
							ArduinoController.Setup ();
						} else
						{
							ArduinoController.Disconnect ();
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

		protected void OnKeyPressEvent (object obj, KeyPressEventArgs a)
		{
			//TODO shotcuts -> mask vergleich
			if (a.Event.Key == Gdk.Key.q && a.Event.State == Gdk.ModifierType.ControlMask)
			{
				OnDeleteEvent (null, null);
			}
		}

		protected async void OnBtnDigitalPingTestClicked (object sender, EventArgs e)
		{
			if (ArduinoController.IsConnected)
			{
				for (int i = 0; i < ArduinoController.NumberOfDigitalPins; i++)
				{
					ArduinoController.SetPin (i, PinMode.OUTPUT, DPinState.HIGH);
					await Task.Delay (500);
				}
				await Task.Delay (2000);
				for (int i = 0; i < ArduinoController.NumberOfDigitalPins; i++)
				{
					ArduinoController.SetPin (i, PinMode.OUTPUT, DPinState.LOW);
					await Task.Delay (500);
				}
			}
		}

		protected async void OnBtnBlinkSequenceTestClicked (object sender, EventArgs e)
		{
			if (ArduinoController.IsConnected)
			{
				var con = new Controller ();
				var scheduler = new Scheduler ();
				con.AddScheduler (scheduler);

				var dpin = new DPin ("D13", 13);
				var sequence = new Sequence () {
					Pin = dpin,
					Repetitions = 10
				};
				sequence.AddSequenceOperation (new SequenceOperation () {
					Duration = TimeSpan.FromSeconds (1),
					State = DPinState.HIGH,
					Time = TimeSpan.FromSeconds (0)
				});
				sequence.AddSequenceOperation (new SequenceOperation {
					Duration = TimeSpan.FromSeconds (1),
					State = DPinState.LOW,
					Time = TimeSpan.FromSeconds (1)
				});
				con.ControlSequences.Add (sequence);

				con.Start ();

				while (sequence.Current () != null)
				{
					await Task.Delay (10);
				}
				con.Stop ();
			} else
			{
				MessageDialog dialog = new MessageDialog (this, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, "Please connect first to a Arduino.");
				dialog.Close += (senderer, ee) => dialog.Dispose ();
				dialog.ShowNow ();
			}
		}

		protected async void OnBtnDoubleBlinkClicked (object sender, EventArgs e)
		{
			if (ArduinoController.IsConnected)
			{
				var con = new Controller ();

				for (int i = 0; i < 2; i++)
				{
					var sequence = new Sequence () {
						Pin = new DPin ("Dings", i + 11),
						Repetitions = 10
					};
					sequence.AddSequenceOperation (new SequenceOperation () {
						Duration = TimeSpan.FromSeconds (1),
						State = DPinState.HIGH,
						Time = TimeSpan.FromSeconds (0)
					});
					sequence.AddSequenceOperation (new SequenceOperation {
						Duration = TimeSpan.FromSeconds (1),
						State = DPinState.LOW,
						Time = TimeSpan.FromSeconds (1 + i)
					});
					con.ControlSequences.Add (sequence);
				}

				foreach (Sequence seq in con.ControlSequences)
				{
					Console.WriteLine (seq.ToString ());
				}

				con.Start ();

				while (con.ControlSequences [0].CurrentState != SequenceState.Done && con.ControlSequences [0].CurrentState != SequenceState.Done)
				{
					await Task.Delay (10);
				}
				con.Stop ();
			} else
			{
				MessageDialog dialog = new MessageDialog (this, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, "Please connect first to a Arduino.");
				dialog.Close += (senderer, ee) => dialog.Dispose ();
				dialog.ShowNow ();
			}
		}
	}
}

