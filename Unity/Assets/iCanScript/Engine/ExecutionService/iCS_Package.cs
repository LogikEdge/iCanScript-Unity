using UnityEngine;

public class iCS_Package : iCS_ParallelDispatcher, iCS_ISignature {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    iCS_SignatureDataSource mySignature;
    int                     myEnablePort= -1;

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

    // =========================================================================
    // Process enable and control output ports
    // -------------------------------------------------------------------------
    public override void Execute(int frameId) {
        // Don't execute if enable port is configured and false.
        if(myEnablePort >= 0) {
            // Wait for enable source to run
            if(!mySignature.IsReady(myEnablePort, frameId)) return;
            // The enable port is ready.  Cancel execution if 'false'.
            if(!(bool)(mySignature.FetchValue(myEnablePort))) {
                return;
            }
        }
        base.Execute(frameId);
    }
    
    // =========================================================================
    // -------------------------------------------------------------------------
    public void ActivateEnablePort(int portIdx) {
        Debug.Log("Setting enable port");
        myEnablePort= portIdx;
    }
    public void ActivateControlOutputPort(int portIdx) {
        Debug.Log("Setting control output port");
    }
}
