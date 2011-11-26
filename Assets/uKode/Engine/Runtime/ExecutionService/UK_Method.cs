using UnityEngine;
using System;
using System.Reflection;
using System.Collections;

public class UK_Method : UK_Function {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    object          myThis          = null;
    UK_Connection   myThisConnection= null;

    // ======================================================================
    // Accessors
    // ----------------------------------------------------------------------
    protected override object DoGetParameter(int idx) {
        return (idx == myParameters.Length+1 || idx == myParameters.Length+2) ? myThis : base.DoGetParameter(idx); 
    }
    protected override void DoSetParameter(int idx, object value) {
        if(idx == myParameters.Length+1 || idx == myParameters.Length+2) { myThis= value; return; }
        base.DoSetParameter(idx, value);
    }
    protected override bool DoIsParameterReady(int idx, int frameId) {
        if(idx == myParameters.Length+2) return IsCurrent(frameId);
        if(idx == myParameters.Length+1) {
            return !myThisConnection.IsConnected ? true : myThisConnection.IsReady(frameId);
        }
        return base.DoIsParameterReady(idx, frameId);
    }

    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public UK_Method(string name, MethodInfo methodInfo, object[] parameters, bool[] paramIsOuts) : base(name, methodInfo, parameters, paramIsOuts) {
    }
    public void SetConnections(UK_Connection thisConnection, UK_Connection[] connections) {
        myThisConnection= thisConnection;
        base.SetConnections(connections);
    }
    
    // ======================================================================
    // Execution
    // ----------------------------------------------------------------------
    protected override void DoExecute(int frameId) {
        // Validate that this is ready.
        if(myThisConnection.IsConnected && !myThisConnection.IsReady(frameId)) return;
        // Fetch this.
        if(myThisConnection.IsConnected) {
            myThis= myThisConnection.Value;
        }
        // Execute function
        if(myThis != null) myReturn= myMethodInfo.Invoke(myThis, myParameters);
        MarkAsCurrent(frameId);
    }
}
