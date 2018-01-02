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
    public InputField No1Input_Time;
    public InputField No2Input_Number;
    public InputField No2Input_Time;
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
	}
	
	// Update is called once per frame
	void Update () 
	{
		RSpeed = (int)Rmotor_Power.value;
		LSpeed = (int)Lmotor_Power.value;
		RText.text = Rmotor_Power.value + " / " + NumberToRPM (Rmotor_Power.value).ToString("0.00") + " RPM";
		LText.text = Lmotor_Power.value + " / " + NumberToRPM (Lmotor_Power.value).ToString("0.00") + " RPM";
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
	    int time = int.Parse(No1Input_Time.text);
	    for(int i = 0 ; i < int.Parse(No1Input_Number.text) ; i++)
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
	    int time = int.Parse(No2Input_Time.text);
	    if(RSpeed < 100) RSpeed = 100;
	    if(LSpeed < 100) LSpeed = 100;
	    for(int i = 0 ; i < int.Parse(No2Input_Number.text) ; i++)
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
}
