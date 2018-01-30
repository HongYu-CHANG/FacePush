using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cam_pos : MonoBehaviour {

    public GameObject root;
    private Quaternion rot;
    private Vector3 pos;
    private Vector3 rots;

    // Use this for initialization
    void Start () {
        rot = root.transform.rotation;
        rots = root.transform.rotation.eulerAngles;
        pos = root.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        this.transform.rotation = Quaternion.Euler(root.transform.rotation.eulerAngles - rots);    
        this.transform.position += root.transform.position - pos;
        rot = root.transform.rotation;
        pos = root.transform.position;
    }
}
