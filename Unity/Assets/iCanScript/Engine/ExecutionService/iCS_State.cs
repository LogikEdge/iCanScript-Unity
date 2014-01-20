using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public sealed class iCS_State : iCS_Object {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    public iCS_State             myEntryState    = null;
    public iCS_Action            myOnEntryAction = null;
    public iCS_Action            myOnUpdateAction= null;
    public iCS_Action            myOnExitAction  = null;
    public iCS_State             myParentState   = null;
    public List<iCS_State>       myChildren      = new List<iCS_State>();
           iCS_VerifyTransitions myTransitions   = null;
           
    // ======================================================================
    // Accessors
    // ----------------------------------------------------------------------
    public iCS_State             ParentState    { get { return myParentState; } }
    public iCS_State             EntryState     { get { return myEntryState; }     set { myEntryState= value; }}
    public iCS_Action            OnEntryAction  { get { return myOnEntryAction; }  set { myOnEntryAction= value; }}
    public iCS_Action            OnUpdateAction { get { return myOnUpdateAction; } set { myOnUpdateAction= value; }}
    public iCS_Action            OnExitAction   { get { return myOnExitAction; }   set { myOnExitAction= value; }}
    public iCS_VerifyTransitions Transitions    { get { return myTransitions; }}
    
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_State(iCS_VisualScriptImp visualScript, int priority)
    : base(visualScript, priority) {
        myTransitions= new iCS_VerifyTransitions(visualScript, priority);
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
    public void AddChild(iCS_Object _object) {
        Prelude.choice<iCS_State, iCS_Transition, iCS_Package>(_object,
            (state)=> {
                state.myParentState= this;
                myChildren.Add(state);
            },
            (transition)=> {
                myTransitions.AddChild(transition);
            },
            (package)=> {
                if(package.Name == iCS_Strings.OnEntry) {
                    myOnEntryAction= package;
                }
                else if(package.Name == iCS_Strings.OnUpdate) {
                    myOnUpdateAction= package;
                }
                else if(package.Name == iCS_Strings.OnExit) {
                    myOnExitAction= package;
                }
                else {
                    Debug.LogWarning("iCanScript: Only OnEntry, OnUpdate, and OnExit modules can be added to a iCS_State");
                }
            },
            (otherwise)=> {
                Debug.LogWarning("iCanScript: Invalid child type "+_object.TypeName+" being added to state "+Name);
            }
        );
    }
    public void RemoveChild(iCS_Object _object) {
        Prelude.choice<iCS_State, iCS_Transition, iCS_Package>(_object,
            (state)=> {
                if(state == myEntryState) myEntryState= null;
                myChildren.Remove(state);
            },
            (transition)=> {
                myTransitions.RemoveChild(transition);
            },
            (package)=> {
                if(package == myOnEntryAction) {
                    myOnEntryAction= null;
                }
                else if(package == myOnUpdateAction) {
                    myOnUpdateAction= null;
                }
                else if(package == myOnExitAction) {
                    myOnExitAction= null;
                }
                else {
                    Debug.LogWarning("iCanScript: Only OnEntry, OnUpdate, and OnExit modules can be removed from a iCS_State");
                }
            },
            (otherwise)=> {
                Debug.LogWarning("iCanScript: Invalid child type "+_object.TypeName+" being removed from state "+Name);
            }
        );
    }
}
