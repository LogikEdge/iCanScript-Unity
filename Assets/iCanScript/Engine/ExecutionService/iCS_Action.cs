using UnityEngine;
using System.Collections;

/// =========================================================================
// An action is the base class of the execution.  It includes a frame
// identifier that is used to indicate if the action has been run.  This
// indicator is the bases for the execution synchronization.
public abstract class iCS_Action : iCS_Object {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    int  myFrameId  = 0;
    bool myIsStalled= false;

    // ======================================================================
    // Accessors
    // ----------------------------------------------------------------------
    public int  FrameId   { get { return myFrameId; }}
    public bool IsStalled { get { return myIsStalled; } set { myIsStalled= value; }}
    
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_Action(string name, int priority) : base(name, priority) {}
     
    // ======================================================================
    // Execution
    // ----------------------------------------------------------------------
    public abstract void Execute(int frameId);
    public abstract void ForceExecute(int frameId);
    
    // ----------------------------------------------------------------------
    public bool IsCurrent(int frameId)     { return myFrameId == frameId; }
    public void MarkAsCurrent(int frameId) { myFrameId= frameId; myIsStalled= false; }
}
