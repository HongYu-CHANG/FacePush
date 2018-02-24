using UnityEngine;
using System.Collections;

public class mouseSphere : MonoBehaviour
{

	Vector3 mouse;
	public GameObject canvas;

	// Use this for initialization
	void Start()
	{
		//canvas = GameObject.Find ("Canvas");
	}

	// Update is called once per frame
	void Update()
	{

		mouse = (Input.mousePosition);
		//Debug.Log (mouse.x);
		mouse.z = 91;
		//Debug.Log (mouse.y);
		transform.position = Camera.main.ScreenToWorldPoint(mouse);
		mouse.z = 91;
	}

}