using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DSVerticalLayoutView : DSViewWithTitle {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
	List<DSView>	mySubviews= new List<DSView>();
	
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
	public Vector2 FullFrameSize {
		get { return Vector2.zero; }
	}
	
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
    public void ReloadData() {
		Relayout();
	}
	
    // ======================================================================
    // Display
    // ----------------------------------------------------------------------
    protected override void Display() {
        // Duisplay bounding box and title.
        base.Display();
    }
    // ----------------------------------------------------------------------
	void Relayout() {
		mySubviews.Clear();
		ForEachSubview(mySubviews.Add);
		float minHeight= 0;
		foreach(var sv in mySubviews) {
			minHeight+= sv.MinimumFrameSize.y;
		}
	}
	
    // ======================================================================
    // View dimension change notification.
    // ----------------------------------------------------------------------
    protected override void ViewAreaDidChange() {
		base.ViewAreaDidChange();
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
