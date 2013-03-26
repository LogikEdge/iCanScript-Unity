using UnityEngine;
using System;
using System.Collections;
using P=Prelude;

public partial class iCS_EditorObject {
    // ======================================================================
	// Fields
    // ----------------------------------------------------------------------
	private P.Animate<Rect> myAnimatedRect=
		new P.Animate<Rect>((start,end,ratio)=>Math3D.Lerp(start,end,ratio));
    
    // ======================================================================
    // Queries
    // ----------------------------------------------------------------------
    // Returns true if the display size or position are being animated.
    public bool IsAnimated {
        get { return myAnimatedRect.IsActive; }
    }
	public void StopAnimation() {
		myAnimatedRect.Reset(myAnimatedRect.TargetValue);
	}
	public float AnimationTimeRatio { get { return myAnimatedRect.Ratio; }}
	
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
		if(IsAnimated) {
			var remainingTime= myAnimatedRect.RemainingTime;
			return Mathf.Max(remainingTime, time);
		}
        var minAnimationTime= iCS_PreferencesEditor.MinAnimationTime;
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
            Rect newRect;
			if(myAnimatedRect.IsElapsed) {
				myAnimatedRect.Reset(myAnimatedRect.TargetValue);
                newRect= LayoutRect;
                IsFloating= false;
			} else {
				myAnimatedRect.Update();
                newRect= myAnimatedRect.CurrentValue;
			}
			if(!IsFloating && Math3D.IsNotEqual(prevRect, newRect)) {
                // Update child ports
                LayoutPorts();
                // Update parent node
				var parent= ParentNode;
				if(parent != null && !parent.IsAnimated) {
//					LayoutUnfoldedParentNodesUsingAnimatedChildren();
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
            if(!IsVisibleInLayout) {
                return 1f-myAnimatedRect.Ratio;
            }
            if(IsAnimated && Math3D.Area(myAnimatedRect.StartValue) < 0.1f) {
                return myAnimatedRect.Ratio;
            }
            return 1f;
        }
    }

    // ======================================================================
    // Full graph animation
    // ----------------------------------------------------------------------
	public P.TimeRatio AnimateGraph(Action<iCS_EditorObject> fnc) {
        if(iCS_PreferencesEditor.AnimationEnabled) {
            EditorObjects[0].ForEachRecursiveDepthLast(
                (c,_)=> c.IsNode,
                node => {
                    // FIXME: Should not have to redo what should be done by AnimatedRect when not visible.
                    if(node.IsVisibleOnDisplay) {
                        node.AnimationStart= node.AnimatedRect;
                    } else {
                        var t= node.ParentNode;
                        while(!t.IsVisibleOnDisplay) t= t.ParentNode;
                        node.AnimationStart= BuildRect(t.AnimatedPosition, Vector2.zero);                    
                    }
                }
            );            
        }
		fnc(this);
        myIStorage.ForcedRelayoutOfTree(EditorObjects[0]);
        var timeRatio= BuildTimeRatioFromRect(AnimationStart, LayoutRect);		
        if(iCS_PreferencesEditor.AnimationEnabled) {
            EditorObjects[0].ForEachRecursiveDepthLast(
                (c,_)=> c.IsNode,
                node=> {
                    var r= node.LayoutRect;
                    if(!node.IsFloating) {
                        if(Math3D.Area(node.AnimationStart) > 0.1f || Math3D.Area(r) > 0.1f) {
                            node.Animate(r, timeRatio);
                        }                        
                    }
                }
            );
        }
		return timeRatio;
	}
    
}
