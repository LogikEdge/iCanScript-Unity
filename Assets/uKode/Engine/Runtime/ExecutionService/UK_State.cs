using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public sealed class UK_State : UK_Object {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    public UK_State             myEntryState    = null;
    public UK_Action            myOnEntryAction = null;
    public UK_Action            myOnUpdateAction= null;
    public UK_Action            myOnExitAction  = null;
    public UK_State             myParentState   = null;
    public List<UK_State>       myChildren      = new List<UK_State>();
           UK_VerifyTransitions myTransitions   = null;
           
    // ======================================================================
    // Accessors
    // ----------------------------------------------------------------------
    public UK_State             ParentState    { get { return myParentState; } }
    public UK_State             EntryState     { get { return myEntryState; }     set { myEntryState= value; }}
    public UK_Action            OnEntryAction  { get { return myOnEntryAction; }  set { myOnEntryAction= value; }}
    public UK_Action            OnUpdateAction { get { return myOnUpdateAction; } set { myOnUpdateAction= value; }}
    public UK_Action            OnExitAction   { get { return myOnExitAction; }   set { myOnExitAction= value; }}
    public UK_VerifyTransitions Transitions    { get { return myTransitions; }}
    
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public UK_State(string name) : base(name) {
        myTransitions= new UK_VerifyTransitions(name);
    }
    
    // ======================================================================
    // Update
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

    // ======================================================================
    // Child Management
    // ----------------------------------------------------------------------
    public void AddChild(UK_Object _object) {
        Prelude.choice<UK_State, UK_Transition, UK_Module>(_object,
            (state)=> {
                state.myParentState= this;
                myChildren.Add(state);
            },
            (transition)=> {
                myTransitions.AddChild(transition);
            },
            (module)=> {
                if(module.Name == UK_EngineStrings.OnEntryModule) {
                    myOnEntryAction= module;
                }
                else if(module.Name == UK_EngineStrings.OnUpdateModule) {
                    myOnUpdateAction= module;
                }
                else if(module.Name == UK_EngineStrings.OnExitModule) {
                    myOnExitAction= module;
                }
                else {
                    Debug.LogError("Only OnEntry, OnUpdate, and OnExit modules can be added to a UK_State");
                }
            },
            (otherwise)=> {
                Debug.LogError("Invalid child type "+_object.TypeName+" being added to state "+Name);
            }
        );
    }
    public void RemoveChild(UK_Object _object) {
        Prelude.choice<UK_State, UK_Transition, UK_Module>(_object,
            (state)=> {
                if(state == myEntryState) myEntryState= null;
                myChildren.Remove(state);
            },
            (transition)=> {
                myTransitions.RemoveChild(transition);
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
                    Debug.LogError("Only OnEntry, OnUpdate, and OnExit modules can be removed from a UK_State");
                }
            },
            (otherwise)=> {
                Debug.LogError("Invalid child type "+_object.TypeName+" being removed from state "+Name);
            }
        );
    }
}
