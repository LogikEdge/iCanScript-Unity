using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Subspace;

public class iCS_NoWaitSequencialDispatcher : iCS_Dispatcher {
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_NoWaitSequencialDispatcher(string name, SSObject parent, int priority, int nbOfParameters, int nbOfEnables)
    : base(name, parent, priority, nbOfParameters, nbOfEnables) {}
    
    // ======================================================================
    // Execution
    // ----------------------------------------------------------------------
    protected override void DoEvaluate() {
        // Attempt to execute child functions.
        bool stalled= true;
        for(int i= myQueueIdx; i < myExecuteQueue.Count; ++i) {
            SSAction action= myExecuteQueue[i];
            bool didExecute= action.IsEvaluated;
            if(!didExecute) {
                action.Evaluate();                
                if(action.IsEvaluated) {
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
        ResetIterator();
    }
}
