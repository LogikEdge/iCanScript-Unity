using UnityEngine;
using System.Collections;

public enum TriggerType { TriggerOnTrueState, TriggerOnFalseState, TriggerOnFalseToTrueEdge, TriggerOnTrueToFalseEdge };
public enum OutputType { SinglePulse, ContinuousState }

//[iCS_Class(Company="iCanScript", Icon= "iCS_ClockIcon.psd")]
/// \deprecated
public class iCS_TimeDelay {
    bool            myState= false;
    bool            myIsArmed= false;
    float           myElapseTime= 0;
    TriggerType     myTriggerType;
    OutputType      myOutputType;
    
//    [iCS_Function]
    public iCS_TimeDelay(TriggerType triggerType, OutputType outputControl) {
        myElapseTime    = Time.time-1f;
        myTriggerType   = triggerType;
        myOutputType    = outputControl;
    }
//    [iCS_Function(Return="output")]
    public bool Delay(float delayTime, bool startTrigger) {
        switch(myTriggerType) {
            case TriggerType.TriggerOnTrueState: {
                // Restart delay.
                if(startTrigger == true) {
                    if(myState == false) {
                        myElapseTime= Time.time+delayTime;
                        myIsArmed= true;
                    }
                    myState= startTrigger;
                    if(Time.time >= myElapseTime) {
                        if(myOutputType == OutputType.ContinuousState) {
                            return true;
                        }
                        if(myIsArmed == true) {
                            myIsArmed= false;
                            return true;
                        }
                        return false;
                    }
                }
                myState= startTrigger;
                return false;
            }
            case TriggerType.TriggerOnFalseState: {
                startTrigger ^= true;
                myState ^= true;
                if(startTrigger == true) {
                    if(myState == false) {
                        myElapseTime= Time.time+delayTime;
                        myIsArmed= true;
                    }
                    myState= startTrigger ^ true;
                    if(Time.time >= myElapseTime) {
                        if(myOutputType == OutputType.ContinuousState) {
                            return true;
                        }
                        if(myIsArmed == true) {
                            myIsArmed= false;
                            return true;
                        }
                        return false;
                    }
                }
                myState= startTrigger ^ true;
                return false;
            }
            // FIXME: Reset output on edge instead of state for edge triggers.
            case TriggerType.TriggerOnFalseToTrueEdge: {
                // Restart delay.
                if(startTrigger == true && myState == false) {
                    myElapseTime= Time.time+delayTime;
                    myIsArmed= true;
                }
                if(myOutputType == OutputType.ContinuousState) {
                    if(startTrigger == false) {
                        myIsArmed= false;
                    }
                }
                myState= startTrigger;
                if(myIsArmed == true && Time.time >= myElapseTime) {
                    if(myOutputType == OutputType.ContinuousState) {
                        return true;
                    }
                    if(myIsArmed == true) {
                        myIsArmed= false;
                        return true;
                    }
                }
                return false;
            }
            case TriggerType.TriggerOnTrueToFalseEdge: {
                startTrigger ^= true;
                myState ^= true;
                // Restart delay.
                if(startTrigger == true && myState == false) {
                    myElapseTime= Time.time+delayTime;
                    myIsArmed= true;
                }
                if(myOutputType == OutputType.ContinuousState) {
                    if(startTrigger == false) {
                        myIsArmed= false;
                    }
                }
                myState= startTrigger ^ true;
                if(myIsArmed == true && Time.time >= myElapseTime) {
                    if(myOutputType == OutputType.ContinuousState) {
                        return true;
                    }
                    if(myIsArmed == true) {
                        myIsArmed= false;
                        return true;
                    }
                }
                return false;
            }
        }
        return false;
    }
}
