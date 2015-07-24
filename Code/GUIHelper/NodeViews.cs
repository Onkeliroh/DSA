﻿using System;
using PrototypeBackend;
using Gdk;
using Gtk;

namespace GUIHelper
{

	public class DPinTreeNode : Gtk.TreeNode
	{
		[Gtk.TreeNodeValue (Column = 0)]
		public string Name;
		[Gtk.TreeNodeValue (Column = 1)]
		public Pixbuf Color;
		[Gtk.TreeNodeValue (Column = 2)]
		public string Sequence = "";

		public string RealName { get; private set; }

		public DPinTreeNode (DPin pin)
		{
			Name = pin.Name + "( D" + pin.Number + " )";
			Color = ColorHelper.ColoredPixbuf (pin.PlotColor);
			Sequence = "";

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

		public string RealName { get; private set; }

		public APinTreeNode (APin pin)
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
		}


	}

	public class SignalTreeNode : Gtk.TreeNode
	{
		[Gtk.TreeNodeValue (Column = 0)]
		public string Name;
		[Gtk.TreeNodeValue (Column = 1)]
		public Gtk.Button btnAddRemove;

		public SignalTreeNode (Signal analogSignal)
		{
			Name = analogSignal.SignalName;
			btnAddRemove = new Button (){ WidthRequest = 24, HeightRequest = 24, Label = "-" };
		}

		public void ToggleButton (bool last = false)
		{
			if (last) {
				btnAddRemove.Label = "+";
			} else {
				btnAddRemove.Label = "-";
			}
		}
	}

	public class SequenceOperationTreeNode : Gtk.TreeNode
	{
		[Gtk.TreeNodeValue (Column = 0)]
		public TimeSpan Duration;
		[Gtk.TreeNodeValue (Column = 1)]
		public DPinState State;

		public SequenceOperationTreeNode (SequenceOperation seqop)
		{
			Duration = seqop.Duration;
			State = seqop.State;
		}
	}

	public class SequenceTreeNode : Gtk.TreeNode
	{
		[Gtk.TreeNodeValue (Column = 0)]
		public string Name;
		[Gtk.TreeNodeValue (Column = 1)]
		public string Pins;

		public SequenceTreeNode (Sequence seq)
		{
			Name = seq.Name;
			Pins = String.Format ("{0}({1})", seq.Pin.Name, seq.Pin.Number);
		}
	}
}