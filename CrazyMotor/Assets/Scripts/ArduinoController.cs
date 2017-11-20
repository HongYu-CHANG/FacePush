using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class ArduinoController : MonoBehaviour {

    //serial connecting

    public Button SendButton;

    private CommunicateWithArduino arduino;

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
       new Thread (arduino.initailDecide).Start ();
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
    public string readArduino()
    {
        return arduinoController.ReadLine ();
    }
    public void initailDecide()
    {
        for (int i = 0; i <= 150; i+=5)
       {
           SendData(i.ToString() + " " + (150-i).ToString());
           Thread.Sleep(60);
           SendData("R");
           Thread.Sleep(60);
           Debug.Log(readArduino());
           Thread.Sleep(60);
           
       }
    }
}