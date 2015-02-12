using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using Subspace;

public class iCS_ClassFunction : iCS_FunctionBase {
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_ClassFunction(int instanceId, string name, MethodBase methodBase, SSContext context, int priority, int nbOfParameters, int nbOfEnables)
    : base(instanceId, name, methodBase, context, priority, nbOfParameters, nbOfEnables) {}

    // ======================================================================
    // Execution
    // ----------------------------------------------------------------------
    protected override void DoExecute(int runId) {
        // Wait until all inputs are ready.
        var len= Parameters.Length;
        for(int i= 0; i < len; ++i) {
            if(IsParameterReady(i, runId) == false) {
                return;
            }
        }
        // Execute associated function.
        DoForceExecute(runId);
    }
    // ----------------------------------------------------------------------
    protected override void DoForceExecute(int runId) {
//#if UNITY_EDITOR
        try {
//#endif
            // Fetch all parameters.
            var len= Parameters.Length;
            for(int i= 0; i < len; ++i) {
                UpdateParameter(i);
            }
            
            // Execute function
            ReturnValue= myMethodBase.Invoke(This, Parameters);            
            MarkAsExecuted(runId);
//#if UNITY_EDITOR
        }
        catch(Exception e) {
            string thisName= (This == null ? "null" : This.ToString());
            string parametersAsStr= "";
            int nbOfParams= Parameters.Length;
            if(nbOfParams != 0) {
                for(int i= 0; i < nbOfParams; ++i) {
					var p= Parameters[i];
                    string paramStr= "";
                    if(p == null) {
                        paramStr= "null";
                    }
                    else {
    					paramStr+= p.ToString();
    					if(p is String) {
    						paramStr= "\""+paramStr+"\"";
    					}
    					if(p is Char) {
    						paramStr= "\'"+paramStr+"\'";
    					}                        
                    }
					parametersAsStr+= paramStr;
                    if(i != nbOfParams-1) {
                        parametersAsStr+=", ";
                    }
                }
            }
            var msg= "Exception thrown in=> "+FullName+"("+thisName+", "+parametersAsStr+") EXCEPTION=> "+e.Message;
			ErrorControllerProxy.AddError("Runtime", msg, Context.VisualScript, InstanceId);
            MarkAsCurrent(runId);
        }
//#endif        
    }
}
