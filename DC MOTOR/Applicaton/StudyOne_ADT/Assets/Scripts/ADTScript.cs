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
	public String  userName ="User1";
	private StreamWriter fileWriter;

	//motor parament
	public int fraction = 20;
	public int[] fractionArray = new int[24];
	private int motorSpeed = 200;
	private int initialDegree = 90;
	//private int LinitialDegree = 90;
	private CommunicateWithArduino Uno = new CommunicateWithArduino();

	//timer
	public Text timerTextDisplay;
	private TimeSpan prevTaskTime = DateTime.Now.TimeOfDay;
	private bool timerStart = false;
	private bool stimTimerStart = false;
	private float remainingTime = 0;
	private bool stimSent = true;
	private int stimTime = 4;
	private int stimDelay = 2;

	// Use this for initialization
	void Start () 
	{
		//Uno = new CommunicateWithArduino();
        new Thread (Uno.connectToArdunio).Start ();
		fileWriter = new StreamWriter( "Results/"+"AnsOf" + userName+".csv", true);
		//coverImage.enabled=false;
		finishedText.enabled = false;
		finishedImage.enabled = false;
		//demoText.enabled = false;
		Debug.Log("start");
		writeFile(userName + ": "+ DateTime.Now.TimeOfDay.ToString()+"\n");
		fractionArray[0] = fraction;
		for(int i = 1 ; i < 24 ; i++ )
		{
			if(i % 8 == 0)fraction /= 2;

			fractionArray[i] = fraction;
		}
	}

	// Update is called once per frame
	void Update () 
	{
		if (timerStart) //倒數計時
		{
            remainingTime  -= (Time.fixedDeltaTime*(float)0.6);
			updateTimerText ((int)remainingTime);
			if (remainingTime < 1) {
				remainingTime = 0;
				timerStart = false;
				startButton.interactable = true;
			}
		}
		if (stimTimerStart) //感受刺激
		{
            int stimremainingTime = stimTime - Mathf.Abs((DateTime.Now.TimeOfDay.Seconds) - prevTaskTime.Seconds);           
            if (!stimSent) {             
                if (Mathf.Abs (stimremainingTime - stimDelay) < 1) {                   
					new Thread (Uno.SendData).Start(degreeConvertToRotaryCoder(20)+" "+motorSpeed+" "+degreeConvertToRotaryCoder(100)+" "+motorSpeed);                   
                    stimSent = true;
                }
            }
			if (stimremainingTime < 1) {
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
		Debug.Log("startButton");
		coverImage.enabled=true;
		stimTimerStart = true;
		prevTaskTime = DateTime.Now.TimeOfDay;
		stimSent = false;

		confirmButton.interactable = true;
		startButton.interactable = true;
	}

	public void confirmButtonClick()
	{
		//Debug.Log("confirmButton");
		writeData ();
		timerStart=true;
		confirmButton.interactable = false;
		startButton.interactable = false;
        remainingTime = 15;
    }
    
    private void writeData()
    {
		//Debug.Log (wetSlider.value);
		Debug.Log ("writeData");
		//Debug.Log (comfortSlider.value.ToString());

//		writeFile (","+task[randomizedOrder[taskNum]].ToString()+","+command+","+frequency+","+wetSlider.value.ToString()+","+comfortSlider.value.ToString()+"\n");
		//writeFile ("\n");
	}

	private void writeFile(String data){
		
		fileWriter.Write (data);
		fileWriter.Flush();
		fileWriter.Close();
		fileWriter.Dispose();
	}

	private int degreeConvertToRotaryCoder(int degree)
    {
        return (degree * 1024/360);
    }

	class CommunicateWithArduino
    {

        public bool connected = true;
        public bool mac = true;
        public string choice = "cu.usbmodem1421";
        private SerialPort arduinoController;
        
        public void connectToArdunio()
        {
            
            if (connected)
            {
				string portChoice = "COM2";
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
