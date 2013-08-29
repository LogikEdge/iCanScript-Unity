using UnityEngine;
using System;
using System.Reflection;
using System.Collections;

public class iCS_GetClassField : iCS_FieldBase {
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_GetClassField(FieldInfo fieldInfo, iCS_Storage storage, int instanceId, int priority, int nbOfEnables)
    : base(fieldInfo, storage, instanceId, priority, 0, nbOfEnables) {}
    
    // ======================================================================
    // Execution
    // ----------------------------------------------------------------------
    protected override void DoExecute(int frameId) {
        DoForceExecute(frameId);
    }
    // ----------------------------------------------------------------------
    protected override void DoForceExecute(int frameId) {
#if UNITY_EDITOR
        try {
#endif
            // Execute function
            ReturnValue= myFieldInfo.GetValue(This);
            MarkAsExecuted(frameId);
#if UNITY_EDITOR
        }
        catch(Exception e) {
            Debug.LogWarning("iCanScript: Exception throw in  "+FullName+" => "+e.Message);
        }
#endif        
    }
}
