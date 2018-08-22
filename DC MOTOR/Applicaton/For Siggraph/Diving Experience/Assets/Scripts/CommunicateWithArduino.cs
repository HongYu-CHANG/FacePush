﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.IO.Ports;
using System.Threading;

public class CommunicateWithArduino
{
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
    private bool isLocked;
    private string motorFinishMessage = "";
    private DateTime getFinishMessageTime = DateTime.Now;
    private SerialPort arduinoController;

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
        isLocked = false;
        startConnect();
    }

    private void startConnect()
    {
        if (isConnected)
        {
            if (isMac)
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
                portName = "/dev/" + portName;
            }
            arduinoController = new SerialPort(portName, baudRate, parity, dataBits, stopBits);
            arduinoController.Handshake = handshake;
            arduinoController.RtsEnable = RtsEnable;
            arduinoController.ReadTimeout = ReadTimeout;
            arduinoController.Open();
            Debug.Log("Connected!!");
        }
    }
    public void sendData(object obj)
    {
        string data = obj as string;
        if (isConnected && !isLocked && arduinoController != null)
        {
            isLocked = true;
            Debug.LogWarning(data);
            arduinoController.Write(data);
            arduinoController.Write("\n");
            getFinishMessageTime = DateTime.Now;
            Thread.Sleep(500);
        }
        else
        {
            Debug.Log("Not Connected or Locked");
        }  
    }

    public string receiveData()
    {
        return arduinoController.ReadLine();
    }

    public void motorLocker()
    {
        double seconds = (DateTime.Now - getFinishMessageTime).TotalSeconds;
        //Debug.Log(seconds);
        try
        {
            motorFinishMessage = arduinoController.ReadLine();
        }
        catch (Exception e)
        {
            if (seconds > 2.5f)
            {
                motorFinishMessage = "P";
            }

        }
        if (motorFinishMessage == "P" && seconds > 1.0f)
        {
            getFinishMessageTime = DateTime.Now;
            isLocked = false;
            motorFinishMessage = "";
        }

    }

    public bool getisLocked()
    {
        return isLocked;
    }

    public void closeSerial()
    {
        arduinoController.Close();
    }
}