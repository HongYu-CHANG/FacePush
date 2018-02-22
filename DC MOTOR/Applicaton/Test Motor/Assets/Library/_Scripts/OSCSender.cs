using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OSCsharp.Data;
using UniOSC;

public class OSCSender : UniOSCEventDispatcher
{
    private string whichMotor;

    public override void Awake()
    {
        base.Awake();
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

    public void setWhichMotor(string whichMotor){this.whichMotor = whichMotor;}

    public void SendOSCMessageTriggerMethod(int degree, int speed)
    {
        if (_OSCeArg.Packet is OscMessage)
        {
            OscMessage msg = ((OscMessage)_OSCeArg.Packet);
            _updateOscMessageData(msg, degree, speed);
            
        }
        _SendOSCMessage(_OSCeArg);
    }

    private void _updateOscMessageData(OscMessage msg, int degree, int speed)
    {
        msg.UpdateDataAt(0, whichMotor);
        msg.UpdateDataAt(1, degreeConvertToRotaryCoder(degree));
        msg.UpdateDataAt(2, speed);

    }

    private int degreeConvertToRotaryCoder(int degree)
    {
        return (degree * 1024/360);
    }
}


