using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiverControll : MonoBehaviour {
    //Diver Body
    public Transform driverLeftHand;
    public Transform driverRightHand;
    public Transform driver;
    public Transform directionContorl;
    public Transform DistanceCalculator;
    private Vector3 LlastPos = Vector3.zero;//左手上一次的位置
    private Vector3 RlastPos = Vector3.zero;//右手上一次的位置
    private Vector3 Lvector = Vector3.zero;//左手向量
    private Vector3 Rvector = Vector3.zero;//右手向量
    private float Ldistance;//左手距離後面的有多遠的值
    private float Rdistance;//右手距離後面的有多遠的值
    private float LLastDistance;//左手距離後面的有多遠上一次的值
    private float RLastDistance;//右手距離後面的有多遠上一次的值
    private Vector3 diveDirection = Vector3.zero; //身體的方向

    //Fishflock
    public FishFlock.FishFlockController fishflockFlowControl;
    public Transform fishflockOn;
    public Transform fishflockOff;

    //旋轉角度 移動offset 向量和 啟動bool
    private Vector3 LRvector = Vector3.zero; // 兩向量相加
    private float body_vector_angle;
    private float offset = 0;
    private Vector3 rotateValue = Vector3.zero;
    private bool isStarting = false;

    //Motor 參數
    private float waitingTime = 0.5f;
    private int Left_degreeConvertToRotaryCoder(int degree) { return (degree * 1024 / 360); }
    private int Right_degreeConvertToRotaryCoder(int degree) { return (degree * 824 / 360); }

    // Use this for initialization
    void Start ()
    {
        //Driver body, right hand, left hand, face, start game
        this.transform.position = new Vector3((driverLeftHand.position.x + driverRightHand.position.x) / 2,
            0.47f, (driverLeftHand.position.z + driverRightHand.position.z) / 2);
        LlastPos = driverLeftHand.position;
        RlastPos = driverRightHand.position;
        diveDirection = directionContorl.position - driver.position;
    }
	
	// Update is called once per frame
	void FixedUpdate()
    {
        GameDataManager.Uno.motorLocker();
        Lvector = new Vector3(LlastPos.x - driverLeftHand.position.x, 0, LlastPos.z - driverLeftHand.position.z);
        Ldistance = Vector3.Distance(driverLeftHand.position, DistanceCalculator.position);
        Rvector = new Vector3(RlastPos.x - driverRightHand.position.x, 0, RlastPos.z - driverRightHand.position.z);
        Rdistance = Vector3.Distance(driverRightHand.position, DistanceCalculator.position);
        diveDirection = directionContorl.position - driver.position;
        if ((LLastDistance - Ldistance) > 0.005 || (RLastDistance - Rdistance) > 0.005)
            LRvector = Lvector + Rvector;
        else
            LRvector -= LRvector * 3.0f * Time.deltaTime;
        body_vector_angle = Vector3.Angle(new Vector3(diveDirection.x, 0, diveDirection.z), new Vector3(LRvector.x, 0, LRvector.z));
        //rotation
        Debug.DrawRay(driver.transform.position, diveDirection * 10, Color.red, 10);
        if ((Rvector.magnitude > 0.025f || Lvector.magnitude > 0.025f) && ((LLastDistance - Ldistance) > 0.005 || (RLastDistance - Rdistance) > 0.005))
        {
            if ((Lvector.magnitude - Rvector.magnitude) > 0.025f) //trun right
            {
                if (rotateValue.magnitude <= 25)
                    rotateValue += Vector3.up * body_vector_angle * 0.16f;
                Debug.LogWarning("Right!!");
                if (rotateValue.magnitude > 25) rotateValue = new Vector3(0, 25, 0);
            }
            else if ((Rvector.magnitude - Lvector.magnitude) > 0.025f)//turn left
            {
                if (rotateValue.magnitude <= 25)
                    rotateValue += Vector3.down * body_vector_angle * 0.16f;
                Debug.LogWarning("Left!!");
                if (rotateValue.magnitude > 25) rotateValue = new Vector3(0, -25, 0);
            }
            else if (Mathf.Abs(Rvector.magnitude - Lvector.magnitude) < 0.02f)
            {
                Debug.LogWarning("Foward!!");
            }
            
        }

        //dive Foward offset control
        if (LRvector.magnitude > 0.02f && LRvector.magnitude < 0.03f) offset += 0.12f;
        else if (LRvector.magnitude > 0.03f && LRvector.magnitude < 0.04f) offset += 0.11f;
        else if (LRvector.magnitude> 0.04f && LRvector.magnitude < 0.05f) offset += 0.1f;
        else if (LRvector.magnitude > 0.05f) offset += 0.11f;
        if (offset > 12) offset = 12;

        //foward and rotation
        transform.position += new Vector3(diveDirection.x, 0, diveDirection.z) * (LRvector.magnitude + offset) * 3.0f * Time.deltaTime;
        transform.Rotate(rotateValue * Time.deltaTime * (Time.deltaTime * 25));
        rotateValue -= rotateValue * Time.deltaTime * (Time.deltaTime * 50);

        //reset parameter
        LlastPos = driverLeftHand.position;
        RlastPos = driverRightHand.position;
        LLastDistance = Ldistance;
        RLastDistance = Rdistance;
        Lvector = Vector3.zero;
        Rvector = Vector3.zero;
        if (rotateValue.magnitude <= 1)// 為了讓旋轉的值可以很快歸零，因為要讓它不要一值有殘餘的值
            rotateValue = Vector3.zero;
        if (offset > 0.021125) offset -= 0.021125f;
        else
        {
            offset = 0;
            LRvector = Vector3.zero;
        }

        if (Input.GetKey(KeyCode.S))
        {
            GameDataManager.Uno.sendData("512 255 412 255");
        }
        if (Input.GetKey(KeyCode.A))
        {
            GameDataManager.Uno.sendData("0 255 0 255");
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            StartCoroutine(fishflock());
        }
    }                                                                                                                                                                       

    IEnumerator Right_Turn(int angle, int speed)
    {
        yield return new WaitForSeconds(waitingTime);
    }
    IEnumerator Left_Turn(int angle, int speed)
    {
        yield return new WaitForSeconds(waitingTime);
    }
    IEnumerator Forward_Turn(int angle, int speed)
    {
        yield return new WaitForSeconds(waitingTime);
    }
    IEnumerator fishflock()
    {
        fishflockFlowControl.target = fishflockOn;
        yield return new WaitForSeconds(20f);
        fishflockFlowControl.target = fishflockOff;
    }
}
