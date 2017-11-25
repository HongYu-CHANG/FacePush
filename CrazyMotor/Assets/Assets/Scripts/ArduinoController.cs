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
       Uno.SendData(Rmotor.addDegree(UnityEngine.Random.Range(2,12))+" "+Lmotor.getnowDegree());
       
    }
    public void AddRandomLeftButtonOnClick()
    {
       Uno.SendData(Rmotor.getnowDegree()+" "+Lmotor.addDegree(UnityEngine.Random.Range(2,12)));
    }
    public void AddRandomAllButtonOnClick()
    {
       Uno.SendData(Rmotor.addDegree(UnityEngine.Random.Range(2,12))+" "+Lmotor.addDegree(UnityEngine.Random.Range(2,12)));
    }

    //Sub Random Degree
    public void SubRandomRightButtonOnClick()
    {
      Uno.SendData(Rmotor.subDegree(UnityEngine.Random.Range(2,12))+" "+Lmotor.getnowDegree());
    }
    public void SubRandomLeftButtonOnClick()
    {
        Uno.SendData(Rmotor.getnowDegree()+" "+Lmotor.subDegree(UnityEngine.Random.Range(2,12)));
    }
    public void SubRandomAllButtonOnClick()
    {
        Uno.SendData(Rmotor.subDegree(UnityEngine.Random.Range(2,12))+" "+Lmotor.subDegree(UnityEngine.Random.Range(2,12)));
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
        int Rtmp = 0;
        int Ltmp = 0;
        for(int i = 0 ; i < repeatTime ; i++)
        {
            Rtmp = i % RRepeatdegree.Length;
            Uno.SendData(Rmotor.repeatMotor(RRepeatdegree, Rtmp)+" "+Lmotor.getnowDegree());
        }
    }
    public void RepeatLeftButtonOnClick()
    {
        int Rtmp = 0;
        int Ltmp = 0;
        for(int i = 0 ; i < repeatTime ; i++)
        {
            Ltmp = i % LRepeatdegree.Length;
            Uno.SendData(Rmotor.getnowDegree()+" "+Lmotor.repeatMotor(LRepeatdegree, Ltmp));
        }
    }
    public void RepeatAllButtonOnClick()
    {
        int Rtmp = 0;
        int Ltmp = 0;
        for(int i = 0 ; i < repeatTime ; i++)
        {
            Rtmp = i % RRepeatdegree.Length;
            Ltmp = i % LRepeatdegree.Length;
            Uno.SendData(Rmotor.repeatMotor(RRepeatdegree, Rtmp)+" "+Lmotor.repeatMotor(LRepeatdegree, Ltmp));
        }
    }

    //Motor Reset
    public void ResetRightButtonOnClick()
    {
        Uno.SendData(Rmotor.reset()+" "+Lmotor.getnowDegree());
    }
    public void ResetLeftButtonOnClick()
    {
        Uno.SendData(Rmotor.getnowDegree()+" "+Lmotor.reset());
    }
    public void ResetAllButtonOnClick()
    {
        Uno.SendData(Rmotor.reset()+" "+Lmotor.reset());
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
                    portChoice = "/dev/" + choice;
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
            else
            {
                Debug.Log("未連接");
            }
            Thread.Sleep(1000);
        }
    }
}