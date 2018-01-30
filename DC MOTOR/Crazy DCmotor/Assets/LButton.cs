using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LButton : MonoBehaviour 
	,IPointerDownHandler
	,IPointerUpHandler
{
	public int MotorSpeed;
	public GameObject LMotor;
	public Slider Lmotor_Speed;
	public Text LText;
	private int LSpeed;
	private OSCSender LOSCSender;

	void Start () 
	{
		LOSCSender = LMotor.GetComponent<OSCSender>();
    	LOSCSender.setWhichMotor("L");	
	}
	
	// Update is called once per frame
	void Update () 
	{
		LSpeed = (int)Lmotor_Speed.value;
		LText.text = Lmotor_Speed.value + " / " + NumberToRPM (Lmotor_Speed.value).ToString("0.00") + " RPM";
	}

	private double NumberToRPM (float sliderValue)
	{
		return sliderValue*MotorSpeed/255;
	}

	public void OnPointerDown(PointerEventData eventData)
	{
  		Debug.Log("LDown");
  		LOSCSender.SendOSCMessageTriggerMethod(170, LSpeed);
 	}
 	public void OnPointerUp(PointerEventData eventData)
 	{
  		Debug.Log("LUp");
  		LOSCSender.SendOSCMessageTriggerMethod(10, LSpeed);
 	}
}
