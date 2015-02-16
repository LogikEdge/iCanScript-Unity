//#define EXECUTION_TRACE
using UnityEngine;
using System.Collections.Generic;
using Subspace;

public class iCS_VSContext {
    // ======================================================================
    // Fields
    SSContext           myContext= null;
//    int                 myFrameId= 0;
    SSAction            myAction= null;
    List<SSAction>      myStalledActions= new List<SSAction>();
    
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    public SSAction Action {
        get { return myAction; }
        set { myAction= value; }
    }
    public int FrameId {
        get { return myContext.RunId; }
    }
    
    // ======================================================================
    // Methods
    // ----------------------------------------------------------------------
    public iCS_VSContext(SSAction action, SSContext context) {
        myContext= context;
        myAction= action;
        if(myAction != null) {
            myAction.IsActive= false;
        }
    }

    // ----------------------------------------------------------------------
    // Executes the run context (if it is valid)
    public void Run() {
        if(myAction == null) return;
        myContext.RunId= myContext.RunId+1;
        myAction.IsActive= true;
        do {
            myAction.Execute(myContext.RunId);                                
            if(myAction.IsStalled) {
                ResolveDeadLock(0);
            }
        } while(!myAction.IsCurrent(myContext.RunId));
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
                        myAction.Execute(myContext.RunId);
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
            if(myAction.Context.IsTraceEnabled) {
                Debug.LogWarning("TOO MANY ATTEMPTS TO RESOLVE DEADLOCKS...FORCING EXECUTION");
            }
//#endif
            myAction.ForceExecute(myContext.RunId);
            return;
        }
        // Get a producer port being waited on.
        var stalledProducerPort= myAction.GetStalledProducerPort(myContext.RunId);
        if(stalledProducerPort != null) {
            var node= stalledProducerPort.Action;
//#if UNITY_EDITOR
            if(myAction.Context.IsTraceEnabled) {
                Debug.LogWarning("Deactivating=> "+node.FullName+" ("+myContext.RunId+")");
            }
//#endif
            myStalledActions.Add(node);
            node.IsActive= false;
            myAction.Execute(myContext.RunId);
            if(myAction.IsStalled) {
                ResolveDeadLock(attempts+1);
            }
            node.IsActive= true;
//#if UNITY_EDITOR
            if(myAction.Context.IsTraceEnabled) {
                Debug.LogWarning("Activating=> "+node.FullName+" ("+myContext.RunId+")");
            }
//#endif
        }                    
        else {
//#if UNITY_EDITOR
            if(myAction.Context.IsTraceEnabled) {
                Debug.LogWarning("DID NOT FIND STALLED PORT BUT MESSAGE HANDLER IS STALLED !!!");
            }
//#endif
            myAction.ForceExecute(myContext.RunId);                    
        }
    }
}
