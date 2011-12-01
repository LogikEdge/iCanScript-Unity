using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UK_NoWaitSequencialDispatcher : UK_DispatcherBase {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    const int retriesBeforeDeclaringStaled= 3;

    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public UK_NoWaitSequencialDispatcher(string name) : base(name) {}
    
    // ======================================================================
    // Execution
    // ----------------------------------------------------------------------
    public override void Execute(int frameId) {
        // Attempt to execute child functions.
        bool stalled= true;
        for(int i= myQueueIdx; i < myExecuteQueue.Count; ++i) {
            UK_Action action= myExecuteQueue[i];
            bool didExecute= action.IsCurrent(frameId);
            if(!didExecute) {
                action.Execute(frameId);                
                if(action.IsCurrent(frameId)) {
                    didExecute= true;
                    stalled= false;
                } else {
                    // Verify if the child is a staled dispatcher.
                    UK_IDispatcher childDispatcher= action as UK_IDispatcher;
                    if(childDispatcher != null && !childDispatcher.IsStalled) {
                        stalled= false;
                    }                    
                }
            }
            if(didExecute && i == myQueueIdx) {
                ++myQueueIdx;                
            }
        }
        // Verify for a staled condition.
        if(!stalled) {
            myNbOfRetries= 0;
            myIsStalled= false;
        } else {
            if(++myNbOfRetries >= retriesBeforeDeclaringStaled) {
                myIsStalled= true;
            }
        }
        // Don't mark as completed if not all actions have ran.
        if(myQueueIdx < myExecuteQueue.Count) return;
        // Reset iterators for next frame.
        myQueueIdx= 0;
        myNbOfRetries= 0;
        MarkAsCurrent(frameId);
    }
}
