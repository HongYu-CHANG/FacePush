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

    //motor (serial port) - Arduino connection
    private CommunicateWithArduino Uno = new CommunicateWithArduino();

    //v2: 用合力計算旋轉角度 & 移動
    private Vector3 LRvector;
	private float body_vector_angle;
	private int i = 0;
    private float offset = 1;
    private bool rotated = false;

	//face
	private GameObject hit;
	private Color color;
	private GameObject hit_r;
	private Color color_r;

	private int done = 0;
	private int fish_done = 0;
	private int shark_done = 0;

	// Use this for initialization
	void Start()
	{

		Ltracker.gameObject.SetActive(true);

        //motor (serial port)
        new Thread(Uno.connectToArdunio).Start();

        LlastPos = Ltracker.transform.position;
		RlastPos = Rtracker.transform.position;
		body_head_direction = headpos.transform.position - bodypos.transform.position;

		//face
		hit = GameObject.FindGameObjectWithTag("Hit");
		color = hit.GetComponent<Renderer>().material.color;
		hit_r = GameObject.FindGameObjectWithTag("Hit_R");
		color_r = hit_r.GetComponent<Renderer>().material.color;
	}


	void FixedUpdate()
	{

		if (Input.GetKeyDown(KeyCode.H) && done == 0)
		{
			Debug.Log("Press H");
			transform.position = new Vector3((Ltracker.transform.position.x + Rtracker.transform.position.x) / 2,0.47f, (Ltracker.transform.position.z + Rtracker.transform.position.z) / 2);
			Debug.Log("swimmer's position: " + transform.position);
			posInitialized = true;
			done = 1;
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
                                StartCoroutine(No1Work(true, false, 0, 0)); //R,L,angle,speed (OSC)
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
                        StartCoroutine(No1Work(false, false, 150, 255));
                        Debug.Log("move forward, max");
                    }
					else if ((int)(LRvector.magnitude + offset) > 1)
					{
						StartCoroutine(No1Work(false, false, (int)(LRvector.magnitude + offset) * 10, 255));
						Debug.Log("move forward, default");
					}
                    else
                    {
                        //放鬆
                        StartCoroutine(No1Work(false, false, 0, 255));
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

			if (fish_control.fish == 1 && fish_done == 0)
			{
				fish_done = 1;
				StartCoroutine(No1Work());
				
			}

			if (shark_control.shark == 1 && shark_done == 0)
			{
				shark_done = 1;
				if(shark_control.r == 0) StartCoroutine(No1Work_Shark(true, false));
				else if (shark_control.r == 1) StartCoroutine(No1Work_Shark(false, true));


			}
		}

	}


    IEnumerator No1Work(bool R, bool L, int angle, int speed)
    {
        float waitingTime = 1f;
		const int turningSpeed = 255; //constant
		

        if (R)//右轉，要動右馬達 (1,0)
        {
			/*
			//original ver.
			new Thread(Uno.SendData).Start("20 150 120 150"); // L Lspeed R Rspeed
            yield return new WaitForSeconds(waitingTime);
            new Thread(Uno.SendData).Start("10 150 10 150"); // L Lspeed R Rspeed
			*/

			new Thread(Uno.SendData).Start(degreeConvertToLeftRotaryCoder(20) + " " + turningSpeed + " " + degreeConvertToRightRotaryCoder(120) + " " + turningSpeed); // L Lspeed R Rspeed																																						   //face
			//face
			color_r.a = (float)120f / 150;
			hit_r.GetComponent<Renderer>().material.color = color_r;
			color.a = (float)20f / 150;
			hit.GetComponent<Renderer>().material.color = color;

			yield return new WaitForSeconds(waitingTime);
			new Thread(Uno.SendData).Start("0 255 0 255"); // L Lspeed R Rspeed

			//face
			color_r.a = (float)0f / 150;
			hit_r.GetComponent<Renderer>().material.color = color_r;
			color.a = (float)0f / 150;
			hit.GetComponent<Renderer>().material.color = color;
		}
        else if (L)//左轉，要動左馬達 (0,1)
        {
			/*
			//original ver.
			new Thread(Uno.SendData).Start("150 150 20 150"); // L Lspeed R Rspeed
            yield return new WaitForSeconds(waitingTime);
            new Thread(Uno.SendData).Start("10 150 10 150"); // L Lspeed R Rspeed
			*/

			new Thread(Uno.SendData).Start(degreeConvertToLeftRotaryCoder(120) + " " + turningSpeed + " " + degreeConvertToRightRotaryCoder(20) + " " + turningSpeed); // L Lspeed R Rspeed
			//face
			color.a = (float)120f / 150;
			hit.GetComponent<Renderer>().material.color = color;
			color_r.a = (float)20f / 150;
			hit_r.GetComponent<Renderer>().material.color = color_r;
			yield return new WaitForSeconds(waitingTime);
			new Thread(Uno.SendData).Start("0 255 0 255"); // L Lspeed R Rspeed

			//face
			color.a = (float)0f / 150;
			hit.GetComponent<Renderer>().material.color = color;
			color_r.a = (float)0f / 150;
			hit_r.GetComponent<Renderer>().material.color = color_r;
		}
        else //前進，兩馬達都要動 (0,0)
        {
			/*
			//original ver.
			new Thread(Uno.SendData).Start((angle+20)+" "+speed+" "+angle+" "+speed); // L Lspeed R Rspeed
			*/
			
			new Thread(Uno.SendData).Start(degreeConvertToLeftRotaryCoder(angle) + " " + turningSpeed + " " + degreeConvertToRightRotaryCoder(angle) + " " + turningSpeed); // L Lspeed R Rspeed

			//face
			color.a = (float)angle / 150;
			hit.GetComponent<Renderer>().material.color = color;
			color_r.a = (float)angle / 150;
			hit_r.GetComponent<Renderer>().material.color = color_r;
		}
    }

	//fish
	private void face_color_r()
	{
		//face
		color_r.a = (float)120f / 150;
		hit_r.GetComponent<Renderer>().material.color = color_r;
		color.a = (float)20f / 150;
		hit.GetComponent<Renderer>().material.color = color;
	}

	private void face_color()
	{
		//face
		color.a = (float)120f / 150;
		hit.GetComponent<Renderer>().material.color = color;
		color_r.a = (float)20f / 150;
		hit_r.GetComponent<Renderer>().material.color = color_r;
	}

	IEnumerator No1Work()
	{
		float waitingTime = 2f;
		int speed = 255;
		float tempTime = UnityEngine.Random.Range(0.01f, 0.03f);//tempTime = 0;
		yield return new WaitForSeconds(waitingTime);

		new Thread(Uno.SendData).Start(degreeConvertToLeftRotaryCoder(170) + " " + speed + " " + degreeConvertToRightRotaryCoder(0) + " " + speed);
		yield return new WaitForSeconds(0.15f - tempTime);
		face_color();
		new Thread(Uno.SendData).Start(degreeConvertToLeftRotaryCoder(0) + " " + speed + " " + degreeConvertToRightRotaryCoder(170) + " " + speed);
		yield return new WaitForSeconds(0.15f + tempTime);
		face_color_r();
		new Thread(Uno.SendData).Start(degreeConvertToLeftRotaryCoder(170) + " " + speed + " " + degreeConvertToRightRotaryCoder(0) + " " + speed);
		yield return new WaitForSeconds(0.15f - tempTime);
		face_color();
		new Thread(Uno.SendData).Start(degreeConvertToLeftRotaryCoder(0) + " " + speed + " " + degreeConvertToRightRotaryCoder(170) + " " + speed);
		yield return new WaitForSeconds(0.15f + tempTime);
		face_color_r();
		new Thread(Uno.SendData).Start(degreeConvertToLeftRotaryCoder(170) + " " + speed + " " + degreeConvertToRightRotaryCoder(0) + " " + speed);
		yield return new WaitForSeconds(0.15f - tempTime);
		face_color();
		new Thread(Uno.SendData).Start(degreeConvertToLeftRotaryCoder(0) + " " + speed + " " + degreeConvertToRightRotaryCoder(170) + " " + speed);
		yield return new WaitForSeconds(0.15f + tempTime);
		face_color_r();
		new Thread(Uno.SendData).Start(degreeConvertToLeftRotaryCoder(170) + " " + speed + " " + degreeConvertToRightRotaryCoder(0) + " " + speed);
		yield return new WaitForSeconds(0.15f - tempTime);
		face_color();
		new Thread(Uno.SendData).Start(degreeConvertToLeftRotaryCoder(0) + " " + speed + " " + degreeConvertToRightRotaryCoder(170) + " " + speed);
		yield return new WaitForSeconds(0.15f + tempTime);
		face_color_r();
		new Thread(Uno.SendData).Start(degreeConvertToLeftRotaryCoder(170) + " " + speed + " " + degreeConvertToRightRotaryCoder(0) + " " + speed);
		yield return new WaitForSeconds(0.15f - tempTime);
		face_color();
		new Thread(Uno.SendData).Start(degreeConvertToLeftRotaryCoder(0) + " " + speed + " " + degreeConvertToRightRotaryCoder(170) + " " + speed);
		yield return new WaitForSeconds(0.15f + tempTime);
		face_color_r();
		new Thread(Uno.SendData).Start(degreeConvertToLeftRotaryCoder(170) + " " + speed + " " + degreeConvertToRightRotaryCoder(0) + " " + speed);
		yield return new WaitForSeconds(0.15f - tempTime);
		face_color();
		new Thread(Uno.SendData).Start(degreeConvertToLeftRotaryCoder(0) + " " + speed + " " + degreeConvertToRightRotaryCoder(170) + " " + speed);
		yield return new WaitForSeconds(0.15f + tempTime);
		face_color_r();
		new Thread(Uno.SendData).Start(degreeConvertToLeftRotaryCoder(170) + " " + speed + " " + degreeConvertToRightRotaryCoder(0) + " " + speed);
		yield return new WaitForSeconds(0.15f - tempTime);
		face_color();
		new Thread(Uno.SendData).Start(degreeConvertToLeftRotaryCoder(0) + " " + speed + " " + degreeConvertToRightRotaryCoder(0) + " " + speed);
		color_r.a = (float)20f / 150;
		hit_r.GetComponent<Renderer>().material.color = color_r;
		color.a = (float)20f / 150;
		hit.GetComponent<Renderer>().material.color = color;
		fish_done = 0;
		fish_control.fish = 0;
	}

	IEnumerator No1Work_Shark(bool R, bool L)
	{
		float waitingTime = 1f;
		int rotateSpeed = 255;


		if (R)//右轉，要動右馬達 (1,0)
		{
			//new Thread(Uno.SendData).Start("20 150 120 150"); // L Lspeed R Rspeed
			new Thread(Uno.SendData).Start(degreeConvertToLeftRotaryCoder(20) + " " + rotateSpeed + " " + degreeConvertToRightRotaryCoder(120) + " " + rotateSpeed);
			//face
			color_r.a = (float)120f / 150;
			hit_r.GetComponent<Renderer>().material.color = color_r;
			color.a = (float)20f / 150;
			hit.GetComponent<Renderer>().material.color = color;

			yield return new WaitForSeconds(waitingTime);
			//new Thread(Uno.SendData).Start("10 150 10 150"); // L Lspeed R Rspeed
			new Thread(Uno.SendData).Start("0 255 0 255"); // L Lspeed R Rspeed

			//face
			color_r.a = (float)20f / 150;
			hit_r.GetComponent<Renderer>().material.color = color_r;
			color.a = (float)20f / 150;
			hit.GetComponent<Renderer>().material.color = color;
		}
		else if (L)//左轉，要動左馬達 (0,1)
		{
			//new Thread(Uno.SendData).Start("150 150 20 150"); // L Lspeed R Rspeed
			new Thread(Uno.SendData).Start(degreeConvertToLeftRotaryCoder(120) + " " + rotateSpeed + " " + degreeConvertToRightRotaryCoder(20) + " " + rotateSpeed);
			//face
			color.a = (float)120f / 150;
			hit.GetComponent<Renderer>().material.color = color;
			color_r.a = (float)20f / 150;
			hit_r.GetComponent<Renderer>().material.color = color_r;

			yield return new WaitForSeconds(waitingTime);
			//new Thread(Uno.SendData).Start("10 150 10 150"); // L Lspeed R Rspeed
			new Thread(Uno.SendData).Start("0 255 0 255"); // L Lspeed R Rspeed

			//face
			color.a = (float)20f / 150;
			hit.GetComponent<Renderer>().material.color = color;
			color_r.a = (float)20f / 150;
			hit_r.GetComponent<Renderer>().material.color = color_r;
		}
		shark_done = 0;
		shark_control.shark = 0;
	}

	// motor control for serial port

	private int degreeConvertToLeftRotaryCoder(int degree)
    {
        // alternation
        // increase another converter for right motor
        return ((degree * 1024 / 360) + 150);
    }

    private int degreeConvertToRightRotaryCoder(int degree)
    {
        // alternation
        // increase another converter for right motor
        return ((degree * 682 / 360) + 60);
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
                string portChoice = "COM5";
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
                arduinoController = new SerialPort(portChoice, 9600, Parity.None, 8, StopBits.One);
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
