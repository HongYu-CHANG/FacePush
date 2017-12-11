using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OSCsharp.Data;
using UniOSC;

public class OSCSender : UniOSCEventDispatcher
{
    public InputField Command;
    private DCMotor RMotor;

    public override void Awake()
    {
        base.Awake();
        RMotor = new DCMotor();
    }

    public override void OnEnable()
    {
        //Here we setup our OSC message
        base.OnEnable();
        ClearData();
        //now we could add data;
        //右
        AppendData(0);//旋轉方向
        AppendData(0);//旋轉速度
        AppendData(0);//旋轉時間
        //左
        AppendData(0);//旋轉方向
        AppendData(0);//旋轉速度
        AppendData(0);//旋轉時間

    }
    public override void OnDisable()
    {
        //Don't forget this!!!!
        base.OnDisable();

    }

    public void MySendOSCMessageTriggerMethod(){
        
        Debug.Log(RMotor.handleCommand("FORWARD", "100", "10"));
        Debug.Log(RMotor.getTotalMove());
        Debug.Log(RMotor.handleCommand("REVERSE", "10", "10"));
        Debug.Log(RMotor.getTotalMove());
        Debug.Log(RMotor.reset());
        if (_OSCeArg.Packet is OscMessage)
        {
            //message
            OscMessage msg = ((OscMessage)_OSCeArg.Packet);
            _updateOscMessageData(msg);

        }
        else if (_OSCeArg.Packet is OscBundle)
        {
            //bundle 
            foreach (OscMessage msg2 in ((OscBundle)_OSCeArg.Packet).Messages)
            {
                _updateOscMessageData(msg2);
            }
        }

        //Here we trigger the sending
        _SendOSCMessage(_OSCeArg);
    }

    private void _updateOscMessageData(OscMessage msg)
    {
        msg.UpdateDataAt(0, -1);
        msg.UpdateDataAt(1, 100);
        msg.UpdateDataAt(2, 10);

    }

     class DCMotor
    {
        private int totalMove;
        public DCMotor()
        {
            totalMove = 0;
        }
        public int getTotalMove(){return totalMove;}
        public string handleCommand(string runCommand, string speedCommand, string timeCommand)
        {
            if(runCommand == "FORWARD")
            {
                totalMove += (int.Parse(speedCommand) * int.Parse(timeCommand));
            }
            else if (runCommand == "REVERSE")
            {
                totalMove -= (int.Parse(speedCommand) * int.Parse(timeCommand));
            }

            return runCommand+"-"+speedCommand+"-"+timeCommand;

        }

        public string reset()
        {
            int temp = totalMove/255;
            if(totalMove > 0)
            {
                totalMove = 0;
                return "REVERSE-"+255+"-"+(temp);
            }
            else if (totalMove < 0)
            {
                totalMove = 0;
                return "FORWARD-"+255+"-"+(temp);
            }
            return "RELEASE-0-10";
        }
    }
}


