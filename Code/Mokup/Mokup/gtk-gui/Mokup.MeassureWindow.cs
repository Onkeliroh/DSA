
// This file has been generated by the GUI designer. Do not modify.
namespace Mokup
{
	public partial class MeassureWindow
	{
		private global::Gtk.UIManager UIManager;
		
		private global::Gtk.Action FileAction;
		
		private global::Gtk.Action quitAction;
		
		private global::Gtk.Action HelpAction;
		
		private global::Gtk.Action aboutAction;
		
		private global::Gtk.Action goBackAction;
		
		private global::Gtk.ToggleAction mediaPauseAction;
		
		private global::Gtk.Action goForwardAction;
		
		private global::Gtk.Action zoomInAction;
		
		private global::Gtk.Action zoomOutAction;
		
		private global::Gtk.Action zoom100Action;
		
		private global::Gtk.VBox MainVBox;
		
		private global::Gtk.MenuBar menubar1;
		
		private global::Gtk.Toolbar toolbar2;
		
		private global::Gtk.HBox ContentHBox;
		
		private global::Gtk.VBox PlotVBox;
		
		private global::Gtk.HBox PlotHBox;
		
		private global::Gtk.Notebook notebook1;
		
		private global::Gtk.Label label2;
		
		private global::Gtk.Fixed fixed1;
		
		private global::Gtk.Toolbar toolbar3;
		
		private global::Gtk.Statusbar statusbar1;

		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget Mokup.MeassureWindow
			this.UIManager = new global::Gtk.UIManager ();
			global::Gtk.ActionGroup w1 = new global::Gtk.ActionGroup ("Default");
			this.FileAction = new global::Gtk.Action ("FileAction", global::Mono.Unix.Catalog.GetString ("File"), null, null);
			this.FileAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("File");
			w1.Add (this.FileAction, null);
			this.quitAction = new global::Gtk.Action ("quitAction", global::Mono.Unix.Catalog.GetString ("Quit"), null, "gtk-quit");
			this.quitAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("Quit");
			w1.Add (this.quitAction, null);
			this.HelpAction = new global::Gtk.Action ("HelpAction", global::Mono.Unix.Catalog.GetString ("Help"), null, null);
			this.HelpAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("Help");
			w1.Add (this.HelpAction, null);
			this.aboutAction = new global::Gtk.Action ("aboutAction", global::Mono.Unix.Catalog.GetString ("About"), null, "gtk-about");
			this.aboutAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("About");
			w1.Add (this.aboutAction, null);
			this.goBackAction = new global::Gtk.Action ("goBackAction", null, null, "gtk-go-back");
			w1.Add (this.goBackAction, null);
			this.mediaPauseAction = new global::Gtk.ToggleAction ("mediaPauseAction", null, null, "gtk-media-pause");
			w1.Add (this.mediaPauseAction, null);
			this.goForwardAction = new global::Gtk.Action ("goForwardAction", null, null, "gtk-go-forward");
			w1.Add (this.goForwardAction, null);
			this.zoomInAction = new global::Gtk.Action ("zoomInAction", null, null, "gtk-zoom-in");
			w1.Add (this.zoomInAction, null);
			this.zoomOutAction = new global::Gtk.Action ("zoomOutAction", null, null, "gtk-zoom-out");
			w1.Add (this.zoomOutAction, null);
			this.zoom100Action = new global::Gtk.Action ("zoom100Action", null, null, "gtk-zoom-100");
			w1.Add (this.zoom100Action, null);
			this.UIManager.InsertActionGroup (w1, 0);
			this.AddAccelGroup (this.UIManager.AccelGroup);
			this.WidthRequest = 800;
			this.HeightRequest = 600;
			this.Name = "Mokup.MeassureWindow";
			this.Title = global::Mono.Unix.Catalog.GetString ("MeassureWindow");
			this.WindowPosition = ((global::Gtk.WindowPosition)(4));
			// Container child Mokup.MeassureWindow.Gtk.Container+ContainerChild
			this.MainVBox = new global::Gtk.VBox ();
			this.MainVBox.Name = "MainVBox";
			// Container child MainVBox.Gtk.Box+BoxChild
			this.UIManager.AddUiFromString ("<ui><menubar name='menubar1'><menu name='FileAction' action='FileAction'><menuitem name='quitAction' action='quitAction'/></menu><menu name='HelpAction' action='HelpAction'><menuitem name='aboutAction' action='aboutAction'/></menu></menubar></ui>");
			this.menubar1 = ((global::Gtk.MenuBar)(this.UIManager.GetWidget ("/menubar1")));
			this.menubar1.Name = "menubar1";
			this.MainVBox.Add (this.menubar1);
			global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(this.MainVBox [this.menubar1]));
			w2.Position = 0;
			w2.Expand = false;
			w2.Fill = false;
			// Container child MainVBox.Gtk.Box+BoxChild
			this.UIManager.AddUiFromString ("<ui><toolbar name='toolbar2'/></ui>");
			this.toolbar2 = ((global::Gtk.Toolbar)(this.UIManager.GetWidget ("/toolbar2")));
			this.toolbar2.Name = "toolbar2";
			this.toolbar2.ShowArrow = false;
			this.MainVBox.Add (this.toolbar2);
			global::Gtk.Box.BoxChild w3 = ((global::Gtk.Box.BoxChild)(this.MainVBox [this.toolbar2]));
			w3.Position = 1;
			w3.Expand = false;
			w3.Fill = false;
			// Container child MainVBox.Gtk.Box+BoxChild
			this.ContentHBox = new global::Gtk.HBox ();
			this.ContentHBox.Name = "ContentHBox";
			this.ContentHBox.Spacing = 1;
			// Container child ContentHBox.Gtk.Box+BoxChild
			this.PlotVBox = new global::Gtk.VBox ();
			this.PlotVBox.Name = "PlotVBox";
			this.PlotVBox.Spacing = 1;
			// Container child PlotVBox.Gtk.Box+BoxChild
			this.PlotHBox = new global::Gtk.HBox ();
			this.PlotHBox.Name = "PlotHBox";
			this.PlotHBox.Spacing = 1;
			// Container child PlotHBox.Gtk.Box+BoxChild
			this.notebook1 = new global::Gtk.Notebook ();
			this.notebook1.CanFocus = true;
			this.notebook1.Name = "notebook1";
			this.notebook1.CurrentPage = 0;
			// Notebook tab
			global::Gtk.Label w4 = new global::Gtk.Label ();
			w4.Visible = true;
			this.notebook1.Add (w4);
			this.label2 = new global::Gtk.Label ();
			this.label2.Name = "label2";
			this.label2.LabelProp = global::Mono.Unix.Catalog.GetString ("page1");
			this.notebook1.SetTabLabel (w4, this.label2);
			this.label2.ShowAll ();
			this.PlotHBox.Add (this.notebook1);
			global::Gtk.Box.BoxChild w5 = ((global::Gtk.Box.BoxChild)(this.PlotHBox [this.notebook1]));
			w5.Position = 0;
			// Container child PlotHBox.Gtk.Box+BoxChild
			this.fixed1 = new global::Gtk.Fixed ();
			this.fixed1.Name = "fixed1";
			this.fixed1.HasWindow = false;
			this.PlotHBox.Add (this.fixed1);
			global::Gtk.Box.BoxChild w6 = ((global::Gtk.Box.BoxChild)(this.PlotHBox [this.fixed1]));
			w6.Position = 1;
			this.PlotVBox.Add (this.PlotHBox);
			global::Gtk.Box.BoxChild w7 = ((global::Gtk.Box.BoxChild)(this.PlotVBox [this.PlotHBox]));
			w7.Position = 0;
			// Container child PlotVBox.Gtk.Box+BoxChild
			this.UIManager.AddUiFromString ("<ui><toolbar name='toolbar3'><toolitem name='goBackAction' action='goBackAction'/><toolitem name='mediaPauseAction' action='mediaPauseAction'/><toolitem name='goForwardAction' action='goForwardAction'/><separator/><toolitem name='zoomInAction' action='zoomInAction'/><toolitem name='zoomOutAction' action='zoomOutAction'/><toolitem name='zoom100Action' action='zoom100Action'/></toolbar></ui>");
			this.toolbar3 = ((global::Gtk.Toolbar)(this.UIManager.GetWidget ("/toolbar3")));
			this.toolbar3.Name = "toolbar3";
			this.toolbar3.ShowArrow = false;
			this.toolbar3.IconSize = ((global::Gtk.IconSize)(2));
			this.PlotVBox.Add (this.toolbar3);
			global::Gtk.Box.BoxChild w8 = ((global::Gtk.Box.BoxChild)(this.PlotVBox [this.toolbar3]));
			w8.Position = 1;
			w8.Expand = false;
			w8.Fill = false;
			this.ContentHBox.Add (this.PlotVBox);
			global::Gtk.Box.BoxChild w9 = ((global::Gtk.Box.BoxChild)(this.ContentHBox [this.PlotVBox]));
			w9.Position = 0;
			this.MainVBox.Add (this.ContentHBox);
			global::Gtk.Box.BoxChild w10 = ((global::Gtk.Box.BoxChild)(this.MainVBox [this.ContentHBox]));
			w10.Position = 2;
			// Container child MainVBox.Gtk.Box+BoxChild
			this.statusbar1 = new global::Gtk.Statusbar ();
			this.statusbar1.Name = "statusbar1";
			this.statusbar1.Spacing = 6;
			this.MainVBox.Add (this.statusbar1);
			global::Gtk.Box.BoxChild w11 = ((global::Gtk.Box.BoxChild)(this.MainVBox [this.statusbar1]));
			w11.Position = 3;
			w11.Expand = false;
			w11.Fill = false;
			this.Add (this.MainVBox);
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.DefaultWidth = 800;
			this.DefaultHeight = 600;
			this.Show ();
		}
	}
}
