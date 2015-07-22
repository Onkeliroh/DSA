using System;
using Gtk;
using PrototypeBackend;
using System.Threading.Tasks;
using System.Security;
using Gdk;

namespace PrototypeDebugWindow
{
	public partial class MainWindow : Gtk.Window
	{
		Controller con = new Controller ();

		private Gtk.NodeStore NodeStoreDigitalPins = new NodeStore (typeof(DPinTreeNode));
		private Gtk.NodeStore NodeStoreAnalogPins = new NodeStore (typeof(APinTreeNode));


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
			BuildNodeViews ();

			con.PinsUpdated += (o, a) =>
			{
				if (a.UpdateOperation == PinUpdateOperation.Add)
				{
					tvLog.Buffer.Text += String.Format ("{0} | Added Pin -> {1}\n", DateTime.Now.ToString ("T"), a.Pin);
				} else if (a.UpdateOperation == PinUpdateOperation.Remove)
				{
					tvLog.Buffer.Text += String.Format ("{0} | Removed Pin -> {1}\n", DateTime.Now.ToString ("T"), a.Pin);
				}
				if (a.Type == PinType.DIGITAL)
				{
					FillDigitalPinNodes ();
				} else if (a.Type == PinType.ANALOG)
				{
					FillAnalogPinNodes ();
				}
			};
		}

		private void FillDigitalPinNodes ()
		{
			NodeStoreDigitalPins.Clear ();
			foreach (IPin pin in con.ControllerPins)
			{
				if (pin.Type == PinType.DIGITAL)
				{
					NodeStoreDigitalPins.AddNode (new DPinTreeNode (pin as DPin));
				}
			}
			nvDigitalPins.QueueDraw ();
		}

		private void FillAnalogPinNodes ()
		{
			NodeStoreAnalogPins.Clear ();
			foreach (IPin pin in con.ControllerPins)
			{
				if (pin.Type == PinType.ANALOG)
				{
					NodeStoreAnalogPins.AddNode (new APinTreeNode (pin as APin));
				}
			}
			nvAnalogPins.QueueDraw ();
		}

		private void BuildNodeViews ()
		{
			nvDigitalPins.NodeStore = NodeStoreDigitalPins;

			nvDigitalPins.AppendColumn ("Name(Pin)", new Gtk.CellRendererText (), "text", 0);
			nvDigitalPins.AppendColumn ("Color", new Gtk.CellRendererText (), "text", 1);
			nvDigitalPins.AppendColumn ("Seqeuence", new Gtk.CellRendererText (), "text", 2);

			nvDigitalPins.Show ();

			nvAnalogPins.NodeStore = NodeStoreAnalogPins;

			nvAnalogPins.AppendColumn ("Name(Pin)", new Gtk.CellRendererText (), "text", 0);
			nvAnalogPins.AppendColumn ("Color", new Gtk.CellRendererText (), "text", 1);
			nvAnalogPins.AppendColumn ("Signal", new Gtk.CellRendererText (), "text", 2);
			nvAnalogPins.AppendColumn ("Slope", new Gtk.CellRendererText (), "text", 3);
			nvAnalogPins.AppendColumn ("Offset", new Gtk.CellRendererText (), "text", 4);
			nvAnalogPins.AppendColumn ("Unit", new Gtk.CellRendererText (), "text", 5);
			nvAnalogPins.AppendColumn ("Frequency", new Gtk.CellRendererText (), "text", 6);
			nvAnalogPins.AppendColumn ("Interval", new Gtk.CellRendererText (), "text", 7);

			nvAnalogPins.Show ();
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
			con.Stop ();
			ArduinoController.Disconnect ();
			Application.Quit ();
		}

		protected void OnKeyPressEvent (object obj, KeyPressEventArgs a)
		{
			//TODO shotcuts -> mask vergleich
			if (a.Event.Key == Gdk.Key.q && (a.Event.State & Gdk.ModifierType.ControlMask) == Gdk.ModifierType.ControlMask)
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

		protected void OnBtnBlinkSequenceTestClicked (object sender, EventArgs e)
		{
			if (ArduinoController.IsConnected)
			{
				con.ControlSequences.Clear (); 
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
				});
				sequence.AddSequenceOperation (new SequenceOperation {
					Duration = TimeSpan.FromSeconds (1),
					State = DPinState.LOW,
				});
				con.ControlSequences.Add (sequence);

				con.Start ();
			} else
			{
				MessageDialog dialog = new MessageDialog (this, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, "Please connect first to a Arduino.");
				dialog.Close += (senderer, ee) => dialog.Dispose ();
				dialog.ShowNow ();
			}
		}

		protected void OnBtnDoubleBlinkClicked (object sender, EventArgs e)
		{
			if (ArduinoController.IsConnected)
			{
				con.ControlSequences.Clear ();

				var sequence = new Sequence () {
					Pin = new DPin ("Dings", 3),
					Repetitions = 10
				};
				sequence.AddSequenceOperation (new SequenceOperation () {
					Duration = TimeSpan.FromSeconds (1),
					State = DPinState.HIGH,
				});
				sequence.AddSequenceOperation (new SequenceOperation {
					Duration = TimeSpan.FromSeconds (1),
					State = DPinState.LOW,
				});
				con.ControlSequences.Add (sequence);

				sequence = new Sequence () {
					Pin = new DPin ("Dings", 4),
					Repetitions = 10
				};
				sequence.AddSequenceOperation (new SequenceOperation () {
					Duration = TimeSpan.FromSeconds (1),
					State = DPinState.LOW,
				});
				sequence.AddSequenceOperation (new SequenceOperation {
					Duration = TimeSpan.FromSeconds (1),
					State = DPinState.HIGH,
				});
				con.ControlSequences.Add (sequence);

				foreach (Sequence seq in con.ControlSequences)
				{
					Console.WriteLine (seq.ToString ());
				}

				con.Start ();
			} else
			{
				MessageDialog dialog = new MessageDialog (this, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, "Please connect first to a Arduino.");
				dialog.Run ();
				dialog.Destroy ();
			}
		}

		protected void OnBtnStopNResetClicked (object sender, EventArgs e)
		{
			con.Stop ();
			for (int i = 0; i < ArduinoController.NumberOfDigitalPins; i++)
			{
				ArduinoController.SetPin (i, PinMode.OUTPUT, DPinState.LOW);
			}
		}

		protected void OnBtnAddDPinClicked (object sender, EventArgs e)
		{
			int[] dings = con.AvailableDigitalPins;

			var dialog = new DigitalPinConfigurationDialog.DigitalPinConfiguration (dings);
			dialog.Response += (o, args) =>
			{
				if (args.ResponseId == ResponseType.Apply)
				{
					con.AddPin (dialog.Pin);
				}
			};
			dialog.Run ();
			dialog.Destroy ();
		}

		protected void OnBtnClearDPinsClicked (object sender, EventArgs e)
		{
			con.ClearPins (PinType.DIGITAL);
		}

		protected void OnBtnRemoveDPinClicked (object sender, EventArgs e)
		{
			DPinTreeNode node = (DPinTreeNode)nvDigitalPins.NodeSelection.SelectedNode;
			if (node != null)
			{
				con.RemovePin (node.RealName);
			}
		}

		protected void OnBtnAddAPinClicked (object sender, EventArgs e)
		{
			int[] dings = con.AvailableAnalogPins;

			var dialog = new AnalogPinConfigurationDialog.AnalogPinConfiguration (dings);
			dialog.Response += (o, args) =>
			{
				if (args.ResponseId == ResponseType.Apply)
				{
					con.AddPin (dialog.Pin);
				}
			};
			dialog.Run ();
			dialog.Destroy ();
		}

		protected void OnBtnRemoveDPin1Clicked (object sender, EventArgs e)
		{
			APinTreeNode node = (APinTreeNode)nvAnalogPins.NodeSelection.SelectedNode;
			if (node != null)
			{
				con.RemovePin (node.RealName);
			}
		}

		protected void OnBtnClearDPins1Clicked (object sender, EventArgs e)
		{
			con.ClearPins (PinType.ANALOG);
		}
	}

	public class DPinTreeNode : Gtk.TreeNode
	{
		[Gtk.TreeNodeValue (Column = 0)]
		public string Name;
		[Gtk.TreeNodeValue (Column = 1)]
		public string Color = "";
		[Gtk.TreeNodeValue (Column = 2)]
		public string Sequence = "";

		public string RealName { get; private set; }

		public DPinTreeNode (DPin pin)
		{
			Name = pin.Name + "(" + pin.Number + ")";
			Color = "";
			Sequence = "";

			RealName = pin.Name;
		}
	}

	public class APinTreeNode : Gtk.TreeNode
	{
		[Gtk.TreeNodeValue (Column = 0)]
		public string Name;
		[Gtk.TreeNodeValue (Column = 1)]
		public string Color = "";
		[Gtk.TreeNodeValue (Column = 2)]
		public string Signal = "";
		[Gtk.TreeNodeValue (Column = 3)]
		public double Slope = 1;
		[Gtk.TreeNodeValue (Column = 4)]
		public double Offset = 0;
		[Gtk.TreeNodeValue (Column = 5)]
		public string Unit = "";
		[Gtk.TreeNodeValue (Column = 6)]
		public double Frequency = 1;
		[Gtk.TreeNodeValue (Column = 7)]
		public double Interval = 1;

		public string RealName { get; private set; }

		public APinTreeNode (APin pin)
		{
			Name = pin.Name + "(" + pin.Number + ")";
			Color = "";
			Signal = "";
			Slope = pin.Slope;
			Offset = pin.Offset;
			Frequency = pin.Frequency;
			Interval = pin.Interval;
			Unit = pin.Unit;

			RealName = pin.Name;
		}
	}

	public class SignalTreeNode : Gtk.NodeView
	{
		[Gtk.TreeNodeValue (Column = 0)]
		public string Name;
		[Gtk.TreeNodeValue (Column = 1)]
		public Gtk.Button btnAddRemove;

		public SignalTreeNode (Signal analogSignal)
		{
			
		}

		public void ToggleButton (bool last = false)
		{
			if (last)
			{
				btnAddRemove.Label = "+";
			} else
			{
				btnAddRemove.Label = "-";
			}
		}
	}
}
