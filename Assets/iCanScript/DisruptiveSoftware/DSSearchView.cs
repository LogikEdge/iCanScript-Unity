using UnityEngine;
using System;
using System.Collections;

public class DSSearchView : DSView {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    Action<Rect>    myDrawCellAction= null;
    Func<Vector2>   myGetContentSize= null;
    Vector2         myContentSize   = Vector2.zero;
        
    // ======================================================================
    // Propreties
    // ----------------------------------------------------------------------
    public Action<Rect> DrawCellAction {
        get { return myDrawCellAction; }
        set { myDrawCellAction= value; }
    }
    public Func<Vector2> GetContentSize {
        get { return myGetContentSize; }
        set { myGetContentSize= value; ReloadData(); }
    }

    // ======================================================================
    // Initialization
    // ----------------------------------------------------------------------
    public DSSearchView(Func<Vector2> getContentSizeFnc, Action<Rect> drawCellAction,
                      RectOffset margins, bool shouldDisplayFrame= false)
     : base(margins, shouldDisplayFrame) {
        myDrawCellAction= drawCellAction;
        GetContentSize= getContentSizeFnc;
    }
    
    // ======================================================================
    // Display
    // ----------------------------------------------------------------------
    public override void Display() {
        if(myDrawCellAction != null) myDrawCellAction(DisplayArea);
    }
    protected override void OnViewAreaChange() {
        base.OnViewAreaChange();
        ReloadData();
    }
    protected override Vector2 GetMinimumFrameSize() {
        return base.GetMinimumFrameSize() + myContentSize;
    }
    protected override Vector2 GetFullFrameSize() {
        return base.GetFullFrameSize() + myContentSize;
    }

    public override void ReloadData() {
        myContentSize= myGetContentSize != null ? myGetContentSize() : Vector2.zero;        
    }
}
