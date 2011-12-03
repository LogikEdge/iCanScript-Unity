using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UK_ParallelDispatcher : UK_Dispatcher {
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public UK_ParallelDispatcher(string name) : base(name) {}
    
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
            UK_Action action= myExecuteQueue[myQueueIdx];
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
}
