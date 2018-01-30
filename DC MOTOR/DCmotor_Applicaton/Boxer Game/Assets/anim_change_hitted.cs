using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class anim_change_hitted : MonoBehaviour {

    private Animator _animator;
    int count = 0;
    int s = 0;

    // Use this for initialization
    void Start () {
        _animator = this.GetComponent<Animator>();
        //Debug.Log("s: "+anim_change.s);
        _animator.SetInteger("change", anim_change.s);
        count = 0;
    }
	
	// Update is called once per frame
	void Update () {

        if (s != anim_change.s) {
            Debug.Log("s: " + anim_change.s);
            _animator.SetInteger("change", anim_change.s);
            s = anim_change.s;
        }

        
        /*if (!AnimatorIsPlaying() && _animator.GetCurrentAnimatorStateInfo(0).IsName("State"))
        {
            if (count == 0)
            {
                //Debug.Log("s: " + anim_change.s);
                _animator.SetInteger("change", anim_change.s);
            }
            count = 1;
        }
        else
        {
            if (count == 1)
            {
                //Debug.Log("s: " + anim_change.s);
                _animator.SetInteger("change", anim_change.s);
            }
        }*/
    }

    

    

    bool AnimatorIsPlaying()
    {
        return _animator.GetCurrentAnimatorStateInfo(0).length >
               _animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }
}
