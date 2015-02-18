using UnityEngine;
using System.Collections;
using Subspace;

public class iCS_Transition : SSAction {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    iCS_Package    myTransitionPackage;
    iCS_State      myEndState;
    SSNodeAction   myTriggerFunction;
    int            myTriggerPortIdx;
    bool           myIsTriggered= false;

    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    public iCS_State     EndState    { get { return myEndState; }}
    public bool          DidTrigger  { get { return myIsTriggered; }}
    
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_Transition(string name, SSObject parent, iCS_State endState, iCS_Package transitionPackage, SSNodeAction triggerFunc, int portIdx, int priority)
    : base(name, parent, priority) {
        myTransitionPackage= transitionPackage;
        myEndState         = endState;
        myTriggerPortIdx   = portIdx;
        myTriggerFunction  = triggerFunc;
    }
    
    // ======================================================================
    // Update
    // ----------------------------------------------------------------------
    public override void Evaluate() {
        if(!IsActive) return;
        myIsTriggered= false;
        if(myTransitionPackage != null && myTriggerFunction != null) {
            myTransitionPackage.Evaluate();            
            if(!myTriggerFunction.IsEvaluated) {
                IsStalled= myTransitionPackage.IsStalled;
                return;
            }
            myIsTriggered= (bool)myTriggerFunction[myTriggerPortIdx];
        }
        MarkAsExecuted();
    }
    // ----------------------------------------------------------------------
    public override SSPullBinding GetStalledProducerPort() {
        if(IsEvaluated) return null;
        return myTransitionPackage.GetStalledProducerPort();
    }
    // ----------------------------------------------------------------------
    public override void Execute() {
        myIsTriggered= false;
        if(myTransitionPackage != null && myTriggerFunction != null) {
            myTransitionPackage.Execute();            
            if(!myTransitionPackage.IsEvaluated) {
                IsStalled= myTransitionPackage.IsStalled;
                return;
            }
            myIsTriggered= (bool)myTriggerFunction[myTriggerPortIdx];
        }
        MarkAsExecuted();
    }
}
