using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class DCMotorController : MonoBehaviour {

	public int MotorSpeed;
	public GameObject RMotor;
    public GameObject LMotor;
    public Slider Lmotor_Power;
    public Slider Rmotor_Power;
    public Text LText;
    public Text RText;
    public InputField No1Input_Number;
    public Text No1Time;
    public Slider No1Input_Time;
    public InputField No2Input_Number;
    public Text No2Time;
    public Slider No2Input_Time;
    public InputField No3Input_Number;
    public Text No3Time;
    public Slider No3Input_Time;
    public InputField No4Input_Number;
    public InputField No5Input_Number;
    private int LSpeed;
    private int RSpeed;
    private OSCSender ROSCSender;
    private OSCSender LOSCSender;

	// Use this for initialization
	void Start () 
	{
		ROSCSender = RMotor.GetComponent<OSCSender>();
    	ROSCSender.setWhichMotor("R");
    	LOSCSender = LMotor.GetComponent<OSCSender>();
    	LOSCSender.setWhichMotor("L");
    	No1Input_Number.text = "1";
    	No2Input_Number.text = "1";
    	No3Input_Number.text = "1";
    	No4Input_Number.text = "1";
    	No5Input_Number.text = "1";
	}
	
	// Update is called once per frame
	void Update () 
	{
		No1Time.text = No1Input_Time.value.ToString();
		No2Time.text = No2Input_Time.value.ToString();
		No3Time.text = No3Input_Time.value.ToString();
		RSpeed = (int)Rmotor_Power.value;
		LSpeed = (int)Lmotor_Power.value;
		RText.text = Rmotor_Power.value + " / " + NumberToRPM (Rmotor_Power.value).ToString("0.00") + " RPM";
		LText.text = Lmotor_Power.value + " / " + NumberToRPM (Lmotor_Power.value).ToString("0.00") + " RPM";
	}

	public void stop()
	{
		LOSCSender.SendOSCMessageTriggerMethod("RELEASE", LSpeed, 1);
		ROSCSender.SendOSCMessageTriggerMethod("RELEASE", LSpeed, 1);
	}

	private double NumberToRPM (float sliderValue)
	{
		return sliderValue*MotorSpeed/255;
	}

	public void No1()
	{

        StartCoroutine(No1Work(true, true));
	}

	public void No1R()
	{

        StartCoroutine(No1Work(true, false));
	}

	public void No1L()
	{

        StartCoroutine(No1Work(false, true));
	}

	IEnumerator No1Work(bool R, bool L)
	{
	    float time = No1Input_Time.value;
	    int number = int.Parse(No1Input_Number.text);
	    Debug.Log(R);
	    for(int i = 0 ; i < number ; i++)
		{
			if(R)ROSCSender.SendOSCMessageTriggerMethod("FORWARD", RSpeed, 1);//加壓
            if(L)LOSCSender.SendOSCMessageTriggerMethod("FORWARD", LSpeed, 1);
            yield return new WaitForSeconds(time);
            if(R)ROSCSender.SendOSCMessageTriggerMethod("BACKWARD", RSpeed, 1);//加壓
            if(L)LOSCSender.SendOSCMessageTriggerMethod("BACKWARD", LSpeed, 1);
            yield return new WaitForSeconds(time);
		}
		if(R)ROSCSender.SendOSCMessageTriggerMethod("RELEASE", 255, 2);
        if(L)LOSCSender.SendOSCMessageTriggerMethod("RELEASE", 255, 2);
	}

	public void No2()
	{

        StartCoroutine(No2Work(true, true));
	}

	public void No2R()
	{

        StartCoroutine(No2Work(true, false));
	}

	public void No2L()
	{

        StartCoroutine(No2Work(false, true));
	}

	IEnumerator No2Work(bool R, bool L)
	{
	   float time = No2Input_Time.value;
	   int number = int.Parse(No2Input_Number.text);
	    if(RSpeed < 100) RSpeed = 101;
	    if(LSpeed < 100) LSpeed = 101;
	    for(int i = 0 ; i < number ; i++)
		{
			if(R)ROSCSender.SendOSCMessageTriggerMethod("FORWARD", RSpeed, 1);//加壓
            if(L)LOSCSender.SendOSCMessageTriggerMethod("FORWARD", LSpeed, 1);
            yield return new WaitForSeconds(time);
            if(R)ROSCSender.SendOSCMessageTriggerMethod("BACKWARD", RSpeed-100, 1);//加壓
            if(L)LOSCSender.SendOSCMessageTriggerMethod("BACKWARD", LSpeed-100, 1);
            yield return new WaitForSeconds(time);
		}
		if(R)ROSCSender.SendOSCMessageTriggerMethod("RELEASE", 255, 2);
        if(L)LOSCSender.SendOSCMessageTriggerMethod("RELEASE", 255, 2);
	}

	public void No3R()
	{

        StartCoroutine(No3Work(true, false));
	}

	public void No3L()
	{

        StartCoroutine(No3Work(false, true));
	}

	IEnumerator No3Work(bool R, bool L)
	{
	   float time = No3Input_Time.value;
	   int number = int.Parse(No3Input_Number.text);
	    for(int i = 0 ; i < number ; i++)
		{
			if(R)
			{
				ROSCSender.SendOSCMessageTriggerMethod("FORWARD", RSpeed, 1);//加壓
				yield return new WaitForSeconds(time/2);
				LOSCSender.SendOSCMessageTriggerMethod("FORWARD", LSpeed, 1);
				yield return new WaitForSeconds(time/2);
				ROSCSender.SendOSCMessageTriggerMethod("BACKWARD", RSpeed, 1);//加壓
				yield return new WaitForSeconds(time/2);
				LOSCSender.SendOSCMessageTriggerMethod("BACKWARD", LSpeed, 1);
				yield return new WaitForSeconds(time/2);
			}
			else if(L)
			{
				LOSCSender.SendOSCMessageTriggerMethod("FORWARD", LSpeed, 1);
            	yield return new WaitForSeconds(time/2);
            	ROSCSender.SendOSCMessageTriggerMethod("FORWARD", RSpeed, 1);//加壓
            	yield return new WaitForSeconds(time/2);
            	LOSCSender.SendOSCMessageTriggerMethod("BACKWARD", LSpeed, 1);
            	yield return new WaitForSeconds(time/2);
            	ROSCSender.SendOSCMessageTriggerMethod("BACKWARD", RSpeed, 1);//加壓
            	yield return new WaitForSeconds(time/2);
			}
		}
		ROSCSender.SendOSCMessageTriggerMethod("RELEASE", 255, 2);
        LOSCSender.SendOSCMessageTriggerMethod("RELEASE", 255, 2);
	}

	public void No4()
	{

        StartCoroutine(No4Work(true, true));
	}

	public void No4R()
	{

        StartCoroutine(No4Work(true, false));
	}

	public void No4L()
	{
        StartCoroutine(No4Work(false, true));
	}

	IEnumerator No4Work(bool R, bool L)
	{
	   bool IsR = true;
	   int number = int.Parse(No4Input_Number.text);
	   int DiffSpeed = 1400;
	   if(RSpeed > LSpeed) 
	   {
	   		IsR = true;
	   		DiffSpeed = RSpeed - LSpeed;
	   }
	   	else if(LSpeed > RSpeed)
	   	{
	   		IsR = false;
	   		DiffSpeed = LSpeed - RSpeed;
	   	}
	   	else if (RSpeed == LSpeed)
	   	{
	   		DiffSpeed = 1400;
	   	}

	    for(int i = 0 ; i < number ; i++)
		{
			if(R)ROSCSender.SendOSCMessageTriggerMethod("FORWARD", 70, 1);//加壓
            if(L)LOSCSender.SendOSCMessageTriggerMethod("FORWARD", 70, 1);
            yield return new WaitForSeconds(20);
            if(R)ROSCSender.SendOSCMessageTriggerMethod("BACKWARD", RSpeed, 1);//加壓
            if(L)LOSCSender.SendOSCMessageTriggerMethod("BACKWARD", LSpeed, 1);
            if(IsR)
            {
            	yield return new WaitForSeconds(1400/RSpeed);
            	if(R)ROSCSender.SendOSCMessageTriggerMethod("RELEASE", 255, 2);
            	yield return new WaitForSeconds((1400/DiffSpeed));
            }
            else
            {
            	yield return new WaitForSeconds(1400/LSpeed);
            	if(L)LOSCSender.SendOSCMessageTriggerMethod("RELEASE", 255, 2);
            	yield return new WaitForSeconds(1400/DiffSpeed);
            }
		}
		if(R)ROSCSender.SendOSCMessageTriggerMethod("RELEASE", 255, 2);
        if(L)LOSCSender.SendOSCMessageTriggerMethod("RELEASE", 255, 2);
	}

	public void No5R()
	{

        StartCoroutine(No5Work(true, false));
	}
	public void No5L()
	{

        StartCoroutine(No5Work(false, true));
	}
	IEnumerator No5Work(bool R, bool L)
	{
	   int number = int.Parse(No5Input_Number.text);
	   	for(int i = 0 ; i < number ; i++)
		{
		   if(R)
		   {
		   		float needTime = 1400/RSpeed;
		   		ROSCSender.SendOSCMessageTriggerMethod("FORWARD", RSpeed, 1);
		   		yield return new WaitForSeconds(needTime/2);
		   		LOSCSender.SendOSCMessageTriggerMethod("FORWARD", RSpeed, 1);
		   		yield return new WaitForSeconds(needTime/2);
		   		ROSCSender.SendOSCMessageTriggerMethod("RELEASE", RSpeed, 1);
		   		yield return new WaitForSeconds(needTime/2);
		   		LOSCSender.SendOSCMessageTriggerMethod("RELEASE", RSpeed, 1);
		   		yield return new WaitForSeconds(1);
		   		ROSCSender.SendOSCMessageTriggerMethod("BACKWARD", RSpeed, 1);//加壓
	            LOSCSender.SendOSCMessageTriggerMethod("BACKWARD", RSpeed, 1);
	            yield return new WaitForSeconds(needTime);
	           	LOSCSender.SendOSCMessageTriggerMethod("RELEASE", LSpeed, 1);
		   		ROSCSender.SendOSCMessageTriggerMethod("RELEASE", LSpeed, 1);
		   }
		   else if(L)
		   {
		   		float needTime = 1400/LSpeed;
		   		LOSCSender.SendOSCMessageTriggerMethod("FORWARD", LSpeed, 1);
		   		yield return new WaitForSeconds(needTime/2);
		   		ROSCSender.SendOSCMessageTriggerMethod("FORWARD", LSpeed, 1);
		   		yield return new WaitForSeconds(needTime/2);
		   		LOSCSender.SendOSCMessageTriggerMethod("RELEASE", LSpeed, 1);
		   		yield return new WaitForSeconds(needTime/2);
		   		ROSCSender.SendOSCMessageTriggerMethod("RELEASE", LSpeed, 1);
		   		yield return new WaitForSeconds(1);
		   		ROSCSender.SendOSCMessageTriggerMethod("BACKWARD", LSpeed, 1);//加壓
	            LOSCSender.SendOSCMessageTriggerMethod("BACKWARD", LSpeed, 1);
	            yield return new WaitForSeconds(needTime);
	           	LOSCSender.SendOSCMessageTriggerMethod("RELEASE", LSpeed, 1);
		   		ROSCSender.SendOSCMessageTriggerMethod("RELEASE", LSpeed, 1);
		   }
		}
	}
}
