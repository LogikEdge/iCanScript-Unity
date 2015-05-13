using UnityEngine;
using System;
using System.Collections;
using P=iCanScript.Internal.Prelude;
using iCanScript;

namespace iCanScript.Internal.Editor {
    using Prefs= PreferencesController;

    public partial class iCS_EditorObject {
        // ======================================================================
    	// Fields
        // ----------------------------------------------------------------------
    	public P.Animate<Rect> myAnimatedRect=
    		new P.Animate<Rect>((start,end,ratio)=>Math3D.Lerp(start,end,ratio), TimerService.EditorTime);
    
        // ======================================================================
        // Properties
        // ----------------------------------------------------------------------
        public Rect AnimationStartRect {
            get { return myAnimatedRect.StartValue; }
            set { myAnimatedRect.StartValue= value; }
        }
        public void ResetAnimationRect(Rect r) {
            AnimationStartRect= r; myAnimatedRect.Reset(r);
        }
        public Rect AnimationTargetRect {
            get { return myAnimatedRect.TargetValue; }
            set { myAnimatedRect.TargetValue= value; }
        }
    
        // ======================================================================
        // Queries
        // ----------------------------------------------------------------------
        // Returns true if the display size or position are being animated.
        public bool IsAnimated {
            get { return myAnimatedRect.IsActive && !myAnimatedRect.IsElapsed; }
        }

        // ======================================================================
        // Queries
        // ----------------------------------------------------------------------
    	public float AnimationTimeRatio { get { return myAnimatedRect.Ratio; }}
	
        // ======================================================================
    	// Animation update
        // ----------------------------------------------------------------------
        public float DisplayAlpha {
            get {
                if(IsPort) {
                    return ParentNode.DisplayAlpha;
                }
                if(!IsVisibleInLayout) {
                    return 1f-myAnimatedRect.Ratio;
                }
                if(IsAnimated && Math3D.Area(myAnimatedRect.StartValue) < 0.1f) {
                    return myAnimatedRect.Ratio;
                }
                return 1f;
            }
        }
    
    }
}

