using System;
using Gtk;

namespace Mokup
{
	[System.ComponentModel.ToolboxItem (true)]
	public partial class CollapseFrame : Gtk.Bin
	{
		private bool collapsed = false;

		public CollapseFrame ()
		{
			this.Build ();
			this.SizeArrow.WidgetEvent += delegate (object obj, WidgetEventArgs args) {
				Console.WriteLine (args.Event.Type);
			};
		}

		private void Collapse (object obj, EventArgs args)
		{
			if (collapsed) {
				this.SizeArrow.ArrowType = ArrowType.Right;
			} else {
				this.SizeArrow.ArrowType = ArrowType.Down;
			} 
			collapsed = !collapsed;
		}
	}
}

