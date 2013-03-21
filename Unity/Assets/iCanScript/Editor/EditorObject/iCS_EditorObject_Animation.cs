using UnityEngine;
using System;
using System.Collections;
using P=Prelude;

public partial class iCS_EditorObject {
    // ======================================================================
	// Fields
    // ----------------------------------------------------------------------
	private bool IsAlphaAnimated= false;
    private P.Animate<Vector2> myAnimatedLayoutOffset=
        new P.Animate<Vector2>((start,end,ratio)=>Math3D.Lerp(start,end,ratio));
	private P.Animate<Vector2> myAnimatedSize=
		new P.Animate<Vector2>((start,end,ratio)=>Math3D.Lerp(start,end,ratio));
	private P.Animate<Rect> myAnimatedRect=
		new P.Animate<Rect>((start,end,ratio)=>Math3D.Lerp(start,end,ratio));
    private Vector2 PreviousAnchor= Vector2.zero;
    
    // ======================================================================
    // Queries
    // ----------------------------------------------------------------------
    // Returns true if the display size is currently being animated.
    public bool IsSizeAnimated {
        get { return myAnimatedSize.IsActive; }
    }
    // ----------------------------------------------------------------------
    // Returns true if the display position is currently being animated.
    public bool IsPositionAnimated {
        get { return myAnimatedLayoutOffset.IsActive; }
    }
    // ----------------------------------------------------------------------
    // Returns true if the display size or position are being animated.
    public bool IsAnimated {
        get { return IsSizeAnimated || IsPositionAnimated || myAnimatedRect.IsActive; }
    }

    // ======================================================================
    // Display Size Animation
    // ----------------------------------------------------------------------
	public void PrepareToAnimateSize() {
		myAnimatedSize.StartValue= myAnimatedSize.CurrentValue;
		if(IsSizeAnimated) {
			myAnimatedSize.Start(myAnimatedSize.RemainingTime);
		}		
	}
    // ----------------------------------------------------------------------
	public void AnimateSize(P.TimeRatio timeRatio) {
		myAnimatedSize.Start(timeRatio);
	}
    // ----------------------------------------------------------------------
	void AnimateSize(Vector2 targetSize) {
        var startSize= myAnimatedSize.CurrentValue;
		var timeRatio= BuildTimeRatioFromSize(startSize, targetSize); 
		AnimateSize(targetSize, timeRatio);
	}
    // ----------------------------------------------------------------------
	void AnimateSize(Vector2 targetSize, P.TimeRatio timeRatio) {
        myAnimatedSize.StartValue= myAnimatedSize.CurrentValue;
		myAnimatedSize.TargetValue= targetSize;
		myAnimatedSize.Start(timeRatio);
	}
    // ----------------------------------------------------------------------
    void StopSizeAnimation() {
        myAnimatedSize.Reset(myAnimatedSize.TargetValue);
    }
    
    // ======================================================================
    // Layout Offset Animation
    // ----------------------------------------------------------------------
	void PrepareToAnimateLayoutOffset() {
        PreviousAnchor= LocalAnchorPosition;
		myAnimatedLayoutOffset.StartValue= myAnimatedLayoutOffset.CurrentValue;
		if(IsPositionAnimated) {
			myAnimatedLayoutOffset.Start(myAnimatedLayoutOffset.RemainingTime);
		} 
	}
    // ----------------------------------------------------------------------
	void AnimateLayoutOffset(P.TimeRatio timeRatio) {
        if(Math3D.IsNotEqual(PreviousAnchor, LocalAnchorPosition)) {
            var offset= LocalAnchorPosition-PreviousAnchor;
            myAnimatedLayoutOffset.StartValue-= offset;
        }
		myAnimatedLayoutOffset.Start(timeRatio);
	}
    // ----------------------------------------------------------------------
    void AnimateLayoutOffset(Vector2 targetLayoutOffset) {
        var startLayoutOffset= myAnimatedLayoutOffset.CurrentValue;
		var timeRatio= BuildTimeRatioFromSize(startLayoutOffset, targetLayoutOffset);         
        AnimateLayoutOffset(targetLayoutOffset, timeRatio);
    }
    // ----------------------------------------------------------------------
    void AnimateLayoutOffset(Vector2 targetLayoutOffset, P.TimeRatio timeRatio) {
        myAnimatedLayoutOffset.StartValue= myAnimatedLayoutOffset.CurrentValue;
		myAnimatedLayoutOffset.TargetValue= targetLayoutOffset;
		myAnimatedLayoutOffset.Start(timeRatio);        
    }
    // ----------------------------------------------------------------------
    void StopLayoutOffsetAnimation() {
        myAnimatedLayoutOffset.Reset(myAnimatedLayoutOffset.TargetValue);
    }
    
    // ======================================================================
	// Position Animation
    // ----------------------------------------------------------------------
	void PrepareToAnimatePosition() {
		PrepareToAnimateLayoutOffset();
	}
    // ----------------------------------------------------------------------
	void AnimatePosition(P.TimeRatio timeRatio) {
		AnimateLayoutOffset(timeRatio);
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
    void StopPositionAnimation() {
        StopLayoutOffsetAnimation();
    }

    // ======================================================================
	// Rect Animation
    // ----------------------------------------------------------------------	
	public void PrepareToAnimateRect() {
//        AnimatedGlobalRect.StartValue= GlobalDisplayRect;
		PrepareToAnimatePosition();
		PrepareToAnimateSize();
	}
    // ----------------------------------------------------------------------
	public void AnimateRect(P.TimeRatio timeRatio) {
//        AnimatedGlobalRect.TargetValue= GlobalDisplayRect;
//        AnimatedGlobalRect.Start(timeRatio);
		AnimatePosition(timeRatio);
		AnimateSize(timeRatio);
	}
    // ----------------------------------------------------------------------
    public void AnimateRect(Rect globalRect) {
        AnimatePosition(PositionFrom(globalRect));
        AnimateSize(SizeFrom(globalRect));
    }
    // ----------------------------------------------------------------------
    void StopRectAnimation() {
        StopPositionAnimation();
        StopSizeAnimation();
    }
    // ----------------------------------------------------------------------
    void StopAnimation() {
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
			var remainingTime= myAnimatedLayoutOffset.RemainingTime;
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
			var remainingTime= myAnimatedSize.RemainingTime;
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
	public static P.TimeRatio BuildTimeRatio(float time) {
		var timeRatio= new P.TimeRatio();
        timeRatio.Start(time);
		return timeRatio;		
	}
    // ======================================================================
	// Animation update
    // ----------------------------------------------------------------------
    // IMPROVE: Should avoid performing the layout on the parents multiple times.
	public void UpdateAnimation() {
		if(myAnimatedRect.IsActive) {
            var prevRect= myAnimatedRect.CurrentValue;
			if(myAnimatedRect.IsElapsed) {
				myAnimatedRect.Reset(myAnimatedRect.TargetValue);
                IsFloating= false;
			} else {
				myAnimatedRect.Update();
			}
			if(!IsFloating && Math3D.IsNotEqual(prevRect, myAnimatedRect.CurrentValue)) {
                LayoutPorts();
				var parent= ParentNode;
				if(parent != null && !parent.IsAnimated) {
	                LayoutParentNodesUntilTop();					
				}
			}
		}
        if(myAnimatedLayoutOffset.IsActive) {
            var prevLayoutOffset= myAnimatedLayoutOffset.CurrentValue;
            if(myAnimatedLayoutOffset.IsElapsed) {
                myAnimatedLayoutOffset.Reset(myAnimatedLayoutOffset.TargetValue);
                IsFloating= false;
            } else {
                myAnimatedLayoutOffset.Update();
            }
            if(!IsFloating && Math3D.IsNotEqual(prevLayoutOffset, myAnimatedLayoutOffset.CurrentValue)) {
				var parent= ParentNode;
				if(parent != null && !parent.IsAnimated) {
	                LayoutParentNodesUntilTop();					
				}
            }
        }
		if(myAnimatedSize.IsActive) {
            var prevSize= myAnimatedSize.CurrentValue;
			if(myAnimatedSize.IsElapsed) {
				myAnimatedSize.Reset(myAnimatedSize.TargetValue);
                IsFloating= false;
			} else {
				myAnimatedSize.Update();
			}
			if(!IsFloating && Math3D.IsNotEqual(prevSize, myAnimatedSize.CurrentValue)) {
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
                return 1f-myAnimatedSize.Ratio;
            }
            return myAnimatedSize.Ratio;
        }
    }
    
}
