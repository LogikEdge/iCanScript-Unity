using UnityEngine;
using System.Collections;

[iCS_Class(Company="iCanScript")]
public class iCS_Timer {
	float myTime= 0f;
    float myElapseTime;
    bool  myAutoRestart= false;
	bool  myNoDrift= false;
	
    public float RemainingTime {
        [iCS_Function]
        get {
            return myElapseTime-Time.timeSinceLevelLoad;
        }
    }
	public bool AutoRestart {
		[iCS_Function]
		get {
			return myAutoRestart;
		}
		[iCS_Function]
		set {
			myAutoRestart= value;
		}
	}
	public bool NoDrift {
		[iCS_Function]
		get {
			return myNoDrift;
		}
		[iCS_Function]
		set {
			myNoDrift= value;
		}
	}
	public bool IsElapsed {
		[iCS_Function]
		get {
			return RemainingTime <= 0f;
		}
	}
    [iCS_Function]
    public iCS_Timer(float time= 0f, bool autoRestart= false, bool noDrift= false) {
		NoDrift= noDrift;
        Restart(time);
		AutoRestart= autoRestart;
    }
    [iCS_Function]
    public void Restart(float time) {
		myTime= time;
		Restart();
    }
	[iCS_Function]
	public void Restart() {
		if(myNoDrift) {
			myElapseTime= myElapseTime+myTime;
		} else {				
			myElapseTime= Time.timeSinceLevelLoad+myTime;
		}
	}
}
