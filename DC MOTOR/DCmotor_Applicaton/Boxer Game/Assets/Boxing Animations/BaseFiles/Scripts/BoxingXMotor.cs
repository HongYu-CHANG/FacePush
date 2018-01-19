using System.Collections;
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
	void Update () {
		
	}

	public void Punch(string directionAndSpeed)
    {
        string [] motorParameter= directionAndSpeed.Split(' ');
        print("哪顆馬達" + motorParameter[0] + " 速度: " + motorParameter[1] +  " 秒數: " + float.Parse(motorParameter[2]));
        StartCoroutine(SHAKEWORK(int.Parse(motorParameter[0]), int.Parse(motorParameter[1]), float.Parse(motorParameter[2])));
    }

    IEnumerator SHAKEWORK(int motor, int speed, float second)
	{
		OSCSenderS[motor].SendOSCMessageTriggerMethod("FORWARD", speed , 1);//加壓
		yield return new WaitForSeconds(second);
		OSCSenderS[motor].SendOSCMessageTriggerMethod("BACKWARD", speed, 1);//放鬆
		yield return new WaitForSeconds(second);
		OSCSenderS[motor].SendOSCMessageTriggerMethod("RELEASE", 0, 1);
	}
}
