using UnityEngine;
using System;
using System.Collections;

public class DSScrollView : DSView {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    Vector2 		        myScrollPosition= Vector2.zero;
    Vector2 		        myContentSize   = Vector2.zero;
	Func<DSView,Vector2>    myGetContentSizeDelegate= null;
	Action<DSView,Rect>	    myDisplayContentDelegate= null;
    Action<DSView>          myOnViewChangeDelegate  = null;
	
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
    public override void Display(DSView parent, Rect displayArea) {
        // Display bounding box and title.
        base.Display();
        myContentSize= GetContentSize();
        myScrollPosition= GUI.BeginScrollView(DisplayArea, myScrollPosition, ContentArea, false, false);
            DisplayContent();
        GUI.EndScrollView();
    }
	public virtual Vector2 GetContentSize(DSView parent) {
        return InvokeGetContentSizeDelegate();
    }
    
}
