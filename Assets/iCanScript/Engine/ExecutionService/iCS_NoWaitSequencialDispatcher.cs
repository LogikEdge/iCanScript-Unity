using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class iCS_NoWaitSequencialDispatcher : iCS_Dispatcher {
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_NoWaitSequencialDispatcher(string name, Vector2 layout) : base(name, layout) {}
    
    // ======================================================================
    // Execution
    // ----------------------------------------------------------------------
    public override void Execute(int frameId) {
        // Attempt to execute child functions.
        bool stalled= true;
        for(int i= myQueueIdx; i < myExecuteQueue.Count; ++i) {
            iCS_Action action= myExecuteQueue[i];
            bool didExecute= action.IsCurrent(frameId);
            if(!didExecute) {
                action.Execute(frameId);                
                if(action.IsCurrent(frameId)) {
                    didExecute= true;
                    stalled= false;
                } else {
                    // Verify if the child is a staled dispatcher.
                    if(!action.IsStalled) {
                        stalled= false;
                    }                    
                }
            }
            if(didExecute && i == myQueueIdx) {
                ++myQueueIdx;                
            }
        }
        // Don't mark as completed if not all actions have ran.
        if(myQueueIdx < myExecuteQueue.Count) {
            IsStalled= stalled;
            return;
        }
        // Reset iterators for next frame.
        ResetIterator(frameId);
    }
}
