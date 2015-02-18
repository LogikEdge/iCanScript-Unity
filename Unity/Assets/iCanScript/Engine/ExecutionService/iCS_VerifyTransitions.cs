using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Subspace;

public class iCS_VerifyTransitions : SSAction {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    List<iCS_Transition>  myTransitions        = new List<iCS_Transition>();
    int                   myQueueIdx           = 0;
    iCS_Transition        myTriggeredTransition= null;
    
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    public iCS_Transition TriggeredTransition { get { return myTriggeredTransition; }}
    
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_VerifyTransitions(string name, SSObject parent, int priority)
    : base(name, parent, priority) {}

    // ======================================================================
    // Execution
    // ----------------------------------------------------------------------
    public override void Evaluate() {
        if(!IsActive) return;
        IsStalled= true;
        myTriggeredTransition= null;
        var end= myTransitions.Count;
        for(int cursor= myQueueIdx; cursor < end; ++cursor) {
            // Attempt to execute child function.
            iCS_Transition transition= myTransitions[cursor];
            if(transition.IsEvaluated) {
                if(cursor == myQueueIdx) {
                    ++myQueueIdx;                    
                }
                continue;
            }
            transition.Evaluate();            
            // Move to next child if sucessfully executed.
            if(transition.IsEvaluated) {
                if(transition.DidTrigger) {
                    myTriggeredTransition= transition;
                    ResetIterator();
                    return;
                }
                IsStalled= false;
                if(cursor == myQueueIdx) {
                    ++myQueueIdx;                    
                }
                continue;
            }
            // Verify if the child is a stalled dispatcher.
            IsStalled&= transition.IsStalled;
        }
        // Reset iterators for next frame.
        if(myQueueIdx >= end) {
            ResetIterator();            
        }
    }
    // ----------------------------------------------------------------------
    public override SSConnection GetStalledProducerPort() {
        for(int cursor= myQueueIdx; cursor < myTransitions.Count; ++cursor) {
            iCS_Transition transition= myTransitions[cursor];
            if(!transition.IsEvaluated) {
                var result= transition.GetStalledProducerPort();
                if(result != null) {
                    return result;
                }
            }
        }
        return null;        
    }
    // ----------------------------------------------------------------------
    public override void Execute() {
        myTriggeredTransition= null;
        if(myQueueIdx < myTransitions.Count) {
            iCS_Transition transition= myTransitions[myQueueIdx];
            transition.Execute();            
            if(transition.IsEvaluated) {
                if(transition.DidTrigger) {
                    myTriggeredTransition= transition;
                    ResetIterator();
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
            ResetIterator();
        }
    }
    // ----------------------------------------------------------------------
    void ResetIterator() {
        myQueueIdx= 0;
        MarkAsExecuted();
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
