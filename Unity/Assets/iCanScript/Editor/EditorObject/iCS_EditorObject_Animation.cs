using UnityEngine;
using System;
using System.Collections;
using P=Prelude;

public partial class iCS_EditorObject {
    // ======================================================================
	// Fields
    // ----------------------------------------------------------------------
	private P.Animate<Vector2> AnimatedPosition=
		new P.Animate<Vector2>((start,end,ratio)=>Math3D.Lerp(start,end,ratio));
	private P.Animate<Vector2> AnimatedSize=
		new P.Animate<Vector2>((start,end,ratio)=>Math3D.Lerp(start,end,ratio));

    // ======================================================================
    // Queries
    // ----------------------------------------------------------------------
    // Returns true if the display size is currently being animated.
    public bool IsSizeAnimated {
        get { return  AnimatedSize.IsActive; }
    }
    // ----------------------------------------------------------------------
    // Returns true if the display position is currently being animated.
    public bool IsPositionAnimated {
        get { return  AnimatedPosition.IsActive; }
    }
    // ----------------------------------------------------------------------
    // Returns true if the display size or position are being animated.
    public bool IsAnimated {
        get { return IsSizeAnimated || IsPositionAnimated; }
    }

    // ======================================================================
    // Display Size Animation
    // ----------------------------------------------------------------------
	void AnimateSize(Vector2 targetSize) {
        var startSize= AnimatedSize.CurrentValue;
		var timeRatio= BuildTimeRatioFromSize(startSize, targetSize); 
		AnimateSize(targetSize, timeRatio);
	}
    // ----------------------------------------------------------------------
	void AnimateSize(Vector2 targetSize, P.TimeRatio timeRatio) {
        AnimatedSize.StartValue= AnimatedSize.CurrentValue;
		AnimatedSize.TargetValue= targetSize;
		AnimatedSize.Start(timeRatio);
	}
    // ----------------------------------------------------------------------
    void StopSizeAnimation() {
        AnimatedSize.Reset(AnimatedSize.TargetValue);
    }
    
    // ======================================================================
	// Animation start value setters.
    // ----------------------------------------------------------------------
	void SetRectAnimationStartValue(Rect r) {
		SetPositionAnimationStartValue(Math3D.Middle(r));
	}
    // ----------------------------------------------------------------------
	void SetPositionAnimationStartValue(Vector2 startPos) {
		if(!IsPositionAnimated) {
			AnimatedPosition.StartValue= startPos;
		} else {
			AnimatedPosition.StartValue= AnimatedPosition.CurrentValue;
			AnimatedPosition.Start(AnimatedPosition.RemainingTime);
		}
	}

    // ======================================================================
	// Animation start functions
    // ----------------------------------------------------------------------
	void StartRectAnimation(Rect targetRect) {
		var startSize= AnimatedSize.StartValue;
        var startRect= new Rect(AnimatedPosition.StartValue.x-0.5f*startSize.x,
                                AnimatedPosition.StartValue.y-0.5f*startSize.y,
                                startSize.x,
                                startSize.y);
		var timeRatio= BuildTimeRatioFromRect(startRect, targetRect);
		StartRectAnimation(targetRect, timeRatio);
	}
    // ----------------------------------------------------------------------
	void StartPositionAnimation(Vector2 targetPosition) {
        var startPos= AnimatedPosition.StartValue;
		var timeRatio= BuildTimeRatioFromPosition(startPos, targetPosition);
		StartPositionAnimation(targetPosition, timeRatio);
	}
    // ----------------------------------------------------------------------
	void StartRectAnimation(Rect targetRect, P.TimeRatio timeRatio) {
		var targetPosition= Math3D.Middle(targetRect);
		StartPositionAnimation(targetPosition, timeRatio);
	}
    // ----------------------------------------------------------------------
	void StartPositionAnimation(Vector2 targetPosition, P.TimeRatio timeRatio) {
		AnimatedPosition.TargetValue= targetPosition;
		AnimatedPosition.Start(timeRatio);
	}

    // ======================================================================
	// Animation stop functions
    // ----------------------------------------------------------------------
    void StopAnimations() {
        StopRectAnimation();
    }
    // ----------------------------------------------------------------------
    void StopRectAnimation() {
        StopSizeAnimation();
        StopPositionAnimation();
    }
    // ----------------------------------------------------------------------
    void StopPositionAnimation() {
        AnimatedPosition.Reset();
    }
    
    // ======================================================================
	// Animation timer builders
    // ----------------------------------------------------------------------
    public static float AnimationTimeFromPosition(Vector2 p1, Vector2 p2) {
        var distance= Vector2.Distance(p1,p2);
	    return AnimationTimeFromDistance(distance);
    }
    // ----------------------------------------------------------------------
    public static float AnimationTimeFromSize(Vector2 s1, Vector2 s2) {
        var distance= Vector2.Distance(s1,s2);
	    return AnimationTimeFromDistance(distance);
    }
    // ----------------------------------------------------------------------
    public static float AnimationTimeFromRect(Rect r1, Rect r2) {
        var distance= Vector2.Distance(new Vector2(r1.x,r1.y), new Vector2(r2.x,r2.y));
        var t= Vector2.Distance(new Vector2(r1.xMax,r1.y), new Vector2(r2.xMax,r2.y));
        if(t > distance) distance= t;
        t= Vector2.Distance(new Vector2(r1.xMax,r1.yMax), new Vector2(r2.xMax,r2.yMax));
        if(t > distance) distance= t;
        t= Vector2.Distance(new Vector2(r1.x,r1.yMax), new Vector2(r2.x,r2.yMax));
        if(t > distance) distance= t;        
	    return AnimationTimeFromDistance(distance);
    }
    // ----------------------------------------------------------------------
    public static float AnimationTimeFromDistance(float distance) {
	    return distance/iCS_PreferencesEditor.AnimationPixelsPerSecond;
    }
    // ----------------------------------------------------------------------
	public static P.TimeRatio BuildTimeRatioFromRect(Rect r1, Rect r2) {
	    float time= AnimationTimeFromRect(r1, r2);
		var timeRatio= new P.TimeRatio();
        timeRatio.Start(time);
		return timeRatio;
	}
    // ----------------------------------------------------------------------
	public static P.TimeRatio BuildTimeRatioFromPosition(Vector2 p1, Vector2 p2) {
	    float time= AnimationTimeFromPosition(p1, p2);
		var timeRatio= new P.TimeRatio();
        timeRatio.Start(time);
		return timeRatio;
	}
    // ----------------------------------------------------------------------
	public static P.TimeRatio BuildTimeRatioFromSize(Vector2 s1, Vector2 s2) {
	    float time= AnimationTimeFromSize(s1, s2);
		var timeRatio= new P.TimeRatio();
        timeRatio.Start(time);
		return timeRatio;
	}
    // ----------------------------------------------------------------------
	public static P.TimeRatio BuildTimeRatioFromDistance(float distance) {
	    float time= AnimationTimeFromDistance(distance);
		var timeRatio= new P.TimeRatio();
        timeRatio.Start(time);
		return timeRatio;
	}

    // ======================================================================
	// Animation update
    // ----------------------------------------------------------------------
	public void UpdateAnimation() {
		if(AnimatedSize.IsActive) {
			if(AnimatedSize.IsElapsed) {
				AnimatedSize.Reset();
			} else {
				AnimatedSize.Update();
			}
		}
		if(AnimatedPosition.IsActive) {
			if(AnimatedPosition.IsElapsed) {
				AnimatedPosition.Reset(GlobalLayoutPosition);
			} else {
				AnimatedPosition.Update();
			}
		}
	}

    // ----------------------------------------------------------------------
    public float DisplayAlpha {
        get {
            if(IsPort)      return ParentNode.DisplayAlpha;
            if(!IsAnimated) {
                myInvisibleBeforeAnimation= false;
                return 1f;
            }
            if(!IsVisibleInLayout) {
                return 1f-AnimatedPosition.Ratio;
            }
            if(myInvisibleBeforeAnimation) {
                return AnimatedPosition.Ratio;
            }
            return 1f;
        }
    }
    
}
