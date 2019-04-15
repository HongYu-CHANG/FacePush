using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boxer_pos : MonoBehaviour {

    private Vector3 offset = new Vector3(4.3f, 0.5f, 0.3f);//相机相对于玩家的位置
    private Transform target;
    private Vector3 pos;
	private Vector3 pos_head;

	public float speed = 2;

    // Use this for initialization
    void Start () {
		/*target = GameObject.FindGameObjectWithTag("Boxer_01").transform;
        this.transform.position = pos;
        Quaternion angel = Quaternion.LookRotation(target.position - this.transform.position);//获取旋转角度
        this.transform.rotation = angel;*/
		pos = this.transform.position;
		pos_head = GameObject.FindGameObjectWithTag("HEAD").transform.position;
		offset = pos - pos_head;
	}

    // Update is called once per frame
    void Update () {
		//position and rotation change with camera (with offset)
		/*
        pos = target.position + offset;
        this.transform.position = Vector3.Lerp(this.transform.position, pos, speed * Time.deltaTime);//调整相机与玩家之间的距离
        Quaternion angel = Quaternion.LookRotation(target.position - this.transform.position);//获取旋转角度
        // this.transform.rotation = Quaternion.Slerp(this.transform.rotation, angel, speed * Time.deltaTime);
        this.transform.rotation = angel;*/
		pos = GameObject.FindGameObjectWithTag("HEAD").transform.position + offset;
		this.transform.position = Vector3.Lerp(this.transform.position, pos, speed * Time.deltaTime);//调整相机与玩家之间的距离
		Quaternion angel = Quaternion.LookRotation(GameObject.FindGameObjectWithTag("HEAD").transform.position - this.transform.position);//获取旋转角度
		//this.transform.rotation = Quaternion.Slerp(this.transform.rotation, angel, speed * Time.deltaTime);
	}
}
