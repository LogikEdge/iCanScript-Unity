using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Subspace;

public class iCS_NoWaitSequencialDispatcher : iCS_Dispatcher {
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_NoWaitSequencialDispatcher(int instanceId, string name, SSContext context, int priority, int nbOfParameters, int nbOfEnables)
    : base(instanceId, name, context, priority, nbOfParameters, nbOfEnables) {}
    
    // ======================================================================
    // Execution
    // ----------------------------------------------------------------------
    protected override void DoExecute(int runId) {
        // Attempt to execute child functions.
        bool stalled= true;
        for(int i= myQueueIdx; i < myExecuteQueue.Count; ++i) {
            SSAction action= myExecuteQueue[i];
            bool didExecute= action.IsCurrent(runId);
            if(!didExecute) {
                action.Execute(runId);                
                if(action.IsCurrent(runId)) {
                    didExecute= true;
                    stalled= false;
                } else {
                    // Verify if the child is a staled dispatcher.
                    if(!action.IsStalled) {
                        stalled= false;
                    }                    
                }
            }
            if(didExecute && i == myQueueIdx) {
                ++myQueueIdx;                
            }
        }
        // Don't mark as completed if not all actions have ran.
        if(myQueueIdx < myExecuteQueue.Count) {
            IsStalled= stalled;
            return;
        }
        // Reset iterators for next frame.
        ResetIterator(runId);
    }
}
