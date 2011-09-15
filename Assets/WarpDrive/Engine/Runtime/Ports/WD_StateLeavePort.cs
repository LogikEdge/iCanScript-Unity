using UnityEngine;
using System.Collections;
using System.Reflection;

public class WD_StateLeavePort : WD_Port {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    public WD_StateEntryPort    myTargetPort= null;
    public WD_Function          myGuard= null;
    public WD_Action            myAction= null;
    
    // ======================================================================
    // ACCESSORS
    // ----------------------------------------------------------------------
    public WD_State TargetState { get { return myTargetPort.Parent as WD_State; } }

    // ======================================================================
    // INITIALIZATION
    // ----------------------------------------------------------------------
    public static WD_StateLeavePort CreateInstance(string name, WD_State parent) {
        WD_StateLeavePort instance= CreateInstance<WD_StateLeavePort>();
        instance.Init(name, parent);
        return instance;
    }
    
    // ======================================================================
    // EXECUTION
    // ----------------------------------------------------------------------
    public override bool IsReady() {
        if(!IsConditionPresent()) return false;
        if(myAction != null) myAction.Execute();
        return true;        
    }
    public override WD_Port GetConnectedPort() {
        return myTargetPort;
    }
    bool IsConditionPresent() {
        if(myGuard == null) return false;
        myGuard.Execute();
        FieldInfo guardField= (myGuard.GetOutputFields())[0];
        return (bool)(guardField.GetValue(myGuard));
    }
    
}
