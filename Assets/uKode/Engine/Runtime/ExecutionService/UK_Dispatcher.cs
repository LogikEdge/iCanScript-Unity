using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class UK_Dispatcher : UK_Action, UK_IDispatcher {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    protected List<UK_IAction> myExecuteQueue= new List<UK_IAction>();
    protected int              myQueueIdx = 0;
    protected bool             myIsStalled= false;
    
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    public bool IsStalled { get { return myIsStalled; }}
    
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
                myIsStalled= false;
            } else {
                // Verify if the child is a staled dispatcher.
                UK_IDispatcher childDispatcher= action as UK_IDispatcher;
                if(childDispatcher != null && !childDispatcher.IsStalled) {
                    myIsStalled= false;
                }
            }
        }
        if(myQueueIdx >= myExecuteQueue.Count) {
            ResetIterator(frameId);
        }
    }
    // ----------------------------------------------------------------------
    protected void ResetIterator(int frameId) {
        myIsStalled= false;
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
