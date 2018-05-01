using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.IO;
using System.IO.Ports;
using System.Threading;
using UnityEngine.UI;

public class hitted_v2 : MonoBehaviour
{

	int s = 0;
	int state = 0;

	//hit_pos_on_face

	/*
	public GameObject RMotor;
	public GameObject LMotor;
	private OSCSender ROSCSender;
	private OSCSender LOSCSender;
    */

	// Arduino connection
	private CommunicateWithArduino Uno = new CommunicateWithArduino();

	private GameObject hit;
	private Transform face;
	private Vector3 hit_position;
	int count = 0;
	Color color = Color.black;
	private Vector3 offset;
	private Vector3 move;
	private Vector3 hit_move;

	private Transform hit_face;

	//when hitted
	float time = 0.5f;
	float l = 0.7f;
	float r = 0.7f;
	float k = 0.7f;

	public GameObject superGameObject;
	public Texture[] myTextures = new Texture[7];
	public GameObject head;
	private GameObject myLine;
	private Vector3 Line;

	private Transform player_blood;
	
	private float hp = 0;

	//write data
	private int writecounter = 0;
	private bool send2motor = false;
	private string motordata = "";
	public String userName = "User1";
	private StreamWriter fileWriter;
	public GameObject HMD;
	public GameObject Lcontroller;
	public GameObject Rcontroller;
	public int times = 1;
	private int motor_release = 0;
	private string motor_data_release = "";

	//timer
	public Text timerTextDisplay;
	private float remainingTime = 60;
	private int t = 1;
	private int T = 0;

	// Use this for initialization
	void Start()
	{

		/*
		ROSCSender = RMotor.GetComponent<OSCSender>();
		ROSCSender.setWhichMotor("R");
		LOSCSender = LMotor.GetComponent<OSCSender>();
		LOSCSender.setWhichMotor("L");
        */

		//controller setActive
		//Lcontroller.SetActive(true);
		//Rcontroller.SetActive(true);

		new Thread(Uno.connectToArdunio).Start();

		//hit_pos_on_face
		hit = GameObject.FindGameObjectWithTag("Hit");//show where hitted on image---> delete now
		face = GameObject.FindGameObjectWithTag("Face").transform;//image center
		hit_position = hit.transform.position;
		hit.transform.localScale = new Vector3(0, 0, 0);
		color = hit.GetComponent<Renderer>().material.color;
		offset = face.position - hit.transform.position;

		hit_face = GameObject.FindGameObjectWithTag("hitted").transform;//show where boxer hit on sphere

		player_blood = GameObject.FindGameObjectWithTag("Player_blood").transform;
		
		//write data
		writeFile("Time, HMD_pos.x, HMD_pos.y, HMD_pos.z, HMD_rot.x, HMD_rot.y, HMD_rot.z, Lcontroller_pos.x, Lcontroller_pos.y, Lcontroller_pos.z, Lcontroller_rot.x, Lcontroller_rot.y, Lcontroller_rot.z, Rcontroller_pos.x, Rcontroller_pos.y, Rcontroller_pos.z, Rcontroller_rot.x, Rcontroller_rot.y, Rcontroller_rot.z, send2motor, motor_data, punch_type\n");
	}

	void Update()
	{
		//get animater in which state
		if (s != anim_change.s)
		{
			s = anim_change.s;
			if (anim_change.s != 0) state = anim_change.s;
		}

		//blood
		player_blood.localPosition = new Vector3(-332 * (hp / 250f), 0, 0);
		

		if (collider_dir.Rhit == 1)
		{

			if (state == 1)
			{
				//moving position
				time = 0.2f;
				l = 1f;
				//moving rotation
				r = 10f;
				//hit
				color.a = 0.8f;
				//
				k = 1f;
				//Debug.Log("R 重 ");
			}
			else if (state == 3)
			{
				//moving position
				time = 0.4f;
				l = 0.7f;
				//moving rotation
				r = 5f;
				//hit
				color.a = 0.5f;
				//
				k = 0.5f;
				//Debug.Log("R 輕 ");
			}

			move = collider_dir.Rdir;
			move = move.normalized;

			// camera move by where the punch hit from and the power -> move position //camera位移方向和大小依拳頭來的方向和距離決定
			//Vector3 pos = this.transform.position + collider_dir.Rdir * l;    
			//pos 位移固定
			Vector3 pos = this.transform.position + new Vector3(0.2f, -0.1f, -1.4f);
			r = 8f;

			Sequence mySequence = DOTween.Sequence();

			Tweener move1 = transform.DOMove(pos, time, true);
			Tweener rot1 = transform.DORotate(this.transform.rotation.eulerAngles + new Vector3(r, 0, r), 0.2f);
			Tweener move2 = transform.DOMove(this.transform.position, 0.5f);
			Tweener rot2 = transform.DORotate(this.transform.rotation.eulerAngles, 0.2f);

			mySequence.Append(move1);
			mySequence.Join(rot1);
			mySequence.Append(move2);
			mySequence.Join(rot2);
			Debug.Log("Rhit ");
			collider_dir.Rhit = 0;

			//hit_pos_on_face
			if (collider_dir.hit_pos.x > 0.42) collider_dir.hit_pos.x = 0.42f;
			else if (collider_dir.hit_pos.x < -0.42) collider_dir.hit_pos.x = -0.42f;

			hit.transform.position = new Vector3(face.position.x + collider_dir.hit_pos.x, face.position.y + collider_dir.hit_pos.y, face.position.z);
			hit_position = new Vector3(face.position.x + collider_dir.hit_pos.x * 0.5f, face.position.y + collider_dir.hit_pos.y * 0.5f, face.position.z);

			hit_face.position = collider_dir.pos;
			if (collider_dir.hit_pos.x > 0.1)
			{
				if (state == 3) head.GetComponent<Renderer>().material.mainTexture = myTextures[1];
				else head.GetComponent<Renderer>().material.mainTexture = myTextures[4];
				StartCoroutine(No1Work(true, false, state));
			}
			else if (collider_dir.hit_pos.x < -0.1)
			{
				if (state == 3) head.GetComponent<Renderer>().material.mainTexture = myTextures[3];
				else head.GetComponent<Renderer>().material.mainTexture = myTextures[6];
				StartCoroutine(No1Work(false, true, state));
			}
			else
			{
				if (state == 3) head.GetComponent<Renderer>().material.mainTexture = myTextures[2];
				else head.GetComponent<Renderer>().material.mainTexture = myTextures[5];
				StartCoroutine(No1Work(false, false, state));
			}
			count++;
			Line = hit.transform.position;
			DrawLine(hit_position + move * k * 2, hit_position, 1f);

		}
		else if (collider_dir.Lhit == 1)
		{
			if (state == 2)
			{
				//moving position
				time = 0.2f;
				l = 1.2f;
				//moving rotation
				r = 10f;
				//hit
				color.a = 0.8f;
				//
				k = 1f;
				//Debug.Log("L 重 ");
			}
			else if (state == 4)
			{
				//moving position
				time = 0.5f;
				l = 1f;
				//moving rotation
				r = 5f;
				//hit
				color.a = 0.5f;
				//
				k = 0.5f;
				//Debug.Log("L 輕 ");
			}
			else if (state == 5)
			{
				//moving position
				time = 0.2f;
				l = 1.2f;
				//moving rotation
				r = 10f;
				//hit
				color.a = 0.8f;
				//
				k = 1f;
				//Debug.Log("L 重 ");
			}

			move = collider_dir.Ldir;
			move = move.normalized;

			// camera move by where the punch hit from and the power -> move position //camera位移方向和大小依拳頭來的方向和距離決定
			//Vector3 pos = this.transform.position + collider_dir.Ldir * l;
			//pos 位移固定
			Vector3 pos = this.transform.position + new Vector3(0.2f, -0.1f, -1.4f);
			r = 8f;
			Sequence mySequence = DOTween.Sequence();

			Tweener move1 = transform.DOMove(pos, time, true);
			Tweener rot1 = transform.DORotate(this.transform.rotation.eulerAngles + new Vector3(r, 0, -r), 0.2f);
			Tweener move2 = transform.DOMove(this.transform.position, 0.5f);
			Tweener rot2 = transform.DORotate(this.transform.rotation.eulerAngles, 0.2f);

			mySequence.Append(move1);
			mySequence.Join(rot1);
			mySequence.Append(move2);
			mySequence.Join(rot2);
			Debug.Log("Lhit ");
			collider_dir.Lhit = 0;

			//hit_pos_on_face
			if (collider_dir.hit_pos.x > 0.42) collider_dir.hit_pos.x = 0.42f;
			else if (collider_dir.hit_pos.x < -0.42) collider_dir.hit_pos.x = -0.42f;

			hit.transform.position = new Vector3(face.position.x + collider_dir.hit_pos.x, face.position.y + collider_dir.hit_pos.y, face.position.z);
			hit_position = new Vector3(face.position.x + collider_dir.hit_pos.x * 0.5f, face.position.y + collider_dir.hit_pos.y * 0.5f, face.position.z);

			hit_face.position = collider_dir.pos;
			if (collider_dir.hit_pos.x > 0.1)
			{
				if (state == 4) head.GetComponent<Renderer>().material.mainTexture = myTextures[1];
				else head.GetComponent<Renderer>().material.mainTexture = myTextures[4];
				StartCoroutine(No1Work(true, false, state));
			}
			else if (collider_dir.hit_pos.x < -0.1)
			{
				if (state == 4) head.GetComponent<Renderer>().material.mainTexture = myTextures[3];
				else head.GetComponent<Renderer>().material.mainTexture = myTextures[6];
				StartCoroutine(No1Work(false, true, state));
			}
			else
			{
				if (state == 4) head.GetComponent<Renderer>().material.mainTexture = myTextures[2];
				else head.GetComponent<Renderer>().material.mainTexture = myTextures[5];
				StartCoroutine(No1Work(false, false, state));
			}
			count++;
			Line = hit.transform.position;
			DrawLine(hit_position + move * k * 2, hit_position, 1f);

		}


		if (count != 0)
		{
			count++;
			offset = hit.transform.position - hit_move - hit_position;
			offset = hit.transform.position - Line;
			if (myLine != null)
			{
				myLine.transform.position = hit_position + move * k * 2 + offset;
				LineRenderer lr = myLine.GetComponent<LineRenderer>();
				lr.SetPosition(0, hit_position + move * k * 2 + offset);
				lr.SetPosition(1, hit_position + offset);
			}
		}
		if (count == 80)
		{
			count = 0;
			hit.transform.localScale = new Vector3(0, 0, 0);
			head.GetComponent<Renderer>().material.mainTexture = myTextures[0];
		}

		//write data
		writecounter++;
		if (writecounter == 5)
		{
			if (send2motor == true) motor_release = 1;
			if (motor_release == 1 && send2motor == false)
			{
				motordata = motor_data_release;
				send2motor = true;
				motor_release = 0;
				motor_data_release = "";
			}
			writeFile(System.DateTime.Now.ToString() + "," + HMD.transform.position.ToString("f4") + "," + HMD.transform.rotation.eulerAngles.ToString("f4") + ","
				+ Lcontroller.transform.position.ToString("f4") + "," + Lcontroller.transform.rotation.eulerAngles.ToString("f4") + ","
				+ Rcontroller.transform.position.ToString("f4") + "," + Rcontroller.transform.rotation.eulerAngles.ToString("f4") + ","
				+ send2motor.ToString() + "," + motordata + ","
				+ state + "\n");

			send2motor = false;
			motordata = "";

			writecounter = 0;
		}

		//Timer
		if (Input.GetKeyDown(KeyCode.S)) {
			t = 1;
			remainingTime = 60;
		} 

		if(t == 1) remainingTime -= (Time.deltaTime);
		if(remainingTime > 0 && T != (int)remainingTime) {Debug.Log((int)remainingTime); T = (int)remainingTime; timerTextDisplay.text = T.ToString();}
	}

	void DrawLine(Vector3 start, Vector3 end, float duration = 1f)
	{
		myLine = new GameObject();
		myLine.transform.SetParent(superGameObject.transform);
		myLine.transform.position = start;
		myLine.AddComponent<LineRenderer>();
		LineRenderer lr = myLine.GetComponent<LineRenderer>();
		lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
		lr.startColor = Color.red;
		lr.endColor = Color.gray;
		lr.startWidth = 0.05f;
		lr.endWidth = 0.01f;
		lr.SetPosition(0, start);
		lr.SetPosition(1, end);
		GameObject.Destroy(myLine, duration);
	}

	//write data
	private void writeFile(String data)
	{
		fileWriter = new StreamWriter("Results\\" + userName + "_" + times + ".csv", true);
		fileWriter.Write(data);
		fileWriter.Flush();
		fileWriter.Close();
		fileWriter.Dispose();
	}

	//motor & blood
	IEnumerator No1Work(bool R, bool L, int state)
	{
		float time = 0.5f;
		int RSpeed = 50;
		int LSpeed = 50;
		int angle = 150;
		int langle = 150;

		//study2 94 , 134

		if (R)//奇數次點擊
		{
			if (state == 1 || state == 2 || state == 5) { RSpeed = 255; angle = 134; Debug.Log("R 重 "); if (hp + 25 < 250) hp += 25; else hp = 250; }
			else if (state == 3 || state == 4) { RSpeed = 255; angle = 94; Debug.Log("R 輕 "); if (hp + 15 < 250) hp += 15; else hp = 250; }
			new Thread(Uno.SendData).Start("0 255 " + degreeConvertToRightRotaryCoder(angle) + " " + RSpeed); //L Lspeed R Rspeed
			yield return new WaitForSeconds(time);
			new Thread(Uno.SendData).Start("0 255 0 " + RSpeed); //L Lspeed R Rspeed

			//write data
			motordata = "0 255 " + degreeConvertToRightRotaryCoder(angle).ToString() + " " + RSpeed.ToString();
			motor_data_release = "0 255 0 " + RSpeed.ToString();
		}
		else if (L)
		{
			if (state == 1 || state == 2 || state == 5) { LSpeed = 255; angle = 134; Debug.Log("L 重 "); if (hp + 25 < 250) hp += 25; else hp = 250; }
			else if (state == 3 || state == 4) { LSpeed = 255; angle = 94; Debug.Log("L 輕 "); if (hp + 15 < 250) hp += 15; else hp = 250; }
			new Thread(Uno.SendData).Start(degreeConvertToLeftRotaryCoder(angle) + " " + LSpeed + " 0 255"); //L Lspeed R Rspeed
			yield return new WaitForSeconds(time);
			new Thread(Uno.SendData).Start("0 " + LSpeed + " 0 255"); //L Lspeed R Rspeed

			//write data
			motordata = degreeConvertToLeftRotaryCoder(angle).ToString() + " " + LSpeed.ToString() + " 0 255";
			motor_data_release = "0 " + LSpeed.ToString() + " 0 255";
		}
		else
		{
			if (state == 1 || state == 2 || state == 5) { RSpeed = 255; angle = 134; Debug.Log("C 重 "); if (hp + 25 < 250) hp += 25; else hp = 250; }
			else if (state == 3 || state == 4) { RSpeed = 255; angle = 94; Debug.Log("C 輕 "); if (hp + 15 < 250) hp += 15; else hp = 250; }

			//no langle
			new Thread(Uno.SendData).Start(degreeConvertToLeftRotaryCoder(angle) + " " + RSpeed + " " + degreeConvertToRightRotaryCoder(angle) + " " + RSpeed); //L Lspeed R Rspeed
			yield return new WaitForSeconds(time);
			new Thread(Uno.SendData).Start("0 " + RSpeed + " 0 " + RSpeed); //L Lspeed R Rspeed

			//write data
			motordata = degreeConvertToLeftRotaryCoder(angle).ToString() + " " + RSpeed.ToString() + " " + degreeConvertToRightRotaryCoder(angle).ToString() + " " + RSpeed.ToString();
			motor_data_release = "0 " + RSpeed.ToString() + " 0 " + RSpeed.ToString();
		}

		//write data
		send2motor = true;
	}


	// motor control (serial port)
	private int degreeConvertToLeftRotaryCoder(int degree)
	{
		// alternation
		// increase another converter for right motor
		return ((degree * 1024 / 360) + 100);
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
