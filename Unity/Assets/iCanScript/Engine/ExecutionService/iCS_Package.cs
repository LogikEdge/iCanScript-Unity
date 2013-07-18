using UnityEngine;
using System;

public class iCS_Package : iCS_ParallelDispatcher, iCS_ISignature {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    iCS_SignatureDataSource mySignature;
    int[]                   myInTriggerPorts= null;
    int                     myOutTriggerPort= -1;

    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_Package(iCS_Storage storage, int instanceId, int priority, int nbOfParameters= 0)
    : base(storage, instanceId, priority) {
        mySignature= new iCS_SignatureDataSource(nbOfParameters, false, true);
    }

    // ======================================================================
    // Accessors
    // ----------------------------------------------------------------------
    public object this[int idx] {
        get { return mySignature.GetValue(idx); }
        set { mySignature.SetValue(idx, value); }
    }
    // ======================================================================
    // Signature implementation
    // An aggregate only support value input parameters.  All other types of
    // parameters are ignored.
    // ----------------------------------------------------------------------
    public iCS_SignatureDataSource GetSignatureDataSource() {
        return mySignature;
    }
    public iCS_Action GetAction() {
        return this;
    }
    public void SetConnection(int idx, iCS_Connection connection) {
        mySignature.SetConnection(idx, connection);
    }
    

    // =========================================================================
    // Process enable and control output ports
    // -------------------------------------------------------------------------
    public override void Execute(int frameId) {
        // Don't execute if enable port is configured and false.
        if(myInTriggerPorts != null) {
            foreach(var trigger in myInTriggerPorts) {
                // Wait for enable source to run
                if(!mySignature.IsReady(trigger, frameId)) return;
                // The enable port is ready.  Cancel execution if 'false'.
                var enabled= mySignature.FetchValue(trigger);
                if(enabled != null && !(bool)(enabled)) {
                    MarkAsCurrent(frameId);
                    return;
                }                
            }
        }
        base.Execute(frameId);
    }
    // -------------------------------------------------------------------------
    public override void ForceExecute(int frameId) {
        // Don't execute if enable port is configured and false.
        if(myInTriggerPorts != null) {
            foreach(var trigger in myInTriggerPorts) {
                // The enable port is ready.  Cancel execution if 'false'.
                var enabled= mySignature.FetchValue(trigger);
                if(enabled != null && !(bool)(enabled)) {
                    MarkAsCurrent(frameId);
                    return;
                }                
            }
        }
        base.ForceExecute(frameId);
    }
    
    // =========================================================================
    // -------------------------------------------------------------------------
    public void ActivateEnablePort(int portIdx) {
        if(myInTriggerPorts == null) {
            myInTriggerPorts= new int[0];
        }
        int idx= myInTriggerPorts.Length;
        Array.Resize(ref myInTriggerPorts, idx+1);
        myInTriggerPorts[idx]= portIdx;
    }
    public void ActivateOutTriggerPort(int portIdx) {
        myOutTriggerPort= portIdx;
        Debug.Log("Setting out trigger port");
    }
}
