using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class directionControl : MonoBehaviour {

    public GameObject Ltracker;
    public GameObject Rtracker;
    public Vector3 Lpos;
    public Vector3 Rpos;
    public Plane plane; //different from the plane showed on the screen

    //construct vertical plane
    public GameObject cube;
    public GameObject cube1;
    public GameObject cube2;
    public Vector3 cubepos;
    public Vector3 cube1pos;
    public Vector3 cube2pos;
    public float Ldist;
    public float Rdist;

    public float LRdistDifference = 0;


    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {

        // Construct the vertical plane

        cubepos = new Vector3(cube.transform.position.x, cube.transform.position.y, cube.transform.position.z);
        cube1pos = new Vector3(cube1.transform.position.x, cube1.transform.position.y, cube1.transform.position.z);
        cube2pos = new Vector3(cube2.transform.position.x, cube2.transform.position.y, cube2.transform.position.z);

        plane = new Plane(cubepos, cube1pos, cube2pos);

        //draw raycast to check the plane is correct or not
        //Debug.DrawRay(cubepos, cubepos - cube1pos, Color.red, 10);
        //Debug.DrawRay(cubepos, cube2pos - cubepos, Color.green, 10);

        Lpos = new Vector3(Ltracker.transform.position.x, Ltracker.transform.position.y, Ltracker.transform.position.z);
        Rpos = new Vector3(Rtracker.transform.position.x, Rtracker.transform.position.y, Rtracker.transform.position.z);

        Ldist = Mathf.Abs(plane.GetDistanceToPoint(Lpos));
        Rdist = Mathf.Abs(plane.GetDistanceToPoint(Rpos));

        Debug.Log("L:" + Ldist);
        Debug.Log("R:" + Rdist);


        // Decide rotation angle according to the difference of 

        LRdistDifference = Ldist - Rdist;
        Debug.Log(LRdistDifference);

        if(LRdistDifference >= 0.1f)
        {
            //turn right
            transform.Rotate(Vector3.up * LRdistDifference * (0.1f));
            //get the degree
        }

        else if (LRdistDifference <= 0.1f)
        {
            //turn left
            transform.Rotate(Vector3.down * LRdistDifference * (-0.1f));
            //get the degree
        }

        /*
        else
        {
            //go straight
        }
        */


    }
}
