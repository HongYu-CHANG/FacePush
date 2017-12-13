using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoContoller : MonoBehaviour {

	//public GameObject Video;
	public VideoPlayer videoPlayer;

    public GameObject RMotor;
    public GameObject LMotor;
    private bool In = false;
    private OSCSender ROSCSender;
    private OSCSender LOSCSender;
	
	// Use this for initialization
	void Start () 
	{
    	Debug.Log("Start play Video");
    	ROSCSender = RMotor.GetComponent<OSCSender>();
    	ROSCSender.setWhichMotor("R");
    	LOSCSender = LMotor.GetComponent<OSCSender>();
    	LOSCSender.setWhichMotor("L");
	}
	
	// Update is called once per frame
	void Update () 
	{
		
		int videoTime = Mathf.FloorToInt((float)videoPlayer.time);
		Debug.LogWarning("Video Time: " + videoTime);
		
		if(videoTime == 6 && !In)
		{
            ROSCSender.SendOSCMessageTriggerMethod("FORWARD", 100, 10);
            LOSCSender.SendOSCMessageTriggerMethod("FORWARD", 10, 10);
            Debug.LogWarning("Video Time: " + videoTime);
            In = true;
		}
		else if(videoTime == 10  && In)
		{
            Debug.LogWarning("Video Time: " + videoTime);
            In = false;
		}
        
		else if(videoTime == 19 && !In)
		{
            Debug.LogWarning("Video Time: " + videoTime);
            In = true;
		}
        
		else if(videoTime == 27 && In)
		{
            Debug.LogWarning("Video Time: " + videoTime);
            In = false;
		}
	}

}

