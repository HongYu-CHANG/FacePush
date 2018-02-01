using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LhandControl : MonoBehaviour {

    public GameObject LhandMoveObj;
    private Vector3 LlastPos;

	// Use this for initialization
	void Start () {

        LlastPos = LhandMoveObj.transform.position;
    }
	
	// Update is called once per frame
	void Update () {

        if(LhandMoveObj.transform.position != LlastPos)
        {
            transform.Rotate((Vector3.down) * (LhandMoveObj.transform.position.z - LlastPos.z));
            Debug.Log("lastPos = " + LlastPos);
            Debug.Log("nowPos = " + transform.position);
            LlastPos = LhandMoveObj.transform.position;
        }
        else
        {
            Debug.Log("same L pos");
        }

	}

}
