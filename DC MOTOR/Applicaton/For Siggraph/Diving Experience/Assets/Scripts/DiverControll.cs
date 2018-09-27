using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class DiverControll : MonoBehaviour {
    //Diver Body parameter
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

    //Fishflock parameter
    public FishFlock.FishFlockController fishflockFlowControl;
    public Transform fishflockOn;
    public Transform fishflockOff;

    //旋轉角度 移動offset 向量和 啟動bool
    private Vector3 LRvector = Vector3.zero; // 兩向量相加
    private float body_vector_angle;
    private float offset = 0;
    private Vector3 rotateValue = Vector3.zero;

    //馬達參數
    private float waitingTime = 0.5f;
    private int Left_degreeConvertToRotaryCoder(int degree) { return (degree * 1024 / 360); }
    private int Right_degreeConvertToRotaryCoder(int degree) { return (degree * 824 / 360); }
    private struct angle
    {
        public int leftAngle;
        public int rightAngle;
        public static bool operator ==(angle a1, angle a2)
        {
            return a1.Equals(a2);
        }
        public static bool operator !=(angle a1, angle a2)
        {
            return !a1.Equals(a2);
        }
    };
    private angle lastAngle;
    private angle nowAngle;
    private float waitTime = 1.3f;
    private DateTime motorTurnTime = DateTime.Now;

    //Shark parameter
    public Transform shark;
    private Animator _sharkAnimator;
    private int randomTurn = 0;
    private static bool isShark = false;

    //For test
    private bool specialEffectOn;

    // Use this for initialization
    void Start ()
    {
        //Driver body, right hand, left hand, face, start game
        this.transform.position = new Vector3((driverLeftHand.position.x + driverRightHand.position.x) / 2,
            0.47f, (driverLeftHand.position.z + driverRightHand.position.z) / 2);
        LlastPos = driverLeftHand.position;
        RlastPos = driverRightHand.position;
        diveDirection = directionContorl.position - driver.position;
        UnityEngine.Random.InitState(1337);
        _sharkAnimator = shark.GetComponent<Animator>();
        _sharkAnimator.SetInteger("Start", 0);
        _sharkAnimator.SetInteger("Turn", 0);
    }
	
	// Update is called once per frame
	void FixedUpdate()
    {
        //檢測Arduino是否可以接受訊號
        //GameDataManager.Uno.motorLocker();

        // 抓取左右手的位置，並計算成前進的方向、角度和速度
        positionCal();

        //決定是只有往前還是有往左右
        numeralCal();

        //send informantion to motor
        if (!specialEffectOn)
        {
            nowAngle = motorAngle(LRvector.magnitude + offset, rotateValue.y, lastAngle);

            if ((lastAngle != nowAngle && GameDataManager.Uno.getIntervalSeconds() > 0.5f) || GameDataManager.Uno.getIntervalSeconds() > waitTime)
            {
                if (GameDataManager.Uno.getIntervalSeconds() > waitTime)
                    waitTime = Mathf.Pow(waitTime, 1.1f);
                else
                    waitTime = 1.3f;
                Debug.Log(nowAngle.leftAngle + " " + nowAngle.rightAngle + " " + rotateValue.y);
                new Thread(GameDataManager.Uno.sendData).Start(Left_degreeConvertToRotaryCoder((int)nowAngle.leftAngle) + " " + Right_degreeConvertToRotaryCoder((int)nowAngle.rightAngle));
                lastAngle = nowAngle;
            }
        }

        //reset parameter
        resetParameter();

        // Shark
        sharkControl();

        //FishFlock
        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("Fish");
            StartCoroutine(fishflock());
        }
    }

    private void positionCal()
    {
        // 抓取左右手的位置，並計算成前進的方向、角度和速度
        Lvector = new Vector3(LlastPos.x - driverLeftHand.position.x, 0, LlastPos.z - driverLeftHand.position.z);
        Ldistance = Vector3.Distance(driverLeftHand.position, DistanceCalculator.position);
        Rvector = new Vector3(RlastPos.x - driverRightHand.position.x, 0, RlastPos.z - driverRightHand.position.z);
        Rdistance = Vector3.Distance(driverRightHand.position, DistanceCalculator.position);
        diveDirection = directionContorl.position - driver.position;
        if ((LLastDistance - Ldistance) > 0.005 || (RLastDistance - Rdistance) > 0.005)
            LRvector = Lvector + Rvector;
        else
            LRvector -= LRvector * 3f * Time.deltaTime;
        body_vector_angle = Vector3.Angle(new Vector3(diveDirection.x, 0, diveDirection.z), new Vector3(LRvector.x, 0, LRvector.z));
    }

    private void numeralCal()
    {
        //決定是只有往前還是有往左右
        Debug.DrawRay(driver.transform.position, diveDirection * 10, Color.red, 10);
        if ((Rvector.magnitude > 0.025f || Lvector.magnitude > 0.025f) && ((LLastDistance - Ldistance) > 0.005 || (RLastDistance - Rdistance) > 0.005))
        {
			if ((Lvector.magnitude - Rvector.magnitude) > 0.075f) //旋轉的閾值
            {
                if (rotateValue.magnitude <= 25)
                    rotateValue += Vector3.up * body_vector_angle * 0.16f;
                //Debug.LogWarning("Right!!");
                if (rotateValue.magnitude > 25) rotateValue = new Vector3(0, 25, 0);
            }
			else if ((Rvector.magnitude - Lvector.magnitude) > 0.075f)//旋轉的閾值
            {
                if (rotateValue.magnitude <= 25)
                    rotateValue += Vector3.down * body_vector_angle * 0.16f;
                //Debug.LogWarning("Left!!");
                if (rotateValue.magnitude > 25) rotateValue = new Vector3(0, -25, 0);
            }
            else if (Mathf.Abs(Rvector.magnitude - Lvector.magnitude) < 0.02f)
            {
				rotateValue = Vector3.zero;
				//Debug.LogWarning("Foward!!");
            }

        }

        //前進時的offset計算
        if (LRvector.magnitude > 0.02f && LRvector.magnitude < 0.03f) offset += 0.06f;
        else if (LRvector.magnitude > 0.03f && LRvector.magnitude < 0.04f) offset += 0.055f;
        else if (LRvector.magnitude > 0.04f && LRvector.magnitude < 0.05f) offset += 0.05f;
        else if (LRvector.magnitude > 0.05f) offset += 0.055f;
        if (offset > 8) offset = 8;

        //最後進行物體移動或旋轉的時候
        transform.position += new Vector3(diveDirection.x, 0, diveDirection.z) * (LRvector.magnitude + offset) * 3f * Time.deltaTime;
        transform.Rotate(rotateValue * Time.deltaTime * (Time.deltaTime * 25));
        rotateValue -= rotateValue * Time.deltaTime * (Time.deltaTime * 50);
    }

    private void sharkControl()
    {
        if (Input.GetKeyDown(KeyCode.S) && _sharkAnimator.GetBool("On"))
        {
            specialEffectOn = true;
            Debug.Log("Press\"S\" for shark");
            randomTurn = UnityEngine.Random.Range(1, 3);
            _sharkAnimator.SetBool("On", false);
            _sharkAnimator.SetInteger("Start", 1);
            _sharkAnimator.SetInteger("Turn", randomTurn);
            shark.transform.localPosition = new Vector3(1.7f, 1.5f, -25.25465f);
            shark.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }

        if (_sharkAnimator.GetCurrentAnimatorStateInfo(0).IsName("Swiming"))
        {
            if (isShark)
            {
                isShark = false;
                _sharkAnimator.SetInteger("Start", 0);
            }
            else if (Vector3.Distance(shark.position, this.transform.position) < 16)
            {
                StartCoroutine(sharkMotor(randomTurn));
                _sharkAnimator.SetBool("On", true);
                
            }
        }
        else if (_sharkAnimator.GetCurrentAnimatorStateInfo(0).IsName("Turn Left"))// Trun = 2 Right
        {
            if (!isShark)
            {
                Debug.Log("shark_right");
            }
            isShark = true;
        }
        else if (_sharkAnimator.GetCurrentAnimatorStateInfo(0).IsName("Turn Right"))// Trun = 1 //Left
        {
            if (!isShark)
            {
                Debug.Log("shark_left");
            }
            isShark = true;
        }
    }

    private void resetParameter()
    {
        LlastPos = driverLeftHand.position;
        RlastPos = driverRightHand.position;
        LLastDistance = Ldistance;
        RLastDistance = Rdistance;
        Lvector = Vector3.zero;
        Rvector = Vector3.zero;
        if (rotateValue.magnitude <= 1)// 為了讓旋轉的值可以很快歸零，因為要讓它不要一值有殘餘的值
            rotateValue = Vector3.zero;
        if (offset > 0.0105625) offset -= 0.0105625f;
        else
        {
            offset = 0;
            LRvector = Vector3.zero;
        }
    }

    	private angle motorAngle (float fowardValue, float rotateValue, angle last)
    {
        angle answer = last;
        if (rotateValue > 22.5f) // right
        {
            //Debug.Log("right: " + rotateValue);
			if (fowardValue >= 0 && fowardValue <= 0.25)  //0~1.5 20
            {
                answer.rightAngle = -30;
                answer.leftAngle = -30;
            }
			else if (fowardValue > 0.25 && fowardValue <= 3.5)//1.5~5 100
            {
                answer.rightAngle = -5;
                answer.leftAngle = 57;
            }
            else if (fowardValue > 3.5)//5up 130
            {
                answer.rightAngle = -5;
                answer.leftAngle = 90;
            }
        }
        else if (rotateValue < -22.5f) //left
        {
            //Debug.Log("left: " + rotateValue);
			if (fowardValue >= 0 && fowardValue <= 0.25)  //0~1.5 20
            {
                answer.rightAngle = -30;
                answer.leftAngle = -30;
            }
			else if (fowardValue > 0.25 && fowardValue <= 3.5)//1.5~5 100
            {
                answer.rightAngle = 40;
                answer.leftAngle = -5;
            }
            else if (fowardValue > 3.5)//5up 130
            {
                answer.rightAngle = 60;
                answer.leftAngle = -5;
            }
        }
        else if (rotateValue > -12.5f && rotateValue < 12.5f) // foward
        {
            //Debug.Log("foward: " + rotateValue);
            if (fowardValue >= 0 && fowardValue <= 0.25)  //0~1.5 20
            {
                answer.rightAngle = -30;
                answer.leftAngle = -30;
            }
            else if (fowardValue > 0.25 && fowardValue <= 2)  //0~1.5 20
            {
                answer.rightAngle = -10;
                answer.leftAngle = -10;
            }
            else if (fowardValue > 2 && fowardValue <= 5)//1.5~5 100
            {
                answer.rightAngle = 20;
                answer.leftAngle = 37;
            }
            else if (fowardValue > 5)//5up 130
            {
                answer.rightAngle = 60 ;
                answer.leftAngle = 90;
            }
        }

        if (Input.GetKey(KeyCode.Q))
        {
            answer.rightAngle = 10;
            answer.leftAngle = 27;
        }

        return answer;
    }

    IEnumerator fishflock()
    {
        specialEffectOn = true;
        fishflockFlowControl.target = fishflockOn;
        yield return new WaitForSeconds(20f);
        fishflockFlowControl.target = fishflockOff;
    }

    IEnumerator sharkMotor(int state)
    {
        yield return new WaitForSeconds(1.5f);
        if (specialEffectOn)
        {
            specialEffectOn = false;
            if (state == 1)
            {
                new Thread(GameDataManager.Uno.sendData).Start(Left_degreeConvertToRotaryCoder((int)nowAngle.leftAngle) + " " + Right_degreeConvertToRotaryCoder(134));
                yield return new WaitForSeconds(0.5f);
                new Thread(GameDataManager.Uno.sendData).Start(Left_degreeConvertToRotaryCoder((int)nowAngle.leftAngle) + " " + Right_degreeConvertToRotaryCoder((int)nowAngle.rightAngle));
            }
            else
            {
                //new Thread(GameDataManager.Uno.sendData).Start(Left_degreeConvertToRotaryCoder(134) + " " + Right_degreeConvertToRotaryCoder((int)nowAngle.rightAngle));
                new Thread(GameDataManager.Uno.sendData).Start(Left_degreeConvertToRotaryCoder(150) + " " + Right_degreeConvertToRotaryCoder((int)nowAngle.rightAngle));
                yield return new WaitForSeconds(0.5f);
                new Thread(GameDataManager.Uno.sendData).Start(Left_degreeConvertToRotaryCoder((int)nowAngle.leftAngle) + " " + Right_degreeConvertToRotaryCoder((int)nowAngle.rightAngle));
            }
        }
        

    }
}
