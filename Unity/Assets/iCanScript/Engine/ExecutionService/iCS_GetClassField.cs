using UnityEngine;
using System;
using System.Reflection;
using System.Collections;

public class iCS_GetClassField : iCS_FieldBase {
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_GetClassField(FieldInfo fieldInfo, iCS_Storage storage, int instanceId, int priority)
    : base(fieldInfo, storage, instanceId, priority, 0, true, false) {}
    
    // ======================================================================
    // Execution
    // ----------------------------------------------------------------------
    protected override void DoExecute(int frameId) {
        // Execute function
#if UNITY_EDITOR
        try {
#endif
            ReturnValue= myFieldInfo.GetValue(null);
            MarkAsCurrent(frameId);
#if UNITY_EDITOR
        }
        catch(Exception e) {
            Debug.LogWarning("iCanScript: Exception throw in  "+this+" => "+e.Message);
        }
#endif
    }
}
