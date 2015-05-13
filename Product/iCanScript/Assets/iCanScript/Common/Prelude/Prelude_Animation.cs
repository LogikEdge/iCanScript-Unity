using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace iCanScript.Internal {
    
    public static partial class Prelude {
        // ----------------------------------------------------------------------
        // Class used to track a ratio between a start and ent time.
        public class TimeRatio {
            float       myStartTime= 0f;
            float       myEndTime  = 0f;
            Func<float> myTimeFnc  = null;
        
            public bool  IsActive       { get { return myStartTime != 0f || myEndTime != 0f; }}
            public bool  IsElapsed      { get { return myTimeFnc() >= myEndTime; }}
            public float RemainingTime  { get { return myEndTime-myTimeFnc(); }}
            public float Ratio          {
                get {
                    var now= myTimeFnc();
    				if(!IsActive || now >= myEndTime) return 1f;
                    var ratio= (now-myStartTime)/(myEndTime-myStartTime);
    				return Math3D.IsSmaller(ratio, 1f) ? ratio : 1f; 
                }
            }

            public TimeRatio(Func<float> timeFnc) { myTimeFnc= timeFnc; }
            public void Start(float deltaTime) {
                myStartTime= myTimeFnc();
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
            Func<float>         myTimeFnc         = null;
        
            public bool     IsActive        { get { return myIsActive && myTimeRatio != null; }}
            public bool     IsElapsed       { get { return myTimeRatio != null ? myTimeRatio.IsElapsed : true; }}
            public T        CurrentValue    { get { return myCurrentValue; }}
            public T        StartValue      { get { return myStartValue; } set { myStartValue= value; }}
            public T        TargetValue     { get { return myTargetValue; } set { myTargetValue= value; }}
            public float    Ratio           { get { return myTimeRatio != null ? myTimeRatio.Ratio : 0.0f; }}
            public float    RemainingTime   { get { return myTimeRatio != null ? myTimeRatio.RemainingTime : 0.0f; }}
    		public Func<T,T,float,T> AnimFunc	{ get { return myAnimFunc; } set { myAnimFunc= value; }}
        
    		public Animate(Func<float> timeFnc) { myTimeFnc= timeFnc; }
    		public Animate(Func<T,T,float,T> animFunc, Func<float> timeFnc) { myAnimFunc= animFunc; myTimeFnc= timeFnc; }
		
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
            public void Start(T startValue, T targetValue, TimeRatio timeRatio) {
    			myStartValue= startValue;
    			myTargetValue= targetValue;
    			Start(timeRatio);
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
    				myTimeRatio= new TimeRatio(myTimeFnc);
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

}
