using System;
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
using System.Collections.Generic;

namespace Frontend
{
	public partial class MainWindow : Gtk.Window
	{

		#region Member

		Controller con;

		private Gtk.NodeStore NodeStoreDigitalPins = new NodeStore (typeof(DPinTreeNode));
		private Gtk.NodeStore NodeStoreAnalogPins = new NodeStore (typeof(APinTreeNode));
		private Gtk.NodeStore NodeStoreSequences = new NodeStore (typeof(SequenceTreeNode));
		private Gtk.NodeStore NodeStoreMeasurementCombinations = new NodeStore (typeof(MeasurementCombinationTreeNode));

		private PlotView SequencePreviewPlotView;
		private PlotModel SequencePreviewPlotModel;
		private LinearAxis XAxis;

		private PlotView RealTimePlotView;
		private PlotModel RealTimePlotModel;
		private LinearAxis RealTimeXAxis;
		private Dictionary<string,Collection<DateTimeValue>> RealTimeDictionary;

		private System.Timers.Timer TimeKeeperPresenter;

		public MainWindow (Controller controller = null) :
			base (Gtk.WindowType.Toplevel)
		{
			this.Build ();

			if (controller != null)
			{
				con = controller;
			} else
			{
				con = new Controller ();
			}

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
					autoConnectAction.Sensitive = false;
					mediaPlayAction.Sensitive = true;
					mediaStopAction.Sensitive = true;
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
							this.con.Configuration.Board = Array.Find (con.BoardConfigs, o => o.MCU == ArduinoController.MCU);
						} else
						{
							if (mcuW.SelectedBoard.MCU != con.Configuration.Board.MCU && con.Configuration.Board != null)
							{
								Application.Invoke (RunMCUMessageDialog);
							}
						}
					}
				} else
				{
					lblConnectionStatus.Text = "<b>NOT</b> connected";
					lblConnectionStatus.UseMarkup = true;
					autoConnectAction.Sensitive = true;
					mediaPlayAction.Sensitive = false;
					mediaStopAction.Sensitive = false;
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

			mcuW.OnBoardSelected += EnableConfig;


			BuildMenu ();
			BuildNodeViews ();
			BuildMCUWidget ();
			BuildSequencePreviewPlot ();
			BuildRealTimePlot ();

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

			con.Configuration.OnPinsUpdated += (o, a) =>
			{
				if (a.Pin is DPin)
				{
					FillDigitalPinNodes ();
					FillSequencePreviewPlot ();
					FillMeasurementCombinationNodes ();
				} else if (a.Pin is APin)
				{
					FillAnalogPinNodes ();
					FillMeasurementCombinationNodes ();
				} else
				{
					FillAnalogPinNodes ();
					FillDigitalPinNodes ();
					FillSequenceNodes ();
					FillMeasurementCombinationNodes ();
				}
			};
			con.Configuration.OnSequencesUpdated += (o, a) =>
			{
				FillSequenceNodes ();
				FillSequencePreviewPlot ();
			};
			con.Configuration.OnSignalsUpdated += (o, a) => FillMeasurementCombinationNodes ();

			con.OnControllerStarted += (o, a) => LockControlls (false);
			con.OnControllerStoped += (o, a) => LockControlls (true);

			nvDigitalPins.ButtonPressEvent += new ButtonPressEventHandler (OnDigitalPinNodePressed);
			nvSequences.ButtonPressEvent += new ButtonPressEventHandler (OnItemButtonPressed);
			nvMeasurementCombinations.ButtonPressEvent += new ButtonPressEventHandler (OnItemButtonPressed);
			nvAnalogPins.ButtonPressEvent += new ButtonPressEventHandler (OnAnalogPinNodePressed);

			TimeKeeperPresenter = new System.Timers.Timer (1000);
			TimeKeeperPresenter.Elapsed += (sender, e) =>
			{
				lblTimePassed.Text = string.Format ("{0:c}", con.TimePassed); 
				UpdateRealTimePlot ();
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
						con.Configuration.RemovePin ((node as DPinTreeNode).Index);
					} else if (node is SequenceTreeNode)
					{
						con.Configuration.RemoveSequence ((node as SequenceTreeNode).Index);
					} else if (node is APinTreeNode)
					{
						con.Configuration.RemovePin ((node as APinTreeNode).Index);
					} else if (node is MeasurementCombinationTreeNode)
					{
						con.Configuration.RemoveMeasurementCombination ((node as MeasurementCombinationTreeNode).Index);
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
						con.Configuration.RemovePin (pin.Index);

					MenuItem addSignal = new MenuItem ("Add new Signal");
					MenuItem editSignal = new MenuItem ("Edit Signal");
					if (pin.Combination == null)
					{
						editSignal.Sensitive = false;
						addSignal.ButtonPressEvent += (o, args) => RunMeasurementCombinationDialog (null, pin.Pin);
					} else
					{
						addSignal.Sensitive = false;
						editSignal.ButtonPressEvent += (o, args) => RunMeasurementCombinationDialog (pin.Combination);
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
						con.Configuration.RemovePin (pin.Index);

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
			foreach (IPin pin in con.Configuration.Pins)
			{
				if (pin.Type == PinType.DIGITAL)
				{
					NodeStoreDigitalPins.AddNode (new DPinTreeNode (pin as DPin, index, con.Configuration.GetCorespondingSequence (pin as DPin)));
					index++;
				}
			}
			nvDigitalPins.QueueDraw ();
		}

		private void FillAnalogPinNodes ()
		{
			NodeStoreAnalogPins.Clear ();
			int index = 0;
			foreach (IPin pin in con.Configuration.Pins)
			{
				if (pin.Type == PinType.ANALOG)
				{
					NodeStoreAnalogPins.AddNode (new APinTreeNode (pin as APin, index, con.Configuration.GetCorespondingCombination (pin as APin)));
					index++;
				}
			}
			nvAnalogPins.QueueDraw ();
		}

		private void FillSequenceNodes ()
		{
			FillDigitalPinNodes ();
			NodeStoreSequences.Clear ();
			for (int i = 0; i < con.Configuration.Sequences.Count; i++)
			{
				NodeStoreSequences.AddNode (new SequenceTreeNode (con.Configuration.Sequences [i], i));
			}
			nvSequences.QueueDraw ();
		}

		private void FillMeasurementCombinationNodes ()
		{
			FillAnalogPinNodes ();
			NodeStoreMeasurementCombinations.Clear ();
			for (int i = 0; i < con.Configuration.MeasurementCombinations.Count; i++)
			{
				NodeStoreMeasurementCombinations.AddNode (new MeasurementCombinationTreeNode (con.Configuration.MeasurementCombinations [i], i));
			}
			nvMeasurementCombinations.QueueDraw ();
		}

		private void FillSequencePreviewPlot ()
		{
			SequencePreviewPlotModel.Axes.Clear ();
			SequencePreviewPlotModel.Series.Clear ();
			SequencePreviewPlotModel.Axes.Add (XAxis);

			double size = 1 / (double)con.Configuration.Sequences.Count;
			double startPos = 1;

			for (int i = 0; i < con.Configuration.Sequences.Count; i++)
			{
				var seq = con.Configuration.Sequences [i];

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
				var data = new Collection<TimeValue> ();
				var current = new TimeSpan (0);
				for (int j = 0; j < seq.Chain.Count; j++)
				{
					data.Add (new TimeValue () {
						Time = current,
						Value = ((seq.Chain [j].State == DPinState.HIGH) ? 1 : 0)
					});
					current = current.Add (seq.Chain [j].Duration);

					data.Add (new TimeValue () {
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

		private void UpdateRealTimePlot ()
		{
			try
			{
				foreach (APin a in con.Configuration.AnalogPins)
				{
					var values = a.Values;
					values = values.OrderByDescending (o => o.Time).ToList ();
					foreach (DateTimeValue dtv in values)
					{
						if (RealTimeDictionary [a.DisplayName].Contains (dtv))
						{
							break;
						}
						RealTimeDictionary [a.DisplayName].Add (dtv);
					}
					RealTimePlotView.Model.Series.Clear ();
					RealTimePlotView.InvalidatePlot (true);
					RealTimePlotView.Model.Series.Add (
						new LineSeries () {
							Color = ColorHelper.GdkColorToOxyColor (a.PlotColor),
							DataFieldX = "Time",
							DataFieldY = "Value",
							ItemsSource = RealTimeDictionary [a.DisplayName],
							Title = a.DisplayName
						}
					);
				}

				double step = RealTimeXAxis.Transform ((-1 * 1) + RealTimeXAxis.Offset);
				RealTimeXAxis.Pan (step);


				RealTimePlotView.InvalidatePlot (true);
			} catch (Exception ex)
			{
				con.ConLogger.Log (ex.ToString (), LogLevel.DEBUG);
			}
		}

		#endregion

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
				var pin = con.Configuration.Pins
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
				var pin = con.Configuration.Pins
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
				var Seq = con.Configuration.Sequences [((o as NodeView).NodeSelection.SelectedNode as SequenceTreeNode).Index];
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
				var sig = con.Configuration.MeasurementCombinations [((o as NodeView).NodeSelection.SelectedNode as MeasurementCombinationTreeNode).Index];
				RunMeasurementCombinationDialog (sig);
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

			#region FileMenu
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
			#endregion

			#region Edit
			Menu editmenu = new Menu ();
			MenuItem edit = new MenuItem ("Edit");
			edit.Submenu = editmenu;
			MenuItem preferences = new MenuItem ("Preferences");
			preferences.Activated += (o, e) => RunPreferencesDialog ();
			editmenu.Append (preferences);

			mbar.Append (edit);
			#endregion

			#region ConnectionMenu
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
			#endregion

			#region Help
			Menu helpmenu = new Menu ();
			MenuItem help = new MenuItem ("Help");
			help.Submenu = helpmenu;
			MenuItem about = new MenuItem ("About");
			about.Activated += (sender, e) =>
			{
				var dialog = new AboutDialog () {
					Authors = new string[]{ "Daniel Pollack" },
					Documenters = new string[]{ "Daniel Pollack" },
					License = "not yet"
				};
				dialog.Run ();
				dialog.Destroy ();
			};

			helpmenu.Append (about);

			mbar.Append (help);
			#endregion

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

		private void BuildRealTimePlot ()
		{
			RealTimeXAxis = new LinearAxis {
				Key = "X",
				Position = AxisPosition.Bottom,
				LabelFormatter = x =>
				{
					if (con != null && x == con.StartTime.Ticks)
					{
						return string.Format ("Start\n{0}", DateTime.FromOADate (x).ToString ("g"));
					}
					return string.Format ("{0}", DateTime.FromOADate (x).ToString ("g"));
				},
				MajorGridlineThickness = 1,
				MajorGridlineStyle = LineStyle.Solid,
				MinorGridlineColor = OxyColors.LightGray,
				MinorGridlineStyle = LineStyle.Dot,
				MinorGridlineThickness = .5,
				MinorStep = TimeSpan.FromSeconds (1).Ticks,
//				MajorStep = TimeSpan.FromSeconds (30).Ticks
			};

			var YAxis = new LinearAxis {
				Position = AxisPosition.Left,
				IsPanEnabled = false,
				IsZoomEnabled = false,
				MajorGridlineThickness = 1,
				MajorGridlineStyle = LineStyle.Solid,
				MinorGridlineColor = OxyColors.LightGray,
				MinorGridlineStyle = LineStyle.Dot,
				MinorGridlineThickness = .5,
			};

			RealTimePlotModel = new PlotModel {
				PlotType = PlotType.XY,
				Background = OxyPlot.OxyColors.White,
				IsLegendVisible = true,
				LegendOrientation = LegendOrientation.Horizontal,
				LegendPlacement = LegendPlacement.Outside,
				LegendPosition = LegendPosition.RightMiddle
			};

			RealTimePlotModel.Axes.Add (YAxis);
			RealTimePlotModel.Axes.Add (RealTimeXAxis);
			RealTimePlotView = new PlotView (){ Name = "", Model = RealTimePlotModel  };

			vboxRealTimePlot.PackStart (RealTimePlotView, true, true, 0);
			(vboxRealTimePlot [RealTimePlotView] as Box.BoxChild).Position = 0;

			RealTimePlotView.SetSizeRequest (hbSequences.Allocation.Width, fSequences.Allocation.Height / 2);
			vpanedSequences.Position = fSequences.Allocation.Height / 2;

			RealTimePlotView.ShowAll ();	
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
				for (uint i = 0; i < ArduinoController.NumberOfDigitalPins; i++)
				{
					ArduinoController.SetPin (i, PinMode.OUTPUT, DPinState.HIGH);
					await Task.Delay (500);
				}
				await Task.Delay (2000);
				for (uint i = 0; i < ArduinoController.NumberOfDigitalPins; i++)
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
				con.Configuration.Sequences.Clear ();

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
				con.Configuration.Sequences.Add (sequence);

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
				con.Configuration.Sequences.Add (sequence);

				foreach (Sequence seq in con.Configuration.Sequences)
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
			for (uint i = 0; i < ArduinoController.NumberOfDigitalPins; i++)
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
				var pin = con.Configuration.Pins
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
				var pin = con.Configuration.Pins
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
				var seq = con.Configuration.Sequences [node.Index];
				RunSequenceDialog (seq);
			}
		}

		protected void OnBtnEditSignalClicked (object sender, EventArgs e)
		{
			MeasurementCombinationTreeNode node = (MeasurementCombinationTreeNode)nvMeasurementCombinations.NodeSelection.SelectedNode;
			if (node != null)
			{
				var seq = con.Configuration.MeasurementCombinations [node.Index];
				RunMeasurementCombinationDialog (seq);
			}
		}

		protected void OnBtnClearDPinsClicked (object sender, EventArgs e)
		{
			con.Configuration.ClearPins (PinType.DIGITAL);
		}

		protected void OnBtnRemoveDPinClicked (object sender, EventArgs e)
		{
			DPinTreeNode node = (DPinTreeNode)nvDigitalPins.NodeSelection.SelectedNode;
			if (node != null)
			{
				con.Configuration.RemovePin (node.Pin.Name);
			}
		}

		protected void OnBtnRemoveAPinClicked (object sender, EventArgs e)
		{
			APinTreeNode node = (APinTreeNode)nvAnalogPins.NodeSelection.SelectedNode;
			if (node != null)
			{
				con.Configuration.RemovePin (node.Pin.Name);
			}
		}

		protected void OnBtnRemoveSignalClicked (object sender, EventArgs e)
		{
			MeasurementCombinationTreeNode node = (MeasurementCombinationTreeNode)nvMeasurementCombinations.NodeSelection.SelectedNode;
			if (node != null)
			{
				con.Configuration.RemoveMeasurementCombination (node.Index);
			}
		}

		protected void OnBtnRemoveSequenceClicked (object sender, EventArgs e)
		{
			SequenceTreeNode node = (SequenceTreeNode)nvSequences.NodeSelection.SelectedNode;
			if (node != null)
			{
				con.Configuration.RemoveSequence (node.Index);
			}
		}

		protected void OnBtnClearSignalsClicked (object sender, EventArgs e)
		{
			con.Configuration.ClearMeasurementCombinations ();
		}

		protected void OnBtnClearAPinsClicked (object sender, EventArgs e)
		{
			con.Configuration.ClearPins (PinType.ANALOG);
		}

		protected void OnBtnClearSequenceClicked (object sender, EventArgs e)
		{
			con.Configuration.ClearSequences ();
		}

		protected void OnBtnAddSignalClicked (object sender, EventArgs e)
		{
			RunMeasurementCombinationDialog ();
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

		protected void OnAutoConnectActionActivated (object sender, EventArgs e)
		{
			Task.Run (() =>
			{
				if (!ArduinoController.AttemdAutoConnect ())
				{
					Application.Invoke ((o, ee) =>
					{
						var dialog = new MessageDialog (
							             this, 
							             DialogFlags.Modal, 
							             MessageType.Info, 
							             ButtonsType.Ok, 
							             "The attemd to automaticly connect to a controller failed.\n " +
							             "Please make shure to have a controller connected and uploaded " +
							             "with the provided software."
						             );
						dialog.Run ();
						dialog.Destroy ();
					});
				}
			});
		}

		protected void EnableConfig (object sender, BoardSelectionArgs e)
		{
			//TODO englisch prüfen
			if (e != null)
			{
				notebook1.Foreach (o => o.Sensitive = true);
				if (e.Board != con.Configuration.Board && ArduinoController.IsConnected)
				{
					var dialog = new MessageDialog (
						             this,
						             DialogFlags.Modal,
						             MessageType.Question,
						             ButtonsType.YesNo,
						             "The selected Board Type does not match the connected one. Do you wish to change the selected Board Type to the detected Board Type?\n" +
						             "Beware: This may alter you configuration!"
					             );
					dialog.Response += (o, args) =>
					{
						if (args.ResponseId == ResponseType.Yes)
						{
							con.Configuration.Board = e.Board;
						}
					};
					dialog.Run ();
					dialog.Destroy ();
				} else
				{
					con.Configuration.Board = e.Board;
				}
			} else
			{
				notebook1.Foreach (o => o.Sensitive = false);
			}
		}

		#endregion

		#region RunDialogs

		private void RunAddDPinDialog (DPin pin = null)
		{
			var dings = con.Configuration.AvailableDigitalPins;

			var dialog = new DigitalPinConfigurationDialog.DigitalPinConfiguration (dings, pin, this);
			dialog.Response += (o, args) =>
			{
				if (args.ResponseId == ResponseType.Apply)
				{
					if (pin == null)
					{
						con.Configuration.AddPin (dialog.Pin);
					} else
					{
						for (int i = 0; i < con.Configuration.Pins.Count; i++)
						{
							if (con.Configuration.Pins [i] == pin)
							{
								con.Configuration.SetPin (i, dialog.Pin);
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
			var dings = con.Configuration.AvailableAnalogPins;

			var dialog = new AnalogPinConfigurationDialog.AnalogPinConfiguration (dings, pin, this);
			dialog.Response += (o, args) =>
			{
				if (args.ResponseId == ResponseType.Apply)
				{
					if (pin == null)
					{
						con.Configuration.AddPin (dialog.Pin);
					} else
					{
						for (int i = 0; i < con.Configuration.Pins.Count; i++)
						{
							if (con.Configuration.Pins [i] == pin)
							{
								con.Configuration.SetPin (i, dialog.Pin);
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
			var dialog = new SequenceConfigurationsDialog.SequenceConfiguration (con.Configuration.GetPinsWithoutSequence (), seq, RefPin, this);
			dialog.Response += (o, args) =>
			{
				if (args.ResponseId == ResponseType.Apply)
				{
					if (seq == null)
					{
						con.Configuration.AddSequence (dialog.PinSequence);
					} else
					{
						con.Configuration.SetSequence (con.Configuration.Sequences.IndexOf (seq), dialog.PinSequence);
					}
				}
			};
			dialog.Run ();
			dialog.Destroy ();
		}

		private void RunMeasurementCombinationDialog (MeasurementCombination sig = null, APin refPin = null)
		{
			var dialog = new SignalConfigurationDialog.MeasurementCombinationDialog (con.Configuration.GetPinsWithoutCombinations (), sig, refPin, this);
			dialog.Response += (o, args) =>
			{
				if (args.ResponseId == ResponseType.Apply)
				{
					if (sig == null)
					{
						con.Configuration.AddMeasurementCombination (dialog.Combination);
					} else
					{
						con.Configuration.SetMeasurmentCombination (con.Configuration.MeasurementCombinations.IndexOf (sig), dialog.Combination);
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

		protected  void RunPreferencesDialog (object sernder = null, EventArgs e = null)
		{
			var dialog = new PreferencesDialog.PreferencesDialog ();
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
				PrepareRealTimePlot ();
				TimeKeeperPresenter.Start ();
			}
		}

		protected void LockControlls (bool sensitive)
		{
			//TODO rekursiv machen
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

		private void PrepareRealTimePlot ()
		{
			if (RealTimeDictionary != null)
			{
				RealTimeDictionary.Clear ();
			} else
			{
				RealTimeDictionary = new Dictionary<string, Collection<DateTimeValue>> ();
			}
			foreach (APin a in con.Configuration.AnalogPins)
			{
				RealTimeDictionary.Add (a.DisplayName, new Collection<DateTimeValue> ());	
			}
		}

		#region DEBUGHelperly

		protected void OnBtnFillAnalogInputsClicked (object sender, EventArgs e)
		{
			var list = con.Configuration.AvailableAnalogPins.ToList ();
			foreach (APin pin in list)
			{
				pin.PlotColor = ColorHelper.GetRandomGdkColor ();
				con.Configuration.AddPin (pin);
			}
		}

		protected void OnBtnFillDigitalOutputsClicked (object sender, EventArgs e)
		{
			foreach (DPin i in  con.Configuration.AvailableDigitalPins.ToList())
			{
				i.PlotColor = ColorHelper.GetRandomGdkColor ();
				con.Configuration.AddPin (i);
			}
		}

		protected void OnBtnAlternateBlinkSetupClicked (object sender, EventArgs e)
		{
			con.Configuration.ClearPins ();
			con.Configuration.ClearSequences ();

			OnBtnFillDigitalOutputsClicked (null, null);

			int i = 0;
			while (i < con.Configuration.Pins.Count)
			{
				var seq1 = new Sequence () {
					Pin = (DPin)con.Configuration.Pins [i],
					Repetitions = 0
				};
				var seq2 = new Sequence () {
					Pin = (DPin)con.Configuration.Pins [i + 1],
					Repetitions = 0
				};
				for (int j = 0; j < 100; j++)
				{
					
					seq1.Chain.Add (new SequenceOperation () {
						Duration = TimeSpan.FromMilliseconds (1000),
						State = DPinState.HIGH
					});
					seq1.Chain.Add (new SequenceOperation () {
						Duration = TimeSpan.FromMilliseconds (1000),
						State = DPinState.LOW
					});
					seq2.Chain.Add (new SequenceOperation () {
						Duration = TimeSpan.FromMilliseconds (1000),
						State = DPinState.LOW
					});
					seq2.Chain.Add (new SequenceOperation () {
						Duration = TimeSpan.FromMilliseconds (1000),
						State = DPinState.HIGH
					});
				}
				con.Configuration.AddSequence (seq1);
				con.Configuration.AddSequence (seq2);
				i += 2;
			}
		}

		protected void OnBtnAlternateBlinkSetup2Clicked (object sender, EventArgs e)
		{
			con.Configuration.ClearPins ();
			con.Configuration.ClearSequences ();

			OnBtnFillDigitalOutputsClicked (null, null);

			int i = 0;
			while (i < con.Configuration.Pins.Count)
			{
				var seq = new Sequence () {
					Pin = (DPin)con.Configuration.Pins [i],
					Repetitions = 0,
				};
				seq.Chain.Add (new SequenceOperation () {
					Duration = TimeSpan.FromSeconds (i / 100.0),
					State = DPinState.LOW
				});

				for (int j = 0; j < 100; j++)
				{
					seq.Chain.Add (new SequenceOperation () {
						Duration = TimeSpan.FromSeconds (con.Configuration.Pins.Count / 100.0),
						State = DPinState.HIGH
					});
					seq.Chain.Add (new SequenceOperation () {
						Duration = TimeSpan.FromSeconds (con.Configuration.Pins.Count / 100.0),
						State = DPinState.LOW
					});
				}

				con.Configuration.AddSequence (seq);

				i += 1;
			}
		}


		protected void OnButton359Clicked (object sender, EventArgs e)
		{
			con.Configuration.ClearPins ();
			con.Configuration.ClearSequences ();

			OnBtnFillDigitalOutputsClicked (null, null);

			int i = 0;
			while (i < con.Configuration.Pins.Count)
			{
				var seq1 = new Sequence () {
					Pin = (DPin)con.Configuration.Pins [i],
					Repetitions = -1
				};
				var seq2 = new Sequence () {
					Pin = (DPin)con.Configuration.Pins [i + 1],
					Repetitions = -1
				};
				for (int j = 0; j < 1; j++)
				{
					
					seq1.Chain.Add (new SequenceOperation () {
						Duration = TimeSpan.FromMilliseconds (1000),
						State = DPinState.HIGH
					});
					seq1.Chain.Add (new SequenceOperation () {
						Duration = TimeSpan.FromMilliseconds (1000),
						State = DPinState.LOW
					});
					seq2.Chain.Add (new SequenceOperation () {
						Duration = TimeSpan.FromMilliseconds (1000),
						State = DPinState.LOW
					});
					seq2.Chain.Add (new SequenceOperation () {
						Duration = TimeSpan.FromMilliseconds (1000),
						State = DPinState.HIGH
					});
				}
				con.Configuration.AddSequence (seq1);
				con.Configuration.AddSequence (seq2);
				i += 2;
			}
		}

		protected void OnButton360Clicked (object sender, EventArgs e)
		{
			con.Configuration.ClearPins ();
			con.Configuration.ClearSequences ();

			OnBtnFillDigitalOutputsClicked (null, null);

			int i = 0;
			while (i < con.Configuration.Pins.Count)
			{
				var seq = new Sequence () {
					Pin = (DPin)con.Configuration.Pins [i],
					Repetitions = -1,
				};

				seq.Chain.Add (new SequenceOperation () {
					Duration = TimeSpan.FromSeconds (i / 100.0),
					State = DPinState.LOW
				});
				seq.Chain.Add (new SequenceOperation () {
					Duration = TimeSpan.FromSeconds (.2),
					State = DPinState.HIGH
				});
//				seq.Chain.Add (new SequenceOperation () {
//					Duration = TimeSpan.FromSeconds (.2),
//					State = DPinState.LOW
//				});
				seq.Chain.Add (new SequenceOperation () {
					Duration = TimeSpan.FromSeconds (.2 * (1 - (i / 100.0))),
					State = DPinState.LOW
				});

				con.Configuration.AddSequence (seq);

				i += 1;
			}
		}


		protected void OnButton1125Clicked (object sender, EventArgs e)
		{
			con.Configuration.ClearPins ();
			con.Configuration.ClearSequences ();

			OnBtnFillDigitalOutputsClicked (null, null);

			int i = 2;
			while (i < con.Configuration.Pins.Count)
			{
				var seq = new Sequence () {
					Pin = (DPin)con.Configuration.Pins [i],
					Repetitions = -1
				};

				seq.Chain.Add (new SequenceOperation {
					Duration = TimeSpan.FromMilliseconds (1000 + i),
					State = DPinState.HIGH
				});
				seq.Chain.Add (new SequenceOperation {
					Duration = TimeSpan.FromMilliseconds (1000 + i),
					State = DPinState.LOW
				});

				con.Configuration.AddSequence (seq);

				i++;
			}
		}

		protected void OnBtnBoardDifferenceTestClicked (object sender, EventArgs e)
		{
			con.Configuration.Board = con.BoardConfigs [1];
		}

		#endregion
	}
}
