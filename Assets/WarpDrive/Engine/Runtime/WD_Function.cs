using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// =========================================================================
// A function is an action that has input data depednecies.  Therefore, a
// function waits for the input data before executing.
[System.Serializable]
public abstract class WD_Function : WD_Action {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    public List<WD_FunctionPort>   InPorts= new List<WD_FunctionPort>();
    
    // ======================================================================
    // EXECUTION
    // ----------------------------------------------------------------------
    public override void Execute() {
        if(!IsReady()) return;
        // All verification have passed so let's execute !!!
        foreach(var port in InPorts) { port.UpdateValue(); }
        base.Execute();
    }
    
    // ----------------------------------------------------------------------
    protected virtual bool IsReady() {
        bool isReady= true;
        foreach(var port in InPorts) {
            WD_Port producerPort= port.GetProducerPort();
            if(producerPort != port) {
                WD_Action producer= producerPort.Parent as WD_Action;
                if(!producer.IsCurrent()) {
                    isReady= false;
                }
            }                                        
        }
        return isReady;
    }

    // ----------------------------------------------------------------------
    public override void AddChild(WD_Object obj)     {
        obj.ExecuteIf<WD_InFunctionPort>((port) => { InPorts.Add(port); });
    }
    public override void RemoveChild(WD_Object obj)  {
        obj.ExecuteIf<WD_InFunctionPort>((port) => { InPorts.Remove(port); });
    }

}
