using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AP_State : AP_Node {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    public AP_State             myEntryState    = null;
    public AP_Action            myOnEntryAction = null;
    public AP_Action            myOnUpdateAction= null;
    public AP_Action            myOnExitAction  = null;

    // ======================================================================
    // ACCESSOR
    // ----------------------------------------------------------------------
    public AP_State  ParentState    { get { return Parent as AP_State; } }
    public AP_State  EntryState     { get { return myEntryState; }     set { myEntryState= value; }}
    public AP_Action OnEntryAction  { get { return myOnEntryAction; }  set { myOnEntryAction= value; }}
    public AP_Action OnUpdateAction { get { return myOnUpdateAction; } set { myOnUpdateAction= value; }}
    public AP_Action OnExitAction   { get { return myOnExitAction; }   set { myOnExitAction= value; }}

    // ======================================================================
    // UPDATE
    // ----------------------------------------------------------------------
    public void OnEntry() {
        if(myOnEntryAction != null) {
            myOnEntryAction.Execute();
        }
    }
    public void OnUpdate() {
        if(myOnUpdateAction != null) {
            myOnUpdateAction.Execute();
        }
    }
    public void OnExit() {
        if(myOnExitAction != null) {
            myOnExitAction.Execute();
        }
    }
    public AP_State VerifyTransitions() {
        foreach(var obj in this) {
            AP_StateLeavePort port= obj as AP_StateLeavePort;
            if(port != null && port.IsReady()) {
                return port.TargetState;
            }
        }
        return null;
    }

    // ======================================================================
    // CHILD MANAGEMENT
    // ----------------------------------------------------------------------
    public override void AddChild(AP_Object _object) {
        _object.Case<AP_State, AP_StateEntryPort, AP_StateLeavePort, AP_Module>(
            (state)=> {
                base.AddChild(_object);
            },
            (entryPort)=> {
                base.AddChild(_object);
            },
            (leavePort)=> {
                base.AddChild(_object);
            },
            (module)=> {
                if(module.Name == "OnEntry") {
                    myOnEntryAction= module;
                    base.AddChild(_object);
                }
                else if(module.Name == "OnUpdate") {
                    myOnUpdateAction= module;
                    base.AddChild(_object);
                }
                else if(module.Name == "OnExit") {
                    myOnExitAction= module;
                    base.AddChild(_object);
                }
                else {
                    Debug.LogError("Only OnEntry, OnUpdate, and OnExit modules can be added to an AP_State");
                }
            },
            (otherwise)=> {
                Debug.LogError("Invalid child type "+_object.TypeName+" being added to state "+Name);
            }
        );
    }
    public override void RemoveChild(AP_Object _object) {
        _object.Case<AP_State, AP_StateEntryPort, AP_StateLeavePort, AP_Module>(
            (state)=> {
                if(state == myEntryState) myEntryState= null;
                base.RemoveChild(_object);
            },
            (entryPort)=> {
                base.RemoveChild(_object);
            },
            (leavePort)=> {
                base.RemoveChild(_object);
            },
            (module)=> {
                if(module == myOnEntryAction) {
                    myOnEntryAction= null;
                    base.RemoveChild(_object);
                }
                else if(module == myOnUpdateAction) {
                    myOnUpdateAction= null;
                base.RemoveChild(_object);
                }
                else if(module == myOnExitAction) {
                    myOnExitAction= null;
                    base.RemoveChild(_object);
                }
                else {
                    Debug.LogError("Only OnEntry, OnUpdate, and OnExit modules can be removed from an AP_State");
                }
            },
            (otherwise)=> {
                Debug.LogError("Invalid child type "+_object.TypeName+" being removed from state "+Name);
            }
        );
    }
}
