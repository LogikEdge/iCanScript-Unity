using UnityEngine;
using System;
using System.Reflection;
using System.Collections;

public class iCS_SetClassField : iCS_FieldBase {
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_SetClassField(FieldInfo fieldInfo, iCS_Storage storage, int instanceId, int priority)
    : base(fieldInfo, storage, instanceId, priority, 1, false, false) {}
    
    // ======================================================================
    // Execution
    // ----------------------------------------------------------------------
    protected override void DoExecute(int frameId) {
        // Execute function
#if UNITY_EDITOR
        try {
#endif
            myFieldInfo.SetValue(null, Parameters[0]);
            MarkAsCurrent(frameId);
#if UNITY_EDITOR
        }
        catch(Exception e) {
            Debug.LogWarning("iCanScript: Exception throw in  "+this+" => "+e.Message);
        }
#endif        
    }
}
