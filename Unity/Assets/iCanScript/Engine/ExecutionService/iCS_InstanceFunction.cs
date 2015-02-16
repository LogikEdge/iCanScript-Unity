using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using Subspace;

public class iCS_InstanceFunction : iCS_ClassFunction {
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_InstanceFunction(string name, SSObject parent, MethodBase methodBase, int priority, int nbOfParameters, int nbOfEnables)
    : base(name, parent, methodBase, priority, nbOfParameters, nbOfEnables) {
    }
    
    // ======================================================================
    // Execution
    // ----------------------------------------------------------------------
    protected override void DoEvaluate() {
        if(IsThisReady) {
            base.DoEvaluate();
        }
    }
}
