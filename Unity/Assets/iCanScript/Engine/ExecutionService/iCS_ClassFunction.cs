using UnityEngine;
using System;
using System.Reflection;
using System.Collections;

public class iCS_ClassFunction : iCS_FunctionBase {
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_ClassFunction(MethodBase methodBase, string name, int priority, int nbOfParameters)
    : base(methodBase, name, priority, nbOfParameters, true, false) {}

    // ======================================================================
    // Execution
    // ----------------------------------------------------------------------
    protected override void DoExecute(int frameId) {
        // Execute function
        ReturnValue= myMethodBase.Invoke(null, Parameters);            
        MarkAsCurrent(frameId);
    }
}
