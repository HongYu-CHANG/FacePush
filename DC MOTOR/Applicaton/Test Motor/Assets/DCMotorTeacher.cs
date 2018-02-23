using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class DCMotorTeacher : MonoBehaviour {

	public int MotorSpeed;
	public GameObject RMotor;
    public GameObject LMotor;
    public Slider Lmotor_Speed;
    public Slider Lmotor_Time;
    public Slider Rmotor_Speed;
    public Slider Rmotor_Time;
    public Text LTextSpeed;
    public Text LTextTime;
    public Text RTextSpeed;
    public Text RTextTime;
    private int LSpeed;
    private int RSpeed;
    private OSCSender ROSCSender;
    private OSCSender LOSCSender;
    private bool Rbutton = false;
    private bool Lbutton = false;

    private CommunicateWithArduino Uno;

	// Use this for initialization
	void Start () 
	{
		Uno = new CommunicateWithArduino();
        new Thread (Uno.connectToArdunio).Start ();

		ROSCSender = RMotor.GetComponent<OSCSender>();
    	ROSCSender.setWhichMotor("R");
    	LOSCSender = LMotor.GetComponent<OSCSender>();
    	LOSCSender.setWhichMotor("L");
	}
	
	// Update is called once per frame
	void Update () 
	{
		RSpeed = (int)Rmotor_Speed.value;
		LSpeed = (int)Lmotor_Speed.value;
		Lmotor_Time.maxValue = 1400 / Lmotor_Speed.value;
		Rmotor_Time.maxValue = 1400 / Rmotor_Speed.value;
		RTextSpeed.text = Rmotor_Speed.value + " / " + NumberToRPM (Rmotor_Speed.value).ToString("0.00") + " RPM";
		RTextTime.text = Rmotor_Time.value.ToString("0.00") + "S";
		LTextSpeed.text = Lmotor_Speed.value + " / " + NumberToRPM (Lmotor_Speed.value).ToString("0.00") + " RPM";
		LTextTime.text = Lmotor_Time.value.ToString("0.00") + "S";
	}

	private double NumberToRPM (float sliderValue)
	{
		return sliderValue*MotorSpeed/255;
	}

	public void No1R()
	{

        Rbutton = !Rbutton;
        StartCoroutine(No1Work(true, false, Rbutton));
	}

	public void No1L()
	{
		Rbutton = !Rbutton;
        StartCoroutine(No1Work(false, true, Rbutton));
	}

	IEnumerator No1Work(bool R, bool L, bool click)
	{
		float time;
		if(R)
			time = Rmotor_Time.value;
		else
			time = Lmotor_Time.value;
		if(click)//奇數次點擊
		{
			new Thread (Uno.SendData).Start(degreeConvertToRotaryCoder(100)+" "+RSpeed+" "+degreeConvertToRotaryCoder(100)+" "+LSpeed);
	        yield return new WaitForSeconds(time);
	        Debug.Log("Time's up!");
    	}
    	else
    	{
    		new Thread (Uno.SendData).Start(degreeConvertToRotaryCoder(20)+" "+RSpeed+" "+degreeConvertToRotaryCoder(100)+" "+LSpeed);
	        yield return new WaitForSeconds(time);
	        Debug.Log("Time's up!");
    	}

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
