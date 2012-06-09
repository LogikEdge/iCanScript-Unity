using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/*
	TODO : Add second fold/unfold database so that we can switch when needed.
*/
public class DSTreeView : DSView {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
	DSTreeViewDataSource	myDataSource        = null;
    DSCellView  			myMainView          = null;
	float					myIndentOffset      = kHorizontalSpacer;
    Dictionary<object,bool>	myIsFoldedDictionary= null;
    Dictionary<object,Rect> myRowInfo           = null;

    // ======================================================================
    // Initialization
    // ----------------------------------------------------------------------
    public DSTreeView(RectOffset margin, bool shouldDisplayFrame, DSTreeViewDataSource dataSource, float indentOffset= kHorizontalSpacer) {
        myMainView= new DSCellView(margin, shouldDisplayFrame);
		myDataSource= dataSource;
		myIndentOffset= indentOffset;
		myIsFoldedDictionary= new Dictionary<object,bool>();
        myRowInfo= new Dictionary<object,Rect>();
    }
    
    // ======================================================================
    // DSView functionality implementation.
    // ----------------------------------------------------------------------
    public override void Display(Rect frameArea) { 
		if(myDataSource == null) return;

        // Display tree.
        myRowInfo.Clear();
		float y= frameArea.y;
		float indent= 0;
		myDataSource.Reset();
		if(!myDataSource.MoveToNext()) return;

        myDataSource.BeginDisplay();
		while(true) {
			// Determine if current object is folded.
			object key= myDataSource.CurrentObjectKey();
			bool showChildren= false;
			if(!myIsFoldedDictionary.TryGetValue(key, out showChildren)) {
				showChildren= false;
				myIsFoldedDictionary.Add(key, false);
			}

			// Display current object.
			var currentSize= myDataSource.CurrentObjectDisplaySize();
			Rect displayArea= new Rect(frameArea.x+indent*myIndentOffset, y, currentSize.x, currentSize.y);
            myRowInfo.Add(key, displayArea);
			y+= currentSize.y;
			displayArea= Math3D.Intersection(frameArea, displayArea);
			if(Math3D.IsNotZero(displayArea.width) && Math3D.IsNotZero(displayArea.height)) {
                var fullArea= new Rect(frameArea.x, displayArea.y, frameArea.width, displayArea.height);
				showChildren= myDataSource.DisplayCurrentObject(displayArea, showChildren, fullArea);
				myIsFoldedDictionary[key]= showChildren;
			}

			if(!showChildren) {
				while(!myDataSource.MoveToNextSibling()) {
					if(!myDataSource.MoveToParent()) {
                        ProcessEvents(frameArea);
                        myDataSource.EndDisplay();
						return;
					} else {
						--indent;
					}
				}
			} else {
				if(!myDataSource.MoveToFirstChild()) {
					if(!myDataSource.MoveToNextSibling()) {
						if(!myDataSource.MoveToNext()) {
                            myDataSource.EndDisplay();
                            ProcessEvents(frameArea);
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
    public void Fold(object key) {
		bool showChildren;
		if(myIsFoldedDictionary.TryGetValue(key, out showChildren)) {
            if(showChildren) {
                myIsFoldedDictionary[key]= false;
            }
		}
    }
    // ----------------------------------------------------------------------
    public void Unfold(object key) {
		bool showChildren;
		if(!myIsFoldedDictionary.TryGetValue(key, out showChildren)) {
			myIsFoldedDictionary.Add(key, true);
		} else {
		    if(!showChildren) {
		        myIsFoldedDictionary[key]= true;
		    }
		}        
    }
    // ----------------------------------------------------------------------
    public void ToggleFoldUnfold(object key) {
		bool showChildren= false;
		myIsFoldedDictionary.TryGetValue(key, out showChildren);
        if(showChildren) {
            Fold(key);
        } else {
            Unfold(key);
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
	// ----------------------------------------------------------------------
    void ProcessEvents(Rect frameArea) {
     	Vector2 mousePosition= Event.current.mousePosition;
		switch(Event.current.type) {
            case EventType.ScrollWheel: {
                break;
            }
            case EventType.MouseDown: {
                foreach(var keyValue in myRowInfo) {
                    Rect area= keyValue.Value;
                    if(area.y < mousePosition.y && area.yMax > mousePosition.y) {
                        var mouseInScreenPoint= GUIUtility.GUIToScreenPoint(mousePosition);
                        var areaInScreenPoint= GUIUtility.GUIToScreenPoint(new Vector2(area.x, area.y));
                        var areaInScreenPosition= new Rect(areaInScreenPoint.x, areaInScreenPoint.y, area.width, area.height);
                        myDataSource.MouseDownOn(keyValue.Key, mouseInScreenPoint, areaInScreenPosition);
                        Event.current.Use();
                        return;
                    }
                }
				break;
			}
            case EventType.MouseUp: {
				break;
			}
        }   
    }
}
