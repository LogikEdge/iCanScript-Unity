using UnityEngine;
using System;
using System.Reflection;
using System.Collections;

public class iCS_SetClassField : iCS_FieldBase {
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_SetClassField(FieldInfo fieldInfo, string name, int priority)
    : base(fieldInfo, name, priority, 1, false, false) {}
    
    // ======================================================================
    // Execution
    // ----------------------------------------------------------------------
    protected override void DoExecute(int frameId) {
        // Execute function
        myFieldInfo.SetValue(null, Parameters[0]);
        MarkAsCurrent(frameId);
    }
}
