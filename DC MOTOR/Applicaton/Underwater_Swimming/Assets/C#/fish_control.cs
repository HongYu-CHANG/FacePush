using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class fish_control : MonoBehaviour {
    //motor
    public GameObject RMotor;
    public GameObject LMotor;
    private OSCSender ROSCSender;
    private OSCSender LOSCSender;
    public static int fish = 0;

    // Use this for initialization
    void Start () {
        //motor control
        ROSCSender = RMotor.GetComponent<OSCSender>();
        ROSCSender.setWhichMotor("R");
        LOSCSender = LMotor.GetComponent<OSCSender>();
        LOSCSender.setWhichMotor("L");

		transform.DOLocalMove(new Vector3(20, 0.8f, -0.07f), 5);
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.F))
        {
			//
			transform.localPosition = new Vector3(20, 0.8f, -0.07f);
			Sequence mySequence = DOTween.Sequence();
            mySequence.Append(transform.DOLocalMoveX(-20, 3));
            mySequence.Append(transform.DOLocalMoveZ(20, 1f).SetDelay(2));
            mySequence.Append(transform.DOLocalMoveX(20, 0.5f));
            mySequence.Append(transform.DOLocalMoveZ(-0.07f, 1f));

            StartCoroutine(No1Work());
            fish = 1;
            Debug.Log("fish");
        }

        if (fish == 1 && transform.localPosition.z == 20) fish = 0;

		if (Input.GetKeyDown(KeyCode.B))
		{
			transform.DOLocalMoveX(-20, 3);
		}


	}

    IEnumerator No1Work()
    {
        float waitingTime = 2f;
        int speed = 100;
        float tempTime = UnityEngine.Random.Range(0.01f, 0.03f);//tempTime = 0;
        yield return new WaitForSeconds(waitingTime);

        ROSCSender.SendOSCMessageTriggerMethod(10, speed);
        LOSCSender.SendOSCMessageTriggerMethod(170, speed);
        yield return new WaitForSeconds(0.15f - tempTime);
        LOSCSender.SendOSCMessageTriggerMethod(10, speed);
        ROSCSender.SendOSCMessageTriggerMethod(170, speed);
        yield return new WaitForSeconds(0.15f + tempTime);
        ROSCSender.SendOSCMessageTriggerMethod(10, speed);
        LOSCSender.SendOSCMessageTriggerMethod(170, speed);
        yield return new WaitForSeconds(0.15f - tempTime);
        LOSCSender.SendOSCMessageTriggerMethod(10, speed);
        ROSCSender.SendOSCMessageTriggerMethod(170, speed);
        yield return new WaitForSeconds(0.15f + tempTime);
        ROSCSender.SendOSCMessageTriggerMethod(10, speed);
        LOSCSender.SendOSCMessageTriggerMethod(170, speed);
        yield return new WaitForSeconds(0.15f - tempTime);
        LOSCSender.SendOSCMessageTriggerMethod(10, speed);
        ROSCSender.SendOSCMessageTriggerMethod(170, speed);
        yield return new WaitForSeconds(0.15f + tempTime);
        ROSCSender.SendOSCMessageTriggerMethod(10, speed);
        LOSCSender.SendOSCMessageTriggerMethod(170, speed);
		yield return new WaitForSeconds(0.15f - tempTime);
		LOSCSender.SendOSCMessageTriggerMethod(10, speed);
		ROSCSender.SendOSCMessageTriggerMethod(170, speed);
		yield return new WaitForSeconds(0.15f + tempTime);
		ROSCSender.SendOSCMessageTriggerMethod(10, speed);
		LOSCSender.SendOSCMessageTriggerMethod(170, speed);
		yield return new WaitForSeconds(0.15f - tempTime);
		LOSCSender.SendOSCMessageTriggerMethod(10, speed);
		ROSCSender.SendOSCMessageTriggerMethod(170, speed);
		yield return new WaitForSeconds(0.15f + tempTime);
		ROSCSender.SendOSCMessageTriggerMethod(10, speed);
		LOSCSender.SendOSCMessageTriggerMethod(170, speed);
		yield return new WaitForSeconds(0.15f - tempTime);
		LOSCSender.SendOSCMessageTriggerMethod(10, speed);
		ROSCSender.SendOSCMessageTriggerMethod(170, speed);
		yield return new WaitForSeconds(0.15f + tempTime);
		ROSCSender.SendOSCMessageTriggerMethod(10, speed);
		LOSCSender.SendOSCMessageTriggerMethod(170, speed);
		yield return new WaitForSeconds(0.15f - tempTime);
		ROSCSender.SendOSCMessageTriggerMethod(10, speed);
		LOSCSender.SendOSCMessageTriggerMethod(10, speed);
	}
}
