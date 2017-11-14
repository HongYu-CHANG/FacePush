﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class ArduinoController : MonoBehaviour {

    //serial connecting
    public bool connected = false;
    public bool mac = true;
    public string choice;
    public Dropdown RightDropdown;
    public Dropdown LeftDropdown;
    public InputField DurationField;
    public Button SendButton;
    public int RStartDegree;
    public int RMoveDegree;
    public int LStartDegree;
    public int LMoveDegree;

    private SerialPort arduinoController;
    private List<Dropdown.OptionData> RightOptions;
    private List<Dropdown.OptionData> LeftOptions;
    private Dictionary<String, int> funMap = new Dictionary<string, int>( );

    // Use this for initialization
    private void Start ()
    {
        connectToArdunio();
        Button tempBtn = SendButton.GetComponent<Button>();
        tempBtn.onClick.AddListener(sendButtonOnClick);
        RightOptions = RightDropdown.GetComponent<Dropdown>().options;
        LeftOptions = LeftDropdown.GetComponent<Dropdown>().options;

        funMap.Add("None", 0); funMap.Add("linearTween", 1);
        funMap.Add("easeInQuad", 2); funMap.Add("easeOutQuad", 3); funMap.Add("easeInOutQuad", 4);
        funMap.Add("easeInCubic", 5); funMap.Add("easeOutCubic", 6); funMap.Add("easeInOutCubic", 7);
        funMap.Add("easeInQuart", 8); funMap.Add("easeOutQuart", 9); funMap.Add("easeInOutQuart", 10);
        funMap.Add("easeInQuint", 29); funMap.Add("easeOutQuint", 30); funMap.Add("easeInOutQuint", 31);
        funMap.Add("easeInSine", 11); funMap.Add("easeOutSine", 12); funMap.Add("easeInOutSine", 13);
        funMap.Add("easeInExpo", 14); funMap.Add("easeOutExpo", 15); funMap.Add("easeInOutExpo", 16);
        funMap.Add("easeInCirc", 17); funMap.Add("easeOutCirc", 18); funMap.Add("easeInOutCirc", 19);
        funMap.Add("easeInElastic", 20); funMap.Add("easeOutElastic", 21); funMap.Add("easeInOutElastic", 22);
        funMap.Add("easeInBack", 23); funMap.Add("easeOutBack", 24); funMap.Add("easeInOutBack", 25);
        funMap.Add("easeInBounce", 26); funMap.Add("easeOutBounce", 27); funMap.Add("easeInOutBounce", 28);
    }

    // Update is called once per frame
    private void Update ()
    {
		
	}
    public void sendButtonOnClick()
    {
        int RselectedNum = 0;
        int LselectedNum = 0;
        RselectedNum = RightDropdown.GetComponent<Dropdown>().value;
        LselectedNum = LeftDropdown.GetComponent<Dropdown>().value;

        int dur = int.Parse(DurationField.text);//Catch the duration
        double Rdegree = 0;
        double Ldegree = 0;
        String data = "";
        
        for (int pos = 0; pos < dur; pos++)
        {
            Rdegree = degreeCal(RightOptions[RselectedNum].text, pos, RStartDegree, RMoveDegree, dur);
            Ldegree = degreeCal(LeftOptions[LselectedNum].text, pos, LStartDegree, LMoveDegree, dur);
            data = Rdegree.ToString("f2") + " " + Ldegree.ToString("f2");
            SendData(data);
            Thread.Sleep(100);
        }

        Thread.Sleep(50); //wait a second, then move back using "bounce" easing
        data = RStartDegree.ToString("f2") + " " + LStartDegree.ToString("f2");
       SendData(data);
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
                        if (dev.StartsWith("/dev/tty.")){
                            serial_ports.Add(dev);
                            Debug.Log (String.Format (dev));
                        }
                    }
                }
                //controller = new SerialPort ("/dev/tty.usbmodem1411");
                portChoice = "/dev/" + choice;
                //Debug.Log(portChoice);
                //arduinoController = new SerialPort(portChoice);
            }
            arduinoController =new SerialPort(portChoice, 115200, Parity.None, 8, StopBits.One);
            arduinoController.Handshake = Handshake.None;
            arduinoController.RtsEnable = true;
            arduinoController.Open();

            Thread.Sleep(50);
            SendData(RStartDegree.ToString("f2") + " " + LStartDegree.ToString("f2"));
        }
    }

    void SendData(String data)
    {
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
                    Debug.Log("nullport");
                }
            }
    }
    // simple linear tweening - no easing
    // t: current time, b: beginning value, c: change in value, d: duration
    double linearTween(double t, double b, double c, double d)
    {
        return c * t / d + b;
    }


    ///////////// QUADRATIC EASING: t^2 ///////////////////

    // quadratic easing in - accelerating from zero velocity
    // t: current time, b: beginning value, c: change in value, d: duration
    // t and d can be in frames or seconds/milliseconds
    double easeInQuad(double t, double b, double c, double d)
    {
        return c * (t /= d) * t + b;
    }

    // quadratic easing out - decelerating to zero velocity
    double easeOutQuad(double t, double b, double c, double d)
    {
        return -c * (t /= d) * (t - 2) + b;
    }

    // quadratic easing in/out - acceleration until halfway, then deceleration
    double easeInOutQuad(double t, double b, double c, double d)
    {
        if ((t /= d / 2) < 1) return c / 2 * t * t + b;
        return -c / 2 * ((--t) * (t - 2) - 1) + b;
    }


    ///////////// CUBIC EASING: t^3 ///////////////////////

    // cubic easing in - accelerating from zero velocity
    // t: current time, b: beginning value, c: change in value, d: duration
    // t and d can be frames or seconds/milliseconds
    double easeInCubic(double t, double b, double c, double d)
    {
        return c * (t /= d) * t * t + b;
    }

    // cubic easing out - decelerating to zero velocity
    double easeOutCubic(double t, double b, double c, double d)
    {
        return c * ((t = t / d - 1) * t * t + 1) + b;
    }

    // cubic easing in/out - acceleration until halfway, then deceleration
    double easeInOutCubic(double t, double b, double c, double d)
    {
        if ((t /= d / 2) < 1) return c / 2 * t * t * t + b;
        return c / 2 * ((t -= 2) * t * t + 2) + b;
    }


    ///////////// QUARTIC EASING: t^4 /////////////////////

    // quartic easing in - accelerating from zero velocity
    // t: current time, b: beginning value, c: change in value, d: duration
    // t and d can be frames or seconds/milliseconds
    double easeInQuart(double t, double b, double c, double d)
    {
        return c * (t /= d) * t * t * t + b;
    }

    // quartic easing out - decelerating to zero velocity
    double easeOutQuart(double t, double b, double c, double d)
    {
        return -c * ((t = t / d - 1) * t * t * t - 1) + b;
    }

    // quartic easing in/out - acceleration until halfway, then deceleration
    double easeInOutQuart(double t, double b, double c, double d)
    {
        if ((t /= d / 2) < 1) return c / 2 * t * t * t * t + b;
        return -c / 2 * ((t -= 2) * t * t * t - 2) + b;
    }


    ///////////// QUINTIC EASING: t^5  ////////////////////

    // quintic easing in - accelerating from zero velocity
    // t: current time, b: beginning value, c: change in value, d: duration
    // t and d can be frames or seconds/milliseconds
    double easeInQuint(double t, double b, double c, double d)
    {
        return c * (t /= d) * t * t * t * t + b;
    }

    // quintic easing out - decelerating to zero velocity
    double easeOutQuint(double t, double b, double c, double d)
    {
        return c * ((t = t / d - 1) * t * t * t * t + 1) + b;
    }

    // quintic easing in/out - acceleration until halfway, then deceleration
    double easeInOutQuint(double t, double b, double c, double d)
    {
        if ((t /= d / 2) < 1) return c / 2 * t * t * t * t * t + b;
        return c / 2 * ((t -= 2) * t * t * t * t + 2) + b;
    }



    ///////////// SINUSOIDAL EASING: sin(t) ///////////////

    // sinusoidal easing in - accelerating from zero velocity
    // t: current time, b: beginning value, c: change in position, d: duration
    double easeInSine(double t, double b, double c, double d)
    {
        return -c * System.Math.Cos(t / d * (Math.PI / 2)) + c + b;
    }

    // sinusoidal easing out - decelerating to zero velocity
    double easeOutSine(double t, double b, double c, double d)
    {
        return c * Math.Sin(t / d * (Math.PI / 2)) + b;
    }

    // sinusoidal easing in/out - accelerating until halfway, then decelerating
    double easeInOutSine(double t, double b, double c, double d)
    {
        return -c / 2 * (System.Math.Cos(Math.PI * t / d) - 1) + b;
    }


    ///////////// EXPONENTIAL EASING: 2^t /////////////////

    // exponential easing in - accelerating from zero velocity
    // t: current time, b: beginning value, c: change in position, d: duration
    double easeInExpo(double t, double b, double c, double d)
    {
        return (t == 0) ? b : c * Math.Pow(2, 10 * (t / d - 1)) + b;
    }

    // exponential easing out - decelerating to zero velocity
    double easeOutExpo(double t, double b, double c, double d)
    {
        return (t == d) ? b + c : c * (-Math.Pow(2, -10 * t / d) + 1) + b;
    }

    // exponential easing in/out - accelerating until halfway, then decelerating
    double easeInOutExpo(double t, double b, double c, double d)
    {
        if (t == 0) return b;
        if (t == d) return b + c;
        if ((t /= d / 2) < 1) return c / 2 * Math.Pow(2, 10 * (t - 1)) + b;
        return c / 2 * (-Math.Pow(2, -10 * --t) + 2) + b;
    }


    /////////// CIRCULAR EASING: Math.Sqrt(1-t^2) //////////////

    // circular easing in - accelerating from zero velocity
    // t: current time, b: beginning value, c: change in position, d: duration
    double easeInCirc(double t, double b, double c, double d)
    {
        return -c * (Math.Sqrt(1 - (t /= d) * t) - 1) + b;
    }

    // circular easing out - decelerating to zero velocity
    double easeOutCirc(double t, double b, double c, double d)
    {
        return c * Math.Sqrt(1 - (t = t / d - 1) * t) + b;
    }

    // circular easing in/out - acceleration until halfway, then deceleration
    double easeInOutCirc(double t, double b, double c, double d)
    {
        if ((t /= d / 2) < 1) return -c / 2 * (Math.Sqrt(1 - t * t) - 1) + b;
        return c / 2 * (Math.Sqrt(1 - (t -= 2) * t) + 1) + b;
    }


    /////////// ELASTIC EASING: exponentially decaying sine wave  //////////////

    // t: current time, b: beginning value, c: change in value, d: duration, a: amplitude (optional), p: period (optional)
    // t and d can be in frames or seconds/milliseconds

    double easeInElastic(double t, double b, double c, double d, double a, double p)
    {
        double s;
        if (t == 0) return b; if ((t /= d) == 1) return b + c; if (p == 0) p = d * .3;
        if (a < Math.Abs(c)) { a = c; s = p / 4; }
        else s = p / (2 * Math.PI) * System.Math.Asin(c / a);
        return -(a * Math.Pow(2, 10 * (t -= 1)) * System.Math.Sin((t * d - s) * (2 * Math.PI) / p)) + b;
    }

    double easeOutElastic(double t, double b, double c, double d, double a, double p)
    {
        double s;
        if (t == 0) return b; if ((t /= d) == 1) return b + c; if (p == 0) p = d * .3;
        if (a < Math.Abs(c)) { a = c; s = p / 4; }
        else s = p / (2 * Math.PI) * Math.Asin(c / a);
        return a * Math.Pow(2, -10 * t) * Math.Sin((t * d - s) * (2 * Math.PI) / p) + c + b;
    }

    double easeInOutElastic(double t, double b, double c, double d, double a, double p)
    {
        double s;
        if (t == 0) return b; if ((t /= d / 2) == 2) return b + c; if (p == 0) p = d * (.3 * 1.5);
        if (a < Math.Abs(c)) { a = c; s = p / 4; }
        else s = p / (2 * Math.PI) * Math.Asin(c / a);
        if (t < 1) return -.5 * (a * Math.Pow(2, 10 * (t -= 1)) * Math.Sin((t * d - s) * (2 * Math.PI) / p)) + b;
        return a * Math.Pow(2, -10 * (t -= 1)) * Math.Sin((t * d - s) * (2 * Math.PI) / p) * .5 + c + b;
    }


    //Four parameter versions
    double easeInElastic(double t, double b, double c, double d)
    {
        double s;
        double a = 0.0;
        double p = 0.0;
        if (t == 0) return b; if ((t /= d) == 1) return b + c; if (p == 0) p = d * .3;
        if (a < Math.Abs(c)) { a = c; s = p / 4; }
        else s = p / (2 * Math.PI) * Math.Asin(c / a);
        return -(a * Math.Pow(2, 10 * (t -= 1)) * System.Math.Sin((t * d - s) * (2 * Math.PI) / p)) + b;
    }

    double easeOutElastic(double t, double b, double c, double d)
    {
        double s;
        double a = 0.0;
        double p = 0.0;
        if (t == 0) return b; if ((t /= d) == 1) return b + c; if (p == 0) p = d * .3;
        if (a < Math.Abs(c)) { a = c; s = p / 4; }
        else s = p / (2 * Math.PI) * Math.Asin(c / a);
        return a * Math.Pow(2, -10 * t) * System.Math.Sin((t * d - s) * (2 * Math.PI) / p) + c + b;
    }

    double easeInOutElastic(double t, double b, double c, double d)
    {
        double s;
        double a = 0.0;
        double p = 0.0;
        if (t == 0) return b; if ((t /= d / 2) == 2) return b + c; if (p == 0) p = d * (.3 * 1.5);
        if (a < Math.Abs(c)) { a = c; s = p / 4; }
        else s = p / (2 * Math.PI) * Math.Asin(c / a);
        if (t < 1) return -.5 * (a * Math.Pow(2, 10 * (t -= 1)) * Math.Sin((t * d - s) * (2 * Math.PI) / p)) + b;
        return a * Math.Pow(2, -10 * (t -= 1)) * Math.Sin((t * d - s) * (2 * Math.PI) / p) * .5 + c + b;
    }

    /////////// BACK EASING: overshooting cubic easing: (s+1)*t^3 - s*t^2  //////////////

    // back easing in - backtracking slightly, then reversing direction and moving to target
    // t: current time, b: beginning value, c: change in value, d: duration, s: overshoot amount (optional)
    // t and d can be in frames or seconds/milliseconds
    // s controls the amount of overshoot: higher s means greater overshoot
    // s has a default value of 1.70158, which produces an overshoot of 10 percent
    // s==0 produces cubic easing with no overshoot
    double easeInBack(double t, double b, double c, double d, double s)
    {
        return c * (t /= d) * t * ((s + 1) * t - s) + b;
    }

    // back easing out - moving towards target, overshooting it slightly, then reversing and coming back to target
    double easeOutBack(double t, double b, double c, double d, double s)
    {
        return c * ((t = t / d - 1) * t * ((s + 1) * t + s) + 1) + b;
    }

    // back easing in/out - backtracking slightly, then reversing direction and moving to target,
    // then overshooting target, reversing, and finally coming back to target
    double easeInOutBack(double t, double b, double c, double d, double s)
    {
        if ((t /= d / 2) < 1) return c / 2 * (t * t * (((s *= (1.525)) + 1) * t - s)) + b;
        return c / 2 * ((t -= 2) * t * (((s *= (1.525)) + 1) * t + s) + 2) + b;
    }


    //Four parameter versions
    double easeInBack(double t, double b, double c, double d)
    {
        double s = 1.70158;
        return c * (t /= d) * t * ((s + 1) * t - s) + b;
    }

    double easeOutBack(double t, double b, double c, double d)
    {
        double s = 1.70158;
        return c * ((t = t / d - 1) * t * ((s + 1) * t + s) + 1) + b;
    }

    double easeInOutBack(double t, double b, double c, double d)
    {
        double s = 1.70158;
        if ((t /= d / 2) < 1) return c / 2 * (t * t * (((s *= (1.525)) + 1) * t - s)) + b;
        return c / 2 * ((t -= 2) * t * (((s *= (1.525)) + 1) * t + s) + 2) + b;
    }

    /////////// BOUNCE EASING: exponentially decaying parabolic bounce  //////////////

    // bounce easing in
    // t: current time, b: beginning value, c: change in position, d: duration
    double easeInBounce(double t, double b, double c, double d)
    {
        return c - easeOutBounce(d - t, 0, c, d) + b;
    }

    // bounce easing out
    double easeOutBounce(double t, double b, double c, double d)
    {
        if ((t /= d) < (1 / 2.75))
        {
            return c * (7.5625 * t * t) + b;
        }
        else if (t < (2 / 2.75))
        {
            return c * (7.5625 * (t -= (1.5 / 2.75)) * t + .75) + b;
        }
        else if (t < (2.5 / 2.75))
        {
            return c * (7.5625 * (t -= (2.25 / 2.75)) * t + .9375) + b;
        }
        else
        {
            return c * (7.5625 * (t -= (2.625 / 2.75)) * t + .984375) + b;
        }
    }

    // bounce easing in/out
    double easeInOutBounce(double t, double b, double c, double d)
    {
        if (t < d / 2) return easeInBounce(t * 2, 0, c, d) * .5 + b;
        return easeOutBounce(t * 2 - d, 0, c, d) * .5 + c * .5 + b;
    }

    double degreeCal(string funName, float t, float b, float c, float d)
    {
        double answer = 0.0;
        switch (funMap[funName])
        {
            
            case 0:
                answer = b;
                break;
            case 1:
                answer = linearTween(t, b, c, d);
                break;
            case 2:
                answer = easeInQuad(t, b, c, d);
                break;
            case 3:
                answer = easeOutQuad(t, b, c, d);
                break;
            case 4:
                answer = easeInOutQuad(t, b, c, d);
                break;
            case 5:
                answer = easeInCubic(t, b, c, d);
                break;
            case 6:
                answer = easeOutCubic(t, b, c, d);
                break;
            case 7:
                answer = easeInOutCubic(t, b, c, d);
                break;
            case 8:
                answer = easeInQuart(t, b, c, d);
                break;
            case 9:
                answer = easeOutQuart(t, b, c, d);
                break;
            case 10:
                answer = easeInOutQuart(t, b, c, d);
                break;
            case 11:
                answer = easeInSine(t, b, c, d);
                break;
            case 12:
                answer = easeOutSine(t, b, c, d);
                break;
            case 13:
                answer = easeInOutSine(t, b, c, d);
                break;
            case 14:
                answer = easeInExpo(t, b, c, d);
                break;
            case 15:
                answer = easeOutExpo(t, b, c, d);
                break;
            case 16:
                answer = easeInOutExpo(t, b, c, d);
                break;
            case 17:
                answer = easeInCirc(t, b, c, d);
                break;
            case 18:
                answer = easeOutCirc(t, b, c, d);
                break;
            case 19:
                answer = easeInOutCirc(t, b, c, d);
                break;
            case 20:
                answer = easeInElastic(t, b, c, d);
                break;
            case 21:
                answer = easeOutElastic(t, b, c, d);
                break;
            case 22:
                answer = easeInOutElastic(t, b, c, d);
                break;
            case 23:
                answer = easeInBack(t, b, c, d);
                break;
            case 24:
                answer = easeOutBack(t, b, c, d);
                break;
            case 25:
                answer = easeInOutBack(t, b, c, d);
                break;
            case 26:
                answer = easeInBounce(t, b, c, d);
                break;
            case 27:
                answer = easeOutBounce(t, b, c, d);
                break;
            case 28:
                answer = easeInOutBounce(t, b, c, d);
                break;
            case 29:
                answer = easeInQuint(t, b, c, d);
                break;
            case 30:
                answer = easeOutQuint(t, b, c, d);
                break;
            case 31:
                answer = easeInOutQuint(t, b, c, d);
                break;

        }
        return answer;
    }

}
