using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UK_ParallelDispatcher : UK_Dispatcher {
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public UK_ParallelDispatcher(string name, Vector2 layout) : base(name, layout) {}
    
    // ======================================================================
    // Execution
    // ----------------------------------------------------------------------
    public override void Execute(int frameId) {
        int entryQueueIdx= myQueueIdx;
        bool stalled= true;
        int tries= 0;
        int maxTries= myExecuteQueue.Count-myQueueIdx;
        int queueSize= myExecuteQueue.Count;
        while(myQueueIdx < queueSize) {
            // Attempt to execute child function.
            UK_Action action= myExecuteQueue[myQueueIdx];
            action.Execute(frameId);            
            if(!action.IsCurrent(frameId)) {
                // Verify if the child is a stalled dispatcher.
                if(!action.IsStalled) {
                    stalled= false;
                }
                // Return if we have seen too many stalled children.
                if(++tries > maxTries) {
                    IsStalled= stalled && myQueueIdx == entryQueueIdx;
                    return;
                }
                // The function is not ready to execute so lets delay the execution.
                myExecuteQueue.RemoveAt(myQueueIdx);
                myExecuteQueue.Add(action);
                continue;
            }
            ++myQueueIdx;
        }
        // Reset iterators for next frame.
        ResetIterator(frameId);
    }
    // ----------------------------------------------------------------------
    public override void ForceExecute(int frameId) {
        // Sort stalled action according to layout rule.
        int best= myQueueIdx;
        int queueSize= myExecuteQueue.Count;
        for(int i= myQueueIdx+1; i < queueSize; ++i) {
            if(LayoutRule(myExecuteQueue[best].Layout, myExecuteQueue[i].Layout) == 1) {
                best= i;
            }
        }
        if(best != myQueueIdx) {
            UK_Action tmp= myExecuteQueue[myQueueIdx];
            myExecuteQueue[myQueueIdx]= myExecuteQueue[best];
            myExecuteQueue[best]= tmp;
        }
        // Force execute the selected action.
        base.ForceExecute(frameId);
    }

    // ----------------------------------------------------------------------
    // Returns 0(zero) if l1 is first in layout or 1(one) if l2 is first.
    int LayoutRule(Vector2 l1, Vector2 l2) {
        if(l1.x > l2.x) {
            if(l2.y < l1.y) return 1;
            return (l1.x-l2.x) < 0.03f*(l1.y-l2.y) ? 0 : 1;
        } else {
            if(l1.y < l2.y) return 0;
            return (l2.x-l1.x) < 0.03f*(l1.y-l2.y) ? 1 : 0;
        }
    }
}
