using UnityEngine;
using System.Collections;

public class UK_Transition : UK_Action {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    UK_FunctionBase myGuard;
    int             myGuardIdx;
    UK_Action       myAction;
    UK_State        myEndState;
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
    public UK_Transition(string name, UK_State endState, UK_FunctionBase guard, UK_FunctionBase action= null) : base(name) {
        myEndState= endState;
        myGuard   = guard;
        myGuardIdx= guard.OutIndexes[0];
        myAction  = action;
    }
    
    // ======================================================================
    // Update
    // ----------------------------------------------------------------------
    public override void Execute(int frameId) {
        myIsTriggered= false;
        if(myGuard != null) {
            myGuard.Execute(frameId);            
            if(!myGuard.IsCurrent(frameId)) return;
            myIsTriggered= (bool)myGuard[myGuardIdx];
        }
        MarkAsCurrent(frameId);
    }
    // ----------------------------------------------------------------------
    public override void ForceExecute(int frameId) {
        myIsTriggered= false;
        if(myGuard != null) {
            myGuard.ForceExecute(frameId);            
            if(!myGuard.IsCurrent(frameId)) return;
            myIsTriggered= (bool)myGuard[myGuardIdx];
        }
        MarkAsCurrent(frameId);
    }
}
