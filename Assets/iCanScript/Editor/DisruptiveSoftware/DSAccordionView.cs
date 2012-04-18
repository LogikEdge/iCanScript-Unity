using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DSAccordionView : DSView {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
	DSVerticalLayoutView	myMainView     = null;
	DSCellView				mySelectionView= null;
	GUIContent[]			mySelections   = null;
	int						mySelectionIdx = 0;
	bool					myHasSelectionChanged= true;
	int						mySelectionsPerLine= 1;
	
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
	public int SelectionsPerLine {
		get { return mySelectionsPerLine; }
		set { mySelectionsPerLine= value < 1 ? 1 : value; }
	}
	
//    // ======================================================================
//    // Initialization
//    // ----------------------------------------------------------------------
//	public DSAccordionView(RectOffset margins, bool shouldDisplayFrame= true)
//	: base(margins, shouldDisplayFrame) {
//		myLayoutView= new DSVerticalLayoutView(null, TextAlignment.Center, false,
//	                                           new RectOffset(0,0,0,0), false);
//		mySelectionView= new DSView(new RectOffset(0,0,0,0), false, DrawSelection, GetSelectionSize);
//		myLayoutView.AddSubview(mySelectionView);
//	}
//
	// ======================================================================
	// DSView functionality implementation.
	// ----------------------------------------------------------------------
	public override void Display(Rect frameArea) { 
		myMainView.Display(frameArea);
	}
	public override Vector2 GetSizeToDisplay(Rect frameArea) {
		return myMainView.GetSizeToDisplay(frameArea);
	}
	public override AnchorEnum GetAnchor() {
		return myMainView.GetAnchor();
	}
	public override void SetAnchor(AnchorEnum anchor) {
		myMainView.SetAnchor(anchor);
	}


//    // ======================================================================
//    // Display
//    // ----------------------------------------------------------------------
//    public override void Display(DSView parent, Rect frameArea) {
//		base.Display(parent, frameArea);
//		myHasSelectionChanged= false;
//		myLayoutView.Display(DisplayArea);
//		if(myHasSelectionChanged) {
//			RemoveSwapableSubview();
//			AddSwapableSubview(mySelectionIdx);			
//		}
//	}
//	Vector2 GetSelectionSize() {
//		Vector2 size= Vector2.zero;
//		if(mySelections != null) {
//			foreach(var selection in mySelections) {
//				var sSize= GUI.skin.button.CalcSize(selection);
//				if(sSize.x > size.x) size.x= sSize.x;
//				size.y+= sSize.y;
//			}			
//		}
//		return size;
//	}
//	void DrawSelection(Rect position) {
//		if(mySelections == null || mySelections.Length < 1) return;
//		mySelectionIdx= GUI.SelectionGrid(position, mySelectionIdx, mySelections, SelectionsPerLine);
//		myHasSelectionChanged= GUI.changed;
//	}
//	
//    // ======================================================================
//    // Subview management
//    // ----------------------------------------------------------------------
//    public override void AddSubview(DSView subview) {
//		base.AddSubview(subview);
//		RebuildSelectionView();
//    }
//    // ----------------------------------------------------------------------
//    public override bool RemoveSubview(DSView subview) {
//		bool result= base.RemoveSubview(subview);
//		RebuildSelectionView();
//        return result;
//    }
//    // ----------------------------------------------------------------------
//	void RebuildSelectionView() {
//		mySelectionIdx= 0;
//		mySelections= new GUIContent[Subviews.Count];
//		for(int i= 0; i < Subviews.Count; ++i) {
//			mySelections[i]= new GUIContent("No Title");
//			DSViewWithTitle svt= Subviews[i] as DSViewWithTitle;
//			if(svt != null && svt.Title != null) {
//				mySelections[i]= svt.Title;
//			}
//		}
//	}
//    // ----------------------------------------------------------------------
//	void AddSwapableSubview(int idx) {
//		if(idx < Subviews.Count) {
//			myLayoutView.AddSubview(Subviews[idx]);
//		}
//	}
//    // ----------------------------------------------------------------------
//	void RemoveSwapableSubview() {
//		for(int i= 0; i < myLayoutView.Subviews.Count; ++i) {
//			for(int j= 0; j < Subviews.Count; ++j) {
//				if(myLayoutView.Subviews[i] == Subviews[j]) {
//					myLayoutView.RemoveSubview(myLayoutView.Subviews[i]);
//					return;
//				}
//			}
//		}
//	}
}
