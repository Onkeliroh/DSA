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
using Backend;
using System.Collections.Generic;
using System.Globalization;
using Cairo;
using System.Diagnostics;
using Gdk;

namespace Frontend
{
	public partial class MainWindow : Gtk.Window
	{
		#region Member

		/// <summary>
		/// The <see cref="Controller"/> instance.
		/// </summary>
		Controller con;

		private Gtk.NodeStore NodeStoreDigitalPins = new NodeStore (typeof(DPinTreeNode));
		private Gtk.NodeStore NodeStoreAnalogPins = new NodeStore (typeof(APinTreeNode));
		private Gtk.NodeStore NodeStoreSequences = new NodeStore (typeof(SequenceTreeNode));
		private Gtk.NodeStore NodeStoreMeasurementCombinations = new NodeStore (typeof(MeasurementCombinationTreeNode));

		private ComboBox PortBox;
		private ListStore PortBoxStore = new ListStore (typeof(string));

		private PlotView SequencePreviewPlotView;
		private PlotModel SequencePreviewPlotModel;
		private LinearAxis XAxis;

		private PlotView RealTimePlotView;
		private PlotModel RealTimePlotModel;
		private PlotController RealTimePlotController;
		private DateTimeAxis RealTimeXAxis;
		private bool RealTimePlotUpdate = true;
		private double DefaultZoomValue = 30;
		public List<string> Units = new List<string> ();

		/// <summary>
		/// A timer for keeping track of time after the measurement beginns.
		/// </summary>
		private System.Timers.Timer TimeKeeperPresenter;

		private double LastTimeKeeperPresenterTick = new DateTime (0).ToOADate ();

		public int LastActiveBoard = -1;

		public readonly bool Verbose = false;

		#endregion

		/// <summary>
		/// Initializes a new instance of the <see cref="Frontend.MainWindow"/> class.
		/// </summary>
		/// <param name="controller">Controller.</param>
		/// <param name="verbose">Verbose flag.</param>
		public MainWindow (Controller controller = null, bool verbose = false) :
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

			Verbose = verbose;

			InitComponents ();

			if (con.BoardConfigs.Length > 0)
			{
				if (con.BoardConfigs.ToList ().Count (o => o.MCU == "atmega328p") > 0)
				{
					con.Configuration.Board = con.BoardConfigs.ToList ().Single (o => o.MCU == "atmega328p");
				} else
				{
					con.Configuration.Board = con.BoardConfigs [0];
				}
			}

			if (!Frontend.Settings.Default.DebugMode)
			{
				this.notebook1.GetNthPage (6).Visible = false;
			}

			if (Frontend.Settings.Default.StartMaximized)
			{
				this.Maximize ();
			}

			if (Frontend.Settings.Default.LoadLastFile)
			{
//				con.LoadLastConfig ();
			}

			if (Frontend.Settings.Default.ConnectToLastPort)
			{
				con.ConnectToLastPort ();
			}
		}


		/// <summary>
		/// Inits the components.
		/// </summary>
		private void InitComponents ()
		{
			this.Title = Backend.Controller.SoftwareName + " - Main Window";

			Units = Backend.ConfigHelper.StringToStringList (Properties.Resources.Units);
			hpanedMain.Position = 380;

			ArduinoController.OnConnectionChanged += OnConnection;

			BuildMenu ();
			BuildToolBar ();
			BuildNodeViews ();
			BuildSequencePreviewPlot ();
			BuildRealTimePlot ();
			BuildMCUDisplay ();
			BuildConfigSettings ();
			BindControllerEvents ();
			BindWidgetEvents ();


			nvDigitalPins.ButtonPressEvent += new ButtonPressEventHandler (OnDigitalPinNodePressed);
			nvSequences.ButtonPressEvent += new ButtonPressEventHandler (OnSequeneceNodePressed);
			nvMeasurementCombinations.ButtonPressEvent += new ButtonPressEventHandler (OnMeasurementCombinationNodePressed);
			nvAnalogPins.ButtonPressEvent += new ButtonPressEventHandler (OnAnalogPinNodePressed);
			nvAnalogPins.ButtonPressEvent += (sender, o) => FillAnalogPinNodes ();
			nvDigitalPins.ButtonPressEvent += (sender, o) => FillDigitalPinNodes ();
			nvSequences.ButtonPressEvent += (sender, o) => FillSequenceNodes ();
			nvMeasurementCombinations.ButtonPressEvent += (sender, o) => FillMeasurementCombinationNodes ();
			drawingareaMCU.ExposeEvent += DrawMCU;
			cbBoardType.Changed += OnCbBoardTypeChanged;
			cbAREF.Changed += OnCbAREFChanged;
			btnCSVOpenDirectory.Clicked += (sender, e) => OpenCSVDirectory ();
			con.Configuration.OnPinsUpdated += (sender, o) => DrawMCU (this, null);
			con.Configuration.OnBoardUpdated += RefreshMCUInfos;
			con.OnOnfigurationLoaded += (sender, e) =>
			{
				UpdateSettings ();
				UpdateAllNodes ();
				UpdateFilePathPreview ();
				DrawMCU (this, null);
				ShowConfigurationLoadedMessage (e.Path, e.Success);
			};

			TimeKeeperPresenter = new System.Timers.Timer (1000);
			TimeKeeperPresenter.Elapsed += (sender, e) =>
			{
				UpdateRealTimePlot ();
				Application.Invoke ((o, args) =>
				{  
					lblTimePassed.Text = string.Format ("{0}d {1:D2}:{2:D2}:{3:D2}", (int)con.TimeElapsed.TotalDays, con.TimeElapsed.Hours, con.TimeElapsed.Minutes, con.TimeElapsed.Seconds);
					lblTimePassed.QueueDraw ();
				});
			};
		}

		public void SetGtkTheme (string theme)
		{
			Gtk.Rc.ParseString (theme);
		}

		#region NodeViewMouseActions

		/// <summary>
		/// Creates popupmenu. 
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		[GLib.ConnectBeforeAttribute]
		protected void OnAnalogPinNodePressed (object sender, ButtonPressEventArgs e)
		{
			APinTreeNode pin = (sender as NodeView).NodeSelection.SelectedNode as APinTreeNode;
			//right mouse button
			if (e.Event.Button == 3)
			{
				Menu m = new Menu ();

				var AddPin = new ImageMenuItem ("Add Measurement...");
				var ClonePin = new ImageMenuItem ("Duplicate");
				var EditPin = new ImageMenuItem ("Edit Measurement...");
				var RemovePin = new ImageMenuItem ("Delete Measruement");
				var ClearPins = new ImageMenuItem ("Clear Measurements");
				var AddCombination = new ImageMenuItem ("Add Combination...");
				var EditCombination = new ImageMenuItem ("Edit Combination...");

				AddPin.Image = new Gtk.Image (Gtk.Stock.Add, IconSize.Menu);
				ClonePin.Image = new Gtk.Image (Gtk.Stock.Copy, IconSize.Menu);
				EditPin.Image = new Gtk.Image (Gtk.Stock.Edit, IconSize.Menu);
				RemovePin.Image = new Gtk.Image (Gtk.Stock.Delete, IconSize.Menu);
				ClearPins.Image = new Gtk.Image (Gtk.Stock.Clear, IconSize.Menu);
				AddCombination.Image = new Gtk.Image (Gtk.Stock.Add, IconSize.Menu);
				EditCombination.Image = new Gtk.Image (Gtk.Stock.Edit, IconSize.Menu);

				if (pin == null)
				{
					ClonePin.Sensitive = false;
					EditPin.Sensitive = false;
					RemovePin.Sensitive = false;
					AddCombination.Sensitive = false;
					EditCombination.Sensitive = false;
				} else
				{
					if (pin.Combination == null)
					{
						EditCombination.Sensitive = false;
					} else
					{
						AddCombination.Sensitive = false;
					}
					if (con.Configuration.AvailableDigitalPins.Length == 0)
					{
						ClonePin.Sensitive = false;
					}
				}
				AddPin.ButtonPressEvent += (o, args) => RunAddAPinDialog ();
				ClonePin.ButtonPressEvent += (o, args) => con.Configuration.ClonePin (pin.Pin as APin);
				EditPin.ButtonPressEvent += (o, args) => RunAddAPinDialog (pin.Pin);
				RemovePin.ButtonPressEvent += (o, args) => con.Configuration.RemovePin (pin.Pin);
				ClearPins.ButtonPressEvent += (o, args) => RunAPinClear ();
				AddCombination.ButtonPressEvent += (o, args) => RunMeasurementCombinationDialog (null, pin.Pin);
				EditCombination.ButtonPressEvent += (o, args) => RunMeasurementCombinationDialog (pin.Combination);

				m.Add (AddPin);
				m.Add (ClonePin);
				m.Add (EditPin);
				m.Add (RemovePin);
				m.Add (new SeparatorMenuItem ());
				m.Add (ClearPins);
				m.Add (new SeparatorMenuItem ());
				m.Add (AddCombination);
				m.Add (EditCombination);
				m.ShowAll ();
				m.Popup ();
			}
		}

		/// <summary>
		/// Creates popupmenu. 
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		[GLib.ConnectBeforeAttribute]
		protected void OnDigitalPinNodePressed (object sender, ButtonPressEventArgs e)
		{
			if (e.Event.Button == 3)
			{
				Menu m = new Menu ();
				DPinTreeNode pin = (sender as NodeView).NodeSelection.SelectedNode as DPinTreeNode;

				var AddPin = new ImageMenuItem ("Add Output...");
				var ClonePin = new ImageMenuItem ("Duplicate");
				var EditPin = new ImageMenuItem ("Edit Output...");
				var RemovePin = new ImageMenuItem ("Delete Output");
				var ClearPins = new ImageMenuItem ("Clear Outputs");
				var AddSequence = new ImageMenuItem ("Add Sequence...");
				var EditSequence = new ImageMenuItem ("Edit Sequence...");

				AddPin.Image = new Gtk.Image (Gtk.Stock.Add, IconSize.Menu);
				ClonePin.Image = new Gtk.Image (Gtk.Stock.Copy, IconSize.Menu);
				EditPin.Image = new Gtk.Image (Gtk.Stock.Edit, IconSize.Menu);
				RemovePin.Image = new Gtk.Image (Gtk.Stock.Delete, IconSize.Menu);
				ClearPins.Image = new Gtk.Image (Gtk.Stock.Clear, IconSize.Menu);
				AddSequence.Image = new Gtk.Image (Gtk.Stock.Add, IconSize.Menu);
				EditSequence.Image = new Gtk.Image (Gtk.Stock.Edit, IconSize.Menu);

				if (pin == null)
				{
					ClonePin.Sensitive = false;
					EditPin.Sensitive = false;
					RemovePin.Sensitive = false;
					AddSequence.Sensitive = false;
					EditSequence.Sensitive = false;
				} else
				{
					if (pin.Sequence == null)
					{
						EditSequence.Sensitive = false;
					} else
					{
						AddSequence.Sensitive = false;
					}
					if (con.Configuration.AvailableDigitalPins.Length == 0)
					{
						ClonePin.Sensitive = false;
					}
				}

				AddPin.ButtonPressEvent += (o, args) => RunAddDPinDialog ();
				ClonePin.ButtonPressEvent += (o, args) => con.Configuration.ClonePin (pin.Pin as DPin);
				EditPin.ButtonPressEvent += (o, args) => RunAddDPinDialog (pin.Pin);
				RemovePin.ButtonPressEvent += (o, args) => con.Configuration.RemovePin (pin.Pin);
				ClearPins.ButtonPressEvent += (o, args) => RunDPinClear ();
				AddSequence.ButtonPressEvent += (o, args) =>
				{
					RunSequenceDialog (null, pin.Pin);
					this.notebook1.CurrentPage = 3;
				};
				EditSequence.ButtonPressEvent += (o, args) => RunSequenceDialog (pin.Sequence);

				m.Add (AddPin);
				m.Add (ClonePin);
				m.Add (EditPin);
				m.Add (RemovePin);
				m.Add (new SeparatorMenuItem ());
				m.Add (ClearPins);
				m.Add (new SeparatorMenuItem ());
				m.Add (AddSequence);
				m.Add (EditSequence);
				m.ShowAll ();
				m.Popup ();
			}
		}

		/// <summary>
		/// Creates popupmenu. 
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		[GLib.ConnectBeforeAttribute]
		protected void OnMeasurementCombinationNodePressed (object sender, ButtonPressEventArgs e)
		{
			if (e.Event.Button == 3)
			{
				Menu m = new Menu ();
				MeasurementCombinationTreeNode pin = (sender as NodeView).NodeSelection.SelectedNode as MeasurementCombinationTreeNode;

				var AddPin = new ImageMenuItem ("Add MeasurementCombination...");
				var ClonePin = new ImageMenuItem ("Duplicate");
				var EditPin = new ImageMenuItem ("Edit MeasurementCombination...");
				var RemovePin = new ImageMenuItem ("Delete MeasurementCombination");
				var ClearPins = new ImageMenuItem ("Clear MeasurementCombination");

				AddPin.Image = new Gtk.Image (Gtk.Stock.Add, IconSize.Menu);
				ClonePin.Image = new Gtk.Image (Gtk.Stock.Copy, IconSize.Menu);
				EditPin.Image = new Gtk.Image (Gtk.Stock.Edit, IconSize.Menu);
				RemovePin.Image = new Gtk.Image (Gtk.Stock.Delete, IconSize.Menu);
				ClearPins.Image = new Gtk.Image (Gtk.Stock.Clear, IconSize.Menu);

				if (pin == null)
				{
					ClonePin.Sensitive = false;
					EditPin.Sensitive = false;
					RemovePin.Sensitive = false;
				}

				AddPin.ButtonPressEvent += (o, args) => RunMeasurementCombinationDialog ();
				ClonePin.ButtonPressEvent += (o, args) => con.Configuration.CloneMeasurementCombination (pin.AnalogSignal);
				EditPin.ButtonPressEvent += (o, args) => this.RunMeasurementCombinationDialog (pin.AnalogSignal);
				RemovePin.ButtonPressEvent += (o, args) => con.Configuration.RemoveMeasurementCombination (pin.AnalogSignal);
				ClearPins.ButtonPressEvent += (o, args) => RunMeasurementCombinationClear ();

				m.Add (AddPin);
				m.Add (ClonePin);
				m.Add (EditPin);
				m.Add (RemovePin);
				m.Add (new SeparatorMenuItem ());
				m.Add (ClearPins);
				m.ShowAll ();
				m.Popup ();
			}	
		}

		/// <summary>
		/// Creates popupmenu. 
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		[GLib.ConnectBeforeAttribute]
		protected void OnSequeneceNodePressed (object sender, ButtonPressEventArgs e)
		{
			if (e.Event.Button == 3)
			{
				Menu m = new Menu ();
				SequenceTreeNode pin = (sender as NodeView).NodeSelection.SelectedNode as SequenceTreeNode;

			
				var AddPin = new ImageMenuItem ("Add Sequence...");
				var ClonePin = new ImageMenuItem ("Duplicate");
				var EditPin = new ImageMenuItem ("Edit Sequence...");
				var RemovePin = new ImageMenuItem ("Delete Sequence");
				var AddToGroupItem = new MenuItem ("Add to group");
				var AddToGroupMenu = new Menu ();
				var ClearPins = new ImageMenuItem ("Clear Sequence");
				var RemoveGroupItem = new ImageMenuItem ("Remove from group");
				var RemoveGroupSequences = new ImageMenuItem ("Delete group sequences");

				AddPin.Image = new Gtk.Image (Gtk.Stock.Add, IconSize.Menu);
				ClonePin.Image = new Gtk.Image (Gtk.Stock.Copy, IconSize.Menu);
				EditPin.Image = new Gtk.Image (Gtk.Stock.Edit, IconSize.Menu);
				RemovePin.Image = new Gtk.Image (Gtk.Stock.Delete, IconSize.Menu);
				ClearPins.Image = new Gtk.Image (Gtk.Stock.Clear, IconSize.Menu);
				RemoveGroupItem.Image = new Gtk.Image (Gtk.Stock.Remove, IconSize.Menu);
				RemoveGroupSequences.Image = new Gtk.Image (Gtk.Stock.Delete, IconSize.Menu);


				AddPin.ButtonPressEvent += (o, args) => RunSequenceDialog ();
				ClonePin.ButtonPressEvent += (o, args) => con.Configuration.CloneSequence (pin.Seq);
				EditPin.ButtonPressEvent += (o, args) => this.RunSequenceDialog (pin.Seq);
				RemovePin.ButtonPressEvent += (o, args) => con.Configuration.RemoveSequence (pin.Seq);
				ClearPins.ButtonPressEvent += (o, args) => RunSequenceClear ();
				RemoveGroupItem.ButtonPressEvent += (o, args) =>
				{
					pin.Seq.GroupName = string.Empty;
				};
				RemoveGroupSequences.ButtonPressEvent += (o, args) => RunSequenceGroupDelete (pin.Seq.GroupName);

				AddToGroupItem.Submenu = AddToGroupMenu;
				foreach (string s in con.Configuration.SequenceGroups)
				{
					var item = new MenuItem (s);
					item.ButtonPressEvent += (o, args) =>
					{
						if (pin != null)
						{
							pin.Seq.GroupName = s;
						}
					};
					AddToGroupMenu.Add (item);
				}


				AddToGroupMenu.Add (new SeparatorMenuItem ());
				AddToGroupMenu.Add (RemoveGroupItem);

				if (pin == null)
				{
					ClonePin.Sensitive = false;
					EditPin.Sensitive = false;
					RemovePin.Sensitive = false;
					AddToGroupMenu.Sensitive = false;
					RemoveGroupSequences.Sensitive = false;
				} else
				{
					if (string.IsNullOrEmpty (pin.Seq.GroupName))
					{
						RemoveGroupItem.Sensitive = false;
						RemoveGroupSequences.Sensitive = false;
					}
					if (con.Configuration.SequenceGroups.Count == 0)
					{
						AddToGroupItem.Sensitive = false;
					}
					if (con.Configuration.GetPinsWithoutSequence ().Length == 0)
					{
						ClonePin.Sensitive = false;
					}
				}

				m.Add (AddPin);
				m.Add (ClonePin);
				m.Add (EditPin);
				m.Add (RemovePin);
				m.Add (new SeparatorMenuItem ());
				m.Add (AddToGroupItem);
				m.Add (RemoveGroupSequences);
				m.Add (new SeparatorMenuItem ());
				m.Add (ClearPins);
				m.ShowAll ();
				m.Popup ();
			}		
		}

		#endregion

		#region FillNodes

		/// <summary>
		///	Updates all nodeviews. 
		/// </summary>
		private void UpdateAllNodes ()
		{
			FillDigitalPinNodes ();
			FillAnalogPinNodes ();
			FillSequenceNodes ();
			FillSequencePreviewPlot ();
			FillMeasurementCombinationNodes ();
		}

		/// <summary>
		/// Updates the DPin nodeview.
		/// </summary>
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

		/// <summary>
		/// Updates the APin nodeview.
		/// </summary>
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

		/// <summary>
		/// Updates the sequence nodeview.
		/// </summary>
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

		/// <summary>
		/// Updates the measurementcombination nodeview.
		/// </summary>
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

		/// <summary>
		/// Updates the sequence preview plot. 
		/// </summary>
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
//				if (seq.Repetitions != 0 && seq.Chain.Count > 0) {
//					var followupData = new Collection<TimeValue> ();
//					followupData.Add (data.Last ());
//					followupData.Add (new TimeValue () {
//						Time = data.Last ().Time,
//						Value = ((seq.Chain [0].State == DPinState.HIGH) ? 1 : 0)
//					});
//
//					followupData.Add (new TimeValue () {
//						Time = data.Last ().Time.Add (seq.Chain [0].Duration),
//						Value = ((seq.Chain [0].State == DPinState.HIGH) ? 1 : 0)
//					});
//
//					var followupSeries = new LineSeries () {
//						DataFieldX = "Time",
//						DataFieldY = "Value",
//						YAxisKey = seq.Pin.ToString (),
//						ItemsSource = followupData,
//						StrokeThickness = 2,
//						LineStyle = OxyPlot.LineStyle.Dash,
//						Color = ColorHelper.GdkColorToOxyColor (seq.Color),
//					};
//
//					followupSeries.MouseDown += (sender, e) => {
//						if (e.ChangedButton == OxyMouseButton.Left) {
//							RunSequenceDialog (seq);
//						}
//					};
//
//					SequencePreviewPlotModel.Series.Add (followupSeries);
//				}

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

		/// <summary>
		/// Updates the realtime plot.
		/// </summary>
		private void UpdateRealTimePlot ()
		{
			if (RealTimePlotUpdate)
			{
				double now = DateTime.Now.ToOADate ();
				RealTimeXAxis.Pan (new ScreenPoint (RealTimeXAxis.Transform (now), 0), new ScreenPoint (RealTimeXAxis.Transform (LastTimeKeeperPresenterTick), 0));
				LastTimeKeeperPresenterTick = now;

				RealTimePlotView.InvalidatePlot (true);
			}
		}

		/// <summary>
		/// Initials the real time plot.
		/// Creates plot axes and inits series.
		/// </summary>
		private void InitRealTimePlot ()
		{
			#region Axes
			RealTimePlotView.Model.Axes.Clear ();

			List<string> units = con.Configuration.AnalogPins.Select (o => o.Unit).ToList<string> ();
			units.AddRange (con.Configuration.MeasurementCombinations.Select (o => o.Unit).ToList<string> ());
			units = units.Distinct ().ToList ();

			double startpos = 0.0;
			double step = 1.0 / units.Count;
			//build axes
			for (int i = 0; i < units.Count; i++)
			{
				var axis = new LinearAxis () {
					Position = AxisPosition.Left,
					StartPosition = startpos,
					EndPosition = startpos + step,
					Unit = units [i],
					Key = units [i],
					IsZoomEnabled = false,
					IsPanEnabled = false
				};
				startpos += step;

				RealTimePlotView.Model.Axes.Add (axis);
			}

			RealTimePlotView.Model.Axes.Add (RealTimeXAxis);

			#endregion

			RealTimePlotView.Model.Series.Clear ();
			RealTimePlotView.InvalidatePlot (true);
			foreach (APin a in con.Configuration.AnalogPins)
			{
				var series = new LineSeries () {
					Color = ColorHelper.GdkColorToOxyColor (a.PlotColor),
					Title = a.DisplayName,
					DataFieldX = "Time",
					DataFieldY = "Value",
					YAxisKey = a.Unit,
					XAxisKey = RealTimeXAxis.Key,
					TrackerKey = a.DisplayName,
				};

				a.OnNewValue += (o, args) => OnNewPoint (o, args, ref series);

				RealTimePlotView.Model.Series.Add (series);
			}

			foreach (MeasurementCombination a in con.Configuration.MeasurementCombinations)
			{
				var series = new LineSeries () {
					Color = ColorHelper.GdkColorToOxyColor (a.Color),
					DataFieldX = "Time",
					DataFieldY = "Value",
					Title = a.DisplayName,
					YAxisKey = a.Unit,
					XAxisKey = RealTimeXAxis.Key,
//					CanTrackerInterpolatePoints = true,
//					TrackerKey = a.DisplayName,
//					TrackerFormatString = "X:{2:yyyy-MM-dd} Y:{4}"
				};

				a.GetPinWithLargestInterval ().OnNewValue += (o, args) => OnNewPoint (o, args, ref series);

				RealTimePlotView.Model.Series.Add (series);
			}

			RealTimeXAxis.Minimum = con.StartTime.AddMinutes (-2).ToOADate ();
			RealTimeXAxis.Maximum = con.StartTime.AddMinutes (2).ToOADate ();

			ToggleRealTimePlotMarker ();
			ToggleRealTimePlotSmooth ();
		}

		#endregion

		#region BuildElements

		/// <summary>
		/// Sets every widget concerning the settings.
		/// </summary>
		private void BuildConfigSettings ()
		{
			#region FileNaming
			object[] cbeOptions = new object[]{ "[LOCALTIME]", "[UTC TIME]", "[DATE]", "[EMPTY]" };

			((ListStore)(cbeFileNaming1.Model)).AppendValues (cbeOptions [0]);
			((ListStore)(cbeFileNaming2.Model)).AppendValues (cbeOptions [0]);
			((ListStore)(cbeFileNaming3.Model)).AppendValues (cbeOptions [0]);
			((ListStore)(cbeFileNaming1.Model)).AppendValues (cbeOptions [1]);
			((ListStore)(cbeFileNaming2.Model)).AppendValues (cbeOptions [1]);
			((ListStore)(cbeFileNaming3.Model)).AppendValues (cbeOptions [1]);
			((ListStore)(cbeFileNaming1.Model)).AppendValues (cbeOptions [2]);
			((ListStore)(cbeFileNaming2.Model)).AppendValues (cbeOptions [2]);
			((ListStore)(cbeFileNaming3.Model)).AppendValues (cbeOptions [2]);
			((ListStore)(cbeFileNaming1.Model)).AppendValues (cbeOptions [3]);
			((ListStore)(cbeFileNaming2.Model)).AppendValues (cbeOptions [3]);
			((ListStore)(cbeFileNaming3.Model)).AppendValues (cbeOptions [3]);

			cbeFileNaming1.Active = 2;
			cbeFileNaming2.Active = 0;
			cbeFileNaming3.Active = 3;

			cbeFileNaming1.Changed += (sender, e) =>
			{
				con.Configuration.FileNameConvention [0] = cbeFileNaming1.ActiveText;
				UpdateFilePathPreview ();
			};

			cbeFileNaming2.Changed += (sender, e) =>
			{
				con.Configuration.FileNameConvention [1] = cbeFileNaming2.ActiveText;
				UpdateFilePathPreview ();
			};

			cbeFileNaming3.Changed += (sender, e) =>
			{
				con.Configuration.FileNameConvention [2] = cbeFileNaming3.ActiveText;
				UpdateFilePathPreview ();
			};

			#endregion


			foreach (string s in FormatOptions.TimeFormatOptions.Keys)
			{
				((ListStore)(cbeCSVTimeFormat.Model)).AppendValues (s);
			}

			cbeCSVTimeFormat.Active = 0;

			foreach (string s in SeparatorOptions.Options.Keys)
			{
				((ListStore)(cbeCSVSeparator.Model)).AppendValues (s);
			}

			cbeCSVSeparator.Active = 0;

			#region Values Format Culture

			(CultureInfo.GetCultures (CultureTypes.AllCultures))
				.Select (o => o.EnglishName)
				.OrderBy (o => o)
				.ToList ()
				.ForEach (
				o => ((ListStore)cbValuesFormatCulture.Model)
					.AppendValues (new object[]{ o })
			);
			cbValuesFormatCulture.Active = Array.IndexOf (
				(CultureInfo.GetCultures (CultureTypes.AllCultures)).Select (o => o.EnglishName).OrderBy (o => o).ToArray (),
				"English (United Kingdom)"
			);

			cbValuesFormatCulture.Changed += (sender, e) =>
			{
				if (con.Configuration != null)
				{
					con.Configuration.ValueFormatCultur = cbValuesFormatCulture.ActiveText;
				}
			};
			#endregion
		}

		/// <summary>
		/// Fills the combobox haboring every known board. 
		/// </summary>
		private void BuildMCUDisplay ()
		{
			//Update BoardList
			var store = new ListStore (typeof(string));
			foreach (Board b in con.BoardConfigs)
			{
				store.AppendValues (new object[]{ b.Name });
			}
			cbBoardType.Model = store;
			cbBoardType.Show ();
		}

		/// <summary>
		/// Binds <see cref="Controller"/> events.
		/// </summary>
		private void BindControllerEvents ()
		{
			con.Configuration.OnPinsUpdated += (o, a) =>
			{
				if (a.NewPin is DPin)
				{
					FillDigitalPinNodes ();
					FillSequencePreviewPlot ();
					FillMeasurementCombinationNodes ();
				} else if (a.NewPin is APin)
				{
					FillAnalogPinNodes ();
					FillMeasurementCombinationNodes ();
				} else
				{
					UpdateAllNodes ();
				}
			};
			con.Configuration.OnSequencesUpdated += (o, a) =>
			{
				FillSequenceNodes ();
				FillSequencePreviewPlot ();
			};
			con.Configuration.OnSignalsUpdated += (o, a) => FillMeasurementCombinationNodes ();

			con.OnControllerStarted += (o, a) =>
			{
				mediaStopAction.Sensitive = true;
				mediaPlayAction.Sensitive = false;
				LockControlls (false);
			};
			con.OnControllerStoped += (o, a) =>
			{
				mediaStopAction.Sensitive = false;
				mediaPlayAction.Sensitive = true;
				LockControlls (true);
			};

		}

		/// <summary>
		/// Binds the widget events. This does not seam to work with glade interfaces.
		/// </summary>
		protected void BindWidgetEvents ()
		{
			try
			{
				BuildAnalogButtons ();
			} catch (Exception e)
			{
				con.ConLogger.Log (e.ToString (), LogLevel.DEBUG);
			}

			try
			{
				BuildDigitalButtons ();
			} catch (Exception e)
			{
				con.ConLogger.Log (e.ToString (), LogLevel.DEBUG);
			}

			try
			{
				BuildAnalogCombinationButtons ();
			} catch (Exception e)
			{
				con.ConLogger.Log (e.ToString (), LogLevel.DEBUG);
			}

			try
			{
				BuildSequenceButtons ();
			} catch (Exception e)
			{
				con.ConLogger.Log (e.ToString (), LogLevel.DEBUG);
			}

			cbBoardType.Changed += OnCbBoardTypeChanged;
			cbAREF.Changed += OnCbAREFChanged;

			btnCSVFilePathOpen.Clicked += OnBtnCSVFilePathOpenClicked;
			cbeCSVSeparator.Changed += OnCbeCSVSeparatorChanged;
			cbeCSVTimeFormat.Changed += OnCbeCSVTimeFormatChanged;
			cbCSVUTC.Toggled += OnCbCSVUTCToggled;
		}

		/// <summary>
		/// Builds the digital button signals and connects delegates.
		/// </summary>
		private void BuildDigitalButtons ()
		{
			btnAddDPin.Clicked += OnBtnAddDPinClicked;
			btnEditDPin.Clicked += OnBtnEditDPinClicked;
			btnCloneDPin.Clicked += OnBtnCloneDPinClicked;
			btnRemoveDPin.Clicked += OnBtnRemoveDPinClicked;
			btnClearDPins.Clicked += OnBtnClearDPinsClicked;
		}

		/// <summary>
		/// Builds the analog button signals and connects delegates.
		/// </summary>
		private void BuildAnalogButtons ()
		{
			btnAddAPin.Clicked += OnBtnAddAPinClicked;
			btnCloneAPin.Clicked += OnBtnCloneAPinClicked;
			btnEditAPin.Clicked += OnBtnEditAPinClicked;
			btnRemoveAPin.Clicked += OnBtnRemoveAPinClicked;
			btnClearAPins.Clicked += OnBtnClearAPinsClicked;			
		}

		/// <summary>
		/// Builds the analog combination button signals and connects delegates.
		/// </summary>
		private void BuildAnalogCombinationButtons ()
		{
			btnAddSignal.Clicked += OnBtnAddSignalClicked;
			btnCloneSignal.Clicked += OnBtnCloneSignalClicked;
			btnEditSignal.Clicked += OnBtnEditSignalClicked;
			btnRemoveSignal.Clicked += OnBtnRemoveSignalClicked;
			btnClearSignals.Clicked += OnBtnClearSignalsClicked;
		}

		/// <summary>
		/// Builds the sequence button signals and connects delegates.
		/// </summary>
		private void BuildSequenceButtons ()
		{
			btnAddSequence.Clicked += OnBtnAddSequenceClicked;
			btnCloneSequence.Clicked += OnBtnCloneSequenceClicked;
			btnEditSequence.Clicked += OnBtnEditSequenceClicked;
			btnRemoveSequence.Clicked += OnBtnRemoveSequenceClicked;
			btnClearSequence.Clicked += OnBtnClearSequenceClicked;	
		}

		/// <summary>
		/// Builds the nodeviews.
		/// </summary>
		private void BuildNodeViews ()
		{
			#region Digital
			nvDigitalPins.NodeStore = NodeStoreDigitalPins;
//			TreeModelSort DPinsorter = new TreeModelSort (nvDigitalPins.Model);
			nvDigitalPins.RowActivated += (o, args) =>
			{
				var pin = con.Configuration.Pins
					.Where (x => x.Type == PinType.DIGITAL)
					.ToList () [((o as NodeView).NodeSelection.SelectedNode as DPinTreeNode).Index];
				RunAddDPinDialog (pin as DPin);
			};

			nvDigitalPins.AppendColumn (new TreeViewColumn ("Name", new Gtk.CellRendererText (), "text", 0) {
				Resizable = true,
				Sizing = TreeViewColumnSizing.Autosize,
//				SortColumnId = 0,
//				SortOrder = SortType.Ascending,
				Clickable = true,
			});
			nvDigitalPins.AppendColumn (new TreeViewColumn ("Number", new Gtk.CellRendererText (), "text", 1) {
				Resizable = true,
				Sizing = TreeViewColumnSizing.Autosize,
//				SortColumnId = 1,
//				SortOrder = SortType.Ascending,
				Clickable = true,
			});
			nvDigitalPins.AppendColumn ("Color", new Gtk.CellRendererPixbuf (), "pixbuf", 2);
			nvDigitalPins.AppendColumn (new TreeViewColumn ("Seqeuence", new Gtk.CellRendererText (), "text", 3) {
				Resizable = true,
				Sizing = TreeViewColumnSizing.Autosize,
//				SortColumnId = 3,
//				SortOrder = SortType.Ascending,
				Clickable = true,
			});

//			DPinsorter.SetSortFunc (0, delegate(TreeModel model, TreeIter a, TreeIter b)
//			{
//				string s1 = (string)model.GetValue (a, 0);
//				string s2 = (string)model.GetValue (b, 0);
//				// Analysis disable once StringCompareIsCultureSpecific
//				return String.Compare (s1, s2);
//			});
//			DPinsorter.SetSortFunc (1, delegate(TreeModel model, TreeIter a, TreeIter b)
//			{
//				string s1 = (string)model.GetValue (a, 1);
//				string s2 = (string)model.GetValue (b, 1);
//				// Analysis disable once StringCompareIsCultureSpecific
//				return String.Compare (s1, s2);
//			});
//			DPinsorter.SetSortFunc (3, delegate(TreeModel model, TreeIter a, TreeIter b)
//			{
//				string s1 = (string)model.GetValue (a, 3);
//				string s2 = (string)model.GetValue (b, 3);
//				// Analysis disable once StringCompareIsCultureSpecific
//				return String.Compare (s1, s2);
//			});

//			nvDigitalPins.Model = DPinsorter;
			nvDigitalPins.QueueDraw ();

			#endregion

			#region Analog
			nvAnalogPins.NodeStore = NodeStoreAnalogPins;

			nvAnalogPins.RowActivated += (o, args) =>
			{
				var pin = ((o as NodeView).NodeSelection.SelectedNode as APinTreeNode).Pin;
				RunAddAPinDialog (pin as APin);
			};

			nvAnalogPins.AppendColumn (new TreeViewColumn ("Name", new Gtk.CellRendererText (), "text", 0) {
				Resizable = true,
				Sizing = TreeViewColumnSizing.Autosize,
				Clickable = true,
			});
			nvAnalogPins.AppendColumn (new TreeViewColumn ("Number", new Gtk.CellRendererText (), "text", 1) {
				Resizable = true,
				Sizing = TreeViewColumnSizing.Autosize,
				Clickable = true,
			});
			nvAnalogPins.AppendColumn ("Color", new Gtk.CellRendererPixbuf (), "pixbuf", 2);
			nvAnalogPins.AppendColumn ("Slope", new Gtk.CellRendererText (), "text", 3);
			nvAnalogPins.AppendColumn ("Offset", new Gtk.CellRendererText (), "text", 4);
			nvAnalogPins.AppendColumn (new TreeViewColumn ("Unit", new Gtk.CellRendererText (), "text", 5) {
				Resizable = true,
				Sizing = TreeViewColumnSizing.Autosize,
				Clickable = true,
			});
			nvAnalogPins.AppendColumn (new TreeViewColumn ("Interval", new Gtk.CellRendererText (), "text", 6) {
				Resizable = true,
				Sizing = TreeViewColumnSizing.Autosize,
				Clickable = true,
			});
			nvAnalogPins.AppendColumn (new TreeViewColumn ("Mean Values", new Gtk.CellRendererText (), "text", 7) {
				Resizable = true,
				Sizing = TreeViewColumnSizing.Autosize,
				Clickable = true,
			});
			nvAnalogPins.AppendColumn (new TreeViewColumn ("Combination", new Gtk.CellRendererText (), "text", 8) {
				Resizable = true,
				Sizing = TreeViewColumnSizing.Autosize,
				Clickable = true,
			});

			nvAnalogPins.QueueDraw ();
			#endregion

			#region Sequences

			nvSequences.NodeStore = NodeStoreSequences;
//			Gtk.TreeModelSort sorter = new Gtk.TreeModelSort (nvSequences.Model);
			nvSequences.RowActivated += (o, args) =>
			{
				var Seq = ((o as NodeView).NodeSelection.SelectedNode as SequenceTreeNode).Seq;
				RunSequenceDialog (Seq);
			};

//			nvSequences.AppendColumn (new TreeViewColumn ("Group", new CellRendererText (), "text", 0) {
//				Resizable = true,
//				Sizing = TreeViewColumnSizing.Autosize,
////				SortColumnId = 0,
////				SortOrder = SortType.Ascending,
//				Clickable = true,
//			});
			nvSequences.AppendColumn (new TreeViewColumn ("Name", new CellRendererText (), "text", 1) {
				Resizable = true,
				Sizing = TreeViewColumnSizing.Autosize,
//				SortColumnId = 1,
//				SortOrder = SortType.Ascending,
				Clickable = true,
			});
			nvSequences.AppendColumn (new TreeViewColumn ("Color", new CellRendererPixbuf (), "pixbuf", 2));
			nvSequences.AppendColumn (new TreeViewColumn ("Pin Name", new CellRendererText (), "text", 3) {
				Resizable = true,
				Sizing = TreeViewColumnSizing.Autosize,
//				SortColumnId = 3,
//				SortOrder = SortType.Ascending,
				Clickable = true,
			});
			nvSequences.AppendColumn (new TreeViewColumn ("Pin Number", new CellRendererText (), "text", 4) {
				Resizable = true,
				Sizing = TreeViewColumnSizing.Autosize,
//				SortColumnId = 4,
//				SortOrder = SortType.Ascending,
				Clickable = true,
			});
			nvSequences.AppendColumn (new TreeViewColumn ("Runtime", new CellRendererText (), "text", 5) {
				Resizable = true,
				Sizing = TreeViewColumnSizing.Autosize,
//				SortColumnId = 5,
//				SortOrder = SortType.Ascending,
				Clickable = true,
			});
			nvSequences.AppendColumn (new TreeViewColumn ("Repetitions", new CellRendererText (), "text", 6) {
				Resizable = true,
				Sizing = TreeViewColumnSizing.Autosize,
//				SortColumnId = 6,
//				SortOrder = SortType.Ascending,
				Clickable = true,
			});

//			sorter.SetSortFunc (0, delegate(TreeModel model, TreeIter a, TreeIter b)
//			{
//				string s1 = (string)model.GetValue (a, 0);
//				string s2 = (string)model.GetValue (b, 0);
//				// Analysis disable once StringCompareIsCultureSpecific
//				return String.Compare (s1, s2);
//			});
//			sorter.SetSortFunc (1, delegate(TreeModel model, TreeIter a, TreeIter b)
//			{
//				string s1 = (string)model.GetValue (a, 1);
//				string s2 = (string)model.GetValue (b, 1);
//				// Analysis disable once StringCompareIsCultureSpecific
//				return String.Compare (s1, s2);
//			});
//			sorter.SetSortFunc (3, delegate(TreeModel model, TreeIter a, TreeIter b)
//			{
//				string s1 = (string)model.GetValue (a, 3);
//				string s2 = (string)model.GetValue (b, 3);
//				// Analysis disable once StringCompareIsCultureSpecific
//				return String.Compare (s1, s2);
//			});
//			sorter.SetSortFunc (4, delegate(TreeModel model, TreeIter a, TreeIter b)
//			{
//				string s1 = (string)model.GetValue (a, 4);
//				string s2 = (string)model.GetValue (b, 4);
//				// Analysis disable once StringCompareIsCultureSpecific
//				return String.Compare (s1, s2);
//			});
//			sorter.SetSortFunc (5, delegate(TreeModel model, TreeIter a, TreeIter b)
//			{
//				TimeSpan s1 = TimeSpan.Parse ((string)model.GetValue (a, 5));
//				TimeSpan s2 = TimeSpan.Parse ((string)model.GetValue (b, 5));
//				// Analysis disable once StringCompareIsCultureSpecific
//				return (s1 < s2) ? -1 : 1;
//			});
//			sorter.SetSortFunc (6, delegate(TreeModel model, TreeIter a, TreeIter b)
//			{
//				string s1 = (string)model.GetValue (a, 6);
//				string s2 = (string)model.GetValue (b, 6);
//
//				s1 = s1.Split (' ') [0];
//				s2 = s2.Split (' ') [0];
//
//				int c1 = 0;
//				int c2 = 0;
//
//				if (s1.Equals ("\u221E"))
//				{
//					c1 = int.MaxValue;
//				} else
//				{
//					c1 = Convert.ToInt32 (s1);
//				}
//
//				if (s2.Equals ("\u221E"))
//				{
//					c2 = int.MaxValue;
//				} else
//				{
//					c2 = Convert.ToInt32 (s2);
//				}
//
//				// Analysis disable once StringCompareIsCultureSpecific
//				return (c1 < c2) ? -1 : 1;
//			});

//			nvSequences.Model = sorter;
			nvSequences.QueueDraw ();
			#endregion

			#region MeasurementCombinations
			nvMeasurementCombinations.NodeStore = NodeStoreMeasurementCombinations;
//			TreeModelSort MeComsorter = new TreeModelSort (nvMeasurementCombinations.Model);
			nvMeasurementCombinations.RowActivated += (o, args) =>
			{
				var sig = ((o as NodeView).NodeSelection.SelectedNode as MeasurementCombinationTreeNode).AnalogSignal;
				RunMeasurementCombinationDialog (sig);
			};
			nvMeasurementCombinations.AppendColumn (new TreeViewColumn ("Name", new CellRendererText (), "text", 0) {
				Resizable = true,
				Sizing = TreeViewColumnSizing.Autosize,
//				SortColumnId = 0,
//				SortOrder = SortType.Ascending,
				Clickable = true,
			});
			nvMeasurementCombinations.AppendColumn (new TreeViewColumn ("Color", new CellRendererPixbuf (), "pixbuf", 1));
			nvMeasurementCombinations.AppendColumn (new TreeViewColumn ("Pin Name", new CellRendererText (), "text", 2) {
				Resizable = true,
				Sizing = TreeViewColumnSizing.Autosize,
//				SortColumnId = 2,
//				SortOrder = SortType.Ascending,
				Clickable = true,
			});
			nvMeasurementCombinations.AppendColumn (new TreeViewColumn ("Pin Number", new CellRendererText (), "text", 3) {
				Resizable = true,
				Sizing = TreeViewColumnSizing.Autosize,
//				SortColumnId = 3,
//				SortOrder = SortType.Ascending,
				Clickable = true,
			});
			nvMeasurementCombinations.AppendColumn (new TreeViewColumn ("Unit", new CellRendererText (), "text", 4) {
				Resizable = true,
				Sizing = TreeViewColumnSizing.Autosize,
				Clickable = true,
			});

			nvMeasurementCombinations.AppendColumn (new TreeViewColumn ("Interval", new CellRendererText (), "text", 5) {
				Resizable = true,
				Sizing = TreeViewColumnSizing.Autosize,
//				SortColumnId = 4,
//				SortOrder = SortType.Ascending,
				Clickable = true,
			});
			nvMeasurementCombinations.AppendColumn (new TreeViewColumn ("Mean Value", new CellRendererText (), "text", 6) {
				Resizable = true,
				Sizing = TreeViewColumnSizing.Autosize,
//				SortColumnId = 5,
//				SortOrder = SortType.Ascending,
				Clickable = true,
			});
			nvMeasurementCombinations.AppendColumn (new TreeViewColumn ("Operation", new CellRendererText (), "text", 7));
//
//			MeComsorter.SetSortFunc (0, delegate(TreeModel model, TreeIter a, TreeIter b)
//			{
//				string s1 = (string)model.GetValue (a, 0);
//				string s2 = (string)model.GetValue (b, 0);
//				// Analysis disable once StringCompareIsCultureSpecific
//				return String.Compare (s1, s2);
//			});
//			MeComsorter.SetSortFunc (2, delegate(TreeModel model, TreeIter a, TreeIter b)
//			{
//				string s1 = (string)model.GetValue (a, 2);
//				string s2 = (string)model.GetValue (b, 2);
//				// Analysis disable once StringCompareIsCultureSpecific
//				return String.Compare (s1, s2);
//			});
//			MeComsorter.SetSortFunc (3, delegate(TreeModel model, TreeIter a, TreeIter b)
//			{
//				string s1 = (string)model.GetValue (a, 3);
//				string s2 = (string)model.GetValue (b, 3);
//				// Analysis disable once StringCompareIsCultureSpecific
//				return String.Compare (s1, s2);
//			});
//			MeComsorter.SetSortFunc (4, delegate(TreeModel model, TreeIter a, TreeIter b)
//			{
//				TimeSpan s1 = TimeSpan.Parse ((string)model.GetValue (a, 4));
//				TimeSpan s2 = TimeSpan.Parse ((string)model.GetValue (b, 4));
//				// Analysis disable once StringCompareIsCultureSpecific
//				return (s1 < s2) ? -1 : 1;
//			});
//			MeComsorter.SetSortFunc (5, delegate(TreeModel model, TreeIter a, TreeIter b)
//			{
//				double s1 = Convert.ToDouble ((string)model.GetValue (a, 5));
//				double s2 = Convert.ToDouble ((string)model.GetValue (b, 5));
//				// Analysis disable once StringCompareIsCultureSpecific
//				return (s1 < s2) ? -1 : 1;
//			});
//			MeComsorter.SetSortFunc (6, delegate(TreeModel model, TreeIter a, TreeIter b)
//			{
//				string s1 = (string)model.GetValue (a, 6);
//				string s2 = (string)model.GetValue (b, 6);
//				// Analysis disable once StringCompareIsCultureSpecific
//				return String.Compare (s1, s2);
//			});
//
//			nvMeasurementCombinations.Model = MeComsorter;
			nvMeasurementCombinations.QueueDraw ();

			#endregion
		}

		/// <summary>
		/// Builds the toolbar.
		/// </summary>
		private void BuildToolBar ()
		{
			PortBox = new ComboBox ();
			PortBox.WidthRequest = 200;
			PortBox.Visible = true;

			EventBox eb = new EventBox ();

			PortBox.Changed += (sender, e) =>
			{
				ArduinoController.SerialPortName = PortBox.ActiveText;
				ArduinoController.Setup ();	
//				UpdatePortBox ();
			};

			PortBox.ButtonPressEvent += (o, args) => UpdatePortBox ();
			eb.ButtonPressEvent += (o, args) =>	UpdatePortBox ();

			var label = new Label ("Connect to:");
			label.Xalign = 0;

			var vbox = new VBox ();

			vbox.PackStart (label, true, true, 0);
			vbox.PackStart (PortBox, false, false, 0);

			eb.Add (vbox);

			var item = new ToolItem ();
			item.Expand = false;
			item.Add (eb);

			this.toolbarMain.Remove (this.toolbarMain.GetNthItem (4));

			this.toolbarMain.Insert (item, 4);
			this.toolbarMain.ShowAll ();

			UpdatePortBox ();
		}

		/// <summary>
		/// Builds the menu.
		/// </summary>
		private void BuildMenu ()
		{
			MenuBar mbar = (this.UIManager.GetWidget ("/menubarMain") as MenuBar);

			#region FileMenu
			Menu filemenu = new Menu ();
			MenuItem file = new MenuItem ("File");
			file.Submenu = filemenu;
			MenuItem newoption = new MenuItem ("New");
			MenuItem openoption = openAction.CreateMenuItem () as MenuItem;
			MenuItem saveoption = saveAction.CreateMenuItem () as MenuItem; 
			MenuItem saveasoption = saveAsAction.CreateMenuItem () as MenuItem;
			MenuItem recentconfigs = new ImageMenuItem ("Recent Configurations");
			MenuItem exit = quitAction.CreateMenuItem () as MenuItem;

			Menu LastConfigurations = new Menu ();
			recentconfigs.Activated += (object sender, EventArgs e) =>
			{
				foreach (MenuItem mi in LastConfigurations.AllChildren)
				{
					LastConfigurations.Remove (mi);
				}

				foreach (string s in con.LastConfigurationLocations)
				{
					if (!string.IsNullOrEmpty (s))
					{
						MenuItem entry = new MenuItem (s);
						entry.ButtonPressEvent += (object o, ButtonPressEventArgs args) => RunOpenConfig (s);
						LastConfigurations.Append (entry);
					}
				}
				LastConfigurations.ShowAll ();
			};
			exit.Activated += (sender, e) => OnDeleteEvent (null, null);

			filemenu.Append (newoption);
			filemenu.Append (openoption);
			filemenu.Append (saveoption);
			filemenu.Append (saveasoption);
			filemenu.Append (new SeparatorMenuItem ());
			filemenu.Append (recentconfigs);
			recentconfigs.Submenu = LastConfigurations;
			filemenu.Append (new SeparatorMenuItem ());
			filemenu.Append (exit);
			mbar.Append (file);
			#endregion

			#region Edit
			Menu editmenu = new Menu ();
			MenuItem edit = new MenuItem ("Edit");
			edit.Submenu = editmenu;
			MenuItem preferences = preferencesAction.CreateMenuItem () as MenuItem;
			editmenu.Append (preferences);

			mbar.Append (edit);
			#endregion

			#region ConnectionMenu
			Menu connectionmenu = new Menu ();
			MenuItem connection = new MenuItem ("Connection");
			connection.Submenu = connectionmenu;
			var autoConnect = refreshAction.CreateMenuItem () as MenuItem;
			Menu portmenu = new Menu ();
			MenuItem port = new MenuItem ("Port");
			connectionmenu.Append (autoConnect);
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
			MenuItem about = aboutAction.CreateMenuItem () as MenuItem;

			helpmenu.Append (about);

			mbar.Append (help);
			#endregion

			mbar.ShowAll ();

		}

		/// <summary>
		/// Builds the sequence preview plot.
		/// </summary>
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
				MajorGridlineStyle = OxyPlot.LineStyle.Solid,
				MinorGridlineColor = OxyColors.LightGray,
				MinorGridlineStyle = OxyPlot.LineStyle.Dot,
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

		/// <summary>
		/// Builds the realtime plot.
		/// </summary>
		private void BuildRealTimePlot ()
		{
			RealTimeXAxis = new DateTimeAxis {
				Key = "RealTimeXAxis",
				Position = AxisPosition.Bottom,
				MajorGridlineThickness = 1,
				MajorGridlineStyle = OxyPlot.LineStyle.Solid,
				MinorGridlineColor = OxyColors.LightGray,
				MinorGridlineStyle = OxyPlot.LineStyle.Dot,
				MinorGridlineThickness = .5,
				MinorStep = TimeSpan.FromSeconds (1).Ticks,
			};

			var YAxis = new LinearAxis {
				Position = AxisPosition.Left,
				IsPanEnabled = false,
				IsZoomEnabled = false,
				MajorGridlineThickness = 1,
				MajorGridlineStyle = OxyPlot.LineStyle.Solid,
				MinorGridlineColor = OxyColors.LightGray,
				MinorGridlineStyle = OxyPlot.LineStyle.Dot,
				MinorGridlineThickness = .5,
			};

			RealTimePlotModel = new PlotModel {
				PlotType = PlotType.XY,
				Background = OxyPlot.OxyColors.White,
				IsLegendVisible = true,
				LegendOrientation = LegendOrientation.Horizontal,
				LegendPlacement = LegendPlacement.Outside,
				LegendPosition = LegendPosition.RightMiddle,
			};

			RealTimePlotController = new PlotController ();

			RealTimePlotModel.Axes.Add (YAxis);
			RealTimePlotModel.Axes.Add (RealTimeXAxis);

			//Enable double buffered to prevent flickering
			RealTimePlotView = new PlotView (){ Name = "", Model = RealTimePlotModel, DoubleBuffered = true  };

			vboxRealTimePlot.PackStart (RealTimePlotView, true, true, 0);
			(vboxRealTimePlot [RealTimePlotView] as Box.BoxChild).Position = 0;

			RealTimePlotView.SetSizeRequest (hbSequences.Allocation.Width, fSequences.Allocation.Height / 2);
			vpanedSequences.Position = fSequences.Allocation.Height / 2;

			RealTimePlotView.ShowAll ();

			cbtnRealTimePlotSmoothValues.Toggled += OnCbtnRealTimePlotSmoothValues;
			cbtnRealTimePlotShowMarker.Toggled += OnCbtnRealTimePlotShowMarkerToggled;
			btnRealTimePlotJumpStart.Clicked += OnBtnRealTimePlotJumpStartClicked;
			btnRealTimePlotJumpLatest.Clicked += OnBtnRealTimePlotJumpLatestClicked;
			btnRealTimePlotSnapshot.Clicked += OnBtnRealTimePlotSnapshotClicked;
			btnRealTimePlotPause.Clicked += OnBtnRealTimePlotPauseClicked;
			btnRealTimePlotResetZoom.Clicked += OnBtnRealTimePlotResetZoomClicked;
			btnRealTimePlotFitData.Clicked += OnBtnRealTimePlotFitDataClicked;
			cbtnRealTimePlotLimitPoints.Active = Frontend.Settings.Default.LimitPlotPoints;
			cbtnRealTimePlotLimitPoints.Toggled += OnCbtnRealTimePlotLimitPoints;
		}

		#endregion

		#region Events

		/// <summary>
		/// Refreshs the MCU infos.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		private void RefreshMCUInfos (object sender, EventArgs e)
		{
			cbBoardType.Active = con.BoardConfigs.ToList ()
				.IndexOf (con.BoardConfigs.ToList ()
					.Single (o => o.MCU == con.Configuration.Board.MCU)
			);
		}

		/// <summary>
		/// Updates the settings.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		private void UpdateSettings (object sender = null, EventArgs e = null)
		{
			eCSVFilePath.Text = con.Configuration.CSVSaveFolderPath;

			//Separator
			int index = 0;
			bool found = false;
			foreach (object[] obj in (ListStore)cbeCSVSeparator.Model)
			{
				if (obj [0].ToString () == con.Configuration.Separator)
				{
					cbeCSVSeparator.Active = index;
					found = true;
					break;
				}
				index++;
			}
			if (!found)
			{
				cbeCSVSeparator.AppendText (con.Configuration.Separator);
				cbeCSVSeparator.Active = index;
			}

			//Timestamps
			cbCSVUTC.Active = con.Configuration.UTCTimestamp;

			//Timeformat
			index = 0;
			found = false;
			foreach (object[] obj in (ListStore)cbeCSVTimeFormat.Model)
			{
				if (obj [0].ToString () == con.Configuration.TimeFormat)
				{
					cbeCSVTimeFormat.Active = index;
					found = true;
					break;
				}
				index++;
			}
			if (!found)
			{
				cbeCSVTimeFormat.AppendText (con.Configuration.TimeFormat);
				cbeCSVTimeFormat.Active = index;
			}

			index = 0;
			found = false;
			foreach (object[] obj in (ListStore)cbeFileNaming1.Model)
			{
				if (obj [0].ToString () == con.Configuration.FileNameConvention [0])
				{
					cbeFileNaming1.Active = index;
					found = true;
					break;
				}
				index++;
			}
			if (!found)
			{
				cbeFileNaming1.AppendText (con.Configuration.FileNameConvention [0]);
				cbeFileNaming1.Active = index;
			}

			index = 0;
			found = false;
			foreach (object[] obj in (ListStore)cbeFileNaming2.Model)
			{
				if (obj [0].ToString () == con.Configuration.FileNameConvention [1])
				{
					cbeFileNaming2.Active = index;
					found = true;
					break;
				}
				index++;
			}
			if (!found)
			{
				cbeFileNaming2.AppendText (con.Configuration.FileNameConvention [1]);
				cbeFileNaming2.Active = index;
			}

			index = 0;
			found = false;
			foreach (object[] obj in (ListStore)cbeFileNaming3.Model)
			{
				if (obj [0].ToString () == con.Configuration.FileNameConvention [2])
				{
					cbeFileNaming3.Active = index;
					found = true;
					break;
				}
				index++;
			}
			if (!found)
			{
				cbeFileNaming3.AppendText (con.Configuration.FileNameConvention [2]);
				cbeFileNaming3.Active = index;
			}
		}

		/// <summary>
		/// Raises the cb board type changed event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		protected void OnCbBoardTypeChanged (object sender, EventArgs e)
		{
			if (LastActiveBoard != cbBoardType.Active && LastActiveBoard != -1)
			{
				//TODO auf unterschied prüfen. sonst ignorieren
				var dialog = new MessageDialog (this.Toplevel as Gtk.Window, DialogFlags.Modal, MessageType.Info, ButtonsType.YesNo,
					             "The Board Type was changed. If you procede parts of your configuration could get lost, due to incompatibility with the new Board Type.\n Do you wish to procede?");
				dialog.Response += (o, args) =>
				{
					if (args.ResponseId == ResponseType.Yes)
					{
						LastActiveBoard = cbBoardType.Active;
						con.Configuration.Board = con.BoardConfigs [cbBoardType.Active];
						drawingareaMCU.QueueDraw ();
						UpdateAREFList ();
					} else
					{
						cbBoardType.Active = LastActiveBoard;
					}
				};
				dialog.Run ();
				dialog.Destroy ();
			} else
			{
				LastActiveBoard = cbBoardType.Active;
				con.Configuration.Board = con.BoardConfigs [cbBoardType.Active];
				UpdateAREFList ();
			}
		}

		/// <summary>
		/// Updates the AREF list.
		/// </summary>
		private void UpdateAREFList ()
		{
			if (con.Configuration.Board != null)
			{
				var store = new ListStore (typeof(string));

				foreach (string key in con.Configuration.Board.AnalogReferences.Keys)
				{
					store.AppendValues (new object[]{ key });
				}

				cbAREF.Model = store;

				if (con.Configuration.Board.AnalogReferenceVoltage != -1 &&
				    con.Configuration.Board.AnalogReferences.ContainsValue (con.Configuration.Board.AnalogReferenceVoltage))
				{
					int index = con.Configuration.Board.AnalogReferences.Values.ToList ()
						.IndexOf (con.Configuration.Board.AnalogReferenceVoltage);

					cbAREF.Active = index;
				}

				cbAREF.Show ();
			}
		}

		/// <summary>
		/// Raises the cb AREF changed event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		protected void OnCbAREFChanged (object sender, EventArgs e)
		{
			if (cbAREF.ActiveText == "EXTERNAL")
			{
				sbAREFExternal.Sensitive = true;
			} else
			{
				sbAREFExternal.Sensitive = false;
			}
			if (con.Configuration.Board != null)
			{
				if (!sbAREFExternal.Sensitive)
				{
					con.Configuration.Board.AnalogReferenceVoltage = con.Configuration.Board.AnalogReferences.ElementAt (cbAREF.Active).Value;
					sbAREFExternal.Value = con.Configuration.Board.AnalogReferenceVoltage;
				} else
				{
					con.Configuration.Board.AnalogReferenceVoltage = sbAREFExternal.Value;
				}
			}
		}

		/// <summary>
		/// Updates the Toolbar combobox containing the available serial ports
		/// </summary>
		/// <param name="portname">Portname.</param>
		private void UpdatePortBox (string portname = null)
		{
			PortBox.Clear ();
			PortBoxStore = new ListStore (typeof(string));
			PortBox.Model = PortBoxStore;

			CellRendererText crt = new CellRendererText ();
			PortBox.PackStart (crt, true);

			PortBox.AddAttribute (crt, "text", 0);


			var portnames = System.IO.Ports.SerialPort.GetPortNames ();
			foreach (string s in portnames)
			{
				PortBoxStore.AppendValues (new object[]{ s });
			}

			if (ArduinoController.IsConnected)
			{
				if (portnames.Contains (ArduinoController.SerialPortName))
				{
					PortBox.Active = Array.IndexOf (portnames, ArduinoController.SerialPortName);
				}
			} else
			{
				PortBox.Active = -1;
			}

			toolbarMain.ShowAll ();

		}

		/// <summary>
		/// Raised if connection was esablished or lost.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		protected void OnConnection (object sender, ConnectionChangedArgs e)
		{
			if (!e.Handled)
			{
				e.Handled = true;

				UpdatePortBox ();

				if (e.Connected)
				{
					lblConnectionStatus.Text = "connected to " + e.Port;
					refreshAction.Sensitive = false;
					mediaPlayAction.Sensitive = true;
					mediaStopAction.Sensitive = false;


					try
					{
						ImageConnectionStatus.Pixbuf = global::Stetic.IconLoader.LoadIcon (this, "gtk-connect", global::Gtk.IconSize.Menu);
					} catch (Exception ex)
					{
						con.ConLogger.Log (ex.ToString (), LogLevel.ERROR);
					}

					Gtk.Application.Invoke (delegate
					{
						var dialog = new MessageDialog (this, DialogFlags.Modal, MessageType.Info, ButtonsType.Ok,
							             "A connection to serial-port: " + ArduinoController.SerialPortName + " has successfully been established.");
						dialog.Run ();
						dialog.Destroy ();
					});

				} else
				{
					
					lblConnectionStatus.Text = "<b>NOT</b> connected";
					lblConnectionStatus.UseMarkup = true;
					refreshAction.Sensitive = true;
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
			}
		}

		/// <summary>
		/// Raised if window is closing. 
		/// </summary>
		/// <param name="obj">Object.</param>
		/// <param name="a">The alpha component.</param>
		protected void OnDeleteEvent (object obj, DeleteEventArgs a)
		{
			if (con.Configuration.AnalogPins.Count != 0 || con.Configuration.DigitalPins.Count != 0)
			{
				RunQuitSaveDialog ();					
			}

			con.Quit ();
			ArduinoController.Exit ();
			Application.Quit ();
		}

		/// <summary>
		///	Raised when quit key combination was pressed.
		/// Closes program. 
		/// </summary>
		/// <param name="obj">Object.</param>
		/// <param name="a">The alpha component.</param>
		protected void OnKeyPressEvent (object obj, KeyPressEventArgs a)
		{
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

		protected void OnBtnCloneAPinClicked (object sender, EventArgs e)
		{
			APinTreeNode node = (APinTreeNode)nvAnalogPins.NodeSelection.SelectedNode;
			if (node != null && con.Configuration.AvailableAnalogPins.Length > 0)
			{
				con.Configuration.ClonePin (node.Pin);
			}
		}

		protected void OnBtnCloneDPinClicked (object sender, EventArgs e)
		{
			DPinTreeNode node = (DPinTreeNode)nvDigitalPins.NodeSelection.SelectedNode;
			if (node != null && con.Configuration.AvailableDigitalPins.Length > 0)
			{
				con.Configuration.ClonePin (node.Pin);
			}
		}

		protected void OnBtnCloneSignalClicked (object sender, EventArgs e)
		{
			MeasurementCombinationTreeNode node = (MeasurementCombinationTreeNode)nvMeasurementCombinations.NodeSelection.SelectedNode;
			if (node != null && con.Configuration.GetPinsWithoutCombinations ().Length > 0)
			{
				con.Configuration.CloneMeasurementCombination (node.AnalogSignal);
			}
		}

		protected void OnBtnCloneSequenceClicked (object sender, EventArgs e)
		{
			SequenceTreeNode node = (SequenceTreeNode)nvSequences.NodeSelection.SelectedNode;
			if (node != null)
			{
				con.Configuration.CloneSequence (node.Seq);
			}
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
			RunDPinClear ();
		}

		protected void OnBtnRemoveDPinClicked (object sender, EventArgs e)
		{
			DPinTreeNode node = (DPinTreeNode)nvDigitalPins.NodeSelection.SelectedNode;
			if (node != null)
			{
				con.Configuration.RemovePin (node.Pin);
			}
		}

		protected void OnBtnRemoveAPinClicked (object sender, EventArgs e)
		{
			APinTreeNode node = (APinTreeNode)nvAnalogPins.NodeSelection.SelectedNode;
			if (node != null)
			{
				con.Configuration.RemovePin (node.Pin);
			}
		}

		protected void OnBtnRemoveSignalClicked (object sender, EventArgs e)
		{
			MeasurementCombinationTreeNode node = (MeasurementCombinationTreeNode)nvMeasurementCombinations.NodeSelection.SelectedNode;
			if (node != null)
			{
				con.Configuration.RemoveMeasurementCombination (node.AnalogSignal);
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
			RunMeasurementCombinationClear ();
		}

		protected void OnBtnClearAPinsClicked (object sender, EventArgs e)
		{
			RunAPinClear ();
		}

		protected void OnBtnClearSequenceClicked (object sender, EventArgs e)
		{
			RunSequenceClear ();
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

		protected void OnBtnCSVFilePathOpenClicked (object sender, EventArgs e)
		{
			var dialog = new FileChooserDialog ("Select a folder", this, FileChooserAction.SelectFolder, "Cancle", ResponseType.Cancel, "Select", ResponseType.Apply);
			dialog.Response += (o, args) =>
			{
				if (args.ResponseId == ResponseType.Apply)
				{
					con.Configuration.CSVSaveFolderPath = dialog.CurrentFolder;

					eCSVFilePath.Text = con.Configuration.CSVSaveFolderPath;

					UpdateFilePathPreview ();
				}
			};
			dialog.Run ();
			dialog.Destroy ();
		}

		protected void OnCbeCSVSeparatorChanged (object sender, EventArgs e)
		{
			con.Configuration.Separator = cbeCSVSeparator.ActiveText;
		}

		protected void OnCbCSVUTCToggled (object sender, EventArgs e)
		{
			con.Configuration.UTCTimestamp = cbCSVUTC.Active;
		}

		protected void OnCbeCSVTimeFormatChanged (object sender, EventArgs e)
		{
			con.Configuration.TimeFormat = cbeCSVTimeFormat.ActiveText;
		}

		protected void OnBtnRealTimePlotPauseClicked (object sender, EventArgs e)
		{
			RealTimePlotUpdate = (!RealTimePlotUpdate);
		}

		protected void OnBtnRealTimePlotSnapshotClicked (object sender, EventArgs e)
		{
			string path = "";
			if (!string.IsNullOrEmpty (con.Configuration.CSVSaveFolderPath))
			{
				path += con.Configuration.CSVSaveFolderPath;
				if (Environment.OSVersion.Platform == PlatformID.Win32NT)
				{
					path += @"\";
				} else if (Environment.OSVersion.Platform == PlatformID.Unix || Environment.OSVersion.Platform == PlatformID.MacOSX)
				{
					path += @"/";
				}
			}
				
			PngExporter.Export (
				RealTimePlotView.Model,
				string.Format (
					"{0}{1}_{2}.png",
					path,
					string.Format (
						"{0:D2}-{1:D2}-{2:D2}_{3:D2}_{4:D2}_{5:D2}",
						DateTime.Now.Year,
						DateTime.Now.Month,
						DateTime.Now.Day,
						DateTime.Now.Hour,
						DateTime.Now.Minute,
						DateTime.Now.Second),
					"Snapshot"
				), 
				RealTimePlotView.Allocation.Width,
				RealTimePlotView.Allocation.Height
			);
		}

		protected void OnCbtnRealTimePlotShowMarkerToggled (object sender, EventArgs e)
		{
			ToggleRealTimePlotMarker ();	
		}

		protected void OnCbtnRealTimePlotSmoothValues (object sender, EventArgs e)
		{
			ToggleRealTimePlotSmooth ();
		}

		protected void OnCbtnRealTimePlotLimitPoints (object sender, EventArgs e)
		{
			Frontend.Settings.Default.LimitPlotPoints = cbtnRealTimePlotLimitPoints.Active;
			Frontend.Settings.Default.Save ();

//			if (!Frontend.Settings.Default.LimitPlotPoints && con.IsRunning)
//			{
//				foreach (LineSeries series in RealTimePlotModel.Series)
//				{
//					lock (series)
//					{
//						var hits = con.Configuration.AnalogPins.Where (o => o.DisplayName.Equals (series.Title)).ToList ();
//						if (hits.Count > 0)
//						{
//							APin pin = hits [0];
//							series.Points.Clear ();
//							pin.Values.ForEach (o => series.Points.Add (new DataPoint (o.Time, o.Value)));
//						} else
//						{
//							var mecomhit = con.Configuration.MeasurementCombinations.Where (o => o.DisplayName.Equals (series.Title)).ToList ();
//							if (mecomhit.Count > 0)
//							{
//								MeasurementCombination pin = mecomhit [0];
//								series.Points.Clear ();
//								pin.Values.ForEach (o => series.Points.Add (new DataPoint (o.Time, o.Value)));
//							} 
//						}
//					}
//				}
//			}
		}

		protected void OnBtnRealTimePlotJumpStartClicked (object sender, EventArgs e)
		{
			if (RealTimeXAxis.ActualMinimum > con.StartTime.ToOADate ())
			{
				RealTimeXAxis.Pan (new ScreenPoint (RealTimeXAxis.Transform (con.StartTime.ToOADate ()), 0), new ScreenPoint (RealTimeXAxis.Transform (RealTimeXAxis.ActualMinimum), 0));
			} else
			{
				RealTimeXAxis.Pan (new ScreenPoint (RealTimeXAxis.Transform (RealTimeXAxis.ActualMinimum), 0), new ScreenPoint (RealTimeXAxis.Transform (con.StartTime.ToOADate ()), 0));
			}
		}

		protected void OnBtnRealTimePlotJumpLatestClicked (object sender, EventArgs e)
		{
			if (RealTimeXAxis.ActualMaximum > LastTimeKeeperPresenterTick)
			{
				RealTimeXAxis.Pan (new ScreenPoint (RealTimeXAxis.Transform (RealTimeXAxis.ActualMaximum), 0), new ScreenPoint (RealTimeXAxis.Transform (LastTimeKeeperPresenterTick), 0));
			} else
			{
				RealTimeXAxis.Pan (new ScreenPoint (RealTimeXAxis.Transform (LastTimeKeeperPresenterTick), 0), new ScreenPoint (RealTimeXAxis.Transform (RealTimeXAxis.ActualMaximum), 0));
			}
		}

		protected void OnBtnRealTimePlotFitDataClicked (object sender, EventArgs e)
		{
			RealTimeXAxis.Zoom (con.StartTime.ToOADate (), LastTimeKeeperPresenterTick);
		}

		protected void OnBtnRealTimePlotResetZoomClicked (object sender, EventArgs e)
		{
			RealTimeXAxis.Zoom (DefaultZoomValue);
		}

		protected void OnNewPoint (object sender, NewMeasurementValueArgs e, ref LineSeries series)
		{
			Console.WriteLine ("New Value\t" + (sender as APin).DisplayName);
			if (Frontend.Settings.Default.LimitPlotPoints)
			{
				if (series.Points.Count > Frontend.Settings.Default.MaximumSeriesSize)
				{
					series.Points.RemoveRange (0, series.Points.Count - Frontend.Settings.Default.MaximumSeriesSize);
				}
			} 
			if (!double.IsNaN (e.Value))
			{
				series.Points.Add (new DataPoint (e.Time.ToOADate (), e.Value));
			}
		}

		#endregion

		/// <summary>
		/// Toggles the real time plot marker.
		/// </summary>
		private void ToggleRealTimePlotMarker ()
		{
			RealTimePlotView.Model.Series.ToList ().ForEach (o => (o as LineSeries).MarkerFill = (o as LineSeries).Color);
			RealTimePlotView.Model.Series.ToList ().ForEach (o => (o as LineSeries).MarkerStroke = (o as LineSeries).Color);
			if (cbtnRealTimePlotShowMarker.Active)
			{
				RealTimePlotView.Model.Series.ToList ().ForEach (o => (o as LineSeries).MarkerType = MarkerType.Cross);
			} else
			{
				RealTimePlotView.Model.Series.ToList ().ForEach (o => (o as LineSeries).MarkerType = MarkerType.None);
			}
		}

		/// <summary>
		/// Toggles the real time plot smoothing.
		/// </summary>
		private void ToggleRealTimePlotSmooth ()
		{
			RealTimePlotView.Model.Series.ToList ().ForEach (o => (o as LineSeries).Smooth = cbtnRealTimePlotSmoothValues.Active);
		}

		#region Drawing

		void DrawMCU (object sender, ExposeEventArgs args)
		{
			var context = CairoHelper.Create (this.drawingareaMCU.GdkWindow);

			//Weiß
			context.SetSourceRGB (1, 1, 1);
			context.Rectangle (0, 0, this.drawingareaMCU.Allocation.Width, this.drawingareaMCU.Allocation.Height);
			context.Fill ();

			MCUDisplayHelper.ShiftX = this.drawingareaMCU.Allocation.Width / 2;
			MCUDisplayHelper.ShiftY = this.drawingareaMCU.Allocation.Height / 2;
			MCUDisplayHelper.PinLocations = con.Configuration.Board.PinLocation;

			MCUDisplayHelper.SetMCUSurface (context, con.Configuration.Board.ImageFilePath);

			MCUDisplayHelper.SetPinLabels (
				context,
				con.Configuration.LeftPinLayout,
				0,
				(this.drawingareaMCU.Allocation.Height / 2) - ((MCUDisplayHelper.FlatHeight * con.Configuration.LeftPinLayout.Count) / 2),
				LabelPosition.Left
			);

			MCUDisplayHelper.SetPinLabels (
				context, 
				con.Configuration.RightPinLayout,
				hpanedMain.Position - MCUDisplayHelper.LabelWidth,
				(this.drawingareaMCU.Allocation.Height / 2) - ((MCUDisplayHelper.FlatHeight * con.Configuration.RightPinLayout.Count) / 2),
				LabelPosition.Right
			);

			MCUDisplayHelper.SetPinLabels (
				context,
				con.Configuration.BottomPinLayout,
				0,
				(this.drawingareaMCU.Allocation.Height / 3) * 2,
				LabelPosition.Bottom
			);

			((IDisposable)context.GetTarget ()).Dispose ();
			context.Dispose ();
		}

		#endregion

		#region RunDialogs

		private void RunOpenConfig (string path = null)
		{
			try
			{
				string location = path;
				if (location == null)
				{
					location = RunOpenDialog ();
				}
				if (con.OpenConfiguration (location))
				{
					UpdateAllNodes ();
					BindControllerEvents ();
				} else
				{
					var dialog = new MessageDialog (
						             this,
						             DialogFlags.Modal,
						             MessageType.Error,
						             ButtonsType.Ok,
						             "Unable to load configuration \n" +
						             "(" + location + ").\n " +
						             "Please make shure that the file exsists and you have read access.");
					dialog.Run ();
					dialog.Destroy ();
				}
			} catch (Exception ex)
			{
				con.ConLogger.Log (ex.ToString (), LogLevel.ERROR);
				var dialog = new MessageDialog (
					             this,
					             DialogFlags.Modal,
					             MessageType.Error,
					             ButtonsType.Ok,
					             @"Unable to load configuration\n" +
					             @"(" + path + @").\n" +
					             "Please make shure that the file exsists and you have read access.");
				dialog.Run ();
				dialog.Destroy ();
			}
		}

		private void RunAPinClear ()
		{
			var message = new MessageDialog (
				              this,
				              DialogFlags.Modal,
				              MessageType.Warning,
				              ButtonsType.YesNo,
				              "You are attemting to delete all Measurements.\nThis will also lead to the removal of every MeasurementCombination.\n\nDo you want to procede?"
			              );
			message.Response += (o, args) =>
			{
				if (args.ResponseId == ResponseType.Yes)
				{
					con.Configuration.ClearPins (PinType.ANALOG);
				}
			};
			message.Run ();
			message.Destroy ();
		}

		private void RunDPinClear ()
		{
			var message = new MessageDialog (
				              this,
				              DialogFlags.Modal,
				              MessageType.Warning,
				              ButtonsType.YesNo,
				              "You are attemting to delete all Outputs.\nThis will also lead to the removal of every Sequences.\n\nDo you want to procede?"
			              );
			message.Response += (o, args) =>
			{
				if (args.ResponseId == ResponseType.Yes)
				{
					con.Configuration.ClearPins (PinType.DIGITAL);
				}
			};
			message.Run ();
			message.Destroy ();
		}

		private void RunMeasurementCombinationClear ()
		{
			var message = new MessageDialog (
				              this,
				              DialogFlags.Modal,
				              MessageType.Warning,
				              ButtonsType.YesNo,
				              "You are attemting to delete all MeasurementCombinations.\n\nDo you want to procede?"
			              );
			message.Response += (o, args) =>
			{
				if (args.ResponseId == ResponseType.Yes)
				{
					con.Configuration.ClearMeasurementCombinations ();
				}
			};
			message.Run ();
			message.Destroy ();
		}

		private void RunSequenceClear ()
		{
			var message = new MessageDialog (
				              this,
				              DialogFlags.Modal,
				              MessageType.Warning,
				              ButtonsType.YesNo,
				              "You are attemting to delete all Sequences.\n\nDo you want to procede?"
			              );
			message.Response += (o, args) =>
			{
				if (args.ResponseId == ResponseType.Yes)
				{
					con.Configuration.ClearSequences ();
				}
			};
			message.Run ();
			message.Destroy ();
		}

		private void RunAddDPinDialog (DPin pin = null)
		{
			var dings = con.Configuration.AvailableDigitalPins;

			DPinConfigDialog dialog = new DPinConfigDialog (dings, pin, this);

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
								con.Configuration.EditPin (i, dialog.Pin);
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
			APinConfigDialog dialog = new APinConfigDialog (con.Configuration.AvailableAnalogPins, pin, this, this.Units);
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
								con.Configuration.EditPin (i, dialog.Pin);
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
			SeqConfigDialog dialog;
			if (seq != null)
			{
				dialog = new SeqConfigDialog (
					con.Configuration.GetPinsWithoutSequence (), 
					con.Configuration.SequenceGroups, 
					new Sequence (seq), 
					RefPin, 
					this
				);
			} else
			{
				dialog = new SeqConfigDialog (
					con.Configuration.GetPinsWithoutSequence (), 
					con.Configuration.SequenceGroups, 
					null, 
					RefPin, 
					this
				);
			}

			dialog.Response += (o, args) =>
			{
				if (args.ResponseId == ResponseType.Apply)
				{
					if (seq == null)
					{
						con.Configuration.AddSequence (dialog.PinSequence);
					} else
					{
						con.Configuration.EditSequence (con.Configuration.Sequences.IndexOf (seq), dialog.PinSequence);
					}
				}
			};
			dialog.Run ();
			dialog.Destroy ();
		}

		private void RunSequenceGroupDelete (string group)
		{
			var dialog = new MessageDialog (
				             this,
				             DialogFlags.Modal,
				             MessageType.Warning,
				             ButtonsType.YesNo,
				             "You are about to delete every sequence related to this group.\nDo you wish to proceed?");
			dialog.Response += (o, args) =>
			{
				if (args.ResponseId == ResponseType.Yes)
				{
					con.Configuration.RemoveSequenceGroup (group);
				}
			};
			dialog.Run ();
			dialog.Destroy ();
		}

		private void RunMeasurementCombinationDialog (MeasurementCombination sig = null, APin refPin = null)
		{
			var dialog = new AComConfigDialog (con.Configuration.GetPinsWithoutCombinations (), sig, refPin, this, this.Units);
			dialog.Response += (o, args) =>
			{
				if (args.ResponseId == ResponseType.Apply)
				{
					if (sig == null)
					{
						con.Configuration.AddMeasurementCombination (dialog.Combination);
					} else
					{
						con.Configuration.EditMeasurmentCombination (con.Configuration.MeasurementCombinations.IndexOf (sig), dialog.Combination);
					}
				}
			};
			dialog.Run ();
			dialog.Destroy ();
		}

		protected  void RunPreferencesDialog (object sender = null, EventArgs e = null)
		{
			var dialog = new Frontend.PreferencesDialog (this, this.con);
			dialog.Run ();
			dialog.Destroy ();
		}

		protected string RunSaveDialog (object sender = null, EventArgs e = null)
		{
			string path = string.Empty;

			var dialog = new FileChooserDialog ("Select save loaction", this, FileChooserAction.Save, "Cancel", ResponseType.Cancel, "Save", ResponseType.Apply);
			dialog.Response += (o, args) =>
			{
				path = dialog.Filename;
			};
			dialog.Run ();
			dialog.Destroy ();

			return path;
		}

		protected string RunOpenDialog (string folder = null)
		{
			string path = string.Empty;

			var dialog = new FileChooserDialog ("select a configuration", this, FileChooserAction.Open, "Cancel", ResponseType.Cancel, "Open", ResponseType.Apply);
			dialog.Response += (o, args) =>
			{
				path = dialog.Filename;
			};
			dialog.Run ();
			dialog.Destroy ();

			return path;
		}

		protected void RunQuitSaveDialog ()
		{
			var dialog = new MessageDialog (this, DialogFlags.Modal, MessageType.Question, ButtonsType.YesNo, "Do you want to save the current configuration?");
			dialog.Response += (o, args) =>
			{
				if (args.ResponseId == ResponseType.Yes)
				{
					saveAction.Activate ();
				}
			};
			dialog.Run ();
			dialog.Destroy ();
		}

		/// <summary>
		/// Shows the configuration loaded message.
		/// </summary>
		/// <param name="path">Path.</param>
		/// <param name="success">If set to <c>true</c> success.</param>
		private void ShowConfigurationLoadedMessage (string path, bool success)
		{
			string msg = "";

			msg += path;
			msg += "\n";

			if (success)
			{
				msg += "Successfully loaded.";
			} else
			{
				msg += "Not loaded.";
			}

			Application.Invoke (delegate
			{
				var dialog = new MessageDialog (this, DialogFlags.Modal, MessageType.Info, ButtonsType.Close, msg);
				dialog.Run ();
				dialog.Destroy ();
			});
		}

		private void OpenCSVDirectory ()
		{
			if (Environment.OSVersion.Platform == PlatformID.Win32NT)
			{
				ProcessStartInfo startInfo = new ProcessStartInfo ();
				startInfo.FileName = "explorer.exe";
				startInfo.Arguments = con.Configuration.CSVSaveFolderPath;
				Process.Start (startInfo);
			} else if (Environment.OSVersion.Platform == PlatformID.Unix || Environment.OSVersion.Platform == PlatformID.MacOSX)
			{
				ProcessStartInfo startInfo = new ProcessStartInfo ();
				startInfo.FileName = "xdg-open";
				startInfo.Arguments = con.Configuration.CSVSaveFolderPath;
				Process.Start (startInfo);
			}
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
				InitRealTimePlot ();
				TimeKeeperPresenter.Start ();

				con.Start ();
				lblStartTime.Text = string.Format ("{0:yyyy-MM-dd HH:mm:ss}", con.StartTime);
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
			btnCloneAPin.Sensitive = sensitive;
			btnCloneDPin.Sensitive = sensitive;
			btnCloneSequence.Sensitive = sensitive;
			btnCloneSignal.Sensitive = sensitive;

			nvAnalogPins.Sensitive = sensitive;
			nvDigitalPins.Sensitive = sensitive;
			nvSequences.Sensitive = sensitive;
			nvMeasurementCombinations.Sensitive = sensitive;

			SequencePreviewPlotView.Sensitive = sensitive;
		}

		private void UpdateFilePathPreview ()
		{
			string preview = string.Empty;

			if (!string.IsNullOrEmpty (con.Configuration.CSVSaveFolderPath))
			{
				preview += con.Configuration.CSVSaveFolderPath;
				if (Environment.OSVersion.Platform == PlatformID.Unix)
				{
					preview += "/";
				} else if (Environment.OSVersion.Platform == PlatformID.Win32NT)
				{
					preview += @"\";
				}
			} 

			preview += con.Configuration.GetCSVLogName ();

			lblPreviewFilePathFormat.Text = preview;
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

					seq1.AddSequenceOperation (new SequenceOperation () {
						Duration = TimeSpan.FromMilliseconds (1000),
						State = DPinState.HIGH
					});
					seq1.AddSequenceOperation (new SequenceOperation () {
						Duration = TimeSpan.FromMilliseconds (1000),
						State = DPinState.LOW
					});
					seq2.AddSequenceOperation (new SequenceOperation () {
						Duration = TimeSpan.FromMilliseconds (1000),
						State = DPinState.LOW
					});
					seq2.AddSequenceOperation (new SequenceOperation () {
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

				seq.AddSequenceOperation (new SequenceOperation () {
					Duration = TimeSpan.FromSeconds (i / 100.0),
					State = DPinState.LOW
				});

				for (int j = 0; j < 100; j++)
				{
					seq.AddSequenceOperation (new SequenceOperation () {
						Duration = TimeSpan.FromSeconds (con.Configuration.Pins.Count / 100.0),
						State = DPinState.HIGH
					});
					seq.AddSequenceOperation (new SequenceOperation () {
						Duration = TimeSpan.FromSeconds (con.Configuration.Pins.Count / 100.0),
						State = DPinState.LOW
					});
				}

				con.Configuration.AddSequence (seq);

				i += 1;
			}
			Console.WriteLine ();
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

					seq1.AddSequenceOperation (new SequenceOperation () {
						Duration = TimeSpan.FromMilliseconds (1000),
						State = DPinState.HIGH
					});
					seq1.AddSequenceOperation (new SequenceOperation () {
						Duration = TimeSpan.FromMilliseconds (1000),
						State = DPinState.LOW
					});
					seq2.AddSequenceOperation (new SequenceOperation () {
						Duration = TimeSpan.FromMilliseconds (1000),
						State = DPinState.LOW
					});
					seq2.AddSequenceOperation (new SequenceOperation () {
						Duration = TimeSpan.FromMilliseconds (1000),
						State = DPinState.HIGH
					});
				}
				con.Configuration.AddSequence (seq1);
				con.Configuration.AddSequence (seq2);
				i += 2;
			}
		}

		protected void OnBtnBoardDifferenceTestClicked (object sender, EventArgs e)
		{
			con.Configuration.Board = con.BoardConfigs [1];
		}

		protected void OnBtnRefreshNVClicked (object sender, EventArgs e)
		{
			UpdateAllNodes ();
		}

		protected void OnButton1125Clicked (object sender, EventArgs e)
		{

			ArduinoController.SetPinModes (
				new uint[]{ 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19 },
				new uint[]{ 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19 }
			);

			while (true)
			{
				ArduinoController.SetDigitalOutputPins ((UInt64)699050);
				System.Threading.Thread.Sleep (500);
				ArduinoController.SetDigitalOutputPins ((UInt64)349525);
				System.Threading.Thread.Sleep (500);
			}
		}

		#endregion

		#region ActionEvents

		protected void OnSaveActionActivated (object sender, EventArgs e)
		{
			if (!string.IsNullOrEmpty (con.Configuration.ConfigSavePath))
			{
//				string path = RunSaveDialog ();
				con.SaveConfiguration ();
			} else
			{
				OnSaveAsActionActivated (sender, e);
			}
		}

		protected void OnSaveAsActionActivated (object sender, EventArgs e)
		{
			string path = RunSaveDialog ();
			if (!string.IsNullOrEmpty (path))
			{
				if (!path.Contains (@".mc"))
				{
					path += @".mc";
				}
				con.Configuration.ConfigSavePath = path;
				con.SaveConfiguration (path);
			}
		}

		protected void OnOpenActionActivated (object sender, EventArgs e)
		{
			RunOpenConfig ();			
		}

		protected void OnAboutActionActivated (object sender, EventArgs e)
		{
			var dialog = new AboutDialog () {
				Authors = new string[]{ "Daniel Pollack (danielpollack@posteo.de)" },
				License = "The MIT License (MIT) \n\nCopyright (c) 2016 Daniel Pollack \n\nPermission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the \"Software\"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:\n\nThe above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.\n\nTHE SOFTWARE IS PROVIDED \"AS IS\", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.\n",
				ProgramName = Backend.Controller.SoftwareName,
				Modal = true,
				DefaultWidth = 300
			};
			dialog.Run ();
			dialog.Destroy ();
		}

		protected void OnPreferencesActionActivated (object sender, EventArgs e)
		{
			RunPreferencesDialog ();
		}

		protected void OnBtnCSVOpenFolderClicked (object sender, EventArgs e)
		{
			if (System.IO.File.Exists (con.Configuration.CSVSaveFolderPath))
			{
				System.Diagnostics.Process pr = new System.Diagnostics.Process ();
				pr.StartInfo.FileName = @con.Configuration.CSVSaveFolderPath.Remove (con.Configuration.CSVSaveFolderPath.Length - 1, 1);
				pr.Start ();
			}
		}

		protected void OnNewActionActivated (object sender, EventArgs e)
		{
			throw new NotImplementedException ();
		}

		#endregion
	}

}
