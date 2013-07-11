using UnityEngine;
using System;
using System.Reflection;
using System.Collections;

public class iCS_GetInstanceField : iCS_FieldBase {
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_GetInstanceField(FieldInfo fieldInfo, string name, int priority)
    : base(fieldInfo, name, priority, 0, true, true) {
    }

    // ======================================================================
    // Execution
    // ----------------------------------------------------------------------
    protected override void DoExecute(int frameId) {
        // Execute function
#if UNITY_EDITOR
        try {
#endif
            ReturnValue= myFieldInfo.GetValue(This);
            MarkAsCurrent(frameId);
#if UNITY_EDITOR
        }
        catch(Exception e) {
            Debug.LogWarning("iCanScript: Exception throw in  "+this+" => "+e.Message);
        }
#endif
    }
}
