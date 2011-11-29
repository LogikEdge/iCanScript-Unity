using UnityEngine;
using System.Collections;

public class UK_Transition : UK_Object {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    UK_FunctionBase myGuard   = null;
    int             myGuardIdx= -1;
    UK_Action       myAction  = null;
    UK_State        myEndState= null;

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
    public UK_State Update(int frameId) {
        if(myGuard == null) return null;
        do {
            myGuard.Execute(frameId);            
        } while(!myGuard.IsCurrent(frameId));
        bool transitionFired= (bool)myGuard[myGuardIdx];
        if(!transitionFired) return null;
        if(myAction != null) {
            do {
                myAction.Execute(frameId);                
            } while(!myAction.IsCurrent(frameId));
        }
        return myEndState;
    }
}
