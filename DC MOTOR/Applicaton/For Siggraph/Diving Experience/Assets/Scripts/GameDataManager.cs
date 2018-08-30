using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDataManager //Global Parameter
{ 
    public static CommunicateWithArduino Uno = new CommunicateWithArduino("COM5", baudRate:115200);
    public static UIController UIManager = new UIController();
}
