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
    public iCS_SetInstanceField(string name, FieldInfo fieldInfo, bool[] paramIsOuts, Vector2 layout) : base(name, fieldInfo, paramIsOuts, layout) {
        myThisConnection= iCS_Connection.NoConnection;
    }
    public new void SetConnection(int id, iCS_Connection connection) {
        if(id == 2) myThisConnection= connection;
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
        myFieldInfo.SetValue(myThis, myParameters[0]);
        MarkAsCurrent(frameId);
    }
}
