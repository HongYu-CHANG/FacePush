using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gloveVibration : MonoBehaviour {

    public SteamVR_TrackedObject controller;
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
        if (other.gameObject.CompareTag("Boss") && ColliderOn)
        {
            SteamVR_Controller.Input((int)controller.index).TriggerHapticPulse(2000);
        }
    }

    public void setColliderOn(bool setting)
    {
        ColliderOn = setting;
    }
}
