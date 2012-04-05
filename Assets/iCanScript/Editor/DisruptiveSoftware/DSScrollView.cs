using UnityEngine;
using System.Collections;

public class DSScrollView : DSViewWithTitle {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    Vector2 myScrollPosition= Vector2.zero;
	Rect    myDataArea      = new Rect(0,0,0,0);
	
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
	
    // =================================================================================
    // Constants
    // ---------------------------------------------------------------------------------
    
    // ======================================================================
    // Initialization
    // ----------------------------------------------------------------------
    public DSScrollView(GUIContent title, TextAlignment titleAlignment, bool titleSeperator,
                        RectOffset margins, bool shouldDisplayFrame= true)
        : base(title, titleAlignment, titleSeperator, margins, shouldDisplayFrame) {}
    
    // ======================================================================
    // View behaviour overrides
    // ----------------------------------------------------------------------
    public override void ReloadData() {
        if(Subviews.Count == 0) { myDataArea= new Rect(0,0,0,0); return; }
        myDataArea= Subviews[0].FrameArea;
        for(int i= 1; i < Subviews.Count; ++i) {
            myDataArea= Math3D.Union(myDataArea, Subviews[i].FrameArea);
        }
	}
	
    // ----------------------------------------------------------------------
    public override void Display() {
        // Display bounding box and title.
        base.Display();
        myScrollPosition= GUI.BeginScrollView(BodyArea, myScrollPosition, myDataArea, false, false);
        ForEachSubview(
            sv=> {
                sv.Display();
            }
        );
        GUI.EndScrollView();
    }
	
    // ----------------------------------------------------------------------
    protected override void OnViewAreaChange() {
		base.OnViewAreaChange();
	}
	
    // ----------------------------------------------------------------------
    public override void AddSubview(DSView subview) {
		base.AddSubview(subview);
    }
    public override bool RemoveSubview(DSView subview) {
		bool result= base.RemoveSubview(subview);
		return result;
    }

    // ----------------------------------------------------------------------
    protected override bool GetHasHorizontalScroller() {
        return BodyArea.width < myDataArea.width;
    }
    protected override bool GetHasVerticalScroller() {
        return BodyArea.height < myDataArea.width;
    }
    
    // ----------------------------------------------------------------------
    protected override Vector2 GetMinimumFrameSize() {
        Vector2 baseSize= base.GetMinimumFrameSize();
        float width= Mathf.Max(baseSize.x, Margins.horizontal+myDataArea.width);
        float height= baseSize.y+myDataArea.height;
        return new Vector2(width, height);
    }
    protected override Vector2 GetFullFrameSize() {
        Vector2 baseSize= base.GetFullFrameSize();
        float width= Mathf.Max(baseSize.x, Margins.horizontal+myDataArea.width);
        float height= baseSize.y+myDataArea.height;
        return new Vector2(width, height);
    }
    
}
