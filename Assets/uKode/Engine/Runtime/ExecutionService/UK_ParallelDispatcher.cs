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
        bool stalled= true;
        int tries= 0;
        int maxTries= myExecuteQueue.Count-myQueueIdx;
        while(myQueueIdx < myExecuteQueue.Count) {
            // Attempt to execute child function.
            UK_Action action= myExecuteQueue[myQueueIdx];
            action.Execute(frameId);            
            // Move to next child if sucessfully executed.
            if(action.IsCurrent(frameId)) {
                stalled= false;
                ++myQueueIdx;
                continue;
            }
            // Verify if the child is a staled dispatcher.
            if(!action.IsStalled) {
                stalled= false;
            }
            // Return if we have seen too many staled children.
            if(++tries > maxTries) {
                IsStalled= stalled;
                return;
            }
            // The function is not ready to execute so lets delay the execution.
            myExecuteQueue.RemoveAt(myQueueIdx);
            myExecuteQueue.Add(action);
        }
        // Reset iterators for next frame.
        ResetIterator(frameId);
    }
}
