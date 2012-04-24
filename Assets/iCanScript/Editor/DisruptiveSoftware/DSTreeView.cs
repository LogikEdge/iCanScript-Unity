using UnityEngine;
using System;
using System.Collections;

public class DSTreeView : DSView {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    DSCellView  myMainView    = null;
	float		myIndentOffset= 1;
	int[]		myTreeIds;
	bool[]		myIsFolded;
    
    // ======================================================================
    // Initialization
    // ----------------------------------------------------------------------
    public DSTreeView(RectOffset margin, bool shouldDisplayFrame, float indentOffset= kHorizontalSpacer) {
        myMainView= new DSCellView(margin, shouldDisplayFrame);
		myIndentOffset= indentOffset;
    }
    
    // ======================================================================
    // DSView functionality implementation.
    // ----------------------------------------------------------------------
    public override void Display(Rect frameArea) { 
    }
    public override Vector2 GetSizeToDisplay(Rect frameArea) {
        return Vector2.zero;
    }
    public override AnchorEnum GetAnchor() {
        return myMainView.Anchor;
    }
    public override void SetAnchor(AnchorEnum anchor) {
        myMainView.Anchor= anchor;
    }

    // ======================================================================
    // Tree structure.
    // ----------------------------------------------------------------------
	void SortTree() {
		Array.Sort(myTreeIds);
	}
}
