using UnityEngine;
using System.Collections;

public abstract class AP_Function : AP_Action {
    // ======================================================================
    // EXECUTION
    // ----------------------------------------------------------------------
    protected abstract void doExecute();
    public override void Execute() {
        // All verification have passed so let's execute !!!
        ForEachChild<AP_DataPort>( (port)=> { if(port.IsInput) port.UpdateValue(); });
        doExecute();            
        MarkAsCurrent();
    }
    
    // ----------------------------------------------------------------------
//    public bool IsReady { get { return doIsReady(); }}
//    public virtual bool doIsReady() {
//        bool isReady= true;
//        ForEachChild<AP_Port>(
//            (port)=> {
//                if(port.IsInput) {
//                    AP_Port producerPort= port.GetProducerPort();
//                    if(producerPort != port) {
//                        AP_Function producer= producerPort.Parent as AP_Function;
//                        if(!producer.IsCurrent()) {
//                            isReady= false;
//                        }
//                    }                            
//                }
//            }
//        );
//        return isReady;
//    }
}
