using UnityEngine;
using System;
using System.Collections;
using P=Prelude;

public partial class iCS_EditorObject {
    // ======================================================================
	// Fields
    // ----------------------------------------------------------------------
	private bool IsAlphaAnimated= false;
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
	public void PrepareToAnimateSize() {
		AnimatedSize.StartValue= AnimatedSize.CurrentValue;
		if(IsSizeAnimated) {
			AnimatedSize.Start(AnimatedSize.RemainingTime);
		}		
	}
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
	public void AnimateSize(P.TimeRatio timeRatio) {
		AnimatedSize.Start(timeRatio);
	}
    // ----------------------------------------------------------------------
    void StopSizeAnimation() {
        AnimatedSize.Reset(AnimatedSize.TargetValue);
    }
    
    // ======================================================================
    // Layout Offset Animation
    // ----------------------------------------------------------------------
	void PrepareToAnimateLayoutOffset() {
		AnimatedLayoutOffset.StartValue= AnimatedLayoutOffset.CurrentValue;
		if(IsPositionAnimated) {
			AnimatedLayoutOffset.Start(AnimatedLayoutOffset.RemainingTime);
		} 
	}
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
	void AnimateLayoutOffset(P.TimeRatio timeRatio) {
		AnimatedLayoutOffset.Start(timeRatio);
	}
    // ----------------------------------------------------------------------
    void StopLayoutOffsetAnimation() {
        AnimatedLayoutOffset.Reset(AnimatedLayoutOffset.TargetValue);
    }
    
    // ======================================================================
	// Position Animation
    // ----------------------------------------------------------------------
	void PrepareToAnimatePosition() {
		PrepareToAnimateLayoutOffset();
	}
    // ----------------------------------------------------------------------
    public void AnimatePosition(Vector2 targetPosition) {
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
	void AnimatePosition(P.TimeRatio timeRatio) {
		AnimateLayoutOffset(timeRatio);
	}
    // ----------------------------------------------------------------------
    void StopPositionAnimation() {
        StopLayoutOffsetAnimation();
    }

    // ======================================================================
	// Rect Animation
    // ----------------------------------------------------------------------
	public void PrepareToAnimateRect() {
		PrepareToAnimatePosition();
		PrepareToAnimateSize();
	}
    // ----------------------------------------------------------------------
    public void AnimateRect(Rect globalRect) {
        AnimatePosition(PositionFrom(globalRect));
        AnimateSize(SizeFrom(globalRect));
    }
    // ----------------------------------------------------------------------
    void AnimateRect(Rect globalRect, P.TimeRatio timeRatio) {
        AnimatePosition(PositionFrom(globalRect), timeRatio);
        AnimateSize(SizeFrom(globalRect), timeRatio);
    }
    // ----------------------------------------------------------------------
	public void AnimateRect(P.TimeRatio timeRatio) {
		AnimatePosition(timeRatio);
		AnimateSize(timeRatio);
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
    public static float RawAnimationTimeFromPosition(Vector2 p1, Vector2 p2) {
        var distance= Vector2.Distance(p1,p2);
	    var time= AnimationTimeFromDistance(distance);
        var minAnimationTime= iCS_PreferencesEditor.MinAnimationTime;
        return time < minAnimationTime ? minAnimationTime : time;
    }
    // ----------------------------------------------------------------------
    public float AnimationTimeFromPosition(Vector2 p1, Vector2 p2) {
        var distance= Vector2.Distance(p1,p2);
	    var time= AnimationTimeFromDistance(distance);
		if(IsPositionAnimated) {
			var remainingTime= AnimatedLayoutOffset.RemainingTime;
			return Mathf.Max(remainingTime, time);
		}
        var minAnimationTime= iCS_PreferencesEditor.MinAnimationTime;
        return time < minAnimationTime ? minAnimationTime : time;
    }
    // ----------------------------------------------------------------------
    public float AnimationTimeFromSize(Vector2 s1, Vector2 s2) {
        var distance= Vector2.Distance(s1,s2);
	    var time= AnimationTimeFromDistance(distance);
		if(IsSizeAnimated) {
			var remainingTime= AnimatedSize.RemainingTime;
			return Mathf.Max(remainingTime, time);			
		}
        var minAnimationTime= iCS_PreferencesEditor.MinAnimationTime;
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
	    return distance/iCS_PreferencesEditor.AnimationPixelsPerSecond;
    }
    // ----------------------------------------------------------------------
	public P.TimeRatio BuildTimeRatioFromRect(Rect r1, Rect r2) {
	    return BuildTimeRatio(AnimationTimeFromRect(r1, r2));
	}
    // ----------------------------------------------------------------------
	public P.TimeRatio BuildTimeRatioFromPosition(Vector2 p1, Vector2 p2) {
	    return BuildTimeRatio(AnimationTimeFromPosition(p1, p2));
	}
    // ----------------------------------------------------------------------
	public P.TimeRatio BuildTimeRatioFromSize(Vector2 s1, Vector2 s2) {
	    return BuildTimeRatio(AnimationTimeFromSize(s1, s2));
	}
    // ----------------------------------------------------------------------
	public P.TimeRatio BuildTimeRatioFromDistance(float distance) {
	    return BuildTimeRatio(AnimationTimeFromDistance(distance));
	}
    // ----------------------------------------------------------------------
	public P.TimeRatio BuildTimeRatio(float time) {
		var timeRatio= new P.TimeRatio();
        timeRatio.Start(time);
		return timeRatio;		
	}
    // ======================================================================
	// Animation update
    // ----------------------------------------------------------------------
    // IMPROVE: Should avoid performing the layout on the parents multiple times.
	public void UpdateAnimation() {
        if(AnimatedLayoutOffset.IsActive) {
            var prevLayoutOffset= AnimatedLayoutOffset.CurrentValue;
            if(AnimatedLayoutOffset.IsElapsed) {
                AnimatedLayoutOffset.Reset(AnimatedLayoutOffset.TargetValue);
                IsFloating= false;
            } else {
                AnimatedLayoutOffset.Update();
            }
            if(!IsFloating && Math3D.IsNotEqual(prevLayoutOffset, AnimatedLayoutOffset.CurrentValue)) {
				var parent= ParentNode;
				if(parent != null && !parent.IsAnimated) {
	                LayoutParentNodesUntilTop();					
				}
            }
        }
		if(AnimatedSize.IsActive) {
            var prevSize= AnimatedSize.CurrentValue;
			if(AnimatedSize.IsElapsed) {
				AnimatedSize.Reset(AnimatedSize.TargetValue);
                IsFloating= false;
			} else {
				AnimatedSize.Update();
			}
			if(!IsFloating && Math3D.IsNotEqual(prevSize, AnimatedSize.CurrentValue)) {
                LayoutPorts();
				var parent= ParentNode;
				if(parent != null && !parent.IsAnimated) {
	                LayoutParentNodesUntilTop();					
				}
			}
		}
	}

    // ----------------------------------------------------------------------
    public float DisplayAlpha {
        get {
            if(IsPort) {
                return ParentNode.DisplayAlpha;
            }
			if(!IsAlphaAnimated) {
				return 1f;
			}
            if(!IsAnimated) {
                IsAlphaAnimated= false;
                return 1f;
            }
            if(!IsVisibleInLayout) {
                return 1f-AnimatedSize.Ratio;
            }
            return AnimatedSize.Ratio;
        }
    }
    
}
