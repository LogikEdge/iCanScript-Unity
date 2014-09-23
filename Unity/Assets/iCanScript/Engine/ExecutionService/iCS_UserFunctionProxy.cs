using UnityEngine;
using System;
using System.Collections;

public class iCS_UserFunctionProxy : iCS_ActionWithSignature {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    protected iCS_ActionWithSignature   myUserAction= null;

    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_UserFunctionProxy(iCS_ActionWithSignature userAction, iCS_VisualScriptImp visualScript, int priority,
                                 int nbOfParameters, int nbOfEnables)
    : base(visualScript, priority, nbOfParameters, nbOfEnables) {
        myUserAction= userAction;
    }

    // ======================================================================
    // Execution
    // ----------------------------------------------------------------------
    protected override void DoExecute(int frameId) {
//#if UNITY_EDITOR
        try {
//#endif
            // Wait until all inputs are ready.
            var parameterStart= ParametersStart;
            var parameterEnd= ParametersEnd;
            for(int i= parameterStart; i <= parameterEnd; ++i) {
                if(IsParameterReady(i, frameId) == false) {
                    return;
                }
            }
            // Fetch all parameters.
            for(int i= parameterStart; i <= parameterEnd; ++i) {
                UpdateParameter(i);
            }
            // Copy input ports
            var parameters= Parameters;
            var userActionParameters= myUserAction.Parameters;
            for(int i= parameterStart; i < parameterEnd; ++i) {
                userActionParameters[i]= parameters[i];
            }
            // Execute associated function.
            // TODO: Should desynchronize frameid to force the execution.
            myUserAction.Execute(frameId);
            // Copy output ports
            for(int i= parameterStart; i < parameterEnd; ++i) {
                parameters[i]= userActionParameters[i];
            }
            ReturnValue= myUserAction.ReturnValue;
            // Reflection the action run status.
            IsStalled= myUserAction.IsStalled;
            if(myUserAction.DidExecute(frameId)) {
                MarkAsExecuted(frameId);
            }
            else if(myUserAction.IsCurrent(frameId)){
                MarkAsCurrent(frameId);
            }            
//#if UNITY_EDITOR
        }
        catch(Exception e) {
            Debug.LogWarning("iCanScript: Exception throw in  "+FullName+" => "+e.Message);
            string thisName= (InInstance == null ? "null" : InInstance.ToString());
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
//#endif
    }
    // ----------------------------------------------------------------------
    // TODO: UserFunction.DoForceExecute()
    protected override void DoForceExecute(int frameId) {
//#if UNITY_EDITOR
        try {
//#endif
            // Wait until all inputs are ready.
            var parameterStart= ParametersStart;
            var parameterEnd= ParametersEnd;
            for(int i= parameterStart; i <= parameterEnd; ++i) {
                if(IsParameterReady(i, frameId) == false) {
                    return;
                }
            }
            // Fetch all parameters.
            for(int i= parameterStart; i <= parameterEnd; ++i) {
                UpdateParameter(i);
            }
            // Copy input ports
            var parameters= Parameters;
            var userActionParameters= myUserAction.Parameters;
            for(int i= parameterStart; i < parameterEnd; ++i) {
                userActionParameters[i]= parameters[i];
            }
            // Execute associated function.
            // TODO: Should desynchronize frameid to force the execution.
            myUserAction.ForceExecute(frameId);
            // Copy output ports
            for(int i= parameterStart; i < parameterEnd; ++i) {
                parameters[i]= userActionParameters[i];
            }
            ReturnValue= myUserAction.ReturnValue;
            // Reflection the action run status.
            IsStalled= myUserAction.IsStalled;
            if(myUserAction.DidExecute(frameId)) {
                MarkAsExecuted(frameId);
            }
            else if(myUserAction.IsCurrent(frameId)){
                MarkAsCurrent(frameId);
            }            
//#if UNITY_EDITOR
        }
        catch(Exception e) {
            Debug.LogWarning("iCanScript: Exception throw in  "+FullName+" => "+e.Message);
            string thisName= (InInstance == null ? "null" : InInstance.ToString());
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
//#endif
    }
}
