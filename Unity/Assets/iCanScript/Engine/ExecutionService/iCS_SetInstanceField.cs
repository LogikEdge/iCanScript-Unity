using UnityEngine;
using System;
using System.Reflection;
using System.Collections;

public class iCS_SetInstanceField : iCS_FieldBase {
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_SetInstanceField(FieldInfo fieldInfo, string name, int priority)
    : base(fieldInfo, name, priority, 1, true, true) {
    }

    // ======================================================================
    // Execution
    // ----------------------------------------------------------------------
    protected override void DoExecute(int frameId) {
        // Execute function
        myFieldInfo.SetValue(This, Parameters[0]);
        ReturnValue= This;
        MarkAsCurrent(frameId);
    }
}
