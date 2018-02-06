using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class handDisplayControl : MonoBehaviour {

    public GameObject tracker;
    private Vector3 trackerLastPos;

	public int frameSegment;
	private int counter = 0;

	// Use this for initialization
	void Start () {
        trackerLastPos = tracker.transform.position;
	}
	
	// Update is called once per frame
	void Update () {

	}

	/*
    void FixedUpdate()
    {
		if(counter < frameSegment - 1)
		{
			counter++;
		}
		else
		{
			transform.position = transform.position + (tracker.transform.position - trackerLastPos) * 0.1f ;
			//transform.position = (tracker.transform.position - trackerLastPos
			trackerLastPos = tracker.transform.position;
			counter = 0;
		}
		
    }
	*/

}
