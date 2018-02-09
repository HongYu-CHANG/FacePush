using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class debug_view : MonoBehaviour {

    Color color = Color.red;
    private int count = 0;
    private int h = 1;

    // Use this for initialization
    void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
		if(count == 80)
        {
            color.a = 0f;
            GameObject.FindGameObjectWithTag("view_c").GetComponent<Renderer>().material.color = color;
            GameObject.FindGameObjectWithTag("view_r").GetComponent<Renderer>().material.color = color;
            GameObject.FindGameObjectWithTag("view_l").GetComponent<Renderer>().material.color = color;
            count = 0;
            h = 1;
        }
        if (h == 0) count++;
	}

    void OnTriggerEnter(Collider other) {
        if(h == 1)
        {
            color.a = 0.5f;
            if (this.name == "0") GameObject.FindGameObjectWithTag("view_c").GetComponent<Renderer>().material.color = color;
            else if (this.name == "1") GameObject.FindGameObjectWithTag("view_r").GetComponent<Renderer>().material.color = color;
            else if (this.name == "2") GameObject.FindGameObjectWithTag("view_l").GetComponent<Renderer>().material.color = color;
            count = 0;
            h = 0;
        }

    }
}
