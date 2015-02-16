using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using Subspace;

public class iCS_SetClassField : iCS_FieldBase {
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_SetClassField(string name, SSObject parent, FieldInfo fieldInfo, int priority, int nbOfEnables)
    : base(name, parent, fieldInfo, priority, 1, nbOfEnables) {}
    
    // ======================================================================
    // Execution
    // ----------------------------------------------------------------------
    protected override void DoExecute() {
        if(!IsParameterReady(0, myContext.RunId)) {
            IsStalled= true;
            return;
        }
        DoForceExecute();
    }
    // ----------------------------------------------------------------------
    protected override void DoForceExecute() {
        // Execute function
        UpdateParameter(0);
//#if UNITY_EDITOR
        try {
//#endif
            myFieldInfo.SetValue(null, Parameters[0]);
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
