using UnityEngine;
using System;
using System.Collections;

//public class DSScrollView : DSView {
//    // ======================================================================
//    // Fields
//    // ----------------------------------------------------------------------
//    Vector2 myScrollPosition= Vector2.zero;
//    Vector2 myContentSize   = Vector2.zero;
//	
//    // ======================================================================
//    // Properties
//    // ----------------------------------------------------------------------
//	Rect ContentArea { get { return new Rect(0,0,myContentSize.x,myContentSize.y); }}
//	
//    // ======================================================================
//    // Initialization
//    // ----------------------------------------------------------------------
//    public DSScrollView(RectOffset margins, bool shouldDisplayFrame= true,
//                        Action<DSView,Rect> displayDelegate= null,
//                        Action<DSView> onViewChangeDelegate= null,
//                        Func<DSView,Vector2> getDisplaySizeDelegate= null)
//    : base(margins, shouldDisplayFrame, displayDelegate, onViewChangeDelegate, getDisplaySizeDelegate) {}
//    
//    // ======================================================================
//    // View behaviour overrides
//    // ----------------------------------------------------------------------
//    public override void Display(DSView parent, Rect displayArea) {
//        // Display bounding box and title.
//        base.Display();
//        myContentSize= GetContentSize();
//        myScrollPosition= GUI.BeginScrollView(DisplayArea, myScrollPosition, ContentArea, false, false);
//            DisplayContent();
//        GUI.EndScrollView();
//    }
//    // ----------------------------------------------------------------------
//	public override Vector2 GetDisplaySize(DSView parent) {
//        Rect contentSize= base.GetDisplaySize(parent);
//        myContentSize= contentSize;
//        // Add scroller if the needed display size exceeds the display area.
//        if(DisplayArea.width < contentSize.x) contentSize.y+= kScrollerSize;
//        if(DisplayArea.height < contentSize.y) contentSize.x+= kScrollerSize;
//        return contentSize;
//    }
//}
