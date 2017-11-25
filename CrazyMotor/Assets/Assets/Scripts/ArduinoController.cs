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

    private CommunicateWithArduino arduino;
    private SMarto Rmotor;

    // Use this for initialization
    private void Start ()
    {
        //connectToArdunio();
        arduino = new CommunicateWithArduino ();
        new Thread (arduino.connectToArdunio).Start ();

        Rmotor = new Smarto();
        Rmotor.initial(0, 75, true);
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
       arduino.setRandomNumber(UnityEngine.Random.Range(2,12));
       new Thread (arduino.MotorAddRandomDegree).Start("Right");
    }
    public void AddRandomLeftButtonOnClick()
    {
       arduino.setRandomNumber(UnityEngine.Random.Range(2,12));
       new Thread (arduino.MotorAddRandomDegree).Start("Left");
    }
    public void AddRandomAllButtonOnClick()
    {
       arduino.setRandomNumber(UnityEngine.Random.Range(2,12));
       new Thread (arduino.MotorAddRandomDegree).Start("All");
    }

    //Sub Random Degree
    public void SubRandomRightButtonOnClick()
    {
       arduino.setRandomNumber(UnityEngine.Random.Range(2,12));
       new Thread (arduino.MotorSubRandomDegree).Start("Right");
    }
    public void SubRandomLeftButtonOnClick()
    {
       arduino.setRandomNumber(UnityEngine.Random.Range(2,12));
       new Thread (arduino.MotorSubRandomDegree).Start("Left");
    }
    public void SubRandomAllButtonOnClick()
    {
       arduino.setRandomNumber(UnityEngine.Random.Range(2,12));
       new Thread (arduino.MotorSubRandomDegree).Start("All");
    }

    //Add 5 Degree
    public void AddFiveRightButtonOnClick()
    {
       new Thread (arduino.MotorAdd5Degree).Start("Right");
    }
    public void AddFiveLeftButtonOnClick()
    {
       new Thread (arduino.MotorAdd5Degree).Start("Left");
    }
    public void AddFiveAllButtonOnClick()
    {
       new Thread (arduino.MotorAdd5Degree).Start("All");
    }

    //Sub 5 Degree
    public void SubFiveRightButtonOnClick()
    {
       new Thread (arduino.MotorSub5Degree).Start("Right");
    }
    public void SubFiveLeftButtonOnClick()
    {
       new Thread (arduino.MotorSub5Degree).Start("Left");
    }
    public void SubFiveAllButtonOnClick()
    {
       new Thread (arduino.MotorSub5Degree).Start("All");
    }

    //Motor Repeat
    public void RepeatRightButtonOnClick()
    {
       new Thread (arduino.MotorRepeat).Start("Right");
    }
    public void RepeatLeftButtonOnClick()
    {
       new Thread (arduino.MotorRepeat).Start("Left");
    }
    public void RepeatAllButtonOnClick()
    {
       new Thread (arduino.MotorRepeat).Start("All");
    }

    //Motor Reset
    public void ResetRightButtonOnClick()
    {
       new Thread (arduino.resetMotor).Start("Right");
    }
    public void ResetLeftButtonOnClick()
    {
       new Thread (arduino.resetMotor).Start("Left");
    }
    public void ResetAllButtonOnClick()
    {
       new Thread (arduino.resetMotor).Start("All");
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
        
        string[] newsArr;
        newsArr = data.Split(' ');
        if(int.Parse(newsArr[0]) <= Rmax && int.Parse(newsArr[1]) >= Lmax)
        {
            Rnow = int.Parse(newsArr[0]);
            if(Rnow < 0) 
            {    
                Rnow = 0;
                data = "0 "+newsArr[1];
            }
            Lnow = int.Parse(newsArr[1]);
            if(Lnow > 150) 
            {
                Lnow = 150;
                data = newsArr[0]+" 150";
            }
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
        else
        {
            Debug.Log("角度太大了！！");
        }
    }

    //reset Motor
    public void resetMotor(object obj)
    {
        string str = obj as string;
        if(str == "All" )
        {
            SendData("0 150");
        }
        else if (str == "Right")
        {
            SendData("0 "+Lnow);
        }
        else if (str == "Left")
        {
            SendData(Rnow+" 150");
        }
        Thread.Sleep(1000);
    }

    //Add 5 Degree
    public void MotorAdd5Degree(object obj)
    {
        string str = obj as string;
        if(str == "All" )
        {
            SendData((Rnow+5)+" "+(Lnow-5));
        }
        else if (str == "Right")
        {
            SendData((Rnow+5)+" "+Lnow);
        }
        else if (str == "Left")
        {
            SendData(Rnow+" "+(Lnow-5));
        }
        Thread.Sleep(1000);
    }

    //Sub 5 Degree
    public void MotorSub5Degree(object obj)
    {
        string str = obj as string;
        if(str == "All" )
        {
            SendData((Rnow-5)+" "+(Lnow+5));
        }
        else if (str == "Right")
        {
            SendData((Rnow-5)+" "+Lnow);
        }
        else if (str == "Left")
        {
            SendData(Rnow+" "+(Lnow+5));
        }
        Thread.Sleep(1000);
    }

    //Add Random Degree
    public void setRandomNumber(int tmp)
    {
        RandomNumber = tmp;
    }    
    public void MotorAddRandomDegree(object obj)
    {
        string str = obj as string;
        if(str == "All" )
        {
            SendData((Rnow+RandomNumber)+" "+(Lnow-RandomNumber));
        }
        else if (str == "Right")
        {
            SendData((Rnow+RandomNumber)+" "+Lnow);
        }
        else if (str == "Left")
        {
            SendData(Rnow+" "+(Lnow-RandomNumber));
        }
        Thread.Sleep(1000);
    }

     //Sub Random Degree
    public void MotorSubRandomDegree(object obj)
    {
        string str = obj as string;
        if(str == "All" )
        {
            SendData((Rnow-RandomNumber)+" "+(Lnow+RandomNumber));
        }
        else if (str == "Right")
        {
            SendData((Rnow-RandomNumber)+" "+Lnow);
        }
        else if (str == "Left")
        {
            SendData(Rnow+" "+(Lnow+RandomNumber));
        }
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
    public void MotorRepeat(object obj)
    {
        string str = obj as string;
        int Rtmp = 0;
        int Ltmp = 0;
        for(int i = 0 ; i < repeatTime ; i++)
        {
            if(str == "All" )
            {
                Rtmp = i % RRepeatdegree.Length;
                Ltmp = i % LRepeatdegree.Length;
                SendData(RRepeatdegree[Rtmp]+" "+LRepeatdegree[Ltmp]);
            }
            else if (str == "Right")
            {
                Rtmp = i % RRepeatdegree.Length;
                SendData(RRepeatdegree[Rtmp]+" "+Lnow);
            }
            else if (str == "Left")
            {
                Ltmp = i % LRepeatdegree.Length;
                SendData(Rnow+" "+LRepeatdegree[Ltmp]);
            }
            Thread.Sleep(1000);
        }
    }
}