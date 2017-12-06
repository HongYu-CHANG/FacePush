using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OSCsharp.Data;
using UniOSC;

public class OSCSender : UniOSCEventDispatcher
{
    public Slider mainSlider;

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
        AppendData(123);//
        //AppendData(123f);
        //AppendData("MyString");
    }
    public override void OnDisable()
    {
        //Don't forget this!!!!
        base.OnDisable();

    }

    public void MySendOSCMessageTriggerMethod(){
        
        //Here we update the data with a new value
        //OscMessage msg = null;
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
        msg.UpdateDataAt(0, (int)mainSlider.value);
        //msg.UpdateDataAt(1, dynamicFloatValue);
        //msg.UpdateDataAt(2, dynamicStringValue);
    }
	
}
