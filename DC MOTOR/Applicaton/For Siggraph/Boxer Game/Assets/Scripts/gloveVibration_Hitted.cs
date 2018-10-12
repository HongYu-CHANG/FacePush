using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gloveVibration_Hitted : MonoBehaviour {

    public SteamVR_TrackedObject controller;
    public OSCSender_UIST osc_sender;
    private bool ColliderOn = true;
    // Use this for initialization
    void Start () {
        
    }
	
	// Update is called once per frame
	void Update ()
    {
    }

    void OnTriggerEnter(Collider other)
    {
        //print("I am in OnTriggerEnter");
        if (other.gameObject.CompareTag("LHand") || other.gameObject.CompareTag("RHand"))
        {
            //SteamVR_Controller.Input((int)controller.index).TriggerHapticPulse(2000);
            osc_sender.callVibrate();
            //print("I am in IF of OnTriggerEnter");
        }
    }

    public void setColliderOn(bool setting)
    {
        ColliderOn = setting;
    }
}
