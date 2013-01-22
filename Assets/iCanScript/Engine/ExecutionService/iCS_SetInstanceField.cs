using UnityEngine;
using System;
using System.Reflection;
using System.Collections;

public class iCS_SetInstanceField : iCS_SetStaticField {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    object          myThis          = null;
    iCS_Connection  myThisConnection= null;

    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_SetInstanceField(string name, FieldInfo fieldInfo, bool[] paramIsOuts, int priority) : base(name, fieldInfo, paramIsOuts, priority) {
        myThisConnection= iCS_Connection.NoConnection;
    }
    public new void SetParameterConnection(int id, iCS_Connection connection) {
        if(id == 2) myThisConnection= connection;
        else base.SetParameterConnection(id, connection);
    }
    
    // ======================================================================
    // Accessors
    // ----------------------------------------------------------------------
    protected override object DoGetParameter(int idx) {
        return (idx == 2 || idx == 3) ? myThis : base.DoGetParameter(idx); 
    }
    protected override void DoSetParameter(int idx, object value) {
        if(idx == 3) return;
        if(idx == 2) { myThis= value; return; }
        base.DoSetParameter(idx, value);
    }
    protected override bool DoIsParameterReady(int idx, int frameId) {
        if(idx == 3) return IsCurrent(frameId);
        if(idx == 2) {
            return !myThisConnection.IsConnected ? true : myThisConnection.IsReady(frameId);
        }
        return base.DoIsParameterReady(idx, frameId);
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
        myFieldInfo.SetValue(myThis, myParameters[0]);
        MarkAsCurrent(frameId);
    }
}
