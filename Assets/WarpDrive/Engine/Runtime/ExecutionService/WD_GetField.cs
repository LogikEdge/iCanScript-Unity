using UnityEngine;
using System;
using System.Reflection;
using System.Collections;

public class WD_GetField : WD_FunctionBase {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    protected FieldInfo   myFieldInfo;

    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public WD_GetField(string name, FieldInfo fieldInfo, object[] parameters, bool[] paramIsOuts) : base(name, parameters, paramIsOuts) {
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
