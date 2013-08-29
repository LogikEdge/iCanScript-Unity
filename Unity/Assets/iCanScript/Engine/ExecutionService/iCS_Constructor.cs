using UnityEngine;
using System;
using System.Reflection;
using System.Collections;

public class iCS_Constructor : iCS_ClassFunction {
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_Constructor(MethodBase methodBase, iCS_Storage storage, int instanceId, int priority, int nbOfParameters, int nbOfEnables)
    : base(methodBase, storage, instanceId, priority, nbOfParameters, nbOfEnables) {}
    
    // ======================================================================
    // Execution
    // ----------------------------------------------------------------------
    protected override void DoExecute(int frameId) {
        if(ReturnValue == null) {
            base.DoExecute(frameId);
        } else {
            MarkAsExecuted(frameId);
        }
    }
    // ----------------------------------------------------------------------
    protected override void DoForceExecute(int frameId) {
        if(ReturnValue == null) {
            base.DoForceExecute(frameId);
        } else {
            MarkAsExecuted(frameId);
        }
    }
}
