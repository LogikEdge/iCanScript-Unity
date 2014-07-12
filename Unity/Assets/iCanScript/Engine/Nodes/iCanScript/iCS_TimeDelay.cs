using UnityEngine;
using System.Collections;

[iCS_Class(Company="iCanScript", Icon= "iCS_ClockIcon.psd")]
public class iCS_TimeDelay {
    bool    myState= false;
    float   myElapseTime= 0;
    
    [iCS_Function]
    public iCS_TimeDelay() {
        myElapseTime= Time.time-1f;
    }
    [iCS_Function]
    public bool Delay(float delayTime, bool trigger) {
        // Restart delay.
        if(myState == false && trigger == true) {
            myElapseTime= Time.time+delayTime;
        }
        myState= trigger;
        return Time.time <= myElapseTime;
    }
}
