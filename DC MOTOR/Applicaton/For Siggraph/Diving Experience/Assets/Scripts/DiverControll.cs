﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiverControll : MonoBehaviour {
    //Diver Body
    public Transform driverLeftHand;
    public Transform driverRightHand;
    public Transform driver;
    public Transform directionContorl;       
    private Vector3 LlastPos;//左手上一次的位置
    private Vector3 RlastPos;//右手上一次的位置
    private Vector3 Lvector;//左手向量
    private Vector3 Rvector;//右手向量
    private Vector3 diveDirection; //身體的方向

    //旋轉角度 移動offset 向量和 啟動bool
    private Vector3 LRvector; // 兩向量相加
    private float body_vector_angle;
    private float offset = 1;
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
        Debug.DrawRay(driver.transform.position, diveDirection * 10, Color.red, 10);
        if ((Rvector.magnitude > 0.05f || Lvector.magnitude > 0.05f) && isStarting && (Lvector.x >= 0 && Lvector.z >= 0 && Rvector.x >= 0 && Rvector.z >= 0))
        {
            Debug.DrawRay(driverLeftHand.transform.position, Lvector, Color.red, 10);
            Debug.DrawRay(driverRightHand.transform.position, Rvector, Color.red, 10);
            if ((Lvector.magnitude - Rvector.magnitude) > 0.05f) //trun right
            {
                Debug.LogWarning("Right!!");
                rotateValue = Vector3.up * body_vector_angle * 0.8f;
            }
            else if ((Rvector.magnitude - Lvector.magnitude) > 0.05f)//turn left
            {
                Debug.LogWarning("Left!!");
                rotateValue = Vector3.down * body_vector_angle * 0.8f;
            }
            else if (Mathf.Abs(Rvector.magnitude - Lvector.magnitude) < 0.02f)
            {
                Debug.LogWarning("Foward!!");
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
        Debug.Log("rotateValue = " + Mathf.RoundToInt(rotateValue.magnitude));
        Debug.Log("LRvector = " + Mathf.RoundToInt(LRvector.magnitude));
        Debug.Log("offset = " + Mathf.RoundToInt(offset));


        //reset parameter
        LlastPos = driverLeftHand.position;
         RlastPos = driverRightHand.position;
         Lvector = Vector3.zero;
         Rvector = Vector3.zero;
        if (rotateValue.magnitude <= 10)// 為了讓旋轉的值可以很快歸零，因為要讓它不要一值有殘餘的值
            rotateValue = Vector3.zero;
        if (offset > 0.2f) offset -= 0.2f;
        else offset = 0;

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
}
