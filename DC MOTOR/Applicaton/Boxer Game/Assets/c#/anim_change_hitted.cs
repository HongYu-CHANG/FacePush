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
        
        _animator.SetInteger("change", anim_change.s);
        count = 0;
    }
	
	// Update is called once per frame
	void Update () {

        if (s != anim_change.s) {
            
            _animator.SetInteger("change", anim_change.s);
            s = anim_change.s;
        }  
        
    }


    bool AnimatorIsPlaying()
    {
        return _animator.GetCurrentAnimatorStateInfo(0).length >
               _animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }
}
