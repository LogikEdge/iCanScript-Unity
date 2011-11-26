using UnityEngine;
using System.Collections;

public class UK_Transition : UK_Object {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    UK_FunctionBase myTriggerFunction = null;
    int             myTriggerReturnIdx= -1;
    UK_State        myEndState        = null;
    UK_Action       myTransitionEntryAction= null;
    UK_Action       myTransitionExitAction = null;

    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public UK_Transition(string name, UK_FunctionBase trigger, UK_State endState) : base(name) {
        myTriggerFunction= trigger;
        myTriggerReturnIdx= trigger.OutIndexes[0];
        myEndState= endState;
    }
    
    // ======================================================================
    // Update
    // ----------------------------------------------------------------------
    public UK_State Update(int frameId) {
        if(myTriggerFunction == null) return null;
        do {
            myTriggerFunction.Execute(frameId);            
        } while(!myTriggerFunction.IsCurrent(frameId));
        bool trigger= (bool)myTriggerFunction[myTriggerReturnIdx];
        if(!trigger) return null;
        if(myTransitionExitAction != null) {
            do {
                myTransitionEntryAction.Execute(frameId);                
            } while(!myTransitionEntryAction.IsCurrent(frameId));
        }
        if(myTransitionExitAction != null) {
            do {
                myTransitionExitAction.Execute(frameId);                
            } while(!myTransitionExitAction.IsCurrent(frameId));
        }
        return myEndState;
    }

    // ======================================================================
    // Child Management
    // ----------------------------------------------------------------------
    public void AddChild(UK_Object obj) {
        if(!(obj is UK_Module)) {
            Debug.LogError("Invalid child type "+obj.TypeName+" being added to transition "+Name);
            return;
        }
        UK_Module module= obj as UK_Module;
        if(module.Name == UK_EngineStrings.TransitionEntryModule) {
            myTransitionEntryAction= module;
        }
        else if(module.Name == UK_EngineStrings.TransitionExitModule) {
            myTransitionExitAction= module;
        }
        else {
            Debug.LogError("Only TransitionEntry and TransitionExit can be added to a Transition");
        }
    }
    public void RemoveChild(UK_Object obj) {
        if(!(obj is UK_Module)) {
            Debug.LogError("Invalid child type "+obj.TypeName+" being added to trasnition "+Name);
            return;
        }
        UK_Module module= obj as UK_Module;
        if(module.Name == UK_EngineStrings.TransitionEntryModule) {
            myTransitionEntryAction= null;
        }
        else if(module.Name == UK_EngineStrings.TransitionExitModule) {
            myTransitionExitAction= null;
        }
        else {
            Debug.LogError("Only TransitionEntry and TransitionExit can be added to a Transition");
        }
    }
}
