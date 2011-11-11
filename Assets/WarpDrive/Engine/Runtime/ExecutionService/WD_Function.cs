using UnityEngine;
using System;
using System.Reflection;
using System.Collections;

public class WD_Function : WD_FunctionBase {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    object          myTargetObject= null;
    MethodInfo      myMethodInfo  = null;

    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public WD_Function(string name, MethodInfo methodInfo, object[] parameters, bool[] paramIsOuts) : base(name, parameters, paramIsOuts) {
        myMethodInfo= methodInfo;
    }
    
    // ======================================================================
    // Execution
    // ----------------------------------------------------------------------
    protected override void DoExecute(int frameId) {
        // Execute function
        myReturn= myMethodInfo.Invoke(myTargetObject, myParameters);
        MarkAsCurrent(frameId);
    }
}
