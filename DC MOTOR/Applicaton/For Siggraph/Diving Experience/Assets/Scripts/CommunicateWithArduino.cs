using System;
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
    private int dataBits;
    private StopBits stopBits;
    private Handshake handshake;
    private bool RtsEnable;
    private int ReadTimeout;
    private Parity parity;
    private bool isMac;
    private bool isConnected;
    private SerialPort arduinoController;

    public CommunicateWithArduino(string portName, int baudRate = 9600, int dataBits = 8, StopBits stopBits = StopBits.One, Handshake handshake = Handshake.None,
        bool RtsEnable = true, int ReadTimeout = 1, bool isMac = false, bool isConnected = true)
    {
        this.portName = portName;
        this.baudRate = baudRate;
        this.dataBits = dataBits;
        this.stopBits = stopBits;
        this.handshake = handshake;
        this.RtsEnable = RtsEnable;
        this.ReadTimeout = ReadTimeout;
        this.isMac = isMac;
        this.isConnected = isConnected;
    }

    public void connectToArdunio()
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
            arduinoController = new SerialPort(portName, 9600, Parity.None, 8, StopBits.One);
            arduinoController.Handshake = Handshake.None;
            arduinoController.RtsEnable = true;
            arduinoController.ReadTimeout = 1;
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

    public int ReceiveData()
    {
        return int.Parse(arduinoController.ReadLine());
    }

    public void closeSerial()
    {
        arduinoController.Close();
    }
}