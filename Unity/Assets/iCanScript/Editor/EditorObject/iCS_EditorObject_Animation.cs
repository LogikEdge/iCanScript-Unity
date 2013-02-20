using UnityEngine;
using System;
using System.Collections;
using P=Prelude;

public partial class iCS_EditorObject {
    // ----------------------------------------------------------------------
    // Returns true if the display size is currently being animated.
    public bool IsDisplaySizeAnimated {
        get {
            return  myAnimatedDisplaySize.IsActive;
        }
    }
    // ----------------------------------------------------------------------
    // Returns true if the display position is currently being animated.
    public bool IsDisplayPositionAnimated {
        get {
            return  myAnimatedDisplayPosition.IsActive;            
        }
    }
    // ----------------------------------------------------------------------
    // Returns true if the display size or position are being animated.
    public bool IsAnimated {
        get {
            if(IsDisplaySizeAnimated) return true;
            return IsDisplayPositionAnimated;
        }
    }
    // ----------------------------------------------------------------------
	void SetStartValueForDisplayRectAnimation() {
		SetStartValueForDisplaySizeAnimation();
		SetStartValueForDisplayPositionAnimation();
	}
    // ----------------------------------------------------------------------
	void SetStartValueForDisplaySizeAnimation() {
		if(!IsDisplaySizeAnimated) {
			myAnimatedDisplaySize.Reset(LayoutSize);
		}
	}
    // ----------------------------------------------------------------------
	void SetStartValueForDisplayPositionAnimation() {
		if(!IsDisplayPositionAnimated) {
			myAnimatedDisplayPosition.Reset(GlobalLayoutPosition);
		}
	}
    // ----------------------------------------------------------------------
	void SetStartValueForDisplayRectAnimation(Rect r) {
		SetStartValueForDisplaySizeAnimation(new Vector2(r.width, r.height));
		SetStartValueForDisplayPositionAnimation(Math3D.Middle(r));
	}
    // ----------------------------------------------------------------------
	void SetStartValueForDisplaySizeAnimation(Vector2 startSize) {
		if(!IsDisplaySizeAnimated) {
			myAnimatedDisplaySize.Reset(startSize);
		}
	}
    // ----------------------------------------------------------------------
	void SetStartValueForDisplayPositionAnimation(Vector2 startPos) {
		if(!IsDisplayPositionAnimated) {
			myAnimatedDisplayPosition.Reset(startPos);
		}
	}
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
    // ----------------------------------------------------------------------
	void StartDisplayRectAnimation() {
        var startRect= new Rect(myAnimatedDisplayPosition.CurrentValue.x,
                                myAnimatedDisplayPosition.CurrentValue.y,
                                myAnimatedDisplaySize.CurrentValue.x,
                                myAnimatedDisplaySize.CurrentValue.y);
		var timeRatio= BuildTimeRatioFromRect(startRect, GlobalLayoutRect);
		StartDisplayRectAnimation(timeRatio);
	}
    // ----------------------------------------------------------------------
	void StartDisplaySizeAnimation() {
        var startSize= myAnimatedDisplaySize.CurrentValue;
		var timeRatio= BuildTimeRatioFromSize(startSize, LayoutSize); 
		StartDisplaySizeAnimation(timeRatio);
	}
    // ----------------------------------------------------------------------
	void StartDisplayPositionAnimation() {
        var startPos= myAnimatedDisplayPosition.CurrentValue;
		var timeRatio= BuildTimeRatioFromPosition(startPos, GlobalLayoutPosition);
		StartDisplayPositionAnimation(timeRatio);
	}
    // ----------------------------------------------------------------------
	void StartDisplayRectAnimation(P.TimeRatio timeRatio) {
		StartDisplaySizeAnimation(timeRatio);
		StartDisplayPositionAnimation(timeRatio);
	}
    // ----------------------------------------------------------------------
	void StartDisplaySizeAnimation(P.TimeRatio timeRatio) {
		myAnimatedDisplaySize.Start(myAnimatedDisplaySize.CurrentValue,
							        LayoutSize,
							        timeRatio,
                                    (start,end,ratio)=>Math3D.Lerp(start,end,ratio));
	}
    // ----------------------------------------------------------------------
	void StartDisplayPositionAnimation(P.TimeRatio timeRatio) {
		myAnimatedDisplayPosition.Start(myAnimatedDisplayPosition.CurrentValue,
										GlobalLayoutPosition,
										timeRatio,
		                                (start,end,ratio)=>Math3D.Lerp(start,end,ratio));
	}
    // ----------------------------------------------------------------------
	public void UpdateAnimation() {
		if(myAnimatedDisplaySize.IsActive) {
			if(myAnimatedDisplaySize.IsElapsed) {
				myAnimatedDisplaySize.Reset(LayoutSize);
			} else {
				myAnimatedDisplaySize.Update();
			}
		}
		if(myAnimatedDisplayPosition.IsActive) {
			if(myAnimatedDisplayPosition.IsElapsed) {
				myAnimatedDisplayPosition.Reset(GlobalLayoutPosition);
			} else {
				myAnimatedDisplayPosition.Update();
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
                return 1f-myAnimatedDisplayPosition.Ratio;
            }
            if(myInvisibleBeforeAnimation) {
                return myAnimatedDisplayPosition.Ratio;
            }
            return 1f;
        }
    }
    
}
