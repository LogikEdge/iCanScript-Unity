using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Subspace;

public abstract class iCS_Dispatcher : iCS_ActionWithSignature {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    protected List<SSAction> myExecuteQueue= new List<SSAction>();
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
        if(IsCurrent(frameId)) {
            return null;
        }
        var producerPort= mySignature.GetStalledProducerPort(frameId, /*enablesOnly=*/true);
        if(producerPort != null) {
            return producerPort;
        }
        int cursor= myQueueIdx;
        if(cursor < myExecuteQueue.Count) {
            SSAction action= myExecuteQueue[myQueueIdx];
            if(!action.IsCurrent(frameId)) {
                producerPort= action.GetStalledProducerPort(frameId);
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
            SSAction action= myExecuteQueue[myQueueIdx];
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
    public void AddChild(SSAction action) {
        myExecuteQueue.Add(action);
        action.Parent= this;
    }
    public void RemoveChild(SSAction action) {
        myExecuteQueue.Remove(action);
        action.Parent= null;
    }
}
