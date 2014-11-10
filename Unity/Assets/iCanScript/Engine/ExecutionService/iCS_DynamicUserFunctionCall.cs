using UnityEngine;
using System;
using System.Collections;

public class iCS_DynamicUserFunctionCall : iCS_ActionWithSignature {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    protected iCS_ActionWithSignature   myUserAction= null;
              bool                      isActionOwner= false;
              int                       actionFrameId= 0;

    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_DynamicUserFunctionCall(iCS_VisualScriptImp visualScript, int priority,
                                       int nbOfParameters, int nbOfEnables)
    : base(visualScript, priority, nbOfParameters, nbOfEnables) {
        Debug.Log("Creating a dynamic user function call");
    }

    // ======================================================================
    // Execution
    // ----------------------------------------------------------------------
    protected override void DoExecute(int frameId) {
//#if UNITY_EDITOR
        try {
//#endif
            // Wait until this port is ready.
            if(IsThisReady(frameId)) {
                // Fetch the user action.
                var gameObject= InInstance as GameObject;
                if(gameObject == null) {
                    Debug.LogWarning("iCanScript: Unable to find game object with variable: "+FullName);
                    MarkAsCurrent(frameId);
                }
                var vs= gameObject.GetComponent(typeof(iCS_VisualScriptImp)) as iCS_VisualScriptImp;
                if(vs == null) {
                    Debug.LogWarning("iCanScript: Unable to find visual script that contains variable: "+FullName+" in game object: "+gameObject.name);
                    MarkAsCurrent(frameId);
                }
                var variableObject= vs.GetPublicInterfaceFromName(Name);
                if(variableObject == null) {
                    Debug.LogWarning("iCanScript: Unable to find variable: "+FullName+" in visual script of game object: "+gameObject.name);
                    MarkAsCurrent(frameId);
                }
                var myUserAction= vs.RuntimeNodes[variableObject.InstanceId] as iCS_ActionWithSignature;
    
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
    protected override void DoForceExecute(int frameId) {
//#if UNITY_EDITOR
        try {
//#endif
            // Fetch the user action.
            var gameObject= InInstance as GameObject;
            if(gameObject == null) {
                Debug.LogWarning("iCanScript: Unable to find game object with variable: "+FullName);
                MarkAsCurrent(frameId);
            }
            var vs= gameObject.GetComponent(typeof(iCS_VisualScriptImp)) as iCS_VisualScriptImp;
            if(vs == null) {
                Debug.LogWarning("iCanScript: Unable to find visual script that contains variable: "+FullName+" in game object: "+gameObject.name);
                MarkAsCurrent(frameId);
            }
            var variableObject= vs.GetPublicInterfaceFromName(Name);
            if(variableObject == null) {
                Debug.LogWarning("iCanScript: Unable to find variable: "+FullName+" in visual script of game object: "+gameObject.name);
                MarkAsCurrent(frameId);
            }
            var myUserAction= vs.RuntimeNodes[variableObject.InstanceId] as iCS_ActionWithSignature;

            // Fetch all parameters.
            var parameterStart= ParametersStart;
            var parameterEnd= ParametersEnd;
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
}
