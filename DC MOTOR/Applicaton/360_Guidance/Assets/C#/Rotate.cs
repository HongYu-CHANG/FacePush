using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.IO.Ports;
using System.Threading;

public class Rotate : MonoBehaviour {

    public GameObject cube;
    private float init = 0f;
    private int start = 0;
    private float rot = 0;
    private int motor_angle = 0;
    private int counter = 3;
    private bool R = true;
    private float angle = 90;

    //motor (serial port) - Arduino connection
    private CommunicateWithArduino Uno = new CommunicateWithArduino();

    // Use this for initialization
    void Start () {
        
        //motor (serial port)
        new Thread(Uno.connectToArdunio).Start();

    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.S))
        {
            init = this.transform.rotation.eulerAngles.y;
            start = 1;
            if (init > 270) angle = 90 - init + 360;
            else angle = 90 - init;
        }


        if(start == 1)
        {
            rot = this.transform.rotation.eulerAngles.y - init;

            //if (rot < 0) rot = rot + 360;
            //make the degree between 0 ~ 180
            if (rot > 270) rot = 0;
            else if (rot > 180) rot = 180;

            if(angle - rot > 2)//未轉到
            {
                motor_angle = (int)((int)((angle - rot) / 5 )* 5 * 1.6);
                R = true;
            }
            else if(angle - rot < -2)//轉過頭
            {
                motor_angle = (int)((int)((rot - angle) / 5) * 5 * 1.6);
                R = false;
            }

        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log(motor_angle);
            Debug.Log("rot: " + rot);
        }

        //motor
        if(counter == 0)
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
