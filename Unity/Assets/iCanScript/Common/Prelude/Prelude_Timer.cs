using UnityEngine;
using System.Collections;

public static partial class Prelude {
    // ======================================================================
    // Base Time Utilities
    // ----------------------------------------------------------------------
    // Return a time stamp in seconds.
    public static float CurrentTime() {
        return Time.realtimeSinceStartup;    
    }


    // ======================================================================
    // ----------------------------------------------------------------------
    public class Timer {
        float myDeltaTime;
        float myElapseTime;
        
        public Timer(float deltaTime) {
            myDeltaTime= deltaTime;
            myElapseTime= CurrentTime()+deltaTime;
        }
        public bool IsElapsed {
            get { return CurrentTime() >= myElapseTime; }
        }
        public float RemainingTime {
            get { return myElapseTime-CurrentTime(); }
        }
        public void Restart() {
            myElapseTime= CurrentTime()+myDeltaTime;
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
    }
}
