using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hurricane_X_Motor : MonoBehaviour {

	public ParticleSystem Debris;
	public ParticleSystem Tumbleweed;
	public ParticleSystem Wind_Particles;
	public ParticleSystem Leaf;
	public OSCSender R_OSCSender;
    public OSCSender L_OSCSender;
    public int ConvertTime = 3;
    private bool HurricaneWork = false;
    private float timer_f = 0f;
    private int timer_i = 0;
    //private OSCSender ROSCSender;
    //private OSCSender LOSCSender;

	// Use this for initialization
	void Start () 
	{
		//ROSCSender = RMotor.GetComponent<OSCSender>();
    	R_OSCSender.setWhichMotor("R");
    	//LOSCSender = LMotor.GetComponent<OSCSender>();
    	L_OSCSender.setWhichMotor("L");
	}
	
	// Update is called once per frame
	void Update () 
	{
		timer_f += Time.deltaTime;
		timer_i = (int) timer_f;
		if(timer_i % ConvertTime == 0 && !HurricaneWork)
		{
			HurricaneWork = true;
			Debug.Log(timer_i);
			Random.seed = System.Guid.NewGuid().GetHashCode();
			int speed = Random.Range(1, 11);
		    Debug.Log("Speed = " + speed);
	        var Debrismain = Debris.main;
	 		Debrismain.simulationSpeed = speed;
	 		var Tumbleweedmain = Tumbleweed.main;
	 		Tumbleweedmain.simulationSpeed = speed;
	 		var Wind_Particlesmain = Wind_Particles.main;
	 		Wind_Particlesmain.simulationSpeed = speed;
	 		var Leafmain = Leaf.main;
	 		Leafmain.simulationSpeed = speed;
		}
		else if (timer_i % ConvertTime != 0 && HurricaneWork)
		{
			HurricaneWork = false;
		}
	}
}
