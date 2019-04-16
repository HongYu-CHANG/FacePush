using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;

public class FacePushController : MonoBehaviour {

    [Header("Motor Parameters")]
    [Range(0, 170)]
    public int left_angle;
    [Range(0, 170)]
    public int right_angle;
    [Range(0.0f, 2.0f)]
    public float delay_time;

    [Header("Thermal Parameters")]
    [Range(-30, 135)]
    public int lower_left_pwm;
    [Range(-30, 135)]
    public int lower_right_pwm;
    [Range(-30, 135)]
    public int upper_left_pwm;
    [Range(-30, 135)]
    public int upper_right_pwm;
    [Range(0.0f, 1.0f)]
    public float lasting_time;

	private int Left_degreeConvertToRotaryCoder(int degree) { return (degree * 1024 / 360); }
    private int Right_degreeConvertToRotaryCoder(int degree) { return (degree * 824 / 360); }

	public void OnPushButtonClick()
	{
	  	Debug.Log("Click PushButton");
	  	StartCoroutine(motorControl(left_angle, right_angle, delay_time));
	 }

	public void OnThermalClick()
	{
	  	Debug.Log("Click Thermal");
	  	StartCoroutine(thermalControl(lower_left_pwm, lower_right_pwm, upper_left_pwm, upper_right_pwm, lasting_time));
	}

	IEnumerator motorControl(int leftAngle, int rightAngle, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        new Thread(GameDataManager.Uno.sendData).Start(Left_degreeConvertToRotaryCoder(leftAngle) + " " + Right_degreeConvertToRotaryCoder(rightAngle));
    }

   	IEnumerator thermalControl(int lowerLeft, int lowerRight, int upperLeft, int upperRight, float lastingTime)
    {
		new Thread(GameDataManager.UnoThermo.sendData).Start(lowerLeft + " " + lowerRight + " " + upperLeft + " " + upperRight);
		yield return new WaitForSeconds(lastingTime);
		new Thread(GameDataManager.UnoThermo.sendData).Start("0" + " " + "0" + " " + "0" + " " + "0");
   	}
}
