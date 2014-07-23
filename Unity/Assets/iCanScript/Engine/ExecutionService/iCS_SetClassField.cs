using UnityEngine;
using System;
using System.Reflection;
using System.Collections;

public class iCS_SetClassField : iCS_FieldBase {
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_SetClassField(FieldInfo fieldInfo, iCS_VisualScriptImp visualScript, int priority, int nbOfEnables)
    : base(fieldInfo, visualScript, priority, 1, nbOfEnables) {}
    
    // ======================================================================
    // Execution
    // ----------------------------------------------------------------------
    protected override void DoExecute(int frameId) {
        if(!IsParameterReady(0, frameId)) {
            IsStalled= true;
            return;
        }
        DoForceExecute(frameId);
    }
    // ----------------------------------------------------------------------
    protected override void DoForceExecute(int frameId) {
        // Execute function
        UpdateParameter(0);
//#if UNITY_EDITOR
        try {
//#endif
            myFieldInfo.SetValue(null, Parameters[0]);
            MarkAsExecuted(frameId);
//#if UNITY_EDITOR
        }
        catch(Exception e) {
            Debug.LogWarning("iCanScript: Exception throw in  "+FullName+" => "+e.Message);
            MarkAsCurrent(frameId);
        }
//#endif        
    }
}
