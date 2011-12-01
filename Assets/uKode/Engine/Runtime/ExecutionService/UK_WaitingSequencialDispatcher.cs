using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UK_WaitingSequencialDispatcher : UK_DispatcherBase {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    const int retriesBeforeDeclaringStaled= 5;

    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public UK_WaitingSequencialDispatcher(string name) : base(name) {}
    
    // ======================================================================
    // Execution
    // ----------------------------------------------------------------------
    public override void Execute(int frameId) {
        bool stalled= true;
        while(myQueueIdx < myExecuteQueue.Count) {
            UK_Action action= myExecuteQueue[myQueueIdx];
            action.Execute(frameId);            
            if(!action.IsCurrent(frameId)) {
                // Verify if the child is a staled dispatcher.
                UK_IDispatcher childDispatcher= action as UK_IDispatcher;
                if(childDispatcher != null && !childDispatcher.IsStalled) {
                    stalled= false;
                }                    
                if(!stalled) {
                    myNbOfRetries= 0;
                    myIsStalled= false;
                } else {
                    if(++myNbOfRetries > retriesBeforeDeclaringStaled) {
                        myIsStalled= true;
                    }
                }
                return;
            }
            stalled= false;
            ++myQueueIdx;
        }
        // Reset iterators for next frame.
        myIsStalled= false;
        myQueueIdx= 0;
        myNbOfRetries= 0;
        MarkAsCurrent(frameId);
    }
}
