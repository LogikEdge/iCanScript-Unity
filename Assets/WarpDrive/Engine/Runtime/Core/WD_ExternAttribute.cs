using UnityEngine;
using System.Reflection;
using System.Collections;

public class WD_ExternAttribute : ScriptableObject {
    // ======================================================================
    // Attributes
    // ----------------------------------------------------------------------
    public Object       Target= null;
    public string       AttributeName= null;
    public FieldInfo    AttributeFieldInfo= null;

    // ======================================================================
    // Validation
    // ----------------------------------------------------------------------
    protected bool IsValid() {
        System.Type targetType= Target.GetType();
        AttributeFieldInfo= targetType.GetField(AttributeName);
        if(AttributeFieldInfo==null) {
            Debug.LogError("Attribute "+AttributeName+" not found on "+Target.name);
            return false;
        }
        return true;
    }
}
