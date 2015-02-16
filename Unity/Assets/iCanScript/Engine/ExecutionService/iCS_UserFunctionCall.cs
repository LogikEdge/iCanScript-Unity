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

    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_UserFunctionCall(string name, SSObject parent, SSActionWithSignature userAction,
							    int priority, int nbOfParameters, int nbOfEnables)
    : base(name, parent, priority, nbOfParameters, nbOfEnables) {
        myUserAction= userAction;
    }

    // ======================================================================
    // Execution
    // ----------------------------------------------------------------------
    protected override void DoExecute() {
//#if UNITY_EDITOR
        try {
//#endif
            // Skip all the processing if we don't have an target action to execute.
            if(myUserAction == null) {
                MarkAsCurrent();
                return;
            }
            
            // Wait until all inputs are ready.
            var parameterLen= Parameters.Length;
            for(int i= 0; i < parameterLen; ++i) {
                if(IsParameterReady(i, myContext.RunId) == false) {
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
                myUserAction.Context.RunId= myUserAction.Context.RunId+1;
            }
            myUserAction.Evaluate();
            // Copy output ports
            for(int i= 0; i < parameterLen; ++i) {
				UpdateParameter(i);
			}
            // Reflection the action run status.
            IsStalled= myUserAction.IsStalled;
            if(myUserAction.DidExecute()) {
                isActionOwner= false;
                myUserAction.IsActive= false;
                MarkAsExecuted();
            }
            else if(myUserAction.IsCurrent){
                isActionOwner= false;
                myUserAction.IsActive= false;
                MarkAsCurrent();
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
            MarkAsCurrent();
        }
//#endif
    }
    // ----------------------------------------------------------------------
    // TODO: UserFunction.DoForceExecute()
    protected override void DoForceExecute() {
//#if UNITY_EDITOR
        try {
//#endif
            // Wait until all inputs are ready.
            var parameterLen= Parameters.Length;
            for(int i= 0; i < parameterLen; ++i) {
                if(IsParameterReady(i, myContext.RunId) == false) {
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
                myUserAction.Context.RunId= myUserAction.Context.RunId+1;
            }
            myUserAction.ForceExecute();
            // Copy output ports
            for(int i= 0; i < parameterLen; ++i) {
				UpdateParameter(i);
            }
            ReturnValue= myUserAction.ReturnValue;
            // Reflection the action run status.
            IsStalled= myUserAction.IsStalled;
            if(myUserAction.DidExecute()) {
                isActionOwner= false;
                myUserAction.IsActive= false;
                MarkAsExecuted();
            }
            else if(myUserAction.IsCurrent){
                isActionOwner= false;
                myUserAction.IsActive= false;
                MarkAsCurrent();
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
            MarkAsCurrent();
        }
//#endif
    }
}
