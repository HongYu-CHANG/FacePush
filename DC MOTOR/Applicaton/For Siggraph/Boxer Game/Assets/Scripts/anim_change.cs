using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class anim_change : MonoBehaviour {

    private Animator _animator;
    int count = 0;
    public static int s = 1;
    private bool isGameOver = false;
	//control
	public int control = 1;
    public int auto = 1;
	private int anim_control = 0;
	private int anim = 0;

	public int key = 0;
	private int ketdown = 3;

    // Use this for initialization
    void Start () {
        _animator = this.GetComponent<Animator>();
        Random.InitState(1337);
        count = 0;
        s = 1;
    }

    
	// Update is called once per frame
	void Update () {
        if (AnimatorIsPlaying() && _animator.GetCurrentAnimatorStateInfo(0).IsName("State") && !isGameOver)
        {
            if (count == 0)
            {
                s = Random.Range(1, 6);
				//control
				//if(auto == 0) s = control;     //control all anim
				if (auto == 0)  //control anim in 2 state
				{
					if(control == 1)
					{
						if (anim_control % 5 == 0) anim = 1;
						if (anim_control % 5 == 1) anim = 2;
						if (anim_control % 5 == 2) anim = 2;
						if (anim_control % 5 == 3) anim = 1;
						if (anim_control % 5 == 4) anim = 5;
						anim_control++;
						s = anim;
					}
					else if(control == 2)
					{
						if (anim_control % 5 == 0) anim = 3;
						if (anim_control % 5 == 1) anim = 4;
						if (anim_control % 5 == 2) anim = 3;
						if (anim_control % 5 == 3) anim = 3;
						if (anim_control % 5 == 4) anim = 4;
						anim_control++;
						s = anim;
					}
				}
				else//auto
				{
					if (anim_control % 14 == 0) anim = 5;
					else if (anim_control % 14 == 1) anim = 2;
					else if (anim_control % 14 == 2) anim = 1;
					else if (anim_control % 14 == 3) anim = 3;
					else if (anim_control % 14 == 4) anim = 4;
					else if (anim_control % 14 == 5) anim = 1;
					else if (anim_control % 14 == 6) anim = 2;
					else if (anim_control % 14 == 7) anim = 4;
					else if (anim_control % 14 == 8) anim = 5;
					else if (anim_control % 14 == 9) anim = 4;
					else if (anim_control % 14 == 10) anim = 2;
					else if (anim_control % 14 == 11) anim = 5;
					else if (anim_control % 14 == 12) anim = 2;
					else if (anim_control % 14 == 13) anim = 3;
					anim_control++;
					s = anim;
				}
				if (key == 1) s = ketdown;
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


		//control
		 //control all anim
		if (Input.GetKeyDown(KeyCode.Q)) ketdown = 1;
		else if (Input.GetKeyDown(KeyCode.W)) ketdown = 2;
		else if (Input.GetKeyDown(KeyCode.E)) ketdown = 3;
		else if (Input.GetKeyDown(KeyCode.R)) ketdown = 4;
		else if (Input.GetKeyDown(KeyCode.T)) ketdown = 5;
		
		//control anim in 2 state
		if (Input.GetKeyDown(KeyCode.Alpha1)) { control = 1; anim_control = 0; }
		else if (Input.GetKeyDown(KeyCode.Alpha2)) { control = 2; anim_control = 0; }

		if (Input.GetKeyDown(KeyCode.A)) auto = 0;

    }

    private bool AnimatorIsPlaying()
    {
        return _animator.GetCurrentAnimatorStateInfo(0).length >
               _animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }

    public void GameOver()
    {
        isGameOver = true;
    }
}
