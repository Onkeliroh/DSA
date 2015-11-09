using System;
using PrototypeBackend;
using Gdk;
using Gtk;

namespace GUIHelper
{
	public class DPinTreeNode : Gtk.TreeNode
	{
		public int Index = -1;

		[Gtk.TreeNodeValue (Column = 0)]
		public string Name { get { return Pin.Name; } private set { } }

		[Gtk.TreeNodeValue (Column = 1)]
		public string Number { get { return Pin.DisplayNumber; } private set { } }

		[Gtk.TreeNodeValue (Column = 2)]
		public Pixbuf Color{ get { return ColorHelper.ColoredPixbuf (Pin.PlotColor); } private set { } }

		[Gtk.TreeNodeValue (Column = 3)]
		public string SequenceName { get { return (Sequence != null) ? Sequence.Name : ""; } private set { } }

		public Sequence @Sequence { get; private set; }

		public DPin Pin { get; private set; }

		public DPinTreeNode (DPin pin, int index = -1, Sequence seq = null)
		{
			Sequence = seq;
			Index = index;
			Pin = pin;
		}
	}

	public class APinTreeNode : Gtk.TreeNode
	{
		[Gtk.TreeNodeValue (Column = 0)]
		public string Name { get { return Pin.Name; } private set { } }

		[Gtk.TreeNodeValue (Column = 1)]
		public string Number { get { return Pin.DisplayNumber; } private set { } }

		[Gtk.TreeNodeValue (Column = 2)]
		public Gdk.Pixbuf Color{ get { return ColorHelper.ColoredPixbuf (Pin.PlotColor); } private set { } }

		[Gtk.TreeNodeValue (Column = 3)]
		public string Slope { get { return FormatHelper.ConvertToString (Pin.Slope); } private set { } }

		[Gtk.TreeNodeValue (Column = 4)]
		public string Offset { get { return FormatHelper.ConvertToString (Pin.Offset); } private set { } }

		[Gtk.TreeNodeValue (Column = 5)]
		public string Unit { get { return Pin.Unit; } private set { } }

		[Gtk.TreeNodeValue (Column = 6)]
		public string Interval { get { return TimeSpan.FromMilliseconds (Pin.Interval).ToString ("g"); } private set { } }

		[Gtk.TreeNodeValue (Column = 7)]
		public int MeanValues { get { return Pin.MeanValuesCount; } private set { } }

		[Gtk.TreeNodeValue (Column = 8)]
		public string CombinationName { get { return (Combination != null) ? Combination.Name : ""; } private set { } }

		public int Index { get; private set; }

		public MeasurementCombination Combination { get; private set; }

		public APin Pin { get; private set; }

		public APinTreeNode (APin pin, int index = -1, MeasurementCombination combination = null)
		{
			Pin = pin;
			Combination = combination;
			Index = index;
		}
	}

	public class MeasurementCombinationTreeNode : Gtk.TreeNode
	{
		[Gtk.TreeNodeValue (Column = 0)]
		public string Name { get { return AnalogSignal.Name; } private set { } }

		[Gtk.TreeNodeValue (Column = 1)]
		public Pixbuf Color;

		[Gtk.TreeNodeValue (Column = 2)]
		public string Pins {
			get {
				var s = "";
				for (int i = 0; i < AnalogSignal.Pins.Count; i++)
				{
					s += AnalogSignal.Pins [i].Name;
					if (i != AnalogSignal.Pins.Count - 1)
					{
						s += "\n";	
					}
				}
				return s;
			}
			private set{ }
		}

		[Gtk.TreeNodeValue (Column = 3)]
		public string PinNumbers {
			get {
				var s = "";
				for (int i = 0; i < AnalogSignal.Pins.Count; i++)
				{
					s += AnalogSignal.Pins [i].DisplayNumber;
					if (i != AnalogSignal.Pins.Count - 1)
					{
						s += "\n";	
					}
				}
				return s;
			}
			private set{ }
		}

		[Gtk.TreeNodeValue (Column = 4)]
		public string Unit { get { return AnalogSignal.Unit; } private set { } }

		[Gtk.TreeNodeValue (Column = 5)]
		public string Interval { get { return TimeSpan.FromMilliseconds (AnalogSignal.Interval).ToString ("g"); } private set { } }

		[Gtk.TreeNodeValue (Column = 6)]
		public string MeanValues { get { return AnalogSignal.MeanValuesCount.ToString (); } private set { } }

		[Gtk.TreeNodeValue (Column = 7)]
		public string Operation { get { return AnalogSignal.OperationString; } private set { } }

		public int Index{ get; private set; }

		public MeasurementCombination AnalogSignal{ get; private set; }

		public MeasurementCombinationTreeNode (MeasurementCombination analogSignal, int index = -1)
		{
			Index = index;
			AnalogSignal = analogSignal;
			Color = ColorHelper.ColoredPixbuf (AnalogSignal.Color);
		}
	}

	public class APinSignalDialogTreeNode : Gtk.TreeNode
	{
		[Gtk.TreeNodeValue (Column = 0)]
		public string Name { get { return Pin.DisplayName; } private set { } }

		[Gtk.TreeNodeValue (Column = 1)]
		public string Frequency { 
			get { 
				return string.Format (
					"{0:D2}:{1:D2}:{2:D2}:{3:D2}", 
					TimeSpan.FromMilliseconds (Pin.Interval).Hours, 
					TimeSpan.FromMilliseconds (Pin.Interval).Minutes, 
					TimeSpan.FromMilliseconds (Pin.Interval).Seconds, 
					TimeSpan.FromMilliseconds (Pin.Interval).Milliseconds); 
			} 
			private set { }
		}

		[Gtk.TreeNodeValue (Column = 2)]
		public int MeanValuesCount { get { return (int)Pin.MeanValuesCount; } private set { } }

		public APin Pin{ get; private set; }

		public int Index { get; private set; }

		public APinSignalDialogTreeNode (APin pin, int index = -1)
		{
			Pin = pin;
			Index = index;
		}
	}

	public class APinListStoreNode : Gtk.TreeNode
	{
		[Gtk.TreeNodeValue (Column = 0)]
		public string Label { get { return Pin.DisplayName; } private set { } }

		[Gtk.TreeNodeValue (Column = 1)]
		public string Frequency { get { return TimeSpan.FromMilliseconds (Pin.EffectiveInterval).ToString ("g"); } private set { } }

		public APin Pin{ get; private set; }

		public int Index{ get; private set; }

		public APinListStoreNode (APin pin, int index = -1)
		{
			Pin = pin;
			Index = index;
		}
	}

	public class SequenceOperationTreeNode : Gtk.TreeNode
	{
		[Gtk.TreeNodeValue (Column = 0)]
		public string TotalTime{ get; private set; }

		[Gtk.TreeNodeValue (Column = 1)]
		public string Duration{ get { return SeqOp.Duration.ToString ("c"); } private set { } }

		[Gtk.TreeNodeValue (Column = 2)]
		public string State{ get { return SeqOp.State.ToString (); } private set { } }

		public SequenceOperation SeqOp{ get; private set; }

		public int Index{ get; private set; }

		public SequenceOperationTreeNode (SequenceOperation seqop, int index = -1, double totalTime = -1)
		{
			SeqOp = seqop;
			Index = index;

			if (totalTime < 0)
			{
				TotalTime = "";
			} else
			{
				TotalTime = "+" + TimeSpan.FromTicks ((long)totalTime).ToString ("g");
			}
		}
	}

	[Gtk.TreeNode (ListOnly = true)]
	public class SequenceTreeNode : Gtk.TreeNode
	{
		public int Index { get; private set; }

		[Gtk.TreeNodeValue (Column = 0)]
		public string GroupName { get { return Seq.GroupName; } private set { } }

		[Gtk.TreeNodeValue (Column = 1)]
		public string Name { get { return Seq.Name; } set { Seq.Name = value; } }

		[Gtk.TreeNodeValue (Column = 2)]
		public Pixbuf Color { get { return ColorHelper.ColoredPixbuf (Seq.Pin.PlotColor); } private set { } }

		[Gtk.TreeNodeValue (Column = 3)]
		public string Pins { get { return Seq.Pin.Name; } private set { } }

		[Gtk.TreeNodeValue (Column = 4)]
		public string Number { get { return Seq.Pin.DisplayNumber; } private set { } }

		[Gtk.TreeNodeValue (Column = 5)]
		public string Runtime { get { return Seq.Runtime.ToString ("g"); } private set { } }

		[Gtk.TreeNodeValue (Column = 6)]
		public string Repetitions { get { return (Seq.Repetitions != -1) ? Seq.Repetitions + " Cycle(s)" : "\u221E Cycles"; } private set { } }

		public Sequence Seq;

		public SequenceTreeNode (Sequence seq, int index = -1)
		{
			Index = index;
			Seq = seq;
		}
	}
}
