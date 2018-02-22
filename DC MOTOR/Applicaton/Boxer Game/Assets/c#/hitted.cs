using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class hitted : MonoBehaviour {

    int s = 0;
    int state = 0;

	//hit_pos_on_face
	public GameObject RMotor;
	public GameObject LMotor;
	private OSCSender ROSCSender;
	private OSCSender LOSCSender;
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

    public GameObject superGameObject;
    public Texture[] myTextures = new Texture[7];
    public GameObject head;
    private GameObject myLine;
    private Vector3 Line;

    // Use this for initialization
    void Start () {
		ROSCSender = RMotor.GetComponent<OSCSender>();
		ROSCSender.setWhichMotor("R");
		LOSCSender = LMotor.GetComponent<OSCSender>();
		LOSCSender.setWhichMotor("L");
		//hit_pos_on_face
		hit = GameObject.FindGameObjectWithTag("Hit");
        face = GameObject.FindGameObjectWithTag("Face").transform;
        hit_position = hit.transform.position;
        hit.transform.localScale = new Vector3(0, 0, 0);
        color = hit.GetComponent<Renderer>().material.color;
        offset = face.position - hit.transform.position;

        hit_face = GameObject.FindGameObjectWithTag("hitted").transform;
    }
	
	void Update () {
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
                //Debug.Log("R 重 ");
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
                //Debug.Log("R 輕 ");
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
            
            hit.transform.position = new Vector3(face.position.x + collider_dir.hit_pos.x, face.position.y + collider_dir.hit_pos.y, face.position.z);
            hit_position = new Vector3(face.position.x + collider_dir.hit_pos.x*0.5f, face.position.y + collider_dir.hit_pos.y * 0.5f, face.position.z);

            hit_face.position = collider_dir.pos;
            if (collider_dir.hit_pos.x > 0.2)
            {
                if (state == 3) head.GetComponent<Renderer>().material.mainTexture = myTextures[1];
                else head.GetComponent<Renderer>().material.mainTexture = myTextures[4];
                StartCoroutine(No1Work(true, false, state));
			}
            else if (collider_dir.hit_pos.x < -0.2)
            {
                if (state == 3) head.GetComponent<Renderer>().material.mainTexture = myTextures[3];
                else head.GetComponent<Renderer>().material.mainTexture = myTextures[6];
                StartCoroutine(No1Work(false, true, state));
			}
            else
            {
                if (state == 3) head.GetComponent<Renderer>().material.mainTexture = myTextures[2];
                else head.GetComponent<Renderer>().material.mainTexture = myTextures[5];
                StartCoroutine(No1Work(false, false, state));
			}
            count ++ ;
            Line = hit.transform.position;
            DrawLine(hit_position + move * k * 2, hit_position, 1f);

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
                //Debug.Log("L 重 ");
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
                //Debug.Log("L 輕 ");
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
                //Debug.Log("L 重 ");
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

            hit.transform.position = new Vector3(face.position.x + collider_dir.hit_pos.x , face.position.y  + collider_dir.hit_pos.y , face.position.z);
            hit_position = new Vector3(face.position.x + collider_dir.hit_pos.x * 0.5f, face.position.y + collider_dir.hit_pos.y * 0.5f, face.position.z);

            hit_face.position = collider_dir.pos;
            if (collider_dir.hit_pos.x > 0.2) {
                if (state == 4) head.GetComponent<Renderer>().material.mainTexture = myTextures[1];
                else head.GetComponent<Renderer>().material.mainTexture = myTextures[4];
                StartCoroutine(No1Work(true, false, state));
			}
            else if (collider_dir.hit_pos.x < -0.2)
            {
                if (state == 4) head.GetComponent<Renderer>().material.mainTexture = myTextures[3];
                else head.GetComponent<Renderer>().material.mainTexture = myTextures[6];
                StartCoroutine(No1Work(false, true, state));
			}
            else
            {
                if (state == 4) head.GetComponent<Renderer>().material.mainTexture = myTextures[2];
                else head.GetComponent<Renderer>().material.mainTexture = myTextures[5];
                StartCoroutine(No1Work(false, false, state));
			} 
            count++;
            Line = hit.transform.position;
            DrawLine(hit_position  + move * k * 2, hit_position , 1f);

        }


        if (count != 0) {
            count++;
            offset = hit.transform.position - hit_move - hit_position;
            offset = hit.transform.position - Line;
            if(myLine != null)
            {
                myLine.transform.position = hit_position + move * k * 2 + offset;
                LineRenderer lr = myLine.GetComponent<LineRenderer>();
                lr.SetPosition(0, hit_position + move * k * 2 + offset);
                lr.SetPosition(1, hit_position + offset);
            }
        }
        if (count == 80) {
            count = 0;
            hit.transform.localScale = new Vector3(0, 0, 0);
            head.GetComponent<Renderer>().material.mainTexture = myTextures[0];
        }
		

	}

    void DrawLine(Vector3 start, Vector3 end, float duration = 1f)
    {
        myLine = new GameObject();
        myLine.transform.SetParent(superGameObject.transform);
        myLine.transform.position = start;
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
        lr.startColor = Color.red;
        lr.endColor = Color.gray;
        lr.startWidth = 0.05f;
        lr.endWidth = 0.01f;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        GameObject.Destroy(myLine, duration);
    }

    IEnumerator No1Work(bool R, bool L, int state )
	{
		float time = 0.5f;
		int RSpeed = 50;
		int LSpeed = 50;
		int angle = 150;
		int langle = 150;

		if (R)//奇數次點擊
		{
			if (state == 1 || state == 2 || state == 5) { RSpeed = 200; angle = 130; Debug.Log("R 重 "); }
			else if (state == 3 || state == 4) { RSpeed = 150; angle = 80; Debug.Log("R 輕 "); }	
			ROSCSender.SendOSCMessageTriggerMethod(angle, RSpeed);//加壓
			yield return new WaitForSeconds(time);
			ROSCSender.SendOSCMessageTriggerMethod(10, RSpeed);
		}
		else if(L)
		{
			if(state == 1 || state == 2 || state == 5) { LSpeed = 200; angle = 150; Debug.Log("L 重 "); }
			else if (state == 3 || state == 4) { LSpeed = 150; angle = 100; Debug.Log("L 輕 "); }
			LOSCSender.SendOSCMessageTriggerMethod(angle, LSpeed);//加壓
			yield return new WaitForSeconds(time);
			LOSCSender.SendOSCMessageTriggerMethod(10, LSpeed);
			
		}
		else
		{
			if (state == 1 || state == 2 || state == 5) { RSpeed = 200; angle = 130; langle = 170; Debug.Log("C 重 "); }
			else if (state == 3 || state == 4) { RSpeed = 150; angle = 80; langle = 120; Debug.Log("C 輕 "); }
			ROSCSender.SendOSCMessageTriggerMethod(angle, RSpeed);//加壓
			LOSCSender.SendOSCMessageTriggerMethod(langle, RSpeed);
			yield return new WaitForSeconds(time);
			ROSCSender.SendOSCMessageTriggerMethod(10, RSpeed);
			LOSCSender.SendOSCMessageTriggerMethod(10, RSpeed);
		}
		
	}
}
