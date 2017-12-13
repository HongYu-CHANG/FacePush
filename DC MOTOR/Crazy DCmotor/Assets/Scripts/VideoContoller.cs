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
		
		if(videoTime == 5 && !In)
		{
            ROSCSender.SendOSCMessageTriggerMethod("FORWARD", 100, 2);//加壓
            LOSCSender.SendOSCMessageTriggerMethod("FORWARD", 100, 2);
            Debug.LogWarning("Video Time: " + videoTime);
            In = true;
		}
		else if(videoTime == 7  && In)
		{
            ROSCSender.SendOSCMessageTriggerMethod("REVERSE", 100, 1);
            LOSCSender.SendOSCMessageTriggerMethod("REVERSE", 100, 1);
            Debug.LogWarning("Video Time: " + videoTime);
            In = false;
		}
        
		else if(videoTime == 10 && !In)
		{
            
            ROSCSender.SendOSCMessageTriggerMethod("FORWARD", 200, 5);
            LOSCSender.SendOSCMessageTriggerMethod("FORWARD", 100, 5);
            Debug.LogWarning("Video Time: " + videoTime);
            In = true;
		}
        else if(videoTime == 15 && In)
		{
            
            ROSCSender.SendOSCMessageTriggerMethod("REVERSE", 200, 2);
            LOSCSender.SendOSCMessageTriggerMethod("REVERSE", 200, 2);
            Debug.LogWarning("Video Time: " + videoTime);
            In = false;
		}
		else if(videoTime == 17 && !In)
		{
            
            ROSCSender.SendOSCMessageTriggerMethod("FORWARD", 200, 8);
            LOSCSender.SendOSCMessageTriggerMethod("FORWARD", 200, 8);
            Debug.LogWarning("Video Time: " + videoTime);
            In = true;
		}
		else if(videoTime == 25 && In)
		{
            
            ROSCSender.SendOSCMessageTriggerMethod("RELEASE", 200, 2);
            LOSCSender.SendOSCMessageTriggerMethod("RELEASE", 200, 2);
            Debug.LogWarning("Video Time: " + videoTime);
            In = false;
		}
		else if(videoTime == 27 && !In)
		{
            
            ROSCSender.SendOSCMessageTriggerMethod("FORWARD", 255, 2);
            LOSCSender.SendOSCMessageTriggerMethod("FORWARD", 255, 2);
            Debug.LogWarning("Video Time: " + videoTime);
            In = true;
		}
		Debug.Log(ROSCSender.getMove());
		Debug.Log(LOSCSender.getMove());
	}
	void onDestroy()
	{
		 ROSCSender.SendOSCMessageTriggerMethod("RESET", 255, 2);
         LOSCSender.SendOSCMessageTriggerMethod("RESET", 255, 2);
	}

}

