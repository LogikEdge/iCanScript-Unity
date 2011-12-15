using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UK_VerifyTransitions : UK_Action {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    List<UK_Transition>  myTransitions        = new List<UK_Transition>();
    int                  myQueueIdx           = 0;
    UK_Transition        myTriggeredTransition= null;
    
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    public UK_Transition TriggeredTransition { get { return myTriggeredTransition; }}
    
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public UK_VerifyTransitions(string name, Vector2 layout) : base(name, layout) {}

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
            UK_Transition transition= myTransitions[myQueueIdx];
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
            UK_Transition transition= myTransitions[myQueueIdx];
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
    public void AddChild(UK_Transition transition) {
        myTransitions.Add(transition);
    }
    public void RemoveChild(UK_Transition transition) {
        myTransitions.Remove(transition);
    }
}
