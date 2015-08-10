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
		public string Name;
		[Gtk.TreeNodeValue (Column = 1)]
		public Pixbuf Color;
		[Gtk.TreeNodeValue (Column = 2)]
		public string Sequence = "";

		public string RealName { get; private set; }

		public DPinTreeNode (DPin pin, int index = -1)
		{
			Name = pin.Name + "( D" + pin.Number + " )";
			Color = ColorHelper.ColoredPixbuf (pin.PlotColor);
			Sequence = "";
			Index = index;

			RealName = pin.Name;
		}
	}

	public class APinTreeNode : Gtk.TreeNode
	{
		[Gtk.TreeNodeValue (Column = 0)]
		public string Name;
		[Gtk.TreeNodeValue (Column = 1)]
		public Gdk.Pixbuf Color;
		[Gtk.TreeNodeValue (Column = 2)]
		public string Signal = "";
		[Gtk.TreeNodeValue (Column = 3)]
		public double Slope = 1;
		[Gtk.TreeNodeValue (Column = 4)]
		public double Offset = 0;
		[Gtk.TreeNodeValue (Column = 5)]
		public string Unit = "";
		[Gtk.TreeNodeValue (Column = 6)]
		public double Frequency = 1;
		[Gtk.TreeNodeValue (Column = 7)]
		public double Interval = 1;

		public int Index { get; private set; }

		public string RealName { get; private set; }

		public APinTreeNode (APin pin, int index = -1)
		{
			Name = pin.Name + "( A" + pin.Number + " )";
			Color = ColorHelper.ColoredPixbuf (pin.PlotColor);

			Signal = "";
			Slope = pin.Slope;
			Offset = pin.Offset;
			Frequency = pin.Frequency;
			Interval = pin.Interval;
			Unit = pin.Unit;

			RealName = pin.Name;

			Index = index;
		}
	}

	public class SignalTreeNode : Gtk.TreeNode
	{
		[Gtk.TreeNodeValue (Column = 0)]
		public string Name { get { return AnalogSignal.SignalName; } private set { } }

		[Gtk.TreeNodeValue (Column = 1)]
		public Pixbuf Color;

		[Gtk.TreeNodeValue (Column = 2)]
		public string Pins {
			get {
				var s = "";
				for (int i = 0; i < AnalogSignal.Pins.Count; i++)
				{
					s += AnalogSignal.Pins [i].Name + "(A" + AnalogSignal.Pins [i].Number + ")";
					if (i < AnalogSignal.Pins.Count - 1)
					{
						s += ", ";
					}
				}
				return s;
			}
			private set{ }
		}

		[Gtk.TreeNodeValue (Column = 3)]
		public double Frequency { get { return AnalogSignal.Frequency; } private set { } }

		[Gtk.TreeNodeValue (Column = 4)]
		public string Operation { get { return AnalogSignal.SignalOperationString; } private set { } }

		public int Index{ get; private set; }

		public Signal AnalogSignal{ get; private set; }

		public SignalTreeNode (Signal analogSignal, int index = -1)
		{
			Index = index;
			AnalogSignal = analogSignal;
			Color = ColorHelper.ColoredPixbuf (AnalogSignal.SignalColor);
		}
	}

	public class APinSignalDialogTreeNode : Gtk.TreeNode
	{
		[Gtk.TreeNodeValue (Column = 0)]
		public string Name { get { return Pin.Name + "(A" + Pin.Number + ")"; } private set { } }

		[Gtk.TreeNodeValue (Column = 1)]
		public double Frequency { get { return Pin.Frequency; } private set { } }

		[Gtk.TreeNodeValue (Column = 2)]
		public int Interval { get { return Pin.Interval; } private set { } }

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
		public string Label { get { return Pin.Name + "(" + Pin.Number + ")"; } private set { } }

		[Gtk.TreeNodeValue (Column = 1)]
		public string Frequency { get { return Pin.EffectiveFrequency.ToString (); } private set { } }

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

	public class SequenceTreeNode : Gtk.TreeNode
	{
		public int Index { get; private set; }

		[Gtk.TreeNodeValue (Column = 0)]
		public string Name { get { return Seq.Name; } set { Seq.Name = value; } }

		[Gtk.TreeNodeValue (Column = 1)]
		public string Pins { get { return String.Format ("{0}({1})", Seq.Pin.Name, Seq.Pin.Number); } private set { } }

		[Gtk.TreeNodeValue (Column = 2)]
		public string Runtime { get { return Seq.Runtime.ToString ("g"); } private set { } }

		public Sequence Seq;

		public SequenceTreeNode (Sequence seq, int index = -1)
		{
			Index = index;
			Seq = seq;
		}
	}
}
