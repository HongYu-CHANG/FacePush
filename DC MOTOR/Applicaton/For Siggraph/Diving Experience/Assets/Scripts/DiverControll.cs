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
    private Vector3 rotateValue = Vector3.zero;

    public float drawRayTime = 10;
    private bool isStarting = false;

    // Use this for initialization
    void Start ()
    {
        //Driver body, right hand, left hand, face, start game
        this.transform.position = new Vector3((driverLeftHand.position.x + driverRightHand.position.x) / 2,
            0.47f, (driverLeftHand.position.z + driverRightHand.position.z) / 2);
        LlastPos = driverLeftHand.position;
        RlastPos = driverRightHand.position;
        diveDirection = directionContorl.position - driver.position;
        StartCoroutine(startGame());
        
    }
	
	// Update is called once per frame
	void FixedUpdate()
    {
        GameDataManager.Uno.motorLocker();
        Lvector = new Vector3(LlastPos.x - driverLeftHand.position.x, 0, LlastPos.z - driverLeftHand.position.z);
        Rvector = new Vector3(RlastPos.x - driverRightHand.position.x, 0, RlastPos.z - driverRightHand.position.z);
        diveDirection = directionContorl.position - driver.position;
        LRvector = Lvector + Rvector;
        body_vector_angle = Vector3.Angle(new Vector3(diveDirection.x, 0, diveDirection.z), new Vector3(LRvector.x, 0, LRvector.z));
        //rotation
        if ((Rvector.magnitude > 0.05f || Lvector.magnitude > 0.05f) && isStarting)
        {
            Debug.Log(body_vector_angle);
            if ((Lvector.magnitude - Rvector.magnitude) > 0.08f) //trun right
            {
                Debug.LogWarning("Right!!");
                rotateValue = Vector3.up * body_vector_angle * 0.5f;
    
            }
            else if ((Rvector.magnitude - Lvector.magnitude) > 0.05f)//turn left
            {
                Debug.LogWarning("Left!!");
                rotateValue = Vector3.down * body_vector_angle * 0.5f;
            }
            else if (Mathf.Abs(Rvector.magnitude - Lvector.magnitude) < 0.04f)
            {
                Debug.LogWarning("Forward!!");
                rotateValue = Vector3.zero;
            }
    
        }

        //dive offset control
        if (LRvector.magnitude > 0.01f && LRvector.magnitude < 0.02f) offset += 0.1f;
        else if (LRvector.magnitude > 0.02f && LRvector.magnitude < 0.03f) offset += 0.2f;
        else if (LRvector.magnitude > 0.03f && LRvector.magnitude < 0.04f) offset += 0.3f;
        else if (LRvector.magnitude> 0.04f && LRvector.magnitude < 0.05f) offset += 0.4f;
        else if (LRvector.magnitude > 0.05f) offset += 0.5f;
        if (offset > 12) offset = 12;

        transform.position += new Vector3(diveDirection.x, 0, diveDirection.z) * (LRvector.magnitude + offset) * 2.2f * Time.deltaTime;
        transform.Rotate(rotateValue * Time.deltaTime);
        rotateValue -= rotateValue * Time.deltaTime;
        

         //reset parameter
         LlastPos = driverLeftHand.position;
         RlastPos = driverRightHand.position;
         Lvector = Vector3.zero;
         Rvector = Vector3.zero;
         counter = 0;
         if (offset > 0.2f) offset -= 0.2f;
         else offset = 0;
         if (motorSegmentCounter == 2) motorSegmentCounter = 0;
         else motorSegmentCounter++;
        if (Input.GetKey(KeyCode.S))
        {
            GameDataManager.Uno.sendData("512 255 412 255");
        }
        if (Input.GetKey(KeyCode.A))
        {
            GameDataManager.Uno.sendData("0 255 0 255");
        }
    }

    IEnumerator startGame()
    {
        yield return new WaitForSeconds(1.5f);
        isStarting = true;
        GameDataManager.Uno.sendData("512 255 412 255");
    }
}
