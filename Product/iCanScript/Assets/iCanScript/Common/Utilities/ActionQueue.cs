using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace iCanScript.Internal {
    
    public class ActionQueue {
        // ======================================================================
        // A list of queued action.
        // ----------------------------------------------------------------------
        List<Action> myQueue= new List<Action>();

        // ----------------------------------------------------------------------
        // Queue an action for later execution.
        // ----------------------------------------------------------------------
        public void QueueAction(Action fnc) {
            myQueue.Add(fnc);
        }
        // ----------------------------------------------------------------------
        // Execute all queued actions then empty queue.
        // ----------------------------------------------------------------------
        public void RunQueuedActions() {
    		myQueue.ForEach(f=> f());
    		myQueue.Clear();
        }
    }

}
