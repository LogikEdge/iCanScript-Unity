using UnityEngine;
using System.Reflection;
using System.Collections;

public class WD_ExternalAction : ScriptableObject {
    // ======================================================================
    // Attributes
    // ----------------------------------------------------------------------
    public ScriptableObject myTarget    = null;
    public string           myMethodName= null;
    protected MethodInfo    myMethodInfo= null;
    
    // ======================================================================
    // Execution
    // ----------------------------------------------------------------------
    public void Execute() {
        myMethodInfo.Invoke(myTarget, new object[]{this});
    }
    
    // ======================================================================
    // Validation
    // ----------------------------------------------------------------------
    protected bool IsValid() {
        System.Type targetType= myTarget.GetType();
        myMethodInfo= targetType.GetMethod(myMethodName);
        if(myMethodInfo == null) {
            Debug.LogError("Cannot find method "+myMethodName+" on object "+myTarget.name);
            return false;
        }
        if(myMethodInfo.ReturnType != typeof(void)) {
            Debug.LogWarning("Expected a void return on method "+myMethodInfo);
        }
        return true;
    }
}
