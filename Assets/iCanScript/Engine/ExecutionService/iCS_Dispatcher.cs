using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class iCS_Dispatcher : iCS_Action {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    protected List<iCS_Action> myExecuteQueue= new List<iCS_Action>();
    protected int              myQueueIdx = 0;
    
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_Dispatcher(string name, Vector2 layout) : base(name, layout) {}

    // ======================================================================
    // Execution
    // ----------------------------------------------------------------------
    public override void ForceExecute(int frameId) {
        if(myQueueIdx < myExecuteQueue.Count) {
            iCS_Action action= myExecuteQueue[myQueueIdx];
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
    public void AddChild(iCS_Action action) {
        myExecuteQueue.Add(action);
    }
    public void RemoveChild(iCS_Action action) {
        myExecuteQueue.Remove(action);
    }
}
