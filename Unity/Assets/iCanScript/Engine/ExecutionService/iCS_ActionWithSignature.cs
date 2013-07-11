using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

public class iCS_ActionWithSignature : iCS_Action, iCS_ISignature {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    iCS_SignatureDataSource mySignature= null;
    
    // ======================================================================
    // Accessors
    // ----------------------------------------------------------------------
    public object[] Parameters {
        get { return mySignature.Parameters; }
        set { mySignature.Parameters= value; }
    }
    public iCS_Connection[] Connections {
        get { return mySignature.Connections; }
    }
    public object ReturnValue {
        get { return mySignature.ReturnValue; }
        set { mySignature.ReturnValue= value; }
    }
    public object This {
        get { return mySignature.This; }
        set { mySignature.This= value; }
    }
    public object this[int idx] {
        get { return mySignature[idx]; }
        set { mySignature[idx]= value; }
    }
    public void SetConnection(int idx, iCS_Connection connection) {
        mySignature.SetConnection(idx, connection);
    }
    
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_ActionWithSignature(iCS_Storage storage, int instanceId, int priority, int nbOfParameters, bool hasReturn, bool hasThis) : base(storage, instanceId, priority) {
        mySignature= new iCS_SignatureDataSource(nbOfParameters, hasReturn, hasThis);
    }
    
    // ======================================================================
    // Implement ISignature delegate.
    // ----------------------------------------------------------------------
    public iCS_SignatureDataSource GetSignatureDataSource() {
        return mySignature;
    }
    
    // ======================================================================
    // Execution
    // ----------------------------------------------------------------------
    public override void Execute(int frameId) {
        // Verify that we are ready to run.
        if(!mySignature.AreAllConnectionsReady(frameId)) {
            IsStalled= true;
            return;            
        }
        ForceExecute(frameId);
    }
    // ----------------------------------------------------------------------
    public override void ForceExecute(int frameId) {
        // Fetch all the inputs.
        mySignature.ForcedFetchConnections();
        // Execute function
        DoExecute(frameId);
    }
    // ----------------------------------------------------------------------
    protected virtual void DoExecute(int frameId) {}
}
