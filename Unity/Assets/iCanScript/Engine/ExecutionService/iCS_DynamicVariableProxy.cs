using UnityEngine;
using System.Collections;

public class iCS_DynamicVariableProxy : iCS_ActionWithSignature {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    protected iCS_ActionWithSignature   myUserAction= null;

    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_DynamicVariableProxy(iCS_VisualScriptImp visualScript, int priority,
                                    int nbOfParameters, int nbOfEnables)
    : base(visualScript, priority, nbOfParameters, nbOfEnables) {
    }

    // ======================================================================
    // Execution
    // ----------------------------------------------------------------------
    protected override void DoExecute(int frameId) {
        // Wait until this port is ready.
        if(IsThisReady(frameId)) {
            // Try to connect with the visual script.
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
            var variable= vs.RuntimeNodes[variableObject.InstanceId] as iCS_ActionWithSignature;
            ReturnValue= variable.ReturnValue;
            MarkAsExecuted(frameId);            
        }
    }

    // ----------------------------------------------------------------------
    protected override void DoForceExecute(int frameId) {
        // Try to connect with the visual script.
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
        var variable= vs.RuntimeNodes[variableObject.InstanceId] as iCS_ActionWithSignature;
        ReturnValue= variable.ReturnValue;
        MarkAsExecuted(frameId);            
    }
}
