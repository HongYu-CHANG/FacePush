using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movingControl : MonoBehaviour {

    //Rigidbody movingBody;
    //float speed;

    // Use this for initialization
    void Start () {
        //movingBody = GetComponent<Rigidbody>();
        //speed = 10.0f;
    }
	
	// Update is called once per frame
	void Update () {

        //transform.position = new Vector3(transform.position.x + 0.01f, transform.position.y, transform.position.z); 

        if (Input.GetKeyDown(KeyCode.A))
        {
            transform.Rotate(Vector3.down);
            //movingBody.velocity = transform.forward * speed;
        }

        if(Input.GetKeyDown(KeyCode.S))
        {
            transform.Rotate(Vector3.up);
        }
    }
}
