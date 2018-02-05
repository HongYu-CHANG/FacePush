using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class hitted : MonoBehaviour {

    int s = 0;
    int state = 0;

    //hit_pos_on_face
    private GameObject hit;
    private Transform face;
    private Vector3 hit_position;
    int count = 0;
    Color color = Color.black;
    private Vector3 offset;
    private Vector3 move;
    private Vector3 hit_move;

    private Transform hit_face;

    //when hitted
    float time = 0.5f;
    float l = 0.7f;
    float r = 0.7f;
    float k = 0.7f;

    // Use this for initialization
    void Start () {
        //hit_pos_on_face
        hit = GameObject.FindGameObjectWithTag("Hit");
        face = GameObject.FindGameObjectWithTag("Face").transform;
        hit_position = hit.transform.position;
        hit.transform.localScale = new Vector3(0, 0, 0);
        color = hit.GetComponent<Renderer>().material.color;
        offset = face.position - hit.transform.position;

        hit_face = GameObject.FindGameObjectWithTag("hitted").transform;
    }
	
	// Update is called once per frame
	void Update () {
        //offset = face.position - hit.transform.position;
        //get animater in which state
        if (s != anim_change.s)
        {
            s = anim_change.s;
            if(anim_change.s != 0) state = anim_change.s;
        }

        

        if (collider_dir.Rhit == 1)
        {
            
            if (state == 1)
            {
                //moving position
                time = 0.2f;
                l = 1f;
                //moving rotation
                r = 10f;
                //hit
                color.a = 0.8f;
                //
                k = 1f;
                Debug.Log("R 重 ");
            }
            else if (state == 3)
            {
                //moving position
                time = 0.4f;
                l = 0.7f;
                //moving rotation
                r = 5f;
                //hit
                color.a = 0.5f;
                //
                k = 0.5f;
                Debug.Log("R 輕 ");
            }

            move = collider_dir.Rdir;
            move = move.normalized ;

            Vector3 pos = this.transform.position + collider_dir.Rdir * l;
            Sequence mySequence = DOTween.Sequence(); 
            
            Tweener move1 = transform.DOMove(pos, time, true);
            Tweener rot1 = transform.DORotate(this.transform.rotation.eulerAngles + new Vector3(r, 0, r), 0.2f);
            Tweener move2 = transform.DOMove(this.transform.position, 0.5f);
            Tweener rot2 = transform.DORotate(this.transform.rotation.eulerAngles, 0.2f);

            mySequence.Append(move1);
            mySequence.Join(rot1);
            mySequence.Append(move2);
            mySequence.Join(rot2);
            Debug.Log("Rhit ");
            collider_dir.Rhit = 0;

            //hit_pos_on_face
            if (collider_dir.hit_pos.x > 0.42) collider_dir.hit_pos.x = 0.42f;
            else if (collider_dir.hit_pos.x < -0.42) collider_dir.hit_pos.x = -0.42f;
            //Debug.Log(collider_dir.hit_pos.ToString("f4"));
            hit.transform.localScale = new Vector3(0.03f, 0.02f, 0.05f);
            hit.transform.position = new Vector3(face.position.x + collider_dir.hit_pos.x, face.position.y + collider_dir.hit_pos.y, face.position.z);
            hit_position = new Vector3(face.position.x + collider_dir.hit_pos.x, face.position.y + collider_dir.hit_pos.y, face.position.z);

            hit_face.position = collider_dir.pos;
            if (collider_dir.hit_pos.x > 0.16)
            {
                hit.transform.position = new Vector3(face.position.x + 0.27f, face.position.y, face.position.z);
                hit_move = hit.transform.position - hit_position;
            }
            else if (collider_dir.hit_pos.x < -0.16)
            {
                hit.transform.position = new Vector3(face.position.x - 0.27f, face.position.y, face.position.z);
                hit_move = hit.transform.position - hit_position;
            }
            else
            {
                hit.transform.position = face.position;
                hit_move = hit.transform.position - hit_position;
                hit.transform.localScale = new Vector3(0.04f, 0.02f, 0.05f);
            }
            hit.GetComponent<Renderer>().material.color = color;
            count ++ ;
        }
        else if (collider_dir.Lhit == 1)
        {
            if (state == 2)
            {
                //moving position
                time = 0.2f;
                l = 1.2f;
                //moving rotation
                r = 10f;
                //hit
                color.a = 0.8f;
                //
                k = 1f;
                Debug.Log("L 重 ");
            }
            else if (state == 4)
            {
                //moving position
                time = 0.5f;
                l = 1f;
                //moving rotation
                r = 5f;
                //hit
                color.a = 0.5f;
                //
                k = 0.5f;
                Debug.Log("L 輕 ");
            }
            else if (state == 5 )
            {
                //moving position
                time = 0.2f;
                l = 1.2f;
                //moving rotation
                r = 10f;
                //hit
                color.a = 0.8f;
                //
                k = 1f;
                Debug.Log("L 重 ");
            }

            move = collider_dir.Ldir;
            move = move.normalized ;

            Vector3 pos = this.transform.position + collider_dir.Ldir * l;
            Sequence mySequence = DOTween.Sequence();
            
            Tweener move1 = transform.DOMove(pos, time, true);
            Tweener rot1 = transform.DORotate(this.transform.rotation.eulerAngles + new Vector3(r, 0, -r), 0.2f);
            Tweener move2 = transform.DOMove(this.transform.position, 0.5f);
            Tweener rot2 = transform.DORotate(this.transform.rotation.eulerAngles, 0.2f);

            mySequence.Append(move1);
            mySequence.Join(rot1);
            mySequence.Append(move2);
            mySequence.Join(rot2);
            Debug.Log("Lhit ");
            collider_dir.Lhit = 0;

            //hit_pos_on_face
            if (collider_dir.hit_pos.x > 0.42) collider_dir.hit_pos.x = 0.42f;
            else if (collider_dir.hit_pos.x < -0.42) collider_dir.hit_pos.x = -0.42f;

            //Debug.Log(collider_dir.hit_pos.ToString("f4"));
            hit.transform.localScale = new Vector3(0.03f, 0.02f, 0.05f);
            hit.transform.position = new Vector3(face.position.x + collider_dir.hit_pos.x , face.position.y  + collider_dir.hit_pos.y , face.position.z);
            hit_position = new Vector3(face.position.x + collider_dir.hit_pos.x, face.position.y + collider_dir.hit_pos.y, face.position.z);

            hit_face.position = collider_dir.pos;
            if (collider_dir.hit_pos.x > 0.16) {
                hit.transform.position = new Vector3(face.position.x + 0.27f, face.position.y, face.position.z);
                hit_move = hit.transform.position - hit_position;
            }
            else if (collider_dir.hit_pos.x < -0.16)
            {
                hit.transform.position = new Vector3(face.position.x - 0.27f, face.position.y, face.position.z);
                hit_move = hit.transform.position - hit_position;   
            }
            else
            {
                hit.transform.position = face.position;
                hit_move = hit.transform.position - hit_position;
                hit.transform.localScale = new Vector3(0.04f, 0.02f, 0.05f);
            } 
            hit.GetComponent<Renderer>().material.color = color;
            count++;
            //Debug.DrawRay(hit.transform.position - collider_dir.hit_pos * l, collider_dir.hit_pos * l, Color.red);

        }


        if (count != 0) {
            count++;
            offset = hit.transform.position - hit_move - hit_position;
            Debug.DrawRay(hit_position + offset - move*k*2, move*k*2, Color.red);
        }
        if (count == 80) {
            count = 0;
            hit.transform.localScale = new Vector3(0, 0, 0);
        }

       
    }
}
