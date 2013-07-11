using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class iCS_ParallelDispatcher : iCS_Dispatcher {
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_ParallelDispatcher(iCS_Storage storage, int instanceId, int priority) : base(storage, instanceId, priority) {}
    
    // ======================================================================
    // Execution
    // ----------------------------------------------------------------------
    public override void Execute(int frameId) {
        int entryQueueIdx= myQueueIdx;
        bool stalled= true;
        int tries= 0;
        int maxTries= myExecuteQueue.Count-myQueueIdx;
        int queueSize= myExecuteQueue.Count;
        while(myQueueIdx < queueSize) {
            // Attempt to execute child function.
            iCS_Action action= myExecuteQueue[myQueueIdx];
            action.Execute(frameId);            
            if(!action.IsCurrent(frameId)) {
                // Verify if the child is a stalled dispatcher.
                if(!action.IsStalled) {
                    stalled= false;
                }
                // Return if we have seen too many stalled children.
                if(++tries > maxTries) {
                    IsStalled= stalled && myQueueIdx == entryQueueIdx;
                    return;
                }
                // The function is not ready to execute so lets delay the execution.
                myExecuteQueue.RemoveAt(myQueueIdx);
                myExecuteQueue.Add(action);
                continue;
            }
            ++myQueueIdx;
        }
        // Reset iterators for next frame.
        ResetIterator(frameId);
    }
    // ----------------------------------------------------------------------
    public override void ForceExecute(int frameId) {
        // Sort stalled action according to layout rule.
        int best= myQueueIdx;
        int queueSize= myExecuteQueue.Count;
        for(int i= myQueueIdx+1; i < queueSize; ++i) {
            if(myExecuteQueue[best].Priority > myExecuteQueue[i].Priority) {
                best= i;
            }
        }
        if(best != myQueueIdx) {
            iCS_Action tmp= myExecuteQueue[myQueueIdx];
            myExecuteQueue[myQueueIdx]= myExecuteQueue[best];
            myExecuteQueue[best]= tmp;
        }
        // Force execute the selected action.
        base.ForceExecute(frameId);
    }
}
