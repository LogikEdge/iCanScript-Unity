using UnityEngine;
using System;
using System.Reflection;
using System.Collections;

public class iCS_InstanceFunction : iCS_ClassFunction {
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_InstanceFunction(MethodBase methodBase, iCS_Storage storage, int instanceId, int priority, int nbOfParameters, int nbOfEnables)
    : base(methodBase, storage, instanceId, priority, nbOfParameters, nbOfEnables) {
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
