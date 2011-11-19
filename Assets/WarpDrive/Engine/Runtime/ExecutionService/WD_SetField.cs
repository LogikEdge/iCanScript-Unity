using UnityEngine;
using System;
using System.Reflection;
using System.Collections;

public class WD_SetField : WD_FunctionBase {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    protected FieldInfo myFieldInfo;

    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public WD_SetField(string name, FieldInfo fieldInfo, object[] parameters, bool[] paramIsOuts) : base(name, parameters, paramIsOuts) {
        myFieldInfo= fieldInfo;
    }
    
    // ======================================================================
    // Execution
    // ----------------------------------------------------------------------
    protected override void DoExecute(int frameId) {
        // Execute function
        myParameters[2]= myParameters[0];
        myFieldInfo.SetValue(myParameters[0], myParameters[1]);
        MarkAsCurrent(frameId);
    }
}
