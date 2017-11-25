using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Threading;

namespace SMarto
{
    public class MotorControl
    {
        private int initialDegree;
        private int nowDegree;
        private int maxDegree;
        private bool isClockwise;//順時針是指從開始

        public string initial(int initialDegree = 0, int maxDegree = 75, bool isClockwise = true)
        {
            this.initialDegree = initialDegree;
            this.nowDegree = initialDegree;
            this.maxDegree = maxDegree;
            this.isClockwise = isClockwise;
            return initialDegree.ToString();
        }

        public string reset()
        {
            return initialDegree.ToString();
        }

        public string addDegree(int addDegree = 5)
        {
            return preventTooBigDegree((nowDegree + addDegree)).ToString();
        }

        public string subDegree(int subDegree = 5)
        {
            return addDegree(subDegree * (-1));
        }

        public string moveTo(int goalDegree = 75)
        {
            return preventTooBigDegree(goalDegree).ToString();
        }

        public string[] repeatMotor(int[] Repeatdegree)
        {
            string[] tmp = { "" };
            for (int i = 0; i < Repeatdegree.Length; i++)
            {
                tmp[i] = preventTooBigDegree(Repeatdegree[i]).ToString();
            }
            return tmp;
        }

        private int preventTooBigDegree(int judgmentDegree)
        {
            if (isClockwise)
            {
                if (judgmentDegree <= maxDegree)
                    return judgmentDegree;
                else
                    return maxDegree;
            }
            else
            {
                if (judgmentDegree >= maxDegree)
                    return judgmentDegree;
                else
                    return maxDegree;
            }
        }
    }

    public class ArduinoConnect
    {
        private string serialPort;
        private SerialPort arduinoController;
        private bool connected;
        private bool mac;

        ArduinoConnect(string serialPort = "tty.usbmodem1421", bool isConnected = true,
            bool isMac = true)
        {
            this.serialPort = serialPort;
            connected = isConnected;
            mac = isMac;
            connectToArdunio();
        }
        private void connectToArdunio()
        {
            if (connected)
            {
                string portChoice = "COM4";
                if (mac)
                {
                    int p = (int)Environment.OSVersion.Platform;
                    // Are we on Unix?
                    if (p == 4 || p == 128 || p == 6)
                    {
                        List<string> serial_ports = new List<string>();
                        string[] ttys = Directory.GetFiles("/dev/", "tty.*");
                        foreach (string dev in ttys)
                        {
                            if (dev.StartsWith("/dev/tty."))
                            {
                                serial_ports.Add(dev);
                                //Debug.Log(String.Format(dev));
                            }
                        }
                    }
                    portChoice = "/dev/" + serialPort;
                }
                arduinoController = new SerialPort(portChoice, 115200, Parity.None, 8, StopBits.One);
                arduinoController.Handshake = Handshake.None;
                arduinoController.RtsEnable = true;
                arduinoController.Open();
            }
        }

        public void SendData(String data)
        {
            string[] newsArr;
            newsArr = data.Split(' ');
            if (connected)
            {
                if (arduinoController != null)
                {
                    arduinoController.Write(data);
                    arduinoController.Write("\n");
                }
            }
            Thread.Sleep(1000);
        }
    }
}
