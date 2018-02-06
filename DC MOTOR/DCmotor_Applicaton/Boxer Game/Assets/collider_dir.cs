using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collider_dir : MonoBehaviour {

    private Transform Ltarget;
    private Transform Rtarget;
    int Rcount = 0;
    int Lcount = 0;

    public Vector3[] Lpos = new Vector3[100];
    public Vector3[] Rpos = new Vector3[100];

    public static Vector3 Ldir;
    public static Vector3 Rdir;
    public static int Lhit;
    public static int Rhit;

    int s = 0;
    int frame = 80;

    public static Vector3 hit_pos;
    public static Vector3 pos;

    // Use this for initialization
    void Start () {
        Ltarget = GameObject.FindGameObjectWithTag("LHand").transform;
        Rtarget = GameObject.FindGameObjectWithTag("RHand").transform;
        Rcount = 0;
        Lcount = 0;
        for(int i = 0; i < 100; i++)
        {
            Lpos[i] = Ltarget.position;
            Rpos[i] = Rtarget.position;
        }

        Lhit = 0;
        Rhit = 0;
    }
	
	// Update is called once per frame
	void Update () {
        Lpos[Lcount % 100] = Ltarget.position;
        Rpos[Rcount % 100] = Rtarget.position;
        Lcount ++;
        Rcount ++;
        

        if (s != anim_change.s) {
            if (anim_change.s == 1 || anim_change.s == 3)
            {
                //Rcount = 0; Lcount = 0;
				s = anim_change.s;
            }
            else if (anim_change.s == 2 || anim_change.s == 4 || anim_change.s == 5)
			{
                //Lcount = 0; Rcount = 0;
				s = anim_change.s;
            }
			s = anim_change.s;
		}
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("RHand")){
            if(Rcount > 30 && Lcount > 30)
            {
                int num = (Rcount - frame) % 100;
                if (num < 0) num += 100; 
                Rdir = Rtarget.position - Rpos[num];
                //Debug.Log("R  " + Rdir.ToString("f4") + " " + Rcount);
                Rhit = 1;

                //distant between boxer's hand and face
                hit_pos = Rtarget.position - this.transform.position;
                pos = Rtarget.position;
            }
            //else Debug.Log("R  " + Rcount);
            Rcount = 0; Lcount = 0;
		}
        else if (other.gameObject.CompareTag("LHand"))
        {
            if (Lcount > 30 && Rcount > 30)
            {
                int num = (Lcount - frame) % 100;
                if (num < 0) num += 100;  
                Ldir = Ltarget.position - Lpos[num];
                //Debug.Log("L  " + Ldir.ToString("f4") + " " + Lcount);
                Lhit = 1;

                //distant between boxer's hand and face
                hit_pos = Ltarget.position - this.transform.position;
                pos = Ltarget.position;
                //hit_pos = GameObject.FindGameObjectWithTag("L").transform.position - this.transform.position;
            }
           // else Debug.Log("L  " + Lcount);
            Lcount = 0; Rcount = 0;
		}
    }
}
