using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class DSTreeView : DSView {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
	DSTreeViewDataSource	myDataSource        = null;
    DSCellView  			myMainView          = null;
	float					myIndentOffset      = kHorizontalSpacer;
    Dictionary<object,bool>	myIsFoldedDictionary= null;

    // ======================================================================
    // Initialization
    // ----------------------------------------------------------------------
    public DSTreeView(RectOffset margin, bool shouldDisplayFrame, DSTreeViewDataSource dataSource, float indentOffset= kHorizontalSpacer) {
        myMainView= new DSCellView(margin, shouldDisplayFrame);
		myDataSource= dataSource;
		myIndentOffset= indentOffset;
		myIsFoldedDictionary= new Dictionary<object,bool>();
    }
    
    // ======================================================================
    // DSView functionality implementation.
    // ----------------------------------------------------------------------
    public override void Display(Rect frameArea) { 
		if(myDataSource == null) return;

		float y= frameArea.y;
		float indent= 0;
		myDataSource.Reset();
		if(!myDataSource.MoveToNext()) return;

		while(true) {
			// Determine if current object is folded.
			object key= myDataSource.CurrentObjectKey();
			bool showChildren= false;
			if(!myIsFoldedDictionary.TryGetValue(key, out showChildren)) {
				showChildren= false;
				myIsFoldedDictionary.Add(key, false);
			}

			// Consider size of the current object.
			var currentSize= myDataSource.CurrentObjectDisplaySize();
			Rect displayArea= new Rect(frameArea.x+indent*myIndentOffset, y, currentSize.x, currentSize.y);
			y+= currentSize.y;
			displayArea= Math3D.Intersection(frameArea, displayArea);
			if(Math3D.IsNotZero(displayArea.width) && Math3D.IsNotZero(displayArea.height)) {
				showChildren= myDataSource.DisplayCurrentObject(displayArea, showChildren);
				myIsFoldedDictionary[key]= showChildren;
			}

			if(!showChildren) {
				while(!myDataSource.MoveToNextSibling()) {
					if(!myDataSource.MoveToParent()) {
						return;
					} else {
						--indent;
					}
				}
			} else {
				if(!myDataSource.MoveToFirstChild()) {
					if(!myDataSource.MoveToNextSibling()) {
						if(!myDataSource.MoveToNext()) {
							return;
						} else {
							--indent;
						}
					}
				} else {
					++indent;
				}
			}			
		}

    }
    // ----------------------------------------------------------------------
    public override Vector2 GetSizeToDisplay(Rect frameArea) {
		Vector2 size= Vector2.zero;
		if(myDataSource == null) return size;

		float indent= 0;
		myDataSource.Reset();
		if(!myDataSource.MoveToNext()) return size;

		while(true) {
			// Determine if current object is folded.
			object key= myDataSource.CurrentObjectKey();
			bool showChildren= false;
			myIsFoldedDictionary.TryGetValue(key, out showChildren);

			// Consider size of the current object.
			var currentSize= myDataSource.CurrentObjectDisplaySize();
			currentSize.x+= indent*myIndentOffset;
			if(currentSize.x > size.x) size.x= currentSize.x;
			size.y+= currentSize.y;

			if(!showChildren) {
				while(!myDataSource.MoveToNextSibling()) {
					if(!myDataSource.MoveToParent()) {
						return size;
					} else {
						--indent;
					}
				}
			} else {
				if(!myDataSource.MoveToFirstChild()) {
					if(!myDataSource.MoveToNextSibling()) {
						if(!myDataSource.MoveToNext()) {
							return size;
						} else {
							--indent;
						}
					}
				} else {
					++indent;
				}
			}			
		}
    }
    // ----------------------------------------------------------------------
    public override AnchorEnum GetAnchor() {
        return myMainView.Anchor;
    }
    // ----------------------------------------------------------------------
    public override void SetAnchor(AnchorEnum anchor) {
        myMainView.Anchor= anchor;
    }

}
