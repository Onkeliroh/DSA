﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Gtk;
using GUIHelper;
using Logger;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.GtkSharp;
using OxyPlot.Series;
using PrototypeBackend;

namespace Frontend
{
	public partial class MainWindow : Gtk.Window
	{

		#region Member

		Controller con = new Controller ();

		private Gtk.NodeStore NodeStoreDigitalPins = new NodeStore (typeof(DPinTreeNode));
		private Gtk.NodeStore NodeStoreAnalogPins = new NodeStore (typeof(APinTreeNode));
		private Gtk.NodeStore NodeStoreSequences = new NodeStore (typeof(SequenceTreeNode));
		private Gtk.NodeStore NodeStoreMeasurementCombinations = new NodeStore (typeof(MeasurementCombinationTreeNode));

		private PlotView SequencePreviewPlotView;
		private PlotModel SequencePreviewPlotModel;
		private LinearAxis XAxis;

		private System.Timers.Timer TimeKeeperPresenter;

		public MainWindow () :
			base (Gtk.WindowType.Toplevel)
		{
			this.Build ();
			InitComponents ();

			#if !DEBUG
			this.notebook1.GetNthPage (3).Visible = false;
			#endif

			this.Maximize ();
		}

		#endregion

		private void InitComponents ()
		{
			#region Connection
			ArduinoController.OnConnectionChanged += (object sender, ConnectionChangedArgs e) =>
			{
				if (e.Connected)
				{
					lblConnectionStatus.Text = "connected to " + e.Port;
					try
					{
						ImageConnectionStatus.Pixbuf = global::Stetic.IconLoader.LoadIcon (this, "gtk-connect", global::Gtk.IconSize.Menu);
					} catch (Exception ex)
					{
						con.ConLogger.Log (ex.ToString (), LogLevel.ERROR);
					}
					if (ArduinoController.MCU != null && ArduinoController.MCU != "")
					{
						if (this.mcuW.SelectedBoard == null)
						{
							this.mcuW.Select (ArduinoController.MCU);
						} else
						{
							if (mcuW.SelectedBoard.MCU != ArduinoController.MCU)
							{
								Application.Invoke (RunMCUMessageDialog);
							}
						}
					}
				} else
				{
					lblConnectionStatus.Text = "<b>NOT</b> connected";
					lblConnectionStatus.UseMarkup = true;
					try
					{
						ImageConnectionStatus.Pixbuf = global::Stetic.IconLoader.LoadIcon (this, "gtk-disconnect", global::Gtk.IconSize.Menu);
					} catch (Exception ex)
					{
						con.ConLogger.Log (ex.ToString (), LogLevel.ERROR);
					}
				}
			};
			#endregion

			BuildMenu ();
			BuildNodeViews ();
			BuildMCUWidget ();
			BuildSequencePreviewPlot ();

			#if DEBUG
//			con.ConLogger.NewMessage += 
//				(sender, e) =>
//			{
//				try
//				{
//					if (tvLog.Buffer != null)
//					{
//						tvLog.Buffer.Text += 
//					String.Format ("{0} | {1} | {2}\n", e.Time.ToString ("T"), e.Level, e.Message);
//					}
//				} catch (Exception)
//				{
//				}
//			};
			#endif
			#if RELEASE
			con.ConLogger.NewMessage += 
			(sender, e) =>
			{
				if (e.Level == LogLevel.INFO)
				{
					tvLog.Buffer.Text += String.Format ("{0} | {1}| {2}\n", e.Time.ToString ("T"), e.Level, e.Message);
				}
			};
			#endif

			con.PinsUpdated += (o, a) =>
			{
				if (a.Type == PinType.DIGITAL)
				{
					FillDigitalPinNodes ();
					FillSequencePreviewPlot ();
					FillSignalNodes ();
				} else if (a.Type == PinType.ANALOG)
				{
					FillAnalogPinNodes ();
					FillSignalNodes ();
				}
			};
			con.SequencesUpdated += (o, a) =>
			{
				FillSequenceNodes ();
				FillSequencePreviewPlot ();
			};
			con.SignalsUpdated += (o, a) => FillSignalNodes ();

			con.OnControllerStarted += (o, a) => LockControlls (false);
			con.OnControllerStoped += (o, a) => LockControlls (true);

			nvDigitalPins.ButtonPressEvent += new ButtonPressEventHandler (OnDigitalPinNodePressed);
			nvSequences.ButtonPressEvent += new ButtonPressEventHandler (OnItemButtonPressed);
			nvMeasurementCombinations.ButtonPressEvent += new ButtonPressEventHandler (OnItemButtonPressed);
			nvAnalogPins.ButtonPressEvent += new ButtonPressEventHandler (OnAnalogPinNodePressed);

			TimeKeeperPresenter = new System.Timers.Timer (500);
			TimeKeeperPresenter.Elapsed += (sender, e) =>
			{
				lblTimePassed.Text = con.TimePassed.ToString ();
			};
		}

		#region NodeViewMouseActions

		[GLib.ConnectBeforeAttribute]
		protected void OnItemButtonPressed (object sender, ButtonPressEventArgs e)
		{
			if (e.Event.Button == 3)
			{ /* right click */
				Menu m = new Menu ();
				MenuItem deleteItem = new MenuItem ("Delete this item");
				deleteItem.ButtonPressEvent += (senderer, ee) =>
				{
					ITreeNode node = (sender as NodeView).NodeSelection.SelectedNode;
					if (node is DPinTreeNode)
					{
						con.RemovePin ((node as DPinTreeNode).Index);
					} else if (node is SequenceTreeNode)
					{
						con.RemoveSequence ((node as SequenceTreeNode).Index);
					} else if (node is APinTreeNode)
					{
						con.RemovePin ((node as APinTreeNode).Index);
					} else if (node is MeasurementCombinationTreeNode)
					{
						con.RemoveMeasurementCombination ((node as MeasurementCombinationTreeNode).Index);
					}
				};
				m.Add (deleteItem);


				m.ShowAll ();
				m.Popup ();
			}
		}

		[GLib.ConnectBeforeAttribute]
		protected void OnAnalogPinNodePressed (object sender, ButtonPressEventArgs e)
		{
			if (e.Event.Button == 3)
			{
				Menu m = new Menu ();
				MenuItem deleteItem = new MenuItem ("Delete this analog input");
				APinTreeNode pin = (sender as NodeView).NodeSelection.SelectedNode as APinTreeNode;

				if (pin != null)
				{

					deleteItem.ButtonPressEvent += (o, args) =>
					con.RemovePin (pin.Index);

					MenuItem addSignal = new MenuItem ("Add new Signal");
					MenuItem editSignal = new MenuItem ("Edit Signal");
					if (pin.Combination == null)
					{
						editSignal.Sensitive = false;
						addSignal.ButtonPressEvent += (o, args) => RunSignalDialog (null, pin.Pin);
					} else
					{
						addSignal.Sensitive = false;
						editSignal.ButtonPressEvent += (o, args) => RunSignalDialog (pin.Combination);
					}

					m.Add (addSignal);
					m.Add (editSignal);
					m.Add (deleteItem);

					m.ShowAll ();
					m.Popup ();
				}
			}
		}

		[GLib.ConnectBeforeAttribute]
		protected void OnDigitalPinNodePressed (object sender, ButtonPressEventArgs e)
		{
			if (e.Event.Button == 3)
			{
				Menu m = new Menu ();
				MenuItem deleteItem = new MenuItem ("Delete this digital output");
				DPinTreeNode pin = (sender as NodeView).NodeSelection.SelectedNode as DPinTreeNode;

				if (pin != null)
				{

					deleteItem.ButtonPressEvent += (o, args) =>
					con.RemovePin (pin.Index);

					MenuItem addSignal = new MenuItem ("Add new Sequence");
					MenuItem editSignal = new MenuItem ("Edit Sequence");
					if (pin.Sequence == null)
					{
						editSignal.Sensitive = false;
						addSignal.ButtonPressEvent += (o, args) => RunSequenceDialog (null, pin.Pin);
					} else
					{
						addSignal.Sensitive = false;
						editSignal.ButtonPressEvent += (o, args) => RunSequenceDialog (pin.Sequence);
					}

					m.Add (addSignal);
					m.Add (editSignal);
					m.Add (deleteItem);

					m.ShowAll ();
					m.Popup ();
				}
			}
		}

		#endregion

		#region FillNodes

		private void FillDigitalPinNodes ()
		{
			NodeStoreDigitalPins.Clear ();
			int index = 0;
			foreach (IPin pin in con.ControllerPins)
			{
				if (pin.Type == PinType.DIGITAL)
				{
					NodeStoreDigitalPins.AddNode (new DPinTreeNode (pin as DPin, index, con.GetCorespondingSequence (pin as DPin)));
					index++;
				}
			}
			nvDigitalPins.QueueDraw ();
		}

		private void FillAnalogPinNodes ()
		{
			NodeStoreAnalogPins.Clear ();
			int index = 0;
			foreach (IPin pin in con.ControllerPins)
			{
				if (pin.Type == PinType.ANALOG)
				{
					NodeStoreAnalogPins.AddNode (new APinTreeNode (pin as APin, index, con.GetCorespondingCombination (pin as APin)));
					index++;
				}
			}
			nvAnalogPins.QueueDraw ();
		}

		private void FillSequenceNodes ()
		{
			FillDigitalPinNodes ();
			NodeStoreSequences.Clear ();
			for (int i = 0; i < con.ControllerSequences.Count; i++)
			{
				NodeStoreSequences.AddNode (new SequenceTreeNode (con.ControllerSequences [i], i));
			}
			nvSequences.QueueDraw ();
		}

		private void FillSignalNodes ()
		{
			FillAnalogPinNodes ();
			NodeStoreMeasurementCombinations.Clear ();
			for (int i = 0; i < con.ControllerMeasurementCombinations.Count; i++)
			{
				NodeStoreMeasurementCombinations.AddNode (new MeasurementCombinationTreeNode (con.ControllerMeasurementCombinations [i], i));
			}
			nvMeasurementCombinations.QueueDraw ();
		}

		#endregion

		private void FillSequencePreviewPlot ()
		{
			SequencePreviewPlotModel.Axes.Clear ();
			SequencePreviewPlotModel.Series.Clear ();
			SequencePreviewPlotModel.Axes.Add (XAxis);


			double size = 1 / (double)con.ControllerSequences.Count;
			double startPos = 1;

			for (int i = 0; i < con.ControllerSequences.Count; i++)
			{
				var seq = con.ControllerSequences [i];

				var YAxis = new LinearAxis {
					Key = seq.Pin.ToString (),
					Title = seq.Name,
					Position = AxisPosition.Left,
					Minimum = -0.1,
					Maximum = 1.1,
					LabelFormatter = x => ((int)x == 0) ? "LOW" : "HIGH",
					IsPanEnabled = false,
					IsZoomEnabled = false,
					AbsoluteMaximum = 1.1,
					AbsoluteMinimum = -0.1,
					MinorStep = 1,
					MajorStep = 1,
					StartPosition = startPos - size,
					EndPosition = startPos,
				};

				startPos -= size;

				SequencePreviewPlotModel.Axes.Add (YAxis);

				//generate collection with operation data
				var data = new Collection<GUIHelper.TimeValue> ();
				var current = new TimeSpan (0);
				for (int j = 0; j < seq.Chain.Count; j++)
				{
					data.Add (new GUIHelper.TimeValue () {
						Time = current,
						Value = ((seq.Chain [j].State == DPinState.HIGH) ? 1 : 0)
					});
					current = current.Add (seq.Chain [j].Duration);

					data.Add (new GUIHelper.TimeValue () {
						Time = current,
						Value = ((seq.Chain [j].State == DPinState.HIGH) ? 1 : 0)
					});
				}

				var series = new LineSeries () {
					DataFieldX = "Time",
					DataFieldY = "Value",
					YAxisKey = seq.Pin.ToString (),
					ItemsSource = data,
					StrokeThickness = 2,
					Color = ColorHelper.GdkColorToOxyColor (seq.Color),
				};

				//generate followup series
				if (seq.Repetitions != 0 && seq.Chain.Count > 0)
				{
					var followupData = new Collection<TimeValue> ();
					followupData.Add (data.Last ());	
					followupData.Add (new TimeValue () {
						Time = data.Last ().Time,
						Value = ((seq.Chain [0].State == DPinState.HIGH) ? 1 : 0)			
					});	

					followupData.Add (new TimeValue () {
						Time = data.Last ().Time.Add (seq.Chain [0].Duration),
						Value = ((seq.Chain [0].State == DPinState.HIGH) ? 1 : 0)			
					});	

					var followupSeries = new LineSeries () {
						DataFieldX = "Time",
						DataFieldY = "Value",
						YAxisKey = seq.Pin.ToString (),
						ItemsSource = followupData,
						StrokeThickness = 2,
						LineStyle = LineStyle.Dash,
						Color = ColorHelper.GdkColorToOxyColor (seq.Color),
					};

					followupSeries.MouseDown += (sender, e) =>
					{
						if (e.ChangedButton == OxyMouseButton.Left)
						{
							RunSequenceDialog (seq);
						}
					};

					SequencePreviewPlotModel.Series.Add (followupSeries);
				}

				series.MouseDown += (sender, e) =>
				{
					if (e.ChangedButton == OxyMouseButton.Left)
					{
						RunSequenceDialog (seq);
					}
				};

				SequencePreviewPlotModel.Series.Add (series);
			}
			SequencePreviewPlotModel.InvalidatePlot (true);
			SequencePreviewPlotView.InvalidatePlot (true);
			SequencePreviewPlotView.ShowAll ();
		}

		#region BuildElements

		private void BuildMCUWidget ()
		{
			mcuW.Boards = con.BoardConfigs;
		}

		private void BuildNodeViews ()
		{
			#region Digital 
			nvDigitalPins.NodeStore = NodeStoreDigitalPins;
			nvDigitalPins.RowActivated += (o, args) =>
			{
				var pin = con.ControllerPins
					.Where (x => x.Type == PinType.DIGITAL)
					.ToList () [((o as NodeView).NodeSelection.SelectedNode as DPinTreeNode).Index];
				RunAddDPinDialog (pin as DPin);
			};

			nvDigitalPins.AppendColumn ("Name(Pin)", new Gtk.CellRendererText (), "text", 0);
			nvDigitalPins.AppendColumn ("Pin Number", new Gtk.CellRendererText (), "text", 1);
			nvDigitalPins.AppendColumn ("Color", new Gtk.CellRendererPixbuf (), "pixbuf", 2);
			nvDigitalPins.AppendColumn ("Seqeuence", new Gtk.CellRendererText (), "text", 3);

			nvDigitalPins.Show ();
			#endregion

			#region Measurment
			nvAnalogPins.NodeStore = NodeStoreAnalogPins;
			nvAnalogPins.RowActivated += (o, args) =>
			{
				var pin = con.ControllerPins
					.Where (x => x.Type == PinType.ANALOG)
					.ToList () [((o as NodeView).NodeSelection.SelectedNode as APinTreeNode).Index];
				RunAddAPinDialog (pin as APin);
			};

			nvAnalogPins.AppendColumn ("Name", new Gtk.CellRendererText (), "text", 0);
			nvAnalogPins.AppendColumn ("Pin Number", new Gtk.CellRendererText (), "text", 1);
			nvAnalogPins.AppendColumn ("Color", new Gtk.CellRendererPixbuf (), "pixbuf", 2);
			nvAnalogPins.AppendColumn ("Slope", new Gtk.CellRendererText (), "text", 3);
			nvAnalogPins.AppendColumn ("Offset", new Gtk.CellRendererText (), "text", 4);
			nvAnalogPins.AppendColumn ("Unit", new Gtk.CellRendererText (), "text", 5);
			nvAnalogPins.AppendColumn ("Frequency", new Gtk.CellRendererText (), "text", 6);
			nvAnalogPins.AppendColumn ("Interval", new Gtk.CellRendererText (), "text", 7);
			nvAnalogPins.AppendColumn ("Combination", new Gtk.CellRendererText (), "text", 8);

			nvAnalogPins.Show ();
			#endregion

			#region Sequences

			nvSequences.NodeStore = NodeStoreSequences;
			nvSequences.RowActivated += (o, args) =>
			{
				var Seq = con.ControllerSequences [((o as NodeView).NodeSelection.SelectedNode as SequenceTreeNode).Index];
				RunSequenceDialog (Seq);
			};

			nvSequences.AppendColumn (new TreeViewColumn ("Sequence-Name", new CellRendererText (), "text", 0));
			nvSequences.AppendColumn (new TreeViewColumn ("Color", new CellRendererPixbuf (), "pixbuf", 1));
			nvSequences.AppendColumn (new TreeViewColumn ("Pin-Name", new CellRendererText (), "text", 2));
			nvSequences.AppendColumn (new TreeViewColumn ("Pin-Number", new CellRendererText (), "text", 3));
			nvSequences.AppendColumn (new TreeViewColumn ("Runtime", new CellRendererText (), "text", 4));
			nvSequences.AppendColumn (new TreeViewColumn ("Repetitions", new CellRendererText (), "text", 5));

			nvSequences.Show ();
			#endregion

			#region MeasurementCombinations
			nvMeasurementCombinations.NodeStore = NodeStoreMeasurementCombinations;
			nvMeasurementCombinations.RowActivated += (o, args) =>
			{
				var sig = con.ControllerMeasurementCombinations [((o as NodeView).NodeSelection.SelectedNode as MeasurementCombinationTreeNode).Index];
				RunSignalDialog (sig);
			};
			nvMeasurementCombinations.AppendColumn (new TreeViewColumn ("Name", new CellRendererText (), "text", 0));
			nvMeasurementCombinations.AppendColumn (new TreeViewColumn ("Color", new CellRendererPixbuf (), "pixbuf", 1));
			nvMeasurementCombinations.AppendColumn (new TreeViewColumn ("Pin-Name", new CellRendererText (), "text", 2));
			nvMeasurementCombinations.AppendColumn (new TreeViewColumn ("Pin-Number", new CellRendererText (), "text", 3));
			nvMeasurementCombinations.AppendColumn (new TreeViewColumn ("Frequency", new CellRendererText (), "text", 4));
			nvMeasurementCombinations.AppendColumn (new TreeViewColumn ("Interval", new CellRendererText (), "text", 5));
			nvMeasurementCombinations.AppendColumn (new TreeViewColumn ("Operation", new CellRendererText (), "text", 6));
			#endregion
		}

		private void BuildMenu ()
		{
			//TODO Add icons
			//TODO Add events
			MenuBar mbar = (this.UIManager.GetWidget ("/menubarMain") as MenuBar);

			//FileMenu
			Menu filemenu = new Menu ();
			MenuItem file = new MenuItem ("File");
			file.Submenu = filemenu;
			MenuItem newoption = new MenuItem ("New");
			MenuItem openoption = new MenuItem ("Open");
			MenuItem saveoption = new MenuItem ("Save");
			MenuItem saveasoption = new MenuItem ("Save as...");

			MenuItem exit = new MenuItem ("Exit");
			exit.Activated += (sender, e) => OnDeleteEvent (null, null);

			filemenu.Append (newoption);
			filemenu.Append (openoption);
			filemenu.Append (saveoption);
			filemenu.Append (saveasoption);
			filemenu.Append (new SeparatorMenuItem ());
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
					if (ArduinoController.SerialPortName != null)
					{
						if (ArduinoController.SerialPortName.Equals (s) && ArduinoController.IsConnected)
						{
							portname.Toggle ();
						}
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

		private void BuildSequencePreviewPlot ()
		{
			XAxis = new LinearAxis {
				Key = "X",
				Position = AxisPosition.Bottom,
				AbsoluteMinimum = TimeSpan.FromSeconds (0).Ticks,
				LabelFormatter = x =>
				{
					if (x <= TimeSpan.FromSeconds (0).Ticks)
					{
						return "Start";
					}
					return string.Format ("+{0}", TimeSpan.FromSeconds (x).ToString ("g"));
				},
				MajorGridlineThickness = 1,
				MajorGridlineStyle = LineStyle.Solid,
				MinorGridlineColor = OxyColors.LightGray,
				MinorGridlineStyle = LineStyle.Dot,
				MinorGridlineThickness = .5,
			};

			var YAxis = new LinearAxis {
				Position = AxisPosition.Left,
				Minimum = -0.1,
				Maximum = 1.1,
				LabelFormatter = x => ((int)x == 0) ? "LOW" : "HIGH",
				IsPanEnabled = false,
				IsZoomEnabled = false,
				AbsoluteMaximum = 1.1,
				AbsoluteMinimum = -0.1,
				MinorStep = 1,
				MajorStep = 1,
			};

			SequencePreviewPlotModel = new PlotModel {
				PlotType = PlotType.XY,
				Background = OxyPlot.OxyColors.White,
			};
			SequencePreviewPlotModel.Axes.Add (YAxis);
			SequencePreviewPlotModel.Axes.Add (XAxis);
			SequencePreviewPlotView = new PlotView (){ Name = "", Model = SequencePreviewPlotModel  };

			vpanedSequences.Pack2 (SequencePreviewPlotView, true, true);


			SequencePreviewPlotView.SetSizeRequest (hbSequences.Allocation.Width, fSequences.Allocation.Height / 2);
			vpanedSequences.Position = fSequences.Allocation.Height / 2;


			SequencePreviewPlotView.ShowAll ();
		}

		#endregion

		#region Events

		protected void OnDeleteEvent (object obj, DeleteEventArgs a)
		{
			con.Quit ();
			ArduinoController.Exit ();
			Application.Quit ();
		}

		protected void OnKeyPressEvent (object obj, KeyPressEventArgs a)
		{
			//TODO Speichern und so einbauen
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
//			if (ArduinoController.IsConnected)
//			{
//				con.ControlSequences.Clear (); 
//				var scheduler = new Scheduler ();
//				con.AddScheduler (scheduler);
//
//				var dpin = new DPin ("D13", 13);
//				var sequence = new Sequence () {
//					Pin = dpin,
//					Repetitions = 10
//				};
//				sequence.AddSequenceOperation (new SequenceOperation () {
//					Duration = TimeSpan.FromSeconds (1),
//					State = DPinState.HIGH,
//				});
//				sequence.AddSequenceOperation (new SequenceOperation {
//					Duration = TimeSpan.FromSeconds (1),
//					State = DPinState.LOW,
//				});
//				con.ControlSequences.Add (sequence);
//
//				con.Start ();
//			} else
//			{
//				MessageDialog dialog = new MessageDialog (this, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, "Please connect first to a Arduino.");
//				dialog.Close += (senderer, ee) => dialog.Dispose ();
//				dialog.ShowNow ();
//			}
		}

		protected void OnBtnDoubleBlinkClicked (object sender, EventArgs e)
		{
			if (ArduinoController.IsConnected)
			{
				con.ControllerSequences.Clear ();

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
				con.ControllerSequences.Add (sequence);

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
				con.ControllerSequences.Add (sequence);

				foreach (Sequence seq in con.ControllerSequences)
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
			RunAddDPinDialog ();
		}

		protected void OnBtnAddAPinClicked (object sender, EventArgs e)
		{
			RunAddAPinDialog ();
		}

		protected void OnBtnEditDPinClicked (object sender, EventArgs e)
		{
			DPinTreeNode node = (DPinTreeNode)nvDigitalPins.NodeSelection.SelectedNode;
			if (node != null)
			{
				var pin = con.ControllerPins
					.Where (x => x.Type == PinType.DIGITAL)
					.ToList () [(nvDigitalPins.NodeSelection.SelectedNode as DPinTreeNode).Index];
				RunAddDPinDialog (pin as DPin);
			}
		}

		protected void OnBtnEditAPinClicked (object sender, EventArgs e)
		{
			APinTreeNode node = (APinTreeNode)nvAnalogPins.NodeSelection.SelectedNode;
			if (node != null)
			{
				var pin = con.ControllerPins
					.Where (x => x.Type == PinType.ANALOG)
					.ToList () [(node).Index];
				RunAddAPinDialog (pin as APin);
			}
		}

		protected void OnBtnEditSequenceClicked (object sender, EventArgs e)
		{
			SequenceTreeNode node = (SequenceTreeNode)nvSequences.NodeSelection.SelectedNode;
			if (node != null)
			{
				var seq = con.ControllerSequences [node.Index];
				RunSequenceDialog (seq);
			}
		}

		protected void OnBtnEditSignalClicked (object sender, EventArgs e)
		{
			MeasurementCombinationTreeNode node = (MeasurementCombinationTreeNode)nvMeasurementCombinations.NodeSelection.SelectedNode;
			if (node != null)
			{
				var seq = con.ControllerMeasurementCombinations [node.Index];
				RunSignalDialog (seq);
			}
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

		protected void OnBtnRemoveAPinClicked (object sender, EventArgs e)
		{
			APinTreeNode node = (APinTreeNode)nvAnalogPins.NodeSelection.SelectedNode;
			if (node != null)
			{
				con.RemovePin (node.Pin.Name);
			}
		}

		protected void OnBtnRemoveSignalClicked (object sender, EventArgs e)
		{
			MeasurementCombinationTreeNode node = (MeasurementCombinationTreeNode)nvMeasurementCombinations.NodeSelection.SelectedNode;
			if (node != null)
			{
				con.RemoveMeasurementCombination (node.Index);
			}
		}

		protected void OnBtnRemoveSequenceClicked (object sender, EventArgs e)
		{
			SequenceTreeNode node = (SequenceTreeNode)nvSequences.NodeSelection.SelectedNode;
			if (node != null)
			{
				con.RemoveSequence (node.Index);
			}
		}

		protected void OnBtnClearSignalsClicked (object sender, EventArgs e)
		{
			con.ClearMeasurementCombinations ();
		}

		protected void OnBtnClearAPinsClicked (object sender, EventArgs e)
		{
			con.ClearPins (PinType.ANALOG);
		}

		protected void OnBtnClearSequenceClicked (object sender, EventArgs e)
		{
			con.ClearSequences ();
		}

		protected void OnBtnAddSignalClicked (object sender, EventArgs e)
		{
			RunSignalDialog ();
		}

		protected void OnBtnAddSequenceClicked (object sender, EventArgs e)
		{
			RunSequenceDialog ();
		}

		protected void OnBtnStartControllerClicked (object sender, EventArgs e)
		{
			if (con.IsRunning)
			{
				con.Stop ();	
			} else
			{
				con.Start ();
			}
		}

		protected void OnMediaPlayActionToggled (object sender, EventArgs e)
		{
			StartStopController ();
		}

		protected void OnMediaStopActionActivated (object sender, EventArgs e)
		{
			StartStopController ();
		}

		#endregion

		#region RunDialogs

		private void RunAddDPinDialog (DPin pin = null)
		{
			int[] dings = con.AvailableDigitalPins;

			var dialog = new DigitalPinConfigurationDialog.DigitalPinConfiguration (dings, pin, this);
			dialog.Response += (o, args) =>
			{
				if (args.ResponseId == ResponseType.Apply)
				{
					if (pin == null)
					{
						con.AddPin (dialog.Pin);
					} else
					{
						for (int i = 0; i < con.ControllerPins.Count; i++)
						{
							if (con.ControllerPins [i] == pin)
							{
								con.SetPin (i, dialog.Pin);
								break;
							}
						}
					}
				}
			};
			dialog.Run ();
			dialog.Destroy ();	
		}

		private void RunAddAPinDialog (APin pin = null)
		{
			int[] dings = con.AvailableAnalogPins;

			var dialog = new AnalogPinConfigurationDialog.AnalogPinConfiguration (dings, pin, this);
			dialog.Response += (o, args) =>
			{
				if (args.ResponseId == ResponseType.Apply)
				{
					if (pin == null)
					{
						con.AddPin (dialog.Pin);
					} else
					{
						for (int i = 0; i < con.ControllerPins.Count; i++)
						{
							if (con.ControllerPins [i] == pin)
							{
								con.SetPin (i, dialog.Pin);
								break;
							}
						}
					}
				}
			};
			dialog.Run ();
			dialog.Destroy ();
		}

		private void RunSequenceDialog (Sequence seq = null, DPin RefPin = null)
		{
			var dialog = new SequenceConfigurationsDialog.SequenceConfiguration (con.GetDPinsWithoutSequence (), seq, RefPin, this);
			dialog.Response += (o, args) =>
			{
				if (args.ResponseId == ResponseType.Apply)
				{
					if (seq == null)
					{
						con.AddSequence (dialog.PinSequence);
					} else
					{
						con.SetSequence (con.ControllerSequences.IndexOf (seq), dialog.PinSequence);
					}
				}
			};
			dialog.Run ();
			dialog.Destroy ();
		}

		private void RunSignalDialog (MeasurementCombination sig = null, APin refPin = null)
		{
			var dialog = new SignalConfigurationDialog.MeasurementCombinationDialog (con.GetApinsWithoutCombination (), sig, refPin, this);
			dialog.Response += (o, args) =>
			{
				if (args.ResponseId == ResponseType.Apply)
				{
					if (sig == null)
					{
						con.AddMeasurementCombination (dialog.Combination);
					} else
					{
						con.SetMeasurmentCombination (con.ControllerMeasurementCombinations.IndexOf (sig), dialog.Combination);
					}
				}
			};
			dialog.Run ();
			dialog.Destroy ();
		}

		protected void RunMCUMessageDialog (object sender = null, EventArgs e = null)
		{
			var dialog = new MessageDialog (
				             this, 
				             DialogFlags.Modal, 
				             MessageType.Info, 
				             ButtonsType.YesNo, 
				             "Apparently a connection was established to a Board, which does not meet the selected Board by you.\nDo you want to replace you selection with the connected Board?");
			dialog.Response += (o, args) =>
			{
				if (args.ResponseId == ResponseType.Yes)
				{
					mcuW.Select (ArduinoController.MCU);
				}
			};
			dialog.Run ();
			dialog.Destroy ();
		}

		#endregion

		private void StartStopController ()
		{
			if (con.IsRunning)
			{
				con.Stop ();
				TimeKeeperPresenter.Stop ();
			} else
			{
				con.Start ();
				lblStartTime.Text = con.StartTime.ToString ("G");
//				TimeKeeperPresenter.Start ();
			}
		}

		protected void LockControlls (bool sensitive)
		{
			btnAddAPin.Sensitive = sensitive;
			btnAddDPin.Sensitive = sensitive;
			btnAddSequence.Sensitive = sensitive;
			btnAddSignal.Sensitive = sensitive;
			btnEditAPin.Sensitive = sensitive;
			btnEditDPin.Sensitive = sensitive;
			btnEditSequence.Sensitive = sensitive;
			btnEditSignal.Sensitive = sensitive;
			btnRemoveAPin.Sensitive = sensitive;
			btnRemoveDPin.Sensitive = sensitive;
			btnRemoveSequence.Sensitive = sensitive;
			btnRemoveSignal.Sensitive = sensitive;
			btnClearAPins.Sensitive = sensitive;
			btnClearDPins.Sensitive = sensitive;
			btnClearSequence.Sensitive = sensitive;
			btnClearSignals.Sensitive = sensitive;

			nvAnalogPins.Sensitive = sensitive;
			nvDigitalPins.Sensitive = sensitive;
			nvSequences.Sensitive = sensitive;
			nvMeasurementCombinations.Sensitive = sensitive;

			SequencePreviewPlotView.Sensitive = sensitive;
		}

		#region DEBUGHelperly

		protected void OnBtnFillAnalogInputsClicked (object sender, EventArgs e)
		{
			foreach (int i in con.AvailableAnalogPins)
			{
				con.AddPin (new APin () {
					Number = i,
					PlotColor = ColorHelper.GetRandomGdkColor ()
				});
			}
		}

		protected void OnBtnFillDigitalOutputsClicked (object sender, EventArgs e)
		{
			foreach (int i in  con.AvailableDigitalPins)
			{
				con.AddPin (new DPin () {
					Number = i,
					PlotColor = ColorHelper.GetRandomGdkColor ()
				});
			}
		}

		protected void OnBtnAlternateBlinkSetupClicked (object sender, EventArgs e)
		{
			con.ClearPins ();
			con.ClearSequences ();

			OnBtnFillDigitalOutputsClicked (null, null);

			int i = 0;
			while (i < con.ControllerPins.Count)
			{
				con.AddSequence (new Sequence () {
					Pin = (DPin)con.ControllerPins [i],
					Repetitions = -1,
					Chain = new System.Collections.Generic.List<SequenceOperation> () {
						new SequenceOperation () {
							Duration = TimeSpan.FromMilliseconds (1000),
							State = DPinState.HIGH
						},
						new SequenceOperation () {
							Duration = TimeSpan.FromMilliseconds (1000),
							State = DPinState.LOW
						}
					}
				});
				con.AddSequence (new Sequence () {
					Pin = (DPin)con.ControllerPins [i + 1],
					Repetitions = -1,
					Chain = new System.Collections.Generic.List<SequenceOperation> () {
						new SequenceOperation () {
							Duration = TimeSpan.FromMilliseconds (1000),
							State = DPinState.LOW
						},
						new SequenceOperation () {
							Duration = TimeSpan.FromMilliseconds (1000),
							State = DPinState.HIGH
						}
					}
				});
				i += 2;
			}
		}


		protected void OnBtnAlternateBlinkSetup2Clicked (object sender, EventArgs e)
		{
			con.ClearPins ();
			con.ClearSequences ();

			OnBtnFillDigitalOutputsClicked (null, null);

			int i = 0;
			while (i < con.ControllerPins.Count)
			{
				var seq = new Sequence () {
					Pin = (DPin)con.ControllerPins [i],
					Repetitions = 0,
				};
				seq.Chain.Add (new SequenceOperation () {
					Duration = TimeSpan.FromSeconds (i),
					State = DPinState.LOW
				});

				for (int j = 0; j < 50; j++)
				{
					seq.Chain.Add (new SequenceOperation () {
						Duration = TimeSpan.FromSeconds (con.ControllerPins.Count),
						State = DPinState.HIGH
					});
					seq.Chain.Add (new SequenceOperation () {
						Duration = TimeSpan.FromSeconds (con.ControllerPins.Count),
						State = DPinState.LOW
					});
				}

				con.ControllerSequences.Add (seq);

				i += 1;
			}
		}

		#endregion
	}
}
