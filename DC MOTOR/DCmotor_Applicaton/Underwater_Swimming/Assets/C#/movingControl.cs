using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movingControl : MonoBehaviour
{

    public GameObject headpos;
    public GameObject bodypos;
    private Vector3 body_head_direction;


    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        body_head_direction = headpos.transform.position - bodypos.transform.position;
        transform.position = transform.position + body_head_direction * (0.01f);

    }
}
