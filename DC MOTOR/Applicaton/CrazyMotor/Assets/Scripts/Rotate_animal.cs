using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.IO.Ports;
using System.Threading;
using UnityEngine.Video;

public class Rotate_animal : MonoBehaviour {
	public GameObject cube;             //target cube (put at fixed position)
	private bool start = true;         //flag: after initialization, set start to 1
	private float initRotation = 0f;    //initial ratation angle
	private float currentRotation = 0f; //curent rotation angle
	private int motor_angle = 0;        //the rotation angle the motors need to rotate
	private int counter = 3;            //every ? frame, send motor's rotation angle to motors
	private bool R = true;              //right motor rotates

	private float cube_angle = 90f;     //the angle user has to rotate (cube's position) (default: at 90 degree)
	public float target_angle = 90f;
	//testing
	/*
	public GameObject cube2;
	private float cube_angle = 180f;     //the angle user has to rotate (cube's position) (default: at 90 degree)
	*/

	//motor (serial port): Arduino connection
	private CommunicateWithArduino Uno = new CommunicateWithArduino();

	public VideoPlayer videoPlayer;

	private GameObject hit;
	private Color color;

	private int done = 0;
	private int type = 1;

	// Use this for initialization
	void Start()
	{

		//motor (serial port)
		new Thread(Uno.connectToArdunio).Start();
		hit = GameObject.FindGameObjectWithTag("Hit");
		color = hit.GetComponent<Renderer>().material.color;

	}

	// Update is called once per frame
	void Update()
	{

		//Initialization
		if (Input.GetKeyDown(KeyCode.S))
		{
			initRotation = this.transform.rotation.eulerAngles.y;
			//if (initRotation > 270) cube_angle = 90 - (initRotation - 360);
			//else cube_angle = 90 - initRotation;

			if (initRotation > target_angle + 180f) cube_angle = target_angle - (initRotation - 360);
			else cube_angle = target_angle - initRotation;
			start = true;
			Debug.Log("Start");
			Debug.Log(initRotation);
		}

		int videoTime = Mathf.FloorToInt((float)videoPlayer.time);
		if (videoTime == 11)
		{
			if (done == 0) initRotation = this.transform.rotation.eulerAngles.y;
			done = 1; type = 2;
			target_angle = 100f;
			if (initRotation > target_angle + 180f) cube_angle = target_angle - (initRotation - 360);
			else cube_angle = target_angle - initRotation;
		}
		else if (videoTime == 45)
		{
			if (done == 0) initRotation = this.transform.rotation.eulerAngles.y;
			done = 1; type = 2;
			target_angle = 70f;
			if (initRotation > target_angle + 180f) cube_angle = target_angle - (initRotation - 360);
			else cube_angle = target_angle - initRotation;
		}
		else if (videoTime == 60)
		{
			if (done == 0) initRotation = this.transform.rotation.eulerAngles.y;
			done = 1; type = 2;
			target_angle = 40f;
			if (initRotation > target_angle + 180f) cube_angle = target_angle - (initRotation - 360);
			else cube_angle = target_angle - initRotation;
		}
		else if (videoTime == 105)//100
		{
			if (done == 0) initRotation = this.transform.rotation.eulerAngles.y;
			done = 1; type = 2;
			target_angle = 25f;
			if (initRotation > target_angle + 180f) cube_angle = target_angle - (initRotation - 360);
			else cube_angle = target_angle - initRotation;
		}
		else if (videoTime == 120)//120
		{
			if (done == 0) initRotation = this.transform.rotation.eulerAngles.y;
			done = 1; type = 2;
			target_angle = -0f;//-21
			if (initRotation > target_angle + 180f) cube_angle = target_angle - (initRotation - 360);
			else cube_angle = target_angle - initRotation;
		}
		else if (videoTime == 140)//120
		{
			if (done == 0) initRotation = this.transform.rotation.eulerAngles.y;
			done = 1; type = 2;
			target_angle = -21f;
			if (initRotation > target_angle + 180f) cube_angle = target_angle - (initRotation - 360);
			else cube_angle = target_angle - initRotation;
		}
		/*else if (videoTime == 140)
		{
			if (done == 0) initRotation = this.transform.rotation.eulerAngles.y;
			done = 1; type = 2;
			target_angle = -40f;
			if (initRotation > target_angle + 180f) cube_angle = target_angle - (initRotation - 360);
			else cube_angle = target_angle - initRotation;
		}*/
		else done = 0;


		if (Input.GetKeyDown(KeyCode.Z)) Debug.Log(videoTime);

		//Rotation angle control and calculation
		if (start == true)
		{
			currentRotation = this.transform.rotation.eulerAngles.y - initRotation;

			if (type == 2) if (this.transform.rotation.eulerAngles.y > 180) currentRotation -= 360; // 20


			//currentRotation = Mathf.Min(this.transform.rotation.eulerAngles.y - initRotation, (this.transform.rotation.eulerAngles.y - 360) - initRotation);

			if (type == 1) if (currentRotation < 0) currentRotation = currentRotation + 360;
			//make the degree between 0 ~ 180
			//if (degree > 180) -> set it to fixed degree -> give biggest pressure)
			if (type == 1)
				if (currentRotation > (360 - (cube_angle * 2)) / 2 + cube_angle * 2) currentRotation = 0; //270~360, 4th quadrant
				else if (currentRotation > cube_angle * 2 && currentRotation < (360 - (cube_angle * 2)) / 2 + cube_angle * 2) currentRotation = cube_angle * 2; //180~270, 3rd quadrant
			
			
			float edge1 = (target_angle + cube_angle + 360) % 360;
			float edge2 = (initRotation - (360 - Mathf.Abs(cube_angle * 2))/2 + 360) % 360;
			if(type == 2)
			{
				if(cube_angle < 0 && currentRotation < cube_angle * 2 ) currentRotation = cube_angle * 2;
				if(cube_angle < 0 && currentRotation > 0) currentRotation = 0;

				if (cube_angle > 0 && currentRotation > cube_angle * 2) currentRotation = cube_angle * 2;
				if (cube_angle > 0 && currentRotation < 0) currentRotation = 0;
			}
			
			

			if (cube_angle - currentRotation > 2) //未轉到, 1st & 4th quadrant
			{
				int l;
				if (Mathf.Abs(cube_angle) < 50) l = 0;
				else l = 0;
				motor_angle = (int)((int)((cube_angle - currentRotation) / 5) * 5 * (150 / (Mathf.Abs(cube_angle) + l)) );
				//motor_angle = (int)((int)((cube_angle - currentRotation) / 5) * 5 * 1.5f);
				if (motor_angle > 100) motor_angle = 100;
				R = true;

				color.a = (float)motor_angle / 150;
				hit.GetComponent<Renderer>().material.color = color;
				hit.transform.localPosition = new Vector3(-2.73f, -0.88f, 3.9f);
			}
			else if (cube_angle - currentRotation < -2) //轉過頭, 2nd & 3rd quadrant
			{
				int l;
				if (Mathf.Abs(cube_angle) < 50) l = 0;
				else l = 0;
				motor_angle = (int)((int)((currentRotation - cube_angle) / 5) * 5 * (150 / (Mathf.Abs(cube_angle) + l)) );
				//motor_angle = (int)((int)((currentRotation - cube_angle) / 5) * 5 * 1.5f);
				if (motor_angle > 100) motor_angle = 100;
				R = false; 

				color.a = (float)motor_angle / 150;
				hit.GetComponent<Renderer>().material.color = color;
				hit.transform.localPosition = new Vector3(-2.73f, -0.88f, 3.9f);
			}

		}

		//Print angles
		if (Input.GetKeyDown(KeyCode.A))
		{
			Debug.Log("currentRotation: " + currentRotation);
			Debug.Log("motor_angle: " + motor_angle);
			//Debug.Log("currentR :" + (this.transform.rotation.eulerAngles.y - initRotation));
			Debug.Log("this.transform.rotation.eulerAngles.y :" + (this.transform.rotation.eulerAngles.y ));

			if (R == true) Debug.Log("R");
			else Debug.Log("L");
		}

		//Motor control: every ? frame -> send data to arduino
		if (counter == 0)
		{
			StartCoroutine(No1Work(R, motor_angle));
			counter = 3;
		}
		counter--;

	}



	//motor function
	IEnumerator No1Work(bool R, int angle)
	{
		float waitingTime = 0.01f;

		if (R)//右轉，要動右馬達 (1,0)
		{
			new Thread(Uno.SendData).Start("10 150 " + angle + " 150"); // L Lspeed R Rspeed
			yield return new WaitForSeconds(waitingTime);
		}
		else //左轉，要動左馬達 (0,1)
		{
			new Thread(Uno.SendData).Start(angle + " 150 10 150"); // L Lspeed R Rspeed
			yield return new WaitForSeconds(waitingTime);
		}

	}

	//angle mapping
	private int degreeConvertToLeftRotaryCoder(int degree)
	{
		// alternation
		// increase another converter for right motor
		return (degree * 1024 / 360);
	}
	private int degreeConvertToRightRotaryCoder(int degree)
	{
		// alternation
		// increase another converter for right motor
		return (degree * 682 / 360);
	}

	// motor control for serial port
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
							if (dev.StartsWith("/dev/tty."))
							{
								serial_ports.Add(dev);
								Debug.Log(String.Format(dev));
							}
						}
					}
					portChoice = "/dev/" + choice;
				}
				arduinoController = new SerialPort(portChoice, 57600, Parity.None, 8, StopBits.One);
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
					Debug.Log(arduinoController);
					Debug.Log("nullport");
				}
			}
			else
			{
				Debug.Log("not connected");
			}
			Thread.Sleep(500);
		}
	}

}
