﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.IO;
using System.IO.Ports;
using System.Threading;
using UnityEngine.UI;

public class hitted_control : MonoBehaviour {
    int s = 0;
    int state = 0;

	// Arduino connection
	private CommunicateWithArduino Uno = new CommunicateWithArduino();
	//OSC
	/*public GameObject RMotor;
    public GameObject LMotor;
    private OSCSender ROSCSender;
    private OSCSender LOSCSender;*/

	//hit_pos_on_face
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

    private int control = 1;
    private int control_LH = 1;
    private int counts = 0;
    private Animator _animator;
    public GameObject box;

    // Use this for initialization
    void Start () {
        _animator = box.GetComponent<Animator>();
		//OSC
		/*
        ROSCSender = RMotor.GetComponent<OSCSender>();
        ROSCSender.setWhichMotor("R");
        LOSCSender = LMotor.GetComponent<OSCSender>();
        LOSCSender.setWhichMotor("L");
        */
		new Thread(Uno.connectToArdunio).Start();

		hit = GameObject.FindGameObjectWithTag("Hit");
        face = GameObject.FindGameObjectWithTag("Face").transform;
        hit_position = hit.transform.position;
        hit.transform.localScale = new Vector3(0, 0, 0);
        color = hit.GetComponent<Renderer>().material.color;
        offset = face.position - hit.transform.position;

        hit_face = GameObject.FindGameObjectWithTag("hitted").transform;
    }
	
	// Update is called once per frame
	void Update () {
        //get animater in which state
        if (s != anim_change.s)
        {
            s = anim_change.s;
            if (anim_change.s != 0) state = anim_change.s;
        }

        if (Input.GetKeyDown(KeyCode.Z)) control = 1;
        else if (Input.GetKeyDown(KeyCode.X)) control = 3;
        else if (Input.GetKeyDown(KeyCode.C)) control = 2;

        if (Input.GetKeyDown(KeyCode.A)) control_LH = 1;
        else if (Input.GetKeyDown(KeyCode.S)) control_LH = 2;

        if (collider_dir.Rhit == 1 && counts == 0)
            {

                if (state == 1 || state == 5)
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

                Vector3 pos = this.transform.position + collider_dir.Rdir * l;
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
                if (control == 1)
                {
                    if (state == 3) head.GetComponent<Renderer>().material.mainTexture = myTextures[1];
                    else head.GetComponent<Renderer>().material.mainTexture = myTextures[4];
                    hit_position = new Vector3(face.position.x + 0.3f, face.position.y + collider_dir.hit_pos.y * 0.5f, face.position.z);
					StartCoroutine(No1Work(true, false, state));
			}
                else if (control == 2)
                {
                    if (state == 3) head.GetComponent<Renderer>().material.mainTexture = myTextures[3];
                    else head.GetComponent<Renderer>().material.mainTexture = myTextures[6];
                    hit_position = new Vector3(face.position.x - 0.3f, face.position.y + collider_dir.hit_pos.y * 0.5f, face.position.z);
					StartCoroutine(No1Work(false, true, state));
			}
                else
                {
                    if (state == 3) head.GetComponent<Renderer>().material.mainTexture = myTextures[2];
                    else head.GetComponent<Renderer>().material.mainTexture = myTextures[5];
                    hit_position = new Vector3(face.position.x , face.position.y + collider_dir.hit_pos.y * 0.5f, face.position.z);
					StartCoroutine(No1Work(false, false, state));
			}
                count++;
                Line = hit.transform.position;
                DrawLine(hit_position + move * k * 2, hit_position, 1f);

            counts = 1;
        }
            else if (collider_dir.Lhit == 1 && counts == 0)
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
                

                move = collider_dir.Ldir;
                move = move.normalized;

                Vector3 pos = this.transform.position + collider_dir.Ldir * l;
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
                if (control == 1)
                {
                    if (state == 4) head.GetComponent<Renderer>().material.mainTexture = myTextures[1];
                    else head.GetComponent<Renderer>().material.mainTexture = myTextures[4];
                    hit_position = new Vector3(face.position.x + 0.3f, face.position.y + collider_dir.hit_pos.y * 0.5f, face.position.z);
					StartCoroutine(No1Work(true, false, state));
			}
                else if (control == 2)
                {
                    if (state == 4) head.GetComponent<Renderer>().material.mainTexture = myTextures[3];
                    else head.GetComponent<Renderer>().material.mainTexture = myTextures[6];
                    hit_position = new Vector3(face.position.x - 0.3f, face.position.y + collider_dir.hit_pos.y * 0.5f, face.position.z);
					StartCoroutine(No1Work(false, true, state));
			}
                else
                {
                    if (state == 4) head.GetComponent<Renderer>().material.mainTexture = myTextures[2];
                    else head.GetComponent<Renderer>().material.mainTexture = myTextures[5];
                    hit_position = new Vector3(face.position.x , face.position.y + collider_dir.hit_pos.y * 0.5f, face.position.z);
					StartCoroutine(No1Work(false, false, state));

			}
                count++;
                Line = hit.transform.position;
                DrawLine(hit_position + move * k * 2, hit_position, 1f);

            counts = 1;
        }


        if (count != 0)
        {
            count++;
            //offset = hit.transform.position - hit_move - hit_position;
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

        if ( _animator.GetCurrentAnimatorStateInfo(0).IsName("State")) counts = 0;
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
	//dis between boxer and player need to be far or it may be hit twice and arrow would be wrong

	//motor & blood
	IEnumerator No1Work(bool R, bool L, int state)
	{
		float time = 0.5f;
		int RSpeed = 50;
		int LSpeed = 50;
		int angle = 150;
		int langle = 150;

		if (R)//奇數次點擊
		{
			if (state == 1 || state == 2 || state == 5) { RSpeed = 255; angle = 150; Debug.Log("R 重 ");  }
			else if (state == 3 || state == 4) { RSpeed = 255; angle = 100; Debug.Log("R 輕 "); }
			new Thread(Uno.SendData).Start("128 255 " + degreeConvertToRightRotaryCoder(angle) + " " + RSpeed); //L Lspeed R Rspeed
			yield return new WaitForSeconds(time);
			new Thread(Uno.SendData).Start("128 255 78 " + RSpeed); //L Lspeed R Rspeed

			
		}
		else if (L)
		{
			if (state == 1 || state == 2 || state == 5) { LSpeed = 255; angle = 150; Debug.Log("L 重 ");  }
			else if (state == 3 || state == 4) { LSpeed = 255; angle = 100; Debug.Log("L 輕 "); }
			new Thread(Uno.SendData).Start(degreeConvertToLeftRotaryCoder(angle) + " " + LSpeed + " 78 255"); //L Lspeed R Rspeed
			yield return new WaitForSeconds(time);
			new Thread(Uno.SendData).Start("128 " + LSpeed + " 78 255"); //L Lspeed R Rspeed

			
		}
		else
		{
			if (state == 1 || state == 2 || state == 5) { RSpeed = 255; angle = 150; /*langle = 170*/; Debug.Log("C 重 ");  }
			else if (state == 3 || state == 4) { RSpeed = 255; angle = 100; /*langle = 120*/; Debug.Log("C 輕 ");  }

			//no langle
			new Thread(Uno.SendData).Start(degreeConvertToLeftRotaryCoder(angle) + " " + RSpeed + " " + degreeConvertToRightRotaryCoder(angle) + " " + RSpeed); //L Lspeed R Rspeed
			yield return new WaitForSeconds(time);
			new Thread(Uno.SendData).Start("128 " + RSpeed + " 78 " + RSpeed); //L Lspeed R Rspeed

			
		}

		
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
