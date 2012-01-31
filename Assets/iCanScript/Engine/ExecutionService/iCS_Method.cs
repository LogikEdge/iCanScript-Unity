using UnityEngine;
using System;
using System.Reflection;
using System.Collections;

public class iCS_Method : iCS_Function {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    object          myThis          = null;
    iCS_Connection  myThisConnection= null;

    // ======================================================================
    // Accessors
    // ----------------------------------------------------------------------
    protected override object DoGetParameter(int idx) {
        return (idx == myParameters.Length+1 || idx == myParameters.Length+2) ? myThis : base.DoGetParameter(idx); 
    }
    protected override void DoSetParameter(int idx, object value) {
        if(idx == myParameters.Length+2) return;
        if(idx == myParameters.Length+1) { myThis= value; return; }
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
    public iCS_Method(string name, MethodBase methodInfo, bool[] paramIsOuts, Vector2 layout) : base(name, methodInfo, paramIsOuts, layout) {
        myThisConnection= iCS_Connection.NoConnection;
    }
    public new void SetConnection(int id, iCS_Connection connection) {
        if(id == myParameters.Length+1) myThisConnection= connection;
        else base.SetConnection(id, connection);
    }
    
    // ======================================================================
    // Execution
    // ----------------------------------------------------------------------
    public override void Execute(int frameId) {
        // Validate that this is ready.
        if(myThisConnection.IsConnected && !myThisConnection.IsReady(frameId)) {
            IsStalled= true;
            return;
        }
        base.Execute(frameId);        
    }
    // ----------------------------------------------------------------------
    protected override void DoExecute(int frameId) {
        // Fetch this.
        if(myThisConnection.IsConnected) {
            myThis= myThisConnection.Value;
        }
        // Execute function
        if(myThis != null) myReturn= myMethodInfo.Invoke(myThis, myParameters);
        MarkAsCurrent(frameId);        
    }
}
