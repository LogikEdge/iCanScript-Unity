using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using Subspace;

public class iCS_ClassFunction : iCS_FunctionBase {
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_ClassFunction(string name, SSObject parent, MethodBase methodBase, int priority, int nbOfParameters, int nbOfEnables)
    : base(name, parent, methodBase, priority, nbOfParameters, nbOfEnables) {}

    // ======================================================================
    // Execution
    // ----------------------------------------------------------------------
    protected override void DoEvaluate() {
        // Wait until all inputs are ready.
        var len= Parameters.Length;
        for(int i= 0; i < len; ++i) {
            if(IsParameterReady(i, myContext.RunId) == false) {
                return;
            }
        }
        // Execute associated function.
        DoExecute();
    }
    // ----------------------------------------------------------------------
    protected override void DoExecute() {
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
            MarkAsExecuted();
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
            Context.ReportError(msg, this);
            MarkAsCurrent();
        }
//#endif        
    }
}
