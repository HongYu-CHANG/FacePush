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

    public void SendOSCMessageTriggerMethod(string direction, int speed, float time)
    {
        if (_OSCeArg.Packet is OscMessage)
        {
           // Debug.Log(direction);
            OscMessage msg = ((OscMessage)_OSCeArg.Packet);
            _updateOscMessageData(msg, direction, speed);
            
            //_updateOscMessageData(msg, "RELEASE", speed);
        }
        _SendOSCMessage(_OSCeArg);
        timePause(time);
        /*if (_OSCeArg.Packet is OscMessage)
        {
            OscMessage msg = ((OscMessage)_OSCeArg.Packet);
            _updateOscMessageData(msg, "RELEASE", speed);
        }
        _SendOSCMessage(_OSCeArg);*/
    }

    IEnumerator timePause(float time)
    {    
        //_updateOscMessageData(msg, direction, speed);
        yield return new WaitForSeconds(time);
        //_updateOscMessageData(msg, "RELEASE", speed);
    }

    private void _updateOscMessageData(OscMessage msg, string direction, int speed)
    {
        msg.UpdateDataAt(0, whichMotor);
        if(direction == "FORWARD")
        {
            msg.UpdateDataAt(1, 1);
        }
        else if (direction == "BACKWARD")
        {
             msg.UpdateDataAt(1, 2);
        }
        else if (direction == "RELEASE")
        {
            msg.UpdateDataAt(1, 0);
        }
        msg.UpdateDataAt(2, speed);

    }
}


