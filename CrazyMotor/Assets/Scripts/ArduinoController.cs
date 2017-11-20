using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class ArduinoController : MonoBehaviour {

    public Text RDegree;
    public Text LDegree;
    public int Rmax = 75;
    public int Lmax = 75;
    public int repeatTime = 1;
    public int[] RRepeatdegree;
    public int[] LRepeatdegree;

    private CommunicateWithArduino arduino;

    // Use this for initialization
    private void Start ()
    {
        //connectToArdunio();
        arduino = new CommunicateWithArduino ();
        new Thread (arduino.connectToArdunio).Start ();
        arduino.setLmax(Lmax);
        arduino.setRmax(Rmax);
        arduino.setrepeatTime(repeatTime);
        arduino.setRRepeatdegree(RRepeatdegree);
        arduino.setLRepeatdegree(LRepeatdegree);
    }

    // Update is called once per frame
    private void Update ()
    {
        LDegree.text = arduino.getLnow().ToString();
        RDegree.text = arduino.getRnow().ToString();
	}
    //Add Random Degree
    public void AddRandomRightButtonOnClick()
    {
       arduino.setRandomNumber(UnityEngine.Random.Range(1,10));
       new Thread (arduino.RMotorAddRandomDegree).Start();
    }
    public void AddRandomLeftButtonOnClick()
    {
       arduino.setRandomNumber(UnityEngine.Random.Range(1,10));
       new Thread (arduino.LMotorAddRandomDegree).Start();
    }
    public void AddRandomAllButtonOnClick()
    {
       arduino.setRandomNumber(UnityEngine.Random.Range(1,10));
       new Thread (arduino.AllMotorAddRandomDegree).Start();
    }

    //Add 5 Degree
    public void AddFiveRightButtonOnClick()
    {
       new Thread (arduino.RMotorAdd5Degree).Start();
    }
    public void AddFiveLeftButtonOnClick()
    {
       new Thread (arduino.LMotorAdd5Degree).Start();
    }
    public void AddFiveAllButtonOnClick()
    {
       new Thread (arduino.AllMotorAdd5Degree).Start();
    }

    //Motor Repeat
    public void RepeatRightButtonOnClick()
    {
       new Thread (arduino.RMotorRepeat).Start();
    }
    public void RepeatLeftButtonOnClick()
    {
       new Thread (arduino.LMotorRepeat).Start();
    }
    public void RepeatAllButtonOnClick()
    {
       new Thread (arduino.AllMotorRepeat).Start();
    }

    //Motor Reset
    public void ResetRightButtonOnClick()
    {
       new Thread (arduino.resetRMotor).Start();
    }
    public void ResetLeftButtonOnClick()
    {
       new Thread (arduino.resetLMotor).Start();
    }
    public void ResetAllButtonOnClick()
    {
       new Thread (arduino.resetAllMotor).Start();
    }
}

class CommunicateWithArduino
{

    public bool connected = true;
    public bool mac = true;
    public string choice = "tty.usbmodem1421";

    private int Rmax = 75;
    private int Rnow = 0;
    private int Lmax = 75;
    private int Lnow = 150;
    private int repeatTime = 1;
    private int[] RRepeatdegree;
    private int[] LRepeatdegree;
    private SerialPort arduinoController;
    private int RandomNumber = 0;
    
    public void setRmax (int tmp){Rmax = tmp <= 75 ? tmp : 75;}
    public void setLmax (int tmp){Lmax = tmp <= 75 ? tmp : 75;}
    public int getRnow (){return Rnow;}
    public int getLnow (){return Lnow;}
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
        string[] newsArr;
        newsArr = data.Split(' ');
        if(int.Parse(newsArr[0]) <= Rmax && int.Parse(newsArr[1]) >= Lmax)
        {
            Rnow = int.Parse(newsArr[0]);
            Lnow = int.Parse(newsArr[1]);
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
        else
        {
            Debug.Log("角度太大了！！");
        }
    }

    //reset Motor
    public void resetAllMotor()
    {
        SendData("0 150");
        Thread.Sleep(1000);
    }
    public void resetRMotor()
    {
        SendData("0 "+Lnow);
        Thread.Sleep(1000);
    }
    public void resetLMotor()
    {
        SendData(Rnow+" 150");
        Thread.Sleep(1000);
    }

    //Add 5 Degree
    public void AllMotorAdd5Degree()
    {
        SendData((Rnow+5)+" "+(Lnow-5));
        Thread.Sleep(1000);
    }
    public void RMotorAdd5Degree()
    {
        SendData((Rnow+5)+" "+Lnow);
        Thread.Sleep(1000);
    }
    public void LMotorAdd5Degree()
    {
        SendData(Rnow+" "+(Lnow-5));
        Thread.Sleep(1000);
    }

    //Add Random Degree
    public void setRandomNumber(int tmp)
    {
        RandomNumber = tmp;
    }    
    public void AllMotorAddRandomDegree()
    {
        SendData((Rnow+RandomNumber)+" "+(Lnow-RandomNumber));
        Thread.Sleep(1000);
    }
    public void RMotorAddRandomDegree()
    {
        SendData((Rnow+RandomNumber)+" "+Lnow);
        Thread.Sleep(1000);
    }
    public void LMotorAddRandomDegree()
    {
        SendData(Rnow+" "+(Lnow-RandomNumber));
        Thread.Sleep(1000);
    }

    //Set Degree
    public void setRRepeatdegree(int[] TmpRepeatdegree)
    {
        RRepeatdegree = TmpRepeatdegree;
    }
    public void setLRepeatdegree(int[] TmpRepeatdegree)
    {
        LRepeatdegree = TmpRepeatdegree;
    }
    
    //Repeat Motor
    public void setrepeatTime(int TmpRepeat)
    {
        repeatTime = TmpRepeat;
    }
    public void RMotorRepeat()
    {
        int tmp = 0;
        for(int i = 0 ; i < repeatTime ; i++)
        {
            tmp = i % RRepeatdegree.Length;
            SendData(RRepeatdegree[tmp]+" "+Lnow);
            Thread.Sleep(1000);
        }
    }
    public void LMotorRepeat()
    {
        int tmp = 0;
        for(int i = 0 ; i < repeatTime ; i++)
        {
            tmp = i % LRepeatdegree.Length;
            SendData(Rnow+" "+LRepeatdegree[tmp]);
            Thread.Sleep(1000);
        }
    }
    public void AllMotorRepeat()
    {
        int Rtmp = 0;
        int Ltmp = 0;
        for(int i = 0 ; i < repeatTime ; i++)
        {
            Rtmp = i % RRepeatdegree.Length;
            Ltmp = i % LRepeatdegree.Length;
            SendData(RRepeatdegree[Rtmp]+" "+LRepeatdegree[Ltmp]);
            Thread.Sleep(1000);
        }
    }
}