using UnityEngine;
using System;
using System.Collections;

namespace iCanScript.Internal {
    
    public static partial class Prelude {
        // ======================================================================
        // Base Time Utilities
        // ----------------------------------------------------------------------
        // Return a time stamp in seconds.
        public static float EngineCurrentTime() {
            return Time.realtimeSinceStartup;    
        }

        // ======================================================================
        // ----------------------------------------------------------------------
        public class Timer {
            float       myDeltaTime;
            float       myElapseTime;
            Func<float> myTimeFnc;
        
            public Timer(float deltaTime, Func<float> timeFnc) {
                myTimeFnc   = timeFnc;
                myDeltaTime = deltaTime;
                myElapseTime= myTimeFnc()+deltaTime;
            }
            public bool IsElapsed {
                get { return myTimeFnc() >= myElapseTime; }
            }
            public float DeltaTime {
                get { return myDeltaTime; }
            }
            public float RemainingTime {
                get { return myElapseTime-myTimeFnc(); }
            }
            public void Restart() {
                myElapseTime= myTimeFnc()+myDeltaTime;
            }
            public void Restart(float deltaTime) {
                myDeltaTime= deltaTime;
                Restart();
            }
            public void RestartNoDrift() {
                myElapseTime+= myDeltaTime;
            }
            public void RestartNoDrift(float deltaTime) {
                myDeltaTime= deltaTime;
                RestartNoDrift();
            }
            public void SanityCheck() {
                if(RemainingTime > myDeltaTime) {
                    Restart();
                }
            }
        }
    }

}
