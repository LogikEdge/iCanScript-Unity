using UnityEngine;
using System;
using System.Reflection;
using System.Collections;

public class iCS_InstanceFunction : iCS_FunctionBase {
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_InstanceFunction(MethodBase methodBase, string name, int priority, int nbOfParameters)
    : base(methodBase, name, priority, nbOfParameters, true, true) {
    }
    
    // ======================================================================
    // Execution
    // ----------------------------------------------------------------------
    protected override void DoExecute(int frameId) {
        // Execute function
#if UNITY_EDITOR
        try {
#endif
		    ReturnValue= myMethodBase.Invoke(This, Parameters);
            MarkAsCurrent(frameId);   
#if UNITY_EDITOR
        }
        catch(Exception e) {
            Debug.LogWarning("iCanScript: Exception throw in  "+this+" => "+e.Message);
        }
#endif             
    }
}
