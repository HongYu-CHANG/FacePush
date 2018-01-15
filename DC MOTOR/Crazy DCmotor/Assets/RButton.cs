using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RButton : MonoBehaviour 
	,IPointerDownHandler
	,IPointerUpHandler
	,IPointerExitHandler
{
	public int MotorSpeed;
	public GameObject RMotor;
	public Slider Rmotor_Speed;
	public Text RText;
	private int RSpeed;
	private OSCSender ROSCSender;

	void Start () 
	{
		ROSCSender = RMotor.GetComponent<OSCSender>();
    	ROSCSender.setWhichMotor("R");	
	}
	
	// Update is called once per frame
	void Update () 
	{
		RSpeed = (int)Rmotor_Speed.value;
		RText.text = Rmotor_Speed.value + " / " + NumberToRPM (Rmotor_Speed.value).ToString("0.00") + " RPM";
	}

	private double NumberToRPM (float sliderValue)
	{
		return sliderValue*MotorSpeed/255;
	}

	public void OnPointerDown(PointerEventData eventData)
	{
  		Debug.Log("RDown");
  		ROSCSender.SendOSCMessageTriggerMethod("FORWARD", RSpeed, 1);
 	}
 	public void OnPointerUp(PointerEventData eventData)
 	{
  		Debug.Log("RUp");
  		ROSCSender.SendOSCMessageTriggerMethod("BACKWARD", RSpeed, 1);
 	}
 	public void OnPointerExit(PointerEventData eventData)
 	{
 	 	Debug.Log("LExit");
 	 	ROSCSender.SendOSCMessageTriggerMethod("RELEASE", 255, 2);
 	}
}
