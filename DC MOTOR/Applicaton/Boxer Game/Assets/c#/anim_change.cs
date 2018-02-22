using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class anim_change : MonoBehaviour {

    private Animator _animator;
    int count = 0;
    public static int s = 1;

    // Use this for initialization
    void Start () {
        _animator = this.GetComponent<Animator>();
        Random.InitState(1337);
        count = 0;
        s = 1;
    }

    
	// Update is called once per frame
	void Update () {
        if (!AnimatorIsPlaying() && _animator.GetCurrentAnimatorStateInfo(0).IsName("State"))
        {
            if (count == 0)
            {
                s = Random.Range(1, 6);
                _animator.SetInteger("change", s);
                Debug.Log(s);
            }
            count = 1;
        }
        else 
        {
            if (count == 1)
            {
                s = 0;
                _animator.SetInteger("change", s);
                count = 0;
            }
        } 

    }

    bool AnimatorIsPlaying()
    {
        return _animator.GetCurrentAnimatorStateInfo(0).length >
               _animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }
}
