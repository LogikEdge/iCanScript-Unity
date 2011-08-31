using UnityEngine;
using System.Collections;
using System.Reflection;

public class AP_StateLeavePort : AP_Port {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    public AP_StateEntryPort    myTargetPort= null;
    public AP_Function          myGuard= null;
    public AP_Action            myAction= null;
    
    // ======================================================================
    // ACCESSORS
    // ----------------------------------------------------------------------
    public AP_State TargetState { get { return myTargetPort.Parent as AP_State; } }

    // ======================================================================
    // INITIALIZATION
    // ----------------------------------------------------------------------
    public static AP_StateLeavePort CreateInstance(string name, AP_State parent) {
        AP_StateLeavePort instance= CreateInstance<AP_StateLeavePort>();
        instance.Init(name, parent);
        return instance;
    }
    
    // ======================================================================
    // EXECUTION
    // ----------------------------------------------------------------------
    public override bool IsReady() {
        if(!IsConditionPresent()) return false;
        if(myAction) myAction.Execute();
        return true;        
    }
    public override AP_Port GetConnectedPort() {
        return myTargetPort;
    }
    bool IsConditionPresent() {
        if(!myGuard) return false;
        myGuard.Execute();
        FieldInfo guardField= (myGuard.GetOutputFields())[0];
        return (bool)(guardField.GetValue(myGuard));
    }
    
}
