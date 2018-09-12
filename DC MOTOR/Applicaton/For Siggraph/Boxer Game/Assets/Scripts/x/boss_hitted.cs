using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boss_hitted : MonoBehaviour {

	public static int hp = 0;
    public gloveVibration Lglove;
    public gloveVibration Rglove;
    private int count = 0;
	private int hit = 0;
	private Transform boss_blood;	
	private Animator _animator;
	
	// Use this for initialization
	void Start () {
		boss_blood = GameObject.FindGameObjectWithTag("Boss_blood").transform;
		_animator = this.GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		if(hit == 1) count++ ; 
		if(count == 2){
            _animator.SetInteger("boss_hitted", 0);
            _animator.SetInteger("gameover", 0);
            Rglove.setColliderOn(false);
            Lglove.setColliderOn(false);
        }
        if (count == 150) { hit = 0; Rglove.setColliderOn(true); Lglove.setColliderOn(true); }
        boss_blood.localPosition = new Vector3(-232 * (hp / 180f), 0, 0);
	}
	void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("glove") && hit == 0){
            Debug.Log("boss hitted!!");
			hit = 1 ;
            count = 0; 
            if (hp + 10 < 200) hp += 10;
			else hp = 200;
			if(hp < 200) _animator.SetInteger("boss_hitted", 1);
			if(hp >= 200)
            {
                _animator.SetInteger("gameover", 1);
                StartCoroutine(gameoverScene());
            }
        }	
	}

    IEnumerator gameoverScene()
    {
        yield return new WaitForSeconds(2.7f);
        float fadeTime = GameObject.Find("Camera (eye)").GetComponent<SteamVrFade>().FadeToBlack();
        yield return new WaitForSeconds(fadeTime);
    }
}
