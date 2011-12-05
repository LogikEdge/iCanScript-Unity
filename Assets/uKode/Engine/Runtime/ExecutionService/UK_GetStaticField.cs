using UnityEngine;
using System;
using System.Reflection;
using System.Collections;

public class UK_GetStaticField : UK_FunctionBase {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    protected FieldInfo   myFieldInfo;

    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public UK_GetStaticField(string name, FieldInfo fieldInfo, object[] parameters, bool[] paramIsOuts, Vector2 layout) : base(name, parameters, paramIsOuts, layout) {
        myFieldInfo= fieldInfo;
    }
    
    // ======================================================================
    // Execution
    // ----------------------------------------------------------------------
    protected override void DoExecute(int frameId) {
        // Execute function
        myParameters[0]= myFieldInfo.GetValue(null);
        MarkAsCurrent(frameId);
    }
}
