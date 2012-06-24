using UnityEngine;
using System.Collections;

[iCS_Class(Company="iCanScript")]
public class iCS_Timer {
    float StartTime;
    
    public float ElapseTime {
        [iCS_Function]
        get {
            return Time.timeSinceLevelLoad-StartTime;
        }
    }
    [iCS_Function]
    public iCS_Timer() {
        Reset();
    }
    [iCS_Function]
    public void Reset() {
        StartTime= Time.timeSinceLevelLoad;
    }
    
}
