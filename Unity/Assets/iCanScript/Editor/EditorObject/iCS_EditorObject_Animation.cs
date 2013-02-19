using UnityEngine;
using System;
using System.Collections;

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
    
    // ----------------------------------------------------------------------    
    void AnimateChildNodes(Action<iCS_EditorObject> fnc) {
        // Create a uniqu timer the all child animations.
		var timer= BuildStandardAnimationTimer();
        // Prepare to animate child nodes.
	    ForEachChildNode(
		    c=> {
			    if(c.IsVisibleInLayout) {
				    c.SetStartValueForDisplayRectAnimation();
			    }
		    }
	    );                
        // Run node modification function.
        fnc(this);
        // Animate all visible children.
	    ForEachChildNode(
		    c=> {
			    if(c.IsVisibleInLayout) {
				    c.StartDisplayRectAnimation(timer);
			    }
		    }
	    );                
    }
}
