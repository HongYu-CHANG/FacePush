using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movingControl : MonoBehaviour {

    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {

        transform.position = new Vector3(transform.position.x + 0.01f, transform.position.y, transform.position.z);
        Debug.Log("=== move: " + transform.position.ToString("F4") + " ===");

        if (Input.GetKeyDown(KeyCode.A))
        {
            transform.Rotate(Vector3.down);
        }

        if(Input.GetKeyDown(KeyCode.S))
        {
            transform.Rotate(Vector3.up);
        }
    }
}
