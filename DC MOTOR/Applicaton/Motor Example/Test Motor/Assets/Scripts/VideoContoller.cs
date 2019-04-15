using System;
using System.Collections;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoContoller : MonoBehaviour {

	//public GameObject Video;
	public VideoPlayer videoPlayer;
	public bool stop = false;

    public GameObject RMotor;
    public GameObject LMotor;
    private bool In = false;
    private int RMove = 0;
    private int LMove =0;
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
	IEnumerator Example()
    {
        //Debug.Log(Time.time);
        yield return new WaitForSeconds(4);
        stop = true;
    }
	// Update is called once per frame
	void Update () 
	{
		
		int videoTime = Mathf.FloorToInt((float)videoPlayer.time);
		Debug.LogWarning("Video Time: " + videoTime);
		
		if(videoTime == 5 && !In)
		{
            ROSCSender.SendOSCMessageTriggerMethod(10, 100);//加壓
            LOSCSender.SendOSCMessageTriggerMethod(10, 100);
            Debug.LogWarning("Video Time: " + videoTime);
            In = true;
		}
		else if(videoTime == 7  && In)
		{
            ROSCSender.SendOSCMessageTriggerMethod(170, 100);
            LOSCSender.SendOSCMessageTriggerMethod(170, 100);
            Debug.LogWarning("Video Time: " + videoTime);
            In = false;
		}
        
		else if(videoTime == 10 && !In)
		{
            
            ROSCSender.SendOSCMessageTriggerMethod(10, 200);
            LOSCSender.SendOSCMessageTriggerMethod(10, 200);
            Debug.LogWarning("Video Time: " + videoTime);
            In = true;
		}
        else if(videoTime == 15 && In)
		{
            
            ROSCSender.SendOSCMessageTriggerMethod(170, 200);
            LOSCSender.SendOSCMessageTriggerMethod(170, 200);
            Debug.LogWarning("Video Time: " + videoTime);
            In = false;
		}
		else if(videoTime == 17 && !In)
		{
            
            ROSCSender.SendOSCMessageTriggerMethod(10, 200);
            LOSCSender.SendOSCMessageTriggerMethod(10, 200);
            Debug.LogWarning("Video Time: " + videoTime);
            In = true;
		}
		else if(videoTime == 22 && In)
		{
        
            Debug.LogWarning("Video Time: " + videoTime);
            In = false;
		}
		else if(videoTime == 27 && !In)
		{
            
            ROSCSender.SendOSCMessageTriggerMethod(10, 255);
            LOSCSender.SendOSCMessageTriggerMethod(10, 255);
            Debug.LogWarning("Video Time: " + videoTime);
            In = true;
		}
		else if(videoTime == 29 && In)
		{
            
           
            Debug.LogWarning("Video Time: " + videoTime);
            In = false;
		}
		if(stop && videoTime == 31)
		{
		}
		else if(!stop && videoTime == 31)
		{
			
			ROSCSender.SendOSCMessageTriggerMethod(170, 255);
            LOSCSender.SendOSCMessageTriggerMethod(170, 255);
            StartCoroutine(Example());
		}
	}

	

}

