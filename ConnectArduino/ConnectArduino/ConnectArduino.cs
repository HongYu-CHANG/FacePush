using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Threading;

namespace ConnectArduino
{
    public class ArduinoConnect
    {
        private string serialPort;
        private SerialPort arduinoController;
        private bool connected;
        private bool mac;

        public void createConnect(string serialPort = "tty.usbmodem1421", bool isConnected = true,
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
