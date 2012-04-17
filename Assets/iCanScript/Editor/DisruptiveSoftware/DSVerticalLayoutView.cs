using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DSVerticalLayoutView : DSView {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
	DSCellView      myMainView= null;
	List<DSView>    mySubviews= null;
    List<Rect>      mySubviewFrames= null;
    
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
	
    // =================================================================================
    // Constants
    // ---------------------------------------------------------------------------------
    
    // ======================================================================
    // Initialization
    // ----------------------------------------------------------------------
    public DSVerticalLayoutView(RectOffset margins, bool shouldDisplayFrame= true) {
        myMainView= new DSCellView(margins, shouldDisplayFrame);
        mySubviews= new List<DSView>();
        mySubviewFrames= new List<Rect>();
    }
    
    // ======================================================================
    // DSView implementation.
    // ----------------------------------------------------------------------
    public override void Display(Rect frameArea) {
        myMainView.Display(frameArea);
        for(int i= 0; i < mySubviews.Count; ++i) {
            mySubviews[i].Display(mySubviewFrames[i]);
        }
    }
    public override Vector2 GetSizeToDisplay(Rect frameArea) {
        return myMainView.GetSizeToDisplay(frameArea);
    }
    public override AnchorEnum GetAnchor() {
        return myMainView.Anchor;
    }
    public override void SetAnchor(AnchorEnum anchor) {
        myMainView.Anchor= anchor;
    }
    

    // ======================================================================
    // Display
    // ----------------------------------------------------------------------
	void Relayout(Rect frameArea) {
        // Nothing to recompute if no subviews.
        if(mySubviews.Count < 1) return;

        // Determine height for each subview.
        bool needAdjustment= true;
        int maxAdjustments= 5;
        float remainingHeight= frameArea.height;
        float subviewHeight= remainingHeight/mySubviews.Count;
        for(int maxAdjustment= 0; maxAdjustment < 5 && needAdjustment; ++maxAdjustment) {
            float y= frameArea.y;
            for(int i= 0; i < mySubviews.Count; ++i) {
                Rect subviewArea= new Rect(frameArea.x, y, frameArea.width, subviewHeight);
                var subviewSize= mySubviews[i].GetSizeToDisplay(subviewArea);
                if(subviewSize.y < subviewHeight) subviewArea.height= subviewSize.y;
                y= subviewArea.yMax;
            }
        }
//        // Determine minimum & maximum height.
//		float totalMinHeight= 0;
//		float totalMaxHeight= 0;
//		ForEachSubview(
//            sv=> {
//    			totalMinHeight+= sv.MinimumFrameSize.y;
//                totalMaxHeight+= sv.FullFrameSize.y;                
//            }
//		);
//		float totalDeltaHeight= totalMaxHeight-totalMinHeight;
//		// Compute frame size for each subview.
//        float remainingHeight= BodyArea.height-totalMinHeight;
//        if(remainingHeight < 0) remainingHeight= 0;
//        float y= BodyArea.y;
//        ForEachSubview(
//            sv=> {
//                var minSize= sv.MinimumFrameSize;
//                var maxSize= sv.FullFrameSize;
//    		    float delta= remainingHeight*(maxSize.y-minSize.y)/totalDeltaHeight;
//    		    float height= minSize.y+delta;
//    		    if(height > maxSize.y) height= maxSize.y;
//    		    if(sv.DisplayRatio.y != 0 && height > BodyArea.height*sv.DisplayRatio.y) height= BodyArea.height*sv.DisplayRatio.y;
//    		    if(height < minSize.y) height= minSize.y;
//    		    remainingHeight-= height- minSize.y;
//    		    totalDeltaHeight-= maxSize.y-minSize.y;
//    		    sv.FrameArea= new Rect(BodyArea.x, y, BodyArea.width, height);
//    		    y+= height;                
//            }
//        );
	}
	
	// ======================================================================
    // Subview management
    // ----------------------------------------------------------------------
    public void AddSubview(DSView subview) {
        mySubviews.Add(subview);
        mySubviewFrames.Add(new Rect(0,0,0,0));
    }
    public bool RemoveSubview(DSView subview) {
		bool result= mySubviews.Remove(subview);
        if(result) {
            mySubviewFrames.RemoveAt(mySubviewFrames.Count-1);
        }
        return result;
    }
}
