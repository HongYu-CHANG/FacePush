using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteamVrFade : MonoBehaviour {

    private float _fadeDuration = 1f;

    // Use this for initialization
    void Start () {
        //FadeToWhite();
        //Invoke("FadeFromWhite", _fadeDuration);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public float FadeToBlack()
    {
        //set start color
        //SteamVR_Fade.Start(Color.clear, 0f);
        //set and start fade to
        SteamVR_Fade.Start(Color.black, _fadeDuration);
        Debug.Log("GameOver");
        return _fadeDuration;
    }

    public void FadeFromBlack()
    {
        //set start color
        SteamVR_Fade.Start(Color.black, 0f);
        //set and start fade to
        SteamVR_Fade.Start(Color.clear, _fadeDuration);
    }
}
