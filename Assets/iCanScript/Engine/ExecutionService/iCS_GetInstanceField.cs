using UnityEngine;
using System;
using System.Reflection;
using System.Collections;

public class iCS_GetInstanceField : iCS_GetStaticField {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    object          myThis          = null;
    iCS_Connection  myThisConnection= null;

    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_GetInstanceField(string name, FieldInfo fieldInfo, bool[] paramIsOuts, Vector2 layout) : base(name, fieldInfo, paramIsOuts, layout) {
        myThisConnection= iCS_Connection.NoConnection;
    }
    public new void SetConnection(int id, iCS_Connection connection) {
        if(id == 1) myThisConnection= connection;
        else base.SetConnection(id, connection);
    }

    // ======================================================================
    // Accessors
    // ----------------------------------------------------------------------
    protected override object DoGetParameter(int idx) {
        return (idx == 1 || idx == 2) ? myThis : base.DoGetParameter(idx); 
    }
    protected override void DoSetParameter(int idx, object value) {
        if(idx == 2) return;
        if(idx == 1) { myThis= value; return; }
        base.DoSetParameter(idx, value);
    }
    protected override bool DoIsParameterReady(int idx, int frameId) {
        if(idx == 2) return IsCurrent(frameId);
        if(idx == 1) {
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
        myReturn= myFieldInfo.GetValue(myThis);
        MarkAsCurrent(frameId);
    }
}
