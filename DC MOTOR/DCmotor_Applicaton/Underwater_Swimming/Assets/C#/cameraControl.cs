using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraControl : MonoBehaviour {

	public GameObject cameraEye;
	private Vector3 offset;

	// Use this for initialization
	void Start () {
		offset = transform.position - cameraEye.transform.position;
		transform.Rotate(cameraEye.transform.rotation.x, 90, cameraEye.transform.rotation.z);
	}
	
	// Update is called once per frame
	void Update () {
		//transform.Rotate(cameraEye.transform.rotation.x, 90, cameraEye.transform.rotation.z);
		//transform.rotation = ;

		//transform.position = cameraEye.transform.position;
		//transform.position = cameraEye.transform.position + offset*2;
		//transform.position = new Vector3(cameraEye.transform.position.x, cameraEye.transform.position.y, cameraEye.transform.position.z);
	}
}
