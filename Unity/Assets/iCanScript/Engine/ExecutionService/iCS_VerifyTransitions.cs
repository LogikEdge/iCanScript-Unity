using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class iCS_VerifyTransitions : iCS_Action {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    List<iCS_Transition>  myTransitions        = new List<iCS_Transition>();
    int                  myQueueIdx           = 0;
    iCS_Transition        myTriggeredTransition= null;
    
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    public iCS_Transition TriggeredTransition { get { return myTriggeredTransition; }}
    
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_VerifyTransitions(string name, int priority) : base(name, priority) {}

    // ======================================================================
    // Execution
    // ----------------------------------------------------------------------
    public override void Execute(int frameId) {
        bool stalled= true;
        int tries= 0;
        int maxTries= myTransitions.Count-myQueueIdx;
        myTriggeredTransition= null;
        while(myQueueIdx < myTransitions.Count) {
            // Attempt to execute child function.
            iCS_Transition transition= myTransitions[myQueueIdx];
            transition.Execute(frameId);            
            // Move to next child if sucessfully executed.
            if(transition.IsCurrent(frameId)) {
                if(transition.DidTrigger) {
                    myTriggeredTransition= transition;
                    ResetIterator(frameId);
                    return;
                }
                stalled= false;
                ++myQueueIdx;
                continue;
            }
            // Verify if the child is a staled dispatcher.
            if(!transition.IsStalled) {
                stalled= false;
            }
            // Return if we have seen too many staled children.
            if(++tries > maxTries) {
                IsStalled= stalled;
                return;
            }
            // The function is not ready to execute so lets delay the execution.
            myTransitions.RemoveAt(myQueueIdx);
            myTransitions.Add(transition);
        }
        // Reset iterators for next frame.
        ResetIterator(frameId);
    }
    // ----------------------------------------------------------------------
    public override void ForceExecute(int frameId) {
        myTriggeredTransition= null;
        if(myQueueIdx < myTransitions.Count) {
            iCS_Transition transition= myTransitions[myQueueIdx];
            transition.ForceExecute(frameId);            
            if(transition.IsCurrent(frameId)) {
                if(transition.DidTrigger) {
                    myTriggeredTransition= transition;
                    ResetIterator(frameId);
                    return;
                }
                ++myQueueIdx;
                IsStalled= false;
            } else {
                // Verify if the child is a staled dispatcher.
                if(!transition.IsStalled) {
                    IsStalled= false;
                }
            }
        }
        if(myQueueIdx >= myTransitions.Count) {
            ResetIterator(frameId);
        }
    }
    // ----------------------------------------------------------------------
    void ResetIterator(int frameId) {
        myQueueIdx= 0;
        MarkAsCurrent(frameId);
    }
    
    // ======================================================================
    // Child management
    // ----------------------------------------------------------------------
    public void AddChild(iCS_Transition transition) {
        myTransitions.Add(transition);
    }
    public void RemoveChild(iCS_Transition transition) {
        myTransitions.Remove(transition);
    }
}
