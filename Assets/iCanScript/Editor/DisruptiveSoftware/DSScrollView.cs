using UnityEngine;
using System;
using System.Collections;

public class DSScrollView : DSView {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    Vector2 		myScrollPosition= Vector2.zero;
    Vector2 		myContentSize   = Vector2.zero;
	Func<Vector2>	myGetContentSize= null;
	Action<Rect>	myDisplayContent= null;
	
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
	Rect ContentArea { get { return new Rect(0,0,myContentSize.x,myContentSize.y); }}
	
    // =================================================================================
    // Constants
    // ---------------------------------------------------------------------------------
    public const float   kScrollerSize = 16f;
    
    // ======================================================================
    // Initialization
    // ----------------------------------------------------------------------
    public DSScrollView(RectOffset margins, bool shouldDisplayFrame= true)
        : base(margins, shouldDisplayFrame) {}
    
    // ======================================================================
    // View behaviour overrides
    // ----------------------------------------------------------------------
    public override void ReloadData() {
		myContentSize= myGetContentSize != null ? myGetContentSize() : Vector2.zero;
	}
	
    // ----------------------------------------------------------------------
    public override void Display() {
        // Display bounding box and title.
        base.Display();
        myScrollPosition= GUI.BeginScrollView(DisplayArea, myScrollPosition, ContentArea, false, false);
			if(myDisplayContent != null) myDisplayContent(ContentArea);
        GUI.EndScrollView();
    }
	
    // ----------------------------------------------------------------------
    protected override void OnViewAreaChange() {
		base.OnViewAreaChange();
		ReloadData();
	}
	
    // ----------------------------------------------------------------------
    protected override Vector2 GetMinimumFrameSize() {
        Vector2 baseSize= base.GetMinimumFrameSize();
        float width= Mathf.Max(baseSize.x, Margins.horizontal+myContentSize.x);
        float height= baseSize.y+myContentSize.y;
        return new Vector2(width, height);
    }
    protected override Vector2 GetFullFrameSize() {
        Vector2 baseSize= base.GetFullFrameSize();
        float width= Mathf.Max(baseSize.x, Margins.horizontal+myContentSize.x);
        float height= baseSize.y+myContentSize.y;
        return new Vector2(width, height);
    }
    
}
