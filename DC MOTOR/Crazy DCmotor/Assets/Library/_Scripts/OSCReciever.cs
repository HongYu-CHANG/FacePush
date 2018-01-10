using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniOSC;
using OSCsharp.Data;

public class OSCReciever :  UniOSCEventTarget {


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override void OnOSCMessageReceived(UniOSCEventArgs args)
    {
        OscMessage msg = (OscMessage)args.Packet;
    }
}
