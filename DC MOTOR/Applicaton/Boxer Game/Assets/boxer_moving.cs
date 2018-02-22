using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boxer_moving : MonoBehaviour {

    public GameObject cam;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey("w"))
        {
            this.transform.position += new Vector3(0, 0, 0.05f);
        }
        if (Input.GetKey("s"))
        {
            this.transform.position += new Vector3(0, 0, -0.05f);
        }
        if (Input.GetKey("a"))
        {
            this.transform.position += new Vector3(0.05f, 0, 0);
        }
        if (Input.GetKey("d"))
        {
            this.transform.position += new Vector3(-0.05f, 0, 0);
        }

        this.transform.rotation = cam.transform.rotation;
        this.transform.position = new Vector3(cam.transform.position.x,0.8f, cam.transform.position.z);
    }
}
