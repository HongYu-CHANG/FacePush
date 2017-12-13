using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OSCsharp.Data;
using UniOSC;

public class OSCSender : UniOSCEventDispatcher
{
    private int totalMove;
    private string whichMotor;

    public override void Awake()
    {
        base.Awake();
        totalMove = 0;
    }

    public override void OnEnable()
    {
        //Here we setup our OSC message
        base.OnEnable();
        ClearData();
        //now we could add data;
        AppendData("0");//哪顆馬達
        AppendData("0");//旋轉方向
        AppendData(1);//旋轉速度
        AppendData(2);//旋轉時間

    }
    public override void OnDisable()
    {
        //Don't forget this!!!!
        base.OnDisable();
    }
    public int getMove(){return totalMove;}

    public void setWhichMotor(string whichMotor){this.whichMotor = whichMotor;}

    public void SendOSCMessageTriggerMethod(string direction, int speed, int time)
    {
        if (_OSCeArg.Packet is OscMessage)
        {
            OscMessage msg = ((OscMessage)_OSCeArg.Packet);
            if(direction != "RESET")
                _updateOscMessageData(msg, direction, speed, time);
            else
                reset(msg);

        }
        _SendOSCMessage(_OSCeArg);
    }

    private void _updateOscMessageData(OscMessage msg, string direction, int speed, int time)
    {
        if(direction == "FORWARD")
        {
            totalMove += speed * time;
        }
        else if (direction == "REVERSE")
        {
            totalMove -= speed * time;
        }

        msg.UpdateDataAt(0, whichMotor);
        msg.UpdateDataAt(1, direction);
        msg.UpdateDataAt(2, speed);
        msg.UpdateDataAt(3, time);

    }
    
    private void reset(OscMessage msg)
    {
        int temp = totalMove/255;
        msg.UpdateDataAt(0, whichMotor);
        if(totalMove > 0)
        {
            totalMove = 0;
            msg.UpdateDataAt(1, "REVERSE");
            msg.UpdateDataAt(2, 255);
            msg.UpdateDataAt(3, temp);

        }
        else if (totalMove < 0)
        {
            totalMove = 0;
            msg.UpdateDataAt(1, "FORWARD");
            msg.UpdateDataAt(2, 255);
            msg.UpdateDataAt(3, temp);
        }
        else if (totalMove == 0)
        {
            msg.UpdateDataAt(1, "RELEASE");
            msg.UpdateDataAt(2, 0);
            msg.UpdateDataAt(3, 10);
        }            
    }
}


