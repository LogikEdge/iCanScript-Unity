using UnityEngine;
using System.Collections;
using Subspace;

public class iCS_Transition : SSAction {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    iCS_Package             myTransitionPackage;
    iCS_State               myEndState;
    SSActionWithSignature   myTriggerFunction;
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
    public iCS_Transition(int instanceId, string name, iCS_VisualScriptImp visualScript, iCS_State endState, iCS_Package transitionPackage, SSActionWithSignature triggerFunc, int portIdx, int priority)
    : base(instanceId, name, visualScript, priority) {
        myTransitionPackage= transitionPackage;
        myEndState         = endState;
        myTriggerPortIdx   = portIdx;
        myTriggerFunction  = triggerFunc;
    }
    
    // ======================================================================
    // Update
    // ----------------------------------------------------------------------
    public override void Execute(int runId) {
        if(!IsActive) return;
        myIsTriggered= false;
        if(myTransitionPackage != null && myTriggerFunction != null) {
            myTransitionPackage.Execute(runId);            
            if(!myTriggerFunction.IsCurrent(runId)) {
                IsStalled= myTransitionPackage.IsStalled;
                return;
            }
            myIsTriggered= (bool)myTriggerFunction[myTriggerPortIdx];
        }
        MarkAsExecuted(runId);
    }
    // ----------------------------------------------------------------------
    public override Connection GetStalledProducerPort(int runId) {
        if(IsCurrent(runId)) return null;
        return myTransitionPackage.GetStalledProducerPort(runId);
    }
    // ----------------------------------------------------------------------
    public override void ForceExecute(int runId) {
        myIsTriggered= false;
        if(myTransitionPackage != null && myTriggerFunction != null) {
            myTransitionPackage.ForceExecute(runId);            
            if(!myTransitionPackage.IsCurrent(runId)) {
                IsStalled= myTransitionPackage.IsStalled;
                return;
            }
            myIsTriggered= (bool)myTriggerFunction[myTriggerPortIdx];
        }
        MarkAsExecuted(runId);
    }
}
