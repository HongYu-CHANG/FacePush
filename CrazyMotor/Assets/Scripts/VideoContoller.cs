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
		
		if(videoTime >= 10 && videoTime <= 13 && !In)
		{
            
            int[] RRepeatdegree = {30};//11
			int[] LRepeatdegree = {110, 100, 90};//11
			setRRepeatdegree(RRepeatdegree, LRepeatdegree);
            Debug.LogWarning("Video Time: " + videoTime);
			new Thread (this.repeat).Start("3");
            In = true;
		}
		else if(videoTime >= 15 && videoTime <= 15 && In)
		{
            //Uno.SendData(Rmotor.reset()+" "+Lmotor.reset());
            Debug.LogWarning("Video Time: " + videoTime);
            new Thread (Uno.SendData).Start(Rmotor.reset()+" "+Lmotor.reset());
            In = false;
		}
        
		else if(videoTime >= 16 && videoTime <= 17 && !In)
		{
            
            int[] RRepeatdegree = {40, 50, 60};//11
			int[] LRepeatdegree = {120};//11
			setRRepeatdegree(RRepeatdegree, LRepeatdegree);
            Debug.LogWarning("Video Time: " + videoTime);
			new Thread (this.repeat).Start("3");
            In = true;
		}
        
		else if(videoTime >= 24 && videoTime <= 28 && In)
		{
            
            int[] RRepeatdegree = {10, 20, 30};//11
			int[] LRepeatdegree = {140, 130, 120};//11
			setRRepeatdegree(RRepeatdegree, LRepeatdegree);
            Debug.LogWarning("Video Time: " + videoTime);
			new Thread (this.repeat).Start("3");
            In = false;
		}
		else if(videoTime == 32 && !In)
		{
            Debug.LogWarning("Video Time: " + videoTime);
            new Thread (Uno.SendData).Start(Rmotor.moveTo(75)+" "+Lmotor.moveTo(75));
            In = true;
            //Uno.SendData(Rmotor.moveTo(75)+" "+Lmotor.moveTo(75));
		}
	}

	void repeat(object obj)
	{
		string str = obj as string;
		int Rtmp = 0;			
		int Ltmp = 0;
		for(int i = 0 ; i <= int.Parse(str) ; i++)
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
            Thread.Sleep(1000);
        }
    }
}

