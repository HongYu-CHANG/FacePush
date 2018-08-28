using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Test : MonoBehaviour {


    private int Left_degreeConvertToRotaryCoder(int degree) { return (degree * 1024 / 360); }
    private int Right_degreeConvertToRotaryCoder(int degree) { return (degree * 824 / 360); }
    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {
        Debug.Log(GameDataManager.Uno.receiveData());

        if (Input.GetKey(KeyCode.T))
        {
            Debug.LogWarning("T " + Left_degreeConvertToRotaryCoder(107) + " 255 " + Right_degreeConvertToRotaryCoder(50) + " 255");
            new Thread(GameDataManager.Uno.sendData).Start(Left_degreeConvertToRotaryCoder(107) + " 255 " + Right_degreeConvertToRotaryCoder(50) + " 255");
        }
        if (Input.GetKey(KeyCode.B))
        {
            Debug.LogWarning("B " + Left_degreeConvertToRotaryCoder(10) + " 255 " + Right_degreeConvertToRotaryCoder(10) + " 255");
            new Thread(GameDataManager.Uno.sendData).Start(Left_degreeConvertToRotaryCoder(10) + " 255 " + Right_degreeConvertToRotaryCoder(10) + " 255");
        }
    }
}
