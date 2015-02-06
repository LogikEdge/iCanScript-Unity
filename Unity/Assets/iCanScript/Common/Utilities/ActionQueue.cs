using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace iCanScript { namespace Utilities {
    
    public class ActionQueue {
        // ======================================================================
        // Fields
        // ----------------------------------------------------------------------
        List<Action> myQueue= new List<Action>();

        // ----------------------------------------------------------------------
        // Queue a command for later execution.
        public void QueueAction(Action fnc) {
            myQueue.Add(fnc);
        }
        // ----------------------------------------------------------------------
        // Execute all queued commands
        public void RunQueuedActions() {
    		myQueue.ForEach(f=> f());
    		myQueue.Clear();
        }
    }

}}
