using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public sealed class WD_State : WD_Node {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    public WD_State             myEntryState    = null;
    public WD_Action            myOnEntryAction = null;
    public WD_Action            myOnUpdateAction= null;
    public WD_Action            myOnExitAction  = null;

    // ======================================================================
    // ACCESSOR
    // ----------------------------------------------------------------------
    public WD_State  ParentState    { get { return Parent as WD_State; } }
    public WD_State  EntryState     { get { return myEntryState; }     set { myEntryState= value; }}
    public WD_Action OnEntryAction  { get { return myOnEntryAction; }  set { myOnEntryAction= value; }}
    public WD_Action OnUpdateAction { get { return myOnUpdateAction; } set { myOnUpdateAction= value; }}
    public WD_Action OnExitAction   { get { return myOnExitAction; }   set { myOnExitAction= value; }}

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
    public WD_State VerifyTransitions() {
        foreach(var obj in this) {
            WD_OutTransitionPort port= obj as WD_OutTransitionPort;
            if(port != null && port.IsReady()) {
                return port.TargetState;
            }
        }
        return null;
    }

    // ======================================================================
    // CHILD MANAGEMENT
    // ----------------------------------------------------------------------
    public override void AddChild(WD_Object _object) {
        _object.Case<WD_State, WD_InTransitionPort, WD_OutTransitionPort, WD_Module>(
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
                    Debug.LogError("Only OnEntry, OnUpdate, and OnExit modules can be added to an WD_State");
                }
            },
            (otherwise)=> {
                Debug.LogError("Invalid child type "+_object.TypeName+" being added to state "+Name);
            }
        );
    }
    public override void RemoveChild(WD_Object _object) {
        _object.Case<WD_State, WD_InTransitionPort, WD_OutTransitionPort, WD_Module>(
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
                    Debug.LogError("Only OnEntry, OnUpdate, and OnExit modules can be removed from an WD_State");
                }
            },
            (otherwise)=> {
                Debug.LogError("Invalid child type "+_object.TypeName+" being removed from state "+Name);
            }
        );
    }
}
