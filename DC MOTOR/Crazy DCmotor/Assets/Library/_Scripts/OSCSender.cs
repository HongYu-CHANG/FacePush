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
        AppendData(1);//旋轉方向
        AppendData(1);//旋轉速度

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
           // Debug.Log(direction);
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
        msg.UpdateDataAt(0, whichMotor);
        if(direction == "FORWARD")
        {
            totalMove += speed * time;
            msg.UpdateDataAt(1, 1);
        }
        else if (direction == "BACKWARD")
        {
            totalMove -= speed * time;
             msg.UpdateDataAt(1, 2);
        }
        msg.UpdateDataAt(2, speed);

    }
    
    private void reset(OscMessage msg)
    {
        int temp = totalMove/255;
        msg.UpdateDataAt(0, whichMotor);
        if(totalMove > 0)//"BACKWARD"
        {
            totalMove = 0;
            msg.UpdateDataAt(1, 2);
            msg.UpdateDataAt(2, 255);

        }
        else if (totalMove < 0)//"FORWARD"
        {
            totalMove = 0;
            msg.UpdateDataAt(1, 1);
            msg.UpdateDataAt(2, 255);
        }
        else if (totalMove == 0)//"RELEASE"
        {
            msg.UpdateDataAt(1, 0);
            msg.UpdateDataAt(2, 0);
        }            
    }
}


