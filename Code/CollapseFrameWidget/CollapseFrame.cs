using System;
using Gtk;

namespace CollapseFrameWidget
{
	[System.ComponentModel.ToolboxItem (true)]
	public partial class CollapseFrame : Gtk.Bin
	{
		Widget child;

		private bool isCollapsed;

		public bool IsCollapsed {
			get{ return isCollapsed; }
			set {
				isCollapsed = value;
				this.CollapseFrameAlignment.Visible = !value;
				ResizeChildren ();
				Show ();
			}
		}

		private string labelProp;

		public string LabelProp {
			get {
				return labelProp;
			}
			set {
				labelProp = value;
				this.CollapseFrameLabelH.LabelProp = value;
				this.CollapseFrameLabelV.LabelProp = value;
				Show ();
			}

		}

		public CollapseFrame ()
		{
			this.Build ();
			this.Added += new AddedHandler (WidgetAdded);

			this.IsCollapsed = false;
			OnCollapse (null, new EventArgs ());
		}

		private void OnCollapse (object obj, EventArgs args)
		{
			switch (isCollapsed) {
			case true:
				this.CollapseFrameArrow.ArrowType = Gtk.ArrowType.Right;
				this.CollapseFrameLabelH.Visible = true;
				this.CollapseFrameLabelV.Visible = false;
				this.CollapseFrameAlignment.Visible = true;
				break;
			case false:
				this.CollapseFrameArrow.ArrowType = Gtk.ArrowType.Down;
				this.CollapseFrameLabelH.Visible = false;
				this.CollapseFrameLabelV.Visible = true;
				this.CollapseFrameAlignment.Visible = false;
				break;
			}
			ResizeChildren ();
			Show ();
			isCollapsed = !isCollapsed;
		}

		private void WidgetAdded (object o, AddedArgs e)
		{
			child = e.Widget;
		}

		//		private void OnSizeRequested ( object o, SizeRequestedArgs e)
		//		{
		//			if ( child != null )
		//			{
		//				int width = e.Requisition.Width;
		//				int height = e.Requisition.Height;
		//
		//				child.GetSizeRequest (out width, out height);
		//				if ( width == -1 || height == -1 )
		//				{
		//					width = height = 80;
		//				}
		//				SetSizeRequest(width + pad)
		//			}
		//		}
	}
}

