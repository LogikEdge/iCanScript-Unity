using UnityEngine;
using System.Collections;

public abstract class AP_Action : AP_Node {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    protected int  myFrameId= 0;
    

    // ======================================================================
    // EXECUTION
    // ----------------------------------------------------------------------
    public abstract void Execute();
    
    // ----------------------------------------------------------------------
    public bool IsCurrent()     { return myFrameId == Top.FrameId; }
    public void MarkAsCurrent() { myFrameId= Top.FrameId; }
}
