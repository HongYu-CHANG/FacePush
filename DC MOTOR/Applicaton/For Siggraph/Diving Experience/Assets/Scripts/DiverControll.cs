using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiverControll : MonoBehaviour {

    public int frameSegment = 3;//num of frames between 2 position recording
    private int counter = 0;

    private int motorSegmentCounter = 0;
    //Diver Body
    public Transform driverLeftHand;
    public Transform driverRightHand;
    public Transform driver;
    public Transform directionContorl;
    private Vector3 LlastPos;//左手上一次的位置
    private Vector3 RlastPos;//右手上一次的位置
    private Vector3 Lvector;
    private Vector3 Rvector;
    private Vector3 diveDirection; //身體的方向

    //v2: 用合力計算旋轉角度 & 移動
    private Vector3 LRvector;
    private float body_vector_angle;
    private int i = 0;
    private float offset = 1;
    private bool isRotated = false;
    private bool isR_Rotated = false;
    private bool isL_Rotated = false;
    private Vector3 rotateValue = Vector3.zero;

    //
    public float drawRayTime = 10;

    // Use this for initialization
    void Start ()
    {
        this.transform.position = new Vector3((driverLeftHand.position.x + driverRightHand.position.x) / 2,
            0.47f, (driverLeftHand.position.z + driverRightHand.position.z) / 2);
        
    }
	
	// Update is called once per frame
	void FixedUpdate()
    {
        Lvector = new Vector3(LlastPos.x - driverLeftHand.position.x, 0, LlastPos.z - driverLeftHand.position.z);
        Rvector = new Vector3(RlastPos.x - driverRightHand.position.x, 0, RlastPos.z - driverRightHand.position.z);
        //Rvector = RlastPos - driverRightHand.position;
        diveDirection = directionContorl.position - driver.position;
        LRvector = Lvector + Rvector;
        body_vector_angle = Vector3.Angle(new Vector3(diveDirection.x, 0, diveDirection.z), new Vector3(LRvector.x, 0, LRvector.z));
        Debug.Log("RlastPos" + RlastPos);
        Debug.Log("driverRightHand.position" + driverRightHand.position);
        //Debug.Log(Mathf.Abs(Rvector.magnitude - Lvector.magnitude));
        Debug.Log(Rvector);
        //rotation
        if (Rvector.magnitude > 0.05f || Lvector.magnitude > 0.05f)
        {   
            //if (Mathf.Abs(Rvector.magnitude - Lvector.magnitude) > 0.08f && LRvector.x > 0)
           // {
                if ((Lvector.magnitude - Rvector.magnitude) > 0.08f) //trun right
                {
                    //Debug.LogWarning("Right!!");
                    isR_Rotated = true;
                    isL_Rotated = false;
                    //Debug.Log("Right" + Vector3.up * body_vector_angle * 0.03f);
                    rotateValue = Vector3.up * body_vector_angle * 1.2f;
                    //transform.Rotate(Vector3.up * body_vector_angle * 0.03f);
                    //if (isRotated == false && fish_control.fish == 0 && shark_control.shark == 0)
                    //{
                    //    StartCoroutine(No1Work(true, false, 0, 0)); //R,L,angle,speed (OSC)
                    //    new Thread(Uno.SendData).Start("20 150 120 150"); // L Lspeed R Rspeed
                    //    Debug.Log("turn right, L motor");
                    //}

                }
                else if ((Rvector.magnitude - Lvector.magnitude) > 0.08f)//turn left
                {
                    //Debug.LogWarning("Left!!");
                    isR_Rotated = false;
                    isL_Rotated = true;
                    //Debug.Log("Left" + Vector3.down * body_vector_angle * 0.03f);
                    rotateValue = Vector3.down * body_vector_angle * 1.2f;
                    //transform.Rotate(Vector3.down * body_vector_angle * 0.03f);
                    //if (isRotated == false && fish_control.fish == 0 && shark_control.shark == 0)
                    //{
                    //    StartCoroutine(No1Work(false, true, 0, 0));
                    //    Debug.Log("turn left, R motor");
                    //}

                }
                else
                {
                    //Debug.LogWarning("Forward!!");
                    rotateValue = Vector3.zero;
                }

                //isRotated = true;
            //}
        }

        //drive offset control
        if (LRvector.magnitude > 0.01f && LRvector.magnitude < 0.02f) offset += 0.1f;
        else if (LRvector.magnitude > 0.02f && LRvector.magnitude < 0.03f) offset += 0.2f;
        else if (LRvector.magnitude > 0.03f && LRvector.magnitude < 0.04f) offset += 0.3f;
        else if (LRvector.magnitude> 0.04f && LRvector.magnitude < 0.05f) offset += 0.4f;
        else if (LRvector.magnitude > 0.05f) offset += 0.5f;

        if (offset > 12) offset = 12;
        //if (Input.GetKeyDown(KeyCode.Q)) { Debug.Log("offset: " + offset); StartCoroutine(No1Work(false, false, 0, 255)); }

        if (isRotated)//swim Right rotation
        {
            transform.Rotate(rotateValue * Time.deltaTime);
            rotateValue -= rotateValue * Time.deltaTime;
            ////transform.position += new Vector3(diveDirection.x, 0, diveDirection.z) * (LRvector.magnitude + offset) * 0.05f;// 0.0005f ;
            //transform.position += new Vector3(diveDirection.x, 0, diveDirection.z) * (LRvector.magnitude + offset) * 0.055f * Time.deltaTime;
            //offset = offset * 0.2f;//* 0.005f;//0.0005f;//
            Debug.Log("---rotate---" + rotateValue);
            Debug.Log("---rotate---" + rotateValue.magnitude);
        }
        else//straight
        {
            transform.position += new Vector3(diveDirection.x, 0, diveDirection.z) * (LRvector.magnitude + offset) * 2.2f * Time.deltaTime;
            transform.Rotate(rotateValue * Time.deltaTime);
            rotateValue -= rotateValue * Time.deltaTime;
            ////transform.position += new Vector3(diveDirection.x, 0, diveDirection.z) * (LRvector.magnitude + offset) * 0.05f;// 0.0005f ;
            //transform.position += new Vector3(diveDirection.x, 0, diveDirection.z) * (LRvector.magnitude + offset) * 0.055f * Time.deltaTime;
            //offset = offset * 0.2f;//* 0.005f;//0.0005f;//
            //Debug.Log("---rotate---" + rotateValue);
            //Debug.Log("---rotate---" + rotateValue.magnitude);

            /*fish & shark & motor control
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
            */

        }

         //reset parameter
         LlastPos = driverLeftHand.position;
         RlastPos = driverRightHand.position;
         Lvector = Vector3.zero;
         Rvector = Vector3.zero;
         counter = 0;
         if (offset > 0.2f) offset -= 0.2f;
         else offset = 0;
        //if (rotateValue.magnitude < 5) rotateValue = Vector3.zero;//isRotated = false;

         if (motorSegmentCounter == 2) motorSegmentCounter = 0;
         else motorSegmentCounter++;
        }
}
