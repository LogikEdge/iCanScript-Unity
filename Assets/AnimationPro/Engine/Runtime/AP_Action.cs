using UnityEngine;
using System.Collections;

/// =========================================================================
// An action is the base class of the execution.  It includes a frame
// identifier that is used to indicate if the action has been run.  This
// indicator is the bases for the execution synchronization.
public abstract class AP_Action : AP_Node {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    protected int  myFrameId= 0;
    

    // ======================================================================
    // EXECUTION
    // ----------------------------------------------------------------------
    protected abstract void Evaluate();
    public    virtual  void Execute()      { Evaluate(); MarkAsCurrent(); }
    
    // ----------------------------------------------------------------------
    public bool IsCurrent()     { return myFrameId == Top.FrameId; }
    public void MarkAsCurrent() { myFrameId= Top.FrameId; }
}
