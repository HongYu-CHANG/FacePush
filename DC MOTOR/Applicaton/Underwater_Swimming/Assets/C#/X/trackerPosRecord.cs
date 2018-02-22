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
	private float Lspeed = 0.75f;
	private float Rspeed = 0.75f;
	private float offset = 0;

    //motor
    public GameObject RMotor;
    public GameObject LMotor;
    private OSCSender ROSCSender;
    private OSCSender LOSCSender;


    // Use this for initialization
    void Start () {
		
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
                //counter == frameSegment - 1 -> get tracker position in this frame
                Lvector = LlastPos - Ltracker.transform.position;
                Rvector = RlastPos - Rtracker.transform.position;
                body_head_direction = headpos.transform.position - bodypos.transform.position;

                Debug.DrawRay(Ltracker.transform.position, Lvector, Color.red, drawRayTime);
                Debug.DrawRay(Rtracker.transform.position, Rvector, Color.red, drawRayTime);
                Debug.DrawRay(bodypos.transform.position, body_head_direction * 10, Color.red, drawRayTime);

				
                //L direction

				if ((Lvector + body_head_direction).magnitude - body_head_direction.magnitude > 0.001f)
				//if(Vector3.Angle(Lvector, body_head_direction) < 90)
                {
                    Debug.Log("L rotate !");

                    //rotation
					/*
					if (Lvector.z < 0) 
						transform.Rotate(Vector3.up * (Lvector - body_head_direction).magnitude * 0.8f);
					else 
						transform.Rotate(Vector3.down * (Lvector - body_head_direction).magnitude * 0.8f);
					*/

					transform.Rotate(Vector3.up * (Lvector - body_head_direction).magnitude * 0.8f);

					/*
                    if((Ltracker.transform.position.x + Rtracker.transform.position.x <= 0.1 && Ltracker.transform.position.x + Rtracker.transform.position.x >= -0.1)
                        && (Ltracker.transform.position.y + Rtracker.transform.position.y <= 0.1 && Ltracker.transform.position.y + Rtracker.transform.position.y >= -0.1)
                        && (Ltracker.transform.position.z + Rtracker.transform.position.z <= 0.1 && Ltracker.transform.position.z + Rtracker.transform.position.z >= -0.1))
                    {
                        // vectos of 2 hands seem to be the same -> don't rotate, just add some speed and offset
                        Debug.Log("L & R are almost the same");
						Debug.Log(Ltracker.transform.position.x + Rtracker.transform.position.x);
                    }
					*/

					/*
					if (Ltracker.transform.position.x > transform.position.x)
					{
						if (Lvector.z < 0)
							transform.Rotate(Vector3.up * (Lvector - body_head_direction).magnitude);
						else
							transform.Rotate(Vector3.down * (Lvector - body_head_direction).magnitude);
					}
					else
					{
						if (Lvector.z < 0)
							transform.Rotate(Vector3.down * (Lvector - body_head_direction).magnitude);
						else
							transform.Rotate(Vector3.up * (Lvector - body_head_direction).magnitude);
					}
					*/

					//speed
					Lspeed = (Lvector - body_head_direction).magnitude * 2;

					Debug.Log("L speed " + Lspeed);

					//速度夠快，之後就算沒有划手臂，身體也會有往前滑行的速度
					if (Lspeed > 1.50f && Lspeed < 1.60f) offset += 0.05f;
					if (Lspeed > 1.60f && Lspeed < 1.62f) offset += 0.1f;
					else if (Lspeed > 1.62f && Lspeed < 1.64f) offset += 0.2f;
					else if (Lspeed > 1.64f && Lspeed < 1.66f) offset += 0.4f;
					else if (Lspeed > 1.66f) offset += 0.8f;

                    Debug.Log("L offset: " + offset);
					Debug.Log("angle:" + Vector3.Angle(body_head_direction, Vector3.forward));
					Debug.Log("Lvector.z: " + Lvector.z);
					Debug.Log("L turn, pos: " + transform.position);
					
				}


                //R direction

                //if ((Rvector + body_head_direction).magnitude > body_head_direction.magnitude)
				if ((Rvector + body_head_direction).magnitude - body_head_direction.magnitude > 0.001f)
                {
                    Debug.Log("R rotate !");

                    //rotation
                    
					/*
					if (Rvector.z < 0)
						transform.Rotate(Vector3.up * (Rvector - body_head_direction).magnitude * 0.8f);
					else
						transform.Rotate(Vector3.down * (Rvector - body_head_direction).magnitude * 0.8f);
					*/

					transform.Rotate(Vector3.down * (Rvector - body_head_direction).magnitude * 0.8f);

					/*
                    //if (Rtracker.transform.position.x > transform.position.x)
					//if (Vector3.Angle(initial_body_head_direction,body_head_direction) < 160)
					if(Vector3.Angle(body_head_direction, Vector3.forward) > 22)
					{
						Debug.Log("> 22");
						if (Rvector.z < 0)
							transform.Rotate(Vector3.up * (Rvector - body_head_direction).magnitude);
						else
							transform.Rotate(Vector3.down * (Rvector - body_head_direction).magnitude);
					}
					else
					{
						Debug.Log("< 22");
						if (Rvector.z < 0)
							transform.Rotate(Vector3.down * (Rvector - body_head_direction).magnitude);
						else
							transform.Rotate(Vector3.up * (Rvector - body_head_direction).magnitude);
					}
					*/

					//speed
					Rspeed = (Rvector - body_head_direction).magnitude * 2;
					Debug.Log("R speed " + Rspeed);

					//速度夠快，之後就算沒有划手臂，身體也會有往前滑行的速度，會隨時間漸慢
					if (Rspeed > 1.50f && Rspeed < 1.60f) offset += 0.05f;
					if (Rspeed > 1.60f && Rspeed < 1.62f) offset += 0.1f;
					else if (Rspeed > 1.62f && Rspeed < 1.64f) offset += 0.2f;
					else if (Rspeed > 1.64f && Rspeed < 1.66f) offset += 0.4f;
					else if (Rspeed > 1.66f) offset += 0.8f;
					Debug.Log("R offset: " + offset);
					Debug.Log("angle:" + Vector3.Angle(body_head_direction, Vector3.forward));
					Debug.Log("Rvector.z: " + Rvector.z);

                    //transform.position = transform.position + Rvector * 2f;
                    //transform.position = transform.position - Vector3.Reflect(Rvector, body_head_direction) * 2f;
                    //transform.position = transform.position + new Vector3(Rvector.x,0,0)*2f;

                    Debug.Log("R turn, pos: " + transform.position);
				}



				//default: swim forward
				transform.position = transform.position + new Vector3(body_head_direction.x, 0, body_head_direction.z) * (Lspeed + Rspeed + offset) * 0.01f; 


				//reset
				LlastPos = Ltracker.transform.position;
                RlastPos = Rtracker.transform.position;
                Lvector = Vector3.zero;
                Rvector = Vector3.zero;
                counter = 0;
				Lspeed = 0.75f;
				Rspeed = 0.75f;
				if (offset > 0.15f) offset -= 0.15f;
				else offset = 0;

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
