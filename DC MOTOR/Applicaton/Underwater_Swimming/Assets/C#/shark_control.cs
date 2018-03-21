using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.IO;
using System.IO.Ports;
using System.Threading;

public class shark_control : MonoBehaviour {

    private Animator _animator;
    int count = 0;
    private int s = 1;
    private int t = 1;
    public static int shark = 0;

    //motor (OSC)
    /*
    public GameObject RMotor;
    public GameObject LMotor;
    private OSCSender ROSCSender;
    private OSCSender LOSCSender;
    */

    //motor (serial port) - Arduino connection
    private CommunicateWithArduino Uno = new CommunicateWithArduino();

	//face
	private GameObject hit;
	private Color color;
	private GameObject hit_r;
	private Color color_r;

	// Use this for initialization
	void Start () {
        transform.localPosition = new Vector3(-30f, 0.31f, -0.26f);
        UnityEngine.Random.InitState(1337);
        _animator = this.GetComponent<Animator>();
        _animator.SetInteger("count", 0);
        _animator.SetInteger("start", 0);
        _animator.SetInteger("turn", 0);

        //motor control (OSC)
        /*
        ROSCSender = RMotor.GetComponent<OSCSender>();
        ROSCSender.setWhichMotor("R");
        LOSCSender = LMotor.GetComponent<OSCSender>();
        LOSCSender.setWhichMotor("L");
        */

        //motor (serial port)
        new Thread(Uno.connectToArdunio).Start();

		//face
		hit = GameObject.FindGameObjectWithTag("Hit");
		color = hit.GetComponent<Renderer>().material.color;
		hit_r = GameObject.FindGameObjectWithTag("Hit_R");
		color_r = hit_r.GetComponent<Renderer>().material.color;
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.S))
        {
            t = UnityEngine.Random.Range(1, 3);
            s = 1;
            _animator.SetInteger("count", s);
            _animator.SetInteger("start", 1);
            _animator.SetInteger("turn", t);
			transform.localPosition = new Vector3(35.5f, 0.31f, -0.26f);
        }

        if (!AnimatorIsPlaying() && _animator.GetCurrentAnimatorStateInfo(0).IsName("Swiming"))
        {
            if (count == 0)
            {
                s ++;
                if(s == 6)
                _animator.SetInteger("count", 8);
                //Debug.Log(s);
            }
            count = 1;
            _animator.SetInteger("start", 0);
        }
        else
        {
            if (count == 1)
            {
                count = 0;
            }
        }

        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Turn Left"))
        {
            if (shark == 0) {
                StartCoroutine(No1Work(true, false));
                Debug.Log("shark_right");
            }
            shark = 1;
            //Debug.Log("shark_r");
        }
        else if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Turn Right"))
        {
            if (shark == 0) {
                StartCoroutine(No1Work(false, true));
                Debug.Log("shark_left");
            } 
            shark = 1;
            //Debug.Log("shark_l");
        }
        else shark = 0;

    }

    bool AnimatorIsPlaying()
    {
        return _animator.GetCurrentAnimatorStateInfo(0).length >
               _animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }

    IEnumerator No1Work(bool R, bool L)
    {
        float waitingTime = 1f;
        int rotateSpeed = 150;


        if (R)//右轉，要動右馬達 (1,0)
        {
            new Thread(Uno.SendData).Start("20 150 120 150"); // L Lspeed R Rspeed
			//face
			color_r.a = (float)120f / 150;
			hit_r.GetComponent<Renderer>().material.color = color_r;
			color.a = (float)20f / 150;
			hit.GetComponent<Renderer>().material.color = color;

			yield return new WaitForSeconds(waitingTime);
            new Thread(Uno.SendData).Start("10 150 10 150"); // L Lspeed R Rspeed

			//face
			color_r.a = (float)20f / 150;
			hit_r.GetComponent<Renderer>().material.color = color_r;
			color.a = (float)20f / 150;
			hit.GetComponent<Renderer>().material.color = color;
		}
        else if (L)//左轉，要動左馬達 (0,1)
        {
            new Thread(Uno.SendData).Start("150 150 20 150"); // L Lspeed R Rspeed
			//face
			color.a = (float)120f / 150;
			hit.GetComponent<Renderer>().material.color = color;
			color_r.a = (float)20f / 150;
			hit_r.GetComponent<Renderer>().material.color = color_r;

			yield return new WaitForSeconds(waitingTime);
            new Thread(Uno.SendData).Start("10 150 10 150"); // L Lspeed R Rspeed

			//face
			color.a = (float)20f / 150;
			hit.GetComponent<Renderer>().material.color = color;
			color_r.a = (float)20f / 150;
			hit_r.GetComponent<Renderer>().material.color = color_r;
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
