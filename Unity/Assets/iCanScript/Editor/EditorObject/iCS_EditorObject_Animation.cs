using UnityEngine;
using System;
using System.Collections;
using P=Prelude;

public partial class iCS_EditorObject {
    // ======================================================================
	// Fields
    // ----------------------------------------------------------------------
    private P.Animate<Vector2> AnimatedLayoutOffset=
        new P.Animate<Vector2>((start,end,ratio)=>Math3D.Lerp(start,end,ratio));
	private P.Animate<Vector2> AnimatedSize=
		new P.Animate<Vector2>((start,end,ratio)=>Math3D.Lerp(start,end,ratio));

    // ======================================================================
    // Queries
    // ----------------------------------------------------------------------
    // Returns true if the display size is currently being animated.
    public bool IsSizeAnimated {
        get { return AnimatedSize.IsActive; }
    }
    // ----------------------------------------------------------------------
    // Returns true if the display position is currently being animated.
    public bool IsPositionAnimated {
        get { return AnimatedLayoutOffset.IsActive; }
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
    // Layout Offset Animation
    // ----------------------------------------------------------------------
    void AnimateLayoutOffset(Vector2 targetLayoutOffset) {
        var startLayoutOffset= AnimatedLayoutOffset.CurrentValue;
		var timeRatio= BuildTimeRatioFromSize(startLayoutOffset, targetLayoutOffset);         
        AnimateLayoutOffset(targetLayoutOffset, timeRatio);
    }
    // ----------------------------------------------------------------------
    void AnimateLayoutOffset(Vector2 targetLayoutOffset, P.TimeRatio timeRatio) {
        AnimatedLayoutOffset.StartValue= AnimatedLayoutOffset.CurrentValue;
		AnimatedLayoutOffset.TargetValue= targetLayoutOffset;
		AnimatedLayoutOffset.Start(timeRatio);        
    }
    // ----------------------------------------------------------------------
    void StopLayoutOffsetAnimation() {
        AnimatedLayoutOffset.Reset(AnimatedLayoutOffset.TargetValue);
    }
    
    // ======================================================================
	// Position Animation
    // ----------------------------------------------------------------------
    void AnimatePosition(Vector2 targetPosition) {
        var targetLayoutOffset= targetPosition-LocalAnchorPosition;
        var parent= ParentNode;
        if(parent != null) {
            targetLayoutOffset-= parent.GlobalDisplayPosition;
        }
        AnimateLayoutOffset(targetLayoutOffset);
    }
    // ----------------------------------------------------------------------
    void AnimatePosition(Vector2 targetPosition, P.TimeRatio timeRatio) {
        var targetLayoutOffset= targetPosition-LocalAnchorPosition;
        var parent= ParentNode;
        if(parent != null) {
            targetLayoutOffset-= parent.GlobalDisplayPosition;
        }
        AnimateLayoutOffset(targetLayoutOffset, timeRatio);
    }
    // ----------------------------------------------------------------------
    void StopPositionAnimation() {
        StopLayoutOffsetAnimation();
    }

    // ======================================================================
	// Rect Animation
    // ----------------------------------------------------------------------
    void AnimateRect(Rect globalRect) {
        AnimatePosition(PositionFrom(globalRect));
        AnimateSize(SizeFrom(globalRect));
    }
    // ----------------------------------------------------------------------
    void AnimateRect(Rect globalRect, P.TimeRatio timeRatio) {
        AnimatePosition(PositionFrom(globalRect), timeRatio);
        AnimateSize(SizeFrom(globalRect), timeRatio);
    }
    // ----------------------------------------------------------------------
    void StopRectAnimation() {
        StopPositionAnimation();
        StopSizeAnimation();
    }
    // ----------------------------------------------------------------------
    void StopAnimations() {
        StopRectAnimation();
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
        if(AnimatedLayoutOffset.IsActive) {
            if(AnimatedLayoutOffset.IsElapsed) {
                AnimatedLayoutOffset.Reset(AnimatedLayoutOffset.TargetValue);
            } else {
                AnimatedLayoutOffset.Update();
            }
        }
		if(AnimatedSize.IsActive) {
			if(AnimatedSize.IsElapsed) {
				AnimatedSize.Reset(AnimatedSize.TargetValue);
			} else {
				AnimatedSize.Update();
			}
		}
	}

    // ----------------------------------------------------------------------
    public float DisplayAlpha {
        get {
            if(IsPort) {
                return ParentNode.DisplayAlpha;
            }
            if(!IsAnimated) {
                myInvisibleBeforeAnimation= false;
                return 1f;
            }
            if(!IsVisibleInLayout) {
                return 1f-AnimatedLayoutOffset.Ratio;
            }
            if(myInvisibleBeforeAnimation) {
                return AnimatedLayoutOffset.Ratio;
            }
            return 1f;
        }
    }
    
}
