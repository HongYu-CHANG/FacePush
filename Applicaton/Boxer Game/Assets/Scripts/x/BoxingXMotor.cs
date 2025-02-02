﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxingXMotor : MonoBehaviour {

	public List<OSCSender> OSCSenderS;
	private enum Motors
    {
        Left = 0,
        Right = 1,
		Both = 2,
    }

	// Use this for initialization
	void Start () 
	{
		OSCSenderS[(int)Motors.Left].setWhichMotor("L");
    	OSCSenderS[(int)Motors.Right].setWhichMotor("R");	
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	void OnApplicationQuit()
	{
		OSCSenderS[(int)Motors.Left].SendOSCMessageTriggerMethod( 0, 1);
		OSCSenderS[(int)Motors.Right].SendOSCMessageTriggerMethod( 0, 1);
	}

	public void Punch(string directionAndSpeed)
    {
        string [] motorParameter= directionAndSpeed.Split(' ');
        print("哪顆馬達" + motorParameter[0] + " 角度: " + motorParameter[1] + " 速度: " + motorParameter[2] +  " 秒數: " + float.Parse(motorParameter[3]));
        StartCoroutine(SHAKEWORK(int.Parse(motorParameter[0]), int.Parse(motorParameter[1]), int.Parse(motorParameter[2]), float.Parse(motorParameter[3])));
    }

    IEnumerator SHAKEWORK(int motor, int degree, int speed, float second)
	{
		OSCSenderS[motor].SendOSCMessageTriggerMethod(degree, speed);//加壓
		yield return new WaitForSeconds(second);
		OSCSenderS[motor].SendOSCMessageTriggerMethod(degree, speed);//放鬆
		yield return new WaitForSeconds(second);
		//OSCSenderS[motor].SendOSCMessageTriggerMethod( 0, 1);
	}
    
}
