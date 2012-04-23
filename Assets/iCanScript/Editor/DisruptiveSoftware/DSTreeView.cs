using UnityEngine;
using System.Collections;

public class DSTreeView : DSView {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    DSCellView  myMainView= null;
    
    // ======================================================================
    // Initialization
    // ----------------------------------------------------------------------
    public DSTreeView(RectOffset margin, bool shouldDisplayFrame) {
        myMainView= new DSCellView(margin, shouldDisplayFrame);
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
}
