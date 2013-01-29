using UnityEngine;
using System;
using System.Reflection;
using System.Collections;

public class iCS_Constructor : iCS_FunctionBase {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    protected object        myThis      = null;
    protected MethodBase    myMethodBase= null;

    // ======================================================================
    // Accessors
    // ----------------------------------------------------------------------
    protected override object DoGetParameter(int idx) {
        return idx == myParameters.Length ? myThis : base.DoGetParameter(idx);
    }
    protected override void DoSetParameter(int idx, object value) {
        if(idx == myParameters.Length) { myThis= value; return; }
        base.DoSetParameter(idx, value);
    }
    protected override bool DoIsParameterReady(int idx, int frameId) {
        return idx == myParameters.Length ? IsCurrent(frameId) : base.DoIsParameterReady(idx, frameId);
    }

    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_Constructor(string name, MethodBase methodBase, bool[] paramIsOuts, int priority) : base(name, paramIsOuts, priority) {
        myMethodBase= methodBase;
    }
    public new void SetParameterConnection(int id, iCS_Connection connection) {
        if(id < myParameters.Length) base.SetParameterConnection(id, connection);
    }
    
    // ======================================================================
    // Execution
    // ----------------------------------------------------------------------
    protected override void DoExecute(int frameId) {
        // Execute function
        if(myThis == null) {
            myThis= myMethodBase.Invoke(null, myParameters);            
        }
        MarkAsCurrent(frameId);
    }
}
