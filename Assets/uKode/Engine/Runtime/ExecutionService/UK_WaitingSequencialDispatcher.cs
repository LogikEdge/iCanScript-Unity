using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UK_WaitingSequencialDispatcher : UK_Dispatcher {
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
            UK_IAction action= myExecuteQueue[myQueueIdx];
            action.Execute(frameId);            
            if(!action.IsCurrent(frameId)) {
                // Verify if the child is a staled dispatcher.
                UK_IDispatcher childDispatcher= action as UK_IDispatcher;
                if(childDispatcher != null && !childDispatcher.IsStalled) {
                    stalled= false;
                }                    
                myIsStalled= stalled;
                return;
            }
            stalled= false;
            ++myQueueIdx;
        }
        // Reset iterators for next frame.
        ResetIterator(frameId);
    }
}
