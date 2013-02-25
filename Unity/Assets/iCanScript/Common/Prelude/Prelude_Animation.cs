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
        public float RemainingTime  { get { return myEndTime-CurrentTime(); }}
        public float Ratio          {
            get {
                var now= CurrentTime();
                return IsActive && now < myEndTime ?
                    (now-myStartTime)/(myEndTime-myStartTime) :
                    1f;
            }
        }

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
		bool				myIsActive		  = false;
        T                   myStartValue      = default(T);
        T                   myTargetValue     = default(T);
        T                   myCurrentValue    = default(T);
		bool				myIsTimeRatioOwner= false;
        TimeRatio           myTimeRatio   	  = null;
        Func<T,T,float,T>   myAnimFunc    	  = null;
        
        public bool     IsActive        { get { return myTimeRatio != null ? myIsActive && myTimeRatio.IsActive : false; }}
        public bool     IsElapsed       { get { return myTimeRatio != null ? myTimeRatio.IsElapsed : true; }}
        public T        CurrentValue    { get { return myCurrentValue; }}
        public T        StartValue      { get { return myStartValue; } set { myStartValue= value; }}
        public T        TargetValue     { get { return myTargetValue; } set { myTargetValue= value; }}
        public float    Ratio           { get { return myTimeRatio != null ? myTimeRatio.Ratio : 0.0f; }}
        public float    RemainingTime   { get { return myTimeRatio != null ? myTimeRatio.RemainingTime : 0.0f; }}
		public Func<T,T,float,T> AnimFunc	{ get { return myAnimFunc; } set { myAnimFunc= value; }}
        
		public Animate() {}
		public Animate(Func<T,T,float,T> animFunc) { myAnimFunc= animFunc; }
		
        public void Start(T startValue, T targetValue, float animTime, Func<T,T,float,T> animFunc) {
			myStartValue= startValue;
			myTargetValue= targetValue;
			Start(animTime, animFunc);
        }
        public void Start(T startValue, T targetValue, TimeRatio timeRatio, Func<T,T,float,T> animFunc) {
			myStartValue= startValue;
			myTargetValue= targetValue;
			Start(timeRatio, animFunc);
        }
		public void Start(float animTime, Func<T,T,float,T> animFunc) {
            myAnimFunc= animFunc;
			Start(animTime);
		}
		public void Start(TimeRatio timeRatio, Func<T,T,float,T> animFunc) {
            myAnimFunc= animFunc;
			Start(timeRatio);
		}
		public void Start(float animTime) {
			if(myTimeRatio == null || myIsTimeRatioOwner == false) {
				myIsTimeRatioOwner= true;
				myTimeRatio= new TimeRatio();
			}
            myTimeRatio.Start(animTime);
            myCurrentValue= myStartValue;
			myIsActive= true;
		}
		public void Start(TimeRatio timeRatio) {
			myIsTimeRatioOwner= false;
			myTimeRatio= timeRatio;
            myCurrentValue= myStartValue;
			myIsActive= true;
		}
        public void Reset() {
			ResetCommon();
        }
        public void Reset(T value) {
            myTargetValue= value;
            myCurrentValue= value;
			ResetCommon();
        }
    	void ResetCommon() {
			myIsActive= false;
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
