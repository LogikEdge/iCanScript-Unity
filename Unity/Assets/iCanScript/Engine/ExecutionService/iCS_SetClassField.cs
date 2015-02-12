using UnityEngine;
using System;
using System.Reflection;
using System.Collections;

public class iCS_SetClassField : iCS_FieldBase {
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_SetClassField(int instanceId, string name, FieldInfo fieldInfo, iCS_VisualScriptImp visualScript, int priority, int nbOfEnables)
    : base(instanceId, name, fieldInfo, visualScript, priority, 1, nbOfEnables) {}
    
    // ======================================================================
    // Execution
    // ----------------------------------------------------------------------
    protected override void DoExecute(int runId) {
        if(!IsParameterReady(0, runId)) {
            IsStalled= true;
            return;
        }
        DoForceExecute(runId);
    }
    // ----------------------------------------------------------------------
    protected override void DoForceExecute(int runId) {
        // Execute function
        UpdateParameter(0);
//#if UNITY_EDITOR
        try {
//#endif
            myFieldInfo.SetValue(null, Parameters[0]);
            MarkAsExecuted(runId);
//#if UNITY_EDITOR
        }
        catch(Exception e) {
            Debug.LogWarning("iCanScript: Exception throw in  "+FullName+" => "+e.Message);
            MarkAsCurrent(runId);
        }
//#endif        
    }
}
