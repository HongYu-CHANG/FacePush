using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class DCMotorTeacher : MonoBehaviour {

	public int MotorSpeed;
	public GameObject RMotor;
    public GameObject LMotor;
    public Slider Lmotor_Speed;
    public Slider Lmotor_Time;
    public Slider Rmotor_Speed;
    public Slider Rmotor_Time;
    public Text LTextSpeed;
    public Text LTextTime;
    public Text RTextSpeed;
    public Text RTextTime;
    private int LSpeed;
    private int RSpeed;
    private OSCSender ROSCSender;
    private OSCSender LOSCSender;
    private bool Rbutton = false;
    private bool Lbutton = false;

	// Use this for initialization
	void Start () 
	{
		ROSCSender = RMotor.GetComponent<OSCSender>();
    	ROSCSender.setWhichMotor("R");
    	LOSCSender = LMotor.GetComponent<OSCSender>();
    	LOSCSender.setWhichMotor("L");
	}
	
	// Update is called once per frame
	void Update () 
	{
		RSpeed = (int)Rmotor_Speed.value;
		LSpeed = (int)Lmotor_Speed.value;
		Lmotor_Time.maxValue = 1400 / Lmotor_Speed.value;
		Rmotor_Time.maxValue = 1400 / Rmotor_Speed.value;
		RTextSpeed.text = Rmotor_Speed.value + " / " + NumberToRPM (Rmotor_Speed.value).ToString("0.00") + " RPM";
		RTextTime.text = Rmotor_Time.value.ToString("0.00") + "S";
		LTextSpeed.text = Lmotor_Speed.value + " / " + NumberToRPM (Lmotor_Speed.value).ToString("0.00") + " RPM";
		LTextTime.text = Lmotor_Time.value.ToString("0.00") + "S";
	}

	private double NumberToRPM (float sliderValue)
	{
		return sliderValue*MotorSpeed/255;
	}

	public void No1R()
	{

        Rbutton = !Rbutton;
        StartCoroutine(No1Work(true, false, Rbutton));
	}

	public void No1L()
	{
		Rbutton = !Rbutton;
        StartCoroutine(No1Work(false, true, Rbutton));
	}

	IEnumerator No1Work(bool R, bool L, bool click)
	{
		float time;
		if(R)
			time = Rmotor_Time.value;
		else
			time = Lmotor_Time.value;
		if(click)//奇數次點擊
		{
			if(R)ROSCSender.SendOSCMessageTriggerMethod(100, RSpeed);//加壓
	        if(L)LOSCSender.SendOSCMessageTriggerMethod(100, LSpeed);
	        yield return new WaitForSeconds(time);
    	}
    	else
    	{
    		if(R)ROSCSender.SendOSCMessageTriggerMethod(20, RSpeed);//加壓
	        if(L)LOSCSender.SendOSCMessageTriggerMethod(20, LSpeed);
	        yield return new WaitForSeconds(time);
    	}

	}

}
