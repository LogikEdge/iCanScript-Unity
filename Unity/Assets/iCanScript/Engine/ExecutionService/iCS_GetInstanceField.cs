using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using Subspace;

public class iCS_GetInstanceField : iCS_GetClassField {
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_GetInstanceField(int instanceId, string name, SSObject parent, FieldInfo fieldInfo, SSContext context, int priority, int nbOfEnables)
    : base(instanceId, name, parent, fieldInfo, context, priority, nbOfEnables) {
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
