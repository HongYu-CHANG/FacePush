using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using SMarto;

public class ArduinoController : MonoBehaviour {

    public Text RDegree;
    public Text LDegree;
    public int Rmax = 75;
    public int Lmax = 75;
    public int repeatTime = 1;
    public int[] RRepeatdegree;
    public int[] LRepeatdegree;

    private CommunicateWithArduino Uno;
    private MotorControl Rmotor;
    private MotorControl Lmotor;

    // Use this for initialization
    private void Start ()
    {
        Uno = new CommunicateWithArduino();
        new Thread (Uno.connectToArdunio).Start ();
        
        Rmotor = new MotorControl();
        Rmotor.initial(0, 75, true);
        Lmotor = new MotorControl();
        Lmotor.initial(150, 75, false);
    }

    // Update is called once per frame
    private void Update ()
    {
	}
    //Add Random Degree
    public void AddRandomRightButtonOnClick()
    {
       Uno.SendData(Rmotor.addDegree(UnityEngine.Random.Range(2,12))+" 100");
       
    }
    public void AddRandomLeftButtonOnClick()
    {
       
    }
    public void AddRandomAllButtonOnClick()
    {
       
    }

    //Sub Random Degree
    public void SubRandomRightButtonOnClick()
    {
      
    }
    public void SubRandomLeftButtonOnClick()
    {
       
    }
    public void SubRandomAllButtonOnClick()
    {
       
    }

    //Add 5 Degree
    public void AddFiveRightButtonOnClick()
    {
       
    }
    public void AddFiveLeftButtonOnClick()
    {
       
    }
    public void AddFiveAllButtonOnClick()
    {
       
    }

    //Sub 5 Degree
    public void SubFiveRightButtonOnClick()
    {
       
    }
    public void SubFiveLeftButtonOnClick()
    {
       
    }
    public void SubFiveAllButtonOnClick()
    {
       
    }

    //Motor Repeat
    public void RepeatRightButtonOnClick()
    {
       
    }
    public void RepeatLeftButtonOnClick()
    {
       
    }
    public void RepeatAllButtonOnClick()
    {
       
    }

    //Motor Reset
    public void ResetRightButtonOnClick()
    {
       
    }
    public void ResetLeftButtonOnClick()
    {
       
    }
    public void ResetAllButtonOnClick()
    {
       
    }

    class CommunicateWithArduino
    {

        public bool connected = true;
        public bool mac = true;
        public string choice = "tty.usbmodem1411";

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
            
            string[] newsArr;
            newsArr = data.Split(' ');
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
            else
            {
                Debug.Log("角度太大了！！");
            }
        }
    }
}