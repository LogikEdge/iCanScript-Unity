using UnityEngine;
using System.Collections;

public class AP_ExternPropertyFloat : AP_ExternProperty {
    // ======================================================================
    // Accessors
    // ----------------------------------------------------------------------
    public float Value {
        get {
            return (float)PropertyInfo.GetValue(Target, null);
        }
        set {
            PropertyInfo.SetValue(Target, value, null);
        }
    }
    
    // ======================================================================
    // Validation
    // ----------------------------------------------------------------------
    protected new bool IsValid() {
        if(!base.IsValid()) return false;
        if(PropertyInfo.GetGetMethod().ReturnType != typeof(float)) {
            Debug.LogError("Property "+PropertyName+" is not a float!!!");
            return false;
        }
        return true;
    }
}
