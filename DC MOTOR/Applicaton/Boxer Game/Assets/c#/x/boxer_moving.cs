using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boxer_moving : MonoBehaviour {

    public GameObject cam;
	private Quaternion rot;

	// Use this for initialization
	void Start () {
		rot = this.transform.rotation;
		//Debug.Log((cam.transform.position - this.transform.position).ToString("f4"));
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

		//this.transform.eulerAngles = (cam.transform.rotation.eulerAngles + rot.eulerAngles);
		//this.transform.rotation = cam.transform.rotation;
		this.transform.position = new Vector3(cam.transform.position.x  , cam.transform.position.y - 2.2f , cam.transform.position.z + 2f);
    }
}
