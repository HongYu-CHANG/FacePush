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
	public List<OSCSender> OSCSenderS;
    public int ConvertTime;
    public int StopTime;
    private bool HurricaneWork = false;
    private bool HurricaneStop = false;
    private float timer_f = 0f;
    private int timer_i = 0;
    private float rotationY;
    private float initialY;
	private int motorActive;
	private int lastSpeed = 5;
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
    	initialY = CameraEye.transform.rotation.eulerAngles.y;
    }
	void Update () 
	{
		rotationY = CameraEye.transform.rotation.eulerAngles.y;
		
		timer_f += Time.deltaTime;
		timer_i = (int) timer_f;
		
		if (rotationY >= 215 && rotationY <= 340)//R
		{
			//motorActive = (int) Motors.Both;
			motorActive = (int) Motors.Right;
		}
		else if (rotationY >= 50 && rotationY <= 175)//L
		{
			//motorActive = (int) Motors.Both;
			motorActive = (int) Motors.Left;
		}
		else
		{
			 motorActive = (int) Motors.Both;
		}

		if((timer_i - ConvertTime) % (ConvertTime+StopTime) == 0 && !HurricaneStop)
		{
			HurricaneStop = true;
			Debug.Log("Time = " + timer_i);
			Wind_Audio.volume = 0;
			var tempMain = Debris.main;
	 		tempMain.simulationSpeed = 0;
	 		tempMain = Tumbleweed.main;
	 		tempMain.simulationSpeed = 0;
	 		tempMain = Wind_Particles.main;
	 		tempMain.simulationSpeed = 0;
	 		tempMain = Leaf.main;
	 		tempMain.simulationSpeed = 0;
			motorStop();
		}
		else if ((timer_i - ConvertTime) % (ConvertTime+StopTime) != 0 && HurricaneStop)
			HurricaneStop = false;

        if (timer_i % (ConvertTime+StopTime) == 0 && !HurricaneWork)
		{
			
			HurricaneWork = true;
			Debug.Log("Time = " + timer_i);
			
			int speed = UnityEngine.Random.Range(1, 11);
			while(ABS(lastSpeed - speed) > 3 || lastSpeed == speed)
			{
				speed = UnityEngine.Random.Range(1, 11);
			}
            lastSpeed = speed;
		    Debug.Log("Speed = " + speed);

		    //audio source
		    Wind_Audio.volume = remapping((float)speed, 1f, 10f, 0.1f, 1f);

		    //particle system setting
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
			Debug.Log("Dir = " + (int)remapping((float)Speed, 85f, 255f, 5, 30));
			StartCoroutine(SHAKEWORK(Speed));
		}
		else if (timer_i % (ConvertTime+StopTime) != 0 && HurricaneWork)
			HurricaneWork = false;
	}

	static int ABS(int value)
    {
        return value < 0 ? -value : value;
    }
	void OnApplicationQuit() 
	{
        motorStop();
    }

	private float remapping (float value, float low1, float high1, float low2, float high2)
	{
		return low2 + (value - low1) * (high2 - low2) / (high1 - low1);
	}

	IEnumerator SHAKEWORK(int tempSpeed)
	{
		
		float tempTime = UnityEngine.Random.Range(0.01f, 0.03f);
		int FirstMotor = 0, SecondMotor = 0;
		int turnDirFirst = 1, turnDirSecond = 0;
		Quaternion quate = Quaternion.identity;
		Vector3 temp = new Vector3(0, initialY + (int)remapping((float)tempSpeed, 85f, 255f, 5, 35) * (-1), 0);
		List<Vector3> turnDir = new List<Vector3>();
		turnDir.Add(temp);
		temp = new Vector3(0, initialY + (int)remapping((float)tempSpeed, 85f, 255f, 5, 35), 0);
		turnDir.Add(temp);
		if(motorActive == (int)Motors.Both)
		{
			 motorPressure( tempSpeed );
		}
		else
		{
			FirstMotor = motorActive;
			SecondMotor = FirstMotor == (int)Motors.Left ? (int)Motors.Right : (int)Motors.Left;
			OSCSenderS[FirstMotor].SendOSCMessageTriggerMethod("RELEASE", 0, 1);
			OSCSenderS[SecondMotor].SendOSCMessageTriggerMethod("FORWARD", tempSpeed, 1);
		}
		
		for(int i = 0 ; i < (ConvertTime - 1); i++)
		{
	       	
	       	if(motorActive == (int)Motors.Both)
			{
				FirstMotor = UnityEngine.Random.Range(0, 2);
				SecondMotor = FirstMotor == (int)Motors.Left ? (int)Motors.Right : (int)Motors.Left;
				turnDirFirst = FirstMotor == (int)Motors.Left ? -1 : 1;
				turnDirSecond = turnDirFirst * (-1);
				OSCSenderS[FirstMotor].SendOSCMessageTriggerMethod("BACKWARD", tempSpeed, 1);//放鬆
				yield return new WaitForSeconds(0.2f - tempTime);

				OSCSenderS[SecondMotor].SendOSCMessageTriggerMethod("BACKWARD", tempSpeed, 1);
				yield return new WaitForSeconds(0.2f + tempTime);

				Debug.Log("turnDir = " + turnDir[FirstMotor]);
				Debug.Log(CameraEye.transform.rotation.eulerAngles);
				OSCSenderS[FirstMotor].SendOSCMessageTriggerMethod("FORWARD", tempSpeed , 1);//加壓
				quate.eulerAngles = turnDir[FirstMotor];
				Debug.Log(CameraEye.transform.rotation.eulerAngles);
				CameraEye.transform.rotation = quate;
				yield return new WaitForSeconds(0.2f - tempTime);
				Debug.Log("turnDir = " + turnDir[SecondMotor]);
				Debug.Log(CameraEye.transform.rotation.eulerAngles);
				OSCSenderS[SecondMotor].SendOSCMessageTriggerMethod("FORWARD", tempSpeed, 1);
				quate.eulerAngles = turnDir[SecondMotor];
				Debug.Log(CameraEye.transform.rotation.eulerAngles);
				CameraEye.transform.rotation = quate;
				yield return new WaitForSeconds(0.2f + tempTime);
				
				
			}
			else 
			{
				FirstMotor = motorActive;
				SecondMotor = FirstMotor == (int)Motors.Left ? (int)Motors.Right : (int)Motors.Left;
				OSCSenderS[FirstMotor].SendOSCMessageTriggerMethod("RELEASE", 0, 1);

				OSCSenderS[SecondMotor].SendOSCMessageTriggerMethod("BACKWARD", tempSpeed, 1);
				yield return new WaitForSeconds(0.2f - tempTime);
				OSCSenderS[SecondMotor].SendOSCMessageTriggerMethod("FORWARD", tempSpeed, 1);
				yield return new WaitForSeconds(0.2f + tempTime);
				OSCSenderS[SecondMotor].SendOSCMessageTriggerMethod("BACKWARD", tempSpeed, 1);
				yield return new WaitForSeconds(0.2f - tempTime);
				OSCSenderS[SecondMotor].SendOSCMessageTriggerMethod("FORWARD", tempSpeed, 1);
				yield return new WaitForSeconds(0.2f + tempTime);
			}
    	}	
	}

	private void motorPressure(int speed)
	{
		OSCSenderS[(int)Motors.Right].SendOSCMessageTriggerMethod("FORWARD", speed, 1);//加壓
	    OSCSenderS[(int)Motors.Left].SendOSCMessageTriggerMethod("FORWARD", speed, 1);
	}

	private void motorRelease(int speed)
	{
		OSCSenderS[(int)Motors.Right].SendOSCMessageTriggerMethod("BACKWARD", speed, 1);//放鬆
	    OSCSenderS[(int)Motors.Left].SendOSCMessageTriggerMethod("BACKWARD", speed, 1);
	}

	private void motorStop()
	{
		OSCSenderS[(int)Motors.Right].SendOSCMessageTriggerMethod("RELEASE", 255, 2);
        OSCSenderS[(int)Motors.Left].SendOSCMessageTriggerMethod("RELEASE", 255, 2);
	}
}
