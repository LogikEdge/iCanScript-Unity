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
        
        public bool  IsActive       { get { return myStartTime != 0f || myEndTime != 0f; }}
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

/*
	TODO : Allow for shared time ratio in animation (for synchronized animations).
*/
    // ----------------------------------------------------------------------
    // Class used to a value between start and target values.
    public class Animate<T> {
        T                   myStartValue      = default(T);
        T                   myTargetValue     = default(T);
        T                   myCurrentValue    = default(T);
		bool				myIsTimeRatioOwner= false;
        TimeRatio           myTimeRatio   	  = null;
        Func<T,T,float,T>   myAnimFunc    	  = null;
        
        public bool     IsActive        { get { return myTimeRatio != null ? myTimeRatio.IsActive : false; }}
        public bool     IsElapsed       { get { return myTimeRatio != null ? myTimeRatio.IsElapsed : true; }}
        public T        CurrentValue    { get { return myCurrentValue; }}
        public T        StartValue      { get { return myStartValue; } set { myStartValue= value; }}
        public T        TargetValue     { get { return myTargetValue; } set { myTargetValue= value; }}
        public float    Ratio           { get { return myTimeRatio != null ? myTimeRatio.Ratio : 0.0f; }}
        public float    RemainingTime   { get { return myTimeRatio != null ? myTimeRatio.RemainingTime : 0.0f; }}
        
        public void Start(T startValue, T targetValue, float deltaTime, Func<T,T,float,T> animFunc) {
            myCurrentValue= startValue;
            myStartValue= startValue;
            myTargetValue= targetValue;
			if(myTimeRatio == null || myIsTimeRatioOwner == false) {
				myIsTimeRatioOwner= true;
				myTimeRatio= new TimeRatio();
			}
            myTimeRatio.Start(deltaTime);
            myAnimFunc= animFunc;
        }
        public void Start(T startValue, T targetValue, TimeRatio timeRatio, Func<T,T,float,T> animFunc) {
            myCurrentValue= startValue;
            myStartValue= startValue;
            myTargetValue= targetValue;
			myIsTimeRatioOwner= false;
			myTimeRatio= timeRatio;
            myAnimFunc= animFunc;
        }
        public void Reset() {
			if(myIsTimeRatioOwner) {
	            myTimeRatio.Reset();				
			}
        }
        public void Reset(T value) {
            myTargetValue= value;
            myCurrentValue= value;
			if(myIsTimeRatioOwner) {
            	myTimeRatio.Reset();
			}
        }
    
        public void Update() {
			if(myTimeRatio == null || myTimeRatio.IsElapsed) {
				Reset(TargetValue);
				return;
			};
            if(myTimeRatio.IsActive) {
                myCurrentValue= myAnimFunc(myStartValue, myTargetValue, Ratio);
            }
        }
    }
}
