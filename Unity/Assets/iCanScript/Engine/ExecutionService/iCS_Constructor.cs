using UnityEngine;
using System;
using System.Reflection;
using System.Collections;

public class iCS_Constructor : iCS_FunctionBase {
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_Constructor(MethodBase methodBase, iCS_Storage storage, int instanceId, int priority, int nbOfParameters)
    : base(methodBase, storage, instanceId, priority, nbOfParameters, true, false) {}
    
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
            Debug.LogWarning("iCanScript: Exception throw in  "+FullName+" => "+e.Message);
        }
#endif        
    }
}
