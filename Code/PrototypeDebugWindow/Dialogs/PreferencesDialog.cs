using System;
using PrototypeBackend;
using Gtk;

namespace Frontend
{
	public partial class PreferencesDialog : Gtk.Dialog
	{
		private Controller Con;

		public PreferencesDialog (Gtk.Window parent = null, Controller con = null) : base ("", parent, Gtk.DialogFlags.Modal)
		{
			this.Con = con;

			this.Build ();
			InitComponents ();
			BindEvents ();
		}

		private void InitComponents ()
		{
			cbDebuggingMode.Active = Frontend.Settings.Default.DebugMode;
			cbMaximizedStart.Active = Frontend.Settings.Default.StartMaximized;
			cbAutoConnect.Active = Frontend.Settings.Default.AutoConnect;
			cbLoadLastConfig.Active = Frontend.Settings.Default.LoadLastFile;
			cbtnenablelogging.Active = Con.LogToFile;
			entryLogFilePath.Text = Con.LogFilePath;

			ListStore store = new ListStore (typeof(string));
			int index = 0;
			foreach (Logger.LogLevel lvl in Enum.GetValues(typeof(Logger.LogLevel))) {
				store.AppendValues (new object[]{ lvl.ToString () });
				if (lvl == Con.LoggerLevel) {
					break;
				}
				index++;
			}

			cboxLogLevel.Model = store;
			cboxLogLevel.Active = index;
		}

		private void BindEvents ()
		{
			cbLoadLastConfig.Toggled += OnCbLoadLastConfigToggled;
		}

		protected void OnCbDebuggingModeToggled (object sender, EventArgs e)
		{
			Frontend.Settings.Default.DebugMode = cbDebuggingMode.Active;
			Frontend.Settings.Default.Save ();
		}

		protected void OnCbMaximizedStartToggled (object sender, EventArgs e)
		{
			Frontend.Settings.Default.StartMaximized = cbMaximizedStart.Active;
			Frontend.Settings.Default.Save ();
		}

		protected void OnCbAutoConnectToggled (object sender, EventArgs e)
		{
			Frontend.Settings.Default.AutoConnect = cbAutoConnect.Active;
			Frontend.Settings.Default.Save ();
		}

		protected void OnCbtnenableloggingToggled (object sender, EventArgs e)
		{
			Con.LogToFile = cbtnenablelogging.Active;
		}

		protected void OnCboxLogLevelChanged (object sender, EventArgs e)
		{
			Logger.LogLevel lvl = (Logger.LogLevel)Enum.Parse (typeof(Logger.LogLevel), cboxLogLevel.ActiveText);
			Con.LoggerLevel = lvl;
		}

		protected void OnBtnLogFilePathClicked (object sender, EventArgs e)
		{
			var dialog = new FileChooserDialog ("Choose a Log-File location.", this, FileChooserAction.SelectFolder, "Select", ResponseType.Accept);
			dialog.Response += (o, args) => {
				if (args.ResponseId == ResponseType.Accept) {
					entryLogFilePath.Text = dialog.CurrentFolder;
					Con.LogFilePath = dialog.CurrentFolder;
				}
			};
			dialog.Run ();
			dialog.Destroy ();
		}

		private void OnCbLoadLastConfigToggled (object sender, EventArgs e)
		{
			Frontend.Settings.Default.LoadLastFile = cbLoadLastConfig.Active;
			Frontend.Settings.Default.Save ();
		}
	}
}

