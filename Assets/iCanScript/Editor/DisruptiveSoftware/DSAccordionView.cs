using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DSAccordionView : DSView {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
	DSVerticalLayoutView	myMainView         = null;
	DSCellView				mySelectionView    = null;
	GUIContent[]			mySelectionIds     = null;
	DSView[]				mySubviews         = null;
	int						mySelectionIdx     = 0;
	int						mySelectionsPerLine= 1;
	
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
	public int SelectionsPerLine {
		get { return mySelectionsPerLine; }
		set { mySelectionsPerLine= value < 1 ? 1 : value; }
	}
	
    // ======================================================================
    // Initialization
    // ----------------------------------------------------------------------
	public DSAccordionView(RectOffset margins, bool shouldDisplayFrame) {
		mySelectionIds= new GUIContent[0];
		mySubviews= new DSView[0];
		
		myMainView= new DSVerticalLayoutView(margins, shouldDisplayFrame);
		mySelectionView= new DSCellView(new RectOffset(0,0,0,0), false, DisplaySelection, GetSelectionSize);
		myMainView.AddSubview(mySelectionView);
	}

	// ======================================================================
	// DSView functionality implementation.
	// ----------------------------------------------------------------------
	public override void Display(Rect frameArea) { 
		int previousSelectionIdx= mySelectionIdx;
		myMainView.Display(frameArea);
		if(mySelectionIdx != previousSelectionIdx) {
			RemoveSwapableSubview(previousSelectionIdx);
			AddSwapableSubview(mySelectionIdx);			
		}
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

	// ======================================================================
	// Selection view implementation.
	// ----------------------------------------------------------------------
	Vector2 GetSelectionSize(DSCellView view, Rect displaySize) {
		Vector2 size= Vector2.zero;
		foreach(var selection in mySelectionIds) {
			var sSize= GUI.skin.button.CalcSize(selection);
			if(sSize.x > size.x) size.x= sSize.x;
			size.y+= sSize.y;
		}			
		return size;
	}
	void DisplaySelection(DSCellView view, Rect position) {
		if(mySelectionIds.Length < 1) return;
		mySelectionIdx= GUI.SelectionGrid(position, mySelectionIdx, mySelectionIds, SelectionsPerLine);
	}
	
    // ======================================================================
    // Subview management
    // ----------------------------------------------------------------------
    public void AddSubview(GUIContent selecionId, DSView subview) {
		// Add to selection id array.
		System.Array.Resize(ref mySelectionIds, mySelectionIds.Length+1);
		mySelectionIds[mySelectionIds.Length-1]= selecionId;
		// Add to subview array.
		System.Array.Resize(ref mySubviews, mySubviews.Length+1);
		mySubviews[mySubviews.Length-1]= subview;
		// Prime first subview.
		if(mySubviews.Length == 1) {
			mySelectionIdx= 0;
			AddSwapableSubview(mySelectionIdx);
		}
    }
    // ----------------------------------------------------------------------
    public bool RemoveSubview(DSView subview) {
		bool found= false;
		int idx= 0;
		for(; idx < mySubviews.Length; ++idx) {
			if(mySubviews[idx] == subview) {
				found= true;
				break;
			}
		}
		if(!found) return false;
		// Recompute selected subview if it was removed.
		bool needNewSelection= false;
		if(idx == mySelectionIdx) {
			RemoveSwapableSubview(mySelectionIdx);
			needNewSelection= true;
		}
		// Cleanup internal information.
		for(int i= idx+1; i < mySelectionIds.Length; ++i) {
			mySelectionIds[i-1]= mySelectionIds[i];
		}
		System.Array.Resize(ref mySelectionIds, mySelectionIds.Length-1);
		for(int i= idx+1; i < mySubviews.Length; ++i) {
			mySubviews[i-1]= mySubviews[i];
		}
		System.Array.Resize(ref mySubviews, mySubviews.Length-1);
		// Rearrange selection.
		if(needNewSelection) {
			if(mySelectionIdx != 0) {
				--mySelectionIdx;
			}
			AddSwapableSubview(mySelectionIdx);
		}
        return true;
    }
    // ----------------------------------------------------------------------
	void AddSwapableSubview(int idx) {
		if(idx < mySubviews.Length) {
			myMainView.AddSubview(mySubviews[idx]);
		}
	}
    // ----------------------------------------------------------------------
	void RemoveSwapableSubview(int idx) {
		if(idx < mySubviews.Length) {
			myMainView.RemoveSubview(mySubviews[idx]);
		}
	}
}
