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
	// Animation start value setters.
    // ----------------------------------------------------------------------
	void SetRectAnimationStartValue(Rect r) {
		SetSizeAnimationStartValue(new Vector2(r.width, r.height));
		SetPositionAnimationStartValue(Math3D.Middle(r));
	}
    // ----------------------------------------------------------------------
	void SetSizeAnimationStartValue(Vector2 startSize) {
		/*
			FIXME: Should resynchronize the existing animation with the new start value.
		*/
		if(!IsSizeAnimated) {
			AnimatedSize.StartValue= startSize;
		}
	}
    // ----------------------------------------------------------------------
	void SetPositionAnimationStartValue(Vector2 startPos) {
		/*
			FIXME: Should resynchronize the existing animation with the new start value.
		*/
		if(!IsPositionAnimated) {
			AnimatedPosition.StartValue= startPos;
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
	void StartSizeAnimation(Vector2 targetSize) {
        var startSize= AnimatedSize.StartValue;
		var timeRatio= BuildTimeRatioFromSize(startSize, targetSize); 
		StartSizeAnimation(targetSize, timeRatio);
	}
    // ----------------------------------------------------------------------
	void StartPositionAnimation(Vector2 targetPosition) {
        var startPos= AnimatedPosition.StartValue;
		var timeRatio= BuildTimeRatioFromPosition(startPos, targetPosition);
		StartPositionAnimation(targetPosition, timeRatio);
	}
    // ----------------------------------------------------------------------
	void StartRectAnimation(Rect targetRect, P.TimeRatio timeRatio) {
		var targetSize= new Vector2(targetRect.width, targetRect.height);
		var targetPosition= Math3D.Middle(targetRect);
		StartSizeAnimation(targetSize, timeRatio);
		StartPositionAnimation(targetPosition, timeRatio);
	}
    // ----------------------------------------------------------------------
	void StartSizeAnimation(Vector2 targetSize, P.TimeRatio timeRatio) {
		AnimatedSize.TargetValue= targetSize;
		AnimatedSize.Start(timeRatio);
	}
    // ----------------------------------------------------------------------
	void StartPositionAnimation(Vector2 targetPosition, P.TimeRatio timeRatio) {
		AnimatedPosition.TargetValue= targetPosition;
		AnimatedPosition.Start(timeRatio);
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
				AnimatedSize.Reset(LayoutSize);
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
