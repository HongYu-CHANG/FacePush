using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.IO;
using System.IO.Ports;
using System.Threading;

public class fish_control : MonoBehaviour {

    //motor (OSC)
    /*
    public GameObject RMotor;
    public GameObject LMotor;
    private OSCSender ROSCSender;
    private OSCSender LOSCSender;
    */

    //motor (serial port) - Arduino connection
    //private CommunicateWithArduino Uno = new CommunicateWithArduino();

    public static int fish = 0;

	//face
	private GameObject hit;
	private Color color;
	private GameObject hit_r;
	private Color color_r;

	// Use this for initialization
	void Start () {
        //motor control (OSC)
        /*
        ROSCSender = RMotor.GetComponent<OSCSender>();
        ROSCSender.setWhichMotor("R");
        LOSCSender = LMotor.GetComponent<OSCSender>();
        LOSCSender.setWhichMotor("L");
        */

        //motor (serial port)
        //new Thread(Uno.connectToArdunio).Start();

		//fishflock initial movement
        //transform.DOLocalMove(new Vector3(20, 0.8f, -0.07f), 5);

		//face
		hit = GameObject.FindGameObjectWithTag("Hit");
		color = hit.GetComponent<Renderer>().material.color;
		hit_r = GameObject.FindGameObjectWithTag("Hit_R");
		color_r = hit_r.GetComponent<Renderer>().material.color;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.H))
		{
			//after initialization
			transform.DOLocalMove(new Vector3(20, 0.8f, -0.07f), 5);
		}

		if (Input.GetKeyDown(KeyCode.F))
        {
			//
			transform.localPosition = new Vector3(20, 0.8f, -0.07f);
			Sequence mySequence = DOTween.Sequence();
            mySequence.Append(transform.DOLocalMoveX(-20, 3));
            mySequence.Append(transform.DOLocalMoveZ(20, 1f).SetDelay(2));
            mySequence.Append(transform.DOLocalMoveX(20, 0.5f));
            mySequence.Append(transform.DOLocalMoveZ(-0.07f, 1f));

            //StartCoroutine(No1Work());
            fish = 1;
            Debug.Log("fish");
        }

        if (fish == 1 && transform.localPosition.z == 20) fish = 0;

		if (Input.GetKeyDown(KeyCode.B))
		{
			transform.DOLocalMoveX(-20, 3);
		}


	}

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
	/*
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
	}
	*/
	/*
	IEnumerator No1Work()
    {
        float waitingTime = 2f;
        int speed = 100;
        float tempTime = UnityEngine.Random.Range(0.01f, 0.03f);//tempTime = 0;
        yield return new WaitForSeconds(waitingTime);

        new Thread(Uno.SendData).Start("170 100 10 100"); // L Lspeed R Rspeed
        yield return new WaitForSeconds(0.15f - tempTime);
		face_color();
        new Thread(Uno.SendData).Start("10 100 170 100"); // L Lspeed R Rspeed
        yield return new WaitForSeconds(0.15f + tempTime);
		face_color_r();
		new Thread(Uno.SendData).Start("170 100 10 100"); // L Lspeed R Rspeed
        yield return new WaitForSeconds(0.15f - tempTime);
		face_color();
		new Thread(Uno.SendData).Start("10 100 170 100"); // L Lspeed R Rspeed
        yield return new WaitForSeconds(0.15f + tempTime);
		face_color_r();
		new Thread(Uno.SendData).Start("170 100 10 100"); // L Lspeed R Rspeed
        yield return new WaitForSeconds(0.15f - tempTime);
		face_color();
		new Thread(Uno.SendData).Start("10 100 170 100"); // L Lspeed R Rspeed
        yield return new WaitForSeconds(0.15f + tempTime);
		face_color_r();
		new Thread(Uno.SendData).Start("170 100 10 100"); // L Lspeed R Rspeed
        yield return new WaitForSeconds(0.15f - tempTime);
		face_color();
		new Thread(Uno.SendData).Start("10 100 170 100"); // L Lspeed R Rspeed
        yield return new WaitForSeconds(0.15f + tempTime);
		face_color_r();
		new Thread(Uno.SendData).Start("170 100 10 100"); // L Lspeed R Rspeed
        yield return new WaitForSeconds(0.15f - tempTime);
		face_color();
		new Thread(Uno.SendData).Start("10 100 170 100"); // L Lspeed R Rspeed
        yield return new WaitForSeconds(0.15f + tempTime);
		face_color_r();
		new Thread(Uno.SendData).Start("170 100 10 100"); // L Lspeed R Rspeed
        yield return new WaitForSeconds(0.15f - tempTime);
		face_color();
		new Thread(Uno.SendData).Start("10 100 170 100"); // L Lspeed R Rspeed
        yield return new WaitForSeconds(0.15f + tempTime);
		face_color_r();
		new Thread(Uno.SendData).Start("170 100 10 100"); // L Lspeed R Rspeed
        yield return new WaitForSeconds(0.15f - tempTime);
		face_color();
		new Thread(Uno.SendData).Start("10 100 10 100"); // L Lspeed R Rspeed
		color_r.a = (float)20f / 150;
		hit_r.GetComponent<Renderer>().material.color = color_r;
		color.a = (float)20f / 150;
		hit.GetComponent<Renderer>().material.color = color;
	}
	*/


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
