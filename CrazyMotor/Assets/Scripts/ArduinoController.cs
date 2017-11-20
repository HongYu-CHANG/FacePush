using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class ArduinoController : MonoBehaviour {

    //serial connecting
    
    public Dropdown RightDropdown;
    public Dropdown LeftDropdown;
    public InputField DurationField;
    public InputField RepeatField;
    public Button SendButton;

    private CommunicateWithArduino arduino;
    private List<Dropdown.OptionData> RightOptions;
    private List<Dropdown.OptionData> LeftOptions;

    // Use this for initialization
    private void Start ()
    {
        //connectToArdunio();
         arduino = new CommunicateWithArduino ();
        new Thread (arduino.connectToArdunio).Start ();
        
        Button tempBtn = SendButton.GetComponent<Button>();
        tempBtn.onClick.AddListener(sendButtonOnClick);
    }

    // Update is called once per frame
    private void Update ()
    {
		//Debug.Log(arduinoController.ReadLine());
	}
    public void sendButtonOnClick()
    {
       arduino.SendData("90 88");
    }
}

class CommunicateWithArduino
{

    public bool connected = true;
    public bool mac = true;
    public string choice = "tty.usbmodem1421";

    private SerialPort arduinoController;
    
    public void connectToArdunio()
    {
        if (connected)
        {
            string portChoice = "COM4";
            if (mac)
            {
                int p = (int)Environment.OSVersion.Platform;
                // Are we on Unix?
                if (p == 4 || p == 128 || p == 6)
                {
                    List<string> serial_ports = new List<string>();
                    string[] ttys = Directory.GetFiles("/dev/", "tty.*");
                    foreach (string dev in ttys)
                    {
                        if (dev.StartsWith("/dev/tty.")){
                            serial_ports.Add(dev);
                            Debug.Log (String.Format (dev));
                        }
                    }
                }
                //controller = new SerialPort ("/dev/tty.usbmodem1411");
                portChoice = "/dev/" + choice;
                //Debug.Log(portChoice);
                //arduinoController = new SerialPort(portChoice);
            }
            arduinoController =new SerialPort(portChoice, 115200, Parity.None, 8, StopBits.One);
            arduinoController.Handshake = Handshake.None;
            arduinoController.RtsEnable = true;
            arduinoController.Open();
        }
       
    }
    public void SendData(String data)
    {
        Debug.Log(data);
            if (connected)
            {
                if (arduinoController != null)
                {
                    arduinoController.Write(data);
                    arduinoController.Write("\n");
                }
                else
                {
                    Debug.Log("nullport");
                }
            }
    }
}