using UnityEngine;
using System;
using System.Collections;
using Subspace;

public class iCS_UserFunctionCall : SSActionWithSignature {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    protected SSActionWithSignature myUserAction = null;
              bool                  isActionOwner= false;
              int                   actionFrameId= 0;

    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_UserFunctionCall(int instanceId, string name, SSActionWithSignature userAction, iCS_VisualScriptImp visualScript, int priority,
                                int nbOfParameters, int nbOfEnables)
    : base(instanceId, name, visualScript, priority, nbOfParameters, nbOfEnables) {
        myUserAction= userAction;
    }

    // ======================================================================
    // Execution
    // ----------------------------------------------------------------------
    protected override void DoExecute(int runId) {
//#if UNITY_EDITOR
        try {
//#endif
            // Skip all the processing if we don't have an target action to execute.
            if(myUserAction == null) {
                MarkAsCurrent(runId);
                return;
            }
            
            // Wait until all inputs are ready.
            var parameterLen= Parameters.Length;
            for(int i= 0; i < parameterLen; ++i) {
                if(IsParameterReady(i, runId) == false) {
                    return;
                }
            }
            // Fetch all parameters.
            for(int i= 0; i < parameterLen; ++i) {
                UpdateParameter(i);
            }
            // Copy input ports
            var parameters= Parameters;
            var userActionParameters= myUserAction.Parameters;
            for(int i= 0; i < parameterLen; ++i) {
                userActionParameters[i]= parameters[i];
            }
            // Wait unitil user function becomes available
            if(!isActionOwner && myUserAction.IsActive == true) {
                IsStalled= false;
                return;
            }
            // Execute associated function.
            myUserAction.IsActive= true;
            if(!isActionOwner) {
                isActionOwner= true;
                actionFrameId= myUserAction.RunId+1;
            }
            myUserAction.Execute(actionFrameId);
            // Copy output ports
            for(int i= 0; i < parameterLen; ++i) {
				UpdateParameter(i);
			}
            // Reflection the action run status.
            IsStalled= myUserAction.IsStalled;
            if(myUserAction.DidExecute(actionFrameId)) {
                isActionOwner= false;
                myUserAction.IsActive= false;
                MarkAsExecuted(runId);
            }
            else if(myUserAction.IsCurrent(actionFrameId)){
                isActionOwner= false;
                myUserAction.IsActive= false;
                MarkAsCurrent(runId);
            }            
//#if UNITY_EDITOR
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
            if(isActionOwner) {
                isActionOwner= false;
                myUserAction.IsActive= false;                
            }
            MarkAsCurrent(runId);
        }
//#endif
    }
    // ----------------------------------------------------------------------
    // TODO: UserFunction.DoForceExecute()
    protected override void DoForceExecute(int runId) {
//#if UNITY_EDITOR
        try {
//#endif
            // Wait until all inputs are ready.
            var parameterLen= Parameters.Length;
            for(int i= 0; i < parameterLen; ++i) {
                if(IsParameterReady(i, runId) == false) {
                    return;
                }
            }
            // Fetch all parameters.
            for(int i= 0; i < parameterLen; ++i) {
                UpdateParameter(i);
            }
            // Copy input ports
            var parameters= Parameters;
            var userActionParameters= myUserAction.Parameters;
            for(int i= 0; i < parameterLen; ++i) {
                userActionParameters[i]= parameters[i];
            }
            // Wait unitil user function becomes available
            if(!isActionOwner && myUserAction.IsActive == true) {
                IsStalled= false;
                return;
            }
            // Execute associated function.
            myUserAction.IsActive= true;
            if(!isActionOwner) {
                isActionOwner= true;
                actionFrameId= myUserAction.RunId+1;
            }
            myUserAction.ForceExecute(actionFrameId);
            // Copy output ports
            for(int i= 0; i < parameterLen; ++i) {
				UpdateParameter(i);
            }
            ReturnValue= myUserAction.ReturnValue;
            // Reflection the action run status.
            IsStalled= myUserAction.IsStalled;
            if(myUserAction.DidExecute(actionFrameId)) {
                isActionOwner= false;
                myUserAction.IsActive= false;
                MarkAsExecuted(runId);
            }
            else if(myUserAction.IsCurrent(actionFrameId)){
                isActionOwner= false;
                myUserAction.IsActive= false;
                MarkAsCurrent(runId);
            }            
//#if UNITY_EDITOR
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
            if(isActionOwner) {
                isActionOwner= false;
                myUserAction.IsActive= false;                
            }
            MarkAsCurrent(runId);
        }
//#endif
    }
}
