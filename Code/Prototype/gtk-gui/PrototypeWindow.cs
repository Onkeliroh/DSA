
// This file has been generated by the GUI designer. Do not modify.

public partial class PrototypeWindow
{
	private global::Gtk.UIManager UIManager;
	
	private global::Gtk.Action FileAction;
	
	private global::Gtk.Action EditAction;
	
	private global::Gtk.Action HelpAction;
	
	private global::Gtk.Action aboutAction;
	
	private global::Gtk.Action newAction;
	
	private global::Gtk.Action Action;
	
	private global::Gtk.Action quitAction;
	
	private global::Gtk.Action saveAction;
	
	private global::Gtk.Action preferencesAction;
	
	private global::Gtk.Action disconnectAction;
	
	private global::Gtk.Action ConnectionAction;
	
	private global::Gtk.Action DeviceTypeAction;
	
	private global::Gtk.Action BaudrateAction;
	
	private global::Gtk.RadioAction Action300Baud;
	
	private global::Gtk.RadioAction Action1200Baud;
	
	private global::Gtk.RadioAction Action2400Baud;
	
	private global::Gtk.RadioAction Action4800Baud;
	
	private global::Gtk.RadioAction Action9600Baud;
	
	private global::Gtk.RadioAction Action19200Baud;
	
	private global::Gtk.RadioAction Action38400Baud;
	
	private global::Gtk.RadioAction Action57600Baud;
	
	private global::Gtk.RadioAction Action115200Baud;
	
	private global::Gtk.Action PortsAction;
	
	private global::Gtk.VBox vboxMain;
	
	private global::Gtk.MenuBar menubar1;
	
	private global::Gtk.HBox hboxGreetings;
	
	private global::Gtk.Button btnNewConfig;
	
	private global::Gtk.Button btnOpenConfig;
	
	private global::Gtk.Table tableConnection;
	
	private global::Gtk.Button btnConnect;
	
	private global::Gtk.Button btnRefresh;
	
	private global::Gtk.ComboBox cbConnectPorts;
	
	private global::Gtk.Label label2;
	
	private global::Gtk.Label label3;
	
	private global::Gtk.Label lblConnectionPorts;
	
	private global::Gtk.Table tableConfig;
	
	private global::Gtk.Button btnConfigBack;
	
	private global::Gtk.Button btnConfigRun;
	
	private global::Gtk.ScrolledWindow ScrolledWindowConfigVBox;
	
	private global::Gtk.VBox vboxConfig;
	
	private global::Gtk.VBox vboxPlot;
	
	private global::Gtk.Button btnPlotBack;
	
	private global::Gtk.Statusbar statusbar1;

	protected virtual void Build ()
	{
		global::Stetic.Gui.Initialize (this);
		// Widget PrototypeWindow
		this.UIManager = new global::Gtk.UIManager ();
		global::Gtk.ActionGroup w1 = new global::Gtk.ActionGroup ("Default");
		this.FileAction = new global::Gtk.Action ("FileAction", global::Mono.Unix.Catalog.GetString ("File"), null, null);
		this.FileAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("File");
		w1.Add (this.FileAction, null);
		this.EditAction = new global::Gtk.Action ("EditAction", global::Mono.Unix.Catalog.GetString ("Edit"), null, null);
		this.EditAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("Edit");
		w1.Add (this.EditAction, null);
		this.HelpAction = new global::Gtk.Action ("HelpAction", global::Mono.Unix.Catalog.GetString ("Help"), null, null);
		this.HelpAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("Help");
		w1.Add (this.HelpAction, null);
		this.aboutAction = new global::Gtk.Action ("aboutAction", global::Mono.Unix.Catalog.GetString ("_About"), null, "gtk-about");
		this.aboutAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("_About");
		w1.Add (this.aboutAction, null);
		this.newAction = new global::Gtk.Action ("newAction", global::Mono.Unix.Catalog.GetString ("_New"), null, "gtk-new");
		this.newAction.Sensitive = false;
		this.newAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("_New");
		w1.Add (this.newAction, null);
		this.Action = new global::Gtk.Action ("Action", global::Mono.Unix.Catalog.GetString ("-"), null, null);
		this.Action.ShortLabel = global::Mono.Unix.Catalog.GetString ("-");
		w1.Add (this.Action, null);
		this.quitAction = new global::Gtk.Action ("quitAction", global::Mono.Unix.Catalog.GetString ("_Quit"), null, "gtk-quit");
		this.quitAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("_Quit");
		w1.Add (this.quitAction, null);
		this.saveAction = new global::Gtk.Action ("saveAction", global::Mono.Unix.Catalog.GetString ("_Save"), null, "gtk-save");
		this.saveAction.Sensitive = false;
		this.saveAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("_Save");
		w1.Add (this.saveAction, null);
		this.preferencesAction = new global::Gtk.Action ("preferencesAction", global::Mono.Unix.Catalog.GetString ("_Preferences"), null, "gtk-preferences");
		this.preferencesAction.Sensitive = false;
		this.preferencesAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("_Preferences");
		w1.Add (this.preferencesAction, null);
		this.disconnectAction = new global::Gtk.Action ("disconnectAction", global::Mono.Unix.Catalog.GetString ("_Disconnect"), null, "gtk-disconnect");
		this.disconnectAction.Sensitive = false;
		this.disconnectAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("_Disconnect");
		w1.Add (this.disconnectAction, null);
		this.ConnectionAction = new global::Gtk.Action ("ConnectionAction", global::Mono.Unix.Catalog.GetString ("Connection"), null, null);
		this.ConnectionAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("Connection");
		w1.Add (this.ConnectionAction, null);
		this.DeviceTypeAction = new global::Gtk.Action ("DeviceTypeAction", global::Mono.Unix.Catalog.GetString ("Device Type"), null, null);
		this.DeviceTypeAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("Device Type");
		w1.Add (this.DeviceTypeAction, null);
		this.BaudrateAction = new global::Gtk.Action ("BaudrateAction", global::Mono.Unix.Catalog.GetString ("Baudrate"), null, null);
		this.BaudrateAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("Baudrate");
		w1.Add (this.BaudrateAction, null);
		this.Action300Baud = new global::Gtk.RadioAction ("Action300Baud", global::Mono.Unix.Catalog.GetString ("300"), null, null, 0);
		this.Action300Baud.Group = new global::GLib.SList (global::System.IntPtr.Zero);
		this.Action300Baud.ShortLabel = global::Mono.Unix.Catalog.GetString ("300");
		w1.Add (this.Action300Baud, null);
		this.Action1200Baud = new global::Gtk.RadioAction ("Action1200Baud", global::Mono.Unix.Catalog.GetString ("1200"), null, null, 0);
		this.Action1200Baud.Group = this.Action300Baud.Group;
		this.Action1200Baud.ShortLabel = global::Mono.Unix.Catalog.GetString ("1200");
		w1.Add (this.Action1200Baud, null);
		this.Action2400Baud = new global::Gtk.RadioAction ("Action2400Baud", global::Mono.Unix.Catalog.GetString ("2400"), null, null, 0);
		this.Action2400Baud.Group = this.Action1200Baud.Group;
		this.Action2400Baud.ShortLabel = global::Mono.Unix.Catalog.GetString ("2400");
		w1.Add (this.Action2400Baud, null);
		this.Action4800Baud = new global::Gtk.RadioAction ("Action4800Baud", global::Mono.Unix.Catalog.GetString ("4800"), null, null, 0);
		this.Action4800Baud.Group = this.Action1200Baud.Group;
		this.Action4800Baud.ShortLabel = global::Mono.Unix.Catalog.GetString ("4800");
		w1.Add (this.Action4800Baud, null);
		this.Action9600Baud = new global::Gtk.RadioAction ("Action9600Baud", global::Mono.Unix.Catalog.GetString ("9600"), null, null, 0);
		this.Action9600Baud.Group = this.Action1200Baud.Group;
		this.Action9600Baud.ShortLabel = global::Mono.Unix.Catalog.GetString ("9600");
		w1.Add (this.Action9600Baud, null);
		this.Action19200Baud = new global::Gtk.RadioAction ("Action19200Baud", global::Mono.Unix.Catalog.GetString ("19200"), null, null, 0);
		this.Action19200Baud.Group = this.Action1200Baud.Group;
		this.Action19200Baud.ShortLabel = global::Mono.Unix.Catalog.GetString ("19200");
		w1.Add (this.Action19200Baud, null);
		this.Action38400Baud = new global::Gtk.RadioAction ("Action38400Baud", global::Mono.Unix.Catalog.GetString ("38400"), null, null, 0);
		this.Action38400Baud.Group = this.Action1200Baud.Group;
		this.Action38400Baud.ShortLabel = global::Mono.Unix.Catalog.GetString ("38400");
		w1.Add (this.Action38400Baud, null);
		this.Action57600Baud = new global::Gtk.RadioAction ("Action57600Baud", global::Mono.Unix.Catalog.GetString ("57600"), null, null, 0);
		this.Action57600Baud.Group = this.Action1200Baud.Group;
		this.Action57600Baud.ShortLabel = global::Mono.Unix.Catalog.GetString ("57600");
		w1.Add (this.Action57600Baud, null);
		this.Action115200Baud = new global::Gtk.RadioAction ("Action115200Baud", global::Mono.Unix.Catalog.GetString ("115200"), null, null, 0);
		this.Action115200Baud.Group = this.Action1200Baud.Group;
		this.Action115200Baud.ShortLabel = global::Mono.Unix.Catalog.GetString ("115200");
		w1.Add (this.Action115200Baud, null);
		this.PortsAction = new global::Gtk.Action ("PortsAction", global::Mono.Unix.Catalog.GetString ("Port(s)"), null, null);
		this.PortsAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("Port(s)");
		w1.Add (this.PortsAction, null);
		this.UIManager.InsertActionGroup (w1, 0);
		this.AddAccelGroup (this.UIManager.AccelGroup);
		this.Name = "PrototypeWindow";
		this.Title = global::Mono.Unix.Catalog.GetString ("MainWindow");
		this.WindowPosition = ((global::Gtk.WindowPosition)(1));
		this.AllowShrink = true;
		// Container child PrototypeWindow.Gtk.Container+ContainerChild
		this.vboxMain = new global::Gtk.VBox ();
		this.vboxMain.Name = "vboxMain";
		this.vboxMain.Spacing = 6;
		// Container child vboxMain.Gtk.Box+BoxChild
		this.UIManager.AddUiFromString (@"<ui><menubar name='menubar1'><menu name='FileAction' action='FileAction'><menuitem name='newAction' action='newAction'/><menuitem name='saveAction' action='saveAction'/><separator/><menuitem name='disconnectAction' action='disconnectAction'/><menuitem name='quitAction' action='quitAction'/></menu><menu name='ConnectionAction' action='ConnectionAction'><menu name='DeviceTypeAction' action='DeviceTypeAction'/><menu name='BaudrateAction' action='BaudrateAction'><menuitem name='Action300Baud' action='Action300Baud'/><menuitem name='Action1200Baud' action='Action1200Baud'/><menuitem name='Action2400Baud' action='Action2400Baud'/><menuitem name='Action4800Baud' action='Action4800Baud'/><menuitem name='Action9600Baud' action='Action9600Baud'/><menuitem name='Action19200Baud' action='Action19200Baud'/><menuitem name='Action38400Baud' action='Action38400Baud'/><menuitem name='Action57600Baud' action='Action57600Baud'/><menuitem name='Action115200Baud' action='Action115200Baud'/></menu><menu name='PortsAction' action='PortsAction'/></menu><menu name='EditAction' action='EditAction'><menuitem name='preferencesAction' action='preferencesAction'/></menu><menu name='HelpAction' action='HelpAction'><menuitem name='aboutAction' action='aboutAction'/></menu></menubar></ui>");
		this.menubar1 = ((global::Gtk.MenuBar)(this.UIManager.GetWidget ("/menubar1")));
		this.menubar1.Name = "menubar1";
		this.vboxMain.Add (this.menubar1);
		global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(this.vboxMain [this.menubar1]));
		w2.Position = 0;
		w2.Expand = false;
		w2.Fill = false;
		// Container child vboxMain.Gtk.Box+BoxChild
		this.hboxGreetings = new global::Gtk.HBox ();
		this.hboxGreetings.Name = "hboxGreetings";
		this.hboxGreetings.Homogeneous = true;
		this.hboxGreetings.Spacing = 6;
		// Container child hboxGreetings.Gtk.Box+BoxChild
		this.btnNewConfig = new global::Gtk.Button ();
		this.btnNewConfig.WidthRequest = 300;
		this.btnNewConfig.HeightRequest = 200;
		this.btnNewConfig.CanFocus = true;
		this.btnNewConfig.Name = "btnNewConfig";
		this.btnNewConfig.UseUnderline = true;
		this.btnNewConfig.Label = global::Mono.Unix.Catalog.GetString ("Create a new configuration");
		global::Gtk.Image w3 = new global::Gtk.Image ();
		w3.Pixbuf = global::Stetic.IconLoader.LoadIcon (this, "gtk-new", global::Gtk.IconSize.Menu);
		this.btnNewConfig.Image = w3;
		this.hboxGreetings.Add (this.btnNewConfig);
		global::Gtk.Box.BoxChild w4 = ((global::Gtk.Box.BoxChild)(this.hboxGreetings [this.btnNewConfig]));
		w4.Position = 0;
		w4.Expand = false;
		w4.Fill = false;
		// Container child hboxGreetings.Gtk.Box+BoxChild
		this.btnOpenConfig = new global::Gtk.Button ();
		this.btnOpenConfig.WidthRequest = 300;
		this.btnOpenConfig.HeightRequest = 200;
		this.btnOpenConfig.Sensitive = false;
		this.btnOpenConfig.CanFocus = true;
		this.btnOpenConfig.Name = "btnOpenConfig";
		this.btnOpenConfig.UseUnderline = true;
		this.btnOpenConfig.Label = global::Mono.Unix.Catalog.GetString ("Load configuration");
		global::Gtk.Image w5 = new global::Gtk.Image ();
		w5.Pixbuf = global::Stetic.IconLoader.LoadIcon (this, "gtk-open", global::Gtk.IconSize.Menu);
		this.btnOpenConfig.Image = w5;
		this.hboxGreetings.Add (this.btnOpenConfig);
		global::Gtk.Box.BoxChild w6 = ((global::Gtk.Box.BoxChild)(this.hboxGreetings [this.btnOpenConfig]));
		w6.Position = 1;
		w6.Expand = false;
		w6.Fill = false;
		this.vboxMain.Add (this.hboxGreetings);
		global::Gtk.Box.BoxChild w7 = ((global::Gtk.Box.BoxChild)(this.vboxMain [this.hboxGreetings]));
		w7.Position = 1;
		w7.Fill = false;
		// Container child vboxMain.Gtk.Box+BoxChild
		this.tableConnection = new global::Gtk.Table (((uint)(3)), ((uint)(5)), false);
		this.tableConnection.Name = "tableConnection";
		this.tableConnection.RowSpacing = ((uint)(6));
		this.tableConnection.ColumnSpacing = ((uint)(6));
		// Container child tableConnection.Gtk.Table+TableChild
		this.btnConnect = new global::Gtk.Button ();
		this.btnConnect.WidthRequest = 120;
		this.btnConnect.CanFocus = true;
		this.btnConnect.Name = "btnConnect";
		this.btnConnect.UseUnderline = true;
		this.btnConnect.Label = global::Mono.Unix.Catalog.GetString ("Connect");
		global::Gtk.Image w8 = new global::Gtk.Image ();
		w8.Pixbuf = global::Stetic.IconLoader.LoadIcon (this, "gtk-connect", global::Gtk.IconSize.Menu);
		this.btnConnect.Image = w8;
		this.tableConnection.Add (this.btnConnect);
		global::Gtk.Table.TableChild w9 = ((global::Gtk.Table.TableChild)(this.tableConnection [this.btnConnect]));
		w9.LeftAttach = ((uint)(3));
		w9.RightAttach = ((uint)(4));
		w9.XOptions = ((global::Gtk.AttachOptions)(4));
		w9.YOptions = ((global::Gtk.AttachOptions)(4));
		// Container child tableConnection.Gtk.Table+TableChild
		this.btnRefresh = new global::Gtk.Button ();
		this.btnRefresh.CanFocus = true;
		this.btnRefresh.Name = "btnRefresh";
		this.btnRefresh.UseUnderline = true;
		this.btnRefresh.Label = global::Mono.Unix.Catalog.GetString ("Refresh");
		global::Gtk.Image w10 = new global::Gtk.Image ();
		w10.Pixbuf = global::Stetic.IconLoader.LoadIcon (this, "gtk-refresh", global::Gtk.IconSize.Menu);
		this.btnRefresh.Image = w10;
		this.tableConnection.Add (this.btnRefresh);
		global::Gtk.Table.TableChild w11 = ((global::Gtk.Table.TableChild)(this.tableConnection [this.btnRefresh]));
		w11.TopAttach = ((uint)(1));
		w11.BottomAttach = ((uint)(2));
		w11.LeftAttach = ((uint)(3));
		w11.RightAttach = ((uint)(4));
		w11.XOptions = ((global::Gtk.AttachOptions)(4));
		w11.YOptions = ((global::Gtk.AttachOptions)(4));
		// Container child tableConnection.Gtk.Table+TableChild
		this.cbConnectPorts = global::Gtk.ComboBox.NewText ();
		this.cbConnectPorts.WidthRequest = 200;
		this.cbConnectPorts.Name = "cbConnectPorts";
		this.tableConnection.Add (this.cbConnectPorts);
		global::Gtk.Table.TableChild w12 = ((global::Gtk.Table.TableChild)(this.tableConnection [this.cbConnectPorts]));
		w12.LeftAttach = ((uint)(2));
		w12.RightAttach = ((uint)(3));
		w12.XOptions = ((global::Gtk.AttachOptions)(4));
		w12.YOptions = ((global::Gtk.AttachOptions)(4));
		// Container child tableConnection.Gtk.Table+TableChild
		this.label2 = new global::Gtk.Label ();
		this.label2.Name = "label2";
		this.tableConnection.Add (this.label2);
		global::Gtk.Table.TableChild w13 = ((global::Gtk.Table.TableChild)(this.tableConnection [this.label2]));
		w13.LeftAttach = ((uint)(4));
		w13.RightAttach = ((uint)(5));
		w13.YOptions = ((global::Gtk.AttachOptions)(4));
		// Container child tableConnection.Gtk.Table+TableChild
		this.label3 = new global::Gtk.Label ();
		this.label3.Name = "label3";
		this.tableConnection.Add (this.label3);
		global::Gtk.Table.TableChild w14 = ((global::Gtk.Table.TableChild)(this.tableConnection [this.label3]));
		w14.YOptions = ((global::Gtk.AttachOptions)(4));
		// Container child tableConnection.Gtk.Table+TableChild
		this.lblConnectionPorts = new global::Gtk.Label ();
		this.lblConnectionPorts.Name = "lblConnectionPorts";
		this.lblConnectionPorts.Xalign = 1F;
		this.lblConnectionPorts.LabelProp = global::Mono.Unix.Catalog.GetString ("Available Ports:");
		this.tableConnection.Add (this.lblConnectionPorts);
		global::Gtk.Table.TableChild w15 = ((global::Gtk.Table.TableChild)(this.tableConnection [this.lblConnectionPorts]));
		w15.LeftAttach = ((uint)(1));
		w15.RightAttach = ((uint)(2));
		w15.XOptions = ((global::Gtk.AttachOptions)(4));
		w15.YOptions = ((global::Gtk.AttachOptions)(4));
		this.vboxMain.Add (this.tableConnection);
		global::Gtk.Box.BoxChild w16 = ((global::Gtk.Box.BoxChild)(this.vboxMain [this.tableConnection]));
		w16.Position = 2;
		w16.Fill = false;
		// Container child vboxMain.Gtk.Box+BoxChild
		this.tableConfig = new global::Gtk.Table (((uint)(3)), ((uint)(2)), false);
		this.tableConfig.Name = "tableConfig";
		this.tableConfig.RowSpacing = ((uint)(6));
		this.tableConfig.ColumnSpacing = ((uint)(6));
		// Container child tableConfig.Gtk.Table+TableChild
		this.btnConfigBack = new global::Gtk.Button ();
		this.btnConfigBack.CanFocus = true;
		this.btnConfigBack.Name = "btnConfigBack";
		this.btnConfigBack.UseUnderline = true;
		this.btnConfigBack.Label = global::Mono.Unix.Catalog.GetString ("Back");
		this.tableConfig.Add (this.btnConfigBack);
		global::Gtk.Table.TableChild w17 = ((global::Gtk.Table.TableChild)(this.tableConfig [this.btnConfigBack]));
		w17.TopAttach = ((uint)(2));
		w17.BottomAttach = ((uint)(3));
		w17.XOptions = ((global::Gtk.AttachOptions)(4));
		w17.YOptions = ((global::Gtk.AttachOptions)(4));
		// Container child tableConfig.Gtk.Table+TableChild
		this.btnConfigRun = new global::Gtk.Button ();
		this.btnConfigRun.CanFocus = true;
		this.btnConfigRun.Name = "btnConfigRun";
		this.btnConfigRun.UseUnderline = true;
		this.btnConfigRun.Label = global::Mono.Unix.Catalog.GetString ("Run");
		this.tableConfig.Add (this.btnConfigRun);
		global::Gtk.Table.TableChild w18 = ((global::Gtk.Table.TableChild)(this.tableConfig [this.btnConfigRun]));
		w18.TopAttach = ((uint)(2));
		w18.BottomAttach = ((uint)(3));
		w18.LeftAttach = ((uint)(1));
		w18.RightAttach = ((uint)(2));
		w18.XOptions = ((global::Gtk.AttachOptions)(4));
		w18.YOptions = ((global::Gtk.AttachOptions)(4));
		// Container child tableConfig.Gtk.Table+TableChild
		this.ScrolledWindowConfigVBox = new global::Gtk.ScrolledWindow ();
		this.ScrolledWindowConfigVBox.Name = "ScrolledWindowConfigVBox";
		this.ScrolledWindowConfigVBox.ShadowType = ((global::Gtk.ShadowType)(1));
		// Container child ScrolledWindowConfigVBox.Gtk.Container+ContainerChild
		global::Gtk.Viewport w19 = new global::Gtk.Viewport ();
		w19.ShadowType = ((global::Gtk.ShadowType)(0));
		// Container child GtkViewport.Gtk.Container+ContainerChild
		this.vboxConfig = new global::Gtk.VBox ();
		this.vboxConfig.Name = "vboxConfig";
		this.vboxConfig.Spacing = 6;
		w19.Add (this.vboxConfig);
		this.ScrolledWindowConfigVBox.Add (w19);
		this.tableConfig.Add (this.ScrolledWindowConfigVBox);
		global::Gtk.Table.TableChild w22 = ((global::Gtk.Table.TableChild)(this.tableConfig [this.ScrolledWindowConfigVBox]));
		w22.BottomAttach = ((uint)(2));
		w22.RightAttach = ((uint)(2));
		this.vboxMain.Add (this.tableConfig);
		global::Gtk.Box.BoxChild w23 = ((global::Gtk.Box.BoxChild)(this.vboxMain [this.tableConfig]));
		w23.Position = 3;
		// Container child vboxMain.Gtk.Box+BoxChild
		this.vboxPlot = new global::Gtk.VBox ();
		this.vboxPlot.Name = "vboxPlot";
		this.vboxPlot.Spacing = 6;
		// Container child vboxPlot.Gtk.Box+BoxChild
		this.btnPlotBack = new global::Gtk.Button ();
		this.btnPlotBack.CanFocus = true;
		this.btnPlotBack.Name = "btnPlotBack";
		this.btnPlotBack.UseUnderline = true;
		this.btnPlotBack.Label = global::Mono.Unix.Catalog.GetString ("Back");
		this.vboxPlot.Add (this.btnPlotBack);
		global::Gtk.Box.BoxChild w24 = ((global::Gtk.Box.BoxChild)(this.vboxPlot [this.btnPlotBack]));
		w24.Position = 1;
		w24.Expand = false;
		w24.Fill = false;
		this.vboxMain.Add (this.vboxPlot);
		global::Gtk.Box.BoxChild w25 = ((global::Gtk.Box.BoxChild)(this.vboxMain [this.vboxPlot]));
		w25.Position = 4;
		// Container child vboxMain.Gtk.Box+BoxChild
		this.statusbar1 = new global::Gtk.Statusbar ();
		this.statusbar1.Name = "statusbar1";
		this.statusbar1.Spacing = 6;
		this.vboxMain.Add (this.statusbar1);
		global::Gtk.Box.BoxChild w26 = ((global::Gtk.Box.BoxChild)(this.vboxMain [this.statusbar1]));
		w26.Position = 5;
		w26.Expand = false;
		w26.Fill = false;
		this.Add (this.vboxMain);
		if ((this.Child != null)) {
			this.Child.ShowAll ();
		}
		this.DefaultWidth = 1147;
		this.DefaultHeight = 735;
		this.tableConnection.Hide ();
		this.vboxConfig.Hide ();
		this.ScrolledWindowConfigVBox.Hide ();
		this.tableConfig.Hide ();
		this.vboxPlot.Hide ();
		this.Show ();
		this.DeleteEvent += new global::Gtk.DeleteEventHandler (this.OnDeleteEvent);
		this.aboutAction.Activated += new global::System.EventHandler (this.OnAboutActionActivated);
		this.quitAction.Activated += new global::System.EventHandler (this.OnQuitActionActivated);
		this.disconnectAction.Activated += new global::System.EventHandler (this.OnDisconnectActionActivated);
		this.btnNewConfig.Clicked += new global::System.EventHandler (this.OnBtnNewConfigClicked);
		this.btnRefresh.Clicked += new global::System.EventHandler (this.OnBtnRefreshClicked);
		this.btnConnect.Clicked += new global::System.EventHandler (this.OnBtnConnectClicked);
		this.btnConfigRun.Clicked += new global::System.EventHandler (this.OnBtnConfigRunClicked);
		this.btnConfigBack.Clicked += new global::System.EventHandler (this.OnBtnConfigBackClicked);
	}
}
