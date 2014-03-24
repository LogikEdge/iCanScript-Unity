using UnityEngine;
using System;
using System.Reflection;
using System.Collections;

public class iCS_ClassFunction : iCS_FunctionBase {
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_ClassFunction(MethodBase methodBase, iCS_VisualScriptImp visualScript, int priority, int nbOfParameters, int nbOfEnables)
    : base(methodBase, visualScript, priority, nbOfParameters, nbOfEnables) {}

    // ======================================================================
    // Execution
    // ----------------------------------------------------------------------
    protected override void DoExecute(int frameId) {
        // Wait until all inputs are ready.
        var end= ParametersEnd;
        for(int i= ParametersStart; i <= end; ++i) {
            if(IsParameterReady(i, frameId) == false) {
                return;
            }
        }
        // Execute associated function.
        DoForceExecute(frameId);
    }
    // ----------------------------------------------------------------------
    protected override void DoForceExecute(int frameId) {
#if UNITY_EDITOR
        try {
#endif
            // Fetch all parameters.
            var end= ParametersEnd;
            for(int i= ParametersStart; i <= end; ++i) {
                UpdateParameter(i);
            }
            
            // Execute function
            ReturnValue= myMethodBase.Invoke(This, Parameters);            
            MarkAsExecuted(frameId);
#if UNITY_EDITOR
        }
        catch(Exception e) {
            Debug.LogWarning("iCanScript: Exception throw in  "+FullName+" => "+e.Message);
            string thisName= (This == null ? "null" : This.ToString());
            string parametersAsStr= "";
            int nbOfParams= Parameters.Length;
            if(nbOfParams != 0) {
                for(int i= 0; i < nbOfParams; ++i) {
                    parametersAsStr+= Parameters[i].ToString();
                    if(i != nbOfParams-1) {
                        parametersAsStr+=", ";
                    }
                }
            }
            Debug.LogWarning("iCanScript: while invoking => "+thisName+"."+Name+"("+parametersAsStr+")");
            MarkAsCurrent(frameId);
        }
#endif        
    }
}
