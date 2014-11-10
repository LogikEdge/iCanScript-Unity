using UnityEngine;
using System;
using System.Collections;

public class iCS_UserFunctionCall : iCS_ActionWithSignature {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    protected iCS_ActionWithSignature   myUserAction = null;
              bool                      isActionOwner= false;
              int                       actionFrameId= 0;

    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_UserFunctionCall(iCS_ActionWithSignature userAction, iCS_VisualScriptImp visualScript, int priority,
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
            // Skip all the processing if we don't have an target action to execute.
            if(myUserAction == null) {
                MarkAsCurrent(frameId);
                return;
            }
            
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
            for(int i= parameterStart; i <= parameterEnd; ++i) {
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
                actionFrameId= myUserAction.FrameId+1;
            }
            myUserAction.Execute(actionFrameId);
            // Copy output ports
            for(int i= parameterStart; i <= parameterEnd; ++i) {
				UpdateParameter(i);
			}
            // Reflection the action run status.
            IsStalled= myUserAction.IsStalled;
            if(myUserAction.DidExecute(actionFrameId)) {
                isActionOwner= false;
                myUserAction.IsActive= false;
                MarkAsExecuted(frameId);
            }
            else if(myUserAction.IsCurrent(actionFrameId)){
                isActionOwner= false;
                myUserAction.IsActive= false;
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
            if(isActionOwner) {
                isActionOwner= false;
                myUserAction.IsActive= false;                
            }
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
            for(int i= parameterStart; i <= parameterEnd; ++i) {
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
                actionFrameId= myUserAction.FrameId+1;
            }
            myUserAction.ForceExecute(actionFrameId);
            // Copy output ports
            for(int i= parameterStart; i <= parameterEnd; ++i) {
				UpdateParameter(i);
            }
            ReturnValue= myUserAction.ReturnValue;
            // Reflection the action run status.
            IsStalled= myUserAction.IsStalled;
            if(myUserAction.DidExecute(actionFrameId)) {
                isActionOwner= false;
                myUserAction.IsActive= false;
                MarkAsExecuted(frameId);
            }
            else if(myUserAction.IsCurrent(actionFrameId)){
                isActionOwner= false;
                myUserAction.IsActive= false;
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
            if(isActionOwner) {
                isActionOwner= false;
                myUserAction.IsActive= false;                
            }
            MarkAsCurrent(frameId);
        }
//#endif
    }
}
