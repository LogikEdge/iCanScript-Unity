using UnityEngine;
using System.Collections;

//public class DSViewAdaptor {
//    // ======================================================================
//    // Fields
//    // ----------------------------------------------------------------------
//    DSView  myParentView= null;
//    DSView  myChildView= null;
//    
//    // ======================================================================
//    // Properties
//    // ----------------------------------------------------------------------
//    public DSView ChildView {
//        get { return myChildView; }
//        set { myChildView= value; }
//    }
//
//    // ======================================================================
//    // Initialization
//    // ----------------------------------------------------------------------
//    public DSViewAdaptor(DSView parentView, DSView childView) {
//        myParentView= parentView;
//        myChildView= childView;
//    }
//
//    // ======================================================================
//    // Initialization
//    // ----------------------------------------------------------------------
//    void DisplayAdaptor(Rect displayArea) {
//        if(myChildView != null) myChildView.Display(displayArea);
//    }
//    Vector2 GetDisplaySizeAdaptor(Rect displayArea) {
//        return myChildView != null ? myChildView.GetDisplaySize(displayArea) : Vector2.zero;
//    }
//}
//