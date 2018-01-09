using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniOSC;
using OSCsharp.Data;

public class OSCReciever :  UniOSCEventTarget {

    public GameObject controlObject;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override void OnOSCMessageReceived(UniOSCEventArgs args)
    {
        OscMessage msg = (OscMessage)args.Packet;
        if (msg.Data.Count < 1) return;

        int scale = (int)msg.Data[0];
        controlObject.transform.localScale = new Vector3((float)scale / 1023.0f, (float)scale / 1023.0f, (float)scale / 1023.0f);

    }
}
