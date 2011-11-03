using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public sealed class WD_State : WD_Object {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    public WD_State             myEntryState    = null;
    public WD_Action            myOnEntryAction = null;
    public WD_Action            myOnUpdateAction= null;
    public WD_Action            myOnExitAction  = null;
    public WD_State             myParentState   = null;
    public List<WD_State>       myChildren      = new List<WD_State>();
    public List<WD_Transition>  myTransitions   = new List<WD_Transition>();
    
    // ======================================================================
    // ACCESSOR
    // ----------------------------------------------------------------------
    public WD_State  ParentState    { get { return myParentState; } }
    public WD_State  EntryState     { get { return myEntryState; }     set { myEntryState= value; }}
    public WD_Action OnEntryAction  { get { return myOnEntryAction; }  set { myOnEntryAction= value; }}
    public WD_Action OnUpdateAction { get { return myOnUpdateAction; } set { myOnUpdateAction= value; }}
    public WD_Action OnExitAction   { get { return myOnExitAction; }   set { myOnExitAction= value; }}

    // ======================================================================
    // UPDATE
    // ----------------------------------------------------------------------
    public void OnEntry(int frameId) {
        if(myOnEntryAction != null) {
            myOnEntryAction.Execute(frameId);
        }
    }
    public void OnUpdate(int frameId) {
        if(myOnUpdateAction != null) {
            myOnUpdateAction.Execute(frameId);
        }
    }
    public void OnExit(int frameId) {
        if(myOnExitAction != null) {
            myOnExitAction.Execute(frameId);
        }
    }
    public WD_State VerifyTransitions(int frameId) {
        foreach(var transition in myTransitions) {
        }
        return null;
    }

    // ======================================================================
    // CHILD MANAGEMENT
    // ----------------------------------------------------------------------
    public void AddChild(WD_Object _object) {
        Prelude.choice<WD_State, WD_Transition, WD_Module>(_object,
            (state)=> {
                state.myParentState= this;
                myChildren.Add(state);
            },
            (transition)=> {
                myTransitions.Add(transition);
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
    public void RemoveChild(WD_Object _object) {
        Prelude.choice<WD_State, WD_Transition, WD_Module>(_object,
            (state)=> {
                if(state == myEntryState) myEntryState= null;
                myChildren.Remove(state);
            },
            (transition)=> {
                myTransitions.Remove(transition);
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
