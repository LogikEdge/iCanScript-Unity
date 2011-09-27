using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public sealed class WD_Module : WD_Function {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    private List<WD_Action> myExecuteQueue= new List<WD_Action>();
    private int             myQueueIdx= 0;
    private int             myNbOfTries= 0;
    

    // ======================================================================
    // EXECUTION
    // ----------------------------------------------------------------------
    protected override void Evaluate() {}
    public override void Execute() {
        // Attempt to execute child functions.
        int maxTries= myExecuteQueue.Count; maxTries= 1+(maxTries*maxTries+maxTries)/2;
        for(; myQueueIdx < myExecuteQueue.Count && myNbOfTries < maxTries; ++myNbOfTries) {
            WD_Action action= myExecuteQueue[myQueueIdx];
            action.Execute();            
            if(!action.IsCurrent()) {
                // The function is not ready to execute so lets delay the execution.
                myExecuteQueue.RemoveAt(myQueueIdx);
                myExecuteQueue.Add(action);
                return;
            }
            ++myQueueIdx;
        }
        // Verify that the graph is not looping.
        if(myNbOfTries >= maxTries) {
            Debug.LogError("Execution of graph is looping!!! "+myExecuteQueue[myQueueIdx].Name+":"+myExecuteQueue[myQueueIdx].TypeName+" is included in the loop. Please break the cycle and retry.");
        }
        // Reset iterators for next frame.
        myQueueIdx= 0;
        myNbOfTries= 0;
        MarkAsCurrent();
    }

    // ----------------------------------------------------------------------
    // Returns true if the object is an executable that we support.
    bool IsExecutable(WD_Object _object) {
        return _object is WD_Action;
    }
    

    // ======================================================================
    // CONNECTOR MANAGEMENT
    // ----------------------------------------------------------------------
    public override void AddChild(WD_Object obj) {
        if(IsExecutable(obj)) {
            myExecuteQueue.Add(obj as WD_Action);
        }
        base.AddChild(obj);
    }
    public override void RemoveChild(WD_Object obj) {
        if(IsExecutable(obj)) {
            myExecuteQueue.Remove(obj as WD_Action);
        }
        base.RemoveChild(obj);
    }
}
