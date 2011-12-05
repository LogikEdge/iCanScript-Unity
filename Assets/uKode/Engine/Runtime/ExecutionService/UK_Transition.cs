using UnityEngine;
using System.Collections;

public class UK_Transition : UK_Action {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    UK_Action       myGuard;
    UK_Action       myAction;
    UK_State        myEndState;
    UK_FunctionBase myTriggerFunction;
    int             myTriggerPortIdx;
    bool            myIsTriggered= false;

    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    public UK_State     EndState    { get { return myEndState; }}
    public bool         DidTrigger  { get { return myIsTriggered; }}
    public UK_Action    Action      { get { return myAction; }}
    
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public UK_Transition(string name, UK_State endState, UK_Action guard, UK_FunctionBase triggerFunc, int portIdx, UK_Action action, Vector2 layout) : base(name, layout) {
        myGuard          = guard;
        myAction         = action;
        myEndState       = endState;
        myTriggerPortIdx = portIdx;
        myTriggerFunction= triggerFunc;
    }
    
    // ======================================================================
    // Update
    // ----------------------------------------------------------------------
    public override void Execute(int frameId) {
        myIsTriggered= false;
        if(myGuard != null && myTriggerFunction != null) {
            myGuard.Execute(frameId);            
            if(!myGuard.IsCurrent(frameId)) {
                IsStalled= myGuard.IsStalled;
                return;
            }
            myIsTriggered= (bool)myTriggerFunction[myTriggerPortIdx];
        }
        MarkAsCurrent(frameId);
    }
    // ----------------------------------------------------------------------
    public override void ForceExecute(int frameId) {
        myIsTriggered= false;
        if(myGuard != null && myTriggerFunction != null) {
            myGuard.ForceExecute(frameId);            
            if(!myGuard.IsCurrent(frameId)) {
                IsStalled= myGuard.IsStalled;
                return;
            }
            myIsTriggered= (bool)myTriggerFunction[myTriggerPortIdx];
        }
        MarkAsCurrent(frameId);
    }
}
