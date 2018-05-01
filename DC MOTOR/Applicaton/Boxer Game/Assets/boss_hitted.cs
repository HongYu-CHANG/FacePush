using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boss_hitted : MonoBehaviour {

	public static int hp = 0;
	private int count = 0;
	private int hit = 0;
	private Transform boss_blood;	
	// Use this for initialization
	void Start () {
		boss_blood = GameObject.FindGameObjectWithTag("Boss_blood").transform;

	}
	
	// Update is called once per frame
	void Update () {
		if(hit == 1) count++ ;
		if(count == 30) hit = 0 ;
		boss_blood.localPosition = new Vector3(-332 * (hp / 250f), 0, 0);
	}
	void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("glove") && hit == 0){
            Debug.Log("boss hitted!!");
			hit = 1 ;
			count = 0 ;
			if (hp + 10 < 250) hp += 10;

		}
		
	}
}
