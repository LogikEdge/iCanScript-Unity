using UnityEngine;
using System;

public class iCS_Package : iCS_ParallelDispatcher, iCS_ISignature {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    iCS_SignatureDataSource mySignature;
    int[]                   myEnablePorts= null;
    int                     myTriggerPort= -1;

    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_Package(iCS_Storage storage, int instanceId, int priority, int nbOfParameters= 0)
    : base(storage, instanceId, priority) {
        mySignature= new iCS_SignatureDataSource(nbOfParameters);
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
        if(myEnablePorts != null) {
            ResetTriggerPort();
            foreach(var trigger in myEnablePorts) {
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
        SetTriggerPort();
        base.Execute(frameId);
    }
    // -------------------------------------------------------------------------
    public override void ForceExecute(int frameId) {
        // Don't execute if enable port is configured and false.
        if(myEnablePorts != null) {
            ResetTriggerPort();
            foreach(var p in myEnablePorts) {
                // The enable port is ready.  Cancel execution if 'false'.
                var enabled= mySignature.FetchValue(p);
                if(enabled != null && !(bool)(enabled)) {
                    MarkAsCurrent(frameId);
                    return;
                }                
            }
        }
        SetTriggerPort();
        base.ForceExecute(frameId);
    }
    
    // =========================================================================
    // Control Ports Management
    // -------------------------------------------------------------------------
    public void ActivateEnablePort(int portIdx) {
        if(myEnablePorts == null) {
            myEnablePorts= new int[0];
        }
        int idx= myEnablePorts.Length;
        Array.Resize(ref myEnablePorts, idx+1);
        myEnablePorts[idx]= portIdx;
    }
    // -------------------------------------------------------------------------
    public void ActivateTriggerPort(int portIdx) {
        myTriggerPort= portIdx;
    }
    // -------------------------------------------------------------------------
    void ResetTriggerPort() {
        if(myTriggerPort == -1) return;
        mySignature.SetValue(myTriggerPort, false);
    }
    // -------------------------------------------------------------------------
    void SetTriggerPort() {
        if(myTriggerPort == -1) return;
        mySignature.SetValue(myTriggerPort, true);
    }
}
