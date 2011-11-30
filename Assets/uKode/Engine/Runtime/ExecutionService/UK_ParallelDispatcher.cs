using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UK_ParallelDispatcher : UK_Action {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    List<UK_Action> myExecuteQueue= new List<UK_Action>();
    int             myQueueIdx = 0;
    int             myNbOfTries= 0;
    
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public UK_ParallelDispatcher(string name) : base(name) {}
    
    // ======================================================================
    // Execution
    // ----------------------------------------------------------------------
    public override void Execute(int frameId) {
        // Attempt to execute child functions.
        int maxTries= myExecuteQueue.Count; maxTries= 1+(maxTries*maxTries+maxTries)/2;
        for(; myQueueIdx < myExecuteQueue.Count && myNbOfTries < maxTries; ++myNbOfTries) {
            UK_Action action= myExecuteQueue[myQueueIdx];
            action.Execute(frameId);            
            if(!action.IsCurrent(frameId)) {
                // The function is not ready to execute so lets delay the execution.
                myExecuteQueue.RemoveAt(myQueueIdx);
                myExecuteQueue.Add(action);
                return;
            }
            ++myQueueIdx;
        }
        // Verify that the graph is not looping.
        if(myNbOfTries >= maxTries) {
            Debug.LogError("Execution of graph is looping!!! "+myExecuteQueue[myQueueIdx].Name+":"+myExecuteQueue[myQueueIdx].GetType().Name+" is included in the loop. Please break the cycle and retry.");
        }
        // Reset iterators for next frame.
        myQueueIdx= 0;
        myNbOfTries= 0;
        MarkAsCurrent(frameId);
    }


    // ======================================================================
    // Queue Management
    // ----------------------------------------------------------------------
    public void AddChild(UK_Action action) {
        myExecuteQueue.Add(action);
    }
    public void RemoveChild(UK_Action action) {
        myExecuteQueue.Remove(action);
    }
}
