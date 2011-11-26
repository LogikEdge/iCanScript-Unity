using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WD_Module : WD_FunctionBase {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    List<WD_Action> myExecuteQueue= new List<WD_Action>();
    int             myQueueIdx = 0;
    int             myNbOfTries= 0;
    
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public WD_Module(string name, object[] parameters, bool[] paramIsOuts) : base(name, parameters, paramIsOuts) {}
    
    // ======================================================================
    // Execution
    // ----------------------------------------------------------------------
    protected override void DoExecute(int frameId) {
        // Attempt to execute child functions.
        int maxTries= myExecuteQueue.Count; maxTries= 1+(maxTries*maxTries+maxTries)/2;
        for(; myQueueIdx < myExecuteQueue.Count && myNbOfTries < maxTries; ++myNbOfTries) {
            WD_Action action= myExecuteQueue[myQueueIdx];
            action.Execute(frameId);            
            if(!action.IsCurrent(frameId)) {
                // The function is not ready to execute so lets delay the execution.
                myExecuteQueue.RemoveAt(myQueueIdx);
                myExecuteQueue.Add(action);
                return;
            }
            ++myQueueIdx;
        }
        // Verify that the graph is not looping.
        if(myNbOfTries >= maxTries) {
            Debug.LogError("Execution of graph is looping!!! "+myExecuteQueue[myQueueIdx].Name+":"+myExecuteQueue[myQueueIdx].GetType().Name+" is included in the loop. Please break the cycle and retry.");
        }
        // Update all outputs.
        foreach(var id in myOutIndexes) {
            if(myConnections[id].IsConnected) {
                myParameters[id]= myConnections[id].Value;
            }
        }        
        // Reset iterators for next frame.
        myQueueIdx= 0;
        myNbOfTries= 0;
        MarkAsCurrent(frameId);
    }


    // ======================================================================
    // Connector Management
    // ----------------------------------------------------------------------
    public void AddChild(object obj) {
        if(IsExecutable(obj)) {
            myExecuteQueue.Add(obj as WD_Action);
        }
    }
    public void RemoveChild(object obj) {
        if(IsExecutable(obj)) {
            myExecuteQueue.Remove(obj as WD_Action);
        }
    }
    // ----------------------------------------------------------------------
    // Returns true if the object is an executable that we support.
    bool IsExecutable(object _object) {
        return _object is WD_Action;
    }
    
}
