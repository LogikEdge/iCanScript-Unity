using UnityEngine;
using System.Collections;

/// =========================================================================
// An action is the base class of the execution.  It includes a frame
// identifier that is used to indicate if the action has been run.  This
// indicator is the bases for the execution synchronization.
public abstract class WD_Action {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    protected int  myFrameId= 0;
    

    // ======================================================================
    // EXECUTION
    // ----------------------------------------------------------------------
    public  abstract void Evaluate();
    public  virtual  void Execute()      { Evaluate(); MarkAsCurrent(); }
    
    // ----------------------------------------------------------------------
    public bool IsCurrent()     { return myFrameId == Top.FrameId; }
    public void MarkAsCurrent() { myFrameId= Top.FrameId; }
}
