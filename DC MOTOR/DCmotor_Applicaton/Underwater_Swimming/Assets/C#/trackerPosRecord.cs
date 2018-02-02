using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trackerPosRecord : MonoBehaviour {

    public int frameSegment = 2;//0: not save position in that frame / 1: save position
    private int counter = 0;

    public GameObject Ltracker;
    public GameObject Rtracker;
    private Vector3 LlastPos;
    private Vector3 RlastPos;
    private Vector3 Lvector;
    private Vector3 Rvector;

    public GameObject headpos;
    public GameObject bodypos;
    private Vector3 body_head_direction;

    // Use this for initialization
    void Start () {

        LlastPos = Ltracker.transform.position;
        RlastPos = Rtracker.transform.position;
        body_head_direction = headpos.transform.position - bodypos.transform.position;

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void FixedUpdate()
    {
        if(counter < frameSegment-1)
        {
            //do not get tracker position in this frame
            counter++;
        }
        else
        {
            //frameSegment = 1 -> get tracker position in this frame
            Lvector = LlastPos - Ltracker.transform.position;
            Rvector = RlastPos - Rtracker.transform.position;
            body_head_direction = headpos.transform.position - bodypos.transform.position;

            //L direction
            //not sure if it's okay to use > or < here to determine the Lvector & body_head_direction
            if ((Lvector + body_head_direction).magnitude > body_head_direction.magnitude)
            {
                //Lvector has the same direction as body_hear_direction
                transform.Rotate((Lvector-body_head_direction)*0.5f);
                transform.position = transform.position + Lvector;
            }

            //R direction
            if ((Rvector + body_head_direction).magnitude > body_head_direction.magnitude)
            {
                //Rvector has the same direction as body_hear_direction
                transform.Rotate((Rvector - body_head_direction) * 0.5f);
                transform.position = transform.position + Rvector;
            }


            //reset
            LlastPos = Ltracker.transform.position;
            RlastPos = Rtracker.transform.position;
            Lvector = Vector3.zero;
            Rvector = Vector3.zero;
            counter = 0;

        }
    }
}
