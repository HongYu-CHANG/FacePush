using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Text debugMessage;

    // Use this for initialization
    void Start()
    {
        Application.logMessageReceived += HandleLog;
    }

    // Update is called once per frame
    void Update()
    {
               
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        debugMessage.text = logString;
    }
}