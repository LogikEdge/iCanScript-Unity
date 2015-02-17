using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Subspace;

public class iCS_ParallelDispatcher : iCS_Dispatcher {
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_ParallelDispatcher(string name, SSObject parent, int priority, int nbOfParameters, int nbOfEnables)
    : base(name, parent, priority, nbOfParameters, nbOfEnables) {}
    
    // ======================================================================
    // Execution
    // ----------------------------------------------------------------------
    protected override void DoEvaluate() {
		int swapCursor= myQueueIdx;
        int queueSize= myExecuteQueue.Count;
        while(myQueueIdx < queueSize) {
            // Attempt to execute child function.
            SSAction action= myExecuteQueue[myQueueIdx];
            action.Evaluate();            
            if(!action.IsEvaluated) {
				
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
        ResetIterator();            
    }
    // ----------------------------------------------------------------------
    protected override void DoExecute() {
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
        base.DoExecute();
    }
}
