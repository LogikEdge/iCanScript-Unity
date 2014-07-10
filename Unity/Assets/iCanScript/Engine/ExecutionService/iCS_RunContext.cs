//#define EXECUTION_TRACE
using UnityEngine;

public class iCS_RunContext {
    // ======================================================================
    // Fields
    int          myFrameId= 0;
    iCS_Action   myAction= null;
    
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    public iCS_Action Action {
        get { return myAction; }
        set { myAction= value; myFrameId= 0; }
    }
    public int FrameId {
        get { return myFrameId; }
    }
    
    // ======================================================================
    // Methods
    // ----------------------------------------------------------------------
    public iCS_RunContext(iCS_Action action) {
        Action= action;
    }

    // ----------------------------------------------------------------------
    // Executes the run context (if it is valid)
    public void Run() {
        if(myAction == null) return;
        ++myFrameId;
        do {
            myAction.Execute(myFrameId);                                
            if(myAction.IsStalled) {
                ResolveDeadLock(0);
            }
        } while(!myAction.IsCurrent(myFrameId));        
    }
    // ----------------------------------------------------------------------
    // Attempt to resolve deadlock by using previous frame data.  This
    // is realized by temporarly deactivating a node that needs to produce
    // data for this frame.
    void ResolveDeadLock(int attempts) {
        // Force execution if to many nested attempts to resolve deadlock
        if(attempts > 10) {
#if UNITY_EDITOR
            if(myAction.VisualScript.IsTraceEnabled) {
                Debug.LogWarning("TOO MANY ATTEMPTS TO RESOLVE DEADLOCKS...FORCING EXECUTION");
            }
#endif
            myAction.ForceExecute(myFrameId);
            return;
        }
        // Get a producer port being waited on.
        var stalledProducerPort= myAction.GetStalledProducerPort(myFrameId);
        if(stalledProducerPort != null) {
            var node= stalledProducerPort.Action;
#if UNITY_EDITOR
            if(myAction.VisualScript.IsTraceEnabled) {
                Debug.LogWarning("Deactivating=> "+node.FullName+" ("+myFrameId+")");
            }
#endif
            node.IsActive= false;
            myAction.Execute(myFrameId);
            if(myAction.IsStalled) {
                ResolveDeadLock(attempts+1);
            }
            node.IsActive= true;
#if UNITY_EDITOR
            if(myAction.VisualScript.IsTraceEnabled) {
                Debug.LogWarning("Activating=> "+node.FullName+" ("+myFrameId+")");
            }
#endif
        }                    
        else {
#if UNITY_EDITOR
            if(myAction.VisualScript.IsTraceEnabled) {
                Debug.LogWarning("DID NOT FIND STALLED PORT BUT MESSAGE HANDLER IS STALLED !!!");
            }
#endif
            myAction.ForceExecute(myFrameId);                    
        }
    }
}
