//#define EXECUTION_TRACE
using UnityEngine;
using System.Collections.Generic;
using Subspace;

public class iCS_RunContext {
    // ======================================================================
    // Fields
    int             myFrameId= 0;
    SSAction        myAction= null;
    List<SSAction>  myStalledActions= new List<SSAction>();
    
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    public SSAction Action {
        get { return myAction; }
        set { myAction= value; myFrameId= 0; }
    }
    public int FrameId {
        get { return myFrameId; }
    }
    
    // ======================================================================
    // Methods
    // ----------------------------------------------------------------------
    public iCS_RunContext(SSAction action) {
        Action= action;
        if(Action != null) {
            Action.IsActive= false;
        }
    }

    // ----------------------------------------------------------------------
    // Executes the run context (if it is valid)
    public void Run() {
        if(myAction == null) return;
        ++myFrameId;
        myAction.IsActive= true;
        do {
            myAction.Execute(myFrameId);                                
            if(myAction.IsStalled) {
                ResolveDeadLock(0);
            }
        } while(!myAction.IsCurrent(myFrameId));
        myAction.IsActive= false;
    }
    // ----------------------------------------------------------------------
    // Attempt to resolve deadlock by using previous frame data.  This
    // is realized by temporarly deactivating a node that needs to produce
    // data for this frame.
    //
    // Prefered order of resolution:
    // 0) Waiting on external message handler or public function
    // 1) Waiting on Mux
    // 2) Waiting on Data
    // 3) Waiting on Enable(s)
    void ResolveDeadLock(int attempts) {
        // Attempt to recover in the same order as last time
        if(attempts == 0) {
            var cnt= myStalledActions.Count;
            if(cnt != 0) {
                for(int i= 0; i < cnt; ++i) {
                    var node= myStalledActions[i];
                    if(node.IsStalled) {
                        node.IsActive= false;
                        myAction.Execute(myFrameId);
                        node.IsActive= true;                        
                        if(!myAction.IsStalled) return;
                    }
                }
                myStalledActions.Clear();
            }
        }
        // Force execution if to many nested attempts to resolve deadlock
        if(attempts > 10) {
//#if UNITY_EDITOR
            if(myAction.VisualScript.IsTraceEnabled) {
                Debug.LogWarning("TOO MANY ATTEMPTS TO RESOLVE DEADLOCKS...FORCING EXECUTION");
            }
//#endif
            myAction.ForceExecute(myFrameId);
            return;
        }
        // Get a producer port being waited on.
        var stalledProducerPort= myAction.GetStalledProducerPort(myFrameId);
        if(stalledProducerPort != null) {
            var node= stalledProducerPort.Action;
//#if UNITY_EDITOR
            if(myAction.VisualScript.IsTraceEnabled) {
                Debug.LogWarning("Deactivating=> "+node.FullName+" ("+myFrameId+")");
            }
//#endif
            myStalledActions.Add(node);
            node.IsActive= false;
            myAction.Execute(myFrameId);
            if(myAction.IsStalled) {
                ResolveDeadLock(attempts+1);
            }
            node.IsActive= true;
//#if UNITY_EDITOR
            if(myAction.VisualScript.IsTraceEnabled) {
                Debug.LogWarning("Activating=> "+node.FullName+" ("+myFrameId+")");
            }
//#endif
        }                    
        else {
//#if UNITY_EDITOR
            if(myAction.VisualScript.IsTraceEnabled) {
                Debug.LogWarning("DID NOT FIND STALLED PORT BUT MESSAGE HANDLER IS STALLED !!!");
            }
//#endif
            myAction.ForceExecute(myFrameId);                    
        }
    }
}
