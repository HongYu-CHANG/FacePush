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
		
	}
}
