using System;
using System.Timers;
using Gtk;
using SamplerLogger;
using System.Collections.Generic;
using PrototypeBackend;

public partial class MainWindow: Gtk.Window
{
	//	private ArduinoController _arduinoController;
	private Timer AnalogTimer;

	private CSVLogger logger = new CSVLogger ("test.csv", new List<string>{ "A0", "A1", "A2", "A3", "A4", "A5" }, false, false);

	public MainWindow () : base (Gtk.WindowType.Toplevel)
	{
		Build ();
		initializeComponents ();
	}

	void initializeComponents ()
	{
		ArduinoController.Init ();
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
		if (!ArduinoController.IsConnected)
		{
			//_arduinoController = new ArduinoController.ArduinoController ();
			ArduinoController.SerialPortName = PortName;
			ArduinoController.Setup ();

			if (ArduinoController.IsConnected)
			{
				BtnConnectRefresh.Sensitive = false;
				BtnConnect.Label = "Disconnect";
				LabelConnectionStatus.Text = @"<b>Connected</b>";
				LabelConnectionStatus.UseMarkup = true;
				nbMain.Sensitive = true;
//				AnalogTimer.Start ();
			} else
			{
				LabelConnectionStatus.Text = @"<b>Something went wrong!</b>";
				LabelConnectionStatus.UseMarkup = true;
			}
		} else
		{
//			AnalogTimer.Stop ();
			BtnConnect.Label = "Connect";
			BtnConnectRefresh.Sensitive = true;
			LabelConnectionStatus.Text = @"<b>Not</b> connected";
			LabelConnectionStatus.UseMarkup = true;
			ArduinoController.Disconnect ();
			nbMain.Sensitive = false;
		}
	}

	void AutoConnect (object sender, EventArgs e)
	{
		foreach (string s in System.IO.Ports.SerialPort.GetPortNames())
		{
			ConnectIntern (s);
			if (ArduinoController.IsConnected)
			{
				return;
			}
		}
	}

	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		logger.Stop ();
		ArduinoController.Exit ();
		Application.Quit ();
		a.RetVal = true;
	}

	private void FetchAnalog (object o, EventArgs e)
	{
		ArduinoController.ReadAnalogPin (0);
		ArduinoController.ReadAnalogPin (1);
		ArduinoController.ReadAnalogPin (2);
		ArduinoController.ReadAnalogPin (3);
		ArduinoController.ReadAnalogPin (4);
		ArduinoController.ReadAnalogPin (5);

		var tmp = new List<float> ();
		tmp.Add (ArduinoController.AnalogValues [0] [ArduinoController.AnalogValues [0].Count - 1]);
		tmp.Add (ArduinoController.AnalogValues [1] [ArduinoController.AnalogValues [1].Count - 1]);
		tmp.Add (ArduinoController.AnalogValues [2] [ArduinoController.AnalogValues [2].Count - 1]);
		tmp.Add (ArduinoController.AnalogValues [3] [ArduinoController.AnalogValues [3].Count - 1]);
		tmp.Add (ArduinoController.AnalogValues [4] [ArduinoController.AnalogValues [4].Count - 1]);
		tmp.Add (ArduinoController.AnalogValues [5] [ArduinoController.AnalogValues [5].Count - 1]);

		logger.Log (tmp);

		Gtk.Application.Invoke (delegate
		{
			LabelValueA.Text = Convert.ToString (ArduinoController.AnalogValues [0] [ArduinoController.AnalogValues [0].Count - 1]);
			LabelValueA1.Text = Convert.ToString (ArduinoController.AnalogValues [1] [ArduinoController.AnalogValues [1].Count - 1]);
			LabelValueA2.Text = Convert.ToString (ArduinoController.AnalogValues [2] [ArduinoController.AnalogValues [2].Count - 1]);
			LabelValueA3.Text = Convert.ToString (ArduinoController.AnalogValues [3] [ArduinoController.AnalogValues [3].Count - 1]);
			LabelValueA4.Text = Convert.ToString (ArduinoController.AnalogValues [4] [ArduinoController.AnalogValues [4].Count - 1]);
			LabelValueA5.Text = Convert.ToString (ArduinoController.AnalogValues [5] [ArduinoController.AnalogValues [5].Count - 1]);
		});
	}

	private void SetDPIN (int PinNr, PinMode Mode, DPinState State)
	{
		ArduinoController.SetPin (PinNr, Mode, State);
	}

	protected void OnCheckbutton1Toggled (object sender, EventArgs e)
	{
		SetDPIN (13, PinMode.OUTPUT, (((CheckButton)sender).Active ? DPinState.HIGH : DPinState.LOW));
	}

	protected void OnCombobox19Changed (object sender, EventArgs e)
	{
		ArduinoController.SetPinMode (14, PinMode.OUTPUT);
	}

	protected void OnHScaleAnalogPinNullValueChanged (object sender, EventArgs e)
	{
		ArduinoController.SetAnalogPin (14, Convert.ToInt16 (HScaleAnalogPinNull.Adjustment.Value));
	}

	protected void OnCBAnalogPin5ModeChanged (object sender, EventArgs e)
	{
		ArduinoController.SetPinMode (19, PinMode.OUTPUT);
	}

	protected void OnHScaleAnalogPinFiveValueChanged (object sender, EventArgs e)
	{
		ArduinoController.SetAnalogPin (19, Convert.ToInt16 (HScaleAnalogPinFive.Adjustment.Value));
	}

	protected void OnBtnConnectRefreshClicked (object sender, EventArgs e)
	{
		PreparePortNames ();
	}

	protected void OnBtnDataCollectClicked (object sender, EventArgs e)
	{
		ArduinoController.GetVersion ();
		ArduinoController.GetModel ();
		ArduinoController.GetNumberDigitalPins ();
		ArduinoController.GetNumberAnalogPins ();
		ArduinoController.GetPinOutputMask ();
		ArduinoController.GetPinModeMask ();
		ArduinoController.GetAnalogReference ();

		labelVersion.Text = ArduinoController.Version;
		labelModel.Text = ArduinoController.Model;
		labelNrDigiPin.Text = Convert.ToString (ArduinoController.NumberOfDigitalPins, 10);
		labelNrAnaPin.Text = Convert.ToString (ArduinoController.NumberOfAnalogPins, 10);
		labelOutputBitMask.Text = Convert.ToString (ArduinoController.PinOutputMask, 2).PadLeft ((int)ArduinoController.NumberOfDigitalPins);
		labelModeBitMask.Text = Convert.ToString (ArduinoController.PinModeMask, 2).PadLeft ((int)ArduinoController.NumberOfDigitalPins);
		string tmp = "";
		foreach (string s in ArduinoController.AnalogReferences.Keys)
		{
			tmp += s + " | ";
		}
		labelAnalogReferences.Text = tmp;
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
			ArduinoController.SetPinMode (pin, PinMode.INPUT);
			break;
		case 1:
			cbtn.Sensitive = true;
			ArduinoController.SetPinMode (pin, PinMode.OUTPUT);
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

	protected void OnButton4Clicked (object sender, EventArgs e)
	{
		ArduinoController.GetVersion ();
		labelVersion.Text = ArduinoController.Version;
	}

	protected void OnButton5Clicked (object sender, EventArgs e)
	{
		ArduinoController.GetModel ();
		labelModel.Text = ArduinoController.Model;
	}

	protected void OnButton6Clicked (object sender, EventArgs e)
	{
		ArduinoController.GetNumberDigitalPins ();
		labelNrDigiPin.Text = Convert.ToString (ArduinoController.NumberOfDigitalPins, 10);
	}

	protected void OnButton7Clicked (object sender, EventArgs e)
	{
		ArduinoController.GetNumberAnalogPins ();
		labelNrAnaPin.Text = Convert.ToString (ArduinoController.NumberOfAnalogPins, 10);
	}

	protected void OnButton1Clicked (object sender, EventArgs e)
	{
		ArduinoController.GetPinOutputMask ();
		labelOutputBitMask.Text = Convert.ToString (ArduinoController.PinOutputMask, 2).PadLeft ((int)ArduinoController.NumberOfDigitalPins);
	}

	protected void OnButton2Clicked (object sender, EventArgs e)
	{
		ArduinoController.GetPinModeMask ();
		labelModeBitMask.Text = Convert.ToString (ArduinoController.PinModeMask, 2).PadLeft ((int)ArduinoController.NumberOfDigitalPins);
	}

	protected void OnButton3Clicked (object sender, EventArgs e)
	{
		ArduinoController.GetAnalogReference ();
		string tmp = "";
		foreach (string s in ArduinoController.AnalogReferences.Keys)
		{
			tmp += s + " | ";
		}
		labelAnalogReferences.Text = tmp;
	}

	protected void OnCbtnDPin12Toggled (object sender, EventArgs e)
	{
		SetDPIN (12, PinMode.OUTPUT, (((CheckButton)sender).Active ? DPinState.HIGH : DPinState.LOW));
	}

	protected void OnCbtnDPin11Toggled (object sender, EventArgs e)
	{
		SetDPIN (11, PinMode.OUTPUT, (((CheckButton)sender).Active ? DPinState.HIGH : DPinState.LOW));
	}

	protected void OnCbtnDPin10Toggled (object sender, EventArgs e)
	{
		SetDPIN (10, PinMode.OUTPUT, (((CheckButton)sender).Active ? DPinState.HIGH : DPinState.LOW));
	}

	protected void OnCbtnDPin9Toggled (object sender, EventArgs e)
	{
		SetDPIN (9, PinMode.OUTPUT, (((CheckButton)sender).Active ? DPinState.HIGH : DPinState.LOW));
	}

	protected void OnCbtnDPin8Toggled (object sender, EventArgs e)
	{
		SetDPIN (8, PinMode.OUTPUT, (((CheckButton)sender).Active ? DPinState.HIGH : DPinState.LOW));
	}

	protected void OnCbtnDPin7Toggled (object sender, EventArgs e)
	{
		SetDPIN (7, PinMode.OUTPUT, (((CheckButton)sender).Active ? DPinState.HIGH : DPinState.LOW));
	}

	protected void OnCbtnDPin6Toggled (object sender, EventArgs e)
	{
		SetDPIN (6, PinMode.OUTPUT, (((CheckButton)sender).Active ? DPinState.HIGH : DPinState.LOW));
	}

	protected void OnCbtnDPin5Toggled (object sender, EventArgs e)
	{
		SetDPIN (5, PinMode.OUTPUT, (((CheckButton)sender).Active ? DPinState.HIGH : DPinState.LOW));
	}

	protected void OnCbtnDPin4Toggled (object sender, EventArgs e)
	{
		SetDPIN (4, PinMode.OUTPUT, (((CheckButton)sender).Active ? DPinState.HIGH : DPinState.LOW));
	}

	protected void OnCbtnDPin3Toggled (object sender, EventArgs e)
	{
		SetDPIN (3, PinMode.OUTPUT, (((CheckButton)sender).Active ? DPinState.HIGH : DPinState.LOW));
	}

	protected void OnCbtnDPin2Toggled (object sender, EventArgs e)
	{
		SetDPIN (2, PinMode.OUTPUT, (((CheckButton)sender).Active ? DPinState.HIGH : DPinState.LOW));
	}

	protected void OnCbtnDPin1Toggled (object sender, EventArgs e)
	{
		SetDPIN (1, PinMode.OUTPUT, (((CheckButton)sender).Active ? DPinState.HIGH : DPinState.LOW));
	}

	protected void OnCbtnDPinToggled (object sender, EventArgs e)
	{
		SetDPIN (0, PinMode.OUTPUT, (((CheckButton)sender).Active ? DPinState.HIGH : DPinState.LOW));
	}
}
