using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class trackerPosRecord_v2 : MonoBehaviour {

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

	private bool posInitialized = false;
	public float drawRayTime;

	//motor
	public GameObject RMotor;
	public GameObject LMotor;
	private OSCSender ROSCSender;
	private OSCSender LOSCSender;

	//v2: 用合力計算旋轉角度 & 移動
	private Vector3 LRvector;
	private float body_vector_angle;
	private int i = 0;
    private float offset = 1;
    private bool rotated = false;


	// Use this for initialization
	void Start()
	{

		Ltracker.gameObject.SetActive(true);

		//motor control
		ROSCSender = RMotor.GetComponent<OSCSender>();
		ROSCSender.setWhichMotor("R");
		LOSCSender = LMotor.GetComponent<OSCSender>();
		LOSCSender.setWhichMotor("L");

		LlastPos = Ltracker.transform.position;
		RlastPos = Rtracker.transform.position;
		body_head_direction = headpos.transform.position - bodypos.transform.position;

		//draw xyz axis
		Debug.DrawLine(new Vector3(-1000, 0, 0), new Vector3(1000, 0, 0), Color.yellow, 10000);
		Debug.DrawLine(new Vector3(0, -1000, 0), new Vector3(0, 1000, 0), Color.cyan, 10000);
		Debug.DrawLine(new Vector3(0, 0, -1000), new Vector3(0, 0, 1000), Color.white, 10000);

	}

	// Update is called once per frame
	void Update()
	{

	}

	void FixedUpdate()
	{

		if (Input.GetKeyDown(KeyCode.H))
		{
			Debug.Log("Press H");
			transform.position = (Ltracker.transform.position + Rtracker.transform.position) / 2;
			Debug.Log("swimmer's position: " + transform.position);
			posInitialized = true;
		}

		if (posInitialized == true)
		{

			if (counter < frameSegment - 1)
			{
				//do not get tracker position in this frame
				counter++;
			}
			else
			{
				//counter == frameSegment - 1 -> get tracker position in this frame

				Lvector = LlastPos - Ltracker.transform.position;
				Rvector = RlastPos - Rtracker.transform.position;
				body_head_direction = headpos.transform.position - bodypos.transform.position;

				Debug.DrawRay(Ltracker.transform.position, Lvector, Color.red, drawRayTime);
				Debug.DrawRay(Rtracker.transform.position, Rvector, Color.red, drawRayTime);
				Debug.DrawRay(bodypos.transform.position, body_head_direction * 10, Color.red, drawRayTime);

				//Debug.Log("L: " + Lvector.ToString("f4") + Lvector.magnitude);
				//Debug.Log("R: " + Rvector.ToString("f4") + Rvector.magnitude);
				LRvector = Lvector + Rvector;
				body_vector_angle = Vector3.Angle(new Vector3(body_head_direction.x, 0, body_head_direction.z), new Vector3(LRvector.x, 0, LRvector.z));
                //Debug.Log("angle = " + body_vector_angle);

                //rotation
                if (Rvector.magnitude > 0.05f || Lvector.magnitude > 0.05f)
				{
					if(Mathf.Abs(Rvector.magnitude - Lvector.magnitude) > 0.025f && LRvector.x > 0)
					{
						if (Rvector.magnitude < Lvector.magnitude)
						{
                            // L > R: turn right
                            transform.Rotate(Vector3.up * body_vector_angle * 0.05f);
                            //transform.DORotate(Vector3.up * body_vector_angle * 0.1f, 0.06f);//旋转动画

                            //motor
                            //use R motor to tighten, speed = ?, angle = ?
                            //R motor loosen

                            //format: StartCoroutine(No1Work(bool R, bool L, int angle, int speed));
							
                            if (rotated == false && fish_control.fish == 0 && shark_control.shark == 0)
                            {
                                StartCoroutine(No1Work(true, false, 0, 0));
                                Debug.Log("turn right, L motor");
                            }
							
                            

                        }
						else
						{
                            // R > L: turn left
                            transform.Rotate(Vector3.down * body_vector_angle * 0.05f);
                            //transform.DORotate(Vector3.down * body_vector_angle * 0.1f, 0.06f);

                            //motor
                            //use L motor,to tighten, speed = ?, angle = ?
                            //L motor loosen
							
							
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
						Debug.Log("dont rotate");
						rotated = false;
					}

				}

				//Debug.Log("LR: " + LRvector.ToString("f4") + LRvector.magnitude);
				//Debug.Log("offset = " + offset.ToString("F4"));

                //offset control
				if (LRvector.magnitude > 0.01f && LRvector.magnitude < 0.02f) offset += 0.1f;
				else if (LRvector.magnitude > 0.02f && LRvector.magnitude < 0.03f) offset += 0.2f;
				else if (LRvector.magnitude > 0.03f && LRvector.magnitude < 0.04f) offset += 0.3f;
				else if (LRvector.magnitude > 0.04f && LRvector.magnitude < 0.05f) offset += 0.4f;
				else if (LRvector.magnitude > 0.05f) offset += 0.5f;
				
                //swim forward
				if(rotated)
					transform.position = transform.position + new Vector3(body_head_direction.x, 0, body_head_direction.z) * (LRvector.magnitude + offset) * 0.0005f ;
                else
                {
                    transform.position = transform.position + new Vector3(body_head_direction.x, 0, body_head_direction.z) * (LRvector.magnitude + offset) * 0.2f;

					//motor
					//use L & R motors to tighten, speed = ?, angle = ?
					//L motor loosen gradually (offset == 0 -> free)

					Debug.Log("---for go straight---");
                    //Debug.Log("body_vector_angle: " + body_vector_angle);
                    //Debug.Log("LRvector.magnitude + offset：" + (LRvector.magnitude + offset));

                    
                    if (fish_control.fish == 1) Debug.Log("fish!!");
                    else if (shark_control.shark == 1) Debug.Log("shark!!");
                    else if((int)(LRvector.magnitude + offset) >= 15)
                    {
                        StartCoroutine(No1Work(false, false, 150, 100));
                        Debug.Log("move forward, default");
                    }
					else if ((int)(LRvector.magnitude + offset) > 1)
					{
						StartCoroutine(No1Work(false, false, (int)(LRvector.magnitude + offset) * 10, 100));
						Debug.Log("move forward, loosen");
					}
                    else
                    {
                        //放鬆
                        StartCoroutine(No1Work(false, false, 10, 100));
                        Debug.Log("move forward, free");
                    }
                    

                }

                //reset
                LlastPos = Ltracker.transform.position;
				RlastPos = Rtracker.transform.position;
				Lvector = Vector3.zero;
				Rvector = Vector3.zero;
				counter = 0;
				if (offset > 0.2f)  offset -= 0.2f;
				else  offset = 0;
				rotated = false;
			}

		}

	}

    //motor control

    //StartCoroutine(No1Work(bool R, bool L, int angle, int speed));

    IEnumerator No1Work(bool R, bool L, int angle, int speed)
    {
        float waitingTime = 1f;
        int rotateSpeed = 150;

        if (R)//右轉，要動右馬達 (1,0)
        {
            ROSCSender.SendOSCMessageTriggerMethod(120, rotateSpeed);//加壓   // (角度0~180, 速度0~255)
            LOSCSender.SendOSCMessageTriggerMethod(20, rotateSpeed);
            yield return new WaitForSeconds(waitingTime);
            ROSCSender.SendOSCMessageTriggerMethod(10, rotateSpeed);//加壓   // (角度0~180, 速度0~255)
            LOSCSender.SendOSCMessageTriggerMethod(10, rotateSpeed);
        }
        else if (L)//左轉，要動左馬達 (0,1)
        {
            ROSCSender.SendOSCMessageTriggerMethod(20, rotateSpeed);//加壓   // (角度0~180, 速度0~255)
            LOSCSender.SendOSCMessageTriggerMethod(150, rotateSpeed);
            yield return new WaitForSeconds(waitingTime);
            ROSCSender.SendOSCMessageTriggerMethod(10, rotateSpeed);//加壓   // (角度0~180, 速度0~255)
            LOSCSender.SendOSCMessageTriggerMethod(10, rotateSpeed);
        }
        else //前進，兩馬達都要動 (0,0)
        {
            ROSCSender.SendOSCMessageTriggerMethod(angle, speed);
            LOSCSender.SendOSCMessageTriggerMethod(angle + 20, speed);
            //yield return new WaitForSeconds(waitingTime);
        }
    }



    /*
    IEnumerator No1Work(bool R, bool L, bool click)
    {
        float time;
        if (R)
            time = Rmotor_Time.value;
        else
            time = Lmotor_Time.value;

        if (click)//奇數次點擊
        {
            if (R) ROSCSender.SendOSCMessageTriggerMethod(100, RSpeed);//加壓
            if (L) LOSCSender.SendOSCMessageTriggerMethod(100, LSpeed);
            yield return new WaitForSeconds(time);
        }
        else if()
        {
            if (R) ROSCSender.SendOSCMessageTriggerMethod(20, RSpeed);//加壓
            if (L) LOSCSender.SendOSCMessageTriggerMethod(20, LSpeed);
            yield return new WaitForSeconds(time);
        }
        else if()
    }
    */

    /*
    IEnumerator No1Work(bool R, bool L, int state )
	{
		float time = 0.5f;
		int RSpeed = 50;
		int LSpeed = 50;
		int angle = 150;
		int langle = 150;

		if (R)//奇數次點擊
		{
			if (state == 1 || state == 2 || state == 5) { RSpeed = 200; angle = 145; Debug.Log("R 重 "); }
			else if (state == 3 || state == 4) { RSpeed = 150; angle = 95; Debug.Log("R 輕 "); }
			//Debug.Log("state " + state);		
			ROSCSender.SendOSCMessageTriggerMethod(angle, RSpeed);//加壓
			yield return new WaitForSeconds(time);
			ROSCSender.SendOSCMessageTriggerMethod(10, RSpeed);
		}
		else if(L)
		{
			if(state == 1 || state == 2 || state == 5) { LSpeed = 200; angle = 150; Debug.Log("L 重 "); }
			else if (state == 3 || state == 4) { LSpeed = 150; angle = 100; Debug.Log("L 輕 "); }
			//Debug.Log("state "+state);
			LOSCSender.SendOSCMessageTriggerMethod(angle, LSpeed);//加壓
			yield return new WaitForSeconds(time);
			LOSCSender.SendOSCMessageTriggerMethod(10, LSpeed);
			
		}
		else
		{
			if (state == 1 || state == 2 || state == 5) { RSpeed = 200; angle = 145; langle = 150; Debug.Log("C 重 "); }
			else if (state == 3 || state == 4) { RSpeed = 150; angle = 95; langle = 100; Debug.Log("C 輕 "); }
			//Debug.Log("state " + state);
			ROSCSender.SendOSCMessageTriggerMethod(angle, RSpeed);//加壓
			LOSCSender.SendOSCMessageTriggerMethod(langle, RSpeed);
			yield return new WaitForSeconds(time);
			ROSCSender.SendOSCMessageTriggerMethod(10, RSpeed);
			LOSCSender.SendOSCMessageTriggerMethod(10, RSpeed);
		}
		
	} 
    */


}
