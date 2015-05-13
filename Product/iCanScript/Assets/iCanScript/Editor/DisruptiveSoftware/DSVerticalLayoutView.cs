using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace iCanScript.Internal.Editor {
    
    public class DSVerticalLayoutView : DSView {
        // ======================================================================
        // Fields
        // ----------------------------------------------------------------------
    	DSCellView      	myMainView= null;
    	List<DSCellView>	mySubviews= null;
        List<Rect>      	mySubviewFrames= null;
    
        // ======================================================================
        // Initialization
        // ----------------------------------------------------------------------
        public DSVerticalLayoutView(RectOffset margins, bool shouldDisplayFrame= true) {
            myMainView= new DSCellView(margins, shouldDisplayFrame, DisplayVerticalLayout, GetVerticalLayoutSize);
            mySubviews= new List<DSCellView>();
            mySubviewFrames= new List<Rect>();
        }
    
        // ======================================================================
        // DSView implementation.
        // ----------------------------------------------------------------------
        public override void Display(Rect frameArea) {
            myMainView.Display(frameArea);
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
        // DSVerticalLayoutView implementation.
        // ----------------------------------------------------------------------
        void DisplayVerticalLayout(DSView view, Rect displayArea) {
    		Relayout(displayArea);
            for(int i= 0; i < mySubviews.Count; ++i) {
                mySubviews[i].Display(mySubviewFrames[i]);
            }
        }
        Vector2 GetVerticalLayoutSize(DSView view, Rect displayArea) {
    		Vector2 size= Vector2.zero;
    		foreach(var subview in mySubviews) {
                var subviewSize= subview.GetSizeToDisplay(displayArea);			
    			size.x= Mathf.Max(size.x, subviewSize.x);
    			size.y+= subviewSize.y;
    		}
    		return size;
        }

        // ======================================================================
        // Display
        // ----------------------------------------------------------------------
    	void Relayout(Rect frameArea) {
            // Nothing to recompute if no subviews.
            if(mySubviews.Count < 1) return;

            // Initialize subview height using equal separation of frameArea.
    		bool[] heightAdjusted= new bool[mySubviews.Count];
    		for(int i= 0; i < mySubviews.Count; ++i) {
    			heightAdjusted[i]= false;
    		}
		
    		// Redistribute height if subview does not need the alotted height.
            float y;
    		bool readjustmentNeeded;
            float averageHeight= 0;
    		int nbOfSubviewsToAdjust= mySubviews.Count;
            float remainingHeight= frameArea.height;
    		do {
    			readjustmentNeeded= false;
    	        y= frameArea.y;
    			if(nbOfSubviewsToAdjust != 0) averageHeight= remainingHeight/nbOfSubviewsToAdjust;
    	        for(int i= 0; i < mySubviews.Count; ++i) {
    				var tmp= mySubviewFrames[i]; tmp.y= y; mySubviewFrames[i]= tmp;
    				if(!heightAdjusted[i]) {
    		            Rect subviewFrame= new Rect(frameArea.x, y, frameArea.width, averageHeight);
    		            var subviewSize= mySubviews[i].GetSizeToDisplay(subviewFrame);
    		            if(subviewSize.y < averageHeight) {
    						subviewFrame.height= subviewSize.y;
    						mySubviewFrames[i]= subviewFrame;
    						remainingHeight-= subviewSize.y; 
    						--nbOfSubviewsToAdjust;
    						heightAdjusted[i]= true;
    						readjustmentNeeded= true;
    						break;
    					} else {
    						mySubviewFrames[i]= subviewFrame;
    					}
    				}
    	            y= mySubviewFrames[i].yMax;
    	        }			
    		} while(readjustmentNeeded);
    	}
	
    	// ======================================================================
        // Subview management
        // ----------------------------------------------------------------------
        public void AddSubview(DSView subview, RectOffset margins, DSView.AnchorEnum alignment= DSView.AnchorEnum.TopLeft) {
    		DSCellView container= new DSCellView(margins, false, subview);
    		container.Anchor= alignment;
            mySubviews.Add(container);
            mySubviewFrames.Add(new Rect(0,0,0,0));
        }
        public bool RemoveSubview(DSView subview) {
    		int idx= FindIndexOfSubview(subview);
    		if(idx < 0) return false;
    		mySubviews.RemoveAt(idx);
            mySubviewFrames.RemoveAt(idx);
            return true;
        }
    	public bool ReplaceSubview(DSView toRemove, DSView bySubview, RectOffset margins, DSView.AnchorEnum alignment= DSView.AnchorEnum.TopLeft) {
    		int idx= FindIndexOfSubview(toRemove);
    		if(idx < 0) return false;
    		DSCellView container= new DSCellView(margins, false, bySubview);
    		container.Anchor= alignment;
            mySubviews[idx]= container;
            mySubviewFrames[idx]= new Rect(0,0,0,0);
    		return true;
    	}
    	int FindIndexOfSubview(DSView subview) {
    		int idx= -1;
    		for(int i= 0; i < mySubviews.Count; ++i) {
    			if(mySubviews[i].Subview == subview) {
    				idx= i;
    				break;
    			}
    		}
    		return idx;
    	}
    }

}
