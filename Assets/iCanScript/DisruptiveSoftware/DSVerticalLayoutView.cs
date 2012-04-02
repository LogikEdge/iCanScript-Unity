using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DSVerticalLayoutView : DSViewWithTitle {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
	
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
	
    // =================================================================================
    // Constants
    // ---------------------------------------------------------------------------------
    
    // ======================================================================
    // Initialization
    // ----------------------------------------------------------------------
    public DSVerticalLayoutView(GUIContent title, TextAlignment titleAlignment, bool titleSeperator,
                                RectOffset margins, bool shouldDisplayFrame= true)
        : base(title, titleAlignment, titleSeperator, margins, shouldDisplayFrame) {}
    
    // ----------------------------------------------------------------------
    public override void ReloadData() {
		Relayout();
	}
	
    // ======================================================================
    // Display
    // ----------------------------------------------------------------------
    public override void Display() {
        // Duisplay bounding box and title.
        base.Display();
        ForEachSubview(
            sv=> {
                sv.Display();
            }
        );
    }
    // ----------------------------------------------------------------------
	void Relayout() {
        // Determine minimum & maximum height.
		float minHeight= 0;
		float maxHeight= 0;
		ForEachSubview(
            sv=> {
    			minHeight+= sv.MinimumFrameSize.y;
                maxHeight+= sv.FullFrameSize.y;                
            }
		);
		float deltaHeight= maxHeight-minHeight;
		// Compute frame size for each subview.
        float remainingHeight= BodyArea.y-minHeight;
        if(remainingHeight < 0) remainingHeight= 0;
        float y= BodyArea.y;
        ForEachSubview(
            sv=> {
                var minSize= sv.MinimumFrameSize;
                var maxSize= sv.FullFrameSize;
    		    float delta= remainingHeight*(maxSize.y-minSize.y)/deltaHeight;
    		    float height= minSize.y+delta;
    		    if(height > maxSize.y) height= maxSize.y;
    		    remainingHeight-= height- minSize.y;
    		    deltaHeight-= maxSize.y-minSize.y;
    		    sv.FrameArea= new Rect(BodyArea.x, y, BodyArea.width, height);
    		    y+= height;                
            }
        );
	}
	
    // ======================================================================
    // View dimension change notification.
    // ----------------------------------------------------------------------
    protected override void OnViewAreaChange() {
		base.OnViewAreaChange();
		Relayout();
	}
	
	// ======================================================================
    // Subview management
    // ----------------------------------------------------------------------
    public override void AddSubview(DSView subview) {
		base.AddSubview(subview);
		Relayout();
    }
    public override bool RemoveSubview(DSView subview) {
		bool result= base.RemoveSubview(subview);
		Relayout();
		return result;
    }

}
