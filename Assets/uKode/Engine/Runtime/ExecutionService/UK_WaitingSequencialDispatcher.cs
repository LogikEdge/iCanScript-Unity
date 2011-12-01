using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UK_WaitingSequencialDispatcher : UK_DispatcherBase {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    const int retriesBeforeDeclaringStaled= 5;
          int myNbOfRetries= 0;

    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public UK_WaitingSequencialDispatcher(string name) : base(name) {}
    
    // ======================================================================
    // Execution
    // ----------------------------------------------------------------------
    public override void Execute(int frameId) {
        bool staled= true;
        while(myQueueIdx < myExecuteQueue.Count) {
            UK_Action action= myExecuteQueue[myQueueIdx];
            action.Execute(frameId);            
            if(!action.IsCurrent(frameId)) {
                // Verify if the child is a staled dispatcher.
                UK_DispatcherBase childDispatcher= action as UK_DispatcherBase;
                if(childDispatcher != null && !childDispatcher.IsStaled) {
                    staled= false;
                }                    
                if(!staled) {
                    myNbOfRetries= 0;
                    myIsStaled= false;
                } else {
                    if(++myNbOfRetries > retriesBeforeDeclaringStaled) {
                        myIsStaled= true;
                    }
                }
                return;
            }
            staled= false;
            ++myQueueIdx;
        }
        // Reset iterators for next frame.
        myIsStaled= false;
        myQueueIdx= 0;
        myNbOfRetries= 0;
        MarkAsCurrent(frameId);
    }
}
