using System;
using System.Linq;
using Gtk;
using PrototypeBackend;
using System.Threading.Tasks;
using GUIHelper;

namespace PrototypeDebugWindow
{
	public partial class MainWindow : Gtk.Window
	{
		Controller con = new Controller ();

		private Gtk.NodeStore NodeStoreDigitalPins = new NodeStore (typeof(DPinTreeNode));
		private Gtk.NodeStore NodeStoreAnalogPins = new NodeStore (typeof(APinTreeNode));
		private Gtk.NodeStore NodeStoreSequences = new NodeStore (typeof(SequenceTreeNode));


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
				if (a.UpdateOperation == UpdateOperation.Add)
				{
					tvLog.Buffer.Text += String.Format ("{0} | Added Pin -> {1}\n", DateTime.Now.ToString ("T"), a.Pin);
				} else if (a.UpdateOperation == UpdateOperation.Remove)
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

			con.SequencesUpdated += (o, a) =>
			{
				if (a.UpdateOperation == UpdateOperation.Add)
				{
					tvLog.Buffer.Text += String.Format ("{0} | Added Sequence -> {1}\n", DateTime.Now.ToString ("T"), a.Seq);
				} else if (a.UpdateOperation == UpdateOperation.Remove)
				{
					tvLog.Buffer.Text += String.Format ("{0} | Removed Sequence -> {1}\n", DateTime.Now.ToString ("T"), a.Seq);
				}
				FillSequenceNodes ();
			};

			nvAnalogPins.NodeSelection.Changed += (o, a) =>
			{
				Gtk.NodeSelection selection = (Gtk.NodeSelection)o;
				APinTreeNode node = (APinTreeNode)selection.SelectedNode;
				Console.WriteLine (node.Name);
			};

			nvDigitalPins.ButtonPressEvent += new ButtonPressEventHandler (OnItemButtonPressed);
		}

		[GLib.ConnectBeforeAttribute]
		protected void OnItemButtonPressed (object sender, ButtonPressEventArgs e)
		{
			if (e.Event.Button == 3) /* right click */
			{
				Menu m = new Menu ();
				MenuItem deleteItem = new MenuItem ("Delete this item");
				deleteItem.ButtonPressEvent += new ButtonPressEventHandler (OnDeleteItemButtonPressed);
				m.Add (deleteItem);
				m.ShowAll ();
				m.Popup ();
			}
		}

		protected void OnDeleteItemButtonPressed (object sender, ButtonPressEventArgs e)
		{
			ITreeNode node = nvDigitalPins.NodeSelection.SelectedNode;

			con.RemovePin ((node as DPinTreeNode).RealName);

			nvDigitalPins.QueueDraw ();
		}

		private int GetNodeIndex (ITreeNode node, ref NodeView view)
		{
			int index = -1;
			if (view.NodeStore.Data.Count > 0)
			{
				index = 0;
				var iter = view.NodeStore.GetEnumerator ();

				while (iter.Current != node)
				{
					if (!iter.MoveNext ())
					{
						return -1;
					}
					index++;
				}
			}
			return index;
		}

		private void FillDigitalPinNodes ()
		{
			NodeStoreDigitalPins.Clear ();
			int index = 0;
			foreach (IPin pin in con.ControllerPins)
			{
				if (pin.Type == PinType.DIGITAL)
				{
					NodeStoreDigitalPins.AddNode (new DPinTreeNode (pin as DPin, index));
					index++;
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

		private void FillSequenceNodes ()
		{
			NodeStoreSequences.Clear ();
			foreach (Sequence seq in con.ControlSequences)
			{
				NodeStoreSequences.AddNode (new SequenceTreeNode (seq));
			}
			nvSequences.QueueDraw ();
		}

		private void BuildNodeViews ()
		{
			nvDigitalPins.NodeStore = NodeStoreDigitalPins;

			nvDigitalPins.AppendColumn ("Name(Pin)", new Gtk.CellRendererText (), "text", 0);
			nvDigitalPins.AppendColumn ("Color", new Gtk.CellRendererPixbuf (), "pixbuf", 1);
			nvDigitalPins.AppendColumn ("Seqeuence", new Gtk.CellRendererText (), "text", 2);

			nvDigitalPins.Show ();

			nvAnalogPins.NodeStore = NodeStoreAnalogPins;

			nvAnalogPins.AppendColumn ("Name(Pin)", new Gtk.CellRendererText (), "text", 0);
			nvAnalogPins.AppendColumn ("Color", new Gtk.CellRendererPixbuf (), "pixbuf", 1);
			nvAnalogPins.AppendColumn ("Signal", new Gtk.CellRendererText (), "text", 2);
			nvAnalogPins.AppendColumn ("Slope", new Gtk.CellRendererText (), "text", 3);
			nvAnalogPins.AppendColumn ("Offset", new Gtk.CellRendererText (), "text", 4);
			nvAnalogPins.AppendColumn ("Unit", new Gtk.CellRendererText (), "text", 5);
			nvAnalogPins.AppendColumn ("Frequency", new Gtk.CellRendererText (), "text", 6);
			nvAnalogPins.AppendColumn ("Interval", new Gtk.CellRendererText (), "text", 7);

			nvAnalogPins.Show ();

			nvSequences.NodeStore = NodeStoreSequences;

			nvSequences.AppendColumn (new TreeViewColumn ("Name", new CellRendererText (), "text", 0));
			nvSequences.AppendColumn (new TreeViewColumn ("Pins [Name(Pin)]", new CellRendererText (), "text", 1));

			nvSequences.Show ();
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

		protected void OnBtnRemoveAPinClicked (object sender, EventArgs e)
		{
			APinTreeNode node = (APinTreeNode)nvAnalogPins.NodeSelection.SelectedNode;
			if (node != null)
			{
				con.RemovePin (node.RealName);
			}
		}

		protected void OnBtnClearAPinsClicked (object sender, EventArgs e)
		{
			con.ClearPins (PinType.ANALOG);
		}

		protected void OnBtnAddSignalClicked (object sender, EventArgs e)
		{
			var dialog = new SignalConfigurationDialog.SignalConfigurationDialog ();
			dialog.Response += (o, args) =>
			{
			};
			dialog.Run ();
			dialog.Destroy ();
		}

		protected void OnBtnAddSequenceClicked (object sender, EventArgs e)
		{
			var dialog = new SequenceConfigurationsDialog.SequenceConfiguration (con.GetDPinsWithoutSequence ());
			dialog.Response += (o, args) =>
			{
				if (args.ResponseId == ResponseType.Apply)
				{
					con.AddSequence (dialog.PinSequence);
				}
			};
			dialog.Run ();
			dialog.Destroy ();
		}
	}
}
