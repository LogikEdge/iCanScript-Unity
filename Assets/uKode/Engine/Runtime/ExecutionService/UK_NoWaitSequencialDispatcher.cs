using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UK_NoWaitSequencialDispatcher : UK_DispatcherBase {
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public UK_NoWaitSequencialDispatcher(string name) : base(name) {}
    
    // ======================================================================
    // Execution
    // ----------------------------------------------------------------------
    public override void Execute(int frameId) {
        // Attempt to execute child functions.
        int maxTries= myExecuteQueue.Count; maxTries= 1+(maxTries*maxTries+maxTries)/2;
        for(int i= myQueueIdx; i < myExecuteQueue.Count && myNbOfTries < maxTries; ++myNbOfTries, ++i) {
            UK_Action action= myExecuteQueue[i];
            if(!action.IsCurrent(frameId)) {
                action.Execute(frameId);                
            }
            if(i == myQueueIdx && action.IsCurrent(frameId)) {
                ++myQueueIdx;                
            }
        }
        // Verify that the graph is not looping.
        if(myNbOfTries >= maxTries) {
            Debug.LogError("Execution of graph is looping!!! "+myExecuteQueue[myQueueIdx].Name+":"+myExecuteQueue[myQueueIdx].GetType().Name+" is included in the loop. Please break the cycle and retry.");
        }
        // Don't mark as completed if not all actions have ran.
        if(myQueueIdx < myExecuteQueue.Count) return;
        // Reset iterators for next frame.
        myQueueIdx= 0;
        myNbOfTries= 0;
        MarkAsCurrent(frameId);
    }
}
