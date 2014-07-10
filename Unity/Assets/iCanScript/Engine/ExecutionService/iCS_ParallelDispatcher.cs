//#define NEW_EXECUTE
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class iCS_ParallelDispatcher : iCS_Dispatcher {
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_ParallelDispatcher(iCS_VisualScriptImp visualScript, int priority, int nbOfParameters, int nbOfEnables)
    : base(visualScript, priority, nbOfParameters, nbOfEnables) {}
    
    // ======================================================================
    // Execution
    // ----------------------------------------------------------------------
    protected override void DoExecute(int frameId) {
#if NEW_EXECUTE
        int queueSize= myExecuteQueue.Count;
        for(int cursor= myQueueIdx; cursor < queueSize; ++cursor) {
            // Attempt to execute child function.
            iCS_Action action= myExecuteQueue[cursor];
            if(action.IsCurrent(frameId)) {
                IsStalled= false;
                if(cursor == myQueueIdx) {
                    ++myQueueIdx;                    
                }
                continue;
            }
            action.Execute(frameId);            
            if(action.IsCurrent(frameId)) {
                if(cursor == myQueueIdx) {
                    IsStalled= false;
                    ++myQueueIdx;                        
                }
                continue;
            }
            IsStalled &= action.IsStalled;                    
        }
        // Reset iterators for next frame.
        if(myQueueIdx >= queueSize) {
            ResetIterator(frameId);            
        }
#else
		int swapCursor= myQueueIdx;
        int queueSize= myExecuteQueue.Count;
        while(myQueueIdx < queueSize) {
            // Attempt to execute child function.
            iCS_Action action= myExecuteQueue[myQueueIdx];
            action.Execute(frameId);            
            if(!action.IsCurrent(frameId)) {
				
                // Update the stalled flag
                IsStalled &= action.IsStalled;
                // Return if we have seen too many stalled children.
                if(++swapCursor >= queueSize) {
                    return;
                }
                // The function is not ready to execute so lets delay the execution.
                Swap(myQueueIdx, swapCursor);
                continue;
            }
            ++myQueueIdx;
			swapCursor= myQueueIdx;
            IsStalled= false;
        }
        ResetIterator(frameId);            
#endif
    }
    // ----------------------------------------------------------------------
    protected override void DoForceExecute(int frameId) {
#if NEW_EXECUTE
        int queueSize= myExecuteQueue.Count;
        for(int cursor= myQueueIdx; cursor < queueSize; ++cursor) {
            // Attempt to execute child function.
            iCS_Action action= myExecuteQueue[cursor];
            if(action.IsCurrent(frameId)) {
                IsStalled= false;
                if(cursor == myQueueIdx) {
                    ++myQueueIdx;                    
                }
                break;
            }
            else {
                action.ForceExecute(frameId);            
                IsStalled= false;
                if(cursor == myQueueIdx) {
                    ++myQueueIdx;                        
                }
                break;
            }                
        }
        // Reset iterators for next frame.
        if(myQueueIdx >= queueSize) {
            ResetIterator(frameId);            
        }
#else
        // Sort stalled action according to layout rule.
        int best= myQueueIdx;
        int queueSize= myExecuteQueue.Count;
        for(int i= myQueueIdx+1; i < queueSize; ++i) {
            if(myExecuteQueue[best].Priority > myExecuteQueue[i].Priority) {
                best= i;
            }
        }
        if(best != myQueueIdx) {
            Swap(myQueueIdx, best);
        }
        // Force execute the selected action.
        base.DoForceExecute(frameId);
#endif
    }
}
