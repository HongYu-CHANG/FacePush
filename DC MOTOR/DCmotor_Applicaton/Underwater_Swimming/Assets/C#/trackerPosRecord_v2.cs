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
	private Vector3 initial_body_head_direction;

	private bool posInitialized = false;
	public float drawRayTime;
	private float Lspeed = 0.75f;
	private float Rspeed = 0.75f;
	private float offset = 1;

	//motor
	public GameObject RMotor;
	public GameObject LMotor;
	private OSCSender ROSCSender;
	private OSCSender LOSCSender;

	//v2
	private Vector3 LRvector;
	private float body_vector_angle;
	private int i = 0;
	private bool rotated = false;


	// Use this for initialization
	void Start()
	{

		//motor control
		ROSCSender = RMotor.GetComponent<OSCSender>();
		ROSCSender.setWhichMotor("R");
		LOSCSender = LMotor.GetComponent<OSCSender>();
		LOSCSender.setWhichMotor("L");

		LlastPos = Ltracker.transform.position;
		RlastPos = Rtracker.transform.position;
		body_head_direction = headpos.transform.position - bodypos.transform.position;
		initial_body_head_direction = body_head_direction;

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
				//body_vector_angle = Vector3.SignedAngle(new Vector3(body_head_direction.x, 0, body_head_direction.z), new Vector3(LRvector.x, 0, LRvector.z), Vector3.up);
				//Debug.Log("angle = " + body_vector_angle);

				//rotation
				if (Rvector.magnitude > 0.05f || Lvector.magnitude > 0.05f)
				//if (LRvector.x > 0.0003f)
				{
					/*
					//if(Mathf.Abs(Rvector.magnitude - Lvector.magnitude) > 0.025f)
					//{
						float rotate_sign = Mathf.Sign(Vector3.Cross(new Vector3(body_head_direction.x, 0, body_head_direction.z), new Vector3(LRvector.x, 0, LRvector.z)).y);
						transform.Rotate(Vector3.up * body_vector_angle * 0.1f * rotate_sign);
						Debug.Log("sign: " + rotate_sign + body_vector_angle);
					//}
					//else
					//{
					//Debug.Log("dont rotate");
					//}
					*/

					if(Mathf.Abs(Rvector.magnitude - Lvector.magnitude) > 0.025f && LRvector.x > 0)
					{
						if (Rvector.magnitude < Lvector.magnitude)
						//if (Lvector.magnitude > 0.05f)
						{
							transform.Rotate(Vector3.up * body_vector_angle * 0.03f);
							//transform.DORotate(Vector3.up * body_vector_angle * 0.1f, 0.06f);//旋转动画  
						}
						else
						{
							transform.Rotate(Vector3.down * body_vector_angle * 0.03f);
							//transform.DORotate(Vector3.down * body_vector_angle * 0.1f, 0.06f);
						}

						rotated = true;
					}
					else
					{
						Debug.Log("dont rotate");
						rotated = false;
					}
					
					


					Debug.Log("angle = " + body_vector_angle);
					//transform.Rotate(Vector3.down * body_vector_angle * 0.1f);

				}


				Debug.Log("LR: " + LRvector.ToString("f4") + LRvector.magnitude);
				Debug.Log("offset = " + offset.ToString("F4"));


				if (LRvector.magnitude > 0.01f && LRvector.magnitude < 0.02f) offset += 0.1f;
				else if (LRvector.magnitude > 0.02f && LRvector.magnitude < 0.03f) offset += 0.2f;
				else if (LRvector.magnitude > 0.03f && LRvector.magnitude < 0.04f) offset += 0.3f;
				else if (LRvector.magnitude > 0.04f && LRvector.magnitude < 0.05f) offset += 0.4f;
				else if (LRvector.magnitude > 0.05f) offset += 0.5f;
				
				if(rotated)
					transform.position = transform.position + new Vector3(body_head_direction.x, 0, body_head_direction.z) * (LRvector.magnitude + offset) * 0.001f ;
				else
					transform.position = transform.position + new Vector3(body_head_direction.x, 0, body_head_direction.z) * (LRvector.magnitude + offset) * 0.2f;

				/*
				//swim forward
				if (LRvector.x > 0)
				{
					if (i == 1)
					{
						offset = 0;
						i = 0;
					}
					transform.position = transform.position + new Vector3(body_head_direction.x, 0, body_head_direction.z) * (LRvector.x) * 20 * offset;
					offset += LRvector.x;
				}
				else
				{
					offset-= 0.1f;
					i = 1;
				}
				*/



				//reset
				LlastPos = Ltracker.transform.position;
				RlastPos = Rtracker.transform.position;
				Lvector = Vector3.zero;
				Rvector = Vector3.zero;
				//body_head_direction = headpos.transform.position - bodypos.transform.position;
				counter = 0;
				if (offset > 0.2f)  offset -= 0.2f;
				else  offset = 0;
				rotated = false;
			}

		}

	}

	//motor control

	//StartCoroutine(No1Work(false, true, Rbutton));
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
        else
        {
            if (R) ROSCSender.SendOSCMessageTriggerMethod(20, RSpeed);//加壓
            if (L) LOSCSender.SendOSCMessageTriggerMethod(20, LSpeed);
            yield return new WaitForSeconds(time);
        }

    }
    */

}
