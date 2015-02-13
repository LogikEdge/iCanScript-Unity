using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using Subspace;

public class iCS_InstanceFunction : iCS_ClassFunction {
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_InstanceFunction(string name, SSObject parent, MethodBase methodBase, SSContext context, int priority, int nbOfParameters, int nbOfEnables)
    : base(name, parent, methodBase, context, priority, nbOfParameters, nbOfEnables) {
    }
    
    // ======================================================================
    // Execution
    // ----------------------------------------------------------------------
    protected override void DoExecute(int runId) {
        if(IsThisReady(runId)) {
            base.DoExecute(runId);
        }
    }
}
