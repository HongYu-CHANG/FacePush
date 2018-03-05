using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.IO;
using System.IO.Ports;
using System.Threading;

public class trackerPosRecord_v2 : MonoBehaviour {

	public int frameSegment;//num of frames between 2 position recording
	private int counter = 0;

	public GameObject Ltracker;
	public GameObject Rtracker;
	private Vector3 LlastPos;
	private Vector3 RlastPos;
	private Vector3 Lvector;
	private Vector3 Rvector;

	public GameObject headpos;
	public GameObject bodypos;
	private Vector3 body_head_direction;

	private bool posInitialized = false;
	public float drawRayTime;

    //motor (OSC)
    /*
	public GameObject RMotor;
	public GameObject LMotor;
	private OSCSender ROSCSender;
	private OSCSender LOSCSender;
    */

    //motor (serial port) - Arduino connection
    private CommunicateWithArduino Uno = new CommunicateWithArduino();

    //v2: 用合力計算旋轉角度 & 移動
    private Vector3 LRvector;
	private float body_vector_angle;
	private int i = 0;
    private float offset = 1;
    private bool rotated = false;


	// Use this for initialization
	void Start()
	{

		Ltracker.gameObject.SetActive(true);

        //motor control (OSC)
        /*
		ROSCSender = RMotor.GetComponent<OSCSender>();
		ROSCSender.setWhichMotor("R");
		LOSCSender = LMotor.GetComponent<OSCSender>();
		LOSCSender.setWhichMotor("L");
        */

        //motor (serial port)
        new Thread(Uno.connectToArdunio).Start();

        LlastPos = Ltracker.transform.position;
		RlastPos = Rtracker.transform.position;
		body_head_direction = headpos.transform.position - bodypos.transform.position;


	}


	void FixedUpdate()
	{

		if (Input.GetKeyDown(KeyCode.H))
		{
			Debug.Log("Press H");
			transform.position = (Ltracker.transform.position + Rtracker.transform.position) / 2;
			Debug.Log("swimmer's position: " + transform.position);
			posInitialized = true;
		}

		if (posInitialized == true)
		{

			if (counter < frameSegment - 1)
			{
				//do not get tracker position in this frame
				counter++;
			}
			else
			{
				//counter == frameSegment - 1 -> get tracker position in this frame

				Lvector = LlastPos - Ltracker.transform.position;
				Rvector = RlastPos - Rtracker.transform.position;
				body_head_direction = headpos.transform.position - bodypos.transform.position;

				Debug.DrawRay(Ltracker.transform.position, Lvector, Color.red, drawRayTime);
				Debug.DrawRay(Rtracker.transform.position, Rvector, Color.red, drawRayTime);
				Debug.DrawRay(bodypos.transform.position, body_head_direction * 10, Color.red, drawRayTime);

				LRvector = Lvector + Rvector;
				body_vector_angle = Vector3.Angle(new Vector3(body_head_direction.x, 0, body_head_direction.z), new Vector3(LRvector.x, 0, LRvector.z));

                //rotation
                if (Rvector.magnitude > 0.05f || Lvector.magnitude > 0.05f)
				{
					if(Mathf.Abs(Rvector.magnitude - Lvector.magnitude) > 0.025f && LRvector.x > 0)
					{
						if (Rvector.magnitude < Lvector.magnitude)
						{
                            // L > R: turn right
                            transform.Rotate(Vector3.up * body_vector_angle * 0.08f);
                            if (rotated == false && fish_control.fish == 0 && shark_control.shark == 0)
                            {
                                //StartCoroutine(No1Work(true, false, 0, 0)); //R,L,angle,speed (OSC)
                                //new Thread(Uno.SendData).Start("20 150 120 150"); // L Lspeed R Rspeed
                                Debug.Log("turn right, L motor");
                            }

                        }
						else
						{
                            // R > L: turn left
                            transform.Rotate(Vector3.down * body_vector_angle * 0.08f);
                            if (rotated == false && fish_control.fish == 0 && shark_control.shark == 0)
                            {
                                StartCoroutine(No1Work(false, true, 0, 0));
                                Debug.Log("turn left, R motor");
                            }

                        }

                        rotated = true;
					}
					else
					{
						Debug.Log("don't rotate");
						rotated = false;
					}

				}

                //offset control
				if (LRvector.magnitude > 0.01f && LRvector.magnitude < 0.02f) offset += 0.1f;
				else if (LRvector.magnitude > 0.02f && LRvector.magnitude < 0.03f) offset += 0.2f;
				else if (LRvector.magnitude > 0.03f && LRvector.magnitude < 0.04f) offset += 0.3f;
				else if (LRvector.magnitude > 0.04f && LRvector.magnitude < 0.05f) offset += 0.4f;
				else if (LRvector.magnitude > 0.05f) offset += 0.5f;
				
                //swim forward
				if(rotated)
					transform.position = transform.position + new Vector3(body_head_direction.x, 0, body_head_direction.z) * (LRvector.magnitude + offset) * 0.0005f ;
                else
                {
                    transform.position = transform.position + new Vector3(body_head_direction.x, 0, body_head_direction.z) * (LRvector.magnitude + offset) * 0.2f;
					Debug.Log("---for go straight---");

                    //fish & shark & motor control
                    if (fish_control.fish == 1) Debug.Log("fish!!");
                    else if (shark_control.shark == 1) Debug.Log("shark!!");
                    else if((int)(LRvector.magnitude + offset) >= 15)
                    {
                        StartCoroutine(No1Work(false, false, 150, 100));
                        Debug.Log("move forward, max");
                    }
					else if ((int)(LRvector.magnitude + offset) > 1)
					{
						StartCoroutine(No1Work(false, false, (int)(LRvector.magnitude + offset) * 10, 100));
						Debug.Log("move forward, default");
					}
                    else
                    {
                        //放鬆
                        StartCoroutine(No1Work(false, false, 10, 100));
                        Debug.Log("move forward, free");
                    }
                    
                }

                //reset
                LlastPos = Ltracker.transform.position;
				RlastPos = Rtracker.transform.position;
				Lvector = Vector3.zero;
				Rvector = Vector3.zero;
				counter = 0;
				if (offset > 0.2f)  offset -= 0.2f;
				else  offset = 0;
				rotated = false;
			}

		}

	}

    //motor control (OSC)
    /*
    IEnumerator No1Work(bool R, bool L, int angle, int speed)
    {
        float waitingTime = 1f;
        int rotateSpeed = 150;

        if (R)//右轉，要動右馬達 (1,0)
        {
            ROSCSender.SendOSCMessageTriggerMethod(120, rotateSpeed);//加壓   // (角度0~180, 速度0~255)
            LOSCSender.SendOSCMessageTriggerMethod(20, rotateSpeed);
            yield return new WaitForSeconds(waitingTime);
            ROSCSender.SendOSCMessageTriggerMethod(10, rotateSpeed);//加壓   // (角度0~180, 速度0~255)
            LOSCSender.SendOSCMessageTriggerMethod(10, rotateSpeed);
        }
        else if (L)//左轉，要動左馬達 (0,1)
        {
            ROSCSender.SendOSCMessageTriggerMethod(20, rotateSpeed);//加壓   // (角度0~180, 速度0~255)
            LOSCSender.SendOSCMessageTriggerMethod(150, rotateSpeed);
            yield return new WaitForSeconds(waitingTime);
            ROSCSender.SendOSCMessageTriggerMethod(10, rotateSpeed);//加壓   // (角度0~180, 速度0~255)
            LOSCSender.SendOSCMessageTriggerMethod(10, rotateSpeed);
        }
        else //前進，兩馬達都要動 (0,0)
        {
            ROSCSender.SendOSCMessageTriggerMethod(angle, speed);
            LOSCSender.SendOSCMessageTriggerMethod(angle + 20, speed);
        }
    }
    */

    IEnumerator No1Work(bool R, bool L, int angle, int speed)
    {
        float waitingTime = 1f;

        if (R)//右轉，要動右馬達 (1,0)
        {
            new Thread(Uno.SendData).Start("20 150 120 150"); // L Lspeed R Rspeed
            yield return new WaitForSeconds(waitingTime);
            new Thread(Uno.SendData).Start("10 150 10 150"); // L Lspeed R Rspeed
        }
        else if (L)//左轉，要動左馬達 (0,1)
        {
            new Thread(Uno.SendData).Start("150 150 20 150"); // L Lspeed R Rspeed
            yield return new WaitForSeconds(waitingTime);
            new Thread(Uno.SendData).Start("10 150 10 150"); // L Lspeed R Rspeed
        }
        else //前進，兩馬達都要動 (0,0)
        {
            new Thread(Uno.SendData).Start((angle+20)+" "+speed+" "+angle+" "+speed); // L Lspeed R Rspeed
        }
    }



    // motor control for serial port

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
