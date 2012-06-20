using UnityEngine;
using System;
using System.Reflection;
using System.Collections;

public class iCS_Function : iCS_FunctionBase {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    protected object        myReturn      = null;
    protected MethodBase    myMethodInfo  = null;

    // ======================================================================
    // Accessors
    // ----------------------------------------------------------------------
    protected override object DoGetParameter(int idx) {
        return idx == myParameters.Length ? myReturn : base.DoGetParameter(idx);
    }
    protected override void DoSetParameter(int idx, object value) {
        if(idx == myParameters.Length) { myReturn= value; return; }
        base.DoSetParameter(idx, value);
    }
    protected override bool DoIsParameterReady(int idx, int frameId) {
        return idx == myParameters.Length ? IsCurrent(frameId) : base.DoIsParameterReady(idx, frameId);
    }

    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_Function(string name, MethodBase methodInfo, bool[] paramIsOuts, Vector2 layout) : base(name, paramIsOuts, layout) {
        myMethodInfo= methodInfo;
    }
    public new void SetParameterConnection(int id, iCS_Connection connection) {
        if(id < myParameters.Length) base.SetParameterConnection(id, connection);
    }
    
    // ======================================================================
    // Execution
    // ----------------------------------------------------------------------
    protected override void DoExecute(int frameId) {
        // Execute function
        myReturn= myMethodInfo.Invoke(null, myParameters);            
        MarkAsCurrent(frameId);
    }
}
