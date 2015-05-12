using UnityEngine;
using System.Collections;

namespace iCanScript.Nodes {

	public class PulseGenerator {
	    bool  myIsActive= false;
	    float myElapseTime;
    
	    [iCS_Function]
	    public PulseGenerator() {}
    
	    [iCS_Function(Return="pulse")]
	    public bool GeneratePulse(float time, bool active= true, bool startWithPulse= false) {
	        // Don't generate a pulse if we are not active.
	        if(active == false) {
	            myIsActive= false;
	            return false;
	        }
	        if(myIsActive == false) {
	            myIsActive= true;
	            myElapseTime= Time.time+time;
	            return startWithPulse;
	        }
	        if(Time.time >= myElapseTime) {
	            myElapseTime= Time.time+time;
	            return true;
	        }
	        return false;
	    }
	}
	
}
