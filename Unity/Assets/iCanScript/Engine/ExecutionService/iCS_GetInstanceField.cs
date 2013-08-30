using UnityEngine;
using System;
using System.Reflection;
using System.Collections;

public class iCS_GetInstanceField : iCS_GetClassField {
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_GetInstanceField(FieldInfo fieldInfo, iCS_VisualScriptImp visualScript, int priority, int nbOfEnables)
    : base(fieldInfo, visualScript, priority, nbOfEnables) {
    }

    // ======================================================================
    // Execution
    // ----------------------------------------------------------------------
    protected override void DoExecute(int frameId) {
        if(IsThisReady(frameId)) {
            base.DoExecute(frameId);
        }
    }
}
