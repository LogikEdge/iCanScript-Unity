using UnityEngine;
using System;
using System.Reflection;
using System.Collections;

public class UK_GetInstanceField : UK_FunctionBase {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    protected FieldInfo   myFieldInfo;

    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public UK_GetInstanceField(string name, FieldInfo fieldInfo, object[] parameters, bool[] paramIsOuts, Vector2 layout) : base(name, parameters, paramIsOuts, layout) {
        myFieldInfo= fieldInfo;
    }
    
    // ======================================================================
    // Execution
    // ----------------------------------------------------------------------
    protected override void DoExecute(int frameId) {
        // Execute function
        myParameters[2]= myParameters[0];
        myParameters[1]= myFieldInfo.GetValue(myParameters[0]);
        MarkAsCurrent(frameId);
    }
}
