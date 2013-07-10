using UnityEngine;
using System.Collections;

public class iCS_Transition : iCS_Action {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    iCS_Action              myGuard;
    iCS_Action              myAction;
    iCS_State               myEndState;
    iCS_ActionWithSignature myTriggerFunction;
    int                     myTriggerPortIdx;
    bool                    myIsTriggered= false;

    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    public iCS_State     EndState    { get { return myEndState; }}
    public bool         DidTrigger  { get { return myIsTriggered; }}
    public iCS_Action    Action      { get { return myAction; }}
    
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_Transition(string name, iCS_State endState, iCS_Action guard, iCS_ActionWithSignature triggerFunc, int portIdx, iCS_Action action, int priority) : base(name, priority) {
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
