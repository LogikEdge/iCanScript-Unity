using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public sealed class WD_State {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    public WD_State     myEntryState    = null;
    public WD_Action    myOnEntryAction = null;
    public WD_Action    myOnUpdateAction= null;
    public WD_Action    myOnExitAction  = null;

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
    public void AddChild(Object _object) {
        Prelude.choice<WD_State, WD_TransitionEntry, WD_TransitionExit, WD_Module>(_object,
            (state)=> {
            },
            (transitionEntry)=> {
            },
            (transitionExit)=> {
            },
            (module)=> {
                if(module.Name == WD_EngineStrings.OnEntryModule) {
                    myOnEntryAction= module;
                }
                else if(module.Name == WD_EngineStrings.OnUpdateModule) {
                    myOnUpdateAction= module;
                }
                else if(module.Name == WD_EngineStrings.OnExitModule) {
                    myOnExitAction= module;
                }
                else {
                    Debug.LogError("Only OnEntry, OnUpdate, and OnExit modules can be added to a WD_State");
                }
            },
            (otherwise)=> {
                Debug.LogError("Invalid child type "+_object.TypeName+" being added to state "+Name);
            }
        );
    }
    public void RemoveChild(Object _object) {
        Prelude.choice<WD_State, WD_TransitionEntry, WD_TransitionExit, WD_Module>(_object,
            (state)=> {
                if(state == myEntryState) myEntryState= null;
            },
            (transitionEntry)=> {
            },
            (transitionExit)=> {
            },
            (module)=> {
                if(module == myOnEntryAction) {
                    myOnEntryAction= null;
                }
                else if(module == myOnUpdateAction) {
                    myOnUpdateAction= null;
                }
                else if(module == myOnExitAction) {
                    myOnExitAction= null;
                }
                else {
                    Debug.LogError("Only OnEntry, OnUpdate, and OnExit modules can be removed from a WD_State");
                }
            },
            (otherwise)=> {
                Debug.LogError("Invalid child type "+_object.TypeName+" being removed from state "+Name);
            }
        );
    }
}
