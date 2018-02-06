using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class handDisplayControl : MonoBehaviour {

    public GameObject tracker;
    private Vector3 trackerLastPos;

	// Use this for initialization
	void Start () {
        trackerLastPos = tracker.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void FixedUpdate()
    {
        transform.position = transform.position + (tracker.transform.position - trackerLastPos) ;
        trackerLastPos = tracker.transform.position;
    }

}
