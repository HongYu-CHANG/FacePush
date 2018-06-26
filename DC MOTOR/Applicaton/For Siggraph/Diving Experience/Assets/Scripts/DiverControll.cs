using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiverControll : MonoBehaviour {
    
    //Diver Body
    public Transform LeftHand;
    public Transform RightHand;

    // Use this for initialization
    void Start ()
    {
        this.transform.position = new Vector3((LeftHand.position.x + RightHand.position.x) / 2, 
            0.47f, (LeftHand.position.z + RightHand.position.z) / 2);
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (counter < frameSegment - 1)
        {
            //do not get tracker position in this frame
            counter++;
        }
        else
        {
            Lvector = LlastPos - LeftHand.position;
            Rvector = RlastPos - RightHand.position;
            body_head_direction = headpos.transform.position - bodypos.transform.position;
            Debug.Log(Lvector);
            Debug.Log(Rvector);
            Debug.DrawRay(LeftHand.position, Lvector, Color.red, drawRayTime);
            Debug.DrawRay(RightHand.position, Rvector, Color.red, drawRayTime);
            Debug.DrawRay(bodypos.transform.position, body_head_direction * 10, Color.red, drawRayTime);

            LRvector = Lvector + Rvector;
            body_vector_angle = Vector3.Angle(new Vector3(body_head_direction.x, 0, body_head_direction.z), new Vector3(LRvector.x, 0, LRvector.z));

            rotation
                if (Rvector.magnitude > 0.05f || Lvector.magnitude > 0.05f)
            {
                if (Mathf.Abs(Rvector.magnitude - Lvector.magnitude) > 0.08f && LRvector.x > 0)
                {
                    if (Rvector.magnitude < Lvector.magnitude)
                    {
                        L > R: turn right
                            transform.Rotate(Vector3.up * body_vector_angle * 0.08f);
                        if (rotated == false && fish_control.fish == 0 && shark_control.shark == 0)
                        {
                            StartCoroutine(No1Work(true, false, 0, 0)); //R,L,angle,speed (OSC)
                            new Thread(Uno.SendData).Start("20 150 120 150"); // L Lspeed R Rspeed
                            Debug.Log("turn right, L motor");
                        }

                    }
                    else
                    {
                        //R > L: turn left
                            transform.Rotate(Vector3.down * body_vector_angle * 0.08f);
                        if (rotated == false && fish_control.fish == 0 && shark_control.shark == 0)
                        {
                            StartCoroutine(No1Work(false, true, 0, 0));
                            Debug.Log("turn left, R motor");
                        }

                    }

                    rotated = true;
                }
                else
                {
                    Debug.Log("don't rotate");
                    rotated = false;
                }

            }

            //offset control
                if (LRvector.magnitude > 0.01f && LRvector.magnitude < 0.02f) offset += 0.1f;
            else if (LRvector.magnitude > 0.02f && LRvector.magnitude < 0.03f) offset += 0.2f;
            else if (LRvector.magnitude > 0.03f && LRvector.magnitude < 0.04f) offset += 0.3f;
            else if (LRvector.magnitude > 0.04f && LRvector.magnitude < 0.05f) offset += 0.4f;
            else if (LRvector.magnitude > 0.05f) offset += 0.5f;

            if (offset > 12) offset = 12;
            if (Input.GetKeyDown(KeyCode.Q)) { Debug.Log("offset: " + offset); StartCoroutine(No1Work(false, false, 0, 255)); }

            //swim forward
                if (rotated)
            {
                transform.position += new Vector3(body_head_direction.x, 0, body_head_direction.z) * (LRvector.magnitude + offset) * 0.05f;// 0.0005f ;
                transform.position += new Vector3(body_head_direction.x, 0, body_head_direction.z) * (LRvector.magnitude + offset) * 0.005f * Time.deltaTime;
                offset = offset * 0.2f;//* 0.005f;//0.0005f;//
                Debug.Log("---rotate---");
            }
            else
            {
                transform.position += new Vector3(body_head_direction.x, 0, body_head_direction.z) * (LRvector.magnitude + offset) * 0.2f;
                transform.position += new Vector3(body_head_direction.x, 0, body_head_direction.z) * (LRvector.magnitude + offset) * 2f * Time.deltaTime;
                Debug.Log("---go straight---");

                fish & shark & motor control
                    if (fish_control.fish == 1) Debug.Log("fish!!");
                else if (shark_control.shark == 1) Debug.Log("shark!!");
                else if ((int)(LRvector.magnitude + offset) >= 12)
                {
                    if (motorSegmentCounter == 0) StartCoroutine(No1Work(false, false, 120, 255));
                    Debug.Log("move forward, max");
                }
                else if ((int)(LRvector.magnitude + offset) > 1)
                {
                    if (motorSegmentCounter == 0) StartCoroutine(No1Work(false, false, (int)(LRvector.magnitude + offset * 0.75) * 10, 255));
                    Debug.Log("move forward, default");
                }
                else
                {
                    //放鬆
                        if (motorSegmentCounter == 0) StartCoroutine(No1Work(false, false, 0, 255));
                    Debug.Log("move forward, relax");
                }

            }

            //reset
            LlastPos = LeftHand.position;
            RlastPos = RightHand.position;
            Lvector = Vector3.zero;
            Rvector = Vector3.zero;
            counter = 0;
            if (offset > 0.2f) offset -= 0.2f;
            else offset = 0;
            rotated = false;

            if (motorSegmentCounter == 2) motorSegmentCounter = 0;
            else motorSegmentCounter++;
        }
    }
}
