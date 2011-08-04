using UnityEngine;
using System.Reflection;
using System.Collections;

public class AP_ExternProperty : ScriptableObject {
    // ======================================================================
    // Attributes
    // ----------------------------------------------------------------------
    public      Object       Target= null;
    public      string       PropertyName= null;
    protected   PropertyInfo PropertyInfo= null;

    // ======================================================================
    // Validation
    // ----------------------------------------------------------------------
    protected bool IsValid() {
        System.Type targetType= Target.GetType();
        PropertyInfo= targetType.GetProperty(PropertyName);
        if(PropertyInfo==null) {
            Debug.LogError("Property "+PropertyName+" not found on "+Target.name);
            return false;
        }
        return true;
    }
}
