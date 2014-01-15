using UnityEngine;
using System.Collections;

public class iCS_Transition : iCS_Action {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    iCS_Action              myGuard;
    iCS_State               myEndState;
    iCS_ActionWithSignature myTriggerFunction;
    int                     myTriggerPortIdx;
    bool                    myIsTriggered= false;

    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    public iCS_State     EndState    { get { return myEndState; }}
    public bool          DidTrigger  { get { return myIsTriggered; }}
    
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_Transition(iCS_VisualScriptImp visualScript, iCS_State endState, iCS_Action guard, iCS_ActionWithSignature triggerFunc, int portIdx, int priority) : base(visualScript, priority) {
        myGuard          = guard;
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
        MarkAsExecuted(frameId);
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
        MarkAsExecuted(frameId);
    }
}
