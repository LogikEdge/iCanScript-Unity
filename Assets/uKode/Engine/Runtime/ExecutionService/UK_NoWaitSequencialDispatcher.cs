using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UK_NoWaitSequencialDispatcher : UK_DispatcherBase {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    const int retriesBeforeDeclaringStaled= 3;
          int myNbOfRetries= 0;

    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public UK_NoWaitSequencialDispatcher(string name) : base(name) {}
    
    // ======================================================================
    // Execution
    // ----------------------------------------------------------------------
    public override void Execute(int frameId) {
        // Attempt to execute child functions.
        bool staled= true;
        for(int i= myQueueIdx; i < myExecuteQueue.Count; ++i) {
            UK_Action action= myExecuteQueue[i];
            bool didExecute= action.IsCurrent(frameId);
            if(!didExecute) {
                action.Execute(frameId);                
                if(action.IsCurrent(frameId)) {
                    didExecute= true;
                    staled= false;
                } else {
                    // Verify if the child is a staled dispatcher.
                    UK_DispatcherBase childDispatcher= action as UK_DispatcherBase;
                    if(childDispatcher != null && !childDispatcher.IsStaled) {
                        staled= false;
                    }                    
                }
            }
            if(didExecute && i == myQueueIdx) {
                ++myQueueIdx;                
            }
        }
        // Verify for a staled condition.
        if(!staled) {
            myNbOfRetries= 0;
            myIsStaled= false;
        } else {
            if(++myNbOfRetries >= retriesBeforeDeclaringStaled) {
                myIsStaled= true;
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
