using System;
using Gtk;
using IniParser;
using IniParser.Model;

namespace BoardIniEditor
{
	public partial class MainWindow: Gtk.Window
	{

		private IniParser.FileIniDataParser Parser = new IniParser.FileIniDataParser ();
		private IniData Data = new IniData ();

		private string FilePath = "";

		public MainWindow () : base (Gtk.WindowType.Toplevel)
		{
			Build ();
			InitializeComponents ();
		}

		private void InitializeComponents ()
		{
			InitializeNodeView ();
		}

		private void InitializeNodeView ()
		{
			var Store = new NodeStore (typeof(SectionTreeNode));

			nvSections.NodeStore = Store;
			nvSections.AppendColumn ("Section", new CellRendererText (), "text", 0);
			nvSections.AppendColumn ("Key", new CellRendererText (), "text", 1);
			nvSections.AppendColumn ("Value", new CellRendererText (), "text", 2);
		}

		protected void OnDeleteEvent (object sender, DeleteEventArgs a)
		{
			Application.Quit ();
			a.RetVal = true;
		}

		protected void OnOpenActionActivated (object sender, EventArgs e)
		{
			var dialog = new FileChooserDialog ("Choose a Boards INI File", this, FileChooserAction.Open, "Cancel", Gtk.ResponseType.Cancel, "Ok", Gtk.ResponseType.Ok);
			dialog.Response += (o, args) =>
			{
				if (args.ResponseId == ResponseType.Ok)
				{
					FilePath = dialog.Filename;
					Data = this.Parser.ReadFile (FilePath);
					ShowNodeView ();
					ShowFilePreview ();
				}
			};
			dialog.Run ();
			dialog.Destroy ();
		}

		protected void OnSaveActionActivated (object sender, EventArgs e)
		{
			if (FilePath != "")
			{
				Parser.WriteFile (FilePath, Data, System.Text.Encoding.UTF8);
				ShowFilePreview ();
			}
		}

		protected void OnSaveAsActionActivated (object sender, EventArgs e)
		{
			var dialog = new FileChooserDialog ("Selecte Save Location", this, FileChooserAction.Save, "Cancel", ResponseType.Cancel, "Save", ResponseType.Accept);
			dialog.Response += (o, args) =>
			{
				if (args.ResponseId == ResponseType.Accept)
				{
					Parser.WriteFile (dialog.Filename, Data, System.Text.Encoding.UTF8);
					ShowFilePreview ();
				}
			};

			dialog.Run ();
			dialog.Destroy ();
		}

		protected void ShowNodeView ()
		{
			nvSections.NodeStore.Clear ();

			foreach (SectionData sd in Data.Sections)
			{
				nvSections.NodeStore.AddNode (new SectionTreeNode (sd.SectionName));
				foreach (KeyData kd in sd.Keys)
				{
					nvSections.NodeStore.AddNode (new SectionTreeNode (sd.SectionName, kd.KeyName, kd.Value));
				}
			}
			ShowAll ();
		}

		protected void ShowFilePreview ()
		{
			if (FilePath != null)
			{
				tvPreview.Buffer.Clear ();

				var file = new System.IO.StreamReader (FilePath);
				while (!file.EndOfStream)
				{
					tvPreview.Buffer.Text += file.ReadLine () + "\n";	
				}

				file.Close ();
			}
		}

		protected void OnNvSectionsRowActivated (object o, RowActivatedArgs args)
		{
			var node = (o as NodeView).NodeSelection.SelectedNode as SectionTreeNode;

			var section = Data [node.Section];

			if (node != null)
			{
				entryName.Text = section.GetKeyData ("Name").Value;
				sbNumDPins.Value = Convert.ToInt32 (section.GetKeyData ("NumberOfDigitalPins").Value);
				sbNumAPins.Value = Convert.ToInt32 (section.GetKeyData ("NumberOfAnalogPins").Value);
				entryMCU.Text = section.GetKeyData ("MCU").Value;
				btnImagePath.Label = section.GetKeyData ("ImagePath").Value;
			}
		}

		protected void OnBtnSaveClicked (object sender, EventArgs e)
		{
			Data.Sections.Add (new SectionData (entryName.Text));
			Data.Sections [entryName.Text].AddKey ("Name", entryName.Text);
			Data.Sections [entryName.Text].AddKey ("NumberOfDigitalPins", sbNumDPins.ValueAsInt.ToString ());
			Data.Sections [entryName.Text].AddKey ("NumberOfAnalogPins", sbNumAPins.ValueAsInt.ToString ());
			Data.Sections [entryName.Text].AddKey ("MCU", entryMCU.Text);
			Data.Sections [entryName.Text].AddKey ("ImagePath", btnImagePath.Label);

			ShowNodeView ();
			ShowFilePreview ();
		}

		protected void OnBtnDeleteClicked (object sender, EventArgs e)
		{
			var node = nvSections.NodeSelection.SelectedNode as SectionTreeNode;
			if (node != null)
			{
				Data.Sections.RemoveSection (node.Section);
			}

			ShowNodeView ();
			ShowFilePreview ();
		}

		protected void OnBtnImagePathClicked (object sender, EventArgs e)
		{
			var dialog = new FileChooserDialog ("Selecte Save Location", this, FileChooserAction.Open, "Cancel", ResponseType.Cancel, "Apply", ResponseType.Apply);
			dialog.Response += (o, args) =>
			{
				if (args.ResponseId == ResponseType.Apply)
				{
					btnImagePath.Label = dialog.Filename;
				}
			};

			dialog.Run ();
			dialog.Destroy ();
		}
	}

	public class SectionTreeNode:Gtk.TreeNode
	{
		[Gtk.TreeNodeValue (Column = 0)]
		public string Section;

		[Gtk.TreeNodeValue (Column = 1)]
		public string Key;

		[Gtk.TreeNodeValue (Column = 2)]
		public string Value;

		public SectionTreeNode (string Name, string key = "", string value = "")
		{
			Section = Name;
			Key = key;
			Value = value;
		}
	}
}