using UnityEngine;
using System;
using System.Collections;

public partial class iCS_EditorObject {
    // ----------------------------------------------------------------------    
    void AnimateNode(Action<iCS_EditorObject> fnc) {
        // Keep a copy of the current node rect.
		SetStartValueForDisplayRectAnimation();
        // Create a uniqu timer the all child animations.
		var timer= BuildStandardAnimationTimer();
        // Prepare to animate child nodes.
	    ForEachChildRecursiveDepthFirst(
		    c=> {
			    if(c.IsNode && c.IsVisibleInLayout) {
				    c.SetStartValueForDisplayRectAnimation();
			    }
		    }
	    );                
        // Run node modification function.
        fnc(this);
        // Animate all visible children.
	    ForEachChildRecursiveDepthFirst(
		    c=> {
			    if(c.IsNode && c.IsVisibleInLayout) {
				    c.StartDisplayRectAnimation(timer);
			    }
		    }
	    );                
        // Animate current node.
    	StartDisplayRectAnimation(timer);        
    }
}
