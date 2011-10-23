using UnityEngine;
using System.Reflection;
using System.Collections;

public class WD_ExternFloat : WD_ExternAttribute {
    // ======================================================================
    // Accessors
    // ----------------------------------------------------------------------
    public float Get() {
        return (float)AttributeFieldInfo.GetValue(Target);
    }
    public void Set(float f) {
        AttributeFieldInfo.SetValue(Target, f);
    }
    
    // ======================================================================
    // Validation
    // ----------------------------------------------------------------------
    protected new bool IsValid() {
        if(!base.IsValid()) return false;
        if(AttributeFieldInfo.FieldType != typeof(float)) {
            Debug.LogError("Attribute "+AttributeName+" is not a float!!!");
            return false;
        }
        return true;
    }
}
