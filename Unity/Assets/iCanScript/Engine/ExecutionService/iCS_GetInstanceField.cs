using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using Subspace;

public class iCS_GetInstanceField : iCS_GetClassField {
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_GetInstanceField(string name, SSObject parent, FieldInfo fieldInfo, int priority, int nbOfEnables)
    : base(name, parent, fieldInfo, priority, nbOfEnables) {
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
