using UnityEngine;
using System;
using System.Reflection;
using System.Collections;

public class UK_Function : UK_FunctionBase {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    protected object        myReturn      = null;
    protected MethodInfo    myMethodInfo  = null;

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
    public UK_Function(string name, MethodInfo methodInfo, object[] parameters, bool[] paramIsOuts) : base(name, parameters, paramIsOuts) {
        myMethodInfo= methodInfo;
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
