using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class iCS_Dispatcher : iCS_ActionWithSignature {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    protected List<iCS_Action> myExecuteQueue= new List<iCS_Action>();
    protected int              myQueueIdx = 0;
    
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_Dispatcher(iCS_VisualScriptImp visualScript, int priority, int nbOfParameters, int nbOfEnables)
    : base(visualScript, priority, nbOfParameters, nbOfEnables) {}

    // ======================================================================
    // Execution
    // ----------------------------------------------------------------------
    public override iCS_Connection GetStalledProducerPort(int frameId) {
        int cursor= myQueueIdx;
        if(cursor < myExecuteQueue.Count) {
            iCS_Action action= myExecuteQueue[myQueueIdx];
            if(!action.IsCurrent(frameId)) {
                var producerPort= action.GetStalledProducerPort(frameId);
                if(producerPort != null) {
                    return producerPort;
                }
            }
        }
        return null;
    }

    // ----------------------------------------------------------------------
    protected override void DoForceExecute(int frameId) {
        if(myQueueIdx < myExecuteQueue.Count) {
            iCS_Action action= myExecuteQueue[myQueueIdx];
            action.ForceExecute(frameId);            
            if(action.IsCurrent(frameId)) {
                ++myQueueIdx;
                IsStalled= false;
            } else {
                IsStalled &= action.IsStalled;
            }
        }
        if(myQueueIdx >= myExecuteQueue.Count) {
            ResetIterator(frameId);
        }
    }
    // ----------------------------------------------------------------------
    protected void Swap(int idx1, int idx2) {
        var tmp= myExecuteQueue[idx1];
        myExecuteQueue[idx1]= myExecuteQueue[idx2];
        myExecuteQueue[idx2]= tmp;
    }
    // ----------------------------------------------------------------------
    protected void ResetIterator(int frameId) {
        myQueueIdx= 0;
        MarkAsExecuted(frameId);        
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
