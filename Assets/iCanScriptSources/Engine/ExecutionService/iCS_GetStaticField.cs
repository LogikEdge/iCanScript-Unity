using UnityEngine;
using System;
using System.Reflection;
using System.Collections;

public class iCS_GetStaticField : iCS_FunctionBase {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    protected object    myReturn= null;
    protected FieldInfo myFieldInfo;

    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_GetStaticField(string name, FieldInfo fieldInfo, bool[] paramIsOuts, Vector2 layout) : base(name, paramIsOuts, layout) {
        myFieldInfo= fieldInfo;
    }
    
    // ======================================================================
    // Accessors
    // ----------------------------------------------------------------------
    protected override object DoGetParameter(int idx) {
        return idx == 0 ? myReturn : base.DoGetParameter(idx);
    }
    protected override void DoSetParameter(int idx, object value) {
        if(idx == 0) { myReturn= value; return; }
        base.DoSetParameter(idx, value);
    }
    protected override bool DoIsParameterReady(int idx, int frameId) {
        return idx == 0 ? IsCurrent(frameId) : base.DoIsParameterReady(idx, frameId);
    }

    // ======================================================================
    // Execution
    // ----------------------------------------------------------------------
    protected override void DoExecute(int frameId) {
        // Execute function
        myReturn= myFieldInfo.GetValue(null);
        MarkAsCurrent(frameId);
    }
}
