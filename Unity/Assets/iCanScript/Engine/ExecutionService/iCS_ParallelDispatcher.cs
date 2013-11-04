using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class iCS_ParallelDispatcher : iCS_Dispatcher {
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_ParallelDispatcher(iCS_VisualScriptImp visualScript, int priority, int nbOfParameters, int nbOfEnables)
    : base(visualScript, priority, nbOfParameters, nbOfEnables) {}
    
    // ======================================================================
    // Execution
    // ----------------------------------------------------------------------
    protected override void DoExecute(int frameId) {
		int swapCursor= myQueueIdx;
        int queueSize= myExecuteQueue.Count;
        while(myQueueIdx < queueSize) {
            // Attempt to execute child function.
            iCS_Action action= myExecuteQueue[myQueueIdx];
            action.Execute(frameId);            
            if(!action.IsCurrent(frameId)) {
				
                // Update the stalled flag
                IsStalled &= action.IsStalled;
                // Return if we have seen too many stalled children.
                if(++swapCursor >= queueSize) {
                    return;
                }
                // The function is not ready to execute so lets delay the execution.
                Swap(myQueueIdx, swapCursor);
                continue;
            }
            ++myQueueIdx;
			swapCursor= myQueueIdx;
            IsStalled= false;
        }
        // Reset iterators for next frame.
        ResetIterator(frameId);
    }
    // ----------------------------------------------------------------------
    protected override void DoForceExecute(int frameId) {
        // Sort stalled action according to layout rule.
        int best= myQueueIdx;
        int queueSize= myExecuteQueue.Count;
        for(int i= myQueueIdx+1; i < queueSize; ++i) {
            if(myExecuteQueue[best].Priority > myExecuteQueue[i].Priority) {
                best= i;
            }
        }
        if(best != myQueueIdx) {
            Swap(myQueueIdx, best);
        }
        // Force execute the selected action.
        base.DoForceExecute(frameId);
    }
}
