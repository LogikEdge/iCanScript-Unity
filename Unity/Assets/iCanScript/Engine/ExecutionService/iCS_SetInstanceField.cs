using UnityEngine;
using System;
using System.Reflection;
using System.Collections;

public class iCS_SetInstanceField : iCS_FieldBase {
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_SetInstanceField(FieldInfo fieldInfo, iCS_Storage storage, int instanceId, int priority, int nbOfEnables)
    : base(fieldInfo, storage, instanceId, priority, 1, nbOfEnables) {
    }

    // ======================================================================
    // Execution
    // ----------------------------------------------------------------------
    protected override void DoExecute(int frameId) {
        if(!IsThisReady(frameId) || !IsParameterReady(0, frameId)) {
            IsStalled= true;
            return;
        }
        DoForceExecute(frameId);
    }
    // ----------------------------------------------------------------------
    protected override void DoForceExecute(int frameId) {
        // Execute function
        UpdateParameter(0);
#if UNITY_EDITOR
        try {
#endif
            myFieldInfo.SetValue(This, Parameters[0]);
            OutThis= This;
            MarkAsExecuted(frameId);
#if UNITY_EDITOR
        }
        catch(Exception e) {
            Debug.LogWarning("iCanScript: Exception throw in  "+FullName+" => "+e.Message);
        }
#endif
    }
}
