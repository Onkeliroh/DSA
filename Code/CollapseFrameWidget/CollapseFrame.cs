using System;

namespace CollapseFrameWidget
{
	[System.ComponentModel.ToolboxItem (true)]
	public partial class CollapseFrame : Gtk.Bin
	{

		public bool IsCollapsed {
			get;
			private set;
		}


		public CollapseFrame ()
		{
			this.Build ();
		}

		private void OnCollapse (object obj, EventArgs args)
		{
			switch (IsCollapsed) {
			case true:
				this.CollapseFrameArrow.ArrowType = Gtk.ArrowType.Right;
				this.CollapseFrameLabel.Visible = true;
				this.CollapseFrameAlignment.Visible = true;
				break;
			case false:
				this.CollapseFrameArrow.ArrowType = Gtk.ArrowType.Down;
				this.CollapseFrameLabel.Visible = false;
				this.CollapseFrameAlignment.Visible = false;
				break;
			}
			ResizeChildren ();
			Show ();
			IsCollapsed = !IsCollapsed;
		}


	}
}

