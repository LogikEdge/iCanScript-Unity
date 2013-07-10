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
        if(This != null) {
			ReturnValue= myMethodBase.Invoke(This, Parameters);
		} else {
			Debug.LogWarning ("Trying to execute "+myMethodBase.Name+" without a connected instance...");
		}
        MarkAsCurrent(frameId);        
    }
}
