using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using SMarto;

public class VideoContoller : MonoBehaviour {

	public GameObject Video;
	public VideoPlayer videoPlayer;

	private CommunicateWithArduino Uno;
    private MotorControl Rmotor;
    private MotorControl Lmotor;
	private bool In = false;
	private int[] RRepeatdegree;//11;
	private int[] LRepeatdegree;//11
	
	// Use this for initialization
	void Start () 
	{
    	Debug.Log("Start play Video");
		Uno = new CommunicateWithArduino();
        new Thread (Uno.connectToArdunio).Start ();
		Rmotor = new MotorControl();
        Rmotor.initial(0, 75, true);
        Lmotor = new MotorControl();
        Lmotor.initial(150, 75, false);
	}
	
	// Update is called once per frame
	void Update () 
	{
		
		int videoTime = Mathf.FloorToInt((float)videoPlayer.time);
		Debug.LogWarning("Video Time: " + videoTime);
		
		if(videoTime == 6 && !In)
		{
            
            int[] RRepeatdegree = {30, 30, 0};//11
			int[] LRepeatdegree = {110, 100, 150};//11
			setRRepeatdegree(RRepeatdegree, LRepeatdegree);
            Debug.LogWarning("Video Time: " + videoTime);
			new Thread (this.repeat).Start("3");
            In = true;
		}
		else if(videoTime == 10  && In)
		{
            int[] RRepeatdegree = {30, 40, 50, 50, 50, 60, 70, 30};//11
            int[] LRepeatdegree = {140, 130, 120, 120, 120, 110, 100, 120};//11
            setRRepeatdegree(RRepeatdegree, LRepeatdegree);
            Debug.LogWarning("Video Time: " + videoTime);
            new Thread (this.repeat).Start("8");
            In = false;
		}
        
		else if(videoTime == 19 && !In)
		{
            
            int[] RRepeatdegree = {40, 50, 60, 70};//11
			int[] LRepeatdegree = {110, 100, 90, 80};//11
			setRRepeatdegree(RRepeatdegree, LRepeatdegree);
            Debug.LogWarning("Video Time: " + videoTime);
			new Thread (this.repeat).Start("4");
            In = true;
		}
        
		else if(videoTime == 27 && In)
		{
            Debug.LogWarning("Video Time: " + videoTime);
            new Thread (Uno.SendData).Start(Rmotor.moveTo(75)+" "+Lmotor.moveTo(75));
            In = false;
            //Uno.SendData(Rmotor.moveTo(75)+" "+Lmotor.moveTo(75));
		}
	}

	void repeat(object obj)
	{
		string str = obj as string;
		int Rtmp = 0;			
		int Ltmp = 0;
		for(int i = 0 ; i < int.Parse(str) ; i++)
		{
			Ltmp = i % this.LRepeatdegree.Length;
			Rtmp = i % this.RRepeatdegree.Length;
            //Debug.LogWarning(Rmotor.repeatMotor(RRepeatdegree, Rtmp)+" "+Lmotor.repeatMotor(LRepeatdegree, Ltmp));
            //new Thread (Uno.SendData).Start();
            Uno.SendData(Rmotor.repeatMotor(RRepeatdegree, Rtmp)+" "+Lmotor.repeatMotor(LRepeatdegree, Ltmp));
		}

	}
	void setRRepeatdegree(int[] RRepeatdegree, int[] LRepeatdegree)
    {
        this.RRepeatdegree = RRepeatdegree;
		this.LRepeatdegree = LRepeatdegree;
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
                    portChoice = "/dev/" + choice;
                }
                arduinoController =new SerialPort(portChoice, 115200, Parity.None, 8, StopBits.One);
                arduinoController.Handshake = Handshake.None;
                arduinoController.RtsEnable = true;
                arduinoController.Open();
            }
        
        }
        public void SendData(object obj)
        {
            string data = obj as string;
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
            Thread.Sleep(500);
        }
    }
}

