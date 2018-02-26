using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
//using UnityEngine;

public class ADTScript : MonoBehaviour {

	
	//canvas components
	public Button startButton;
	public Button confirmButton;
	public Image finishedImage;
	public Image coverImage;
	public Text finishedText;

	//user record
	public Toggle yesToggle;
	public Toggle noToggle;
	public String  userName ="User1";
	public int time = 1;
	private StreamWriter fileWriter;

	//motor parament
	public int fraction = 20;
	public int[] fractionArray = new int[24];
	private int motorSpeed = 255;
	private int initialDegree = 150;
	//private int LinitialDegree = 90;
	private int nowDegree = 0;
	private int yesCounter = 0;
	private int nowCounter = -1;
	private CommunicateWithArduino Uno = new CommunicateWithArduino();

	//timer
	public Text timerTextDisplay;
	private TimeSpan prevTaskTime = DateTime.Now.TimeOfDay;
	private bool timerStart = false;
	private bool stimTimerStart = false;
	private float remainingTime = 0;
	private bool stimSent = true;
	private int stimTime = 6;
	private int stimDelay = 2;

	// Use this for initialization
	void Start () 
	{
		Debug.Log(userName + " Study Start ...\n");
		nowDegree = initialDegree;
		new Thread (Uno.connectToArdunio).Start ();
		finishedText.enabled = false;
		finishedImage.enabled = false;
		fractionArray[0] = fraction;
		for(int i = 1 ; i < 24 ; i++ )
		{
			if(i % 8 == 0)fraction /= 2;

			fractionArray[i] = fraction;
		}
		writeFile("Tester Name, Time, Counter, Degree, yesToggle Value, noToggle Value\n");
	}

	// Update is called once per frame
	void Update () 
	{
		if (timerStart) //倒數計時
		{
            remainingTime  -= (Time.fixedDeltaTime*(float)0.6);
			updateTimerText ((int)remainingTime);
			if (remainingTime < 1) 
			{
				remainingTime = 0;
				timerStart = false;
				startButton.interactable = true;
				if(nowCounter >= 23) // Strudy Finish!
		        {
		        	finishedText.enabled = true;
					finishedImage.enabled = true;
		        }
			}
		}
		if (stimTimerStart) //感受刺激
		{
            int stimremainingTime = stimTime - Mathf.Abs((DateTime.Now.TimeOfDay.Seconds) - prevTaskTime.Seconds);           
            if (!stimSent) {             
                if (Mathf.Abs (stimremainingTime - stimDelay) < 2) {                   
					new Thread (Uno.SendData).Start(LdegreeConvertToRotaryCoder(nowDegree)+" "+motorSpeed+" "+RdegreeConvertToRotaryCoder(nowDegree)+" "+motorSpeed);                   
                    stimSent = true;
                }
            }
			if (stimremainingTime < 1) {
				new Thread(Uno.SendData).Start(LdegreeConvertToRotaryCoder(0) + " " + motorSpeed + " " + RdegreeConvertToRotaryCoder(0) + " " + motorSpeed);
				stimTimerStart = false;
                coverImage.enabled = false;
			}
        }
        
	}

	private void updateTimerText(int time)
	{
		timerTextDisplay.text = "Time: " + time.ToString()+"s";
	}

	public void starttButtonClick()
	{
		nowCounter ++;
		Debug.Log ("第" + (nowCounter + 1) + "次測試...開始\n");
		Debug.Log ("目前角度為 : " + nowDegree + "\n");
		coverImage.enabled=true;
		stimTimerStart = true;
		prevTaskTime = DateTime.Now.TimeOfDay;
		stimSent = false;
		confirmButton.interactable = true;
		startButton.interactable = false;
	}

	public void confirmButtonClick()
	{
		writeData ();
		if(yesToggle.isOn)
		{
			yesCounter++;
		}
		else if(noToggle.isOn)
		{
			yesCounter = 0;

		}
		changeTheDegree();
		timerStart=true;
		confirmButton.interactable = false;
		startButton.interactable = false;
        remainingTime = 5;

    }
    private void changeTheDegree()
    {
    	if(yesCounter == 2)
    	{
    		nowDegree -= fractionArray[nowCounter];
    		yesCounter = 0;
    	}
    	else if (yesCounter == 0)
    		nowDegree += fractionArray[nowCounter];
    }
    private void writeData()
    {
		writeFile (userName+","+System.DateTime.Now.ToString()+","+ (nowCounter + 1) +","+nowDegree+
			","+yesToggle.isOn.ToString()+","+noToggle.isOn.ToString()+"\n");
		Debug.Log ("Data Write... Finish\n");
		Debug.Log ("第" + (nowCounter + 1) + "次測試...完成\n");
	}

	private void writeFile(String data)
	{
		fileWriter = new StreamWriter( "ADT_Results\\"+"ADT Answer Of "+ userName +"_"+ fraction+"_" + time +".csv", true);
		fileWriter.Write (data);
		fileWriter.Flush();
		fileWriter.Close();
		fileWriter.Dispose();
	}

	private int LdegreeConvertToRotaryCoder(int degree)
    {
        return ((degree * 1024 / 360)+15);
    }
	private int RdegreeConvertToRotaryCoder(int degree)
	{
		return ((degree * 1024 / 360)+15);
	}

	class CommunicateWithArduino
    {

        public bool connected = true;
        public bool mac = false;
        public string choice = "cu.usbmodem1421";
        private SerialPort arduinoController;
        
        public void connectToArdunio()
        {
            
            if (connected)
            {
				string portChoice = "COM8";
                if (mac)
                {
                    int p = (int)Environment.OSVersion.Platform;
                    // Are we on Unix?
                    if (p == 4 || p == 128 || p == 6)
                    {
                        List<string> serial_ports = new List<string>();
                        string[] ttys = Directory.GetFiles("/dev/", "cu.*");
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
				arduinoController =new SerialPort(portChoice, 57600, Parity.None, 8, StopBits.One);
                arduinoController.Handshake = Handshake.None;
                arduinoController.RtsEnable = true;
                arduinoController.Open();
				Debug.LogWarning(arduinoController);
            }
        
        }
        public void SendData(object obj)
        {
            string data = obj as string;
            //Debug.Log(data);
            if (connected)
            {
                if (arduinoController != null)
                {
                    arduinoController.Write(data);
                    arduinoController.Write("\n");
                }
                else
                {
					Debug.Log (arduinoController);
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
