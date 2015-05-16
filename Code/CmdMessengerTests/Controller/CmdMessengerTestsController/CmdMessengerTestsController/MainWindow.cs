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
		initializeComponents ();
	}

	void initializeComponents ()
	{
		_arduinoController = new ArduinoController.ArduinoController ();
		AnalogTimer = new Timer (500);
		AnalogTimer.Elapsed += FetchAnalog;

		logger.Start ();
		PreparePortNames ();

		nbMain.Sensitive = false;
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
		ConnectIntern (CBSerialPorts.ActiveText);
	}

	void ConnectIntern (string PortName)
	{
		if (!_arduinoController.IsConnected)
		{
			//_arduinoController = new ArduinoController.ArduinoController ();
			_arduinoController.SerialPortName = PortName;
			_arduinoController.Setup ();

			if (_arduinoController.IsConnected)
			{
				BtnConnectRefresh.Sensitive = false;
				BtnConnect.Label = "Disconnect";
				LabelConnectionStatus.Text = @"<b>Connected</b>";
				LabelConnectionStatus.UseMarkup = true;
				nbMain.Sensitive = true;
				//	AnalogTimer.Start ();
			} else
			{
				LabelConnectionStatus.Text = @"<b>Something went wrong!</b>";
				LabelConnectionStatus.UseMarkup = true;
			}
		} else
		{
			//AnalogTimer.Stop ();
			BtnConnect.Label = "Connect";
			BtnConnectRefresh.Sensitive = true;
			LabelConnectionStatus.Text = @"<b>Not</b> connected";
			LabelConnectionStatus.UseMarkup = true;
			_arduinoController.Disconnect ();
			nbMain.Sensitive = false;
		}
	}

	void AutoConnect (object sender, EventArgs e)
	{
		foreach (string s in System.IO.Ports.SerialPort.GetPortNames())
		{
			ConnectIntern (s);
			if (_arduinoController.IsConnected)
			{
				return;
			}
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
		textview2.Buffer.Text += "DIGITAL BIT MASK:\t" + Convert.ToString (_arduinoController.DigitalBitMask, 2).PadLeft (_arduinoController.NumberOfDigitalPins
		, '0') + "\n";

	}

	protected void OnSetDigitalPinMode (object sender, EventArgs e)
	{
		CheckButton cbtn;
		int pin = -1;
		switch (((ComboBox)sender).Name)
		{
		case "cbDPin13":
			pin = 13;
			cbtn = cbtnDPin13;
			break;
		case "cbDPin12":
			pin = 12;
			cbtn = cbtnDPin12;
			break;
		case "cbDPin11":
			pin = 11;
			cbtn = cbtnDPin11;
			break;
		case "cbDPin10":
			pin = 10;
			cbtn = cbtnDPin10;
			break;
		case "cbDPin9":
			pin = 9;
			cbtn = cbtnDPin9;
			break;
		case "cbDPin8":
			pin = 8;
			cbtn = cbtnDPin8;
			break;
		case "cbDPin7":
			pin = 7;
			cbtn = cbtnDPin7;
			break;
		case "cbDPin6":
			pin = 6;
			cbtn = cbtnDPin6;
			break;
		case "cbDPin5":
			pin = 5;
			cbtn = cbtnDPin5;
			break;
		case "cbDPin4":
			pin = 4;
			cbtn = cbtnDPin4;
			break;
		case "cbDPin3":
			pin = 3;
			cbtn = cbtnDPin3;
			break;
		case "cbDPin2":
			pin = 2;
			cbtn = cbtnDPin2;
			break;
		case "cbDPin1":
			pin = 1;
			cbtn = cbtnDPin1;
			break;
		case "cbDPin":
			pin = 0;
			cbtn = cbtnDPin;
			break;
		default:
			return;
		}

		switch (((ComboBox)sender).Active)
		{
		case 0:
			cbtn.Sensitive = false;
			_arduinoController.SetPinMode (pin, PinMode.INPUT);
			break;
		case 1:
			cbtn.Sensitive = true;
			_arduinoController.SetPinMode (pin, PinMode.OUTPUT);
			break;
		default:
			break;
		}
	}

	protected void OnCbDPin13Changed (object sender, EventArgs e)
	{
		OnSetDigitalPinMode (sender, e);
	}

	protected void OnCbDPin12Changed (object sender, EventArgs e)
	{
		OnSetDigitalPinMode (sender, e);
	}

	protected void OnCbDPin11Changed (object sender, EventArgs e)
	{
		OnSetDigitalPinMode (sender, e);
	}

	protected void OnCbDPin10Changed (object sender, EventArgs e)
	{
		OnSetDigitalPinMode (sender, e);
	}

	protected void OnCbDPin9Changed (object sender, EventArgs e)
	{
		OnSetDigitalPinMode (sender, e);
	}

	protected void OnCbDPin8Changed (object sender, EventArgs e)
	{
		OnSetDigitalPinMode (sender, e);
	}

	protected void OnCbDPin7Changed (object sender, EventArgs e)
	{
		OnSetDigitalPinMode (sender, e);
	}

	protected void OnCbDPin6Changed (object sender, EventArgs e)
	{
		OnSetDigitalPinMode (sender, e);
	}

	protected void OnCbDPin5Changed (object sender, EventArgs e)
	{
		OnSetDigitalPinMode (sender, e);
	}

	protected void OnCbDPin4Changed (object sender, EventArgs e)
	{
		OnSetDigitalPinMode (sender, e);
	}

	protected void OnCbDPin3Changed (object sender, EventArgs e)
	{
		OnSetDigitalPinMode (sender, e);
	}

	protected void OnCbDPin2Changed (object sender, EventArgs e)
	{
		OnSetDigitalPinMode (sender, e);
	}

	protected void OnCbDPin1Changed (object sender, EventArgs e)
	{
		OnSetDigitalPinMode (sender, e);
	}

	protected void OnCbDPinChanged (object sender, EventArgs e)
	{
		OnSetDigitalPinMode (sender, e);
	}
}
