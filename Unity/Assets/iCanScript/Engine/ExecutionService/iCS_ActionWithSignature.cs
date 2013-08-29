using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

public abstract class iCS_ActionWithSignature : iCS_Action, iCS_ISignature {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    iCS_SignatureDataSource mySignature = null;
    
    // ======================================================================
    // Accessors
    // ----------------------------------------------------------------------
    public object This {
        get { return mySignature.This; }
        set { mySignature.This= value; }
    }
    public object OutThis {
        get { return mySignature.OutThis; }
        set { mySignature.OutThis= value; }
    }
    public bool Trigger {
        get { return mySignature.Trigger; }
        set { mySignature.Trigger= value; }
    }
    public object GetValue(int idx) {
        return mySignature.GetValue(idx);
    }
    public void SetValue(int idx, object value) {
        mySignature.SetValue(idx, value);
    }
    public object ReturnValue {
        get { return mySignature.ReturnValue; }
        set { mySignature.ReturnValue= value; }
    }
    public void SetConnection(int idx, iCS_Connection connection) {
        mySignature.SetConnection(idx, connection);
    }
    public object this[int idx] {
        get { return mySignature[idx]; }
        set { mySignature[idx]= value; }
    }
    public object[] Parameters {
        get { return mySignature.Parameters; }
    }
    public iCS_Connection[] ParameterConnections {
        get { return mySignature.ParameterConnections; }
    }
    public int ParametersStart  { get { return mySignature.ParametersStart; }}
    public int ParametersEnd    { get { return mySignature.ParametersEnd; }}
    public bool IsParameterReady(int idx, int frameId) {
        return mySignature.IsParameterReady(idx, frameId);
    }
    public void UpdateParameter(int idx) {
        mySignature.UpdateParameter(idx);
    }
    public bool IsThisReady(int frameId) {
        return mySignature.IsThisReady(frameId);
    }
    
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_ActionWithSignature(iCS_Storage storage, int instanceId, int priority, int nbOfParameters, int nbOfEnables)
    : base(storage, instanceId, priority) {
        mySignature= new iCS_SignatureDataSource(nbOfParameters, nbOfEnables);
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
        // Clear thge output trigger flag.
        mySignature.Trigger= false;
        // Verify that the action is enabled.
        bool isEnabled;
        if(mySignature.GetIsEnabledIfReady(frameId, out isEnabled)) {
            if(isEnabled == false) {
                MarkAsCurrent(frameId);
                return;
            }
        }
        // Invoke derived class to execute normally.
        DoExecute(frameId);
    }
    // ----------------------------------------------------------------------
    public override void ForceExecute(int frameId) {
        // Force verify enables.
        if(mySignature.GetIsEnabled() == false) {
            MarkAsCurrent(frameId);
            return;
        }
        // Invoke derived class to force execute.
        DoForceExecute(frameId);
    }
    // ----------------------------------------------------------------------
    // Override the execute marker to set the output trigger.
    public new void MarkAsExecuted(int frameId) {
        mySignature.OutThis= mySignature.This;
        mySignature.Trigger= true;
        base.MarkAsExecuted(frameId);
    }
    // =========================================================================
    // Functions to override to provide specific behaviours.
    // ----------------------------------------------------------------------
    protected abstract void DoExecute(int frameId);
    protected abstract void DoForceExecute(int frameId);
}
