using UnityEngine;
using System.Collections;
using Subspace;

public class iCS_DynamicVariableProxy : SSActionWithSignature {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    protected SSActionWithSignature   myUserAction= null;

    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_DynamicVariableProxy(string name, SSObject parent, int priority,
                                    int nbOfParameters, int nbOfEnables)
    : base(name, parent, priority, nbOfParameters, nbOfEnables) {
    }

    // ======================================================================
    // Execution
    // ----------------------------------------------------------------------
    protected override void DoEvaluate() {
        // Wait until this port is ready.
        if(IsThisReady) {
            // Try to connect with the visual script.
            var gameObject= This as GameObject;
            if(gameObject == null) {
                Debug.LogWarning("iCanScript: Unable to find game object with variable: "+FullName);
                MarkAsCurrent();
            }
            var vs= gameObject.GetComponent(typeof(iCS_VisualScriptImp)) as iCS_VisualScriptImp;
            if(vs == null) {
                Debug.LogWarning("iCanScript: Unable to find visual script that contains variable: "+FullName+" in game object: "+gameObject.name);
                MarkAsCurrent();
            }
            var variableObject= vs.GetPublicInterfaceFromName(Name);
            if(variableObject == null) {
                Debug.LogWarning("iCanScript: Unable to find variable: "+FullName+" in visual script of game object: "+gameObject.name);
                MarkAsCurrent();
            }
            var variable= vs.RuntimeNodes[variableObject.InstanceId] as SSActionWithSignature;
            ReturnValue= variable.ReturnValue;
            MarkAsExecuted();            
        }
    }

    // ----------------------------------------------------------------------
    protected override void DoExecute() {
        // Try to connect with the visual script.
        var gameObject= This as GameObject;
        if(gameObject == null) {
            Debug.LogWarning("iCanScript: Unable to find game object with variable: "+FullName);
            MarkAsCurrent();
        }
        var vs= gameObject.GetComponent(typeof(iCS_VisualScriptImp)) as iCS_VisualScriptImp;
        if(vs == null) {
            Debug.LogWarning("iCanScript: Unable to find visual script that contains variable: "+FullName+" in game object: "+gameObject.name);
            MarkAsCurrent();
        }
        var variableObject= vs.GetPublicInterfaceFromName(Name);
        if(variableObject == null) {
            Debug.LogWarning("iCanScript: Unable to find variable: "+FullName+" in visual script of game object: "+gameObject.name);
            MarkAsCurrent();
        }
        var variable= vs.RuntimeNodes[variableObject.InstanceId] as SSActionWithSignature;
        ReturnValue= variable.ReturnValue;
        MarkAsExecuted();            
    }
}
