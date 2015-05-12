using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using P=iCanScript.Internal.Prelude;
using TimedAction= iCanScript.Internal.Prelude.TimerService.TimedAction;

namespace iCanScript.Internal.Editor {
    
    public static class BlinkController {
        // ======================================================================
        // INIT / SHUTDOWN
        // ----------------------------------------------------------------------
        static BlinkController()    {
            myAnimationTimer.Schedule();
            SystemEvents.OnSceneChanged+= RestartTimer;
        }
        public static void Start() {}
        public static void Shutdown() {
            myAnimationTimer.Stop();
        }
    
        // ======================================================================
        // PRIVATE FIELDS
        // ----------------------------------------------------------------------
        static TimedAction      myAnimationTimer= TimerService.CreateTimedAction(0.05f, DoAnimation, /*isLooping=*/true);
        static P.Animate<float> mySlowBlink     = new P.Animate<float>(TimerService.EditorTime);
        static P.Animate<float> myNormalBlink   = new P.Animate<float>(TimerService.EditorTime);
        static P.Animate<float> myFastBlink     = new P.Animate<float>(TimerService.EditorTime);
    
        // ======================================================================
        // PUBLIC FIELDS
        // ----------------------------------------------------------------------
        public static float SlowBlinkRatio       { get { return mySlowBlink.CurrentValue; }}
        public static float NormalBlinkRatio     { get { return myNormalBlink.CurrentValue; }}
        public static float FastBlinkRatio       { get { return myFastBlink.CurrentValue; }}
        public static Color SlowBlinkColor       { get { return new Color(1f,1f,1f,SlowBlinkRatio); }}
        public static Color NormalBlinkColor     { get { return new Color(1f,1f,1f,NormalBlinkRatio); }}
        public static Color FastBlinkColor       { get { return new Color(1f,1f,1f,FastBlinkRatio); }}
        public static Color SlowBlinkHighColor   { get { return new Color(1f,1f,1f,0.5f+0.5f*SlowBlinkRatio); }}
        public static Color NormalBlinkHighColor { get { return new Color(1f,1f,1f,0.5f+0.5f*NormalBlinkRatio); }}
        public static Color FastBlinkHighColor   { get { return new Color(1f,1f,1f,0.5f+0.5f*FastBlinkRatio); }}
        
        // ----------------------------------------------------------------------
        public static void RestartTimer() {
            myAnimationTimer.Schedule();
        }
        // ----------------------------------------------------------------------
        static void DoAnimation() {
    		// -- Restart the alpha animation --
    		if(mySlowBlink.IsElapsed) {
    			mySlowBlink.Start(0f, 3f, 3f, (start,end,ratio)=> 0.67f*Mathf.Abs(1.5f-Math3D.Lerp(start,end,ratio)));
    		}
    		if(myNormalBlink.IsElapsed) {
    			myNormalBlink.Start(0f, 2f, 2f, (start,end,ratio)=> Mathf.Abs(1f-Math3D.Lerp(start,end,ratio)));
    		}
    		if(myFastBlink.IsElapsed) {
    			myFastBlink.Start(0f, 1.5f, 1.5f, (start,end,ratio)=> 1.33f*Mathf.Abs(0.75f-Math3D.Lerp(start,end,ratio)));
    		}
    		// Animate the error display alpha.
    		mySlowBlink.Update();
    		myNormalBlink.Update();
    		myFastBlink.Update();
        }
    }
    
}