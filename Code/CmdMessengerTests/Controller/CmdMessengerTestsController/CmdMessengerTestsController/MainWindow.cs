using System;
using System.Timers;
using Gtk;
using ArduinoController;
using CommandMessenger;
using CommandMessenger.Transport.Serial;
using SamplerLogger;
using System.Collections.Generic;

public partial class MainWindow: Gtk.Window
{
	private ArduinoController.ArduinoController _arduinoController;
	private Timer AnalogTimer;

	private CSVLogger logger = new CSVLogger ("test.csv", new List<string>{ "A0", "A1", "A2", "A3", "A4", "A5" }, false, false);

	public MainWindow () : base (Gtk.WindowType.Toplevel)
	{
		Build ();

		_arduinoController = new ArduinoController.ArduinoController ();
		AnalogTimer = new Timer (500);
		AnalogTimer.Elapsed += FetchAnalog;

		LabelValueA.Text = "Hello World";

		initializeComponents ();
	}

	void initializeComponents ()
	{
		logger.Start ();
		PreparePortNames ();
	}

	private void PreparePortNames ()
	{
		((ListStore)CBSerialPorts.Model).Clear ();
		foreach (string s in System.IO.Ports.SerialPort.GetPortNames())
		{
			CBSerialPorts.AppendText (s);
		}
		CBSerialPorts.Active = 0;
	}

	void Connect (object sender, EventArgs e)
	{
		if (!_arduinoController.IsConnected)
		{
			//_arduinoController = new ArduinoController.ArduinoController ();
			_arduinoController.SerialPortName = CBSerialPorts.ActiveText;
			_arduinoController.Setup ();

			if (_arduinoController.IsConnected)
			{
				BtnConnectRefresh.Sensitive = false;
				BtnConnect.Label = "Disconnect";
				LabelConnectionStatus.Text = @"<b>Connected</b>";
				LabelConnectionStatus.UseMarkup = true;
				AnalogTimer.Start ();
			} else
			{
				LabelConnectionStatus.Text = @"<b>Something went wrong!</b>";
				LabelConnectionStatus.UseMarkup = true;
			}
		} else
		{
			AnalogTimer.Stop ();
			BtnConnect.Label = "Connect";
			BtnConnectRefresh.Sensitive = true;
			LabelConnectionStatus.Text = @"<b>Not</b> connected";
			LabelConnectionStatus.UseMarkup = true;
			_arduinoController.Disconnect ();
		}
	}

	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		logger.Stop ();
		_arduinoController.Exit ();
		Application.Quit ();
		a.RetVal = true;
	}

	private void FetchAnalog (object o, EventArgs e)
	{
		_arduinoController.ReadAnalogPin (0);
		_arduinoController.ReadAnalogPin (1);
		_arduinoController.ReadAnalogPin (2);
		_arduinoController.ReadAnalogPin (3);
		_arduinoController.ReadAnalogPin (4);
		_arduinoController.ReadAnalogPin (5);

		var tmp = new List<float> ();
		tmp.Add (_arduinoController.AnalogValues [0] [_arduinoController.AnalogValues [0].Count - 1]);
		tmp.Add (_arduinoController.AnalogValues [1] [_arduinoController.AnalogValues [1].Count - 1]);
		tmp.Add (_arduinoController.AnalogValues [2] [_arduinoController.AnalogValues [2].Count - 1]);
		tmp.Add (_arduinoController.AnalogValues [3] [_arduinoController.AnalogValues [3].Count - 1]);
		tmp.Add (_arduinoController.AnalogValues [4] [_arduinoController.AnalogValues [4].Count - 1]);
		tmp.Add (_arduinoController.AnalogValues [5] [_arduinoController.AnalogValues [5].Count - 1]);

		logger.Log (tmp);

		Gtk.Application.Invoke (delegate
		{
			LabelValueA.Text = Convert.ToString (_arduinoController.AnalogValues [0] [_arduinoController.AnalogValues [0].Count - 1]);
			LabelValueA1.Text = Convert.ToString (_arduinoController.AnalogValues [1] [_arduinoController.AnalogValues [1].Count - 1]);
			LabelValueA2.Text = Convert.ToString (_arduinoController.AnalogValues [2] [_arduinoController.AnalogValues [2].Count - 1]);
			LabelValueA3.Text = Convert.ToString (_arduinoController.AnalogValues [3] [_arduinoController.AnalogValues [3].Count - 1]);
			LabelValueA4.Text = Convert.ToString (_arduinoController.AnalogValues [4] [_arduinoController.AnalogValues [4].Count - 1]);
			LabelValueA5.Text = Convert.ToString (_arduinoController.AnalogValues [5] [_arduinoController.AnalogValues [5].Count - 1]);
		});
	}

	private void SetDPIN (int PinNr, PinMode Mode, DPinState State)
	{
		_arduinoController.SetPin (PinNr, Mode, State);
	}

	protected void OnCheckbutton1Toggled (object sender, EventArgs e)
	{
		SetDPIN (13, PinMode.OUTPUT, (((CheckButton)sender).Active ? DPinState.HIGH : DPinState.LOW));
	}

	protected void OnCombobox19Changed (object sender, EventArgs e)
	{
		_arduinoController.SetAnalogPinMode (14, PinMode.OUTPUT);
	}

	protected void OnHScaleAnalogPinNullValueChanged (object sender, EventArgs e)
	{
		_arduinoController.SetAnalogPin (14, Convert.ToInt16 (HScaleAnalogPinNull.Adjustment.Value));
	}

	protected void OnCBAnalogPin5ModeChanged (object sender, EventArgs e)
	{
		_arduinoController.SetAnalogPinMode (19, PinMode.OUTPUT);
	}

	protected void OnHScaleAnalogPinFiveValueChanged (object sender, EventArgs e)
	{
		_arduinoController.SetAnalogPin (19, Convert.ToInt16 (HScaleAnalogPinFive.Adjustment.Value));
	}

	protected void OnBtnConnectRefreshClicked (object sender, EventArgs e)
	{
		PreparePortNames ();
	}

	protected void OnBtnDataCollectClicked (object sender, EventArgs e)
	{
		textview2.Buffer.Clear ();

		_arduinoController.GetVersion ();
		_arduinoController.GetModel ();
		_arduinoController.GetNumberDigitalPins ();
		_arduinoController.GetNumberAnalogPins ();
		_arduinoController.GetDigitalBitMask ();



		textview2.Buffer.Text += "VERSION:\t" + _arduinoController.Version + "\n";
		textview2.Buffer.Text += "MODEL:\t" + _arduinoController.Model + "\n";
		textview2.Buffer.Text += "NUMBER OF DIGITAL PINS:\t" + Convert.ToString (_arduinoController.NumberOfDigitalPins) + "\n";
		textview2.Buffer.Text += "NUMBER OF ANALOG PINS:\t" + Convert.ToString (_arduinoController.NumberOfAnalogPins) + "\n";
		textview2.Buffer.Text += "DIGITAL BIT MASK:\t" + Convert.ToString (_arduinoController.DigitalBitMask, 2) + "\n";

	}
}
