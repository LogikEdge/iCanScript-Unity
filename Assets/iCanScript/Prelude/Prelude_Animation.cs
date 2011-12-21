using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public static partial class Prelude {
    // ======================================================================
    // Base Time Utilities
    // ----------------------------------------------------------------------
    // Return a time stamp in seconds.
    public static float CurrentTime() {
        return Time.realtimeSinceStartup;    
    }

    // ----------------------------------------------------------------------
    // Class used to track a ratio between a start and ent time.
    public class TimeRatio {
        float   myStartTime= 0f;
        float   myEndTime= 0f;
        
        public bool  IsActive       { get { return myStartTime != myEndTime; }}
        public bool  IsElapsed      { get { return CurrentTime() >= myEndTime; }}
        public float Ratio          { get { return IsActive ? (CurrentTime()-myStartTime)/(myEndTime-myStartTime) : 1f; }}
        public float RemainingTime  { get { return myEndTime-CurrentTime(); }}

        public void Start(float deltaTime) {
            myStartTime= CurrentTime();
            myEndTime= myStartTime+deltaTime;
        }
        public void Reset() {
            myStartTime= myEndTime= 0f;
        }
    }

    // ----------------------------------------------------------------------
    // Class used to a value between start and target values.
    public class Animate<T> {
        T                   myStartValue  = default(T);
        T                   myTargetValue = default(T);
        T                   myCurrentValue= default(T);
        TimeRatio           myTimeRatio   = new TimeRatio();
        Func<T,T,float,T>   myAnimFunc    = null;
        
        public bool     IsActive        { get { return myTimeRatio.IsActive; }}
        public bool     IsElapsed       { get { return myTimeRatio.IsElapsed; }}
        public T        CurrentValue    { get { return myCurrentValue; }}
        public T        StartValue      { get { return myStartValue; } set { myStartValue= value; }}
        public T        TargetValue     { get { return myTargetValue; } set { myTargetValue= value; }}
        public float    Ratio           { get { return myTimeRatio.Ratio; }}
        public float    RemainingTime   { get { return myTimeRatio.RemainingTime; }}
        
        public void Start(T startValue, T targetValue, float deltaTime, Func<T,T,float,T> animFunc) {
            myCurrentValue= startValue;
            myStartValue= startValue;
            myTargetValue= targetValue;
            myTimeRatio.Start(deltaTime);
            myAnimFunc= animFunc;
        }
        public void Reset() {
            myTimeRatio.Reset();
        }
    
        public void Update() {
            if(myTimeRatio.IsActive) {
                if(myTimeRatio.IsElapsed) {
                    myTimeRatio.Reset();
                    myCurrentValue= myTargetValue;
                    return;
                }
                myCurrentValue= myAnimFunc(myStartValue, myTargetValue, Ratio);
            }
        }
    }
}
