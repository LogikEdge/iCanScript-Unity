using UnityEngine;
using System.Collections;

public class iCS_Transition : iCS_Action {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    iCS_Package             myTransitionPackage;
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
    public iCS_Transition(iCS_VisualScriptImp visualScript, iCS_State endState, iCS_Package transitionPackage, iCS_ActionWithSignature triggerFunc, int portIdx, int priority) : base(visualScript, priority) {
        myTransitionPackage= transitionPackage;
        myEndState         = endState;
        myTriggerPortIdx   = portIdx;
        myTriggerFunction  = triggerFunc;
    }
    
    // ======================================================================
    // Update
    // ----------------------------------------------------------------------
    public override void Execute(int frameId) {
        myIsTriggered= false;
        if(myTransitionPackage != null && myTriggerFunction != null) {
            myTransitionPackage.Execute(frameId);            
            if(!myTransitionPackage.IsCurrent(frameId)) {
                IsStalled= myTransitionPackage.IsStalled;
                return;
            }
            myIsTriggered= (bool)myTriggerFunction[myTriggerPortIdx];
        }
        MarkAsExecuted(frameId);
    }
    // ----------------------------------------------------------------------
    public override void ForceExecute(int frameId) {
        myIsTriggered= false;
        if(myTransitionPackage != null && myTriggerFunction != null) {
            myTransitionPackage.ForceExecute(frameId);            
            if(!myTransitionPackage.IsCurrent(frameId)) {
                IsStalled= myTransitionPackage.IsStalled;
                return;
            }
            myIsTriggered= (bool)myTriggerFunction[myTriggerPortIdx];
        }
        MarkAsExecuted(frameId);
    }
}
