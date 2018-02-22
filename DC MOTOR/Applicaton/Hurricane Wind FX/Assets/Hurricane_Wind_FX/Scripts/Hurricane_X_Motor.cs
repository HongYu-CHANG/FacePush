using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; 

public class Hurricane_X_Motor : MonoBehaviour {

	public ParticleSystem Debris;
	public ParticleSystem Tumbleweed;
	public ParticleSystem Wind_Particles;
	public ParticleSystem Leaf;
	public AudioSource Wind_Audio;
	public GameObject CameraEye;
	public GameObject Cube;
	public List<OSCSender> OSCSenderS;
    public int ConvertTime;
    public int StopTime;
	private bool HurricaneWork = false;
    private bool HurricaneStop = false;
	private bool stopView = false;
    private float timer_f = 0f;
    private int timer_i = 0;
    private float rotationY;
    private float initialY;
	private int motorActive;
	private int lastSpeed = 5;
	private float headDegree;
    private enum Motors
    {
        Left = 0,
        Right = 1,
		Both = 2,
    }

    void Start () 
	{
    	OSCSenderS[(int)Motors.Left].setWhichMotor("L");
    	OSCSenderS[(int)Motors.Right].setWhichMotor("R");
    	initialY = Cube.transform.rotation.eulerAngles.y;
	}
	void Update () 
	{
		rotationY = CameraEye.transform.rotation.eulerAngles.y;
		if (ABS(Cube.transform.rotation.eulerAngles.y - initialY) < 0.01f && stopView)
		{
			headDegree = 0;
			stopView = false;
		}
		//Debug.Log(headDegree);
		Cube.transform.Rotate(new Vector3(0f, (headDegree / (1/Time.deltaTime)), 0f));
		
		//Debug.Log(rotationY);
		timer_f += Time.deltaTime;
		timer_i = (int) timer_f;
		
		if (rotationY >= 285 && rotationY <= 350)//R
		{
			motorActive = (int) Motors.Right;
		}
		else if (rotationY >= 150 && rotationY <= 225)//L
		{
			motorActive = (int) Motors.Left;
		}
		else
		{
			motorActive = (int) Motors.Both;
			
		}

		if ((timer_i - ConvertTime) % (ConvertTime + StopTime) == 0 && !HurricaneStop)
		{
			Debug.Log(Cube.transform.rotation.eulerAngles.y - initialY);
			if (Cube.transform.rotation.eulerAngles.y > initialY)
			{
				headDegree = -0.1f;
			}
			else
			{
				headDegree = 0.1f;
			}
			

			/*Quaternion quate = Quaternion.identity;
			quate.eulerAngles = new Vector3(0, initialY, 0);
			Cube.transform.rotation = quate;*/
			HurricaneStop = true;
			Debug.Log("Time = " + timer_i);
			Wind_Audio.volume = 0;
			particleSystemIsStart(false);
			stopView = true;
		}
		else if ((timer_i - ConvertTime) % (ConvertTime + StopTime) != 0 && HurricaneStop)
			HurricaneStop = false;

        if (timer_i % (ConvertTime+StopTime) == 0 && !HurricaneWork)
		{
			
			HurricaneWork = true;
			Debug.Log("Time = " + timer_i);
			
			int speed = UnityEngine.Random.Range(5, 11);
			while(ABS(lastSpeed - speed) > 3 || lastSpeed == speed)
			{
				speed = UnityEngine.Random.Range(5, 11);
			}
            lastSpeed = speed;
		    Debug.Log("Speed = " + speed);

		    //audio source
		    Wind_Audio.volume = remapping((float)speed, 1f, 10f, 0.1f, 1f);

		    //particle system setting
			particleSystemIsStart(true);
	        var tempMain = Debris.main;
	 		tempMain.simulationSpeed = speed;
	 		tempMain = Tumbleweed.main;
	 		tempMain.simulationSpeed = speed;
	 		tempMain = Wind_Particles.main;
	 		tempMain.simulationSpeed = speed;
	 		tempMain = Leaf.main;
	 		tempMain.simulationSpeed = speed;
			
            // DC motor
            int Speed = (int)remapping((float)speed, 1f, 10f, 85f, 255f);
			Debug.Log("Motor = " + Speed);
			StartCoroutine(SHAKEWORK(Speed));
		}
		else if (timer_i % (ConvertTime+StopTime) != 0 && HurricaneWork)
			HurricaneWork = false;
	}

	static float ABS(float value)
    {
        return value < 0 ? -value : value;
    }

	private float remapping (float value, float low1, float high1, float low2, float high2)
	{
		return low2 + (value - low1) * (high2 - low2) / (high1 - low1);
	}

	IEnumerator SHAKEWORK(int tempSpeed)
	{
		float tempTime = 0;//UnityEngine.Random.Range(0.01f, 0.03f);
		int FirstMotor = 0, SecondMotor = 0;
		//headDegree = remapping((float)tempSpeed, 85f, 255f, 0.5f, 5);
		int turnDir = 1;

		if (motorActive == (int)Motors.Both)
		{
			motorPressure( tempSpeed );
			headDegree = 0;
			yield return new WaitForSeconds(0.9f);
		}
		else
		{
			FirstMotor = motorActive;
			SecondMotor = FirstMotor == (int)Motors.Left ? (int)Motors.Right : (int)Motors.Left;
			OSCSenderS[SecondMotor].SendOSCMessageTriggerMethod(170, tempSpeed);
		}
		headDegree = remapping((float)tempSpeed, 85f, 255f, 0.5f, 5);
		for (int i = 0 ; i < ((ConvertTime-1)/2); i++)
		{
	       	
	       	if(motorActive == (int)Motors.Both)
			{
				FirstMotor = UnityEngine.Random.Range(0, 2);
				SecondMotor = FirstMotor == (int)Motors.Left ? (int)Motors.Right : (int)Motors.Left;
				turnDir = FirstMotor == (int)Motors.Left ? 1 : -1;

				
				OSCSenderS[FirstMotor].SendOSCMessageTriggerMethod(170, tempSpeed);//加壓
				headDegree *= turnDir;
				yield return new WaitForSeconds(0.8f - tempTime);

				OSCSenderS[SecondMotor].SendOSCMessageTriggerMethod(170, tempSpeed);
				headDegree *= (turnDir * (-1));
				yield return new WaitForSeconds(0.8f + tempTime);

				OSCSenderS[FirstMotor].SendOSCMessageTriggerMethod(10, tempSpeed);//放鬆
				yield return new WaitForSeconds(0.15f - tempTime);

				OSCSenderS[SecondMotor].SendOSCMessageTriggerMethod(10, tempSpeed);
				yield return new WaitForSeconds(0.15f + tempTime);
			}
			else 
			{
				FirstMotor = motorActive;
				SecondMotor = FirstMotor == (int)Motors.Left ? (int)Motors.Right : (int)Motors.Left;

				OSCSenderS[SecondMotor].SendOSCMessageTriggerMethod(10, tempSpeed);
				yield return new WaitForSeconds(0.2f - tempTime);
				OSCSenderS[SecondMotor].SendOSCMessageTriggerMethod(170, tempSpeed);
				yield return new WaitForSeconds(0.2f + tempTime);
				OSCSenderS[SecondMotor].SendOSCMessageTriggerMethod(10, tempSpeed);
				yield return new WaitForSeconds(0.2f - tempTime);
				OSCSenderS[SecondMotor].SendOSCMessageTriggerMethod(170, tempSpeed);
				yield return new WaitForSeconds(0.2f + tempTime);
				OSCSenderS[SecondMotor].SendOSCMessageTriggerMethod(10, tempSpeed);
				yield return new WaitForSeconds(0.2f - tempTime);
				OSCSenderS[SecondMotor].SendOSCMessageTriggerMethod(170, tempSpeed);
				yield return new WaitForSeconds(0.2f + tempTime);
				OSCSenderS[SecondMotor].SendOSCMessageTriggerMethod(10, tempSpeed);
				yield return new WaitForSeconds(0.2f - tempTime);
				OSCSenderS[SecondMotor].SendOSCMessageTriggerMethod(170, tempSpeed);
				yield return new WaitForSeconds(0.2f + tempTime);
			}
    	}
		
	}

	private void motorPressure(int speed)
	{
		OSCSenderS[(int)Motors.Right].SendOSCMessageTriggerMethod(170, speed);//加壓
	    OSCSenderS[(int)Motors.Left].SendOSCMessageTriggerMethod(170, speed);
	}

	private void motorRelease(int speed)
	{
		OSCSenderS[(int)Motors.Right].SendOSCMessageTriggerMethod(10, speed);//放鬆
	    OSCSenderS[(int)Motors.Left].SendOSCMessageTriggerMethod(10, speed);
	}

	private void particleSystemIsStart(bool isStart)
	{
		Debris.enableEmission = isStart;
		Tumbleweed.enableEmission = isStart;
		Wind_Particles.enableEmission = isStart;
		Leaf.enableEmission = isStart;
	}
}
