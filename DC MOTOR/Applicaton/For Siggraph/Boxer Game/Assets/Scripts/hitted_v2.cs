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
	// Arduino connection
	private CommunicateWithArduino Uno = new CommunicateWithArduino("COM5", baudRate:115200);
	private CommunicateWithArduino UnoThermal = new CommunicateWithArduino("COM7", baudRate:115200);

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
	public GameObject HMD;
	public GameObject Lcontroller;
	public GameObject Rcontroller;
	public int times = 1;
	private int motor_release = 0;
	private string motor_data_release = "";

	//timer
	public Text timerTextDisplay;
	private float remainingTime = 60;

    //gameover
    private bool isGameover = false;

    //Timer
    private float timer_f = 15f;
    private int timer_i = 15;


	// Use this for initialization
	void Start()
	{
		new Thread(Uno.connectToArdunio).Start();
		new Thread(UnoThermal.connectToArdunio).Start();

		//hit_pos_on_face
		hit = GameObject.FindGameObjectWithTag("Hit");//show where hitted on image---> delete now
		face = GameObject.FindGameObjectWithTag("Face").transform;//image center
		hit_position = hit.transform.position;
		hit.transform.localScale = new Vector3(0, 0, 0);
		color = hit.GetComponent<Renderer>().material.color;
		offset = face.position - hit.transform.position;

		hit_face = GameObject.FindGameObjectWithTag("hitted").transform;//show where boxer hit on sphere

		player_blood = GameObject.FindGameObjectWithTag("Player_blood").transform;
		new Thread(Uno.SendData).Start("B"); //Boxing Setting
	}

	void Update()
	{
		//get animater in which state
		timer_f += Time.deltaTime;
        timer_i = (int)timer_f;
        //Debug.Log(timer_i);

		if (s != anim_change.s)
		{
			s = anim_change.s;
			if (anim_change.s != 0) state = anim_change.s;
		}

		//blood
		player_blood.localPosition = new Vector3(-332 * (hp / 250f), 0, 0);
		//Debug.Log (collider_dir.pos.x);

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
			//Debug.Log("Rhit ");
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
			//DrawLine(hit_position + move * k * 2, hit_position, 1f);

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
			//Debug.Log("Lhit ");
			collider_dir.Lhit = 0;

			//hit_pos_on_face
			if (collider_dir.hit_pos.x > 0.42) collider_dir.hit_pos.x = 0.42f;
			else if (collider_dir.hit_pos.x < -0.42) collider_dir.hit_pos.x = -0.42f;

			hit.transform.position = new Vector3(face.position.x + collider_dir.hit_pos.x, face.position.y + collider_dir.hit_pos.y, face.position.z);
			hit_position = new Vector3(face.position.x + collider_dir.hit_pos.x * 0.5f, face.position.y + collider_dir.hit_pos.y * 0.5f, face.position.z);

			hit_face.position = collider_dir.pos;

			if (collider_dir.hit_pos.x > 0.1)
			{
				Debug.Log ("Right !!");
				if (state == 4) head.GetComponent<Renderer>().material.mainTexture = myTextures[1];
				else head.GetComponent<Renderer>().material.mainTexture = myTextures[4];
				StartCoroutine(No1Work(true, false, state));
			}
			else if (collider_dir.hit_pos.x < -0.1)
			{
				Debug.Log ("Left !!");
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
			//DrawLine(hit_position + move * k * 2, hit_position, 1f);

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
			send2motor = false;
			motordata = "";

			writecounter = 0;
		}

		//Timer
		if (Input.GetKeyDown(KeyCode.S)) {
			remainingTime = 60;
		} 

        remainingTime -= (Time.deltaTime);
		if(remainingTime > 0 )
            timerTextDisplay.text = ((int)remainingTime).ToString();

        //gameover
        if ((int)remainingTime <= 0) isGameover = true;
		if(hp >= 200) isGameover = true;
        if(isGameover) StartCoroutine(gameoverScene());
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

	//motor & blood
	IEnumerator No1Work(bool R, bool L, int state)
	{
		float time = 0.5f;
		int angle = 150;
		int langle = 150;
		int thermal = 0; // Hot

        //study2 94 , 134
        if(state != 5 && state != 1)
            yield return new WaitForSeconds(0f);
        else if (state == 5)
            yield return new WaitForSeconds(0f);
        else if (state == 1)
            yield return new WaitForSeconds(0);
        if (R)//奇數次點擊
		{
			if (state == 1 || state == 2 || state == 5) {angle = 134; thermal = 0; Debug.Log("R 重 "); if (hp + 25 < 250) hp += 25; else hp = 250; }
			else if (state == 3 || state == 4) {angle = 94; thermal = 0; Debug.Log("R 輕 "); if (hp + 15 < 250) hp += 15; else hp = 250; }
			new Thread(Uno.SendData).Start("0 " + degreeConvertToRightRotaryCoder(angle)); //L Lspeed R Rspeed
			yield return new WaitForSeconds(time);
			new Thread(Uno.SendData).Start("0 0 "); //L Lspeed R Rspeed
		}
		else if (L)
		{
			if (state == 1 || state == 2 || state == 5) {angle = 160; thermal = 0; Debug.Log("L 重 "); if (hp + 25 < 250) hp += 25; else hp = 250; }
			else if (state == 3 || state == 4) {angle = 94; thermal = 0; Debug.Log("L 輕 "); if (hp + 15 < 250) hp += 15; else hp = 250; }
			new Thread(Uno.SendData).Start(degreeConvertToLeftRotaryCoder(angle) + " " + "0"); //L Lspeed R Rspeed
			yield return new WaitForSeconds(time);
			new Thread(Uno.SendData).Start("0 "+ "0"); //L Lspeed R Rspeed
		}
		else
		{
			if (state == 1 || state == 2 || state == 5) {angle = 134; thermal = -75; Debug.Log("C 重 "); if (hp + 25 < 250) hp += 25; else hp = 250; }
			else if (state == 3 || state == 4) {angle = 94; thermal = -25;Debug.Log("C 輕 "); if (hp + 15 < 250) hp += 15; else hp = 250; }

			//no langle
			if (state == 1 || state == 2 || state == 5) StartCoroutine(diveThermal(thermal, thermal, 1f));
			new Thread(Uno.SendData).Start(degreeConvertToLeftRotaryCoder(angle) + " "+ degreeConvertToRightRotaryCoder(angle)); //L Lspeed R Rspeed
			yield return new WaitForSeconds(time);
			new Thread(Uno.SendData).Start("0 "+ "0 "); //L Lspeed R Rspeed

		}

		//write data
		send2motor = true;
	}


	// motor control (serial port)
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
		return (degree * 824 / 360);
	}

	IEnumerator diveThermal(int Left, int Right, float time)
    {
	    if(timer_i >= 15)
	    {
		    timer_f = 0;
	        timer_i = 0;
		    new Thread(UnoThermal.SendData).Start(Left + " " + Right);
		    yield return new WaitForSeconds(time);
		    new Thread(UnoThermal.SendData).Start("0" + " " + "0");
   		}
    }

    IEnumerator gameoverScene()
    {
        yield return new WaitForSeconds(0.8f);
        float fadeTime = GameObject.Find("Camera (eye)").GetComponent<SteamVrFade>().FadeToBlack();
        GameObject.Find("Boxer_Avatar").GetComponent<anim_change>().GameOver();
        yield return new WaitForSeconds(fadeTime);
    }

    class CommunicateWithArduino
	{
		public bool connected = true;
		public bool mac = false;
		public string choice = "cu.usbmodem1421";
		private SerialPort arduinoController;

		private string portName;
	    private int baudRate;
	    private Parity parity;
	    private int dataBits;
	    private StopBits stopBits;
	    private Handshake handshake;
	    private bool RtsEnable;
	    private int ReadTimeout;
	    private bool isMac;
	    private bool isConnected;

		public CommunicateWithArduino(string portName, int baudRate = 9600, Parity parity = Parity.None, int dataBits = 8, StopBits stopBits = StopBits.One, Handshake handshake = Handshake.None,
		        bool RtsEnable = true, int ReadTimeout = 1, bool isMac = false, bool isConnected = true)
		    {
		        this.portName = portName;
		        this.baudRate = baudRate;
		        this.parity = parity;
		        this.dataBits = dataBits;
		        this.stopBits = stopBits;
		        this.handshake = handshake;
		        this.RtsEnable = RtsEnable;
		        this.ReadTimeout = ReadTimeout;
		        this.isMac = isMac;
		        this.isConnected = isConnected;
		        //connectToArdunio();
		    }
		public void connectToArdunio()
		{

			if (connected)
			{
				string portChoice = portName;
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
				arduinoController = new SerialPort(portChoice, 115200, Parity.None, 8, StopBits.One);
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
