using UnityEngine;
using System.Collections;

public class WD_Transition : WD_Object {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    WD_FunctionBase myTriggerFunction = null;
    int             myTriggerReturnIdx= -1;
    WD_State        myEndState        = null;
    WD_Action       myTransitionEntryAction= null;
    WD_Action       myTransitionExitAction = null;

    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public WD_Transition(string name, WD_FunctionBase trigger, WD_State endState) : base(name) {
        myTriggerFunction= trigger;
        myTriggerReturnIdx= trigger.OutIndexes[0];
        myEndState= endState;
    }
    
    // ======================================================================
    // Update
    // ----------------------------------------------------------------------
    public WD_State Update(int frameId) {
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
    public void AddChild(WD_Object obj) {
        if(!(obj is WD_Module)) {
            Debug.LogError("Invalid child type "+obj.TypeName+" being added to transition "+Name);
            return;
        }
        WD_Module module= obj as WD_Module;
        if(module.Name == WD_EngineStrings.TransitionEntryModule) {
            myTransitionEntryAction= module;
        }
        else if(module.Name == WD_EngineStrings.TransitionExitModule) {
            myTransitionExitAction= module;
        }
        else {
            Debug.LogError("Only TransitionEntry and TransitionExit can be added to a Transition");
        }
    }
    public void RemoveChild(WD_Object obj) {
        if(!(obj is WD_Module)) {
            Debug.LogError("Invalid child type "+obj.TypeName+" being added to trasnition "+Name);
            return;
        }
        WD_Module module= obj as WD_Module;
        if(module.Name == WD_EngineStrings.TransitionEntryModule) {
            myTransitionEntryAction= null;
        }
        else if(module.Name == WD_EngineStrings.TransitionExitModule) {
            myTransitionExitAction= null;
        }
        else {
            Debug.LogError("Only TransitionEntry and TransitionExit can be added to a Transition");
        }
    }
}
