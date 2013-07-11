using UnityEngine;
using System;
using System.Reflection;
using System.Collections;

public class iCS_Constructor : iCS_FunctionBase {
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_Constructor(MethodBase methodBase, string name, int priority, int nbOfParameters)
    : base(methodBase, name, priority, nbOfParameters, true, false) {}
    
    // ======================================================================
    // Execution
    // ----------------------------------------------------------------------
    protected override void DoExecute(int frameId) {
        // Execute function
#if UNITY_EDITOR
        try {
#endif
            if(ReturnValue == null) {
                ReturnValue= myMethodBase.Invoke(null, Parameters);            
            }
            MarkAsCurrent(frameId);
#if UNITY_EDITOR
        }
        catch(Exception e) {
            Debug.LogWarning("iCanScript: Exception throw in  "+this+" => "+e.Message);
        }
#endif        
    }
}
