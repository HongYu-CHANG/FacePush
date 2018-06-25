using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiverControll : MonoBehaviour {
    
    //Diver Body
    public GameObject LeftHand;
    public GameObject RightHand;

    // Use this for initialization
    void Start ()
    {
        transform.position = new Vector3((LeftHand.transform.position.x + RightHand.transform.position.x) / 2, 
            0.47f, (LeftHand.transform.position.z + RightHand.transform.position.z) / 2);
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
