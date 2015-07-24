﻿using System;
using System.Linq;
using PrototypeBackend;
using OxyPlot;
using OxyPlot.GtkSharp;
using OxyPlot.Axes;
using Gdk;
using Gtk;
using GUIHelper;
using GLib;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Configuration;


namespace SequenceConfigurationsDialog
{
	public partial class SequenceConfiguration : Gtk.Dialog
	{
		public Sequence PinSequence{ get { return pinSequence; } set { pinSequence = value; } }

		private Sequence pinSequence;

		private List<SequenceOperation> SeqOps = new List<SequenceOperation> ();

		private DPin[] DPins;

		private DPin selectedPin;

		//Oxyplot----
		private OxyPlot.GtkSharp.PlotView plotView;
		private OxyPlot.PlotModel plotModel;
		private OxyPlot.Series.LineSeries sequenceSeries;

		//NodeView
		private NodeStore nvSequenceOptionsStore = new NodeStore (typeof(SequenceOperationTreeNode));

		public SequenceConfiguration (DPin[] Pins, Sequence seq = null)
		{
			this.Build ();

			SetupNodeView ();
			SetupOxyPlot ();

			if (seq != null) {
				PinSequence = seq;
				SeqOps = seq.Chain;
			} else {
				PinSequence = new Sequence ();
			}

			DPins = Pins;

			for (int i = 0; i < DPins.Length; i++) {
				cbPin.AppendText (string.Format ("{0}(D{1})", DPins [i].Name, DPins [i].Number));
			}
		}

		private void SetupOxyPlot ()
		{
			var XAxis = new OxyPlot.Axes.TimeSpanAxis {
				Position = AxisPosition.Bottom,
				AbsoluteMinimum = -0.1,
				LabelFormatter = x => {
					if ((int)x == 0)
						return "Start";
					return TimeSpanAxis.ToTimeSpan (x).ToString ("c");
				},
				MajorGridlineThickness = 1,
				MajorGridlineStyle = OxyPlot.LineStyle.Solid,
				MinorGridlineColor = OxyPlot.OxyColors.LightGray,
				MinorGridlineStyle = OxyPlot.LineStyle.Dot,
				MinorGridlineThickness = .5
			};

			var YAxis = new OxyPlot.Axes.LinearAxis {
				Position = AxisPosition.Left,
				Minimum = -0.1,
				Maximum = 1.1,
				LabelFormatter = x => ((int)x == 0) ? "LOW" : "HIGH",
				IsPanEnabled = false,
				IsZoomEnabled = false,
				AbsoluteMaximum = 1.1,
				AbsoluteMinimum = -0.1,
				MinorStep = 1,
				MajorStep = 1,
				MajorGridlineColor = OxyPlot.OxyColors.Gray,
				MajorGridlineThickness = 1,
				MajorGridlineStyle = OxyPlot.LineStyle.Solid,
			};

			sequenceSeries = new OxyPlot.Series.LineSeries () {
			};

			plotModel = new PlotModel {
				PlotType = PlotType.XY,
				Background = OxyPlot.OxyColors.White,
			};
			plotModel.Axes.Add (YAxis);
			plotModel.Axes.Add (XAxis);
			plotModel.Series.Add (sequenceSeries);
			plotView = new PlotView (){ Name = "", Model = plotModel };

			vboxOptions.Add (plotView);
			((Box.BoxChild)(vboxOptions [plotView])).Position = 2;

			plotView.SetSizeRequest (nvSequenceOptions.WidthRequest, this.HeightRequest / 3);

			plotView.ShowAll ();
		}

		private void SetupNodeView ()
		{
			nvSequenceOptions.NodeStore = nvSequenceOptionsStore;
			nvSequenceOptions.AppendColumn (new TreeViewColumn ("Duration", new CellRendererText (), "text", 0));
			nvSequenceOptions.AppendColumn (new TreeViewColumn ("State", new CellRendererText (), "text", 1));

			nvSequenceOptions.Show ();
		}

		protected void OnCbPinChanged (object sender, EventArgs e)
		{
			int nr = 0;
			var reg = Regex.Match (cbPin.ActiveText, @"\(D([0-9]+)\)");
			reg = Regex.Match (reg.Value, @"\d");
			if (reg.Success) {
				nr = Convert.ToInt32 (reg.Value);
			
				for (int i = 0; i < DPins.Length; i++) {
					if (DPins [i].Number == nr) {
						selectedPin = DPins [i];
						sequenceSeries.Color = OxyPlot.OxyColor.FromUInt32 (ColorHelper.RGBAFromGdkColor (DPins [i].PlotColor));
						plotView.ShowAll ();
						break;
					}
				}
			}
		}

		protected void OnButtonOkClicked (object sender, EventArgs e)
		{
			pinSequence = new Sequence {
				Name = entryName.Text,
				Pin = selectedPin,
				Repetitions = (rbRepeateContinously.Active) ? -1 : sbRadioBtnStopAfter.ValueAsInt,
				Chain = SeqOps 
			};
				
			Respond (ResponseType.Apply);
		}
	}
}
