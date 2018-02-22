using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shark_control : MonoBehaviour {

    private Animator _animator;
    int count = 0;
    private int s = 1;
    private int t = 1;
    public static int shark = 0;

    //motor
    public GameObject RMotor;
    public GameObject LMotor;
    private OSCSender ROSCSender;
    private OSCSender LOSCSender;

    // Use this for initialization
    void Start () {
        transform.localPosition = new Vector3(-30f, 0.31f, -0.26f);
        Random.InitState(1337);
        _animator = this.GetComponent<Animator>();
        _animator.SetInteger("count", 0);
        _animator.SetInteger("start", 0);
        _animator.SetInteger("turn", 0);

        //motor control
        ROSCSender = RMotor.GetComponent<OSCSender>();
        ROSCSender.setWhichMotor("R");
        LOSCSender = LMotor.GetComponent<OSCSender>();
        LOSCSender.setWhichMotor("L");
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.S))
        {
            t = Random.Range(1, 3);
            s = 1;
            _animator.SetInteger("count", s);
            _animator.SetInteger("start", 1);
            _animator.SetInteger("turn", t);
			transform.localPosition = new Vector3(35.5f, 0.31f, -0.26f);
        }

        if (!AnimatorIsPlaying() && _animator.GetCurrentAnimatorStateInfo(0).IsName("Swiming"))
        {
            if (count == 0)
            {
                s ++;
                if(s == 6)
                _animator.SetInteger("count", 8);
                //Debug.Log(s);
            }
            count = 1;
            _animator.SetInteger("start", 0);
        }
        else
        {
            if (count == 1)
            {
                count = 0;
            }
        }

        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Turn Left"))
        {
            if (shark == 0) {
                StartCoroutine(No1Work(true, false));
                Debug.Log("shark_right");
            }
            shark = 1;
            //Debug.Log("shark_r");
        }
        else if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Turn Right"))
        {
            if (shark == 0) {
                StartCoroutine(No1Work(false, true));
                Debug.Log("shark_left");
            } 
            shark = 1;
            //Debug.Log("shark_l");
        }
        else shark = 0;

    }

    bool AnimatorIsPlaying()
    {
        return _animator.GetCurrentAnimatorStateInfo(0).length >
               _animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }

    IEnumerator No1Work(bool R, bool L)
    {
        float waitingTime = 1f;
        int rotateSpeed = 150;

        if (R)//右轉，要動右馬達 (1,0) t == 2
        {
            ROSCSender.SendOSCMessageTriggerMethod(120, rotateSpeed);//加壓   // (角度0~180, 速度0~255)
            LOSCSender.SendOSCMessageTriggerMethod(20, rotateSpeed);
            yield return new WaitForSeconds(waitingTime);
            ROSCSender.SendOSCMessageTriggerMethod(10, rotateSpeed);//加壓   // (角度0~180, 速度0~255)
            LOSCSender.SendOSCMessageTriggerMethod(10, rotateSpeed);
        }
        else if (L)//左轉，要動左馬達 (0,1) t == 1
        {
            ROSCSender.SendOSCMessageTriggerMethod(20, rotateSpeed);//加壓   // (角度0~180, 速度0~255)
            LOSCSender.SendOSCMessageTriggerMethod(150, rotateSpeed);
            yield return new WaitForSeconds(waitingTime);
            ROSCSender.SendOSCMessageTriggerMethod(10, rotateSpeed);//加壓   // (角度0~180, 速度0~255)
            LOSCSender.SendOSCMessageTriggerMethod(10, rotateSpeed);
        }
       
    }
}
