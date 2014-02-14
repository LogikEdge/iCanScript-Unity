using UnityEngine;
using System;
using System.Collections;
using P=Prelude;
using Prefs= iCS_PreferencesController;

public partial class iCS_EditorObject {
    // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    // Verified
    // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    // ======================================================================
	// Fields
    // ----------------------------------------------------------------------
	public P.Animate<Rect> myAnimatedRect=
		new P.Animate<Rect>((start,end,ratio)=>Math3D.Lerp(start,end,ratio));
    
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
        get { return myAnimatedRect.IsActive; }
    }

    // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    // End Verified
    // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

    // ======================================================================
	// Fields
    // ----------------------------------------------------------------------
	public bool LayoutUsingAnimatedChildren= false;
    
    // ======================================================================
    // Queries
    // ----------------------------------------------------------------------
	public void StopAnimation() {
		myAnimatedRect.Reset(myAnimatedRect.TargetValue);
	}
	public float AnimationTimeRatio { get { return myAnimatedRect.Ratio; }}
	
    // ======================================================================
	// Animation timer builders
    // ----------------------------------------------------------------------
    public float AnimationTimeFromPosition(Vector2 p1, Vector2 p2) {
        var distance= Vector2.Distance(p1,p2);
	    var time= AnimationTimeFromDistance(distance);
		if(IsAnimated) {
			var remainingTime= myAnimatedRect.RemainingTime;
			return Mathf.Max(remainingTime, time);
		}
        var minAnimationTime= Prefs.MinAnimationTime;
        return time < minAnimationTime ? minAnimationTime : time;
    }
    // ----------------------------------------------------------------------
    public float AnimationTimeFromSize(Vector2 s1, Vector2 s2) {
        var distance= Vector2.Distance(s1,s2);
	    var time= AnimationTimeFromDistance(distance);
		if(IsAnimated) {
			var remainingTime= myAnimatedRect.RemainingTime;
			return Mathf.Max(remainingTime, time);			
		}
        var minAnimationTime= Prefs.MinAnimationTime;
        return time < minAnimationTime ? minAnimationTime : time;
    }
    // ----------------------------------------------------------------------
    public float AnimationTimeFromRect(Rect r1, Rect r2) {
		var p1= PositionFrom(r1);
		var p2= PositionFrom(r2);
		var s1= SizeFrom(r1);
		var s2= SizeFrom(r2);
		var positionTime= AnimationTimeFromPosition(p1, p2);
		var sizeTime= AnimationTimeFromSize(s1, s2);
		return Mathf.Max(positionTime, sizeTime);
    }
    // ----------------------------------------------------------------------
    public static float AnimationTimeFromDistance(float distance) {
	    return distance/Prefs.AnimationPixelsPerSecond;
    }
    // ----------------------------------------------------------------------
	public P.TimeRatio BuildTimeRatioFromRect(Rect r1, Rect r2) {
	    return BuildTimeRatio(AnimationTimeFromRect(r1, r2));
	}
    // ----------------------------------------------------------------------
	public static P.TimeRatio BuildTimeRatio(float time) {
		var timeRatio= new P.TimeRatio();
        timeRatio.Start(time);
		return timeRatio;		
	}
    // ======================================================================
	// Animation update
    // ----------------------------------------------------------------------

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
