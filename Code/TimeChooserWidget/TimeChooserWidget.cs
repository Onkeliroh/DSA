using System;

namespace TimeChooserWidget
{
	[System.ComponentModel.ToolboxItem (true)]
	public partial class TimeChooserWidget : Gtk.Bin
	{
		public TimeSpan Time {
			get {
				return new TimeSpan (sbDays.ValueAsInt, sbHours.ValueAsInt, sbMinutes.ValueAsInt, sbSeconds.ValueAsInt, sbMilliSec.ValueAsInt);
			}
			set {
				sbDays.Value = value.Days;
				sbHours.Value = value.Hours;
				sbMinutes.Value = value.Minutes;
				sbSeconds.Value = value.Seconds;
				sbMilliSec.Value = value.Milliseconds;
			}
		}

		public TimeChooserWidget ()
		{
			this.Build ();
		}
	}
}

