using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

public class iCS_ActionWithSignature : iCS_Action, iCS_ISignature {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    bool                    myIsDisabled= false;
    iCS_SignatureDataSource mySignature = null;
    
    // ======================================================================
    // Accessors
    // ----------------------------------------------------------------------
    public bool IsDisabled { get { return myIsDisabled; } set { myIsDisabled= value; }}
    public object[] Parameters {
        get { return mySignature.Parameters; }
    }
    public iCS_Connection[] ParameterConnections {
        get { return mySignature.ParameterConnections; }
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
    public object GetValue(int idx) {
        return mySignature.GetValue(idx);
    }
    public void SetValue(int idx, object value) {
        mySignature.SetValue(idx, value);
    }
    public iCS_Connection GetConnection(int idx) {
        return mySignature.GetConnection(idx);
    }
    public void SetConnection(int idx, iCS_Connection connection) {
        mySignature.SetConnection(idx, connection);
    }
    public bool ForEachParameterConnection(Func<int,iCS_Connection,bool> test) {
        return mySignature.ForEachParameterConnection(test);
    }
    public bool ForEachConnection(Func<int,iCS_Connection,bool> test) {
        return mySignature.ForEachConnection(test);
    }
    
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_ActionWithSignature(iCS_Storage storage, int instanceId, int priority, int nbOfParameters) : base(storage, instanceId, priority) {
        mySignature= new iCS_SignatureDataSource(nbOfParameters);
    }
    
    // ======================================================================
    // Implement ISignature delegate.
    // ----------------------------------------------------------------------
    public iCS_SignatureDataSource GetSignatureDataSource() { return mySignature; }
    public iCS_Action GetAction() { return this; }
    
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
