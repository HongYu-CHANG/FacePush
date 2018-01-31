using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movingControl : MonoBehaviour {

    public GameObject headpos;
    public GameObject bodypos;

    private Vector3 body_head_direction;

    float halfBodyLength = 0;//1.5f;
    float rotateDegree = 0f;

    // Use this for initialization
    void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {

        body_head_direction = headpos.transform.position - bodypos.transform.position;
        transform.position = transform.position + body_head_direction * (0.01f);

        //transform.position = new Vector3(transform.position.x + 0.01f, transform.position.y, transform.position.z);
        //Debug.Log("=== move: " + transform.position.ToString("F4") + " ===");

        //get the degree from directionControl.cs
        //rotateDegree = ???;

        //transform.position = new Vector3(transform.position.x - halfBodyLength * Mathf.Sin(rotateDegree * Mathf.Deg2Rad) , transform.position.y, transform.position.z + halfBodyLength * Mathf.Cos(rotateDegree * Mathf.Deg2Rad));

    }
}
