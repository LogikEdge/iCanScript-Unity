using UnityEngine;
using System.Collections;
using P=Prelude;

// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
//  ANIMATED LAYOUT
// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
public partial class iCS_EditorObject {
    // Fields ========================================================
	private P.Animate<Rect>	myAnimatedPosition= new P.Animate<Rect>();

    // Accessors =====================================================
	public P.Animate<Rect> AnimatedPosition {
		get { return myAnimatedPosition; }
	}
	
	// ----------------------------------------------------------------------
    public Rect AnimationTarget {
        get {
            if(IsVisible) {
                return GlobalRect;
            }
            // Find first visible parent.
            var visibleParent= Parent;
            for(; visibleParent != null && !visibleParent.IsVisible; visibleParent= visibleParent.Parent);
            Vector2 center= (visibleParent ?? this).GlobalPosition;
            return new Rect(center.x, center.y, 0, 0);            
        }
    }
    
	// ----------------------------------------------------------------------
    public bool IsPositionAnimated {
        get {
            return Math3D.IsNotEqual(AnimationTarget, myAnimatedPosition.CurrentValue);
        }
    }
    
	// ----------------------------------------------------------------------
    public void AnimatePosition() {
        var timeRatio= new P.TimeRatio();
        timeRatio.Start(iCS_PreferencesEditor.AnimationTime);
        AnimatePosition(timeRatio);
    }
	// ----------------------------------------------------------------------
    public void AnimatePosition(P.TimeRatio timeRatio) {
        myAnimatedPosition.Start(myAnimatedPosition.CurrentValue,
                                 AnimationTarget,
                                 timeRatio,
                                 (start,end,ratio)=>Math3D.Lerp(start,end,ratio));
    }
	// ----------------------------------------------------------------------
    public void DontAnimatePositionAndChildren() {
        DontAnimatePosition();
        ForEachChildNode(c=> c.DontAnimatePositionAndChildren());                        
    }
	// ----------------------------------------------------------------------
    public void DontAnimatePosition() {
        myAnimatedPosition.Reset(AnimationTarget);
        if(IsNode) {
            ForEachChildPort(p=> p.DontAnimatePosition());
        }
    }
}
