using UnityEngine;
using System.Collections;

/// =========================================================================
// A function is an action that has input data depednecies.  Therefore, a
// function waits for the input data before executing.
[System.Serializable]
public abstract class WD_Function : WD_Action {
    // ======================================================================
    // EXECUTION
    // ----------------------------------------------------------------------
    public override void Execute() {
        if(!IsReady()) return;
        // All verification have passed so let's execute !!!
        ForEachChild<WD_DataPort>( (port)=> { if(port.IsInput) port.UpdateValue(); });
        base.Execute();
    }
    
    // ----------------------------------------------------------------------
    protected virtual bool IsReady() {
        bool isReady= true;
        ForEachChild<WD_InDataPort>(
            (port)=> {
                WD_Port producerPort= port.GetProducerPort();
                if(producerPort != port) {
                    WD_Action producer= producerPort.Parent as WD_Action;
                    if(!producer.IsCurrent()) {
                        isReady= false;
                    }
                }                            
            }
        );
        return isReady;
    }
}
