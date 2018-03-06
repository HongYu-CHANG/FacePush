using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class waterCaustics : MonoBehaviour {

    public float fps = 20.0f;
    public Texture2D[] frames;

    private int frameIndex;
    private Projector projector;

    void Start()
    {
        projector = GetComponent<Projector>();
        NextFrame();
        InvokeRepeating("NextFrame", 1/fps, 1/fps);
    }

    void NextFrame()
    {
        projector.material.SetTexture("_ShadowTex", frames[frameIndex]);
        frameIndex = (frameIndex + 1) % frames.Length;
    }

}
