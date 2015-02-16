using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using Subspace;

public class iCS_SetInstanceField : iCS_FieldBase {
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_SetInstanceField(string name, SSObject parent, FieldInfo fieldInfo, int priority, int nbOfEnables)
    : base(name, parent, fieldInfo, priority, 1, nbOfEnables) {
    }

    // ======================================================================
    // Execution
    // ----------------------------------------------------------------------
    protected override void DoEvaluate() {
        if(!IsThisReady || !IsParameterReady(0)) {
            IsStalled= true;
            return;
        }
        DoExecute();
    }
    // ----------------------------------------------------------------------
    protected override void DoExecute() {
        // Execute function
        UpdateParameter(0);
//#if UNITY_EDITOR
        try {
//#endif
            myFieldInfo.SetValue(This, Parameters[0]);
            MarkAsExecuted();
//#if UNITY_EDITOR
        }
        catch(Exception e) {
            Debug.LogWarning("iCanScript: Exception throw in  "+FullName+" => "+e.Message);
            MarkAsCurrent();
        }
//#endif
    }
}
