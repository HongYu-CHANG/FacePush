using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cam_pos : MonoBehaviour {

    /*public GameObject root;
    private Quaternion rot;
    private Vector3 pos;
    private Vector3 rots;*/
    int s = 0;
    int count = 0;
    private Vector3 offset = new Vector3(0.3f, 0.2f, 0f);//相机相对于玩家的位置
    private Vector3 pos;
    public float speed = 5;
    private Vector3 ori_pos;
    private Quaternion ori_rot;
    private Transform target;
    private Transform Ltarget;
    private Vector3 post_pos;
    float n = 1;
    float l = 0.5f;

    // Use this for initialization
    void Start () {
        /*rot = root.transform.rotation;
        rots = root.transform.rotation.eulerAngles;
        pos = root.transform.position;*/
        count = 0;
        ori_pos = this.transform.position;
        ori_rot = this.transform.rotation;
        target = GameObject.FindGameObjectWithTag("R").transform;
        Ltarget = GameObject.FindGameObjectWithTag("L").transform;

        post_pos = target.position;
    }
    Quaternion angel;
    // Update is called once per frame
    void Update () {
        if (s != anim_change.s)
        {
            
            if (anim_change.s == 1 )
            {
                offset = this.transform.position - target.position;
                l = 2f;
                speed = 15;
                count = 0;
                s = anim_change.s;
                //angel = Quaternion.LookRotation((target.position - this.transform.position)*0.01f);
               
            }
            else if (anim_change.s == 2 )
            {
                offset = this.transform.position - target.position;
                l = 2f;
                speed = 15;
                count = 0;
                s = anim_change.s;
               // Debug.Log("cams: " + anim_change.s);
            }
            else if (anim_change.s == 3 )
            {
                
                offset = this.transform.position - target.position;
                l = 0.5f;
                speed = 5;
                count = 0;
                s = anim_change.s;
                //Debug.Log("cams: " + anim_change.s);
            }
            else if (anim_change.s == 4 )
            {
                offset = this.transform.position - target.position;
                l = 0.5f;
                speed = 5;
                count = 0;
                s = anim_change.s;
                //Debug.Log("cams: " + anim_change.s);
            }
            else if ((anim_change.s == 5 || anim_change.s == 0) )
            {
                //
                //speed = 10;
                offset = new Vector3(0, 0, 0);
                //this.transform.position = ori_pos;
                this.transform.rotation = ori_rot;
                offset = -offset;
                count = 0;
                s = anim_change.s;
              
            }




            
            count++;
        }
        
        pos = ori_pos + offset * l;
        this.transform.position = Vector3.Lerp(this.transform.position, pos, speed * Time.deltaTime);
        
        //this.transform.rotation = Quaternion.Slerp(this.transform.rotation, angel, speed * Time.deltaTime);
        //post_pos = target.position;


        /*this.transform.rotation = Quaternion.Euler(root.transform.rotation.eulerAngles - rots);    
        this.transform.position += root.transform.position - pos;
        rot = root.transform.rotation;
        pos = root.transform.position;*/
    }
}
