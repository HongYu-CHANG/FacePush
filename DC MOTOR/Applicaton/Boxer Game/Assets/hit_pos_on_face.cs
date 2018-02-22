using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hit_pos_on_face : MonoBehaviour {

    private Transform hit;

    // Use this for initialization
    void Start () {
        hit = GameObject.FindGameObjectWithTag("Hit").transform;
    }
	
	// Update is called once per frame
	void Update () {
        hit.position = hit.position + collider_dir.hit_pos;
        
	}
}
