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
	void Update ()
	{

        //if(LhandMoveObj.transform.position != LlastPos)
		if(Vector3.Distance(LhandMoveObj.transform.position, LlastPos) > 0.5)
        {
            //if(transform.rotation.y <= 90 && transform.rotation.y >=-90)
            {
                //transform.Rotate((Vector3.up) * (LhandMoveObj.transform.position.z - LlastPos.z) * 50);
                transform.Rotate((Vector3.down) * (LhandMoveObj.transform.position.z - LlastPos.z) * 50);
                Debug.Log("LlastPos = " + LlastPos);
                Debug.Log("LhandPos = " + LhandMoveObj.transform.position);
                Debug.Log("LnowPos = " + transform.position);
                LlastPos = LhandMoveObj.transform.position;
            }
                
        }
        else
        {
            Debug.Log("same L pos");
        }

	}

}
