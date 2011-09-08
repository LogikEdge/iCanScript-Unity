using UnityEngine;
using System.Collections;

/// =========================================================================
// A function is an action that has input data depednecies.  Therefore, a
// function waits for the input data before executing.
public abstract class AP_Function : AP_Action {
    // ======================================================================
    // EXECUTION
    // ----------------------------------------------------------------------
    public override void Execute() {
        if(!IsReady()) return;
        // All verification have passed so let's execute !!!
        ForEachChild<AP_DataPort>( (port)=> { if(port.IsInput) port.UpdateValue(); });
        base.Execute();
    }
    
    // ----------------------------------------------------------------------
    protected virtual bool IsReady() {
        bool isReady= true;
        ForEachChild<AP_InDataPort>(
            (port)=> {
                AP_Port producerPort= port.GetProducerPort();
                if(producerPort != port) {
                    AP_Action producer= producerPort.Parent as AP_Action;
                    if(!producer.IsCurrent()) {
                        isReady= false;
                    }
                }                            
            }
        );
        return isReady;
    }
}
