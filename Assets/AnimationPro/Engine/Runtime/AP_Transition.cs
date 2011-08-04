using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public class AP_Transition : AP_Aggregate {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    public AP_State     myEndState= null;
    public AP_Action    myAction= null;
    public AP_Function  myGuard= null;
    
    // ======================================================================
    // ACCESSORS
    // ----------------------------------------------------------------------
    public AP_State EndState { get { return myEndState; } }
    
    // ======================================================================
    // INITIALIZATION
    // ----------------------------------------------------------------------
    public static AP_Transition CreateInstance(string name, AP_State parent) {
        AP_Transition instance= CreateInstance<AP_Transition>();
        instance.Init(name, parent);
        return instance;
    }
    
    // ======================================================================
    // EXECUTION
    // ----------------------------------------------------------------------
    public bool Execute() {
        if(!IsConditionPresent()) return false;
        if(myAction) myAction.Execute();
        return true;
    }
    bool IsConditionPresent() {
        if(!myGuard) return false;
        myGuard.Execute();
        FieldInfo guardField= (myGuard.GetOutputFields())[0];
        return (bool)(guardField.GetValue(myGuard));
    }

    // ======================================================================
    // CHILD MANAGEMENT
    // ----------------------------------------------------------------------
    public AP_Function Guard {
        get { return myGuard; }
        set {
            List<FieldInfo> outFields= value.GetOutputFields();
            if(outFields.Count != 1) {
                myGuard= null;
                throw new System.FormatException("Guard expression has more then one output.");
            }
            if(outFields[0].FieldType != typeof(bool)) {
                myGuard= null;
                throw new System.FormatException("Guard expression does not return a boolean value.");
            }
            myGuard= value;
        }
    }
    
}
