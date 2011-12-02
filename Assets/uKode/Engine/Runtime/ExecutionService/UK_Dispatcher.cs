using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class UK_Dispatcher : UK_Action {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    protected List<UK_IAction> myExecuteQueue= new List<UK_IAction>();
    protected int              myQueueIdx = 0;
    
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public UK_Dispatcher(string name) : base(name) {}

    // ======================================================================
    // Execution
    // ----------------------------------------------------------------------
    public override void ForceExecute(int frameId) {
        if(myQueueIdx < myExecuteQueue.Count) {
            UK_IAction action= myExecuteQueue[myQueueIdx];
            action.ForceExecute(frameId);            
            if(action.IsCurrent(frameId)) {
                ++myQueueIdx;
                IsStalled= false;
            } else {
                // Verify if the child is a staled dispatcher.
                if(!action.IsStalled) {
                    IsStalled= false;
                }
            }
        }
        if(myQueueIdx >= myExecuteQueue.Count) {
            ResetIterator(frameId);
        }
    }
    // ----------------------------------------------------------------------
    protected void ResetIterator(int frameId) {
        myQueueIdx= 0;
        MarkAsCurrent(frameId);        
    }

    // ======================================================================
    // Queue Management
    // ----------------------------------------------------------------------
    public void AddChild(UK_IAction action) {
        myExecuteQueue.Add(action);
    }
    public void RemoveChild(UK_IAction action) {
        myExecuteQueue.Remove(action);
    }
}
