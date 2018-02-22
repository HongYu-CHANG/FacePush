using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RhandControl : MonoBehaviour
{
    public GameObject RhandMoveObj;
    private Vector3 RlastPos;

    // Use this for initialization
    void Start()
    {
        RlastPos = RhandMoveObj.transform.position;
    }

    // Update is called once per frame
    void Update()
    {

        if (RhandMoveObj.transform.position != RlastPos)
        {
            transform.Rotate((Vector3.down) * (RhandMoveObj.transform.position.z - RlastPos.z));
            Debug.Log("RlastPos = " + RlastPos);
			Debug.Log("RhandPos = " + RhandMoveObj.transform.position);
			Debug.Log("RnowPos = " + transform.position);
            RlastPos = RhandMoveObj.transform.position;
        }
        else
        {
            Debug.Log("same R pos");
        }

    }

}
