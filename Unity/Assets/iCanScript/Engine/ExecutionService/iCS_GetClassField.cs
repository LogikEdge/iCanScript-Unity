using UnityEngine;
using System;
using System.Reflection;
using System.Collections;

public class iCS_GetClassField : iCS_FieldBase {
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_GetClassField(FieldInfo fieldInfo, string name, int priority)
    : base(fieldInfo, name, priority, 0, true, false) {}
    
    // ======================================================================
    // Execution
    // ----------------------------------------------------------------------
    protected override void DoExecute(int frameId) {
        // Execute function
        ReturnValue= myFieldInfo.GetValue(null);
        MarkAsCurrent(frameId);
    }
}
