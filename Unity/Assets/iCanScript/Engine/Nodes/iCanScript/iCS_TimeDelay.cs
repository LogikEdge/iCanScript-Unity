using UnityEngine;
using System.Collections;

public enum TriggerType { TriggerOnTrueState, TriggerOnFalseState, TriggerOnFalseToTrueEdge, TriggerOnTrueToFalseEdge };
public enum OutputType { SinglePulse, ContinuousState }

[iCS_Class(Company="iCanScript", Icon= "iCS_ClockIcon.psd")]
public class iCS_TimeDelay {
    bool            myState= false;
    bool            myIsArmed= false;
    float           myElapseTime= 0;
    TriggerType     myTriggerType;
    OutputType      myOutputType;
    
    [iCS_Function]
    public iCS_TimeDelay(TriggerType triggerType, OutputType outputControl) {
        myElapseTime    = Time.time-1f;
        myTriggerType   = triggerType;
        myOutputType    = outputControl;
    }
    [iCS_Function]
    public bool Delay(float delayTime, bool trigger) {
        switch(myTriggerType) {
            case TriggerType.TriggerOnTrueState: {
                // Restart delay.
                if(trigger == true) {
                    if(myState == false) {
                        myElapseTime= Time.time+delayTime;
                        myIsArmed= true;
                    }
                    myState= trigger;
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
                myState= trigger;
                return false;
            }
            case TriggerType.TriggerOnFalseState: {
                trigger ^= true;
                myState ^= true;
                if(trigger == true) {
                    if(myState == false) {
                        myElapseTime= Time.time+delayTime;
                        myIsArmed= true;
                    }
                    myState= trigger ^ true;
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
                myState= trigger ^ true;
                return false;
            }
            case TriggerType.TriggerOnFalseToTrueEdge: {
                // Restart delay.
                if(trigger == true && myState == false) {
                    myElapseTime= Time.time+delayTime;
                    myIsArmed= true;
                }
                if(myOutputType == OutputType.ContinuousState) {
                    if(trigger == false) {
                        myIsArmed= false;
                    }
                }
                myState= trigger;
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
                trigger ^= true;
                myState ^= true;
                // Restart delay.
                if(trigger == true && myState == false) {
                    myElapseTime= Time.time+delayTime;
                    myIsArmed= true;
                }
                if(myOutputType == OutputType.ContinuousState) {
                    if(trigger == false) {
                        myIsArmed= false;
                    }
                }
                myState= trigger ^ true;
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
