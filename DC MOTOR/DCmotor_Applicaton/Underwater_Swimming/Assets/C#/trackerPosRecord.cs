using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trackerPosRecord : MonoBehaviour {

    public int frameSegment;//num of frames between 2 position recording
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
	private Vector3 initial_body_head_direction;

	private bool posInitialized = false;
    public float drawRayTime;
	private float speed = 1;


    // Use this for initialization
    void Start () {

        LlastPos = Ltracker.transform.position;
        RlastPos = Rtracker.transform.position;
        body_head_direction = headpos.transform.position - bodypos.transform.position;
		initial_body_head_direction = body_head_direction;

		//xyz axis
		Debug.DrawLine(new Vector3(-1000,0,0), new Vector3(1000,0,0),Color.yellow, 10000);
		Debug.DrawLine(new Vector3(0, -1000, 0), new Vector3(0, 1000, 0), Color.cyan, 10000);
		Debug.DrawLine(new Vector3(0, 0, -1000), new Vector3(0, 0, 1000), Color.white, 10000);

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void FixedUpdate()
    {

        if (Input.GetKeyDown(KeyCode.H))
        {
            Debug.Log("Press H");
            transform.position = (Ltracker.transform.position + Rtracker.transform.position) / 2;
            Debug.Log("swimmer's position: " + transform.position);
            //transform.Rotate();
            posInitialized = true;
        }

        if(posInitialized == true)
        {

            if (counter < frameSegment - 1)
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

                
                Debug.DrawRay(Ltracker.transform.position, Lvector, Color.red, drawRayTime);
                Debug.DrawRay(Rtracker.transform.position, Rvector, Color.red, drawRayTime);
                Debug.DrawRay(bodypos.transform.position, body_head_direction * 10, Color.red, drawRayTime);

				

                //L direction
                //if ((Lvector + body_head_direction).magnitude > body_head_direction.magnitude)
				if ((Lvector + body_head_direction).magnitude - body_head_direction.magnitude > 0.001f)
				//if(Vector3.Angle(Lvector, body_head_direction) < 90)
                {
					//Lvector has the same direction as body_hear_direction

					//transform.Rotate(Lvector - body_head_direction).magnitude);
					//transform.Rotate(Vector3.up * (body_head_direction + Lvector).magnitude);
					//transform.Rotate(Vector3.up * (Lvector - body_head_direction).magnitude);

					
					if (Lvector.z < 0)
					{
						transform.Rotate(Vector3.up * (Lvector - body_head_direction).magnitude * 2);
					}
					else
					{
						transform.Rotate(Vector3.down * (Lvector - body_head_direction).magnitude * 2);
					}

					speed += (Lvector - body_head_direction).magnitude;

					/////////////

					Debug.Log("angle:" + Vector3.Angle(initial_body_head_direction, body_head_direction));
					Debug.Log("Lvector.z: " + Lvector.z);
					
					/*
					//if ((initial_body_head_direction + body_head_direction).magnitude > initial_body_head_direction.magnitude)
					if (Vector3.Angle(initial_body_head_direction, body_head_direction) < 150) //超過160會有問題?????
					{
						if (Lvector.z < 0)
						{
							transform.Rotate(Vector3.up * (Lvector - body_head_direction).magnitude * 5);
						}
						else
						{
							transform.Rotate(Vector3.down * (Lvector - body_head_direction).magnitude * 5);
						}
					}
					else
					{
						if (Lvector.z < 0)
						{
							transform.Rotate(Vector3.down * (Lvector - body_head_direction).magnitude * 5);
						}
						else
						{
							transform.Rotate(Vector3.up * (Lvector - body_head_direction).magnitude * 5);
						}
					}
					*/
					

					//transform.position = transform.position + Lvector * 2f;
					//transform.position = transform.position + new Vector3(Lvector.x, 0, 0)*2f;

					Debug.Log("L turn, pos: " + transform.position);
					

				}

                //R direction
                //if ((Rvector + body_head_direction).magnitude > body_head_direction.magnitude)
				if ((Rvector + body_head_direction).magnitude - body_head_direction.magnitude > 0.001f)
                {
					//Rvector has the same direction as body_hear_direction

					//transform.Rotate((Rvector - body_head_direction) * 0.5f);
					//transform.Rotate(Vector3.down * (Rvector - body_head_direction).magnitude);
					//transform.Rotate(body_head_direction - Vector3.Reflect(Rvector, body_head_direction) * (Rvector - body_head_direction).magnitude);

					Debug.Log("angle:" + Vector3.Angle(initial_body_head_direction, body_head_direction));
					Debug.Log("Rvector.z: " + Rvector.z);

					if (Rvector.z < 0)
					{
						transform.Rotate(Vector3.up * (Rvector - body_head_direction).magnitude * 2);
					}
					else
					{
						transform.Rotate(Vector3.down * (Rvector - body_head_direction).magnitude * 2);
					}

					speed += (Rvector - body_head_direction).magnitude;


					//transform.position = transform.position + Rvector * 2f;
					//transform.position = transform.position - Vector3.Reflect(Rvector, body_head_direction) * 2f;
					//transform.position = transform.position + new Vector3(Rvector.x,0,0)*2f;

					Debug.Log("R turn, pos: " + transform.position);
				}


				//default: swim forward
				//transform.position = transform.position + body_head_direction * 0.1f;
				transform.position = transform.position + new Vector3(body_head_direction.x, 0, body_head_direction.z) * speed * 0.00001f; //////???y


				//reset
				LlastPos = Ltracker.transform.position;
                RlastPos = Rtracker.transform.position;
                Lvector = Vector3.zero;
                Rvector = Vector3.zero;
                counter = 0;
				speed = 1;

            }




        }

        
    }
}
